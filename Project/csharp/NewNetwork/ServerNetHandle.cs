using System.Net;

namespace BrainBlo.NewNetwork
{
    public class ServerNetHandle : NetHandle
    {
        public ServerNetHandle(int port, bool blocking) : base(port) { Blocking = blocking; }
        public ServerNetHandle(int port) : base(IPAddress.Any, port) { }
        protected override void Configure()
        {
            Setup();
            base.Configure();
        }
    }
}
