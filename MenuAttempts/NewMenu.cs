using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *      Resume
 *      New Game
 *      Quit
 * 
 * 
 *      New Game
 *          1 Player
 *              Controls: <- Keyboard, Joystick ->
 *              Speed: <- Slowest, Slow, Normal, Fast, Fastest ->
 *              Start Game
 *              
 *              --------
 *              
 *          Players [1, 2, 3, 4]
 *          
 *          
 *          
 *          [X] Share Grid
 *      Options
 *          Video
 *              [X] Fullscreen
 *              Resolution <- Generated List... ->
 *              Apply Changes()
 *              Back
 *          Sound
 *              Music Volume <- 0 - 100, step 5 ->
 *              Sound Volume <- 0 - 100, step 5 ->
 *          Controls
 *              Player 1
 *                  Player 1 <- Joystick, Keyboard ->
 *                  Remap buttons
 *              
 *      Exit
 * 
 * 
 *      Menu
 *          Update
 * 
 *      Menu
 *          Draw background (before draw)
 *          Draw Title
 *          Draw Each Option (is active selection)
 *          maybe draw active selected description?
 *          
 *          (after draw)
 *          
 *          
 *      Menu 
 *          Title
 *          List of items
 *      
 *      Menu Items
 *          Action - something happens when item is selected
 *                 - open submenu (push onto stack)
 *                 - BACK/close current menu (pop off stack)
 *                 - CANCEL/revert any changes, then close current menu (pop off stack)
 *                 - OK/do something, then close current menu (pop off stack)
 *                 - APPLY/do something, commit changes
 *                 - do something, those close all menus?
 *          Slider/numeric Updown 
 *          checkbox / yesno / toggle bool
 *          
 *          show all choices?  speed: slow [normal] fast
 *          or show arrows     speed:     slow  ->
 *          or show arrows     speed: <- normal ->
 *          or show arrows     speed: <-  fast 
 */

namespace Liztris
{

/*
 * <Menu>
 * 
 * 
 */




    public abstract class MenuItem
    {
        public abstract void Draw();
        public abstract void Update();
    }

    public abstract class Menu1 : MenuItemCollection
    {

    }

    public abstract class SelectableMenuItem
    {
        protected abstract void SelectMenuItem();

    }

    public abstract class MenuItemCollection
    {
        protected abstract IList<MenuItem> MenuItems { get; }
        int SelectedMenuItemIndex;
    }


    public class FooMenu
    {
        public FooMenu(MenuItem[] MenuDefinition)
        {


        }


        Stack<MenuItem[]> CurrentMenu;
    }

    public class TestMenu : Menu1
    {
        private TestSubMenu _Items = new TestSubMenu() { Title = "Main Menu",
            MenuItems = new MenuItem[]
            {
                new TestItem() { Title = "New Game"},
                new TestSubMenu() { Title = "Options",
                    MenuItems = new MenuItem[] 
                    {
                        new TestItem() { Title = "Video"},
                        new TestSubMenu() { Title = "Audio",
                            MenuItems = new MenuItem[] 
                            {
                                new NumberRangeItem(0, 100, 5) { Title = "Sound Volume" },
                                new NumberRangeItem(0, 100, 5) { Title = "Music Volume" },
                            }
                        },
                        new TestItem() { Title = "Controls"},
                        new BackItem(),
                    }
                },
                new TestSubMenu() { Title = "Exit Game", MenuTitle = "Are You Sure?",
                    MenuItems = new MenuItem[] 
                    {
                        new TestItem() { Title = "Yes"},
                        new BackItem() { Title = "No"},
                    }
                },
            }
        };

        //protected override IList<MenuItem> MenuItems => _Items;
    }


    public class TestItem : MenuItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class BackItem : TestItem
    {
        public BackItem() { Title = "Back"; }

        void OnSelect()
        {
            Menu.Pop();
        }
    }

    public class ValueItem : TestItem
    {
        public object value;

        void OnSelect()
        {
        }
    }

    public class NumberRangeItem : ValueItem
    {
        public NumberRangeItem(int Min, int Max, int Step)
        {

        }

        void OnLeft()
        {
            var i = (int)value;
            i -= Step;
            if (i < Min)
                i = Min;
            value = i;
        }

        void OnRight()
        {
            var i = (int)value;
            i += Step;
            if (i > Max)
                i = Max;
            value = i;
        }
    }

    public class BoolItem : ValueItem
    {
        void OnSelect()
        {
            value = !(bool)value;
        }
    }

    public class TestSubMenu : TestItem
    {
        public string MenuTitle { get; set; }
        public MenuItem[] MenuItems { get; set; }

        int DefaultSelectedIndex;
        bool RememberSelectedIndex;
        bool ImplicitBack; //esc, gamepad b triggers 'back' button

        void OnSelect()
        {
            Menu.Push(this);
        }

        Rectangle[] GetTitleLayout(Rectangle MenuRectangle)
        {

        }

        Rectangle GetDescriptionLayout(Rectangle MenuRectangle)
        {

        }


        Rectangle[] GetMenuItemLayout(Rectangle MenuRectangle)
        {

        }
    }
}
