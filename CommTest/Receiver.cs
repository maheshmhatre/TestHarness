/////////////////////////////////////////////////////////////////////////////
//  Iservice.cs - Service contract for communication                       //
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
 *   This module provides provides client to send the test requests, get log results and test results.
 *   Client can also send files to repository and get log files from repository
 * 
 *   Public Interface
 *   ----------------
 *   postMessage(Message msg)                         sends test request messages to server
 *   Message getMessages()                            gets back the log, results
 *   uploadFile(filename)                             uploads the code files to repository
 *   stream downloadFile()                            gets back filestream from repository
 *   listen(port)                                      receives messages on a port
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
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SWTools;

namespace CommTest
{
   public class Receiver : IService
    {
        static BlockingQueue<Message> rcvBlockingQ = null;
        ServiceHost service = null;
        string filename;
        string savePath = "..\\..\\..\\SavedFiles";
        string ToSendPath = "..\\..\\..\\ToSend";
        int BlockSize = 1024;
        byte[] block;

        // constructor
        public Receiver()
        {
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Message>();

            block = new byte[BlockSize];
        }

        // create channel
        public void CreateRecvChannel(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 50000000;
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver), baseAddress);
            service.AddServiceEndpoint(typeof(IService), binding, baseAddress);
            service.Open();
        }

        // close the connection
        public void Close()
        {
            service.Close();
        }

        // receive messsage
        public Message getMessage()
        {
            return rcvBlockingQ.deQ();
        }

        // send message
        public void postMessage(Message msg)
        {
            rcvBlockingQ.enQ(msg);
        }

        // send file
        public void upLoadFile(FileTransferMessage msg)
        {
            int totalBytes = 0;
 
            filename = msg.filename;
            string rfilename = Path.Combine(savePath, filename);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            using (var outputStream = new FileStream(rfilename, FileMode.Create))
            {
                while (true)
                {
                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
                    totalBytes += bytesRead;
                    if (bytesRead > 0)
                        outputStream.Write(block, 0, bytesRead);
                    else
                        break;
                }
            }
          
            Console.Write(
              "\n  Received file \"{0}\" of {1} bytes ",
              filename, totalBytes);
        }

        // get file
        public Stream downLoadFile(string filename)
        {
           // hrt.Start();
            string sfilename = Path.Combine(ToSendPath, filename);
            FileStream outStream = null;
            if (File.Exists(sfilename))
            {
                outStream = new FileStream(sfilename, FileMode.Open);
            }
            else
                throw new Exception("open failed for \"" + filename + "\"");
           // hrt.Stop();
            Console.Write("\n  Sent \"{0}\"", filename);
            return outStream;
        }
    }
}
