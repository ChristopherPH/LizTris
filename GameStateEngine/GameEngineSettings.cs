using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace GameStateEngine
{
    public partial class GameEngineSettings : SettingsBase
    {
        public VideoSettings Video { get; set; } = new VideoSettings();
        public AudioSettings Audio { get; set; } = new AudioSettings();

        public static GameEngineSettings LoadSettings(string FileName)
        {
            var rc = LoadSettings<GameEngineSettings>(FileName);
            if (rc == null)
                rc = new GameEngineSettings();

            return rc;
        }

        public bool SaveSettings(string FileName)
        {
            return SaveSettings(this, FileName);
        }
    }


    public class VideoSettings
    {
        public enum WindowModeTypes
        {
            Windowed,
            Fullscreen,
            WindowedFullscreen,
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
        public bool Sound { get; set; } = true;

        [XmlAttribute, DefaultValue(true)]
        public bool Music { get; set; } = true;
    }
}
