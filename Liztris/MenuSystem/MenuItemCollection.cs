using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MenuSystem
{
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

        public int? StartingIndex
        {
            get { return _StartingIndex; }
            set { _StartingIndex = value; SetDefaultItem(); }
        }
        private int? _StartingIndex;

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
}
