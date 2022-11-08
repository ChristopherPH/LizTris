using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common.GameMenu
{
    /// <summary>
    /// Base Menu Item. Defines a Name and Location for the item.
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(Shape))]
    [XmlInclude(typeof(Label))]
    [XmlInclude(typeof(MenuDefinition))]
    [XmlInclude(typeof(CustomAction))]
    [XmlInclude(typeof(OpenMenu))]
    [XmlInclude(typeof(CloseMenu))]
    [XmlInclude(typeof(BoolValue))]
    [XmlInclude(typeof(IntRange))]
    public abstract class MenuItemBase
    {
        [XmlAttribute]
        [DefaultValue(null)]
        public string Name { get; set; }

        [XmlAttribute, DefaultValue(0)]
        public int X { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int Y { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int Width { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int Height { get; set; }

        [XmlIgnore]
        public Rectangle Location
        {
            get { return new Rectangle(X, Y, Width, Height); }
            set { X = value.X; Y = value.Y; Width = value.Width; Height = value.Height;}
        }

        public abstract void Draw(SpriteBatch spriteBatch);
    }

    public enum ItemAlignment
    {
        TopLeft,
        MiddleLeft,
        BottomLeft,
        TopCenter,
        MiddleCenter,
        BottomCenter,
        TopRight,
        MiddleRight,
        BottomRight
    }

    /// <summary>
    /// Base Menu Item that displays a shape
    /// </summary>
    [Serializable]
    public class Shape : MenuItemBase
    {
        public override void Draw(SpriteBatch spriteBatch)
        {

        }
    }


    /// <summary>
    /// Base Menu Item that displays text
    /// </summary>
    [Serializable]
    public class Label : MenuItemBase
    {
        [XmlAttribute]
        [DefaultValue(ItemAlignment.TopLeft)]
        public ItemAlignment Alignment { get; set; } = ItemAlignment.TopLeft;

        /// <summary>
        /// String to display
        /// </summary>
        [XmlAttribute]
        public string Text { get; set; } = string.Empty;

        /* Hidden properties */
        [XmlIgnore]     public SpriteFont _LabelFont = null;
        [XmlIgnore]     public Color _LabelColor = Color.Black;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_LabelFont != null)
                spriteBatch.DrawString(_LabelFont, Text, new Vector2(X, Y), _LabelColor);
        }
    }

    /// <summary>
    /// Base menu item that displays text, and can be selected (actions, submenus, etc)
    /// </summary>
    [Serializable]
    public abstract class MenuItemSelectionBase : Label
    {
    }

    public class MenuDefinition : MenuItemBase
    {
        public MenuItemBase[] MenuItems { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (MenuItems != null)
                foreach (var item in MenuItems)
                    item.Draw(spriteBatch);
        }
    }

    public class MenuItemActionArgs : EventArgs
    {

    }

    public abstract class MenuItemActionBase : MenuItemSelectionBase
    {
        public EventHandler<MenuItemActionArgs> Action;

    }

    public class CustomAction : MenuItemSelectionBase
    {
        [XmlAttribute]
        public string Action { get; set; } = string.Empty;
    }

    public class OpenMenu : MenuItemActionBase
    {
        public MenuDefinition Menu { get; set; }
    }

    public class CloseMenu : MenuItemActionBase
    {
    }

    public abstract class MenuItemPropertyBase : MenuItemSelectionBase
    {
        [XmlAttribute]
        public string Property { get; set; } = string.Empty;

        [XmlAttribute]
        [DefaultValue(ItemAlignment.TopLeft)]
        public ItemAlignment ValueAlignment { get; set; } = ItemAlignment.TopLeft;

        [XmlAttribute, DefaultValue(0)]
        public int ValueX { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int ValueY { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int ValueWidth { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int ValueHeight { get; set; }

        [XmlIgnore]
        public Rectangle ValueLocation
        {
            get { return new Rectangle(ValueX, ValueY, ValueWidth, ValueHeight); }
            set { ValueX = value.X; ValueY = value.Y; ValueWidth = value.Width; ValueHeight = value.Height; }
        }

        public EventHandler<MenuItemPropertyArgs> GetValue;
    }

    public class MenuItemPropertyArgs : EventArgs
    {

    }

    public class BoolValue : MenuItemPropertyBase
    {

    }

    public class IntRange : MenuItemPropertyBase
    {
        public IntRange() { }
        public IntRange(int Min, int Max, int Step)
        {
            this.Min = Min;
            this.Max = Max;
            this.Step = Step;
        }

        [XmlAttribute, DefaultValue(0)]
        public int Min { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int Max { get; set; }
        [XmlAttribute, DefaultValue(0)]
        public int Step { get; set; }
    }


    public class MenuInstance
    {
        Stack<MenuDefinition> menus = new Stack<MenuDefinition>();

        public MenuInstance(MenuDefinition definition, SpriteFont spriteFont)
        {
            menus.Push(definition);

            foreach (var i in InternalRecurse(definition, true).OfType<Label>())
                i._LabelFont = spriteFont;
        }

        public void Draw(ExtendedSpriteBatch spriteBatch)
        {
            if (menus.Count == 0)
                return;

            menus.Peek().Draw(spriteBatch);
        }

        private static IEnumerable<MenuItemBase> InternalRecurse(MenuItemBase Item, bool IncludeItem)
        {
            if (Item == null)
                yield break;

            if (IncludeItem)
                yield return Item;

            var SubMenu = Item as MenuDefinition;
            if (SubMenu == null)
                yield break;

            foreach (var SubMenuItem in SubMenu.MenuItems)
                foreach (var i in InternalRecurse(SubMenuItem, true))
                    yield return i;
        }
    }

    public static class MenuTest
    {
        public static MenuDefinition _Menu = new MenuDefinition()
        {
            MenuItems = new MenuItemBase[]
            {
                new Label() { Text = "Main Menu", Location = new Rectangle(0, 0, 100, 500) },

                new OpenMenu() { Text = "New Game", Menu = new MenuDefinition() { MenuItems = new MenuItemBase[]
                {
                    new Label() { Text = "Options" },
                    new Label() { Text = "Video"},
                    new OpenMenu() { Text = "Audio", Menu = new MenuDefinition() { MenuItems = new MenuItemBase[]
                    {
                        new IntRange(0, 100, 5) { Text = "Sound Volume" },
                        new IntRange(0, 100, 5) { Text = "Music Volume" },
                    } } },
                    new Label() { Text = "Controls"},
                    new CloseMenu() { Text = "Back" },
                } } },
                new OpenMenu() { Text = "Exit Game", Menu = new MenuDefinition() { MenuItems = new MenuItemBase[]
                {
                    new Label() { Text = "Are You Sure?" },
                    new CustomAction() { Text = "Yes", Action = "Exit" },
                    new CloseMenu() { Text = "No" },
                } } },
            }
        };

        public static void Savemenu()
        {
            var s = ParseHelpers.GetXML(_Menu, null);
            System.Diagnostics.Debug.Print(s);
        }
    }
}
