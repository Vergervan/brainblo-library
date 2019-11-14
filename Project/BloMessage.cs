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
    [DataContract]
    public class BloMessage
    {
        [DataMember]
        public MessageType MessageType { get; private set; }
        [DataMember]
        public string Message { get; private set; }
        public BloMessage(MessageType messageType, string message)
        {
            MessageType = messageType;
            Message = message;
        }
    }
}
