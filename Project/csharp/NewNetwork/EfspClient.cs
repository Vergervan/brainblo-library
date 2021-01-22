using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BrainBlo.NewNetwork
{
    public class EfspClient
    {
        private Socket _socket;
        public EfspClient()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        private void Connect(string host, int port)
        {
            _socket.Connect(Dns.GetHostAddresses(host)[0], port);
        }
        public void SendFile(string path, string host, int port) => SendFile(path, host, port, true, FileFlags.SingleFile);
        public void SendFile(string[] path, string host, int port)
        {
            bool connectOnce = true;
            for(int i = 0; i < path.Length; i++)
            {
                //string truePath = Path.GetDirectoryName(path[i]);
                SendFile(path[i], host, port, connectOnce, FileFlags.MultipleFiles);
                if (connectOnce) connectOnce = false;
            }
        }
        //public void SendFile(string path, string host, int port) => SendFile(path, host, port, true, FileFlags.SingleFile, 1);
        private void SendFile(string path, string host, int port, bool connectOnce, FileFlags flag)
        {
            if (connectOnce) {
                try
                {
                    Connect(host, port);
                } catch (Exception) { return; }
            }
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (NetworkStream ns = new NetworkStream(_socket))
                {
                    using (BinaryWriter bw = new BinaryWriter(ns, Encoding.UTF8, true))
                    {
                        //Packet 1
                        if (connectOnce)
                        {
                            //Part 1 - Flag
                            bw.Write((short)flag);
                            //Additional Part 2 - Count of files
                            if (flag == FileFlags.MultipleFiles)
                            {
                                bw.Write(path.Length);
                            }
                        }
                        //Packet 2
                        string fileName = Path.GetFileName(path);
                        byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                        //Part 1 - File name bytes length
                        bw.Write(fileNameBytes.Length);
                        //Part 2 - File name bytes
                        bw.Write(fileNameBytes);
                        //Packet 3
                        //Part 1 - Length of file
                        bw.Write((int)fs.Length);   
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //Part 2 - File bytes
                        fs.CopyTo(ms);
                        byte[] fileBytes = ms.ToArray();
                        ns.Write(fileBytes, 0, fileBytes.Length);
                    }
                }
            }
        }
        ~EfspClient()
        {
            _socket.Close();
            _socket.Dispose();
        }
    }
}
