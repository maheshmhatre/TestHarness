/////////////////////////////////////////////////////////////////////////////
//  LogManager.cs - create new app domain to execute tests                 //
//  ver 1.0                                                                //
//  Language:     C#, VS 2015                                              //
//  Platform:     Lenovo Thinkpad E450, Windows 10 Ultimate                //
//  Application:  Test Harness   
//  Source:       Jim Fawcett, CST 4-187
//  Author:       Mahesh Mhatre, Syracuse University                       //
//                (315) 412-8489, mrmhatre@syr.edu                         //
/////////////////////////////////////////////////////////////////////////////
/* 
 *   Module Operations
 *   -----------------
 *   This module provides code to be tested for test harness. It is ususally paired with a 
 *   TestDriver.dll file which uses this package. 
 * 
 * 
 *   Public Interface
 *   ----------------
 *   annunciator(string msg)                           Reads xml logs and creates Log objects
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   CodeToTest2.cs
 *   - Compiler command: devenv CodeToTest2.sln /rebuild debug
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

namespace LoadingTests
{
  public class CodeToTest2
  {
    // Announces the usage of the module
    public void annunciator(string msg)
    {
      Console.WriteLine("\n Production Code: {0}", msg);
    }

    // test stub
    static void Main(string[] args)
    {
      CodeToTest2 ctt = new CodeToTest2();
      ctt.annunciator("this is a test");
      Console.Write("\n\n");
    }
  }
}
