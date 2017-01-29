/////////////////////////////////////////////////////////////////////////////
//  Result.cs -   Model classs to store result information                 //
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
 *   This module provides a way to store result information
 * 
 *   Public Interface
 *   ----------------
 *   show()                         shows object
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultMessage
{
    public class Result
    {
        public string testName { get; set; }
        public string to { get; set; }
        public string type { get; set; }
        public string resultString { get; set; }

        // shows result object on console
        public void show()
        {
            Console.WriteLine(" test Name: {0} , Result String: {1} ", testName,resultString);
        }
        static void Main(string[] args)
        {
            Result r = new Result();
            r.testName = "Test1";
            r.resultString = "Test pass";
            r.show();
        }
    }
}
