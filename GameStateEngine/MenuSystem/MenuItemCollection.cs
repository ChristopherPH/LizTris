using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MenuSystem
{
    public abstract class MenuItemCollection : MenuItem
    {
        public MenuItemCollection(string Text, MenuItem[] MenuItems, int SelectedIndex = 0)
            : base(Text)
        {
            this.MenuItems = MenuItems;
            this.SelectedIndex = SelectedIndex;

            if (this.SelectedIndex < 0)
                this.SelectedIndex = 0;
            if (this.SelectedIndex > this.MenuItems.Length - 1)
                this.SelectedIndex = this.MenuItems.Length - 1;
        }

        /// <summary>
        /// List of collection items
        /// </summary>
        public MenuItem[] MenuItems;

        /// <summary>
        /// Currently selected index in collection
        /// </summary>
        private int SelectedIndex = 0;

        /// <summary>
        /// Currently selected item in collection
        /// </summary>
        public MenuItem SelectedItem
        {
            get => MenuItems[SelectedIndex];
            set
            {
                if (MenuItems.Contains(value))
                    SelectedIndex = MenuItems.ToList().IndexOf(value);
            }
        }

        /// <summary>
        /// Index to set menu to when opened. If this is not
        /// set, the last SelectedIndex will be used
        /// </summary>
        public int? DefaultIndex
        {
            get { return _DefaultIndex; }
            set { _DefaultIndex = value; ResetDefaultIndex(); }
        }
        private int? _DefaultIndex;

        public void ResetDefaultIndex()
        {
            if (DefaultIndex != null)
            {
                SelectedIndex = DefaultIndex.Value;

                if (SelectedIndex < 0)
                    SelectedIndex = 0;
                if (SelectedIndex > MenuItems.Length - 1)
                    SelectedIndex = MenuItems.Length - 1;
            }
        }

        public MenuBase.MenuResult NextItem()
        {
            if (SelectedIndex >= MenuItems.Length - 1)
                return MenuBase.MenuResult.None;

            SelectedIndex++;
            return MenuBase.MenuResult.ChangedSelection;
        }

        public MenuBase.MenuResult PreviousItem()
        {
            if (SelectedIndex <= 0)
                return MenuBase.MenuResult.None;

            SelectedIndex--;
            return MenuBase.MenuResult.ChangedSelection;
        }
    }
}
