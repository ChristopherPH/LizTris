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
        }

        public static SimpleMenu PauseMenu = new SimpleMenu("Paused", new MenuItem[]
        {
            new CloseMenu("Resume"),
            new CloseMenu("Quit Game") { DoAction = GameMenuOptions.QuitGame },
        });


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
                    }),
                    new Choice("Grid:", new MenuItem[]
                    {
                        new MenuItem("Player") { SetProperty = "SharedGrid", Value = false },
                        new MenuItem("Shared") { SetProperty = "SharedGrid", Value = true },
                    }),
                    new MenuItem("Start Game") {
                        DoAction = GameMenuOptions.NewGame },
                    new CloseMenu("Back"),
                })  { DefaultIndex = 3 }},
                new CloseMenu("Back"),
            }) { DefaultIndex = 0 } },
            new OpenMenu("Options") { Menu = new SubMenu("Options", new MenuItem[]
            {
                new OpenMenu("Video") { Menu = new SubMenu("Video", new MenuItem[]
                {
                    new Choice("Fullscreen:", new MenuItem[]
                    {
                        new MenuItem("No") { SetProperty = "Fullscreen", Value = false },
                        new MenuItem("Yes") { SetProperty = "Fullscreen", Value = true },
                    }),
                    /*new Choice("VSync:", new MenuItem[]
                    {
                        new MenuItem("No") { SetProperty = "VSync", Value = false },
                        new MenuItem("Yes") { SetProperty = "VSync", Value = true },
                    }),*/
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
                        new MenuItem("Off") { SetProperty = "Music", Value = false, DoAction = GameMenuOptions.ChangeAudio },
                        new MenuItem("On") { SetProperty = "Music", Value = true, DoAction = GameMenuOptions.ChangeAudio },
                    }, 1),
                    new CloseMenu("Back"),
                }) },
                //new MenuItem("Controls"),
                new CloseMenu("Back"),
            }) { DefaultIndex = 0 }  },
            new OpenMenu("Quit") { Menu = new SubMenu("Really Quit?", new MenuItem[]
            {
                new Choice("Quit", new MenuItem[]
                {
                    new MenuItem("Yes") { DoAction = GameMenuOptions.Exit },
                    new CloseMenu("No"),
                }) { DefaultIndex = 1 },
            }) },
        }) { CloseOnBack = false };
    }
}
