using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo.Network
{
    public class ExceptionListException : Exception
    {
        public ExceptionListException() { }
        public ExceptionListException(string message) : base(message) { }
    }
}
