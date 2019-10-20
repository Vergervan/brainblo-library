using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo
{
    namespace Network
    {
        public enum Protocol
        {
            TCP = 0,
        }
        public enum ThreadType
        {
            Thread = 0,
            Task = 1
        }
        public delegate void MessageProcessing(params object[] o);
    }
}
