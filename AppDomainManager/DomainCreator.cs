/////////////////////////////////////////////////////////////////////////////
//  AppDomainManager.cs - create new app domain to execute tests           //
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
 *   This module accepts test requests in the blocking queue, extracts a test
 *   request from queue and starts a new child app domain. The assembly is executed in 
 *   child app domain and log is created.
 * 
 * 
 *   Public Interface
 *   ----------------
 *   makeChildAppDomain()              Creates child app domain
 *   executeTest()                     Executes loaded assembly in child app domain
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   AppDomainManager.cs
 *   - Compiler command: csc AppDomainManager.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 05 October 2016
 *     - first release
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommTest;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using LoadingTests;
using System.Runtime.Remoting;
using SWTools;
using ResultMessage;
using System.Threading;

namespace AppDomainManager
{
   public class DomainCreator
    {
        private AppDomain main;
        private AppDomainSetup domaininfo;
        private Evidence adevidence;
        private Result testResult;
        private string testName;
        private string clientAdrress;
        static private BlockingQueue<ResultMessage.Result> resQueue;

        // Constructor
        public DomainCreator(ref BlockingQueue<Result> rQ, string tname, string client)
        {
            try
            {
                clientAdrress = client;
                resQueue = new BlockingQueue<ResultMessage.Result>();
                resQueue = rQ;
                testName = tname;
                main = AppDomain.CurrentDomain;
                Directory.CreateDirectory(testName);
                domaininfo = new AppDomainSetup();
                domaininfo.ApplicationBase = "file:///" + System.Environment.CurrentDirectory;
                domaininfo.PrivateBinPath = testName;

                //Create evidence for the new AppDomain from evidence of current
                adevidence = AppDomain.CurrentDomain.Evidence;
                
                testResult = new Result();
            }
            catch (Exception except)
            {
                Console.Write("\n  {0}\n\n", except.Message);
            }
        }

        // makes child app domain and starts execution
        public void makeChildDomain(List<string> fileStore)
        {
            // copy files from cache to working directory
            foreach (var file in fileStore)
            {
                File.Copy(file,testName +"\\"+Path.GetFileName(file),true);
            }
            
            foreach (string assembly_ in fileStore)
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, testName, Path.GetFileName(assembly_)));
                try
                {
                    Assembly assem = Assembly.Load(assemblyName);
                    Type[] types = assem.GetExportedTypes();

                    foreach (Type type in types)
                    {
                        if (type.IsClass && typeof(ITest).IsAssignableFrom(type))
                        {
                            // Create Child AppDomain
                            Console.WriteLine("\n Creating a new child app domain");
                            AppDomain ad = AppDomain.CreateDomain("ChildDomain", adevidence, domaininfo);
                            string assemPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, testName,Path.GetFileName( assembly_));
                            executeTest(ad, assemPath, type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            // delete temp dir
            foreach (var file in fileStore)
            {
                File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, testName, Path.GetFileName(file)));
            }

            // delete dir
            Directory.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, testName));
        }

        // execute assembly
        public void executeTest(AppDomain ad, string assemblyName, Type type)
        {
            try
            {
                AssemblyName an = AssemblyName.GetAssemblyName(assemblyName);
                string name = an.ToString();
                ObjectHandle oh = ad.CreateInstance(name, type.FullName);
                object ob = oh.Unwrap();    // unwrap creates proxy to ChildDomain
                Console.WriteLine(" Creating object handle for {0}", ob);

                ITest tdr = (ITest)ob;
                bool result = tdr.test();
                Result r = new Result();

                r.testName = testName;
                r.to = clientAdrress;
                r.type = "result";
                if (result)
                {
                    r.resultString = "Test passed";
                }
                else
                {
                    r.resultString = "Test failed";
                }
                resQueue.enQ(r);

                string log = tdr.getLog();
                Result rLog = new Result();
                rLog.type = "log";
                r.to = clientAdrress;
                rLog.testName = testName;
                rLog.resultString = log;
                resQueue.enQ(rLog);
                // unloading ChildDomain, and so unloading the library
                AppDomain.Unload(ad);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" Exception occured during executing test in child app domain. {0}", ex.Message);
            }
        }

        static void Main(string[] args)
        {
            DomainCreator dc = new DomainCreator(ref DomainCreator.resQueue, "Test1","8085");
            List<string> fList = new List<string>();
            fList.Add("File1.dll");
            fList.Add("File1.dll");
            fList.Add("File1.dll");
            dc.makeChildDomain(fList);
        }
    }
}
