using Common;
using Common.MenuSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public static SimpleMenu MainMenu = new SimpleMenu(string.Empty, new MenuItem[]
        {
            new OpenMenu("New Game") { Menu = new SubMenu(string.Empty, new MenuItem[]
            {
                new OpenMenu("Single Player") {
                    SetProperty = "Players", Value = 1,
                    Menu = new SubMenu(string.Empty, new MenuItem[]
                {
                    new Choice("Speed:", new MenuItem[]
                    {
                        new MenuItem("Slow") { SetProperty = "Speed", Value = 0 },
                        new MenuItem("Normal") { SetProperty = "Speed", Value = 1 },
                        new MenuItem("Fast") { SetProperty = "Speed", Value = 2 },
                    }) { DefaultIndex = 1 },

                    new MenuItem("Start Game") {
                        SetProperty = "SharedGrid", Value = false,
                        DoAction = GameMenuOptions.NewGame },

                    new CloseMenu("Back"),
                }) { DefaultIndex = 1 } },

                new OpenMenu("Multi Player") {
                    Menu = new SubMenu(string.Empty, new MenuItem[]
                {
                    new OpenMenu("Endless Challenge") {
                        SetProperty = "SharedGrid", Value = false,
                        Menu = new SubMenu(string.Empty, new MenuItem[]
                    {
                        new Choice("Players:", new MenuItem[]
                        {
                            new MenuItem("2") { SetProperty = "Players", Value = 2 },
                            new MenuItem("3") { SetProperty = "Players", Value = 3 },
                            new MenuItem("4") { SetProperty = "Players", Value = 4 },
                        }),
                        new Choice("Speed:", new MenuItem[]
                        {
                            new MenuItem("Slow") { SetProperty = "Speed", Value = 0 },
                            new MenuItem("Normal") { SetProperty = "Speed", Value = 1 },
                            new MenuItem("Fast") { SetProperty = "Speed", Value = 2 },
                        }, 1),
                        new MenuItem("Start Game") {
                            DoAction = GameMenuOptions.NewGame },
                        new CloseMenu("Back"),
                    })  { DefaultIndex = 2 }},

                    new OpenMenu("Co-Op") {
                        SetProperty = "SharedGrid", Value = true,
                        Menu = new SubMenu(string.Empty, new MenuItem[]
                    {
                        new Choice("Players:", new MenuItem[]
                        {
                            new MenuItem("2") { SetProperty = "Players", Value = 2 },
                            new MenuItem("3") { SetProperty = "Players", Value = 3 },
                            new MenuItem("4") { SetProperty = "Players", Value = 4 },
                        }),
                        new Choice("Speed:", new MenuItem[]
                        {
                            new MenuItem("Slow") { SetProperty = "Speed", Value = 0 },
                            new MenuItem("Normal") { SetProperty = "Speed", Value = 1 },
                            new MenuItem("Fast") { SetProperty = "Speed", Value = 2 },
                        }, 1),
                        new MenuItem("Start Game") {
                            DoAction = GameMenuOptions.NewGame },
                        new CloseMenu("Back"),
                    })  { DefaultIndex = 2 }},
                    
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
                    /*new Choice("Sound:", new MenuItem[]
                    {
                        new MenuItem("Off") { SetProperty = "Sound", Value = false, DoAction = GameMenuOptions.ChangeAudio },
                        new MenuItem("On") { SetProperty = "Sound", Value = true, DoAction = GameMenuOptions.ChangeAudio },
                    }),*/
                    new Choice("Music:", new MenuItem[]
                    {
                        new MenuItem("Off") {
                            SetProperty = "Music", Value = false,
                            DoAction = GameMenuOptions.ChangeAudio },
                        new MenuItem("On") {
                            SetProperty = "Music", Value = true,
                            DoAction = GameMenuOptions.ChangeAudio },
                    }, 1) { DoActionOnSelect = true },
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
