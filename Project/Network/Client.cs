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
        public class Client : NetworkObject
        {
            public event EventHandler OnConnect;
            public event EventHandler OnSend;
            public event EventHandler OnReceive;
            public event EventHandler<MessageProcessEventArgs> MessageProcessing;
            public event EventHandler<ExceptionEventArgs> OnConnectException;
            public event EventHandler<ExceptionEventArgs> OnSendException;
            public event EventHandler<ExceptionEventArgs> OnReceiveException;
            public ExceptionList exceptionList = new ExceptionList();


            public Client(Protocol protocol) : base(protocol) { }

            public Client(Protocol protocol, AsyncWay asyncWay) : base(protocol, asyncWay) { }

            public void Send(string message, bool useExceptionList)
            {
                Task.Run(() =>
                {
                    byte[] messageBytes = Buffer.AddSplitter(Encoding.UTF8.GetBytes(message), 0);
                    try
                    {
                        Socket.Send(messageBytes);
                        OnSend?.Invoke(this, new EventArgs());
                    }
                    catch(Exception exception)
                    {
                        if (useExceptionList) CheckException(exception);
                        else OnSendException?.Invoke(this, new ExceptionEventArgs(exception));
                    }
                });
            }
         
            public void Connect(string host, int port, bool useExceptionList)
            {
                Connect<string>(IPAddress.Parse(host), port, useExceptionList);
            }

            public void Connect(IPAddress ipAddress, int port, bool useExceptionList)
            {
                Connect<string>(ipAddress, port, useExceptionList);
            }
            public void Connect<M>(string host, int port, bool useExceptionList)
            {
                Connect<M>(IPAddress.Parse(host), port, useExceptionList);
            }
            public void Connect<M>(IPAddress ipAddress, int port, bool useExceptionList)
            {
                switch (AsyncWay)
                {
                    case AsyncWay.Task:
                        Task.Run(() => Connecting<M>(ipAddress, port, useExceptionList));
                        break;
                    case AsyncWay.Thread:
                        Thread thread = new Thread(() => Connecting<M>(ipAddress, port, useExceptionList));
                        thread.Start();
                        break;
                }
            }

            private void Connecting<M>(IPAddress ipAddress, int port, bool useExceptionList)
            {
                try
                {
                    Socket.Connect(ipAddress, port);
                    IsWorking = true;
                    OnConnect?.Invoke(this, new EventArgs());
                    ListenServer<M>();
                }
                catch (Exception exception)
                {
                    if (useExceptionList) CheckException(exception);
                    else OnConnectException?.Invoke(this, new ExceptionEventArgs(exception));
                }
            }

            private void ListenServer<M>()
            {
                int fullMessageSize;
                string fullMessage;
                byte[] messageBuffer = new byte[1024];
                while (true)
                {
                    fullMessageSize = 0;
                    fullMessage = string.Empty;
                    try
                    {
                        do
                        {
                            int messageSize = Socket.Receive(messageBuffer);
                            fullMessageSize += messageSize;
                            fullMessage += Encoding.UTF8.GetString(messageBuffer, 0, messageSize);
                        } while (Socket.Available > 0);
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
