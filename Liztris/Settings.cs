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
        public List<int> HighScores { get; set; } = new List<int>();
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
    public class Settings : Common.SettingsBase
    {
        public static string SettingsFile => "settings.xml";

        public VideoSettings Video { get; set; } = new VideoSettings();
        public AudioSettings Audio { get; set; } = new AudioSettings();
        public ControlSettings Control { get; set; } = new ControlSettings();
        public GameSettings Game { get; set; } = new GameSettings();

        public static Settings LoadSettings()
        {
            var rc = LoadSettings<Settings>(SettingsFile);
            if (rc == null)
                rc = new Settings();

            return rc;
        }

        public bool SaveSettings()
        {
            return SaveSettings(this, SettingsFile);
        }
    }
}
