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
* Module Operations
* This class provides Service contract for communication between client repository and test harness 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.IO;

namespace CommTest
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void postMessage(Message msg);

        Message getMessage();

        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);

        [OperationContract]
        Stream downLoadFile(string filename);
    }

    [Serializable]
    public class Message
    {
        public string to { get; set; }
        public string from { get; set; }
        public string type { get; set; }
        public string body { get; set; }
    }

    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string filename { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream transferStream { get; set; }
    }
}
