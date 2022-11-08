using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using System.ComponentModel;
using Common;

namespace EventMenu
{
    public enum EventMenuItemActions
    {
        Select,
        Back,
        MenuUp,
        MenuDown,
        MenuLeft,
        MenuRight,
    }

    public class EventHandledEventArgs : EventArgs
    {
        public bool Handled { get; set; } = false;
    }

    public class EventMenuItemEventArgs : EventHandledEventArgs
    {
        public EventMenuItemActions Action { get; set; }
    }

    public class EventMenuItem
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public Point Location { get; set; }
        public Point Size { get; set; } = new Point(400, 400);

        public object Tag { get; set; }

        public EventMenuItemCollection Parent { get; set; }

        public event EventHandler<EventMenuItemEventArgs> Select;
        public event EventHandler<EventMenuItemEventArgs> Back;
        public event EventHandler<EventMenuItemEventArgs> Up;
        public event EventHandler<EventMenuItemEventArgs> Down;
        public event EventHandler<EventMenuItemEventArgs> Left;
        public event EventHandler<EventMenuItemEventArgs> Right;

        public event EventHandler<EventHandledEventArgs> Draw;

        public virtual bool DoAction(EventMenuItemActions Action)
        {
            var e = new EventMenuItemEventArgs()
            {
                Handled = false,
                Action = Action
            };

            switch (Action)
            {
                case EventMenuItemActions.Select: Select?.Invoke(this, e); break;
                case EventMenuItemActions.Back: Back?.Invoke(this, e); break;
                case EventMenuItemActions.MenuUp: Up?.Invoke(this, e); break;
                case EventMenuItemActions.MenuDown: Down?.Invoke(this, e); break;
                case EventMenuItemActions.MenuLeft: Left?.Invoke(this, e); break;
                case EventMenuItemActions.MenuRight: Right?.Invoke(this, e); break;
            }

            return e.Handled;
        }

        public virtual void DoDraw(ExtendedSpriteBatch spriteBatch)
        {
            var e = new EventHandledEventArgs()
            {
                Handled = false,
            };

            Draw?.Invoke(this, e);

            DrawBackground(spriteBatch);
        }

        protected virtual void DrawBackground(ExtendedSpriteBatch spriteBatch)
        {

        }

        protected virtual void DrawItem(ExtendedSpriteBatch spriteBatch)
        {

        }
    }

    public abstract class EventMenuItemCollection : EventMenuItem, System.Collections.IEnumerable
    {
        public BindingList<EventMenuItem> MenuItems { get; } = new BindingList<EventMenuItem>();

        public EventMenuItemCollection()
        {
            MenuItems.ListChanged += (s, e) =>
            {
                if (e.ListChangedType == ListChangedType.ItemAdded)
                {
                    MenuItems[e.NewIndex].Parent = this;
                }
            };
        }

        public void Add(EventMenuItem MenuItem)
        {
            if (MenuItem != null)
                MenuItems.Add(MenuItem);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)MenuItems).GetEnumerator();
        }

        public EventMenuItem this[int index] => MenuItems[index];
        public int Count => MenuItems.Count;
    }


    public abstract class EventMenuItemSelectedCollection : EventMenuItemCollection
    {
        public EventMenuItemSelectedCollection()
        {
            MenuItems.ListChanged += (s, e) =>
            {
                if (e.ListChangedType == ListChangedType.ItemAdded)
                {
                    if (Selection == null)
                        Selection = MenuItems[e.NewIndex];
                }
            };
        }

        public EventMenuItem Selection { get; private set; }
        public event EventHandler<EventArgs> SelectionChanged;
        public int SelectedIndex => MenuItems.IndexOf(Selection);

        public void SetSelection(int ID)
        {
            SetSelectionInternal(MenuItems.Where(x => x.ID == ID).FirstOrDefault());
        }

        public void SetSelectionIndex(int index)
        {
            if ((index > 0) && (index < MenuItems.Count))
                SetSelectionInternal(MenuItems[index]);
        }

        private void SetSelectionInternal(EventMenuItem NewSelection)
        {
            if (NewSelection == null)
                return;

            if (Selection != NewSelection)
            {
                Selection = NewSelection;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsSelected(EventMenuItem menuItem)
        {
            return (menuItem == Selection);
        }
    }
}
