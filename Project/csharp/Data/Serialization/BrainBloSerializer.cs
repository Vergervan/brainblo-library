using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainBlo.Data.Serialization
{
    public static class BrainBloSerializer
    {
        public static string Serialize(object o)
        {
            var obj = new StringBuilder();
            obj.Append("<#");

            Type type = o.GetType();
            var attrs = type.GetCustomAttributes(false);
            for (int i = 0; i < attrs.Length; i++)
            {
                Console.WriteLine(attrs[i]);
                if (attrs[i].GetType() == typeof(SerializeClassAttribute))
                {
                    break;
                }
                else if (i == attrs.Length - 1 && attrs[i].GetType() != typeof(SerializeClassAttribute))
                {
                    throw new Exception("У объекта нету аттрибута Класс");
                }
            }

            List<System.Reflection.PropertyInfo> useProps = new List<System.Reflection.PropertyInfo>();

            var props = type.GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                var propAttrs = props[i].CustomAttributes;
                foreach (var c in propAttrs)
                {
                    if (c.AttributeType == typeof(SerializeFieldAttribute))
                    {
                        useProps.Add(props[i]);
                    }
                }

            }

            for (int i = 0; i < useProps.Count; i++)
            {
                obj.Append($"<!{useProps[i].Name}={useProps[i].GetValue(o)}!>");
            }
            obj.Append("#>");

            return obj.ToString();
        }


    }
}
