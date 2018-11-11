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
        public Dictionary<string, object> Options { get; } = new Dictionary<string, object>();
        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();
        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(10);

        public bool Update(GameTime gameTime)
        {
            if (_Menus.Count == 0)
                //throw new Exception();
                return false;

            var CurrentMenu = _Menus.Peek();

            if (AnimationTimer.UpdateAndCheck(gameTime))
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
            }

            inputManager.Update(PlayerIndex.One);

            if (inputManager.IsActionTriggered(MenuCommands.MenuSelect))
            {
                HandleSelectItem(CurrentMenu.SelectedItem);
                return IsMenuActive;
            }

            if (inputManager.IsActionTriggered(MenuCommands.MenuBack))
            {
                CloseMenu();
                return IsMenuActive;
            }

            if (inputManager.IsActionTriggered(MenuCommands.MenuUp))
            {
                CurrentMenu.PreviousItem();
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuDown))
            {
                CurrentMenu.NextItem();
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuLeft))
            {
                var choiceMenu = CurrentMenu.SelectedItem as Choice;
                if (choiceMenu != null)
                {
                    if (choiceMenu.PreviousItem())
                        HandleSelectItem(choiceMenu.SelectedItem);
                }
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuRight))
            {
                var choiceMenu = CurrentMenu.SelectedItem as Choice;
                if (choiceMenu != null)
                {
                    if (choiceMenu.NextItem())
                        HandleSelectItem(choiceMenu.SelectedItem);
                }
            }

            return IsMenuActive;
        }


        public void HandleSelectItem(MenuItem Selection)
        {
            if (Selection == null)
                return;

            if (!string.IsNullOrWhiteSpace(Selection.SetProperty))
            {
                Options[Selection.SetProperty] = Selection.Value;
                System.Diagnostics.Debug.Print("Set {0} to {1}",
                    Selection.SetProperty, Selection.Value);
            }

            if (Selection.DoAction != null)
            {
                //do something for action
                System.Diagnostics.Debug.Print("Do Action {0}",
                    Selection.DoAction);
            }

            var openMenu = Selection as OpenMenu;
            if (openMenu != null)
            {
                OpenMenu(openMenu.SubMenu);
            }

            var closeMenu = Selection as CloseMenu;
            if (closeMenu != null)
            {
                CloseMenu();
            }
        }

        private void CloseMenu()
        {
            _Menus.Pop();
            if (_Menus.Count > 0)
                _Menus.Peek().SetDefaultItem();
        }

        private void OpenMenu(SubMenu menu)
        {
            _Menus.Push(menu);
            menu.SetDefaultItem();
        }

        public void Draw(Common.ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, Rectangle MenuRect)
        {
            if (_Menus.Count == 0)
                return;

            var CurrentMenu = _Menus.Peek();

            var sz = spriteFont.MeasureString("W");
            int linepad = 10;

            var height = ((int)sz.Y * CurrentMenu.MenuItems.Length) + (linepad * (CurrentMenu.MenuItems.Length - 1));
            //spriteBatch.FillRectangle(new Rectangle(MenuRect.X, MenuRect.Y, 100, height), Color.Purple);

            int offset = (MenuRect.Y) + (MenuRect.Height / 2) - (height / 2);
            //spriteBatch.DrawRectangle(new Rectangle(MenuRect.X + 100, offset, 100, height), Color.Purple);

            offset += (int)(sz.Y / 2); //offset start as we scale the font


            for (int i = 0; i < CurrentMenu.MenuItems.Length; i++)
            {
                sz = spriteFont.MeasureString(CurrentMenu.MenuItems[i].Text);
                var c = Color.White;
                var scale = 1.0f;

                var choice = CurrentMenu.MenuItems[i] as Choice;
                if (choice != null)
                {
                    spriteBatch.DrawString(spriteFont, choice.Text,
                        new Vector2(MenuRect.X + (MenuRect.Width / 2) - ((sz.X * scale) / 2),
                        offset - (sz.Y / 2 * scale)), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                    int x = 200;
                    for (int j = 0; j < choice.MenuItems.Length; j++)
                    {

                        spriteBatch.DrawString(spriteFont, choice.MenuItems[j].Text,
                            new Vector2(MenuRect.X + (MenuRect.Width / 2) - ((sz.X * scale) / 2) + x,
                            offset - (sz.Y / 2 * scale)), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                        x += 50;
                    }


                }
                else
                {
                    if (CurrentMenu.MenuItems[i] == CurrentMenu.SelectedItem)
                    {
                        c = Color.LightGreen;
                        scale = _scale;
                    }

                    spriteBatch.DrawString(spriteFont, CurrentMenu.MenuItems[i].Text,
                            new Vector2(MenuRect.X + (MenuRect.Width / 2) - ((sz.X * scale) / 2),
                            offset - (sz.Y / 2 * scale)), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                }
                offset += (int)sz.Y;
                offset += linepad;
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

        public int? StartingIndex;

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
                new MenuItem("1 Player") { SetProperty = "Players", Value = 1, DoAction = 50 },
                new OpenMenu("2 Player") { SetProperty = "Players", Value = 2,
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
                new Choice("Fullscreen", new MenuItem[]
                {
                    new MenuItem("No") { SetProperty = "Fullscreen", Value = false },
                    new MenuItem("Yes") { SetProperty = "Fullscreen", Value = true },
                }),
                new MenuItem("Apply") { DoAction = 99 },
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
