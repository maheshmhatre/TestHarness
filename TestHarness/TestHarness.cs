/////////////////////////////////////////////////////////////////////////////
//  TestHarness.cs - Module to perform tests and send messages             //
//                   and logs to  clients                                  //
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
 *   This module provides provides clients to send the test requests, get log results and test results.
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
 *   - Required files:   TestHarness.cs
 *   - Compiler command: devenv TestHarness.sln /rebuild debug
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppDomainManager;
using SWTools;
using ResultMessage;
using XMLbuilder;

namespace TestHarness
{
     public class TestHarness :MarshalByRefObject
    {
        Receiver recvr;
        Thread rcvThrd = null;
        Message rcvdMsg;
        List<Test> testList_;
        // Sender 
        Thread sndThrd = null;
        string repositoryPath = "..\\..\\..\\TestHarness\\FileCache\\";
        static BlockingQueue<Result> resultTests;
        public string repositoryAddress { get; set; }
        public string testHarnessAddress { get; set; }
        // constructor
        public TestHarness()
        {
            testList_ = new List<Test>();
            recvr = new Receiver();
            resultTests = new BlockingQueue<Result>();

            sndThrd = new Thread(threadResult);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

        // check the message and reply back
        public void sendResult(Result r)
        {
            if (r.type == "result")
            {
                string endPoint = "http://localhost:" + r.to + "/IService";
                Sender sn = new Sender(endPoint);
                Message msg = new Message();
                msg.type = "testresult";
                msg.to = r.to;
                msg.from = testHarnessAddress;
                // serialize result and make xml string to be sent to client
                msg.body = createXMLReply(r);
                sn.PostMessage(msg);

                endPoint = "http://localhost:" + repositoryAddress + "/IService";
                Sender st = new Sender(endPoint);
                Message msg1 = new Message();
                msg1.type = "testresult";
                msg1.to = repositoryAddress;
                msg1.from = testHarnessAddress;
                msg1.body = createXMLReply(r);
                st.PostMessage(msg1);
            }
            else if (r.type == "log")
            {
                string endPoint = "http://localhost:" + repositoryAddress + "/IService";
                Sender sn = new Sender(endPoint);
                Message msg = new Message();
                msg.type = "logmessage";
                msg.to = repositoryAddress;
                msg.from = testHarnessAddress;
                msg.body = createXMLLog(r);
                sn.PostMessage(msg);
            }
        }

        // make msg body
        string createXMLReply(Result r)
        {
            Builder builder = new Builder();
            builder.createRoot("TestResult");
            XElement testName = new XElement("TestName",r.testName);
            builder.addToRoot(testName);
            XElement result = new XElement("Result", r.resultString);
            builder.addToRoot(result);
            return builder.toString();
        }

        // make log message body
        string createXMLLog(Result r)
        {
            Builder builder = new Builder();
            builder.createRoot("LogMessage");
            XElement testName = new XElement("TestName", r.testName);
            builder.addToRoot(testName);
            XElement result = new XElement("Log", r.resultString);
            builder.addToRoot(result);
            return builder.toString();
        }

        // send result
        void threadResult()
        {
            while (true)
            {
                Result res = resultTests.deQ();
                sendResult(res);
            }
        }

        // parse messsage
        public bool parse(string s)
        {
            testList_.Clear();
            XDocument doc_ = XDocument.Parse(s);
            if (doc_ == null)
                return false;
            XElement auth = doc_.Descendants("Author").First();
            string author = auth.Attribute("name").ToString();

            //Test test = null;
            XElement[] xtests = doc_.Descendants("Test").ToArray();
            int numTests = xtests.Count();

            for (int i = 0; i < numTests; ++i)
            {
                Test test = new Test();
                test.testCode = new List<string>();
                test.author = author;
                test.testId = xtests[i].Attribute("id").Value;
                test.timeStamp = DateTime.Now;
                test.testName = xtests[i].Element("TestName").Value;
                test.testDriver = xtests[i].Element("TestDriver").Value;
                IEnumerable<XElement> xtestCode = xtests[i].Elements("TestCode");

                foreach (var xlibrary in xtestCode)
                {
                    test.testCode.Add(xlibrary.Value);
                }

                testList_.Add(test);
            }

            return true;
        }

        // Receive messsages
        void ThreadProc()
        {
            while (true)
            {
                // get message out of receive queue - will block if queue is empty
                rcvdMsg = recvr.getMessage();
                //Console.WriteLine(" Message on server {0} , {1}", rcvdMsg.type, rcvdMsg.body);
                process(rcvdMsg);
            }
        }

        // process messages
        private void process(Message rcvdMsg)
        {
            if (rcvdMsg.type == "testRequest")
            {
                processTestRequest(rcvdMsg);
                Console.WriteLine("\n\n Req #2 Received Test Request on Test Harness server ");
                Console.WriteLine(" Message type: {0}", rcvdMsg.type);
                Console.WriteLine(" Message to:   {0}", rcvdMsg.to);
                Console.WriteLine(" Message from: {0}", rcvdMsg.from);
                Console.WriteLine(" Message body: {0}", rcvdMsg.body);
            }
        }

        // processing test requests
        private void processTestRequest(Message msg)
        {
            if (parse(msg.body))
            {
                foreach (Test test in testList_)
                {
                    // get files from repository
                    if (getFiles(test))
                    {
                        DomainCreator dc = new DomainCreator(ref resultTests, test.testName,msg.from);
                        List<string> filestore = new List<string>();
                        filestore.Add(repositoryPath + test.testDriver);
                        foreach (var code in test.testCode)
                            filestore.Add(repositoryPath + code);
                        
                        Task.Run(() => dc.makeChildDomain(filestore));
                    }
                    else
                    {
                        Result r = new Result();
                        r.testName = test.testName;
                        r.to = msg.from;
                        r.type = "result";
                        r.resultString = "Test failed \nFile not found in repository";
                        resultTests.enQ(r);
                        break;
                    }
                    
                }
            }
        }

        // get files for testing on test harness
        public bool getFiles(Test t)
        {
            string endpoint = "http://localhost:" + "8080" + "/IService";
            Sender sndr = new Sender(endpoint);

            try
            {
                if (!sndr.download(t.testDriver))
                    return false;
                
                foreach (string filename in t.testCode)
                {
                    if (!sndr.download(filename))
                        return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        // listen messages on server
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
        static void Main(string[] args)
        { 
            TestHarness ts = new TestHarness();
            ts.testHarnessAddress = "8090";
            ts.repositoryAddress = "8080";
            ts.listen(ts.testHarnessAddress);

            Console.ReadKey();
        }
    }
}
