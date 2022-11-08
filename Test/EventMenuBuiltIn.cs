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
    public class VerticalEventMenu : EventMenuItemSelectedCollection
    {
        public VerticalEventMenu()
        {
            Up += (s, e) => { SetSelection((SelectedIndex + (Count - 1)) % Count); e.Handled = true; };
            Down += (s, e) => { SetSelection((SelectedIndex + 1) % Count); e.Handled = true; };

            Draw += VerticalEventMenu_Draw;
        }

        private void VerticalEventMenu_Draw(object sender, EventHandledEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void DoDraw(ExtendedSpriteBatch spriteBatch)
        {
            foreach (var m in MenuItems)
            {
                var r = new Rectangle(m.Location, m.Size);

                if (IsSelected(m))
                {
                    spriteBatch.FillRectangle(r, Color.DarkBlue);
                    spriteBatch.DrawRectangle(r, Color.Lerp(Color.Aqua, Color.Black, 0.5f), 10);
                    spriteBatch.DrawRectangle(r, Color.Aqua, 4);
                }

                m.DoDraw(spriteBatch);
            }
        }

        public override bool DoAction(EventMenuItemActions Action)
        {
            if (Selection.DoAction(Action))
                return true;

            return base.DoAction(Action);
        }
    }

    public class HorizontalChoice : EventMenuItemSelectedCollection
    {
        public HorizontalChoice()
        {
            Left += (s, e) => { SetSelection((SelectedIndex + (Count - 1)) % Count); e.Handled = true; };
            Right += (s, e) => { SetSelection((SelectedIndex + 1) % Count); e.Handled = true; };
        }

        public override bool DoAction(EventMenuItemActions Action)
        {
            if (Selection.DoAction(Action))
                return true;

            return base.DoAction(Action);
        }
    }
}

    //menu controls
    //static text, image (could be animated)
    //slider (volume) (or incrementing/decrementing values)
    //chooser (resolutions, profile names) (list, left/right arrows)
    //radio buttons / group chooser
    //check box / boolean
    //action (close menu, open submenu, start game, 
    //  technically with multiple menu states, this is 
    //  just a push/pop state

//choices need to be dynamic or static lists
// resolutions, num players, etc

//actions when item changes - set volume
//actions when button pressed - apply res changes
//  technically apply should be a new state that reverts
//  if there is a problem
