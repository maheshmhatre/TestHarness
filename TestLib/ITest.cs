/////////////////////////////////////////////////////////////////////////////
//  TestExecutive.cs - defines Test interface for Tests and object factory //
//  ver 1.0                                                                //
//  Language:     C#, VS 2015                                              //
//  Platform:     Lenovo Thinkpad E450, Windows 10 Ultimate                //
//  Application:  Test Harness                                             //
//  Source:       Jim Fawcett, CST 4-187                                   //
//  Author:       Mahesh Mhatre, Syracuse University                       //
//                (315) 412-8489, mrmhatre@syr.edu                         //
/////////////////////////////////////////////////////////////////////////////
/* 
 *   Module Operations
 *   -----------------
 *   This module defines test interface for test harness. It has two functions which have to be
 *   included by every test driver. Every test driver implements the interface.
 *
 *   Public Interface
 *   ----------------
 *   test            abstarct function that asks test driver to implements tests
 *   getLog          abstract function that forces client to implemment fetching of logs 
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   ITest.cs
 *   - Compiler command: devenv ITest.sln /rebuild debug
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
    public interface ITest
    {
        bool test();
        string getLog();
    }
}
