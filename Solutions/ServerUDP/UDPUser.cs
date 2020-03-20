using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ServerUDP
{
    public class UDPUser
    {
        public IPEndPoint UserIP { get; set; }
        public byte[] messageBytes { get; set; }
        public int length { get; set; }

        public UDPUser(IPEndPoint userip)
        {
            UserIP = userip;
            messageBytes = new byte[] { };
            length = 0;
        }

        public UDPUser(IPEndPoint userip, byte[] bytes)
        {
            UserIP = userip;
            messageBytes = bytes;
            length = 0;
        }

        public void CombineBuffers(byte[] addedBuffer)
        {
            messageBytes = BrainBlo.Buffer.CombineBuffers(messageBytes, addedBuffer);
        }
    }
}
