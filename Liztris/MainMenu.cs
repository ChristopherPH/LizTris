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
    public static class TestMenu
    {
        public static SimpleMenu MyMenu = new SimpleMenu("LizTris", new MenuItem[]
        {
            new OpenMenu("New Game") { Menu = new SubMenu(string.Empty, new MenuItem[]
            {
                new MenuItem("Single Player") { SetProperty = "Players", Value = 1, DoAction = 50 },

                new OpenMenu("Multi Player") { Menu = new SubMenu(string.Empty, new MenuItem[]
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
                    }) { DefaultIndex = 1 },
                    new Choice("Grid:", new MenuItem[]
                    {
                        new MenuItem("Player") { SetProperty = "SharedGrid", Value = false },
                        new MenuItem("Shared") { SetProperty = "SharedGrid", Value = true },
                    }),
                    new MenuItem("Start Game") { DoAction = -1 },
                }) },

                new OpenMenu("3 Players") { SetProperty = "Players", Value = 3,
                    Menu = new SubMenu(string.Empty, new MenuItem[]
                    {
                        new MenuItem("Grid Per Player") { SetProperty = "SharedGrid", Value = false, DoAction = 50 },
                        new MenuItem("Shared Grid") { SetProperty = "SharedGrid", Value = true, DoAction = 50 },
                        new CloseMenu("Back"),
                    }) },
                new OpenMenu("4 Players") { SetProperty = "Players", Value = 4,
                    Menu = new SubMenu(string.Empty, new MenuItem[]
                    {
                        new MenuItem("Grid Per Player") { SetProperty = "SharedGrid", Value = false, DoAction = 50 },
                        new MenuItem("Shared Grid") { SetProperty = "SharedGrid", Value = true, DoAction = 50 },
                        new CloseMenu("Back"),
                    }) },
                new CloseMenu("Back"),
            }) },
            new OpenMenu("Options") { Menu = new SubMenu("Options", new MenuItem[]
            {
                new OpenMenu("Video") { Menu = new SubMenu("Video", new MenuItem[]
                {
                    new Choice("Fullscreen:", new MenuItem[]
                    {
                        new MenuItem("No") { SetProperty = "Fullscreen", Value = false },
                        new MenuItem("Yes") { SetProperty = "Fullscreen", Value = true },
                    }),
                    new Choice("VSync:", new MenuItem[]
                    {
                        new MenuItem("No") { SetProperty = "VSync", Value = false },
                        new MenuItem("Yes") { SetProperty = "VSync", Value = true },
                    }),
                    new MenuItem("Apply") { DoAction = 99 },
                    new CloseMenu("Back"),
                }) },
                new OpenMenu("Audio") { Menu = new SubMenu("Audio", new MenuItem[]
                {
                    new Choice("Sound:", new MenuItem[]
                    {
                        new MenuItem("Off") { SetProperty = "Sound", Value = false },
                        new MenuItem("On") { SetProperty = "Sound", Value = true },
                    }),
                    new Choice("Music:", new MenuItem[]
                    {
                        new MenuItem("Off") { SetProperty = "Music", Value = false },
                        new MenuItem("On") { SetProperty = "Music", Value = true },
                    }),
                    new CloseMenu("Back"),
                }) },
                new MenuItem("Controls"),
                new CloseMenu("Back"),
            }) },
            new OpenMenu("Quit") { Menu = new SubMenu("Really Quit?", new MenuItem[]
            {
                new Choice("Quit", new MenuItem[]
                {
                    new MenuItem("Yes") { DoAction = -1 },
                    new CloseMenu("No"),
                }) { DefaultIndex = 1 },
            }) },
        });
    }
}
