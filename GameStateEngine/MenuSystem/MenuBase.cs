using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MenuSystem
{
    public abstract class MenuBase : SubMenu
    {
        public enum MenuCommands
        {
            MenuUp,
            MenuDown,
            MenuLeft,
            MenuRight,
            MenuSelect,
            MenuBack,
        }

        [Flags]
        public enum MenuResult
        {
            None = 0x00,
            ChangedSelection = 0x01,
            ChangedChoice = 0x02,
            PerformedAction = 0x04,
            SetProperty = 0x08,
            OpenMenu = 0x10,
            CloseMenu = 0x20,
        }

        public MenuBase(string Title, MenuItem[] MenuItems) :
            base(Title, MenuItems)
        {
            ResetMenu();
        }

        public MenuBase(string Title, MenuItem[] MenuItems, int SelectedIndex) :
            base(Title, MenuItems, SelectedIndex)
        {
            ResetMenu();
        }

        public void ResetMenu()
        {
            _Menus.Clear();
            _Menus.Push(this);
            SetupMenu(this);
        }

        public bool IsMenuActive => _Menus.Count != 0;

        private Stack<SubMenu> _Menus = new Stack<SubMenu>();

        public MenuResult RunMenuCommand(MenuCommands command)
        {
            switch (command)
            {
                case MenuCommands.MenuUp:
                    return RunMenuCommandUp();
                case MenuCommands.MenuDown:
                    return RunMenuCommandDown();
                case MenuCommands.MenuLeft:
                    return RunMenuCommandLeft();
                case MenuCommands.MenuRight:
                    return RunMenuCommandRight();
                case MenuCommands.MenuSelect:
                    return RunMenuCommandSelect();
                case MenuCommands.MenuBack:
                    return RunMenuCommandBack();
            }

            return MenuResult.None;
        }

        public MenuResult RunMenuCommandUp()
        {
            if (_Menus.Count == 0)
                return MenuResult.None;

            return _Menus.Peek().PreviousItem();
        }

        public MenuResult RunMenuCommandDown()
        {
            if (_Menus.Count == 0)
                return MenuResult.None;

            return _Menus.Peek().NextItem();
        }

        public MenuResult RunMenuCommandLeft()
        {
            var rc = MenuResult.None;

            if (_Menus.Count == 0)
                return rc;

            var choice = _Menus.Peek().SelectedItem as Choice;
            if (choice == null)
                return rc;

            //change MenuResult.ChangedSelection to ChangedChoice
            if (choice.PreviousItem() != MenuResult.None)
            {
                rc |= MenuResult.ChangedChoice;

                if (choice.DoActionOnSelect)
                    rc |= HandleSelect(choice.SelectedItem);
                else
                    rc |= HandleProperty(choice.SelectedItem);
            }

            return rc;
        }

        public MenuResult RunMenuCommandRight()
        {
            var rc = MenuResult.None;

            if (_Menus.Count == 0)
                return rc;

            var choice = _Menus.Peek().SelectedItem as Choice;
            if (choice == null)
                return rc;

            //change MenuResult.ChangedSelection to ChangedChoice
            if (choice.NextItem() != MenuResult.None)
            {
                rc |= MenuResult.ChangedChoice;

                if (choice.DoActionOnSelect)
                    rc |= HandleSelect(choice.SelectedItem);
                else
                    rc |= HandleProperty(choice.SelectedItem);
            }

            return rc;
        }

        public MenuResult RunMenuCommandSelect()
        {
            if (_Menus.Count == 0)
                return MenuResult.None;

            var choice = _Menus.Peek().SelectedItem as Choice;
            if (choice != null)
            {
                //skip action if selection trriggers it
                if (choice.DoActionOnSelect)
                    return MenuResult.None;

                return HandleSelect(choice.SelectedItem);
            }

            return HandleSelect(_Menus.Peek().SelectedItem);
        }

        public MenuResult RunMenuCommandBack()
        {
            if (_Menus.Count == 0)
                return MenuResult.None;

            if (_Menus.Peek().CloseOnBack == false)
                return MenuResult.None;

            return CloseMenu();
        }

        private MenuResult HandleProperty(MenuItem Selection)
        {
            if (!string.IsNullOrWhiteSpace(Selection.SetProperty))
            {
                OnSetProperty(Selection.SetProperty, Selection.Value);
                return MenuResult.SetProperty;
            }

            return MenuResult.None;
        }

        private MenuResult HandleSelect(MenuItem Selection)
        {
            MenuResult rc = MenuResult.None;

            if (Selection == null)
                return rc;

            rc |= HandleProperty(Selection);

            if (Selection.DoAction != null)
            {
                rc |= MenuResult.PerformedAction;

                //do something for action
                OnAction(Selection.DoAction);
            }

            var openMenu = Selection as OpenMenu;
            if (openMenu != null)
            {
                rc |= OpenMenu(openMenu.Menu);
            }

            var closeMenu = Selection as CloseMenu;
            if (closeMenu != null)
            {
                rc |= CloseMenu();
            }

            return rc;
        }

        public MenuResult CloseMenu()
        {
            if (_Menus.Count == 0)
                return MenuResult.None;

            _Menus.Pop();
            if (_Menus.Count > 0)
                SetupMenu(_Menus.Peek());

            return MenuResult.CloseMenu;
        }

        public MenuResult OpenMenu(SubMenu menu)
        {
            if ((menu == null) || (menu.MenuItems == null) || (menu.MenuItems.Length == 0))
                return MenuResult.None;

            _Menus.Push(menu);
            SetupMenu(menu);

            return MenuResult.OpenMenu;
        }

        public bool ExitMenu()
        {
            if (_Menus.Count == 0)
                return false;

            _Menus.Clear();
            return true;
        }

        private void SetupMenu(SubMenu menu)
        {
            menu.ResetDefaultIndex();

            foreach (var item in menu.MenuItems)
            {
                var choice = item as Choice;
                if (choice != null)
                {
                    choice.ResetDefaultIndex();
                    HandleProperty(choice.SelectedItem);
                }
            }
        }

        public void Draw(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont, 
            Rectangle MenuRect, bool IncludeMenuTitle, int PixelsBetweenLines = 10)
        {
            if (_Menus.Count == 0)
                return;

            var CurrentMenu = _Menus.Peek();

            DrawMenuBegin(spriteBatch, spriteFont, MenuRect, CurrentMenu);

            var LetterSize = spriteFont.MeasureString("W");

            int ItemCount = CurrentMenu.MenuItems.Length;
            if (IncludeMenuTitle && !string.IsNullOrWhiteSpace(CurrentMenu.Text))
                ItemCount++;

            var TotalLetterHeight = ((int)LetterSize.Y * ItemCount) + (PixelsBetweenLines * (ItemCount - 1));
            int YOffset = (MenuRect.Y) + (MenuRect.Height / 2) - (TotalLetterHeight / 2);

            if (IncludeMenuTitle && !string.IsNullOrWhiteSpace(CurrentMenu.Text))
            {
                var ItemRect = new Rectangle(MenuRect.X, YOffset,
                    MenuRect.Width, (int)LetterSize.Y);

                var ItemSize = spriteFont.MeasureString(CurrentMenu.Text);

                DrawTitle(spriteBatch, spriteFont, MenuRect, CurrentMenu.Text, ItemRect);

                YOffset += (int)LetterSize.Y;
                YOffset += PixelsBetweenLines;
            }

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

            DrawMenuEnd(spriteBatch, spriteFont, MenuRect, CurrentMenu);
        }

        protected abstract void OnSetProperty(string Property, object Value);
        protected abstract void OnAction(object Action);

        protected virtual void DrawMenuBegin(ExtendedSpriteBatch spriteBatch,
            SpriteFont spriteFont, Rectangle MenuRect, SubMenu CurrentMenu) { }
        protected virtual void DrawMenuEnd(ExtendedSpriteBatch spriteBatch,
            SpriteFont spriteFont, Rectangle MenuRect, SubMenu CurrentMenu) { }

        protected abstract void DrawTitle(ExtendedSpriteBatch spriteBatch,
            SpriteFont spriteFont, Rectangle MenuRect,
            string MenuTitle, Rectangle ItemRect);

        protected abstract void DrawChoice(ExtendedSpriteBatch spriteBatch,
            SpriteFont spriteFont, Rectangle MenuRect,
            Choice CurrentItem, Rectangle ItemRect, bool IsSelected, 
            MenuItem[] Choices, MenuItem SelectedChoice);

        protected abstract void DrawItem(ExtendedSpriteBatch spriteBatch, 
            SpriteFont spriteFont, Rectangle MenuRect,
            MenuItem CurrentItem, Rectangle ItemRect, bool IsSelected);
    }
}
