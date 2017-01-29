/////////////////////////////////////////////////////////////////////////////
//  Iservice.cs - Service contract for communication                       //
//  ver 1.0                                                                //
//  Language:     C#, VS 2015                                              //
//  Platform:     Lenovo Thinkpad E450, Windows 10 Ultimate                //
//  Application:  Test Harness                                             //
//  Author:       Mahesh Mhatre, Syracuse University                       //
//                (315) 412-8489, mrmhatre@syr.edu                         //
/////////////////////////////////////////////////////////////////////////////
/* 
 *   Module Operations
 *   -----------------
 *   This module provides provides client to send the test requests, get log results and test results.
 *   Client can also send files to repository and get log files from repository
 * 
 *   Public Interface
 *   ----------------
 *   postMessage(Message msg)                         sends test request messages to server
 *   Message getMessages()                            gets back the log, results
 *   uploadFile(filename)                             uploads the code files to repository
 *   stream downloadFile()                            gets back filestream from repository
 *   listen(port)                                      receives messages on a port
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   client.cs
 *   - Compiler command: devenv CodeToTest.sln /rebuild debug
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 05 October 2016
 *     - first release
 * 
 */

using CommTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Host
{

    class Server
    {
        Receiver recvr;
        Sender sndr;
        Thread rcvThrd = null;
        Message rcvdMsg;
        public string port { get; set; }
        // initialize the sender by giving port
        public void initSender(string port)
        {
            string endpoint = "http://localhost:" + port + "/IService";
            sndr = new Sender(endpoint);
        }

        // send message
        public void postMessage(Message msg)
        {
            sndr.PostMessage(msg);
        }

        // constructor
        public Server()
        {
            rcvdMsg = new Message();
        }

        // receive messages in a loop
        void ThreadProc()
        {
            while (true)
            {
                // get message out of receive queue - will block if queue is empty
                rcvdMsg = recvr.getMessage();

                Console.WriteLine(" Message on repository {0} , {1}", rcvdMsg.type,rcvdMsg.body);
                process(rcvdMsg);
            }
        }

        // Find files in a directory by extension
        public string[] FindFilesInDirectory(string path, string searchPattern)
        {
            string[] files = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories);
            return files;
        }

        // save log files
        public bool saveLogFile(Message msg)
        {
            // extract test name from msg body
            XDocument xdoc = XDocument.Parse(msg.body);
            if (xdoc == null)
                return false;
            XElement tNElement = xdoc.Descendants("TestName").First();
            string testName = tNElement.Value;
            string path = @"..\\..\\..\\Host\\Repository\\Log" + testName + "MaheshMhatre" +DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(msg.body);
            }
            return true;
        }

        // save result file
        public bool saveResultFile(Message msg)
        {
            // extract test name from msg body
            XDocument xdoc = XDocument.Parse(msg.body);
            if (xdoc == null)
                return false;
            XElement tNElement = xdoc.Descendants("TestName").First();
            string testName = tNElement.Value;
            string path = @"..\\..\\..\\Host\\Repository\\Result" + testName + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path, true))
            {
                XElement reslement = xdoc.Descendants("TestResult").First();
                string res = reslement.Value;
                file.WriteLine(res);
            }
            return true;
        }

        // process received messages
        public void process(Message msg)
        {
            if (msg.type == "getfiles")
            {
                sendFiles(msg);
            }
            else if (msg.type == "getlogs")
            {
                sendLogFiles(msg);
            }
            else if (msg.type == "testresult")
            {
                saveResultFile(msg);
            }
            else if (msg.type == "logmessage")
            {
                // save log file
                saveLogFile(msg);
            }
        }

        // send log files to client
        void sendLogFiles(Message msg)
        {
            string endpoint = "http://localhost:" + msg.from + "/IService";
            Sender sClient = new Sender(endpoint);

            string[] files = FindFilesInDirectory("..\\..\\..\\Host\\Repository", "*"+msg.body+"*");
            foreach (string fname in files)
            {
                Message msgReply = new Message();
                msgReply.from = port;
                msgReply.to = msg.from;
                msgReply.type = "logreply";
                //string filename = Path.GetFileName(fname);
                XDocument doc_ = XDocument.Load(fname);
                msgReply.body = doc_.ToString();
                sClient.PostMessage(msgReply);
            }
        }

        // send code files to client
        private void sendFiles(Message msgRcvd)
        {
            Message msg = new Message();
            msg.to = msgRcvd.from;
            msg.from = port;
            msg.type = "filesreply";
           
            string[] files = FindFilesInDirectory("..\\..\\..\\Host\\Repository", "*.dll");

            string endpoint = "http://localhost:" + msg.to + "/IService";
            Sender sClient = new Sender(endpoint);
            
            for(int i=0;i<files.Length;i++)
            {
                msg.body += files[i] + "\n";
            }
            
            sClient.PostMessage(msg);
        }

        // listen to receive messages
        public void listen(string localPort)
        {
            string endpoint = "http://localhost:" + localPort + "/IService";
            try
            {
                recvr = new Receiver();
                recvr.CreateRecvChannel(endpoint);

                // create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc above)
                rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                rcvThrd.IsBackground = true;
                rcvThrd.Start();
            }
            catch (Exception ex)
            {
                StringBuilder msg = new StringBuilder(ex.Message);
                Console.WriteLine(msg);
            }
        }

        // upload file
        public void uploadFile(string filename)
        {
            sndr.uploadFile(filename);
        }

        // download file
        public void downloadFile(string filename)
        {
            sndr.download(filename);
        }

        static void Main(string[] args)
        {
            Server server = new Server();
            Console.WriteLine(" Starting repository server on port 8080");
            server.port = "8080";
            server.listen(server.port);            
            Console.ReadKey();
        }
    }
}



