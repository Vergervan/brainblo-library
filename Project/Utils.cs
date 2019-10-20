using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace BrainBlo
{
    public static class Utils
    {
        public static string SerializeJson(object o)
        {
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(o.GetType());
            MemoryStream ms = new MemoryStream();
            jsonSerializer.WriteObject(ms, o);
            ms.Position = 0;
            StreamReader sr = new StreamReader(ms);

            string json = sr.ReadToEnd();
            sr.Close();
            ms.Close();
            return json;
        }

        public static T DeserializeJson<T>(string jsonString)
        {
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
            DataContractJsonSerializer jsonDerializer = new DataContractJsonSerializer(typeof(T));
            return (T)jsonDerializer.ReadObject(ms);
        }
    }
}
