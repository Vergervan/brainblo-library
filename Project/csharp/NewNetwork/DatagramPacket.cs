﻿using System.Net;

namespace BrainBlo.NewNetwork
{
    public class DatagramPacket
    {
        public byte[] messageBytes;
        public int messageSize;
        public IPEndPoint point;
        public DatagramPacket() { }
        public DatagramPacket(byte[] messageBytes)
        {
            this.messageBytes = messageBytes;
            messageSize = messageBytes.Length;
        }
        public DatagramPacket(byte[] messageBytes, int messageSize, IPEndPoint point)
        {
            this.messageBytes = new byte[messageSize];
            for (int i = 0; i < messageSize; i++) this.messageBytes[i] = messageBytes[i];
            this.messageSize = messageSize;
            this.point = point;
        }
        public DatagramPacket(byte[] messageBytes, IPEndPoint point) : this(messageBytes, messageBytes.Length, point) { }
        public void Send(NetHandle netHandle) => Send(netHandle, netHandle.CurrentEndPoint);
        public void Send(NetHandle netHandle, IPEndPoint point)
        {
            this.point = point;
            netHandle.Send(this);
        }
    }
}
