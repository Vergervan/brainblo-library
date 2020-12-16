﻿using System;
using System.Net;

namespace BrainBlo.NewNetwork
{
    public class Message
    {
        public byte[] messageBuffer;
        public int messageSize;
        public IPEndPoint point;

        public Message(byte[] messageBuffer, int messageSize, IPEndPoint point)
        {
            this.messageBuffer = new byte[messageSize];
            for (int i = 0; i < messageSize; i++) this.messageBuffer[i] = messageBuffer[i];
            this.messageSize = messageSize;
            this.point = point;
        }
        public Message(byte[] messageBuffer, IPEndPoint point) : this(messageBuffer, messageBuffer.Length, point) { }
    }
}
