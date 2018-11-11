using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liztris
{
    public class Grid
    {
        public int Level { get; private set; } = 1;
        public int LineCount { get; private set; } = 0;
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }
        public int BlockSize { get; private set; }

        int[,] _grid;

        public Grid(int SizeX, int SizeY, int BlockSize)
        {
            this.SizeX = SizeX;
            this.SizeY = SizeY;
            this.BlockSize = BlockSize;

            _grid = new int[SizeX, SizeY];
        }

        public void NewGame()
        {
            Level = 1;
            LineCount = 0;

            ClearGrid();
        }


        public void NextLevel()
        {

        }


        private void ClearGrid()
        {
            for (int x = 0; x < SizeX; x++)
                for (int y = 0; y < SizeY; y++)
                    _grid[x, y] = 0;
        }

        public bool CheckPiece(Piece p, int px, int py)
        {
            if (p == null)
                return false;

            for (int x = 0; x < p.Width; x++)
                for (int y = 0; y < p.Height; y++)
                {
                    //piece has no block, ignore
                    if (p.BlockMap[x, y] == 0)
                        continue;

                    //where is this block?
                    int tmp_x = px + x;
                    int tmp_y = py + y;

                    //block out of bounds
                    if ((tmp_x < 0) || (tmp_x >= SizeX))
                        return false;

                    if ((tmp_y < 0) || (tmp_y >= SizeY))
                        return false;

                    //block over grid block
                    if (_grid[tmp_x, tmp_y] > 0)
                        return false;
                }

            return true;
        }

        public void AddPieceToGrid(Piece p, int piece_x, int piece_y)
        {/*
            for (int x = 0; x < p.Width; x++)
                for (int y = 0; y < p.Height; y++)
                {
                    //piece has no block, ignore
                    if (p.CurrentPiece[x, y] == 0)
                        continue;

                    //copy piece to grid
                    grid[piece_x + x, piece_y + y] = p.CurrentPiece[x, y];
                }*/
        }

        public int ClearFilledLines()
        {
            int clearedLines = 0;

            for (int y = SizeY - 1; y >= 0; y--)
            {
                int x;
                for (x = 0; x < SizeX; x++)
                {
                    if (_grid[x, y] == 0)
                        break;
                }

                if (x == SizeX)
                {
                    for (int y2 = y; y2 > 0; y2--)
                        for (x = 0; x < SizeX; x++)
                            _grid[x, y2] = _grid[x, y2 - 1];

                    for (x = 0; x < SizeX; x++)
                        _grid[x, 0] = 0;

                    y++;
                    clearedLines++;
                }
            }

            return clearedLines;
        }

        SpriteSheet Bricks;

        public void LoadContent(ContentManager Content)
        {
            Texture2D tex = Content.Load<Texture2D>("Bricks");
            Bricks = new SpriteSheet(tex, 4, 6);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, int offsetx, int offsety)
        {
            if (Bricks == null)
                return;

            for (int x = 0; x < SizeX; x++)
                for (int y = 0; y < SizeY; y++)
                    if (_grid[x, y] > 0)
                        Bricks.Draw(spriteBatch, 0, _grid[x, y] - 1, 
                            new Vector2(offsetx + BlockSize * x, offsety + BlockSize * y));
        }
    }
}
