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
        namespace Debug 
        {
            //Not working properly!
            //It's better to refuse to use this!
            //TODO Need to refine
            public class ServerTest 
            {
                public IPEndPoint address;
                private byte[] messageBuffer;
                public event EventHandler OnConnect;
                public event EventHandler OnSend;
                public event EventHandler<ExceptionEventArgs> OnConnectException;
                public event EventHandler<ExceptionEventArgs> OnSendException;

                public ServerTest(EndPoint endPoint)
                {
                    address = (IPEndPoint)endPoint;
                }

                public void SetMessage(byte[] message)
                {
                    messageBuffer = message;
                }

                public void StartTest(int clientsCount, int millisecondsDelay)
                {
                    for(int i = 0; i < clientsCount; i++)
                    {
                        using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {
                            Task.Run(() => NewClient(socket, millisecondsDelay));
                            Thread.Sleep(10);
                        }
                    }
                }

                private async void NewClient(Socket socket, int millisecondsDelay)
                {
                    try
                    {
                        socket.Connect(address);
                        OnConnect?.Invoke(this, new EventArgs());
                    }catch(Exception exception)
                    {
                        OnConnectException?.Invoke(this, new ExceptionEventArgs(socket, exception));
                    }
                    while (true)
                    {
                        try
                        {
                            socket.Send(messageBuffer);
                            OnSend?.Invoke(this, new EventArgs());
                        }catch(Exception exception)
                        {
                            OnSendException?.Invoke(this, new ExceptionEventArgs(socket, exception));
                        }
                        await Task.Delay(millisecondsDelay);
                    }
                }
            }
        }
    }
}
