﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Liztris
{
    public static class Program
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-9#top-level-statements
        public static Settings Settings = Settings.LoadSettings();
    }

    public class GameSettings
    {
        public List<HighScore> HighScores { get; set; } = new List<HighScore>();

        [XmlIgnore]
        public List<Profile> Profiles { get; set; } = new List<Profile>()
        {
            new Profile() {Name = "Liz" },
            new Profile() {Name = "Chris" },
            new Profile() {Name = "Gwen" },
            new Profile() {Name = "Guest" },
        };

        [XmlArray(ElementName = nameof(Profiles))]
        public Profile[] Profiles_XMLSerializationProxy_DO_NOT_USE
        {
            get { return Profiles?.ToArray(); }
            set { Profiles = value?.ToList(); }
        }
    }

    public class ControlSettings
    {

    }

    public class Profile
    {
        [XmlAttribute]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue(0)]
        public int BestScore { get; set; } = 0;

        [XmlAttribute]
        [DefaultValue(0)]
        public int BestLines { get; set; } = 0;
    }

    public class HighScore
    {
        [XmlAttribute]
        [DefaultValue("")]
        public string Name { get; set; } = string.Empty;

        /*
        [XmlAttribute]
        [DefaultValue("")]
        public string Mode { get; set; } = string.Empty;
        */

        [XmlAttribute]
        [DefaultValue(0)]
        public int Score { get; set; } = 0;

        [XmlAttribute]
        [DefaultValue(0)]
        public int Lines { get; set; } = 0;
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
        [XmlAttribute, DefaultValue(100)]
        public int MasterVolume { get; set; } = 100;

        [XmlAttribute, DefaultValue(20)]
        public int MusicVolume { get; set; } = 20;

        [XmlAttribute, DefaultValue(false)]
        public bool UseMP3 { get; set; } = false;
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
