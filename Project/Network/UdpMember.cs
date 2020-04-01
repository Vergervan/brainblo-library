using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace BrainBlo.Network
{
    public class UdpMember
    {
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private IPEndPoint ipend;
        private int maxBufferSize = 1024;
        public int Available { get { return socket.Available; } private set { } }
        private BrainBlo.Debug.Log log;

        //Server fields
        private struct OnReceiveMessage
        {
            public IPEndPoint Point { get; set; }
            public byte[] Message { get; set; }
        }
        private bool isWorking = false;
        private List<PendingMessage> pendingMessages = new List<PendingMessage>();
        private readonly object lockPMlist = new object();
        private Queue<OnReceiveMessage> onReceiveMessages = new Queue<OnReceiveMessage>();
        private AutoResetEvent waitHandler = new AutoResetEvent(false);
        public int PendingMessagesCount { get { return pendingMessages.Count; } private set { } }
        public int ReceiveMessagesCount { get { return onReceiveMessages.Count; } private set { } }


        public UdpMember(int port)
        {
            ipend = new IPEndPoint(IPAddress.Any, port);
            socket.Bind(ipend);
            log = BrainBlo.Debug.Log.Initialize();
        }
        
        public UdpMember(string host, int port)
        {
            ipend = new IPEndPoint(IPAddress.Parse(host), port);
            socket.Connect(ipend);
            isWorking = true;
            log = BrainBlo.Debug.Log.Initialize();

            Thread listenThread = new Thread(() => ClientListenMessage());
            listenThread.Start();
        }

        ~UdpMember()
        {

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
            isWorking = true;
            listenThread.Start();
        }

        public void Stop()
        {
            isWorking = false;
        }

        public void Disconnect()
        {
            isWorking = false;
            socket.Disconnect(false);
        }

        private void ServerListenMessage()
        {
            while (isWorking)
            {
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.None, 25000);
                byte[] buffer = new byte[maxBufferSize];
                int messageSize = socket.ReceiveFrom(buffer, ref clientEndPoint);
                IPEndPoint cleanEP = (IPEndPoint)clientEndPoint;
                byte[] messageBuffer = Buffer.ChangeBufferSize(buffer, 0, messageSize);
                byte code = buffer[0];

                if (code == 0)
                {
                    PendingMessage pm = new PendingMessage(cleanEP);
                    pm.length = int.Parse($"{messageBuffer[1]}{messageBuffer[2]}{messageBuffer[3]}{messageBuffer[4]}");
                    log.WriteLine($"Received message with code 0, EndPoint = {cleanEP.ToString()} Length = {pm.length}");
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
                                log.WriteLine($"Received message with code 1, EndPoint = {cleanEP.ToString()}, Length = {messagePart.Length}");
                                pendingMessages[i].messageBytes = Buffer.CombineBuffers(pendingMessages[i].messageBytes, messagePart);
                                pendingMessages[i].length -= messageBuffer.Length - 1;
                                if (pendingMessages[i].length == 0)
                                {
                                    onReceiveMessages.Enqueue(new OnReceiveMessage { Message = pendingMessages[i].messageBytes, Point = pendingMessages[i].Point });
                                    log.WriteLine("Message added in queue");
                                    waitHandler.Set();
                                    log.WriteLine("AutoResetEvent change signal");
                                    pendingMessages.Remove(pendingMessages[i]);
                                    break;
                                }
                            }
                        }

                    }
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
                int messageSize = socket.ReceiveFrom(buffer, ref clientEndPoint);
                IPEndPoint cleanEP = (IPEndPoint)clientEndPoint;
                byte[] messageBuffer = Buffer.ChangeBufferSize(buffer, 0, messageSize);
                byte code = buffer[0];

                if (code == 0)
                {
                    pm = new PendingMessage(cleanEP);
                    pm.length = int.Parse($"{messageBuffer[1]}{messageBuffer[2]}{messageBuffer[3]}{messageBuffer[4]}");
                    log.WriteLine($"Received message with code 0, EndPoint = {cleanEP.ToString()} Length = {pm.length}");
                    pm.messageBytes = new byte[] { };
                }
                else if (code == 1)
                {
                    byte[] messagePart = Buffer.ChangeBufferSize(messageBuffer, 1, messageBuffer.Length - 1);
                    log.WriteLine($"Received message with code 1, EndPoint = {cleanEP.ToString()}, Length = {messagePart.Length}");
                    pm.messageBytes = Buffer.CombineBuffers(pm.messageBytes, messagePart);
                    pm.length -= messageBuffer.Length - 1;
                    if (pm.length == 0)
                    {
                        onReceiveMessages.Enqueue(new OnReceiveMessage { Message = pm.messageBytes, Point = pm.Point });
                        log.WriteLine("Message added in queue");
                        waitHandler.Set();
                        log.WriteLine("AutoResetEvent change signal");
                    }
                }
            }
        }

        public void Receive(ref IPEndPoint sender, ref byte[] message)
        {
            waitHandler.WaitOne();
            log.WriteLine("Receive method was called");
            OnReceiveMessage orm = onReceiveMessages.Dequeue();
            sender = orm.Point;
            message = orm.Message;
            waitHandler.Reset();
        }

        public byte[] Receive(ref IPEndPoint sender)
        {
            waitHandler.WaitOne();
            log.WriteLine("Receive method was called");
            OnReceiveMessage orm = onReceiveMessages.Dequeue();
            sender = orm.Point;
            waitHandler.Reset();
            return orm.Message;
        }

        public void Send(IPEndPoint point, byte[] message)
        {
            int bufferSize = message.Length;
            log.WriteLine($"Request to send message, Point: {point.ToString()}, Length: {bufferSize}");
            int lastIndex = 0;
            byte[] byteLength = new byte[4];
            string str = string.Format($"{message.Length}");
            for (int i = byteLength.Length - str.Length, j = 0; i < byteLength.Length; i++, j++)
            {
                byteLength[i] = (byte)int.Parse($"{str[j]}");
            }
            socket.SendTo(Buffer.AddStartCode(byteLength, 0), point);

            while (bufferSize > 0)
            {
                byte[] messagePartBuffer = Buffer.ChangeBufferSize(message, lastIndex, bufferSize < maxBufferSize ? bufferSize : maxBufferSize - 1);
                socket.SendTo(Buffer.AddStartCode(messagePartBuffer, 1), point);
                bufferSize -= maxBufferSize - 1;
                if (bufferSize < 0) bufferSize = 0;
                lastIndex += maxBufferSize - 1;
                log.WriteLine($"Send part of the message, Point: {point.ToString()}, Length: {messagePartBuffer.Length}, Left to send: {bufferSize}");
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
