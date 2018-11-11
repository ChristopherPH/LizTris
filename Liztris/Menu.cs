using Common;
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
    public class Menu<T> where T : struct, IComparable
    {
        public class MenuItem
        {
            public MenuItem() { }
            public MenuItem(string Title, T value)
            {
                this.Title = Title;
                this.value = value;
            }

            public string Title { get; set; }
            public T value { get; set; }
        }

        public Menu(MenuItem[] MenuItems, SpriteFont font, int StartingIndex = 0, T? BackSelection = null)
        {
            Font = font;
            this.MenuItems = MenuItems;
            this.StartingIndex = StartingIndex;
            this.SelectedIndex = StartingIndex;
            this.BackSelection = BackSelection;

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

            ResetMenu();
        }

        public MenuItem[] MenuItems { get; private set; }

        SpriteFont Font;
        int SelectedIndex;
        int StartingIndex;
        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(10);
        T? BackSelection;


        enum MenuCommands
        {
            MenuBack,
            MenuSelect,
            MenuUp,
            MenuDown,
            MenuLeft,
            MenuRight,
        }

        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();

        public void ResetMenu()
        {
            inputManager.Update(PlayerIndex.One);
            this.SelectedIndex = StartingIndex;
        }

        public bool Update(GameTime gameTime, out T? Selection)
        {
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
                //valid option
                Selection = MenuItems[SelectedIndex].value;
                return true;
            }

            if (inputManager.IsActionTriggered(MenuCommands.MenuBack))
            {
                Selection = BackSelection;
                return true;
            }

            //no option selected
            Selection = null;

            if (inputManager.IsActionTriggered(MenuCommands.MenuUp))
            {
                if (SelectedIndex > 0)
                {
                    SelectedIndex--;
                    _scale = 1;
                    _scaleReverse = false;
                }
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuDown))
            {
                if (SelectedIndex < MenuItems.Length - 1)
                {
                    SelectedIndex++;
                    _scale = 1;
                    _scaleReverse = false;
                }
            }

            return false;
        }

        public void Draw(Common.ExtendedSpriteBatch spriteBatch, Rectangle MenuRect)
        {
            var sz = Font.MeasureString("W");
            int linepad = 10;

            var height = ((int)sz.Y * MenuItems.Length) + (linepad * (MenuItems.Length - 1));
            //spriteBatch.FillRectangle(new Rectangle(MenuRect.X, MenuRect.Y, 100, height), Color.Purple);

            int offset = (MenuRect.Y) + (MenuRect.Height / 2) - (height / 2);
            //spriteBatch.DrawRectangle(new Rectangle(MenuRect.X + 100, offset, 100, height), Color.Purple);

            offset += (int)(sz.Y / 2); //offset start as we scale the font
            

            for (int i = 0; i < MenuItems.Length; i++)
            {
                sz = Font.MeasureString(MenuItems[i].Title);
                var c = Color.White;
                var scale = 1.0f;

                if (i == SelectedIndex)
                {
                    c = Color.LightGreen;
                    scale = _scale;
                }

                spriteBatch.DrawString(Font, MenuItems[i].Title,
                        new Vector2(MenuRect.X + (MenuRect.Width / 2) - ((sz.X * scale) / 2),
                        offset - (sz.Y/2 * scale)), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        //offset ), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                offset += (int)sz.Y;
                offset += linepad;
            }
        }
    }
}
