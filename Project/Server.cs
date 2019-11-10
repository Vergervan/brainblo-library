using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace BrainBlo
{
    namespace Network
    {
        public class Server
        {
            private Socket socket { get; set; }
            public ThreadType threadType { get; private set; }
            private MessageProcessing messageProcessing { get; set; }
            public event StartProcessing OnServerStart;
            public event AcceptProcessing OnServerAccept;
            public event SendProcessing OnSend;
            public event ExceptionProcessing OnSendException;
            public event ExceptionProcessing OnListenClientException;
            public ExceptionList exceptionList = new ExceptionList();

            public Server(Protocol protocol)
            {
                if (protocol == Protocol.TCP) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                threadType = ThreadType.Task;
            }

            public Server(Protocol protocol, ThreadType threadType)
            {
                if (protocol == Protocol.TCP) socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.threadType = threadType;
            }

            public Socket GetSocket()
            {
                return socket;
            }

            public void Send(Socket client, byte[] messageBuffer, bool useExceptionList)
            {
                Task.Run(() =>
                {
                    byte[] messageBytes = Buffer.AddSplitter(messageBuffer, 0);
                    try
                    {
                        client.Send(messageBytes);
                    }
                    catch (Exception exception)
                    {
                        if (useExceptionList) CheckException(exception);
                        else OnSendException?.Invoke(exception);
                    }
                    OnSend?.Invoke();
                });
            }

            public void Start(string ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                socket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                ListenClients<string>();
                OnServerStart?.Invoke();
            }

            public void Start(IPAddress ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                socket.Bind(new IPEndPoint(ipAddress, port));
                ListenClients<string>();
                OnServerStart?.Invoke();
            }

            public void Start<M>(string ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                socket.Bind(new IPEndPoint(IPAddress.Parse(ipAddress), port));
                ListenClients<M>();
                OnServerStart?.Invoke();
            }

            public void Start<M>(IPAddress ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.messageProcessing = messageProcessing;
                socket.Bind(new IPEndPoint(ipAddress, port));
                ListenClients<M>();
                OnServerStart?.Invoke();
            }

            public void ListenClients<M>()
            {
                switch (threadType)
                {
                    case ThreadType.Task:
                        Task.Run(() => AcceptClients<M>());
                        break;
                    case ThreadType.Thread:
                        Thread thread = new Thread(() => AcceptClients<M>());
                        thread.Start();
                        break;
                }
            }

            public void ListenClients()
            {
                switch (threadType)
                {
                    case ThreadType.Task:
                        Task.Run(() => AcceptClients<string>());
                        break;
                    case ThreadType.Thread:
                        Thread thread = new Thread(() => AcceptClients<string>());
                        thread.Start();
                        break;
                }
            }

            private void AcceptClients<M>()
            {
                
                socket.Listen(0);
                while (true)
                {
                    switch (threadType)
                    {
                        case ThreadType.Task:
                            Task.Run(() => ClientHandler<M>(socket.Accept()));
                            break;
                        case ThreadType.Thread:
                            Thread thread = new Thread(() => ClientHandler<M>(socket.Accept()));
                            thread.Start();
                            break;
                    }
                }
            }

            private void ClientHandler<M>(Socket clientSocket)
            {
                OnServerAccept?.Invoke(clientSocket);
                int messageSize = 0;
                string fullMessage = string.Empty;
                byte[] messageBuffer = new byte[1024];
                try
                {
                    while (true)
                    {
                        do
                        {
                            messageSize = clientSocket.Receive(messageBuffer);

                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (clientSocket.Available > 0);

                        List<ByteArray> byteArrays = Buffer.SplitBuffer(Encoding.UTF8.GetBytes(fullMessage), 0);

                        object message = default;
                        lock (byteArrays)
                        {
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
                                messageProcessing?.Invoke(new MessageInfo(message, messageSize, messageBuffer, fullMessage));
                            }
                        }
                        fullMessage = string.Empty;
                    }
                }
                catch (Exception exception)
                {
                    if (OnListenClientException != null) OnListenClientException(exception);
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
