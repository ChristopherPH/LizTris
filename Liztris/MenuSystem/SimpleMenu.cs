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

namespace Common.MenuSystem
{
    public class SimpleMenu : MenuBase
    {
        public SimpleMenu(string Title, MenuItem[] MenuItems) : base(Title, MenuItems)
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

        public Action<object> ActionHandler { get; set; }

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

        public void ResetInputs()
        {
            inputManager.Update(PlayerIndex.One);
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

            if (ActionHandler != null)
                ActionHandler(Action);
        }

        protected override void DrawMenuBegin(ExtendedSpriteBatch spriteBatch, 
            SpriteFont spriteFont, Rectangle MenuRect, SubMenu CurrentMenu)
        {
            spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

            //spriteBatch.Draw(transparentDarkTexture, MenuRect, Color.White* 0.5f);
        }

        protected override void DrawTitle(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont,
            Rectangle MenuRect, string MenuTitle, Rectangle ItemRect)
        {
            spriteBatch.DrawString(spriteFont, MenuTitle, ItemRect,
                ExtendedSpriteBatch.Alignment.Center, Color.DarkTurquoise, 1.5f);
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
}
