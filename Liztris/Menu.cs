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
    public class Menu
    {
        public Menu(string[] Options, SpriteFont font, int Choice = 0)
        {
            Font = font;
            this.Options = Options;
            this._Choice = Choice;

            inputManager.AddAction(MenuCommands.MenuSelect, Keys.Enter);
            inputManager.AddAction(MenuCommands.MenuSelect, InputManager<MenuCommands>.GamePadButtons.A);

            inputManager.AddAction(MenuCommands.MenuBack, Keys.Escape);
            inputManager.AddAction(MenuCommands.MenuBack, InputManager<MenuCommands>.GamePadButtons.B);

            inputManager.AddAction(MenuCommands.MenuUp, Keys.Up);
            inputManager.AddAction(MenuCommands.MenuUp, InputManager<MenuCommands>.GamePadButtons.Up);

            inputManager.AddAction(MenuCommands.MenuDown, Keys.Down);
            inputManager.AddAction(MenuCommands.MenuDown, InputManager<MenuCommands>.GamePadButtons.Down);

            inputManager.Update(PlayerIndex.One);
        }

        SpriteFont Font;
        string[] Options;
        int _Choice = 0;
        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(10);

        enum MenuCommands
        {
            MenuBack,
            MenuSelect,
            MenuUp,
            MenuDown,
        }

        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();

        public bool Update(GameTime gameTime, out int Choice)
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
                Choice = _Choice;
                return true;
            }

            Choice = -1;

            if (inputManager.IsActionTriggered(MenuCommands.MenuBack))
                return true;

            if (inputManager.IsActionTriggered(MenuCommands.MenuUp))
            {
                if (_Choice > 0)
                {
                    _Choice--;
                    _scale = 1;
                    _scaleReverse = false;
                }
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuDown))
            {
                if (_Choice < Options.Length - 1)
                {
                    _Choice++;
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

            var height = ((int)sz.Y * Options.Length) + (linepad * (Options.Length - 1));
            //spriteBatch.FillRectangle(new Rectangle(MenuRect.X, MenuRect.Y, 100, height), Color.Purple);

            int offset = (MenuRect.Y) + (MenuRect.Height / 2) - (height / 2);
            //spriteBatch.DrawRectangle(new Rectangle(MenuRect.X + 100, offset, 100, height), Color.Purple);

            offset += (int)(sz.Y / 2); //offset start as we scale the font
            

            for (int i = 0; i < Options.Length; i++)
            {
                sz = Font.MeasureString(Options[i]);
                var c = Color.White;
                var scale = 1.0f;

                if (i == _Choice)
                {
                    c = Color.LightGreen;
                    scale = _scale;
                }

                spriteBatch.DrawString(Font, Options[i],
                        new Vector2(MenuRect.X + (MenuRect.Width / 2) - ((sz.X * scale) / 2),
                        offset - (sz.Y/2 * scale)), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                        //offset ), c, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

                offset += (int)sz.Y;
                offset += linepad;
            }
        }
    }
}
