using Common;
using Common.MenuSystem;
using Common.Misc;
using Common.Interpolator;
using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using Test.ControllerManager;
using System.Collections;
using System.ComponentModel;
using EventMenu;

namespace Test
{
    public class TestState : GameState
    {
        ControlManager<EventMenuItemActions> EventMenuControl = 
            new ControlManager<EventMenuItemActions>();

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }

        SpriteFont _font;

        public override bool RenderPreviousState => false;


        VerticalEventMenu menu = new VerticalEventMenu();

        public TestState()
        {
            EventMenuControl.MapControl(Controls.Start, EventMenuItemActions.Select);
            EventMenuControl.MapControl(Controls.Back, EventMenuItemActions.Back);
            EventMenuControl.MapControl(Controls.Up, EventMenuItemActions.MenuUp);
            EventMenuControl.MapControl(Controls.Down, EventMenuItemActions.MenuDown);
            EventMenuControl.MapControl(Controls.Left, EventMenuItemActions.MenuLeft);
            EventMenuControl.MapControl(Controls.Right, EventMenuItemActions.MenuRight);




            menu.Add(new EventMenuItem()
            {
                ID = 0,
                Text = "Item TL",
                // Down = 1,
                //  Right = 2,
                Location = new Point(0, 0),
                Size = new Point(400, 100),
            });


            menu.MenuItems.Add(new EventMenuItem()
            {
                ID = 1,
                Text = "Item BL",
                //     Up = 0,
                //   Right = 3,
                Location = new Point(0, 300),
                Size = new Point(400, 100),
            });

            menu.MenuItems.Add(new EventMenuItem()
            {
                ID = 2,
                Text = "Item TR",
                //    Down = 3,
                //Left = 0,
                Location = new Point(500, 0),
                Size = new Point(400, 100),
            });

            menu.MenuItems.Add(new EventMenuItem()
            {
                ID = 3,
                Text = "Item BR",
                // Up = 2,
                //  Left = 1,
                Location = new Point(500, 300),
                Size = new Point(400, 100),
            });

            menu.MenuItems[0].Select +=
                (sender, e) => { System.Diagnostics.Debug.Print("foo"); };

            menu.Draw +=
                (s, e) => { };
        }

        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, 
            Rectangle GameRectangle)
        {
            spriteBatch.DrawString(_font, "Test State", GameRectangle,
                ExtendedSpriteBatch.Alignment.Center, Color.Black, 3.0f);

            foreach (var m in menu.MenuItems)
            {
                var r = new Rectangle(m.Location, m.Size);

                if (menu.IsSelected(m))
                {
                    spriteBatch.FillRectangle(r, Color.DarkBlue);
                    spriteBatch.DrawRectangle(r, Color.Lerp(Color.Aqua, Color.Black, 0.5f), 10);
                    spriteBatch.DrawRectangle(r, Color.Aqua, 4);
                }
                spriteBatch.DrawString(_font, m.Text, r,
                    ExtendedSpriteBatch.Alignment.Center, Color.White);
            }

            pm.Draw(gameTime, spriteBatch);
        }

        ParticleManager pm = new ParticleManager();
        Random rand = new Random();

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            EventMenuControl.Update();

            foreach (EventMenuItemActions action in Enum.GetValues(typeof(EventMenuItemActions)))
            {
                if (EventMenuControl.IsActionTriggered(action))
                    menu.DoAction(action);
            }

            pm.Update(gameTime);

            double dx, dy, rad;
            double speed = 1;

            for (int i = 0; i < 4; i++) // 2 particles per tick
            {
                rad = (double)(rand.Next(0, 359)) * (3.1415926 / 180.0);
                dx = Math.Cos(rad) * speed;
                dy = Math.Sin(rad) * speed;

                pm.AddParticle(
                    this.Engine.GameRectangle.Center.ToVector2(),
                    new Vector2((float)dx, (float)dy), 
                    Color.Yellow, 3,
                    400, true);
            }

            /* confetti
            double speed = (float)rand.Next(10, 50) / 10;

            for (int i = 0; i < rand.Next(1, 3); i++) // 2 particles per tick
            {
                rad = (double)(rand.Next(0, 359)) * (3.1415926 / 180.0);
                dx = Math.Cos(rad) * speed;
                dy = Math.Sin(rad) * speed;
             
                pm.AddParticle(
                    this.Engine.GameRectangle.Center.ToVector2(),
                    new Vector2((float)dx, (float)dy), 
                    //Color.Yellow, 
                    Color.FromNonPremultiplied(
                        rand.Next(0, 255),
                        rand.Next(0, 255),
                        rand.Next(0, 255),
                        255
                        ),
                    rand.Next(1, 14),
                    rand.Next(300, 500), true);
            } */
        }


        public abstract class MenuRegion
        {
            public Rectangle rectangle { get; set; }
            
            public abstract void Draw();
        }

        public abstract class MenuText : MenuRegion
        {
            public string Text { get; set; }

            
        }

        public interface IMenuText
        {
            string Text { get; set; }
        }

        //public interface IMenuSelectable
       // {
            
       // }

        public interface IMenuSelectable
        {
            //IMenuSelectable Next { get; set; }
            //IMenuSelectable Previous { get; set; }

            IMenuSelectable Left { get; set; } 
            IMenuSelectable Right { get; set; }
            IMenuSelectable Up { get; set; }
            IMenuSelectable Down { get; set; }
        }
    }
}
