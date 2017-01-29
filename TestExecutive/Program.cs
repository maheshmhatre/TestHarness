using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommTest;

namespace TestExecutive
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(500);
            Client client = new Client();
            client.connect("8080", "8090");
            client.listen("8075");
            Console.WriteLine("\n\n Req #1 Implemented in C# using the facilities of the .Net Framework Class Library and Visual Studio 2015, as provided in the ECS clusters.");
            Console.WriteLine("\n\n Req #2: Created test request in the form of xml each in the form of an a message with XML body that specifies the test developer's identity");
            Console.WriteLine(" and the names of a set of one or more test libraries to be tested");
            Console.WriteLine(" Sending dll files to repository ");
            client.uploadFile("TestDriver1.dll");
            client.uploadFile("CodeToTest1.dll");

            Message testRequestMessage = new Message();
            Test test1 = new Test();
            test1.author = " Mahesh Mhatre";
            test1.testId = "1";
            test1.testName = "Test1";
            test1.testDriver = "TestDriver1.dll";
            List<string> codeFiles = new List<string>();
            codeFiles.Add("CodeToTest1.dll");
            test1.testCode = codeFiles;
            Console.WriteLine(client.createTestRequest(test1));

            testRequestMessage.body = client.createTestRequest(test1);
            testRequestMessage.type = "testRequest";
            testRequestMessage.to = client.TestHarnessAddress;
            testRequestMessage.from = client.ClientAddress;
            
            client.postMessage(testRequestMessage);
            Console.WriteLine("\n\n Result of Test request ");
            Console.WriteLine(client.getMessage().body);

            Console.WriteLine("\n\n Showing req #3: Sending a test request whose file is not available on repository");
            Console.WriteLine(" File TestDriver11.dll is not present in repository. Sending Request to Test Harness");
            Test test2 = new Test();
            test2.author = " Mahesh Mhatre";
            test2.testId = "2";
            test2.testName = "Test2";
            test2.testDriver = "TestDriver11.dll";
            List<string> codeFiles1 = new List<string>();
            codeFiles1.Add("CodeToTest21.dll");
            test2.testCode = codeFiles1;
            testRequestMessage.body = client.createTestRequest(test2);
            client.postMessage(testRequestMessage);
            Console.WriteLine("\n\n Result of Test request ");
            Console.WriteLine(client.getMessage().body);

            Console.WriteLine("\n\n Req #4: Current test executive serves as a client for the system, another client is Wpf client and can be verified");
            Console.WriteLine(" The App domain creation can be verified in AppDomainManager.DomainCreator line no 113");
            Console.WriteLine(" The requests are processed concurrently by creating tasks, can be veified in TestHarness.cs file line no 225");

            Console.WriteLine(" \n\n Req #5: Derivation of ITest interface can be verified in TestDriver1 project line80");

            Console.WriteLine(" \n\n Req #6: Please look at The folder \\CommTest\\Host\\Repository for uploaded Library code and created logs");
            Console.WriteLine(" Test result on client has already beeen shown in req# 3");

            Console.WriteLine(" \n\n Req #7: Check the logs of Test Harness console for files uploaded on test harness server");

            Console.WriteLine(" Sending test libraries to the repository ");
            client.uploadFile("TestDriver2.dll");
            client.uploadFile("CodeToTest2.dll");

            Console.WriteLine("\n\n Req #8 & 9 Showing log file received on client ");
            client.getLogs("test2");
            Console.WriteLine(client.getMessage().body);

            Console.WriteLine("\n\n Req #10 All communication is implemented uing WCF, verify Sender, Reciver, & Iservice classes of Client, Host(Repository), and Test Harness Server");
            Console.WriteLine("\n\n Req #11 WPF client is created");

            Console.WriteLine(" \n\n Req #12 Communication latency can be tested by looking at GUI test result tab after running a test. Test running procedure is explained in readme.txt");
            Console.WriteLine(" \n\n Req #13 Provided Test executive");
            Console.WriteLine(" \n\n Req #14 Provided document Req14.txt in the root folder");
            Console.ReadKey();
        }
    }
}
