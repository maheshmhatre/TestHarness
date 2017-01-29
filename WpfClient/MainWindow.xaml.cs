using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommTest;
using System.Xml.Linq;
using System.Threading;
using System.IO;
using HRTimer;
namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string testDriver;
        private List<string> testCode;
        private Client client;
        private int id;
        private Thread rcvThrd;
        Message rcvdMsg;
        HiResTimer hTime;
        public MainWindow()
        {
            InitializeComponent();
            testCode = new List<string>();
            hTime = new HiResTimer();
            rcvdMsg = new Message();
            // Set the SelectionMode to select multiple items.
            listBoxCodeTest.SelectionMode = SelectionMode.Multiple;
            client = new Client();
            id = 0;
        }

        void ThreadProc()
        {
            while (true)
            {
                
                rcvdMsg = client.getMessage();
                process(rcvdMsg);
                Console.WriteLine(" Message on server {0} , {1}", rcvdMsg.type, rcvdMsg.body);
            }
        }

        private void process(Message message)
        {
            if (message.type == "filesreply")
            {
                Dispatcher.Invoke(() => updateListBoxes(rcvdMsg.body));
                //Console.WriteLine(message.body);

            }
            else if (message.type == "testresult")
            {
                Console.WriteLine("\n Result from Test Harness received");
                XDocument docRes = XDocument.Parse(message.body);
                XElement tNElem = docRes.Descendants("TestName").First();
                string testName = tNElem.Value;

                XElement reslement = docRes.Descendants("TestResult").First();
                string res = reslement.Value;
                //Console.WriteLine(" Test {0} , Result {1}", testName, res);
                string resT = " Test: " + testName + "\n" + res;

                Dispatcher.Invoke(() => { updateGUI(resT); });
            }
            else if (message.type == "logreply")
            {
                Dispatcher.Invoke(() => { updateLogs(message.body); });
            }
        }

        void updateLogs(string log)
        {
            XDocument doc_ = XDocument.Parse(log);
            IEnumerable<XElement> children = doc_.Root.Descendants();

            foreach (XElement child in children)
            {
                if (child.Name == "TestName")
                {
                    textBoxLog.AppendText(" Test: " + child.Value + "\n");
                }
                else if (child.Name == "Log")
                {
                    textBoxLog.AppendText(" Log: " + child.Value + "\n");
                }
            }
        }

        void updateListBoxes(string files)
        {
            if (files != null)
            {
                string[] displayFiles = files.Split('\n');

                foreach (var file in displayFiles)
                {
                    listBoxCodeTest.Items.Insert(0, file);
                    listBoxTestDriver.Items.Insert(0, file);
                }
            }
        }

        void updateGUI(string message)
        {
            textBoxResult.Text = message;
            hTime.Stop();

            textBoxResult.AppendText("\n Time elapsed: " + hTime.ElapsedMicroseconds +" microS.");

        }

        private void buttonClientListen_Click(object sender, RoutedEventArgs e)
        {
            string port = clientHost.Text;
            client.listen(port);

            rcvThrd = new Thread(new ThreadStart(this.ThreadProc));
            rcvThrd.IsBackground = true;
            rcvThrd.Start();
        }

        private void buttonTestRequest_Click(object sender, RoutedEventArgs e)
        {
            hTime.Start();
            id++;
            Message testRequestMessage = new Message();
            Test test1 = new Test();
            test1.author = " Mahesh Mhatre";
            test1.testId = id.ToString();
            test1.testName = textBoxTestName.Text;
            test1.testDriver = System.IO.Path.GetFileName( testDriver);
            test1.testCode = testCode;

            testRequestMessage.body = client.createTestRequest(test1);
            testRequestMessage.type = "testRequest";
            testRequestMessage.to = serverHost.Text;
            testRequestMessage.from = clientHost.Text;
            client.postMessage(testRequestMessage);

        }

        private void listBoxTestDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            testDriver = listBoxTestDriver.SelectedItem.ToString();
        }

        private void listBoxCodeTest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> selecteds  = new List<string>();

            foreach (string item in listBoxCodeTest.SelectedItems)
            {
                selecteds.Add(System.IO.Path.GetFileName(item));
            }
        }

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            if (repositoryHost.Text == "" || serverHost.Text == "")
            {
                labelStatus.Content = " Please enter valid addr to textbox";
            }
            else
            {
                client.connect(repositoryHost.Text, serverHost.Text);
                labelStatus.Content = " Connected successfully";

                Message msg = new Message();
                msg.type = "getfiles";
                msg.to = client.RepositoryAddress;
                msg.from = client.ClientAddress;
                msg.body = "empty";
                client.postMessage(msg);
            } 
        }

        private void buttonLog_Click(object sender, RoutedEventArgs e)
        {
            string key = textBoxLogRequest.Text;

            client.getLogs(key);
        }
    }
}
