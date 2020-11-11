using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace BrainBlo.Network
{
    internal class PendingMessage
    {
        public IPEndPoint Point { get; set; }
        public byte[] messageBytes { get; set; }
        public int length { get; set; }

        public PendingMessage(IPEndPoint point)
        {
            Point = point;
            messageBytes = new byte[] { };
            length = 0;
        }

        public PendingMessage(IPEndPoint point, byte[] bytes)
        {
            Point = point;
            messageBytes = bytes;
            length = 0;
        }

        public void CombineBuffers(byte[] addedBuffer)
        {
            messageBytes = Buffer.CombineBuffers(messageBytes, addedBuffer);
        }
    }
}
