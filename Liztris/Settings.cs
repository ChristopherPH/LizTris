using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace Liztris
{
    public class GameSettings
    {
        
    }

    public class ControlSettings
    {

    }

    public class VideoSettings
    {
        public enum WindowModeTypes
        {
            Windowed,
            Fullscreen,
            //WindowedFullscreen,
        }

        [XmlAttribute, DefaultValue(1280)]
        public int Width { get; set; } = 1280;

        [XmlAttribute, DefaultValue(720)]
        public int Height { get; set; } = 720;

        [XmlAttribute, DefaultValue(WindowModeTypes.Windowed)]
        public WindowModeTypes WindowMode { get; set; } = WindowModeTypes.Windowed;

        [XmlAttribute, DefaultValue(true)]
        public bool VSync { get; set; } = true;
    }

    public class AudioSettings
    {
        [XmlAttribute, DefaultValue(typeof(uint), "100")]
        public uint SoundVolume { get; set; } = 100;

        [XmlAttribute, DefaultValue(typeof(uint), "100")]
        public uint MusicVolume { get; set; } = 100;

        [XmlAttribute, DefaultValue(true)]
        public bool Music { get; set; } = true;
    }

    [Serializable()]
    public class Settings
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(Settings));

        public static string SettingsFile => "settings.xml";

        public VideoSettings Video { get; set; } = new VideoSettings();
        public AudioSettings Audio { get; set; } = new AudioSettings();
        public ControlSettings Control { get; set; } = new ControlSettings();
        public GameSettings Game { get; set; } = new GameSettings();

        public static Settings LoadSettings()
        {
            string XMLText;

            try
            {
                XMLText = File.ReadAllText(Settings.SettingsFile);
            }
            catch
            {
                return new Settings();
            }

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
                        return serializer.Deserialize(reader) as Settings;
                    }
                }
            }
        }

        public void SaveSettings()
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

                serializer.Serialize(stream, this, ns);
            }

            if (File.Exists(SettingsFile))
                File.Delete(SettingsFile);

            File.Move(tmpFileName, SettingsFile);
        }
    }
}
