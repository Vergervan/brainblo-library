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
            public event EventHandler<DisconnectEventArgs> OnDisconnect;
            public event EventHandler<ExceptionEventArgs> OnSendException;
            public event EventHandler<ExceptionEventArgs> OnReceiveException;
            public ExceptionList exceptionList = new ExceptionList();
            private List<Socket> clientList = new List<Socket>();

            public Server(Protocol protocol) : base(protocol) { }
            public Server(Protocol protocol, AsyncWay asyncWay) : base(protocol, asyncWay) { }

            public void Send(Socket client, byte[] messageBuffer)
            {
                Send(client, messageBuffer, false);
            }
            public void Send(Socket client, string messageString)
            {
                Send(client, Encoding.UTF8.GetBytes(messageString), false);
            }
            public void Send(Socket client, string messageString, bool useExceptionList)
            {
                Send(client, Encoding.UTF8.GetBytes(messageString), useExceptionList);
            }
            public void Send(Socket client, byte[] messageBuffer, bool useExceptionList)
            {
                Task.Run(() => SendAsync(client, messageBuffer, useExceptionList));
            }

            private void SendAsync(Socket client, byte[] messageBuffer, bool useExceptionList)
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
                    else OnSendException?.Invoke(this, new ExceptionEventArgs(client, exception));
                }
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
                    Socket socket = Socket.Accept();
                    NewClient<M>(socket);
                }
            }
            private void NewClient<M>(Socket socket)
            {
                switch (AsyncWay)
                {
                    case AsyncWay.Task:
                        Task task = new Task(() => ClientHandler<M>(socket));
                        task.Start();
                        break;
                    case AsyncWay.Thread:
                        Thread thread = new Thread(() => ClientHandler<M>(socket));
                        thread.Start();
                        break;
                }
            }
            private void ClientHandler<M>(Socket clientSocket)
            {
                clientList.Add(clientSocket);
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
                        OnReceiveException?.Invoke(this, new ExceptionEventArgs(clientSocket, exception));
                        break;
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
            public Socket[] GetClientList()
            {
                return clientList.ToArray();
            }

            public void CheckIsConnected()
            {
                foreach (var socket in clientList.ToArray())
                {
                    if (!IsConnected(socket))
                    {
                        clientList.Remove(socket);
                        OnDisconnect(this, new DisconnectEventArgs(socket));
                    }
                }
            }

            private bool IsConnected(Socket socket)
            {
                if(socket.Poll(1000, SelectMode.SelectRead) && (socket.Available == 0))
                {
                    return false;
                }else if (!socket.Connected)
                {
                    return false;
                }
                return true;
            }
            private void CheckException(Exception exception)
            {
                exceptionList.InvokeExceptionProcess(exception);
            }
        }
    }
}
