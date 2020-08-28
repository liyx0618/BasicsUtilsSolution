using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Util.Basics
{
    public static class XmlUtils
    {
        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        /// <typeparam name="T">对象实例对应的类.T的修饰符应当为Public</typeparam>
        /// <param name="obj">对象实例</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToXml<T>(this T obj)
        {
            if (obj == null) throw new NullReferenceException();
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, obj);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public static T ToInstanceFromXml<T>(this string xmlDocument)
        {
            try
            {
                using (StringReader reader = new StringReader(xmlDocument))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    var obj = serializer.Deserialize(reader);
                    if (obj is T)
                    {
                        return (T)obj;
                    }
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }
        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToXml(this object obj)
        {
            return ToXml(obj, Encoding.UTF8);
        }
        /// <summary>
        /// 将对象序列化为Xml
        /// </summary>
        /// <param name="obj">对象实例</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>序列化后的字符串</returns>
        public static string ToXml(this object obj, Encoding encoding)
        {
            if (obj == null)
            {
                throw new ArgumentException("对象实例不能为空");
            }
            if (encoding == null)
            {
                throw new Exception("编码不能为空");
            }
            try
            {
                using (var stream = new MemoryStream())
                {
                    var serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(stream, obj);
                    stream.Position = 0;
                    return encoding.GetString(stream.ToArray());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("", ex);
            }
        }
    }
}
