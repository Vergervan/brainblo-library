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
        public class Server : NetworkObject
        {
            public event EventHandler OnStart;
            public event EventHandler<AcceptEventArgs> OnAccept;
            public event EventHandler OnSend;
            public event EventHandler OnReceive;
            public event EventHandler<MessageProcessEventArgs> MessageProcessing;
            public event EventHandler<ExceptionEventArgs> OnSendException;
            public event EventHandler<ExceptionEventArgs> OnReceiveException;
            public ExceptionList exceptionList = new ExceptionList();

            public Server(Protocol protocol) : base(protocol) { }
            public Server(Protocol protocol, AsyncWay asyncWay) : base(protocol, asyncWay) { }

            public void Send(Socket client, byte[] messageBuffer)
            {
                Send(client, messageBuffer, false);
            }
            public void Send(Socket client, byte[] messageBuffer, bool useExceptionList)
            {
                Task.Run(() =>
                {
                    byte[] messageBytes = Buffer.AddSplitter(messageBuffer, 0);
                    try
                    {
                        client.Send(messageBytes);
                        OnSend?.Invoke(this, new EventArgs());
                    }
                    catch (Exception exception)
                    {
                        if (useExceptionList) CheckException(exception);
                        else OnSendException?.Invoke(this, new ExceptionEventArgs(exception));
                    }
                });
            }

            public void Start(string ipAddress, int port)
            {
                Start<string>(IPAddress.Parse(ipAddress), port);
            }
            public void Start(IPAddress ipAddress, int port)
            {
                Start<string>(ipAddress, port);
            }
            public void Start<M>(string ipAddress, int port)
            {
                Start<M>(IPAddress.Parse(ipAddress), port);
            }
            public void Start<M>(IPAddress ipAddress, int port)
            {
                Socket.Bind(new IPEndPoint(ipAddress, port));
                ListenClients<M>();
            }

            private void ListenClients<M>()
            {
                IsWorking = true;
                OnStart?.Invoke(this, new EventArgs());
                switch (AsyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() => AcceptClients<M>());
                        break;
                    case AsyncWay.Thread:
                        Thread thread = new Thread(() => AcceptClients<M>());
                        thread.Start();
                        break;
                }
            }

            private void AcceptClients<M>()
            {        
                Socket.Listen(0);
                while (true)
                {
                    switch (AsyncWay)
                    {
                        case AsyncWay.Task:
                            Task.Run(() => ClientHandler<M>(Socket.Accept()));
                            break;
                        case AsyncWay.Thread:
                            Thread thread = new Thread(() => ClientHandler<M>(Socket.Accept()));
                            thread.Start();
                            break;
                    }
                }
            }

            private void ClientHandler<M>(Socket clientSocket)
            {
                OnAccept?.Invoke(this, new AcceptEventArgs(clientSocket));
                int fullMessageSize;
                string fullMessage;
                byte[] messageBuffer = new byte[1024];
                while (true)
                {
                    fullMessage = string.Empty;
                    fullMessageSize = 0;
                    try
                    {
                        do
                        {
                            int messageSize = clientSocket.Receive(messageBuffer);
                            fullMessageSize += messageSize;
                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (clientSocket.Available > 0);
                        OnReceive?.Invoke(this, new EventArgs());
                    }
                    catch (Exception exception)
                    {
                        OnReceiveException?.Invoke(this, new ExceptionEventArgs(exception));
                    }

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
                        MessageProcessing?.Invoke(this, new MessageProcessEventArgs(new MessageData(message, fullMessageSize, fullMessage)));
                    }
                }
            }

            private void CheckException(Exception exception)
            {
                exceptionList.InvokeExceptionProcess(exception);
            }
        }
    }
}
