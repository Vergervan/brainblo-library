using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace BrainBlo.Network
{
    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
        public ExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
    public class AcceptEventArgs : EventArgs
    {
        public Socket Socket { get; private set; }
        public AcceptEventArgs(Socket socket)
        {
            Socket = socket;
        }
    }
    public class MessageProcessEventArgs : EventArgs
    {
        public MessageData MessageData { get; private set; }
        public MessageProcessEventArgs(MessageData messageData)
        {
            MessageData = messageData;
        }
    }
}
