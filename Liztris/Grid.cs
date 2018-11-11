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

        public int WidthInBlocks { get; private set; }
        public int HeightInBlocks { get; private set; }

        public int[,] BlockMap { get; private set; }

        public Rectangle ScreenRect { get; set; } = Rectangle.Empty;

        public Grid(int WidthInBlocks, int HeightInBlocks)
        {
            this.WidthInBlocks = WidthInBlocks;
            this.HeightInBlocks = HeightInBlocks;

            BlockMap = new int[HeightInBlocks, WidthInBlocks];
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
            for (int x = 0; x < WidthInBlocks; x++)
                for (int y = 0; y < HeightInBlocks; y++)
                    BlockMap[y, x] = 0;
        }

        public bool CheckPiece(Piece p, int px, int py)
        {
            if (p == null)
                return false;

            for (int x = 0; x < p.Width; x++)
                for (int y = 0; y < p.Height; y++)
                {
                    //piece has no block, ignore
                    if (p.BlockMap[y, x] == 0)
                        continue;

                    //where is this block?
                    int tmp_x = px + x;
                    int tmp_y = py + y;

                    //block out of bounds
                    if ((tmp_x < 0) || (tmp_x >= WidthInBlocks))
                        return false;

                    if ((tmp_y < 0) || (tmp_y >= HeightInBlocks))
                        return false;

                    //block over grid block
                    if (BlockMap[tmp_y, tmp_x] > 0)
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

            for (int y = HeightInBlocks - 1; y >= 0; y--)
            {
                int x;
                for (x = 0; x < WidthInBlocks; x++)
                {
                    if (BlockMap[y, x] == 0)
                        break;
                }

                if (x == WidthInBlocks)
                {
                    for (int y2 = y; y2 > 0; y2--)
                        for (x = 0; x < WidthInBlocks; x++)
                            BlockMap[y2, x] = BlockMap[y2 - 1, x];

                    for (x = 0; x < WidthInBlocks; x++)
                        BlockMap[0, x] = 0;

                    y++;
                    clearedLines++;
                }
            }

            return clearedLines;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteSheet Blocks, int BlockPixelSize)
        {
            for (int x = 0; x < WidthInBlocks; x++)
            {
                for (int y = 0; y < HeightInBlocks; y++)
                {
                    var BlockIndex = BlockMap[y, x] - 1;
                    if (BlockIndex <= -1)
                        continue;

                    Blocks.Draw(spriteBatch, 0, BlockIndex,
                        new Vector2(
                            ScreenRect.X + (BlockPixelSize * x),
                            ScreenRect.Y + (BlockPixelSize * y)));
                }
            }
        }
    }
}
