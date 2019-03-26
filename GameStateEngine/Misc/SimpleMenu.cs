using Common.MenuSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Misc
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

        public void SetPropertyValue(string Property, object value)
        {
            Options[Property] = value;

            SetIndex(null, this, Property, value);
        }

        private static void SetIndex(MenuItemCollection parent, MenuItem item, string Property, object value)
        {
            var collect = item as MenuItemCollection;
            if (collect != null)
            {
                foreach (var m in collect.MenuItems)
                    SetIndex(collect, m, Property, value);
            }

            var open = item as OpenMenu;
            if (open != null)
            {
                foreach (var m in open.Menu.MenuItems)
                    SetIndex(open.Menu, m, Property, value);
            }

            if (item.SetProperty == null)
                return;

            if ((item.SetProperty == Property) && (object.Equals(item.Value, value)) && (parent != null))
            {
                parent.SelectedItem = item;
            }
        }

        InputManager<MenuCommands> inputManager = new InputManager<MenuCommands>();
        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(20);
        object _LastAction = null;

        public MenuResult Update(GameTime gameTime, out object MenuAction)
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

            var rc = MenuResult.None;
            _LastAction = null;

            foreach (var action in new MenuCommands[] {
                MenuCommands.MenuUp, MenuCommands.MenuDown, MenuCommands.MenuLeft,
                MenuCommands.MenuRight,MenuCommands.MenuSelect, MenuCommands.MenuBack})
            {
                if (inputManager.IsActionTriggered(action))
                {
                    rc = RunMenuCommand(action);
                    break;
                }
            }

            MenuAction = _LastAction;
            return rc;
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

            _LastAction = Action;

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
            ItemRect.Offset(2, 2);

            spriteBatch.DrawString(spriteFont, MenuTitle, ItemRect,
                ExtendedSpriteBatch.Alignment.Center, Color.Black, 1.5f);

            ItemRect.Offset(-2, -2);

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
            var scale = 1.0f;

            if (IsSelected)
            {
                c = Color.Aquamarine;
                //scale = _scale;
            }

            var r = new Rectangle(ItemRect.X, ItemRect.Y, (int)size.X, ItemRect.Height);
            spriteBatch.DrawString(spriteFont, CurrentItem.Text, r,
                ExtendedSpriteBatch.Alignment.Center, c, scale);

            int x = ItemRect.Left + (int)size.X + PixelsBetweenChoices;


            if (Choices.Length <= 2)
            {
                foreach (var choice in Choices)
                {
                    c = Color.White;
                    scale = 1.0f;

                    if (choice == SelectedChoice)
                    {
                        c = Color.LightBlue;

                        if (IsSelected)
                            scale = _scale;
                    }

                    size = spriteFont.MeasureString(choice.Text);
                    r = new Rectangle(x, ItemRect.Y, (int)size.X, ItemRect.Height);

                    spriteBatch.DrawString(spriteFont, choice.Text, r,
                        ExtendedSpriteBatch.Alignment.Center, c, scale);

                    x += (int)size.X + PixelsBetweenChoices;
                }
            }
            else
            {
                bool HasLeft = Choices[0] != SelectedChoice;
                bool HasRight = Choices[Choices.Length - 1] != SelectedChoice;

                //draw left arrow
                if (HasLeft)
                {
                    var sz = r.Height / 4;

                    if (IsSelected)
                        sz = (int)(sz * _scale);

                    var offsetY = r.Height / 2 - sz - 2;
                    var offsetX = sz - r.Height / 4;

                    spriteBatch.DrawEquilateralTriangle(
                        new Vector2(x - offsetX, ItemRect.Y + offsetY), sz, Color.White, 270, 3f);

                    x += (r.Height / 4) + PixelsBetweenChoices;
                }

                //draw selected item
                c = Color.LightBlue;

                size = spriteFont.MeasureString(SelectedChoice.Text);
                r = new Rectangle(x, ItemRect.Y, (int)size.X, ItemRect.Height);

                spriteBatch.DrawString(spriteFont, SelectedChoice.Text, r,
                    ExtendedSpriteBatch.Alignment.Center, c, scale);

                x += (int)size.X + PixelsBetweenChoices;


                //draw right arrow
                if (HasRight)
                {
                    var sz = r.Height / 4;

                    if (IsSelected)
                        sz = (int)(sz * _scale);

                    var offset = r.Height / 2 - sz - 2;

                    spriteBatch.DrawEquilateralTriangle(
                        new Vector2(x, ItemRect.Y + offset), sz, Color.White, 90, 3f);

                    x += (r.Height / 4) + PixelsBetweenChoices;
                }
            }
        }
    }
}
