using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BrainBlo.Debug
{
    public class Log
    {
        static Log log;
        public delegate void LogMessage(string str);
        public event LogMessage OnGetLog;
        private Log() { }
        public static Log Initialize()
        {
            if(log == null)
            {
                log = new Log();
            }
            log.Write("Log was initialized");
            return log;
        }
        public void Write(string text)
        {
            OnGetLog?.Invoke(text);
        }

        ~Log()
        {
            log.Write("Log was destroyed");
        }
    }
}
