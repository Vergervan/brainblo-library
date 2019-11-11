using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace BrainBlo
{
    namespace Network
    {
        public delegate void MessageProcessing(MessageData messageData);
        public delegate void StartProcessing();
        public delegate void AcceptProcessing(Socket socket);
        public delegate void ConnectProcessing();
        public delegate void SendProcessing();
        public delegate void ExceptionProcessing(Exception exception);

        public enum Protocol
        {
            TCP = 0,
        }
        public enum AsyncWay
        {
            Thread = 0,
            Task = 1
        }

        public interface INetworkObject
        {
            void NetworkEventDefinitions();
        }

        public class MessageData
        {
            public object messageObject { get; private set; }
            public int messageSize { get; private set; }
            public string messageString { get; private set; }

            public MessageData(object messageObject, int messageSize, string messageString)
            {
                this.messageObject = messageObject;
                this.messageSize = messageSize;
                this.messageString = messageString;
            }
        }

        public struct ExceptionCell
        {
            public Type exception;
            public ExceptionProcessing exceptionProcessing; 

            public ExceptionCell(Type exception, ExceptionProcessing exceptionProcessing)
            {
                this.exception = exception;
                this.exceptionProcessing = exceptionProcessing;
            }
        }
        public class ExceptionListException : Exception
        {
            public ExceptionListException() { }
            public ExceptionListException(string message) : base(message) { }
        }

        public class ExceptionList
        {
            ExceptionCell[] exceptionArray;

            public ExceptionList()
            {
                exceptionArray = new ExceptionCell[1];
                exceptionArray[0] = new ExceptionCell { exception = typeof(Exception), exceptionProcessing = null };
            }

            public ExceptionList(ExceptionProcessing defaultExceptionProcessing)
            {
                exceptionArray = new ExceptionCell[1];
                exceptionArray[0] = new ExceptionCell { exception = typeof(Exception), exceptionProcessing = defaultExceptionProcessing };
            }

            public void SetDefaultProcess(ExceptionProcessing exceptionProcessing)
            {
                exceptionArray[0].exceptionProcessing = exceptionProcessing;
            }

            public void Add(Type exceptionType, ExceptionProcessing exceptionProcessing)
            {
                ExceptionCell[] newExceptionArray;

                if (exceptionArray.Length > 0)
                {
                    foreach (var exception_for in exceptionArray)
                    {
                        if (exceptionType == exception_for.exception.GetType()) throw new ExceptionListException("This type of exception already in the exception list");
                    }

                    newExceptionArray = new ExceptionCell[exceptionArray.Length + 1];
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        newExceptionArray[i] = exceptionArray[i];
                    }

                    newExceptionArray[exceptionArray.Length] = new ExceptionCell(exceptionType, exceptionProcessing);

                    exceptionArray = newExceptionArray;
                    return;
                }
                newExceptionArray = new ExceptionCell[1];
                newExceptionArray[0] = new ExceptionCell(exceptionType, exceptionProcessing);
                exceptionArray = newExceptionArray;
            }

            public bool Remove(Type exceptionType)
            {
                if (exceptionArray.Length > 1)
                {
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        if (exceptionType == exceptionArray[i].exception)
                        {
                            ExceptionCell[] newExceptionArray = new ExceptionCell[exceptionArray.Length - 1];
                            for (int j = 0, k = 0; j < exceptionArray.Length; j++, k++)
                            {
                                if (j != i)
                                {
                                    if (k != exceptionArray.Length - 1)
                                    {
                                        newExceptionArray[k] = exceptionArray[j];
                                    }
                                    exceptionArray = newExceptionArray;
                                    return true;
                                }
                            }
                        }
                    }
                }
                else if(exceptionArray.Length == 1)
                {
                    if(exceptionType == exceptionArray[0].exception)
                    {
                        throw new ExceptionListException("You can not remove a default exception");
                    }
                }
                return false;
            }

            public bool FindException(Exception exception)
            {
                if (exceptionArray.Length > 0)
                {
                    foreach (var exception_foreach in exceptionArray)
                    {
                        if (exception.GetType() == exception_foreach.exception) return true;
                    }
                }
                return false;
            }

            public void InvokeExceptionProcess(Exception exception)
            {
                if (exceptionArray.Length > 0)
                {
                    for(int i = 0; i<exceptionArray.Length; i++)
                    {
                        if(i == exceptionArray.Length-1 && exception.GetType() != exceptionArray[i].exception)
                        {
                            exceptionArray[0].exceptionProcessing?.Invoke(exception);
                            return;
                        }

                        if(exception.GetType() == exceptionArray[i].exception)
                        {
                            exceptionArray[i].exceptionProcessing?.Invoke(exception);
                            return;
                        }
                    }
                }
            }

            public ExceptionCell this[int index]
            {
                get
                {
                    return exceptionArray[index];
                }

                set
                {
                    exceptionArray[index] = value;
                }
            }

            public int Length
            {
                get
                {
                    return exceptionArray.Length;
                }
            }

            public ExceptionCell[] ToArray()
            {
                return exceptionArray;
            }
        }
    }
}
