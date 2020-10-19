using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BrainBlo.NewNetwork
{
    public class ServerNetHandle : NetHandle
    {
        public ServerNetHandle(int port) : base(IPAddress.Any, port) { }
        protected override void Run(MessageCallbackHandler messageCallback)
        {
            SocketObject.Bind(CurrentEndPoint);
            EndPoint endPoint = new IPEndPoint(IPAddress.None, CurrentEndPoint.Port);
            byte[] messageBuffer = new byte[1024];
            int messageSize = 0;
            while (IsRunning)
            {
                messageSize = SocketObject.ReceiveFrom(messageBuffer, ref endPoint);
                IPEndPoint cleanEndPoint = (IPEndPoint)endPoint;
                messageCallback(new Message { messageSize = messageSize, messageBuffer = messageBuffer, point = cleanEndPoint });
            }
        }
    }
}
