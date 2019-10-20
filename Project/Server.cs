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
        public enum Protocol
        {
            TCP = 0,
        }

        public enum ThreadType
        {
            Thread = 0,
            Task = 1
        }

        public delegate void MessageProcessing(params object[] o);

        public class Server
        {
            private Socket socket { get; set; }
            public ThreadType threadType { get; private set; }
            private MessageProcessing messageProcessing { get; set; }

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

            public void Send(Socket client, byte[] messageBuffer)
            {
                byte[] messageBytes = Buffer.AddSplitter(messageBuffer, 0);
                client.Send(messageBytes);
            }

            public void Start(string ipAddress, int port)
            {
                socket.Bind(new IPEndPoint(long.Parse(ipAddress), port));
            }

            public void Start(IPAddress ipAddress, int port)
            {
                socket.Bind(new IPEndPoint(ipAddress, port));
            }

            public void ListenClients<M>(MessageProcessing messageProcessing)
            {
                switch (threadType)
                {
                    case ThreadType.Task:
                        Task.Run(() => AcceptClients<M>(messageProcessing));
                        break;
                    case ThreadType.Thread:
                        Thread thread = new Thread(() => AcceptClients<M>(messageProcessing));
                        thread.Start();
                        break;
                }
              
            }

            public void ListenClients(MessageProcessing messageProcessing)
            {
                switch (threadType)
                {
                    case ThreadType.Task:
                        Task.Run(() => AcceptClients<string>(messageProcessing));
                        break;
                    case ThreadType.Thread:
                        Thread thread = new Thread(() => AcceptClients<string>(messageProcessing));
                        thread.Start();
                        break;
                }

            }

            private void AcceptClients<M>(MessageProcessing messageProcessing)
            {

                socket.Listen(0);
                while (true)
                {
                    switch (threadType)
                    {
                        case ThreadType.Task:
                            Task.Run(() => ClientHandler<M>(socket.Accept(), messageProcessing));
                            break;
                        case ThreadType.Thread:
                            Thread thread = new Thread(() => ClientHandler<M>(socket.Accept(), messageProcessing));
                            thread.Start();
                            break;
                    }
                }
            }

            private void ClientHandler<M>(Socket clientSocket, MessageProcessing messageProcessing)
            {
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
                                messageProcessing(message, messageBuffer, fullMessage, messageSize);
                            }
                        }
                        fullMessage = string.Empty;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine("CH:" + e.Message);
                }
            }
        }
    }
}
