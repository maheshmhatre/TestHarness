﻿/////////////////////////////////////////////////////////////////////////////
//  TestDriver.cs - Drive test 2 and produce logs                          //
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
 *  
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

  public class TestDriver2 : MarshalByRefObject, ITest
  {
    private CodeToTest2 code;  // will be compiled into separate DLL
        public string getLog() { return "Test 2 log was created"; }
    //----< Testdriver constructor >---------------------------------
    /*
    *  For production code the test driver may need the tested code
    *  to provide a creational function.
    */
    public TestDriver2()
    {
      code = new CodeToTest2();
    }
    //----< factory function >---------------------------------------
    /*
    *   This can't be used by any code that doesn't know the name
    *   of this class.  That means the TestHarness will need to
    *   use reflection - ugh!
    *
    *   The language gives us this problem because it won't
    *   allow a static method in an interface or abstract class.
    */
    public static ITest create()
    {
      return new TestDriver2();
    }
    //----< test method is where all the testing gets done >---------

    public bool test()
    {
      code.annunciator("second being tested");
      return false;
    }

        //----< test stub - not run in test harness >--------------------

        static void Main(string[] args)
    {
      Console.Write("\n  Local test:\n");

      ITest test = TestDriver2.create();

      if (test.test() == true)
        Console.Write("\n  test passed");
      else
        Console.Write("\n  test failed");
      Console.Write("\n\n");
    }
  }
}
