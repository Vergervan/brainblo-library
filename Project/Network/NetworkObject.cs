using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace BrainBlo.Network
{
    public enum NetworkObjectType
    {
        Server = 0,
        Client = 1
    }
    public class NetworkObject
    {
        protected Socket Socket { get; set; }
        protected AsyncWay AsyncWay { get; set; }
        public bool IsWorking { get; set; }
        //public NetworkObjectType NetworkObjectType { get; private set; }
        public NetworkObject(Protocol protocol) : this(protocol, AsyncWay.Task) { }
        public NetworkObject(Protocol protocol, AsyncWay asyncWay)
        {
            if (protocol == Protocol.TCP) Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            AsyncWay = asyncWay;
        }
    }
}
