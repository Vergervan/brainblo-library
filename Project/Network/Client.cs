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
            private Socket socket { get; set; }
            public AsyncWay asyncWay { get; private set; }
            private MessageProcessing messageProcessing { get; set; }
            public event ConnectProcessing OnConnect;
            public event SendProcessing OnSend;
            public event ReceiveProcessing OnReceive;
            public event ExceptionProcessing OnConnectException;
            public event ExceptionProcessing OnSendException;
            public event ExceptionProcessing OnServerListenException;
            public ExceptionList exceptionList = new ExceptionList();

            
            public Client(Protocol protocol)
            {
                if (protocol == Protocol.TCP) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                asyncWay = AsyncWay.Task;
            }

            public Client(Protocol protocol, AsyncWay asyncWay)
            {
                if (protocol == Protocol.TCP) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.asyncWay = asyncWay;
            }

            public Socket GetSocket()
            {
                return socket;
            }

            public void Send(string message, bool useExceptionList)
            {
                Task.Run(() =>
                {
                    byte[] messageBytes = Buffer.AddSplitter(Encoding.UTF8.GetBytes(message), 0);
                    try
                    {
                        socket.Send(messageBytes);
                        OnSend?.Invoke();
                    }
                    catch(Exception exception)
                    {
                        if (useExceptionList) CheckException(exception);
                        else OnSendException?.Invoke(exception);
                    }
                });
            }

            public void Connect<M>(string host, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                this.messageProcessing = messageProcessing;
                switch (asyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() =>
                        {
                            try
                            {
                                socket.Connect(host, port);
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
                                socket.Connect(host, port);
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

            public void Connect<M>(IPAddress ipAddress, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                this.messageProcessing = messageProcessing;
                switch (asyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() =>
                        {
                            try
                            {
                                socket.Connect(ipAddress, port);
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
                                socket.Connect(ipAddress, port);
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
         
            public void Connect(string host, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                this.messageProcessing = messageProcessing;
                switch (asyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() =>
                        {
                            try
                            {
                                socket.Connect(host, port);
                                OnConnect?.Invoke();
                                ListenServer<string>();
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
                                socket.Connect(host, port);
                                OnConnect?.Invoke();
                                ListenServer<string>();
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

            public void Connect(IPAddress ipAddress, int port, MessageProcessing messageProcessing, bool useExceptionList)
            {
                this.messageProcessing = messageProcessing;
                switch (asyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() =>
                        {
                            try
                            {
                                socket.Connect(ipAddress, port);
                                OnConnect?.Invoke();
                                ListenServer<string>();
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
                                socket.Connect(ipAddress, port);
                                OnConnect?.Invoke();
                                ListenServer<string>();
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
                            int messageSize = socket.Receive(messageBuffer);
                            fullMessageSize += messageSize;
                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (socket.Available > 0);
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
                            messageProcessing(new MessageData(message, fullMessageSize, fullMessage));
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
