using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Timers;
using System.Reflection;
using Timer = System.Timers.Timer;

namespace BrainBlo.Network
{
    public enum SendOption
    {
        None = 0,
        FullPacket = 1
    }
    public class UdpMember
    {
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public Socket Socket { get => socket; }
        private SendOption _sendOption = SendOption.None;
        public event EventHandler OnDispose; 
        public IPEndPoint EndPoint { get; private set; }
        private int maxBufferSize = 1024;
        public int Available { get => socket.Available; }
        private BrainBlo.Debug.Log log;

        //Server fields
        private struct OnReceiveMessage
        {
            public IPEndPoint Point { get; set; }
            public byte[] Message { get; set; }
        }
        private bool isWorking;
        private List<PendingMessage> pendingMessages = new List<PendingMessage>();
        private readonly object lockPMlist = new object();
        private Queue<OnReceiveMessage> onReceiveMessages = new Queue<OnReceiveMessage>();
        private AutoResetEvent waitHandler = new AutoResetEvent(false);
        private AutoResetEvent waitResponse = new AutoResetEvent(false);
        public int PendingMessagesCount { get { return pendingMessages.Count; } private set { } }
        public int ReceiveMessagesCount { get { return onReceiveMessages.Count; } private set { } }

        public UdpMember() { }

        public UdpMember(int port)
        {
            log = BrainBlo.Debug.Log.Initialize();
            EndPoint = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(EndPoint);
            isWorking = true;
        }
        
        public UdpMember(string host, int port)
        {
            log = BrainBlo.Debug.Log.Initialize();
            EndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            try
            {
                socket.Connect(EndPoint);
            }catch(Exception e)
            {
                log.Write($"{this} {MethodBase.GetCurrentMethod()}: {e.Message}");
            }
            isWorking = true;

            Thread listenThread = new Thread(() => ClientListenMessage());
            listenThread.Start();
        }

        ~UdpMember()
        {
            OnDispose?.Invoke(this, new EventArgs());
            socket.Close();
            socket.Dispose();
        }

        public void SetMaxBufferSize(int count)
        {
            maxBufferSize = count;
        }

        

        public void Start()
        {
            Thread listenThread = new Thread(() => ServerListenMessage());
            listenThread.Start();
            log.Write("ServerListenMessage() start new Thread");
        }

        public void Stop()
        {
            isWorking = false;
        }

        public void Disconnect()
        {
            socket.SendTo(new byte[] { 3 }, EndPoint);
            isWorking = false;
            socket.Disconnect(false);
        }

        private void ServerListenMessage()
        {
            while (isWorking)
            {
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.None, 25000);
                byte[] buffer = new byte[maxBufferSize];
                int messageSize = 0;
                try
                {
                    messageSize = socket.ReceiveFrom(buffer, ref clientEndPoint);
                }catch(Exception e)
                {
                    log.Write($"{this} {MethodBase.GetCurrentMethod()}: {e.Message}");
                    Stop();
                    return;
                }
                IPEndPoint cleanEP = (IPEndPoint)clientEndPoint;
                byte[] messageBuffer = Buffer.ChangeBufferSize(buffer, 0, messageSize);
                byte code = buffer[0];

                if (code == 0)
                {
                    int length = int.Parse($"{messageBuffer[1]}{messageBuffer[2]}{messageBuffer[3]}{messageBuffer[4]}");
                    if (length == 0) continue;
                    PendingMessage pm = new PendingMessage(cleanEP);
                    pm.length = length;
                    pm.messageBytes = new byte[] { };
                    pendingMessages.Add(pm);
                }
                else if (code == 1)
                {
                    lock (lockPMlist)
                    {
                        for (int i = 0; i < pendingMessages.Count; i++)
                        {
                            if (pendingMessages[i].Point.ToString() == cleanEP.ToString())
                            {
                                byte[] messagePart = Buffer.ChangeBufferSize(messageBuffer, 1, messageBuffer.Length - 1);
                                pendingMessages[i].messageBytes = Buffer.CombineBuffers(pendingMessages[i].messageBytes, messagePart);
                                pendingMessages[i].length -= messageBuffer.Length - 1;
                                if (pendingMessages[i].length == 0)
                                {
                                    onReceiveMessages.Enqueue(new OnReceiveMessage { Message = pendingMessages[i].messageBytes, Point = pendingMessages[i].Point });
                                    waitHandler.Set();
                                    pendingMessages.Remove(pendingMessages[i]);
                                    break;
                                }
                            }
                        }
                    }
                }
                else if(code == 3)
                {

                }
            }
        }

        private void ClientListenMessage()
        {
            PendingMessage pm = null;
            while (isWorking)
            {
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.None, 25000);
                byte[] buffer = new byte[maxBufferSize];
                int messageSize = 0;
                try
                {
                    messageSize = socket.ReceiveFrom(buffer, ref clientEndPoint);
                }catch(Exception e)
                {
                    log.Write($"{this} {MethodBase.GetCurrentMethod()}: {e.Message}");
                    Stop();
                    return;
                }
                IPEndPoint cleanEP = (IPEndPoint)clientEndPoint;
                byte[] messageBuffer = Buffer.ChangeBufferSize(buffer, 0, messageSize);
                byte code = buffer[0];

                if (code == 0)
                {
                    int length = int.Parse($"{messageBuffer[1]}{messageBuffer[2]}{messageBuffer[3]}{messageBuffer[4]}");
                    if (length == 0) continue;
                    pm = new PendingMessage(cleanEP);
                    pm.length = length;
                    pm.messageBytes = new byte[] { };
                }
                else if (code == 1)
                {
                    byte[] messagePart = Buffer.ChangeBufferSize(messageBuffer, 1, messageBuffer.Length - 1);
                    pm.messageBytes = Buffer.CombineBuffers(pm.messageBytes, messagePart);
                    pm.length -= messageBuffer.Length - 1;
                    if (pm.length == 0)
                    {
                        onReceiveMessages.Enqueue(new OnReceiveMessage { Message = pm.messageBytes, Point = pm.Point });
                        waitHandler.Set();
                    }
                }
            }
        }

        public void Receive(ref IPEndPoint sender, ref byte[] message)
        {
            waitHandler.WaitOne();
            OnReceiveMessage orm = onReceiveMessages.Dequeue();
            sender = orm.Point;
            message = orm.Message;
            waitHandler.Reset();
        }

        public byte[] Receive()
        {
            waitHandler.WaitOne();
            OnReceiveMessage orm = onReceiveMessages.Dequeue();
            waitHandler.Reset();
            return orm.Message;
        }

        public byte[] Receive(ref IPEndPoint sender)
        {
            waitHandler.WaitOne();
            OnReceiveMessage orm = onReceiveMessages.Dequeue();
            sender = orm.Point;
            waitHandler.Reset();
            return orm.Message;
        }
        //TODO Response mechanism
        public void WaitResponse(IPEndPoint point)
        {

        }

        public void Send(byte[] message) => Send(EndPoint, message, SendOption.None);
        public void Send(IPEndPoint point, byte[] message) => Send(point, message, SendOption.None);

        public void Send(IPEndPoint point, byte[] message, SendOption sendOption)
        {
            int bufferSize = message.Length;
            
            byte[] byteLength = new byte[4];
            string str = string.Format($"{message.Length}");
            for (int i = byteLength.Length - str.Length, j = 0; i < byteLength.Length; i++, j++)
            {
                byteLength[i] = (byte)int.Parse($"{str[j]}");
            }
            try
            {
                socket.SendTo(Buffer.AddStartCode(byteLength, 0), point);
            }
            catch (Exception e)
            {
                log.Write($"{this} {MethodBase.GetCurrentMethod()}: {e.Message}");
                return;
            }

            switch (sendOption) {
                case SendOption.None:
                    int lastIndex = 0;
                    while (bufferSize > 0)
                    {
                        byte[] messagePartBuffer = Buffer.ChangeBufferSize(message, lastIndex, bufferSize < maxBufferSize ? bufferSize : maxBufferSize - 1);
                        try
                        {
                            socket.SendTo(Buffer.AddStartCode(messagePartBuffer, 1), point);
                        } catch (Exception e)
                        {
                            log.Write($"{this} {MethodBase.GetCurrentMethod()}: {e.Message}");
                            return;
                        }
                        bufferSize -= maxBufferSize - 1;
                        if (bufferSize < 0) bufferSize = 0;
                        lastIndex += maxBufferSize - 1;
                    }
                    break;
                case SendOption.FullPacket:
                    socket.SendTo(Buffer.AddStartCode(message, 1), point);
                    break;
            }
        }

        

        private bool PendingMessageInList(IPEndPoint point)
        {
            for (int i = 0; i < pendingMessages.Count; i++)
            {
                lock (lockPMlist)
                    if (point.ToString() == pendingMessages[i].Point.ToString()) return true;
            }
            return false;
        }
    }
}
