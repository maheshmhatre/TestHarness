/////////////////////////////////////////////////////////////////////////////
//  Builder.cs -  Module to build xml tests                                //
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
 *   This module provides provides a way to build xml test requests
 * 
 *   Public Interface
 *   ----------------
 *   createRoot(string)           create root element to the document
 *   XElement makeElement(tring)  creates XElement
 *   addAttribute                 creates attribute
 *   addTo(XElement, XElement)    add element to a parent element
 *   string tostring()            converts document to string
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XMLbuilder
{
   public class Builder
    {
        private XDocument doc_;
        private XElement root;

        // constructor
        public Builder()
        {
            doc_ = new XDocument();
            // doc_.Add(root);
        }

        // root of the xml 
        public void createRoot(string rootName)
        {
            root = new XElement(rootName);
        }

        // add an element to root
        public void addToRoot(XElement elem)
        {
            root.Add(elem);
        }


        // create XElement
        public XElement makeElement(string elem)
        {
            return new XElement(elem);
        }

        // add to other XElement
        public void addTo(ref XElement parent, ref XElement child)
        {
            parent.Add(child);
        }

        // add attribute to XElement
        public void addAttribute(ref XElement elem, ref XAttribute attrib)
        {
            elem.Add(attrib);
        }

        // convert to string
        public string toString()
        {
            doc_.Add(root);
            return doc_.ToString();
        }


        static void Main(string[] args)
        {
            Builder builder = new Builder();

            builder.createRoot("TestRequest");

            XElement author = builder.makeElement("Author");
            XAttribute attribute = new XAttribute("name", "Mahesh Mhatre");
            builder.addAttribute(ref author, ref attribute);
            builder.addToRoot(author);

            XElement test = builder.makeElement("Test");
            XAttribute testId = new XAttribute("id", 1);
            builder.addAttribute(ref test, ref testId);

            XElement testName = new XElement("test1");
            builder.addTo(ref test, ref testName);

            XElement testDriver1 = new XElement("TestDriver", "td1.dll");//builder.makeElement("TestDriver");
            builder.addTo(ref test, ref testDriver1);

            XElement testCode1 = new XElement("TestCode", "tc1.dll");
            builder.addTo(ref test, ref testCode1);
            XElement testCode2 = new XElement("TestCode", "tc2.dll");
            builder.addTo(ref test, ref testCode2);
            builder.addToRoot(test);
            string serializedString = builder.toString();
        }
    }
}

