﻿using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Liztris
{
    public static class PieceDefinition
    {
        public static int AvailablePieceCount => AvailablePieces.Length;

        static int[][,] AvailablePieces =>
            StandardPieces;
            //StupidPieces;
            //TestHalfBlock;
            //TestFullBlock;

        private static int[][,] TestFullBlock = new int[][,]
        {
            new int[,] {
                {1,5,9,13},
                {2,6,10,14},
                {3,7,11,15},
                {4,8,12,16},
            },
        };

        private static int[][,] TestHalfBlock = new int[][,]
        {
            new int[,] {
                {1,5,9,13},
                {2,6,10,14},
            },
        };

        private static int[][,] StandardPieces = new int[][,]
        {
            new int[,] {
                {14,14},
                {14,14},
            },
            new int[,] {
                {0,0,3},
                {3,3,3},
                {0,0,0},
            },
            new int[,] {
                {15,0,0},
                {15,15,15},
                {0,0,0},
            },
            new int[,] {
                {0,0,0,0},
                {20,20,20,20},
                {0,0,0,0},
                {0,0,0,0},
            },
            new int[,] {
                {0,0,0},
                {0,13,13},
                {13,13,0},
            },
            new int[,] {
                {0,0,0},
                {6,6,0},
                {0,6,6},
            },
            new int[,] {
                {0,8,0},
                {8,8,8},
                {0,0,0},
            },
        };

        private static int[][,] StupidPieces = new int[][,]
        {
            new int[,] {
                {0,14,14,14},
                {0,14,0,14},
                {0,14,14,14},
                {0,0,0,0},
            },
            new int[,] {
                {3,3,3,0},
                {3,0,0,0},
                {3,3,3,0},
            },
            new int[,] {
                {0,15,15,0},
                {0,0,15,0},
                {0,15,15,0},
            },
            new int[,] {
                {0,20,0,0},
                {0,20,20,20},
                {0,20,0,20},
                {0,20,0,0},
            },
            new int[,] {
                {0,13,0,0},
                {13,13,13,0},
                {13,0,13,0},
            },
            new int[,] {
                {0,0,6,0},
                {0,0,6,0},
                {0,0,0,0},
            },
            new int[,] {
                {0,0,8,0},
                {0,0,8,0},
                {0,0,8,0},
            },
            new int[,] {
                {0,0,7,0},
                {0,0,7,0},
                {0,0,0,0},
            },
            new int[,] {
                {0,0,0,0},
                {0,0,5,0},
                {0,0,0,0},
            },
            new int[,] {
                {0,0,0,0},
                {0,0,4,0},
                {0,0,0,0},
            },
            new int[,] {
                {0,8,0,0},
                {8,8,8,0},
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

        public void RotateCounterClockwise()
        {
            var h = BlockMap.GetHeight();
            var w = BlockMap.GetWidth();

            int[,] new_piece = new int[w, h];

            for (int x = w - 1; x >= 0; x--)
                for (int y = 0; y < h; y++)
                    new_piece[w - 1 - x, y] = BlockMap[y, x];

            BlockMap = new_piece;
        }

        public void RotateClockwise()
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
