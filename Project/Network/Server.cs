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
            public Socket Socket { get; private set; }
            public AsyncWay AsyncWay { get; private set; }
            private MessageProcessing MessageProcessing { get; set; }
            public event StartProcessing OnServerStart;
            public event AcceptProcessing OnServerAccept;
            public event SendProcessing OnSend;
            public event ReceiveProcessing OnReceive;
            public event ExceptionProcessing OnSendException;
            public event ExceptionProcessing OnListenClientException;
            public ExceptionList exceptionList = new ExceptionList();

            public Server(Protocol protocol) : this(protocol, AsyncWay.Task) { }

            public Server(Protocol protocol, AsyncWay asyncWay)
            {
                if (protocol == Protocol.TCP) Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.AsyncWay = asyncWay;
            }

            public void Send(Socket client, byte[] messageBuffer, bool useExceptionList)
            {
                Task.Run(() =>
                {
                    byte[] messageBytes = Buffer.AddSplitter(messageBuffer, 0);
                    try
                    {
                        client.Send(messageBytes);
                        OnSend?.Invoke();
                    }
                    catch (Exception exception)
                    {
                        if (useExceptionList) CheckException(exception);
                        else OnSendException?.Invoke(exception);
                    }
                });
            }

            public void Start(string ipAddress, int port, MessageProcessing messageProcessing)
            {
                Start<string>(IPAddress.Parse(ipAddress), port, messageProcessing);
            }

            public void Start(IPAddress ipAddress, int port, MessageProcessing messageProcessing)
            {
                Start<string>(ipAddress, port, messageProcessing);
            }

            public void Start<M>(string ipAddress, int port, MessageProcessing messageProcessing)
            {
                Start<M>(IPAddress.Parse(ipAddress), port, messageProcessing);
            }

            public void Start<M>(IPAddress ipAddress, int port, MessageProcessing messageProcessing)
            {
                this.MessageProcessing = messageProcessing;
                Socket.Bind(new IPEndPoint(ipAddress, port));
                ListenClients<M>();
            }

            private void ListenClients<M>()
            {
                OnServerStart?.Invoke();
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
                OnServerAccept?.Invoke(clientSocket);
                int fullMessageSize;
                string fullMessage;
                byte[] messageBuffer = new byte[1024];
                try
                {
                    while (true)
                    {
                        fullMessage = string.Empty;
                        fullMessageSize = 0;
                        do
                        {
                            int messageSize = clientSocket.Receive(messageBuffer);
                            fullMessageSize += messageSize;
                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (clientSocket.Available > 0);
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
                            MessageProcessing?.Invoke(new MessageData(message, fullMessageSize, fullMessage));
                        }
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
