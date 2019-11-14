using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BrainBlo
{
    namespace Network
    {
        public class Client
        {
            public Socket Socket { get; private set; }
            public AsyncWay AsyncWay { get; private set; }
            private MessageProcessing MessageProcessing { get; set; }
            public event ConnectProcessing OnConnect;
            public event SendProcessing OnSend;
            public event ReceiveProcessing OnReceive;
            public event ExceptionProcessing OnConnectException;
            public event ExceptionProcessing OnSendException;
            public event ExceptionProcessing OnServerListenException;
            public ExceptionList exceptionList = new ExceptionList();


            public Client(Protocol protocol) : this(protocol, AsyncWay.Task) { }

            public Client(Protocol protocol, AsyncWay asyncWay)
            {
                if (protocol == Protocol.TCP) Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                AsyncWay = asyncWay;
            }

            public void Send(string message, bool useExceptionList)
            {
                Task.Run(() =>
                {
                    byte[] messageBytes = Buffer.AddSplitter(Encoding.UTF8.GetBytes(message), 0);
                    try
                    {
                        Socket.Send(messageBytes);
                        OnSend?.Invoke();
                    }
                    catch(Exception exception)
                    {
                        if (useExceptionList) CheckException(exception);
                        else OnSendException?.Invoke(exception);
                    }
                });
            }
         
            public void Connect(string host, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                Connect<string>(IPAddress.Parse(host), port, messageProcessing, useExceptionList);
            }

            public void Connect(IPAddress ipAddress, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                Connect<string>(ipAddress, port, messageProcessing, useExceptionList);
            }
            public void Connect<M>(string host, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                Connect<M>(IPAddress.Parse(host), port, messageProcessing, useExceptionList);
            }
            public void Connect<M>(IPAddress ipAddress, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                this.MessageProcessing = messageProcessing;
                switch (AsyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() =>
                        {
                            try
                            {
                                Socket.Connect(ipAddress, port);
                                OnConnect?.Invoke();
                                ListenServer<M>();
                            }
                            catch (Exception exception)
                            {
                                if (useExceptionList) CheckException(exception);
                                else OnConnectException?.Invoke(exception);
                            }
                        });
                        break;
                    case AsyncWay.Thread:
                        Thread thread = new Thread(() =>
                        {
                            try
                            {
                                Socket.Connect(ipAddress, port);
                                OnConnect?.Invoke();
                                ListenServer<M>();
                            }
                            catch (Exception exception)
                            {
                                if (useExceptionList) CheckException(exception);
                                else OnConnectException?.Invoke(exception);
                            }
                        });
                        thread.Start();
                        break;

                }
            }

            private void ListenServer<M>()
            {
                int fullMessageSize;
                string fullMessage;
                byte[] messageBuffer = new byte[1024];
                try
                {
                    while (true)
                    {
                        fullMessageSize = 0;
                        fullMessage = string.Empty;
                        do
                        {
                            int messageSize = Socket.Receive(messageBuffer);
                            fullMessageSize += messageSize;
                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (Socket.Available > 0);
                        OnReceive?.Invoke();

                        List<ByteArray> byteArrays = Buffer.SplitBuffer(Encoding.UTF8.GetBytes(fullMessage), 0);

                        object message = default;
                        foreach (var byteArray in byteArrays)
                        {
                            if (typeof(M) != typeof(string))
                            {
                                message = Utils.DeserializeJson<M>(Encoding.UTF8.GetString(byteArray.bytes));
                            }
                            else
                            {
                                message = Encoding.UTF8.GetString(byteArray.bytes);
                            }
                            MessageProcessing(new MessageData(message, fullMessageSize, fullMessage));
                        }
                    }
                }catch(Exception exception)
                {
                    if (OnServerListenException != null) OnServerListenException(exception);
                    else CheckException(exception);
                }
            }

            private void CheckException(Exception exception)
            {
                exceptionList.InvokeExceptionProcess(exception);
            }
        }
    }
}
