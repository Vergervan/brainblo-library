using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BrainBlo;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Buffer = BrainBlo.Buffer;


namespace ServerUDP
{
    class Program
    {
        static List<UDPUser> users = new List<UDPUser>();
        static Queue<UDPMessage> messages = new Queue<UDPMessage>();
        static int maxBufferSize = 1024;
        static private readonly object lockUsersList = new object();
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipend = new IPEndPoint(IPAddress.Any, 25000);
            socket.Bind(ipend);

            Console.WriteLine("Сервер запущен");
            Thread thread = new Thread(() => ClientListen(socket));
            thread.Start();
            Thread sendThread = new Thread(() => AutoSender(socket));
            sendThread.Start();

            while (true)
            {
                /*string str = Console.ReadLine();

                if (str == "users")
                {
                    Console.WriteLine(users.Count);
                }*/
                Console.ReadKey();
                lock (lockUsersList)
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        messages.Enqueue(new UDPMessage(users[i].UserIP, ToBinary(new BloMessage(MessageType.Debug, "Сообщение от сервера"))));
                    }
                }

            }
        }

        static void ClientListen(Socket socket)
        {
            while (true)
            {
                EndPoint clientEndPoint = new IPEndPoint(IPAddress.None, 25000);
                byte[] buffer = new byte[maxBufferSize];
                int messageSize = socket.ReceiveFrom(buffer, ref clientEndPoint);
                IPEndPoint cleanEP = (IPEndPoint)clientEndPoint;
                if (!UserInList(cleanEP))
                {
                    Console.WriteLine("Новый пользователь");
                    lock (lockUsersList) users.Add(new UDPUser(cleanEP));
                }
                byte[] messageBuffer = Buffer.ChangeBufferSize(buffer, 0, messageSize);
                for (int i = 0; i < users.Count; i++)
                {
                    lock (lockUsersList)
                    {
                        if (users[i].UserIP.ToString() == cleanEP.ToString())
                        {
                            byte code = buffer[0];
                            if (code == 0)
                            {
                                users[i].length = int.Parse($"{messageBuffer[1]}{messageBuffer[2]}{messageBuffer[3]}{messageBuffer[4]}");
                                users[i].messageBytes = new byte[] { };
                            }
                            else if (users[i].length != 0 && code == 1)
                            {
                                byte[] messagePart = Buffer.ChangeBufferSize(messageBuffer, 1, messageBuffer.Length-1);
                                users[i].messageBytes = Buffer.CombineBuffers(users[i].messageBytes, messagePart);
                                users[i].length -= messageBuffer.Length-1;
                                if (users[i].length == 0)
                                {
                                    MessageProcess(users[i].messageBytes, socket, cleanEP);
                                    users[i].messageBytes = new byte[] { };
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }

        static void AutoSender(Socket socket)
        {
            while (true)
            {
                if (messages.Count != 0)
                {
                    UDPMessage udpm = messages.Dequeue();
                    int bufferSize = udpm.message.Length;
                    int lastIndex = 0;

                    byte[] byteLength = new byte[4];
                    string str = string.Format($"{udpm.message.Length}");
                    for (int i = byteLength.Length - str.Length, j = 0; i < byteLength.Length; i++, j++)
                    {
                        byteLength[i] = (byte)int.Parse($"{str[j]}");
                    }
                    socket.SendTo(Buffer.AddStartCode(byteLength, 0), udpm.ipend);

                    while (bufferSize > 0)
                    {
                        socket.SendTo(Buffer.AddStartCode(Buffer.ChangeBufferSize(udpm.message, lastIndex, bufferSize < maxBufferSize ? bufferSize : maxBufferSize-1), 1), udpm.ipend);
                        bufferSize -= maxBufferSize-1;
                        lastIndex += maxBufferSize-1;
                    }
                    Console.WriteLine("Сообщение отправлено");
                }
            }
        }

        static bool UserInList(IPEndPoint ipend)
        {
            for (int i = 0; i < users.Count; i++)
            {
                lock (lockUsersList)
                    if (ipend.ToString() == users[i].UserIP.ToString()) return true;
            }
            return false;
        }

        static byte[] ToBinary(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        static void MessageProcess(byte[] buffer, Socket socket, IPEndPoint ipend)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                BloMessage bm = (BloMessage)bf.Deserialize(ms);
                Console.WriteLine(bm.Message);
            }
        }
    }
}
