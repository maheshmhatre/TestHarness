/////////////////////////////////////////////////////////////////////////////
//  Sender.cs -   Provides sender endpoint for communication               //
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
 *   This module provides provides client to send the test requests 
 * 
 *   Public Interface
 *   ----------------
 *   postMessage(Message msg)                         sends test request messages to server
 *   download(string filename)                        downloads file
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
using SWTools;
using System.ServiceModel;
using System.Threading;
using System.IO;

namespace CommTest
{
    public class Sender
    {
        IService channel;
        BlockingQueue<Message> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;
        string lastError = "";
        string ToSendPath = "..\\..\\..\\Host\\Repository";
        string SavePath = "..\\..\\..\\Host\\Repository";
        int BlockSize = 1024;
        byte[] block;
        void ThreadProc()
        {
            while (true)
            {
                Message msg = sndBlockingQ.deQ();
                channel.postMessage(msg);
                if (msg.type == "quit")
                    break;
            }
        }

        public Sender(string url)
        {
            block = new byte[BlockSize];
            sndBlockingQ = new BlockingQueue<Message>();
            while (true)
            {
                try
                {
                    CreateSendChannel(url);
                    tryCount = 0;
                    Console.WriteLine(" Connection successful ");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Could not connect, trying again");
                    if (++tryCount < MaxCount)
                        Thread.Sleep(100);
                    else
                    {
                        lastError = ex.Message;
                        break;
                    }
                }
            }
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }

        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 50000000;
            ChannelFactory<IService> factory
              = new ChannelFactory<IService>(binding, address);
            channel = factory.CreateChannel();
        }

        // Sender posts message to another Peer's queue using
        // Communication service hosted by receipient via sndThrd

        public void PostMessage(Message msg)
        {
            sndBlockingQ.enQ(msg);
        }

        public void Close()
        {
            ChannelFactory<IService> temp = (ChannelFactory<IService>)channel;
            temp.Close();
        }

        public void uploadFile(string filename)
        {
            string fqname = Path.Combine(ToSendPath, filename);
            try
            {
                // hrt.Start();
                using (var inputStream = new FileStream(fqname, FileMode.Open))
                {
                    FileTransferMessage msg = new FileTransferMessage();
                    msg.filename = filename;
                    msg.transferStream = inputStream;
                    channel.upLoadFile(msg);
                }
                // hrt.Stop();
                Console.Write("\n  Uploaded file \"{0}\"", filename);
            }
            catch
            {
                Console.Write("\n  can't find \"{0}\"", fqname);
            }
        }

       public void download(string filename)
        {
            int totalBytes = 0;
            try
            {
               // hrt.Start();
                Stream strm = channel.downLoadFile(filename);
                string rfilename = Path.Combine(SavePath, filename);
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                using (var outputStream = new FileStream(rfilename, FileMode.Create))
                {
                    while (true)
                    {
                        int bytesRead = strm.Read(block, 0, BlockSize);
                        totalBytes += bytesRead;
                        if (bytesRead > 0)
                            outputStream.Write(block, 0, bytesRead);
                        else
                            break;
                    }
                }
             //   hrt.Stop();
              //  ulong time = hrt.ElapsedMicroseconds;
                Console.Write("\n  Received file \"{0}\" of {1} bytes", filename, totalBytes);
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
            }
        }
    }
}
