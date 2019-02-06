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
        public void DrawLine(Vector2 start, Vector2 end, Color color, float Thickness = 1.0f)
        {
            float length = (end - start).Length();
            float rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            this.Draw(this.WhiteTexture, start, null,color, rotation,
                new Vector2(0, (float)this.WhiteTexture.Height / 2),
                new Vector2(length, Thickness), 
                SpriteEffects.None, 0);
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
            var size = font.MeasureString(text);
            var pos = new Vector2(bounds.Left + bounds.Width / 2,
                             bounds.Top + bounds.Height / 2);
            var origin = size * 0.5f;

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

        public void DrawPoly(Vector2[] Verticies, Color color, float Thickness = 1.0f)
        {
            if ((Verticies == null) || (Verticies.Length <= 1))
                return;

            int i = 0;
            for (i = 1; i < Verticies.Length; i++)
                DrawLine(Verticies[i - 1], Verticies[i], color, Thickness);

            DrawLine(Verticies[i - 1], Verticies[0], color, Thickness);
        }

        public void DrawEquilateralTriangle(Vector2 Location, float Radius, Color color, float Angle, float Thickness = 1.0f)
        {
            Angle = (float)(Math.PI / 180) * Angle;

            //https://qph.fs.quoracdn.net/main-qimg-92aee96dac9fb6ba3730d9f01a085022.webp
            var root3xRadiusDiv2 = (float)Math.Sqrt(3) * Radius / 2;
            var radiusx15 = (float)(Radius * 1.5);

            //start point is top center
            var top = new Vector2(Location.X + Radius, Location.Y);

            //bottom point y is 1.5 * radius below top center
            //distance between left and right bottom points are root 3 * radius
            var bl = new Vector2(top.X - root3xRadiusDiv2, Location.Y + radiusx15);
            var br = new Vector2(top.X + root3xRadiusDiv2, Location.Y + radiusx15);

            //find center and rotate points before drawing
            if (Angle != 0)
            {
                var center = new Vector2(Location.X + Radius, Location.Y + Radius);
                bl = RotateAboutOrigin(bl, center, Angle);
                br = RotateAboutOrigin(br, center, Angle);
                top = RotateAboutOrigin(top, center, Angle);
            }

            this.DrawLine(bl, br, color, Thickness);
            this.DrawLine(br, top, color, Thickness);
            this.DrawLine(top, bl, color, Thickness);
        }


        private Vector2 RotateAboutOrigin(Vector2 point, Vector2 origin, float rotation)
        {
            return Vector2.Transform(point - origin, Matrix.CreateRotationZ(rotation)) + origin;
        }

    }
}
