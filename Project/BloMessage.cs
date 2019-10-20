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
        public MessageType messageType { get; private set; }
        [DataMember]
        public string message { get; private set; }
        public BloMessage(MessageType messageType, string message)
        {
            this.messageType = messageType;
            this.message = message;
        }
    }
}
