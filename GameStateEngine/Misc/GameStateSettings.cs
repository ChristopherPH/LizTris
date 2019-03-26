using Common;
using Common.MenuSystem;
using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Misc
{
    class GameStateEngineSettings : GameState
    {
        SpriteFont _font;
        SimpleMenu SettingsMenu = new SimpleMenu(string.Empty, new MenuItem[]
        {
            AudioMenu,
            VideoMenu,
            new CloseMenu("Back"),
        });

        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
            var winWidth = Engine.GameRectangle.Width * 0.34;
            var winHeight = Engine.GameRectangle.Height * 0.5;

            var MenuRect = new Rectangle(
                (Engine.GameRectangle.Width / 2) - (int)(winWidth / 2),
                (Engine.GameRectangle.Height / 2) - (int)(winHeight / 2),
                (int)winWidth, (int)winHeight);

            SettingsMenu.Draw(spriteBatch, _font, MenuRect, true);
        }

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            var rc = SettingsMenu.Update(gameTime, out object MenuAction);

            if (rc.HasFlag(MenuBase.MenuResult.PerformedAction) && (MenuAction != null))
            {
                switch ((SettingOptions)MenuAction)
                {
                    case SettingOptions.ChangeAudio:
                        var tmpMusicVolume = (int)SettingsMenu.Options["MusicVolume"];
                        var tmpMasterVolume = (int)SettingsMenu.Options["MasterVolume"];
                        Engine.SetVolume(tmpMasterVolume, tmpMusicVolume, true);
                        break;

                    case SettingOptions.ChangeVideo:
                        var resString = (string)SettingsMenu.Options["Resolution"];
                        if (string.IsNullOrWhiteSpace(resString))
                        {
                            System.Diagnostics.Debug.Print("Invalid Resolution");
                            return;
                        }

                        var resStrings = resString.Split('x');
                        if ((resStrings == null) || (resStrings.Length != 2))
                        {
                            System.Diagnostics.Debug.Print("Invalid Resolution {0}", resString);
                            return;
                        }

                        if ((int.TryParse(resStrings[0], out int tmpWidth) == false) || (tmpWidth <= 0))
                        {
                            System.Diagnostics.Debug.Print("Invalid Resolution Width {0}", resStrings[0]);
                            return;
                        }

                        if ((int.TryParse(resStrings[1], out int tmpHeight) == false) || (tmpHeight <= 0))
                        {
                            System.Diagnostics.Debug.Print("Invalid Resolution Height {0}", resStrings[1]);
                            return;
                        }

                        var tmpVSync = (bool)SettingsMenu.Options["VSync"];
                        var tmpMode = (VideoSettings.WindowModeTypes)SettingsMenu.Options["WindowMode"];

                        Engine.SetResolution(tmpWidth, tmpHeight, tmpMode, tmpVSync, true);
                        break;
                }
            }

            if (!SettingsMenu.IsMenuActive)
                Operation = GameStateOperation.CompleteState;
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }

        public override void OnStateStarted(bool Resumed)
        {
            //HACK: There must be a better way to sync the settings and menu
            SettingsMenu.SetPropertyValue("WindowMode", Engine.Video_WindowMode);
            SettingsMenu.SetPropertyValue("VSync", Engine.Video_VSync);

            SettingsMenu.SetPropertyValue("Resolution",
                string.Format("{0}x{1}", Engine.Video_Width, Engine.Video_Height));

            SettingsMenu.SetPropertyValue("MasterVolume", Engine.Audio_MasterVolume);
            SettingsMenu.SetPropertyValue("MusicVolume", Engine.Audio_MusicVolume);

            SettingsMenu.ResetMenu();
            SettingsMenu.ResetInputs();
        }

        public enum SettingOptions
        {
            ChangeAudio,
            ChangeVideo
        }

        public static MenuItem AudioMenu = new OpenMenu("Audio")
        {
            Menu = new SubMenu("Audio", new MenuItem[]
            {
                new AudioVolumeChoice("Master Volume:", "MasterVolume"),
                new AudioVolumeChoice("Music Volume:", "MusicVolume"),
                new CloseMenu("Back"),
            })
        };

        public static MenuItem VideoMenu = new OpenMenu("Video")
        {
            Menu = new SubMenu("Video", new MenuItem[]
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
                new MenuItem("Apply") { DoAction = SettingOptions.ChangeVideo },
                new CloseMenu("Back"),
            })
        };

        public class AudioVolumeChoice : Choice
        {
            public AudioVolumeChoice(string Text, string Property) :
                base(Text, new MenuItem[]
                {
                    new MenuItem("0") { SetProperty = Property, Value = 0, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("10") { SetProperty = Property, Value = 10, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("20") { SetProperty = Property, Value = 20, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("30") { SetProperty = Property, Value = 30, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("40") { SetProperty = Property, Value = 40, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("50") { SetProperty = Property, Value = 50, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("60") { SetProperty = Property, Value = 60, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("70") { SetProperty = Property, Value = 70, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("80") { SetProperty = Property, Value = 80, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("90") { SetProperty = Property, Value = 90, DoAction = SettingOptions.ChangeAudio },
                    new MenuItem("100") { SetProperty = Property, Value = 100, DoAction = SettingOptions.ChangeAudio },
                })
            { DoActionOnSelect = true; }
        }

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
    }
}
