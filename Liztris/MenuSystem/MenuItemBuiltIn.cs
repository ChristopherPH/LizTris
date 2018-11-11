using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MenuSystem
{
    public class OpenMenu : MenuItem
    {
        public OpenMenu(string Text) : base(Text)
        {
            this.SubMenu = SubMenu;
        }

        public SubMenu SubMenu { get; set; }
    }

    public class CloseMenu : MenuItem
    {
        public CloseMenu(string Text) : base(Text) { }
    }

    public class SubMenu : MenuItemCollection
    {
        public SubMenu(string Text, MenuItem[] MenuItems) :
            base(Text, MenuItems)
        { }
    }

    public class Choice : MenuItemCollection
    {
        public Choice(string Text, MenuItem[] MenuItems) :
            base(Text, MenuItems)
        { }
    }
}
