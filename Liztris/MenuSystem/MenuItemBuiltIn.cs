using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MenuSystem
{
    public class OpenMenu : MenuItem
    {
        public OpenMenu(string Text) : base(Text) { }

        public SubMenu Menu { get; set; }
    }

    public class CloseMenu : MenuItem
    {
        public CloseMenu(string Text) : base(Text) { }
    }

    public class SubMenu : MenuItemCollection
    {
        public SubMenu(string Title, MenuItem[] MenuItems) :
            base(Title, MenuItems) { }

        public SubMenu(string Title, MenuItem[] MenuItems, int SelectedIndex) :
            base(Title, MenuItems, SelectedIndex) { }
    }

    public class Choice : MenuItemCollection
    {
        public Choice(string Text, MenuItem[] MenuItems) :
            base(Text, MenuItems) { }

        public Choice(string Text, MenuItem[] MenuItems, int SelectedChoice) :
            base(Text, MenuItems, SelectedChoice) { }
    }
}
