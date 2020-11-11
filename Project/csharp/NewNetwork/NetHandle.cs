using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace BrainBlo.NewNetwork
{
    public delegate void MessageCallbackHandler(NetHandle caller, Message message);
    public abstract class NetHandle
    {
        private Socket _socket; //Socket object of this handle
        private IPEndPoint _curEndPoint; //Current end point which was used in bind or connect
        private bool _isRunning;
        private ILog log;
        private int _bufferSize = 1024;
        public int BufferSize { get; set; }
        protected Socket SocketObject { get { return _socket; } set { _socket = value; }}
        public IPEndPoint CurrentEndPoint { get { return _curEndPoint; } }
        public bool IsRunning { get { return _isRunning; } }

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
            if (_isRunning)
            {
                Stop(true);
                throw new Exception("Server already is running");
            }
            try
            {
                Configure(); //Configures the socket. Must be overridden by derivative class
            }catch(Exception) { Stop(true); }
            _isRunning = true;
            Task.Run(() => Run(messageCallback));
        }
        private void Run(MessageCallbackHandler messageCallback)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.None, CurrentEndPoint.Port);
            byte[] messageBuffer = new byte[_bufferSize];
            int messageSize = 0;
            while (IsRunning)
            {
                try
                {
                    messageSize = SocketObject.ReceiveFrom(messageBuffer, ref endPoint);
                }
                catch (Exception) { continue; } //Need to make a log code for this exception
                messageCallback?.Invoke(this, new Message(messageBuffer, messageSize, (IPEndPoint) endPoint));
            }
            Stop(true);
        }
        public virtual void Send(Message message) //If you need to override the Send method, then you should use base.Send at the end of the new overridden method
        {
            try
            {
                _socket.SendTo(Resize(message), message.point);
            }
            catch (Exception) {}
        }

        private byte[] Resize(Message message)
        {
            if (message.messageBuffer.Length == message.messageSize) return message.messageBuffer;
            byte[] newBuffer = new byte[message.messageSize];
            for (int i = 0; i < newBuffer.Length; i++) newBuffer[i] = message.messageBuffer[i];
            return newBuffer;
        }

        protected virtual void Configure() { }

        public void Unuse()
        {
            Stop(true);
        }
        ~NetHandle()
        {
            Stop(false);
        }
        private void Stop(bool reuse)
        {
            _isRunning = false;
            _socket.Close();
            _socket.Dispose();
            if (reuse) _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            SendLog(0);
        }
        private void SendLog(LogCode logCode)
        {
            log?.LogCallback(new LogData(logCode));
        }
        public void SetLog(ILog log)
        {
            this.log = log;
        }
        public void ClearLog()
        {
            log = null;
        }

        public void SetReceiveBlock(bool state)
        {
            _socket.Blocking = state;
        }
    }
    public class LogData
    {
        public LogCode logCode { get; set; }
        public object addData;
        public LogData(LogCode logCode, object addData)
        {
            this.logCode = logCode;
            this.addData = addData;
        }
        public LogData(LogCode logCode) : this(logCode, null) { }
    }
    public enum LogCode
    {
        Debug = 0,
        Error = 1
    }
    public interface ILog
    {
        void LogCallback(LogData logData);
    }
}
