using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BrainBlo.NewNetwork
{
    public class ClientNetHandle : NetHandle
    {
        public ClientNetHandle(int port) : base(port) { }
        public ClientNetHandle(IPAddress ipAddress, int port) : base(ipAddress, port) { }
        public ClientNetHandle(string hostname, int port) : base(hostname, port) { }
        protected override void Run(MessageCallbackHandler messageCallback)
        {
            SocketObject.Connect(CurrentEndPoint);
            EndPoint endPoint = new IPEndPoint(IPAddress.None, CurrentEndPoint.Port);
            byte[] messageBuffer = new byte[1024];
            int messageSize = 0;
            while (IsRunning)
            {
                messageSize = SocketObject.ReceiveFrom(messageBuffer, ref endPoint);
                messageCallback(new Message { messageSize = messageSize, messageBuffer = messageBuffer, point = CurrentEndPoint });
            }
        }
    }
}
