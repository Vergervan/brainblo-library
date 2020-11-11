using System;
using System.Net;

namespace BrainBlo.NewNetwork
{
    public class ServerNetHandle : NetHandle
    {
        public ServerNetHandle(int port) : base(IPAddress.Any, port) { }
        protected override void Configure()
        {
            SocketObject.Bind(CurrentEndPoint);
        }
    }
}
