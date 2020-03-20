using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ServerUDP
{
    public class UDPMessage
    {
        public IPEndPoint ipend { get; set; }
        public byte[] message { get; set; }

        public UDPMessage(IPEndPoint ipend, byte[] message)
        {
            this.ipend = ipend;
            this.message = message;
        }
    }
}
