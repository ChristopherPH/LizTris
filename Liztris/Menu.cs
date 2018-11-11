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

        }

        SpriteFont Font;
        string[] Options;
        int _Choice = 0;

        enum MenuCommands
        {
            MenuBack,
            MenuSelect,
            MenuUp,
            MenuDown,
        }

        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();

        public bool Update(out int Choice)
        {
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
                    _Choice--;
            }
            else if (inputManager.IsActionTriggered(MenuCommands.MenuDown))
            {
                if (_Choice < Options.Length - 1)
                    _Choice++;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle MenuRect)
        {
            int offset = MenuRect.Y;

            for (int i = 0; i < Options.Length; i++)
            {
                var c = Color.White;
                if (i == _Choice)
                    c = Color.Green;

                spriteBatch.DrawString(Font, Options[i],
                     new Vector2(MenuRect.X, offset), c);

                offset += 40;
            }
        }
    }
}
