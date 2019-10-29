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
            public ThreadType threadType { get; private set; }
            private MessageProcessing messageProcessing { get; set; }
            public event ConnectProcessing OnConnect;
            public ExceptionList exceptionList = new ExceptionList();

            
            public Client(Protocol protocol)
            {
                if (protocol == Protocol.TCP) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                threadType = ThreadType.Task;
            }

            public Client(Protocol protocol, ThreadType threadType)
            {
                if (protocol == Protocol.TCP) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.threadType = threadType;
            }

            public Socket GetSocket()
            {
                return socket;
            }

            public void Send(byte[] messageBuffer)
            {
                byte[] messageBytes = Buffer.AddSplitter(messageBuffer, 0);
                try
                {
                    socket.Send(messageBytes);
                }catch(Exception e)
                {
                    CheckException(e);
                }
            }

            public void Connect<M>(string host, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                try
                {
                    switch (threadType)
                    {
                        case ThreadType.Task:
                            Task.Run(() =>
                            {
                                socket.Connect(host, port);
                                ListenServer<M>();
                            });
                            break;
                        case ThreadType.Thread:
                            Thread thread = new Thread(() =>
                            {
                                socket.Connect(host, port);
                                ListenServer<M>();
                            });
                            thread.Start();
                            break;

                    }
                    OnConnect?.Invoke();
                }
                catch(Exception e)
                {
                    CheckException(e);
                }
            }

            public void Connect<M>(IPAddress ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                try
                {
                    switch (threadType)
                    {
                        case ThreadType.Task:
                            Task.Run(() =>
                            {
                                socket.Connect(ipAddress, port);
                                ListenServer<M>();
                            });
                            break;
                        case ThreadType.Thread:
                            Thread thread = new Thread(() =>
                            {
                                socket.Connect(ipAddress, port);
                                ListenServer<M>();
                            });
                            thread.Start();
                            break;
                    }
                    OnConnect?.Invoke();
                }catch(Exception e)
                {
                    CheckException(e);
                }
            }
            public void Connect(string host, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                try
                {
                    switch (threadType)
                    {
                        case ThreadType.Task:
                            Task.Run(() =>
                            {
                                socket.Connect(host, port);
                                ListenServer<string>();
                            });
                            break;
                        case ThreadType.Thread:
                            Thread thread = new Thread(() =>
                            {
                                socket.Connect(host, port);
                                ListenServer<string>();
                            });
                            thread.Start();
                            break;
                    }
                    OnConnect?.Invoke();
                }
                catch(Exception e)
                {
                    CheckException(e);
                }
            }

            public void Connect(IPAddress ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                try
                {
                    switch (threadType)
                    {
                        case ThreadType.Task:
                            Task.Run(() =>
                            {
                                socket.Connect(ipAddress, port);
                                ListenServer<string>();
                            });
                            break;
                        case ThreadType.Thread:
                            Thread thread = new Thread(() =>
                            {
                                socket.Connect(ipAddress, port);
                                ListenServer<string>();
                            });
                            thread.Start();
                            break;
                    }
                    OnConnect?.Invoke();
                }
                catch(Exception e)
                {
                    CheckException(e);
                }
            }

            private void ListenServer<M>()
            {
                try
                {
                    while (true)
                    {
                        int messageSize;
                        string fullMessage = string.Empty;
                        byte[] messageBuffer = new byte[1024];
                        do
                        {
                            messageSize = socket.Receive(messageBuffer);
                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (socket.Available > 0);
                        List<ByteArray> byteArrays = Buffer.SplitBuffer(Encoding.UTF8.GetBytes(fullMessage), 0);
                        lock (byteArrays)
                        {
                            foreach (var c in byteArrays)
                            {
                                object message = default;
                                if (typeof(M) != typeof(string))
                                {
                                    message = Utils.DeserializeJson<M>(Encoding.UTF8.GetString(c.bytes));
                                }
                                else
                                {
                                    message = Encoding.UTF8.GetString(c.bytes);
                                }
                                messageProcessing(new MessageInfo(message, messageSize, messageBuffer, fullMessage));
                            }

                        }
                        fullMessage = string.Empty;
                    }
                }catch(Exception e)
                {
                    CheckException(e);
                }
            }

            private void CheckException(Exception exception)
            {
                exceptionList.FindAndInvokeException(exception);
            }
        }
    }
}
