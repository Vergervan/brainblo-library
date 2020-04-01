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
        static FileStream fs = new FileStream("log.txt", FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);
        StreamReader sr = new StreamReader(fs);
        private Log() { }
        public static Log Initialize()
        {
            if(log == null)
            {
                log = new Log();
            }
            log.WriteLine("Log was initialized");
            return log;
        }
        public void Write(string text)
        {
            sw.Write(text);
            sw.Flush();
        }
        public void WriteLine(string text)
        {
            sw.WriteLine(text);
            sw.Flush();
        }

        ~Log()
        {
            log.sw.Close();
            log.sw.Dispose();
            log.sr.Close();
            log.sr.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}
