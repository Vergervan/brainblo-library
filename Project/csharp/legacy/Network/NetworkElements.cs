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
        public delegate void ExceptionAction(Exception exception);
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

        public struct MessageData
        {
            public object MessageObject { get; private set; }
            public int MessageSize { get; private set; }
            public string MessageString { get; private set; }

            public MessageData(object messageObject, int messageSize, string messageString)
            {
                MessageObject = messageObject;
                MessageSize = messageSize;
                MessageString = messageString;
            }
        }
        public struct ExceptionCell
        {
            public Type ExceptionType { get; set; }
            public ExceptionAction ExceptionAction { get; set; }
            public ExceptionCell(Type exceptionType) : this(exceptionType, null) { }
            public ExceptionCell(Type exceptionType, ExceptionAction exceptionAction)
            {
                ExceptionType = exceptionType;
                ExceptionAction = exceptionAction;
            }
        }

        public class ExceptionList
        {
            ExceptionCell[] exceptionArray;

            public ExceptionList() : this(null) {}

            public ExceptionList(ExceptionAction defaultExceptionAction)
            {
                exceptionArray = new ExceptionCell[1];
                exceptionArray[0] = new ExceptionCell { ExceptionType = typeof(Exception), ExceptionAction = defaultExceptionAction };
            }

            public void SetDefaultProcess(ExceptionAction exceptionAction)
            {
                exceptionArray[0].ExceptionAction = exceptionAction;
            }

            public void Add(Type exceptionType, ExceptionAction exceptionAction)
            {
                ExceptionCell[] newExceptionArray;

                if (exceptionArray.Length > 0)
                {
                    foreach (var exception_for in exceptionArray)
                    {
                        if (exceptionType == exception_for.ExceptionType) throw new ExceptionListException("This type of exception already in the exception list");
                    }

                    newExceptionArray = new ExceptionCell[exceptionArray.Length + 1];
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        newExceptionArray[i] = exceptionArray[i];
                    }

                    newExceptionArray[exceptionArray.Length] = new ExceptionCell(exceptionType, exceptionAction);

                    exceptionArray = newExceptionArray;
                    return;
                }
                newExceptionArray = new ExceptionCell[1];
                newExceptionArray[0] = new ExceptionCell(exceptionType, exceptionAction);
                exceptionArray = newExceptionArray;
            }

            public bool Remove(Type exceptionType)
            {
                if (exceptionArray.Length > 1)
                {
                    for (int i = 0; i < exceptionArray.Length; i++)
                    {
                        if (exceptionType == exceptionArray[i].ExceptionType)
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
                    if(exceptionType == exceptionArray[0].ExceptionType)
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
                        if (exception.GetType() == exception_foreach.ExceptionType) return true;
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
                        if(i == exceptionArray.Length-1 && exception.GetType() != exceptionArray[i].ExceptionType)
                        {
                            exceptionArray[0].ExceptionAction?.Invoke(exception);
                            return;
                        }

                        if(exception.GetType() == exceptionArray[i].ExceptionType)
                        {
                            exceptionArray[i].ExceptionAction?.Invoke(exception);
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
