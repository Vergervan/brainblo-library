using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrainBlo.NewNetwork
{
    public class EfspServer
    {
        private Socket _socket;
        private Thread listeningThread;
        private bool isRunning = false;
        public event EventHandler<FileReceiveEventArgs> OnFileReceive;
        public EfspServer()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start(int port)
        {
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(10);
            isRunning = true;
            listeningThread = new Thread(() => Listening());
            listeningThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            _socket.Close();
            _socket.Dispose();
        }

        private void Listening()
        {
            while (isRunning)
            {
                Socket client = _socket.Accept();
                Task.Factory.StartNew(() => ReceiveFile(client));
            }
        }
        private void ReceiveFile(Socket clientSocket)
        {
            FileFlags flag;
            int filesCount = 1;
            int fileNameBytesLength;
            string fileName;
            int fileSize = 0;
            try
            {
                using (NetworkStream ns = new NetworkStream(clientSocket))
                {
                    using (BinaryReader br = new BinaryReader(ns))
                    {
                        //Packet 1
                        //Part 1 - Flag;
                        flag = (FileFlags)br.ReadInt16();
                        //Additional Part 2 - Count of files
                        if (flag == FileFlags.MultipleFiles)
                        {
                            filesCount = br.ReadInt32();
                        }

                        for (int i = 0; i < filesCount; i++)
                        {
                            //Packet 2
                            //Part 1 - File name length
                            fileNameBytesLength = br.ReadInt32();
                            //Part 2 - File name
                            fileName = Encoding.UTF8.GetString(br.ReadBytes(fileNameBytesLength));

                            //Packet 3
                            fileSize = br.ReadInt32();
                            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                            {
                                byte[] fileBytes = br.ReadBytes(fileSize);
                                fs.Write(fileBytes, 0, fileBytes.Length);
                                OnFileReceive?.Invoke(this, new FileReceiveEventArgs(fileSize, fileName));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        ~EfspServer()
        {
            Stop();
        }
    }
}
