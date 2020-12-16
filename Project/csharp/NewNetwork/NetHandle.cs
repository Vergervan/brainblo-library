using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using static BrainBlo.NewNetwork.LogCode;

namespace BrainBlo.NewNetwork
{
    public delegate void MessageCallbackHandler(NetHandle caller, Message message);
    public abstract class NetHandle
    {
        private Socket _socket; //Socket object of this handle
        private IPEndPoint _curEndPoint; //Current end point which was used in bind or connect
        private bool _isRunning;
        private ILog log; //Object realized this interface will receive states of NetHandle
        private int _bufferSize = 1024; //Number of allowed bytes to receive per packet
        private MessageCallbackHandler _msgCallback; //Delegate contains a callback function to process message
        private bool Blocking { get { return _socket.Blocking; } set { _socket.Blocking = value; } }
        public int BufferSize { get; set; }
        protected Socket SocketObject { get { return _socket; } set { _socket = value; }}
        public IPEndPoint CurrentEndPoint { get { return _curEndPoint; } }
        public bool IsRunning { get { return _isRunning; } }

        /// <param name="ipAddress">IP address for setting the server point</param>
        public NetHandle(IPAddress ipAddress, int port, bool blocking)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _curEndPoint = new IPEndPoint(ipAddress, port);
            Blocking = blocking;
        }
        public NetHandle(IPAddress ipAddress, int port) : this(ipAddress, port, true) { }
        public NetHandle(int port) : this(IPAddress.Parse("127.0.0.1"), port, true) { } //In the constructor without IP, the endpoint will be localhost
        public NetHandle(string hostname, int port, bool block)
        {
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostname); //Getting a list of addresses by hostname
            Blocking = block;
            if (hostAddresses.Length != 0) //If there are more than 0 addresses in the list, then we'll use the first IP in the IPEndPoint constructor
                _curEndPoint = new IPEndPoint(hostAddresses[0], port);
            else
                SendLog(ERR_CONSTRUCTOR_HOSTNAME);
        }
        public NetHandle(string hostname, int port) : this(hostname, port, true) { }
        public void Use(MessageCallbackHandler messageCallback)
        {
            _msgCallback = messageCallback;
            if (!Blocking)
            { 
                TryToReceiveMessage();
                return;
            }
            if (_isRunning)
            {
                SendLog(ERR_OBJ_ALREADY_USED);
            }
            try
            {
                Configure(); //Configures the socket. Must be overridden by derivative class
            }catch(Exception e) { SendLog(ERR_CONFIGURE, e); }
            finally { SendLog(ST_USE); }
            _isRunning = true;
            Task.Run(() => Run());
        }

        private void Run()
        {
            while (IsRunning)
            {
                TryToReceiveMessage();
            }
            Stop(true);
        }

        private void TryToReceiveMessage()
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.None, CurrentEndPoint.Port);
            byte[] messageBuffer = new byte[_bufferSize];
            int messageSize = 0;
            try
            {
                messageSize = SocketObject.ReceiveFrom(messageBuffer, ref endPoint);
            }
            catch (Exception e)
            {
                SendLog(ERR_RECEIVE, e);
                return;
            }
            _msgCallback?.Invoke(this, new Message(messageBuffer, messageSize, (IPEndPoint)endPoint)); //Starts a callback function with a new message
        }

        public virtual void Send(Message message) //If you need to override the Send method, then you should use base.Send at the end of the new overridden method
        {
            try
            {
                _socket.SendTo(message.messageBuffer, message.point);
            }
            catch (Exception e) { SendLog(ERR_SEND, e); }
            finally { SendLog(ST_SEND); }
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
            SendLog(ST_STOP);
        }
        private void SendLog(LogCode logCode)
        {
            log?.LogCallback(new LogData(logCode));
        }
        private void SendLog(LogCode logCode, object addData)
        {
            log?.LogCallback(new LogData(logCode, addData));
        }
        public void SetLog(ILog log)
        {
            this.log = log;
        }
        public void ClearLog()
        {
            log = null;
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
        DEBUG = 0,
        //Error codes
        ERR_RECEIVE = 1,
        ERR_OBJ_ALREADY_USED = 2,
        ERR_CONSTRUCTOR_HOSTNAME = 3,
        ERR_CONFIGURE = 4,
        ERR_SEND = 5,

        //State codes
        ST_USE = 20,
        ST_SEND = 21,
        ST_STOP = 22

    }
    public interface ILog
    {
        void LogCallback(LogData logData);
    }
}
