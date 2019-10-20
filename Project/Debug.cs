using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo
{
    public static class Debug
    {
        static int[] x = new int[0];

        public static void ExceptionDebug()
        {
            x[1] = 5;
        }

        public static void ThrowExceptionDebug()
        {
            throw new Exception("Exception debug");
        }
    }
}
