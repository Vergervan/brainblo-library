using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BrainBlo;
using Buffer = BrainBlo.Buffer;

namespace ClientUDP
{
    class Program
    {
        static int maxBufferSize = 1024;
        static void Main(string[] args)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint ipend = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 25000);
            socket.Connect(ipend);
            Thread thread = new Thread(() => ListenServer(socket, ipend));
            thread.Start();
            while (true)
            {
                Console.ReadKey();
                Console.WriteLine("Сообщение отправлено");
                Send(new BloMessage(MessageType.Debug, "Тестовое сообщение от клиента"), socket, ipend);
            }
        }

        static void ListenServer(Socket socket, IPEndPoint ipend)
        {
            EndPoint ep = ipend;
            byte[] container = new byte[] { };
            int length = 0;
            while (true)
            {
                byte[] fullBuffer = new byte[maxBufferSize];
                int messageSize = socket.ReceiveFrom(fullBuffer, ref ep);
                byte[] buffer = Buffer.ChangeBufferSize(fullBuffer, 0, messageSize);
                byte code = buffer[0];
                if (code == 0)
                {
                    length = int.Parse($"{buffer[1]}{buffer[2]}{buffer[3]}{buffer[4]}");
                    container = new byte[] { };
                }
                else if(length != 0 && code == 1)
                {
                    container = Buffer.CombineBuffers(container, 0, buffer, 1, buffer.Length);
                    length -= buffer.Length-1;
                    if (length == 0)
                    {
                        MessageProcess(container, socket, ipend);
                        container = new byte[] { };
                    }
                }
            }
        }
        static void MessageProcess(byte[] buffer, Socket socket, IPEndPoint ipend)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(buffer, 0, buffer.Length);
                ms.Position = 0;
                BloMessage bm = (BloMessage)bf.Deserialize(ms);
                Console.WriteLine(bm.Message);
            }
        }

        static void Send(object obj, Socket socket, IPEndPoint ipend)
        {
            byte[] message = ToBinary(obj);
            int bufferSize = message.Length;
            int lastIndex = 0;
            byte[] byteLength = new byte[4];
            string str = string.Format($"{message.Length}");
            for(int i = byteLength.Length-str.Length, j = 0; i < byteLength.Length; i++, j++)
            {
                byteLength[i] = (byte)int.Parse($"{str[j]}");
            }
            socket.SendTo(Buffer.AddStartCode(byteLength, 0), ipend);

            while (bufferSize > 0)
            {
                byte[] messagePartBuffer = Buffer.ChangeBufferSize(message, lastIndex, bufferSize < maxBufferSize ? bufferSize : maxBufferSize - 1);
                socket.SendTo(Buffer.AddStartCode(messagePartBuffer, 1), ipend);
                bufferSize -= maxBufferSize-1;
                lastIndex += maxBufferSize-1;
            }

        }

        static byte[] ToBinary(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
