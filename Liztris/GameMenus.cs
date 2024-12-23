﻿using Common.MenuSystem;
using Common.Misc;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Liztris
{
    public static class GameMenus
    {
        public enum GameMenuOptions
        {
            QuitGame,
            NewGame,
            Exit,
            ApplyGraphics,
            ChangeAudio,
            ShowScores
        }

        public static SimpleMenu PauseMenu = new SimpleMenu("Paused", new MenuItem[]
        {
            new CloseMenu("Resume"),

            new OpenMenu("Audio") { Menu = new SubMenu("Audio", new MenuItem[]
            {
                new AudioVolumeChoice("Master Volume:", "MasterVolume"),
                new AudioVolumeChoice("Music Volume:", "MusicVolume"),

                new Choice("MP3 Music:", new MenuItem[]
                {
                    new MenuItem("No") { SetProperty = "UseMP3", Value = false, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("Yes") { SetProperty = "UseMP3", Value = true, DoAction = GameMenuOptions.ChangeAudio },
                }) { DoActionOnSelect = true },

                new CloseMenu("Back"),
            }) },

            new CloseMenu("Quit Game") { DoAction = GameMenuOptions.QuitGame },
        }) { DefaultIndex = 0 };

        public class MenuResolutionChoice : Choice
        {
            public MenuResolutionChoice(string Text) :
                base(Text, new MenuItem[] { new MenuItem("Default") })
            {
                this.MenuItems = GraphicsAdapter.DefaultAdapter.SupportedDisplayModes
                    .Where(x => x.Format == SurfaceFormat.Color)
                    .Where(x => x.Width >= 640)
                    .Where(x => x.Height >= 480)
                    .Select(x => new MenuItem(string.Format("{0}x{1}", x.Width, x.Height))
                    {
                        SetProperty = "Resolution",
                        Value = string.Format("{0}x{1}", x.Width, x.Height)
                    }).ToArray();
            }
        }

        public class ProfileChoice : Choice
        {
            public ProfileChoice(string Text, int ProfileNum) :
                base(Text, new MenuItem[] { new MenuItem("Default") })
            {
                this.MenuItems = Program.Settings.Game.Profiles
                    .Select(x => new MenuItem(x.Name)
                    {
                        SetProperty = "Profile" + ProfileNum,
                        Value = x.Name
                    }).ToArray();
            }
        }

        public class SpeedChoice : Choice
        {
            public SpeedChoice(string Text, int ProfileNum) :
                base(Text, new MenuItem[]
                {
                    new MenuItem("Slow") { SetProperty = "Speed" + ProfileNum, Value = 0 },
                    new MenuItem("Normal") { SetProperty = "Speed" + ProfileNum, Value = 1 },
                    new MenuItem("Fast") { SetProperty = "Speed" + ProfileNum, Value = 2 },
                }) { DefaultIndex = 1; }
        }

        public class AudioVolumeChoice : Choice
        {
            public AudioVolumeChoice(string Text, string Property) :
                base(Text, new MenuItem[]
                {
                    new MenuItem("0") { SetProperty = Property, Value = 0, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("10") { SetProperty = Property, Value = 10, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("20") { SetProperty = Property, Value = 20, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("30") { SetProperty = Property, Value = 30, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("40") { SetProperty = Property, Value = 40, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("50") { SetProperty = Property, Value = 50, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("60") { SetProperty = Property, Value = 60, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("70") { SetProperty = Property, Value = 70, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("80") { SetProperty = Property, Value = 80, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("90") { SetProperty = Property, Value = 90, DoAction = GameMenuOptions.ChangeAudio },
                    new MenuItem("100") { SetProperty = Property, Value = 100, DoAction = GameMenuOptions.ChangeAudio },
                })
            { DoActionOnSelect = true; }
        }


        public static SimpleMenu MainMenu = new SimpleMenu(string.Empty, new MenuItem[]
        {
            new OpenMenu("New Game") { Menu = new SubMenu(string.Empty, new MenuItem[]
            {
                new OpenMenu("Single Player") {
                    SetProperty = "Players", Value = 1,
                    Menu = new SubMenu(string.Empty, new MenuItem[]
                {
                    new ProfileChoice("Player:", 0),
                    new SpeedChoice("Speed:", 0),

                    new MenuItem("Start Game") {
                        SetProperty = "SharedGrid", Value = false,
                        DoAction = GameMenuOptions.NewGame },

                    new CloseMenu("Back"),
                }) { DefaultIndex = 2 } },

                new OpenMenu("Multi Player") {
                    Menu = new SubMenu(string.Empty, new MenuItem[]
                {
                    new OpenMenu("Endless Challenge") {
                        SetProperty = "SharedGrid", Value = false,
                        Menu = new SubMenu(string.Empty, new MenuItem[]
                    {
                        new OpenMenu("2 Players") {
                            SetProperty = "Players", Value = 2,
                            Menu = new SubMenu(string.Empty, new MenuItem[]
                        {
                            new ProfileChoice("Player 1:", 0),
                            new SpeedChoice("Speed 1:", 0),

                            new ProfileChoice("Player 2:", 1)  { DefaultIndex = 1 },
                            new SpeedChoice("Speed 2:", 1),

                            new MenuItem("Start Game") {
                                DoAction = GameMenuOptions.NewGame },
                            new CloseMenu("Back"),
                        }) { DefaultIndex = 4 }},

                        new OpenMenu("3 Players") {
                            SetProperty = "Players", Value = 3,
                            Menu = new SubMenu(string.Empty, new MenuItem[]
                        {
                            new ProfileChoice("Player 1:", 0),
                            new SpeedChoice("Speed 1:", 0),

                            new ProfileChoice("Player 2:", 1)  { DefaultIndex = 1 },
                            new SpeedChoice("Speed 2:", 1),

                            new ProfileChoice("Player 3:", 2)  { DefaultIndex = 2 },
                            new SpeedChoice("Speed 3:", 2),

                            new MenuItem("Start Game") {
                                DoAction = GameMenuOptions.NewGame },
                            new CloseMenu("Back"),
                        }) { DefaultIndex = 6 }},

                        new OpenMenu("4 Players") {
                            SetProperty = "Players", Value = 4,
                            Menu = new SubMenu(string.Empty, new MenuItem[]
                        {
                            new ProfileChoice("Player 1:", 0),
                            new SpeedChoice("Speed 1:", 0),

                            new ProfileChoice("Player 2:", 1)  { DefaultIndex = 1 },
                            new SpeedChoice("Speed 2:", 1),

                            new ProfileChoice("Player 3:", 2)  { DefaultIndex = 2 },
                            new SpeedChoice("Speed 3:", 2),

                            new ProfileChoice("Player 4:", 3)  { DefaultIndex = 3 },
                            new SpeedChoice("Speed 4:", 3),

                            new MenuItem("Start Game") {
                                DoAction = GameMenuOptions.NewGame },
                            new CloseMenu("Back"),
                        }) { DefaultIndex = 8 }},

                        new CloseMenu("Back"),
                    })},

                    new OpenMenu("Co-Op") {
                        SetProperty = "SharedGrid", Value = true,
                        Menu = new SubMenu(string.Empty, new MenuItem[]
                    {
                        new OpenMenu("2 Players") {
                            SetProperty = "Players", Value = 2,
                            Menu = new SubMenu(string.Empty, new MenuItem[]
                        {
                            new ProfileChoice("Player 1:", 0),
                            new ProfileChoice("Player 2:", 1)  { DefaultIndex = 1 },
                            new SpeedChoice("Speed:", 0),

                            new MenuItem("Start Game") {
                                DoAction = GameMenuOptions.NewGame },
                            new CloseMenu("Back"),
                        }) { DefaultIndex = 3 }},

                        new OpenMenu("3 Players") {
                            SetProperty = "Players", Value = 3,
                            Menu = new SubMenu(string.Empty, new MenuItem[]
                        {
                            new ProfileChoice("Player 1:", 0),
                            new ProfileChoice("Player 2:", 1)  { DefaultIndex = 1 },
                            new ProfileChoice("Player 3:", 2)  { DefaultIndex = 2 },
                            new SpeedChoice("Speed:", 0),

                            new MenuItem("Start Game") {
                                DoAction = GameMenuOptions.NewGame },
                            new CloseMenu("Back"),
                        }) { DefaultIndex = 4 }},

                        new OpenMenu("4 Players") {
                            SetProperty = "Players", Value = 4,
                            Menu = new SubMenu(string.Empty, new MenuItem[]
                        {
                            new ProfileChoice("Player 1:", 0),
                            new ProfileChoice("Player 2:", 1)  { DefaultIndex = 1 },
                            new ProfileChoice("Player 3:", 2)  { DefaultIndex = 2 },
                            new ProfileChoice("Player 4:", 3)  { DefaultIndex = 3 },
                            new SpeedChoice("Speed:", 0),

                            new MenuItem("Start Game") {
                                DoAction = GameMenuOptions.NewGame },
                            new CloseMenu("Back"),
                        }) { DefaultIndex = 5 }},

                        new CloseMenu("Back"),

                    }) },

                    new CloseMenu("Back"),
                })  { DefaultIndex = 0 }},
                new CloseMenu("Back"),
            }) { DefaultIndex = 0 } },
            new OpenMenu("Options") { Menu = new SubMenu("Options", new MenuItem[]
            {
                new OpenMenu("Video") { Menu = new SubMenu("Video", new MenuItem[]
                {
                    new Choice("Mode:", new MenuItem[]
                    {
                        new MenuItem("Windowed") { SetProperty = "WindowMode", Value = VideoSettings.WindowModeTypes.Windowed },
                        new MenuItem("Fullscreen") { SetProperty = "WindowMode", Value = VideoSettings.WindowModeTypes.Fullscreen },
                        new MenuItem("WindowFull") { SetProperty = "WindowMode", Value = VideoSettings.WindowModeTypes.WindowedFullscreen },
                    }, 0),
                    new MenuResolutionChoice("Screen:"),
                    new Choice("VSync:", new MenuItem[]
                    {
                        new MenuItem("No") { SetProperty = "VSync", Value = false },
                        new MenuItem("Yes") { SetProperty = "VSync", Value = true },
                    }, 1),
                    new MenuItem("Apply") { DoAction = GameMenuOptions.ApplyGraphics },
                    new CloseMenu("Back"),
                }) },
                new OpenMenu("Audio") { Menu = new SubMenu("Audio", new MenuItem[]
                {
                    new AudioVolumeChoice("Master Volume:", "MasterVolume"),
                    new AudioVolumeChoice("Music Volume:", "MusicVolume"),

                    new Choice("MP3 Music:", new MenuItem[]
                    {
                        new MenuItem("No") { SetProperty = "UseMP3", Value = false, DoAction = GameMenuOptions.ChangeAudio },
                        new MenuItem("Yes") { SetProperty = "UseMP3", Value = true, DoAction = GameMenuOptions.ChangeAudio },
                    }) { DoActionOnSelect = true },

                    new CloseMenu("Back"),
                }) },
                //new MenuItem("Controls"),
                new CloseMenu("Back"),
            }) { DefaultIndex = 0 }  },
            new MenuItem("High Scores") { DoAction = GameMenuOptions.ShowScores },
            new OpenMenu("Quit") { Menu = new SubMenu("Really Quit?", new MenuItem[]
            {
                new Choice("Quit:", new MenuItem[]
                {
                    new MenuItem("Yes") { DoAction = GameMenuOptions.Exit },
                    new CloseMenu("No"),
                }) { DefaultIndex = 1 },
            }) },
        }) { CloseOnBack = false };
    }
}
