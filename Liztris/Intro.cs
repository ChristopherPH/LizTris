using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Liztris
{
    public static class Intro
    {
        enum IntroCommands
        {
            PressAKey,
        }

        static InputManager<IntroCommands> inputManager = new InputManager<IntroCommands>();

        static Intro()
        {
            inputManager.AddAction(IntroCommands.PressAKey, Keys.Escape);
            inputManager.AddAction(IntroCommands.PressAKey, InputManager<IntroCommands>.GamePadButtons.Start);
            inputManager.AddAction(IntroCommands.PressAKey, InputManager<IntroCommands>.GamePadButtons.Back);
        }

        public static bool IsActive
        {
            get { return _IsActive; }
            set
            {
                if (value && !_IsActive)
                {
                    introTimer.Reset();
                    moveTimer.Reset();
                    Setup();
                }

                _IsActive = value;
            }
        }
        static bool _IsActive = false;

        public static void Update(GameTime gameTime)
        {
            if (introTimer.UpdateAndCheck(gameTime))
            {
                IsActive = false;
                return;
            }

            inputManager.Update(PlayerIndex.One);
            if (inputManager.IsActionTriggered(IntroCommands.PressAKey))
            {
                IsActive = false;
                return;
            }

            moveTimer.UpdateAndCheck(gameTime, Update);
        }

        static void Setup()
        {
            var steps = introTimer.Delay / moveTimer.Delay;

            inc_x = (float)((endx - startx) / steps);
            inc_y = (float)((endy - starty) / steps);
            inc_rot = (float)((endrotation - startrotation) / steps);

            inc_a = (float)((end_a - start_a) / steps);
            inc_r = (float)((end_r - start_r) / steps);
            inc_g = (float)((end_g - start_g) / steps);
            inc_b = (float)((end_b - start_b) / steps);

            curx = startx;
            cury = starty;
            currotation = startrotation;

            cur_a = start_a;
            cur_r = start_r;
            cur_g = start_g;
            cur_b = start_b;
        }

        static void Update()
        {
            curx += inc_x;
            cury += inc_y;
            currotation += inc_rot;

            cur_a += inc_a;
            cur_r += inc_r;
            cur_g += inc_g;
            cur_b += inc_b;
        }

        static float startx = 900;
        static float starty = -150;
        static float startrotation = 2.5f;

        static float endx = 28;
        static float endy = 248;
        static float endrotation = -0.5f;

        //Color.White
        static float start_a = 0;
        static float start_r = 255;
        static float start_g = 255;
        static float start_b = 255;

        //Color.Wheat
        static float end_a = 255;
        static float end_r = 245;
        static float end_g = 222;
        static float end_b = 179;

        static float curx;
        static float cury;
        static float currotation;

        static float inc_x;
        static float inc_y;
        static float inc_rot;



        static float cur_a;
        static float cur_r;
        static float cur_g;
        static float cur_b;

        static float inc_a;
        static float inc_r;
        static float inc_g;
        static float inc_b;

        static Timer introTimer = new Timer(1500);
        static Timer moveTimer = new Timer(10);


        public static void Draw(ExtendedSpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            var back = Color.FromNonPremultiplied(0, 0, 0, (int)cur_a);
            var fore = Color.FromNonPremultiplied((int)cur_r, (int)cur_g, (int)cur_b, (int)cur_a);

            spriteBatch.DrawString(spriteFont, "LIZTRIS", new Vector2(curx + 2, cury + 2), back,
                currotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(spriteFont, "LIZTRIS", new Vector2(curx, cury), fore,
                currotation, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
