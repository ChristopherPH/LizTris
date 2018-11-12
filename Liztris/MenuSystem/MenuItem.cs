using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.MenuSystem
{
    public class MenuItem
    {
        public MenuItem(string Text)
        {
            this.Text = Text;
        }

        public string Text { get; }

        public string SetProperty { get; set; }
        public object Value { get; set; }
        public object DoAction { get; set; }
    }
}
