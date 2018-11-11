using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Common
{
    public static class ParseHelpers
    {
        static Dictionary<Type, List<KeyValuePair<Type[], XmlSerializer>>> cache = new Dictionary<Type, List<KeyValuePair<Type[], XmlSerializer>>>();

        private static XmlSerializer AddSerializer(Type t, Type[] ExtraTypes)
        {
            if ((ExtraTypes != null) && (ExtraTypes.Length == 0))
                ExtraTypes = null;

            if (!cache.ContainsKey(t))
                cache[t] = new List<KeyValuePair<Type[], XmlSerializer>>();

            XmlSerializer serializer;

            if ((ExtraTypes == null) || (ExtraTypes.Length == 0))
            {
                serializer = new XmlSerializer(t);
            }
            else
            {
                serializer = new XmlSerializer(t, ExtraTypes);
            }

            cache[t].Add(new KeyValuePair<Type[], XmlSerializer>(ExtraTypes, serializer));

            return serializer;
        }

        private static XmlSerializer GetSerializer(Type t, Type[] ExtraTypes)
        {
            if ((ExtraTypes != null) && (ExtraTypes.Length == 0))
                ExtraTypes = null;

            if (!cache.ContainsKey(t))
                return AddSerializer(t, ExtraTypes);


            if (ExtraTypes == null)
            {
                foreach (var kv in cache[t])
                {
                    if (kv.Key == null)
                        return kv.Value;
                }

                return AddSerializer(t, null);
            }

            foreach (var kv in cache[t])
            {
                if (kv.Key == null)
                    continue;

                if (kv.Key.SequenceEqual(ExtraTypes))
                    return kv.Value;
            }

            return AddSerializer(t, ExtraTypes);
        }

        public static T ParseXML<T>(string XMLText) where T : class
        {
            return ParseXML<T>(XMLText, null);
        }

        public static T ParseXML<T>(string XMLText, Type[] ExtraTypes) where T : class
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(XMLText);
                    writer.Flush();
                    stream.Position = 0;

                    using (var reader = XmlReader.Create(stream, new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document }))
                    {
                        var serializer = GetSerializer(typeof(T), ExtraTypes);
                        return serializer.Deserialize(reader) as T;
                    }
                }
            }
        }

        public static void SaveXML<T>(T obj, String FileName) where T : class
        {
            SaveXML(obj, FileName, null);
        }

        public static void SaveXML<T>(T obj, String FileName, Type[] ExtraTypes) where T : class
        {
            var tmpFileName = Path.GetTempFileName();

            using (StreamWriter writer = new StreamWriter(tmpFileName))
            {
                XmlSerializer serializer = GetSerializer(typeof(T), ExtraTypes);

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                serializer.Serialize(writer, obj, ns);
            }

            if (File.Exists(FileName))
                File.Delete(FileName);

            File.Move(tmpFileName, FileName);
        }

        public static void SaveXMLTab<T>(T obj, String FileName) where T : class
        {
            SaveXMLTab(obj, FileName, null);
        }

        public static void SaveXMLTab<T>(T obj, String FileName, Type[] ExtraTypes) where T : class
        {
            var tmpFileName = Path.GetTempFileName();

            XmlTextWriter stream = new XmlTextWriter(tmpFileName, System.Text.Encoding.UTF8);
            stream.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"");
            using (Stream baseStream = stream.BaseStream)
            {
                stream.Formatting = Formatting.Indented;
                stream.IndentChar = '\t';
                stream.Indentation = 1;

                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                XmlSerializer serializer = GetSerializer(typeof(T), ExtraTypes);
                serializer.Serialize(stream, obj, ns);
            }

            if (File.Exists(FileName))
                File.Delete(FileName);

            File.Move(tmpFileName, FileName);
        }

        public static T CloneXML<T>(T obj) where T : class
        {
            return CloneXML(obj, null);
        }

        public static T CloneXML<T>(T obj, Type[] ExtraTypes) where T : class
        {
            using (MemoryStream writer = new MemoryStream())
            {
                XmlSerializer serializer = GetSerializer(typeof(T), ExtraTypes);
                serializer.Serialize(writer, obj);

                writer.Position = 0;

                using (var reader = XmlReader.Create(writer, new XmlReaderSettings()
                    { ConformanceLevel = ConformanceLevel.Document }))
                {
                    return serializer.Deserialize(reader) as T;
                }
            }
        }

        public static string GetXML<T>(T obj, Type[] ExtraTypes) where T : class
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(ms, Encoding.UTF8))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.IndentChar = '\t';
                    writer.Indentation = 1;

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);

                    XmlSerializer serializer = GetSerializer(typeof(T), ExtraTypes);
                    serializer.Serialize(writer, obj, ns);

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}
