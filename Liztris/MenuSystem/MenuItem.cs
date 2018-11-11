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
        //public MenuItem2(string Text, string Key, object Value) { }
        //public MenuItem2(string Text, object Action) { }

        public string Text { get; set; }
        public string SetProperty { get; set; }
        public object Value { get; set; }
        public object DoAction { get; set; }
    }
}
