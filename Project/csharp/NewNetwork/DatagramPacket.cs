using System.Net;

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
        }
        public DatagramPacket(byte[] messageBytes, int messageSize, IPEndPoint point)
        {
            this.messageBytes = messageBytes;
            this.messageSize = messageSize;
            this.point = point;
        }
        public DatagramPacket(byte[] messageBytes, IPEndPoint point) : this(messageBytes, messageBytes.Length, point) { }
        public void Send(NetHandle netHandle)
        {
            netHandle.Send(this);
        }
    }
}
