using Common;
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
    public class Menu2 : SubMenu
    {
        enum MenuCommands
        {
            MenuBack,
            MenuSelect,
            MenuUp,
            MenuDown,
            MenuLeft,
            MenuRight,
        }

        public Menu2(MenuItem[] MenuItems) :
            base(string.Empty, MenuItems)
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

            Reset();
        }

        public void Reset()
        {
            _Menus.Clear();
            _Menus.Push(this);
        }

        public bool IsMenuActive => _Menus.Count != 0;

        private Stack<SubMenu> _Menus = new Stack<SubMenu>();
        public SerializableDictionary<string, object> Options { get; } = new SerializableDictionary<string, object>();
        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();
        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(20);

        public bool Update(GameTime gameTime)
        {
            if (_Menus.Count == 0)
                //throw new Exception();
                return false;

            var CurrentMenu = _Menus.Peek();

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
                System.Diagnostics.Debug.Print("{0}", gameTime.TotalGameTime.TotalMilliseconds);
            });

            inputManager.Update(PlayerIndex.One);

            //handle choices
            var choiceMenu = CurrentMenu.SelectedItem as Choice;

            if ((choiceMenu != null) &&
                inputManager.IsActionTriggered(MenuCommands.MenuSelect) &&
                HandleSelectItem(choiceMenu.SelectedItem))
            {
                //play sound
            }
            else if ((choiceMenu != null) &&
                inputManager.IsActionTriggered(MenuCommands.MenuLeft) &&
                choiceMenu.PreviousItem() &&
                HandleSetProperty(choiceMenu.SelectedItem))
            {
                //play sound
            }
            else if ((choiceMenu != null) &&
                inputManager.IsActionTriggered(MenuCommands.MenuRight) &&
                choiceMenu.NextItem() &&
                HandleSetProperty(choiceMenu.SelectedItem))
            {
                //play sound
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuSelect) && 
                HandleSelectItem(CurrentMenu.SelectedItem))
            {
                //play sound
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuBack) &&
                CloseMenu())
            {
                //play sound
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuUp) &&
                CurrentMenu.PreviousItem())
            {
                //play sound
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuDown) &&
                CurrentMenu.NextItem())
            {
                //play sound
            }
            
            return IsMenuActive;
        }


        public bool HandleSetProperty(MenuItem Selection)
        {
            if (!string.IsNullOrWhiteSpace(Selection.SetProperty))
            {
                Options[Selection.SetProperty] = Selection.Value;
                System.Diagnostics.Debug.Print("Set {0} to {1}",
                    Selection.SetProperty, Selection.Value);

                return true;
            }

            return false;
        }


        public bool HandleSelectItem(MenuItem Selection)
        {
            if (Selection == null)
                return false;

            bool rc = false;

            if (HandleSetProperty(Selection))
            {
                rc = true;
            }

            if (Selection.DoAction != null)
            {
                //do something for action
                System.Diagnostics.Debug.Print("Do Action {0}",
                    Selection.DoAction);
                rc = true;
            }

            var openMenu = Selection as OpenMenu;
            if (openMenu != null)
            {
                OpenMenu(openMenu.SubMenu);
                rc = true;
            }

            var closeMenu = Selection as CloseMenu;
            if (closeMenu != null)
            {
                CloseMenu();
                rc = true;
            }

            return rc;
        }

        private bool CloseMenu()
        {
            if (_Menus.Count == 0)
                return false;

            _Menus.Pop();
            if (_Menus.Count > 0)
                _Menus.Peek().SetDefaultItem();

            return true;
        }

        private bool OpenMenu(SubMenu menu)
        {
            if ((menu == null) || (menu.MenuItems == null) || (menu.MenuItems.Length == 0))
                return false;

            _Menus.Push(menu);
            menu.SetDefaultItem();
            return true;
        }

        public void Draw(Common.ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect)
        {
            if (_Menus.Count == 0)
                return;

            var CurrentMenu = _Menus.Peek();

            Draw(_Menus.Peek(), spriteBatch, spriteFont, MenuRect, _scale, Color.Aquamarine);
        }

        private void DrawBackground(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect)
        {
            spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

            //spriteBatch.Draw(transparentDarkTexture, MenuRect, Color.White* 0.5f);
        }

        private void DrawItem(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect, 
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

        private void DrawChoice(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect,
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

        private void Draw(SubMenu CurrentMenu, ExtendedSpriteBatch spriteBatch, 
            SpriteFont spriteFont, Rectangle MenuRect, float SelectedItemScale, Color SelectedItemColor)
        {
            var LetterSize = spriteFont.MeasureString("W");
            int PixelsBetweenLines = 10;

            int ItemCount = CurrentMenu.MenuItems.Length;
            var TotalLetterHeight = ((int)LetterSize.Y * ItemCount) + (PixelsBetweenLines * (ItemCount - 1));
            int YOffset = (MenuRect.Y) + (MenuRect.Height / 2) - (TotalLetterHeight / 2);


            foreach (var CurrentItem in CurrentMenu.MenuItems)
            {
                var ItemRect = new Rectangle(MenuRect.X, YOffset, 
                    MenuRect.Width, (int)LetterSize.Y);

                var ItemSize = spriteFont.MeasureString(CurrentItem.Text);

                var choice = CurrentItem as Choice;
                if (choice != null)
                {
                    DrawChoice(spriteBatch, spriteFont, MenuRect, choice, ItemRect,
                        CurrentItem == CurrentMenu.SelectedItem, choice.MenuItems, choice.SelectedItem);
                }
                else
                {
                    DrawItem(spriteBatch, spriteFont, MenuRect, CurrentItem, ItemRect, 
                        CurrentItem == CurrentMenu.SelectedItem);
                }

                YOffset += (int)LetterSize.Y;
                YOffset += PixelsBetweenLines;
            }
        }
    }

    public class MenuItem
    {
        public MenuItem(string Text)
        {
            this.Text = Text;
        }
        //public MenuItem2(string Text, string Key, object Value) { }
        //public MenuItem2(string Text, object Action) { }

        public string Text { get; set; }
        public string SetProperty { get; set; }
        public object Value { get; set; }
        public object DoAction { get; set; }
    }

    public class OpenMenu : MenuItem
    {
        public OpenMenu(string Text) : base(Text)
        {
            this.SubMenu = SubMenu;
        }

        public Menu2 SubMenu { get; set; }
    }

    public class CloseMenu : MenuItem
    {
        public CloseMenu(string Text) : base(Text) { }
    }

    public abstract class MenuItemCollection : MenuItem
    {
        public MenuItemCollection(string Text, MenuItem[] MenuItems)
            : base(Text)
        {
            this.MenuItems = MenuItems;

            SetDefaultItem();
        }

        public MenuItem[] MenuItems;
        private int SelectedIndex = 0;

        public int? StartingIndex
        {
            get { return _StartingIndex; }
            set { _StartingIndex = value; SetDefaultItem(); }
        }
        private int? _StartingIndex;

        public void SetDefaultItem()
        {
            if (StartingIndex != null)
            {
                SelectedIndex = StartingIndex.Value;

                if (SelectedIndex < 0)
                    SelectedIndex = 0;
                if (SelectedIndex > MenuItems.Length - 1)
                    SelectedIndex = MenuItems.Length - 1;
            }
            else
            {
                SelectedIndex = 0;
            }
        }

        public bool NextItem()
        {
            if (SelectedIndex >= MenuItems.Length - 1)
                return false;

            SelectedIndex++;
            return true;
        }

        public bool PreviousItem()
        {
            if (SelectedIndex <= 0)
                return false;

            SelectedIndex--;
            return true;
        }

        public MenuItem SelectedItem => MenuItems[SelectedIndex];
    }

    public class SubMenu : MenuItemCollection
    {
        public SubMenu(string Text, MenuItem[] MenuItems) :
            base(Text, MenuItems) { }
    }

    public class Choice : MenuItemCollection
    {
        public Choice(string Text, MenuItem[] MenuItems) :
            base(Text, MenuItems) { }
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
