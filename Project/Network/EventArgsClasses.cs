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
        public Socket Socket { get; private set; }
        public Exception Exception { get; private set; }
        public ExceptionEventArgs(Socket socket, Exception exception)
        {
            Socket = socket;
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

    public class DisconnectEventArgs : EventArgs
    {
        public Socket Socket { get; private set; }
        public DisconnectEventArgs(Socket socket)
        {
            Socket = socket;
        }
    }
}
