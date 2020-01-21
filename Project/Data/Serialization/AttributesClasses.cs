using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo.Data.Serialization
{
    public class SerializeClassAttribute : Attribute
    {
        public SerializeClassAttribute() { }
    }

    public class SerializeFieldAttribute : Attribute
    {
        public SerializeFieldAttribute() { }
    }

}
