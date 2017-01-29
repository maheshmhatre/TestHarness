/////////////////////////////////////////////////////////////////////////////
//  Client.cs - create new app domain to execute tests                 //
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


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using XMLbuilder;

namespace CommTest
{
   public class Client
    {
        Sender sndrRepository;
        Sender sndrTestHarness;
        Receiver recvr;
        //Thread rcvThrd = null;
        Message rcvdMsg;

        public string TestHarnessAddress { get; set; }
        public string RepositoryAddress { get; set; }
        public string ClientAddress { get; set; }

        public Client()
        {

        }

        // constructor
        public void connect(string repositoryAddr,string testHarnessAddr)
        {
            TestHarnessAddress = testHarnessAddr;
            RepositoryAddress = repositoryAddr;

            string endpoint = "http://localhost:" + repositoryAddr + "/IService";
            sndrRepository = new Sender(endpoint);

            endpoint = "http://localhost:" + testHarnessAddr + "/IService";
            sndrTestHarness = new Sender(endpoint);

            rcvdMsg = new Message();
        }
       
        // query log files from repository
       public void getLogs(string key)
        {
            key = "Log" + key;
            Message msg = new Message();
            msg.type = "getlogs";
            msg.from = ClientAddress;
            msg.to = RepositoryAddress;
            msg.body = key;
            sndrRepository.PostMessage(msg);
        }
       
        // make xml test request string
       public string createTestRequest(Test t)
        {
            Builder builder = new Builder();

            builder.createRoot("TestRequest");

            // Add author to root
            XElement author = builder.makeElement("Author");
            XAttribute attribute = new XAttribute("name", t.author);
            builder.addAttribute(ref author, ref attribute);
            builder.addToRoot(author);

            // First test
            XElement test = builder.makeElement("Test");
            // test id
            XAttribute testId = new XAttribute("id", t.testId);
            builder.addAttribute(ref test, ref testId);
            // name
            XElement testName = new XElement("TestName",t.testName);
            builder.addTo(ref test, ref testName);
            // driver
            XElement testDriver1 = new XElement("TestDriver", t.testDriver);//builder.makeElement("TestDriver");
            builder.addTo(ref test, ref testDriver1);
            // code 
            foreach (string code in t.testCode)
            {
                XElement testCode1 = new XElement("TestCode",code);
                builder.addTo(ref test, ref testCode1);
            }

            builder.addToRoot(test);
            string serializedString = builder.toString();
            return serializedString;
        }

        // thread to listen on a port
        public Message getMessage()
        {
                // get message out of receive queue - will block if queue is empty
               return recvr.getMessage();
        }


        // process messages
        private void process(Message message)
        {
            if (message.type == "filesreply")
            {
                Console.WriteLine(message.body);
            }
            else if (message.type == "testresult")
            {
                Console.WriteLine("\n Result from Test Harness received");
                XDocument docRes = XDocument.Parse(message.body);
                XElement tNElem = docRes.Descendants("TestName").First();
                string testName = tNElem.Value;
               
                XElement reslement = docRes.Descendants("TestResult").First();
                string res = reslement.Value;
                Console.WriteLine(" Test {0} , Result {1}",testName,res);
            }
        }

        // start sender and thread
        public void listen(string localPort)
        {
            string endpoint = "http://localhost:" + localPort + "/IService";
            ClientAddress = localPort;

            try
            {
                recvr = new Receiver();
                recvr.CreateRecvChannel(endpoint);

                // create receive thread which calls rcvBlockingQ.deQ() (see ThreadProc above)
                //rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
                //rcvThrd.IsBackground = true;
                //rcvThrd.Start();
            }
            catch (Exception ex)
            {
                StringBuilder msg = new StringBuilder(ex.Message);
                Console.WriteLine(msg);
            }
        }

        // send message
        public void postMessage(Message msg)
        {
            if (msg.to == RepositoryAddress)
                sndrRepository.PostMessage(msg);
            else if (msg.to == TestHarnessAddress)
                sndrTestHarness.PostMessage(msg);
        }

        // send file to destination
        public void uploadFile(string filename)
        {
            sndrRepository.uploadFile(filename);
        }

        // get a file from a source
        public void downloadFile(string filename)
        {
            sndrRepository.download(filename);
        }

        // Get names of files from repository
        public void getFileListRepository()
        {
            Message msg = new Message();
            msg.to = "8080";
            msg.from = "8085";
            msg.type = "getfiles";

            sndrRepository.PostMessage(msg);
        }

        static void Main(string[] args)
        {
            Thread.Sleep(500);
            Client client = new Client();
            client.listen("8085");
            client.connect("8080", "8090");

         
            Message msg = new Message();
            msg.type = "getfiles";
            msg.to = client.RepositoryAddress;
            msg.from = client.ClientAddress;
            msg.body = "empty";

            Console.WriteLine(" Showing the filename fetching operation from repository");

            client.postMessage(msg);
            

            Console.WriteLine(" Constructing test request and sending it to test harness server");
            Message testRequestMessage = new Message();
            Test test1 = new Test();
            test1.author = " Mahesh Mhatre";
            test1.testId = "1";
            test1.testName = "Test1";
            test1.testDriver = "TestDriver1.dll";
            List<string> codeFiles = new List<string>();
            codeFiles.Add("CodeToTest1.dll");
            //codeFiles.Add("tc2.dll");
            test1.testCode = codeFiles;
            testRequestMessage.body = client.createTestRequest(test1);
            testRequestMessage.type = "testRequest";
            testRequestMessage.to = "8090";
            testRequestMessage.from = "8085";
            client.postMessage(testRequestMessage);

            client.getLogs("test1");
            //client.uploadFile("test.txt");
            //client.downloadFile("nystateofhealth.pdf");

            Console.ReadKey();
        }
    }
}
