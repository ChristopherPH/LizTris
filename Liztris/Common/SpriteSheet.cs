using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Common
{
    public class SpriteSheet
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        public SpriteSheet(Texture2D texture, int rows, int columns)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
        }

        public void Draw(SpriteBatch spriteBatch, int Tile, Vector2 location)
        {
            int TileX = Tile / Columns;
            int TileY = Tile % Columns;

            Draw(spriteBatch, TileX, TileY, location);
        }

        public void Draw(SpriteBatch spriteBatch, int TileX, int TileY, Vector2 location)
        {
            int width = Texture.Width / Rows;
            int height = Texture.Height / Columns;

            Rectangle sourceRectangle = new Rectangle(width * TileX, height * TileY, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            //spriteBatch.Begin();
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            //spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, int TileX, int TileY, Vector2 location, Color color)
        {
            int width = Texture.Width / Rows;
            int height = Texture.Height / Columns;

            Rectangle sourceRectangle = new Rectangle(width * TileX, height * TileY, width, height);
            Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, color);
        }

        public void Draw(SpriteBatch spriteBatch, int Tile, Vector2 location, Color color)
        {
            int TileX = Tile / Columns;
            int TileY = Tile % Columns;

            Draw(spriteBatch, TileX, TileY, location, color);
        }
    }
}
