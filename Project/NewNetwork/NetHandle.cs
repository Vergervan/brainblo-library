using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BrainBlo.NewNetwork
{
    public abstract class NetHandle
    {
        private Socket _socket; //Socket object of this handle
        private IPEndPoint _curEndPoint; //Current end point which was used in bind or connect
        private bool _isRunning;
        public Socket SocketObject { get { return _socket; } }
        public IPEndPoint CurrentEndPoint { get { return _curEndPoint; } }
        public bool IsRunning { get { return _isRunning; } }

        public delegate void MessageCallbackHandler(Message message);

        /// <param name="ipAddress">IP address for setting the server point</param>
        public NetHandle(IPAddress ipAddress, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _curEndPoint = new IPEndPoint(ipAddress, port);
        }
        public NetHandle(int port) : this(IPAddress.Parse("127.0.0.1"), port) { } //In the constructor without IP, the endpoint will be localhost
        public NetHandle(string hostname, int port)
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname); //Getting a list of addresses by hostname
            if (hostAddresses.Length != 0) //If there are more than 0 addresses in the list, then we'll use the first IP in the IPEndPoint constructor
                _curEndPoint = new IPEndPoint(hostAddresses[0], port);
        }
        public void Use(MessageCallbackHandler messageCallback)
        {
            if (_isRunning) throw new Exception("Server already is running");
            _isRunning = true;
            Task.Run(() => Run(messageCallback));
        }
        protected virtual void Run(MessageCallbackHandler messageCallback) { }
        public virtual void Send(Message message)
        {
            _socket.SendTo(message.messageBuffer, message.point);
        }
        public void Stop()
        {
            _isRunning = false;
        }
        ~NetHandle()
        {
            _socket.Close();
            _socket.Dispose();
        }
    }
    public class Message
    {
        public int messageSize;
        public byte[] messageBuffer;
        public IPEndPoint point;
    }
}
