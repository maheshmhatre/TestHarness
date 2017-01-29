/////////////////////////////////////////////////////////////////////////////
//  TestDriver.cs - Drive test 3 and produce logs                          //
//  ver 1.0                                                                //
//  Language:     C#, VS 2015                                              //
//  Platform:     Lenovo Thinkpad E450, Windows 10 Ultimate                //
//  Application:  Test Harness                                             //
//  Source:        James Fawcett, CST 4-187                                //
//  Author:       Mahesh Mhatre, Syracuse University                       //
//                (315) 412-8489, mrmhatre@syr.edu                         //
/////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
*    Test Driver calls Code to test1 and pewrforms test on test harness.
 *   Test driver needs to know the types and their interfaces
 *   used by the code it will test.  It doesn't need to know
 *   anything about the test harness.
 * 
 *   NOTE:
 *   This blocking queue is implemented using a Monitor and lock, which is
 *   equivalent to using a condition variable with a lock.
 * 
 *   Public Interface
 *   ----------------
 *   Create()    returns the instance of TestDriver
 *   Test()      calls the codetobetested and performs test
 *   getLog()    returns log after test
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs, Program.cs, CodeToBeTested.cs
 *   - Compiler command: devenv TestDriver.sln /rebuild debug
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 22 October 2013
 *     - first release
 *   ver 2.0 : 05 October 2016
       - second release
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDemo
{
    using LoadingTests;

    public class TestDriver3 : MarshalByRefObject, ITest
    {
        // constructor
        public TestDriver3()
        {
           
        }

        //----< test method is where all the testing gets done >---------
        public bool test()
        {
            int x = 6;
            int y = 0;
            int z = x / y;
            return true;
        }
        //----< test stub - not run in test harness >--------------------
        public string getLog()
        {
            return "Divide by zero exception was caught during the test";
        }
        static void Main(string[] args)
        {
            Console.Write("\n  Local test:\n");

            ITest test = new TestDriver3();

            if (test.test() == true)
                Console.Write("\n  test passed");
            else
                Console.Write("\n  test failed");
            Console.Write("\n\n");
        }
    }
}
