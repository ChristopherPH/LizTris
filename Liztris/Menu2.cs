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

/*
 * New Game
 *      One Player
 *      Two Players
 *          Grid per Player
 *          Shared Grid
 *      Three Players
 *          Grid per Player
 *          Shared Grid
 *      Four Players
 *          Grid per Player
 *          Shared Grid
 * Full Screen
 * Quit
 * 
 * New Game
 *      Players:        [1] [2] [3] [4]
 *      Shared Grid     [No] [Yes]
 *      Player 1 Speed  [Slow Medium Fast]
 *      Player 2 Speed  [Slow Medium Fast]
 *      Player 3 Speed  [Slow Medium Fast]
 *      Player 4 Speed  [Slow Medium Fast]
 *      Player 1 Controls  [Gamepad 1/2/3/4 Keyboard lrud wsad ikjl numpad]
 *      Player 2 Speed  [Slow Medium Fast]
 *      Player 3 Speed  [Slow Medium Fast]
 *      Player 4 Speed  [Slow Medium Fast]
 */



/*
 * New Game
 *      Single Player
 *          Speed  (Slow [Normal] Fast)
 *      Multi Player
 *          2 Players
 *              Start Game
 *              Player 1 Speed  (Slow [Normal] Fast)
 *              Player 2 Speed  (Slow [Normal] Fast)
 *              Back
 *          3 Players
 *              Start Game
 *              Player 1 Speed  (Slow [Normal] Fast)
 *              Player 2 Speed  (Slow [Normal] Fast)
 *              Player 3 Speed  (Slow [Normal] Fast)
 *              Back
 *          4 Players
 *              Start Game
 *              Player 1 Speed  (Slow [Normal] Fast)
 *              Player 2 Speed  (Slow [Normal] Fast)
 *              Player 3 Speed  (Slow [Normal] Fast)
 *              Player 4 Speed  (Slow [Normal] Fast)
 *              Back
 *          Back
 *      Multi Player Co-Op (Shared Grid)    SETPROP(shared, true)   OPENMENU
 *          2 Players                       SETPROP(players, 2)
 *              Start Game                  EXITMENU, CUSTOMACTION
 *              Player 1 Speed  (Slow [Normal] Fast)    CHOICE(p1speed, ["Slow", "Normal", "Fast"])
 *              Player 2 Speed  (Slow [Normal] Fast)    CHOICE(p2speed, ["Slow", "Normal", "Fast"], [Speed.Slow, Speed.Normal, Speed.Fast]
 *              Back                        CLOSEMENU
 *          3 Players
 *              Start Game
 *              Player 1 Speed  (Slow [Normal] Fast)
 *              Player 2 Speed  (Slow [Normal] Fast)
 *              Player 3 Speed  (Slow [Normal] Fast)
 *              Back
 *          4 Players
 *              Start Game
 *              Player 1 Speed  (Slow [Normal] Fast)
 *              Player 2 Speed  (Slow [Normal] Fast)
 *              Player 3 Speed  (Slow [Normal] Fast)
 *              Player 4 Speed  (Slow [Normal] Fast)
 *              Back
 *          Back
 *
 * Options
 *      Fullscreen  [no] [yes]
 *     
 * Quit Game
 *      Are you sure?
 *          Yes     [No]        CHOICEACTION(quit, ["Yes", "No"], [EXITMENU/CUSTOMACTION, CLOSEMENU])
 *  
 *      
 * Dictionary<string, object>()...
 * MenuOptions["Fullscreen"] = "no";
 * 
 * 
 * Choice("Fullscreen", new kvp("yes", true), new kvp("no", false))
 * Action("Apply", videoapply)
 * 
 *      
 */
namespace Liztris
{
    public class Menu2 : MenuBase
    {
        public Menu2(MenuItem[] MenuItems) : base(MenuItems)
        {
            inputManager.AddAction(MenuCommands.MenuSelect, Keys.Enter);
            inputManager.AddAction(MenuCommands.MenuSelect, InputManager<MenuCommands>.GamePadButtons.A);

            inputManager.AddAction(MenuCommands.MenuBack, Keys.Escape);
            inputManager.AddAction(MenuCommands.MenuBack, InputManager<MenuCommands>.GamePadButtons.B);

            inputManager.AddAction(MenuCommands.MenuUp, Keys.Up);
            inputManager.AddAction(MenuCommands.MenuUp, InputManager<MenuCommands>.GamePadButtons.Up);

            inputManager.AddAction(MenuCommands.MenuDown, Keys.Down);
            inputManager.AddAction(MenuCommands.MenuDown, InputManager<MenuCommands>.GamePadButtons.Down);

            inputManager.AddAction(MenuCommands.MenuLeft, Keys.Left);
            inputManager.AddAction(MenuCommands.MenuLeft, InputManager<MenuCommands>.GamePadButtons.Left);

            inputManager.AddAction(MenuCommands.MenuRight, Keys.Right);
            inputManager.AddAction(MenuCommands.MenuRight, InputManager<MenuCommands>.GamePadButtons.Right);
        }



        
        public SerializableDictionary<string, object> Options { get; } = new SerializableDictionary<string, object>();
        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();
        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(20);

        public bool Update(GameTime gameTime)
        {
            AnimationTimer.UpdateAndCheck(gameTime, () =>
            {
                if (!_scaleReverse)
                {
                    _scale += 0.02f;
                    if (_scale >= 1.2)
                        _scaleReverse = true;
                }
                else
                {
                    _scale -= 0.02f;
                    if (_scale <= 1)
                        _scaleReverse = false;
                }
            });

            inputManager.Update(PlayerIndex.One);

            bool rc = false;

            if (inputManager.IsActionTriggered(MenuCommands.MenuUp))
                rc = RunMenuCommand(MenuCommands.MenuUp);
            else if (inputManager.IsActionTriggered(MenuCommands.MenuDown))
                rc = RunMenuCommand(MenuCommands.MenuDown);
            else if (inputManager.IsActionTriggered(MenuCommands.MenuLeft))
                rc = RunMenuCommand(MenuCommands.MenuLeft);
            else if (inputManager.IsActionTriggered(MenuCommands.MenuRight))
                rc = RunMenuCommand(MenuCommands.MenuRight);
            else if (inputManager.IsActionTriggered(MenuCommands.MenuSelect))
                rc = RunMenuCommand(MenuCommands.MenuSelect);
            else if (inputManager.IsActionTriggered(MenuCommands.MenuBack))
                rc = RunMenuCommand(MenuCommands.MenuBack);

            if (rc)
            {
                //play sound
            }

            return IsMenuActive;
        }


        protected override void OnSetProperty(string Property, object Value)
        {
            Options[Property] = Value;
            System.Diagnostics.Debug.Print("Set {0} to {1}",
                Property, Value);
        }


        protected override void OnAction(object Action)
        {
            System.Diagnostics.Debug.Print("Action {0}", Action);
        }

        protected override void DrawMenuBegin(ExtendedSpriteBatch spriteBatch, 
            SpriteFont spriteFont, Rectangle MenuRect, SubMenu CurrentMenu)
        {
            spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

            //spriteBatch.Draw(transparentDarkTexture, MenuRect, Color.White* 0.5f);
        }

        protected override void DrawItem(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect, 
            MenuItem CurrentItem, Rectangle ItemRect, bool IsSelected)
        {
            var c = Color.White;
            var scale = 1.0f;

            if (IsSelected)
            {
                c = Color.Aquamarine;
                scale = _scale;
            }

            spriteBatch.DrawString(spriteFont, CurrentItem.Text, ItemRect, 
                ExtendedSpriteBatch.Alignment.Center, c, scale);
        }

        protected override void DrawChoice(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect,
            Choice CurrentItem, Rectangle ItemRect, bool IsSelected, MenuItem[] Choices, MenuItem SelectedChoice)
        {
            int PixelsBetweenChoices = 30;

            Vector2 size = spriteFont.MeasureString(CurrentItem.Text);

            var c = Color.White;
            if (IsSelected)
                c = Color.Aquamarine;

            spriteBatch.DrawString(spriteFont, CurrentItem.Text, ItemRect,
                ExtendedSpriteBatch.Alignment.Left, c, 1.0f);

            int x = ItemRect.Left + (int)size.X + PixelsBetweenChoices;

            foreach (var choice in Choices)
            {
                c = Color.White;

                if (choice == SelectedChoice)
                    c = Color.LightBlue;

                spriteBatch.DrawString(spriteFont, choice.Text, new Vector2(x, ItemRect.Y), c);

                x += (int)spriteFont.MeasureString(choice.Text).X + PixelsBetweenChoices;
            }
        }

    }


    public static class TestMenu
    {
        public static Menu2 MyMenu = new Menu2(new MenuItem[]
        {
            new OpenMenu("New Game") { SubMenu = new Menu2(new MenuItem[]
            {
                new MenuItem("Single Player") { SetProperty = "Players", Value = 1, DoAction = 50 },

                new OpenMenu("Multi Player") { SubMenu = new Menu2(new MenuItem[]
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
                    }) { StartingIndex = 1 },
                    new Choice("Grid:", new MenuItem[]
                    {
                        new MenuItem("Player") { SetProperty = "SharedGrid", Value = false },
                        new MenuItem("Shared") { SetProperty = "SharedGrid", Value = true },
                    }),
                    new MenuItem("Start Game") { DoAction = -1 },
                }) },

                new OpenMenu("3 Players") { SetProperty = "Players", Value = 3,
                    SubMenu = new Menu2(new  MenuItem[]
                    {
                        new MenuItem("Grid Per Player") { SetProperty = "SharedGrid", Value = false, DoAction = 50 },
                        new MenuItem("Shared Grid") { SetProperty = "SharedGrid", Value = true, DoAction = 50 },
                        new CloseMenu("Back"),
                    }) },
                new OpenMenu("4 Players") { SetProperty = "Players", Value = 4,
                    SubMenu = new Menu2(new  MenuItem[]
                    {
                        new MenuItem("Grid Per Player") { SetProperty = "SharedGrid", Value = false, DoAction = 50 },
                        new MenuItem("Shared Grid") { SetProperty = "SharedGrid", Value = true, DoAction = 50 },
                        new CloseMenu("Back"),
                    }) },
                new CloseMenu("Back"),
            }) },
            new OpenMenu("Options") { SubMenu = new Menu2(new MenuItem[]
            {
                new OpenMenu("Video") { SubMenu = new Menu2(new MenuItem[]
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
                new OpenMenu("Audio") { SubMenu = new Menu2(new MenuItem[]
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
            new OpenMenu("Quit") { SubMenu = new Menu2(new MenuItem[]
            {
                new Choice("Quit", new MenuItem[]
                {
                    new MenuItem("Yes") { DoAction = -1 },
                    new CloseMenu("No"),
                }) { StartingIndex = 1 },
            }) },
        });
    }
}
