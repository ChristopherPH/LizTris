using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liztris
{
    public static class PieceDefinition
    {
        public static int AvailablePieceCount => AvailablePieces.Length;

        private static int[][,] AvailablePieces = new int[][,]
        {
            new int[,] {
                {0,0,0,0},
                {0,14,14,0},
                {0,14,14,0},
                {0,0,0,0},
            },
            new int[,] {
                {0,3,0,0},
                {0,3,0,0},
                {0,3,3,0},
            },
            new int[,] {
                {0,0,15,0},
                {0,0,15,0},
                {0,15,15,0},
            },
            new int[,] {
                {0,20,0,0},
                {0,20,0,0},
                {0,20,0,0},
                {0,20,0,0},
            },
            new int[,] {
                {0,13,0,0},
                {0,13,13,0},
                {0,0,13,0},
            },
            new int[,] {
                {0,0,6,0},
                {0,6,6,0},
                {0,6,0,0},
            },
            new int[,] {
                {0,8,0,0},
                {0,8,8,0},
                {0,8,0,0},
            },
        };

        private static Random randomizer = new Random();

        public static Piece GetRandomPiece()
        {
            int index = randomizer.Next(0, AvailablePieceCount);

            return GetPiece(index);
        }

        public static Piece GetPiece(int index)
        {
            if (index < 0)
                index = 0;
            else if (index >= PieceDefinition.AvailablePieceCount - 1)
                index = PieceDefinition.AvailablePieceCount - 1;

            return new Piece(AvailablePieces[index]);
        }
    }

    public class Piece
    {
        public Piece(int[,] BlockMap)
        {
            if (BlockMap == null)
                throw new Exception();

            this.BlockMap = BlockMap;
        }

        public int[,] BlockMap { get; private set; }
        public int Height => BlockMap.GetHeight();
        public int Width => BlockMap.GetWidth();

        public void RotateClockwise()
        {
            var h = BlockMap.GetHeight();
            var w = BlockMap.GetWidth();

            int[,] new_piece = new int[w, h];

            for (int x = w - 1; x >= 0; x--)
                for (int y = 0; y < h; y++)
                    new_piece[w - 1 - x, y] = BlockMap[y, x];

            BlockMap = new_piece;
        }

        public void RotateCounterClockwise()
        {
            var h = BlockMap.GetHeight();
            var w = BlockMap.GetWidth();

            int[,] new_piece = new int[w, h];

            for (int x = w - 1; x >= 0; x--)
                for (int y = 0; y < h; y++)
                    new_piece[x, h - 1 - y] = BlockMap[y, x];

            BlockMap = new_piece;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteSheet Blocks, int BlockPixelSize, 
            int XOffset, int YOffset)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var BlockIndex = BlockMap[y, x] - 1;
                    if (BlockIndex <= -1)
                        continue;

                    //if (BlockIndex > Blocks.Rows - 1)
                    //    BlockIndex = Blocks.Rows - 1;

                    Blocks.Draw(spriteBatch, BlockIndex,
                        new Vector2(
                            XOffset + (BlockPixelSize * x),
                            YOffset + (BlockPixelSize * y)));
                }
            }
        }
    }
}
