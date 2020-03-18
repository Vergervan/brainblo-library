using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BrainBlo
{
    public enum MessageType
    {
        Connect = 0,
        Debug = 1
    }
    [Serializable]
    public class BloMessage
    {
        public MessageType MessageType { get; private set; }
        public string Message { get; private set; }
        public BloMessage(MessageType messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }
    }
}
