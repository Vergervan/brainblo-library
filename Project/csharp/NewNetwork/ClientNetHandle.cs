using System.Net;

namespace BrainBlo.NewNetwork
{
    public class ClientNetHandle : NetHandle
    {
        public ClientNetHandle(IPAddress ipAddress, int port, bool blocking) : base(ipAddress, port, blocking) { }
        public ClientNetHandle(int port) : base(port) { }
        public ClientNetHandle(IPAddress ipAddress, int port) : base(ipAddress, port) { }
        public ClientNetHandle(string hostname, int port) : base(hostname, port) { }
        protected override void Configure()
        {
            Connect();
        }
    }
}
