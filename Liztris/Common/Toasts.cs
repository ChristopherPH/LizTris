using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    static class Toasts
    {
        public static void AddToast(int x, int y, int ms, string Text, Color c, float Scale = 1.0f)
        {
            _toasts.Add(new Toast()
            {
                x = x,
                y = y,
                ttl = ms,
                text = Text,
                c = c,
                scale = Scale,

                dx = 0,
                dy = -1,
                dalpha = 0.05f,
            });
        }

        public static void AddToast(Rectangle r, int ms, string Text, Color c, float Scale = 1.0f)
        {
            Vector2 size = spriteFont.MeasureString(Text);

            int x = (r.Width / 2) - ((int)size.X / 2) + r.X;
            int y = (r.Height / 2) - ((int)size.Y / 2) + r.Y;

            _toasts.Add(new Toast()
            {
                x = x,
                y = y,
                ttl = ms,
                text = Text,
                c = c,
                scale = Scale,

                dx = 0,
                dy = -1,
                dalpha = 0.05f,
            });
        }

        private class Toast
        {
            public string text;

            public int x;
            public int y;
            public float alpha;
            public Color c;
            public float scale;

            public int dx;
            public int dy;
           
            public float dalpha;
            public double ttl;

            
        }


        static List<Toast> _toasts = new List<Toast>();

        public static SpriteFont spriteFont { get; set; }

        public static void Update(GameTime gameTime)
        {
            for (int i = _toasts.Count - 1; i >= 0; i--)
            {
                var t = _toasts[i];

                t.ttl -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (t.ttl <= 0)
                {
                    _toasts.RemoveAt(i);
                    continue;
                }

                t.x += t.dx;
                t.y += t.dy;
                t.alpha += t.dalpha;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var t in _toasts)
            {
                spriteBatch.DrawString(spriteFont, t.text, new Vector2(t.x, t.y), t.c
                    , 0, Vector2.Zero, t.scale, SpriteEffects.None, 0);
            }
        }
    }
}
