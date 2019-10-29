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
        public delegate void MessageProcessing(MessageInfo messageInfo);
        public delegate void StartProcessing();
        public delegate void ListenProcessing();
        public delegate void AcceptProcessing(Socket socket);
        public delegate void ConnectProcessing();
        public delegate void ExceptionProcessing();

        public enum Protocol
        {
            TCP = 0,
        }
        public enum ThreadType
        {
            Thread = 0,
            Task = 1
        }

        public class MessageInfo
        {
            public object message { get; private set; }
            public int messageSize { get; private set; }
            public byte[] messageBuffer { get; private set; }
            public string fullMessage { get; private set; }

            public MessageInfo(object message, int messageSize, byte[] messageBuffer, string fullMessage)
            {
                this.message = message;
                this.messageSize = messageSize;
                this.messageBuffer = messageBuffer;
                this.fullMessage = fullMessage;
            }
        }

        public struct ExceptionCell
        {
            public Exception exception;
            public ExceptionProcessing exceptionProcessing; 

            public ExceptionCell(Exception exception)
            {
                this.exception = exception;
                this.exceptionProcessing = null;
            }

            public ExceptionCell(Exception exception, ExceptionProcessing exceptionProcessing)
            {
                this.exception = exception;
                this.exceptionProcessing = exceptionProcessing;
            }
        }

        public class AlreadyInTheListException : Exception
        {
            public new string Message { get; private set; }
            public AlreadyInTheListException() { }
            public AlreadyInTheListException(string Message)
            {
                this.Message = Message;
            }
        }

        public class ExceptionList
        {
            ExceptionCell[] exceptionArray = new ExceptionCell[0];
            

            public void Add(Exception exception)
            {
                ExceptionCell[] newExceptionArray;

                foreach(var exception_for in exceptionArray)
                {
                    if (exception.GetType() == exception_for.exception.GetType()) throw new AlreadyInTheListException("This type of exception already in the exception list");
                }

                if (exceptionArray.Length > 0)
                {
                    newExceptionArray = new ExceptionCell[exceptionArray.Length + 1];
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        newExceptionArray[i] = exceptionArray[i];
                    }

                    newExceptionArray[exceptionArray.Length] = new ExceptionCell(exception);

                    exceptionArray = newExceptionArray;
                    return;
                }
                newExceptionArray = new ExceptionCell[1];
                newExceptionArray[0] = new ExceptionCell(exception);
                exceptionArray = newExceptionArray;
            }

            public void Add(Exception exception, ExceptionProcessing exceptionProcessing)
            {
                ExceptionCell[] newExceptionArray;

                if (exceptionArray.Length > 0)
                {
                    foreach (var exception_for in exceptionArray)
                    {
                        if (exception.GetType() == exception_for.exception.GetType()) throw new AlreadyInTheListException("This type of exception already in the exception list");
                    }

                    newExceptionArray = new ExceptionCell[exceptionArray.Length + 1];
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        newExceptionArray[i] = exceptionArray[i];
                    }

                    newExceptionArray[exceptionArray.Length] = new ExceptionCell(exception, exceptionProcessing);

                    exceptionArray = newExceptionArray;
                    return;
                }
                newExceptionArray = new ExceptionCell[1];
                newExceptionArray[0] = new ExceptionCell(exception, exceptionProcessing);
                exceptionArray = newExceptionArray;
            }

            public bool Remove(Exception exception)
            {
                if (exceptionArray.Length > 1)
                {
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        if (exception.GetType() == exceptionArray[i].exception.GetType())
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
                    if(exception.GetType() == exceptionArray[0].exception.GetType())
                    {
                        exceptionArray = new ExceptionCell[0];
                        return true;
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
                        if (exception.GetType() == exception_foreach.exception.GetType()) return true;
                    }
                }
                return false;
            }

            public void FindAndInvokeException(Exception exception)
            {
                if (exceptionArray.Length > 0)
                {
                    foreach (var exception_foreach in exceptionArray)
                    {
                        if (exception.GetType() == exception_foreach.exception.GetType())
                        {
                            exception_foreach.exceptionProcessing?.Invoke();
                            break;
                        }
                    }
                }
            }

            public Exception this[int index]
            {
                get
                {
                    return exceptionArray[index].exception;
                }

                set
                {
                    exceptionArray[index].exception = value;
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
