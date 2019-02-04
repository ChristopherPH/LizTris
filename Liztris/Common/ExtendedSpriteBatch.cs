using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common
{
    /// <summary>
    /// SpriteBatch that allows Lines and Rectangles to be drawn directly
    /// </summary>
    public class ExtendedSpriteBatch : SpriteBatch
    {
        /// <summary>
        /// The texture used when drawing rectangles, lines and other 
        /// primitives. This is a 1x1 white texture created at runtime.
        /// </summary>
        protected Texture2D WhiteTexture { get; private set; }

        public ExtendedSpriteBatch(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            this.WhiteTexture = new Texture2D(this.GraphicsDevice, 1, 1);
            this.WhiteTexture.SetData(new Color[] { Color.White });
        }


        /// <summary>
        /// Draw texture to location with no tint
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="destinationRectangle"></param>
        public void Draw(Texture2D texture, Rectangle destinationRectangle)
        {
            this.Draw(texture, destinationRectangle, Color.White);
        }

        /// <summary>
        /// Draw a line between the two supplied points.
        /// </summary>
        /// <param name="start">Starting point.</param>
        /// <param name="end">End point.</param>
        /// <param name="color">The draw color.</param>
        public void DrawLine(Vector2 start, Vector2 end, Color color)
        {
            float length = (end - start).Length();
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            this.Draw(this.WhiteTexture, start, null, color, rotation, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="color">The draw color.</param>
        /// <param name="Border">thickness of lines</param>
        /// <param name="Inset">Rectangle is inside bounding box</param>
        public void DrawRectangle(Rectangle rectangle, Color color, int Border = 1, bool Inset = false)
        {
            if (Inset)
            {
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, Border), color); //top
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Left, rectangle.Bottom - Border, rectangle.Width, Border), color); //bottom
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Left, rectangle.Top, Border, rectangle.Height), color); //left
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Right - Border, rectangle.Top, Border, rectangle.Height - Border), color); //right
            }
            else
            {
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Left - Border, rectangle.Top - Border, rectangle.Width + (Border * 2), Border), color); //top
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Left - Border, rectangle.Bottom, rectangle.Width + (Border * 2), Border), color); //bottom
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Left - Border, rectangle.Top, Border, rectangle.Height), color); //left
                this.Draw(this.WhiteTexture, new Rectangle(rectangle.Right, rectangle.Top, Border, rectangle.Height), color); //right
            }
        }

        /// <summary>
        /// Fill a rectangle.
        /// </summary>
        /// <param name="rectangle">The rectangle to fill.</param>
        /// <param name="color">The fill color.</param>
        public void FillRectangle(Rectangle rectangle, Color color)
        {
            this.Draw(this.WhiteTexture, rectangle, color);
        }

        /// <summary>
        /// Fill a rectangle with alpha
        /// </summary>
        /// <param name="rectangle">The rectangle to fill.</param>
        /// <param name="color">The fill color.</param>
        /// <param name="Alpha">0.0f to 1.0f</param>
        public void FillRectangle(Rectangle rectangle, Color color, float Alpha)
        {
            Alpha = Math.Min(1.0f, Alpha);
            Alpha = Math.Max(0.0f, Alpha);
            this.Draw(this.WhiteTexture, rectangle, color * Alpha);
        }

        [Flags]
        public enum Alignment { Center = 0, Left = 1, Right = 2, Top = 4, Bottom = 8 }

        public void DrawString(SpriteFont font, string text, Rectangle bounds, Alignment align, Color color, float scale = 1.0f)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = bounds.GetCenter();
            Vector2 origin = size * 0.5f; //set origin to center of text

            if (align.HasFlag(Alignment.Left))
                origin.X += bounds.Width / 2 - (size.X * scale) / 2;

            if (align.HasFlag(Alignment.Right))
                origin.X -= bounds.Width / 2 - (size.X * scale) / 2;

            if (align.HasFlag(Alignment.Top))
                origin.Y += bounds.Height / 2 - (size.Y * scale) / 2;

            if (align.HasFlag(Alignment.Bottom))
                origin.Y -= bounds.Height / 2 - (size.Y * scale) / 2;

            DrawString(font, text, pos, color, 0, origin, scale, SpriteEffects.None, 0);
        }
    }
}
