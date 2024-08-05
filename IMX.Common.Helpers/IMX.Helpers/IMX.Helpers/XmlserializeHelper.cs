using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace IMX.Common.Helpers
{
    public class XmlserializeHelper
    {
        public static string ObjToXml(object obj)
        {
            using (MemoryStream Stream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(obj.GetType());
                xml.Serialize(Stream, obj);
                Stream.Position = 0;
                StreamReader sr = new StreamReader(Stream);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            return Serialize<T>(obj, Encoding.UTF8);
        }

        public static string Serialize<T>(T obj, Encoding encoding)
        {
            try
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }
                var ser = new XmlSerializer(obj.GetType());
                using (var ms = new MemoryStream())
                {
                    using (var writer = new XmlTextWriter(ms, encoding))
                    {
                        writer.Formatting = Formatting.Indented;
                        ser.Serialize(writer, obj);
                    }
                    var xml = encoding.GetString(ms.ToArray());
                    xml = xml.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", "");
                    xml = xml.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", "");
                    xml = Regex.Replace(xml, @"\s{2}", "");
                    xml = Regex.Replace(xml, @"\s{1}/>", "/>");
                    return xml;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T DeSerialize<T>(string xml) where T : new()
        {
            return DeSerialize<T>(xml, Encoding.UTF8);
        }

        public static T DeSerialize<T>(string xml, Encoding encoding) where T : new()
        {
            try
            {
                var mySerializer = new XmlSerializer(typeof(T));
                using (var ms = new MemoryStream(encoding.GetBytes(xml)))
                {
                    using (var sr = new StreamReader(ms, encoding))
                    {
                        return (T)mySerializer.Deserialize(sr);
                    }
                }
            }
            catch (Exception ex)
            {
                return default;
            }
        }
    }
}
