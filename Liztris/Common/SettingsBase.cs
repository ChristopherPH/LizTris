using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Common
{
    public abstract class SettingsBase
    {
        private static Dictionary<Type, XmlSerializer> _cache = new Dictionary<Type, XmlSerializer>();

        public static T LoadSettings<T>(string FileName) where T : SettingsBase
        {
            string XMLText;

            try
            {
                XMLText = File.ReadAllText(FileName);
                if (string.IsNullOrWhiteSpace(XMLText))
                    return null;

                using (var stream = new MemoryStream())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(XMLText);
                        writer.Flush();
                        stream.Position = 0;

                        using (var reader = XmlReader.Create(stream, new XmlReaderSettings()
                        { ConformanceLevel = ConformanceLevel.Document }))
                        {
                            if (!_cache.ContainsKey(typeof(T)))
                                _cache[typeof(T)] = new XmlSerializer(typeof(T));

                            return _cache[typeof(T)].Deserialize(reader) as T;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public static bool SaveSettings<T>(T obj, string FileName) where T : SettingsBase
        {
            try
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

                    if (!_cache.ContainsKey(typeof(T)))
                        _cache[typeof(T)] = new XmlSerializer(typeof(T));

                    _cache[typeof(T)].Serialize(stream, obj, ns);
                }

                if (File.Exists(FileName))
                    File.Delete(FileName);

                File.Move(tmpFileName, FileName);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
