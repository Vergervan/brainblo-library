using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo
{
    namespace Network
    {
        public class MessageInfo
        {
            public Exception[] exceptions { get; private set; }
            public object message { get; private set; }
            public int messageSize { get; private set; }
            public byte[] messageBuffer { get; private set; }
            public string fullMessage { get; private set; }

            public MessageInfo(Exception[] exceptions, object message, int messageSize, byte[] messageBuffer, string fullMessage)
            {
                this.exceptions = exceptions;
                this.message = message;
                this.messageSize = messageSize;
                this.messageBuffer = messageBuffer;
                this.fullMessage = fullMessage;
            }
        }
        public enum Protocol
        {
            TCP = 0,
        }
        public enum ThreadType
        {
            Thread = 0,
            Task = 1
        }
        public delegate void MessageProcessing(MessageInfo messageInfo);
    }
}
