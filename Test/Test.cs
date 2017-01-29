/////////////////////////////////////////////////////////////////////////////
//  test.cs -     Model class for test objects                             //
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
 *   This module provides a way to store test objects in it
 * 
 *   Public Interface
 *   ----------------
 *   show()                     shows test object
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   Test.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace CommTest
{
    public class Test
    {
        public string testId { get; set; }
        public string testName { get; set; } // using default, if using authentication use your own
        public string author { get; set; }
        public DateTime timeStamp { get; set; }
        public string testDriver { get; set; }
        public List<string> testCode { get; set; }


        // shows test object
        public void show()
        {
            Console.Write("\n {0,-12} : {1}", "author ", author);
            Console.Write("\n {0,12} : {1}", author);
            Console.Write("\n {0,12} : {1}", timeStamp);
            Console.Write("\n {0,12} : {1}", testDriver);
            // foreach(string in )
        }

        static void Main(string[] args)
        {
            Test t = new Test();
            t.author = "Mahesh Mhatre";
            t.testName = "test1";
            t.timeStamp = DateTime.Now;
            t.testDriver = "td1.dll";

            t.testCode.Add("tc1.dll");
            t.testCode.Add("tc2.dll");
            t.testCode.Add("tc3.dll");
            t.show();
        }
    }
}
