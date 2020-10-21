using System;
using System.Net;

namespace BrainBlo.NewNetwork
{
    public class Message
    {
        public byte[] messageBuffer { get; private set; }
        public int messageSize { get; private set; }
        public IPEndPoint point { get; private set; }

        public Message(byte[] messageBuffer, int messageSize, IPEndPoint point)
        {
            this.messageBuffer = messageBuffer;
            this.messageSize = messageSize;
            this.point = point;
        }
        public Message(byte[] messageBuffer, IPEndPoint point) : this(messageBuffer, messageBuffer.Length, point) { }
    }
}
