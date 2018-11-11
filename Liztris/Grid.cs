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

        int[] LevelSpeeds;
        public List<Player> Players { get; } = new List<Player>();
        Timer GridDropTime = new Timer();
        Queue<Piece> NextPieces = new Queue<Piece>();

        public Grid(int WidthInBlocks, int HeightInBlocks, int[] LevelSpeeds)
        {
            this.WidthInBlocks = WidthInBlocks;
            this.HeightInBlocks = HeightInBlocks;
            this.LevelSpeeds = LevelSpeeds;

            BlockMap = new int[HeightInBlocks, WidthInBlocks];
        }

        public void NewGame()
        {
            Level = 1;
            LineCount = 0;
            GridDropTime.SetDelay(LevelSpeeds[LineCount]);
            GridDropTime.Reset();

            ClearGrid();

            NextPieces.Clear();

            foreach (var player in Players)
            {
                player.NewGame();
                NextPieces.Enqueue(PieceDefinition.GetRandomPiece());
                SetupPlayerPiece(player);
            }
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
            {
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
            }

            return true;
        }

        public void AddPieceToGrid(Piece p, int piece_x, int piece_y)
        {
            if (p == null)
                return;

            for (int x = 0; x < p.Width; x++)
            {
                for (int y = 0; y < p.Height; y++)
                {
                    //piece has no block, ignore
                    if (p.BlockMap[y, x] <= 0)
                        continue;

                    //copy piece to grid
                    BlockMap[piece_y + y, piece_x + x] = p.BlockMap[y, x];
                }
            }
        }

        bool SetupPlayerPiece(Player player)
        {
            var nextPiece = NextPieces.Dequeue();

            //split the grid into even sections for players
            var blocksPerPlayer = (double)WidthInBlocks / (double)(Players.Count);

            //find middle of that section for current player
            var tmpX = (Players.IndexOf(player) * blocksPerPlayer) + (blocksPerPlayer / 2);

            var piece_x = (int)tmpX - (nextPiece.Width / 2);
            var piece_y = -nextPiece.Height;

            int y = 0;
            for (y = 0; y <= nextPiece.Height; y++)
            {
                if (CheckPiece(nextPiece, piece_x, piece_y))
                    break;

                piece_y++;
            }
            if (y == nextPiece.Height + 1)
            {
                return false;
            }


            player.CurrentPiece = nextPiece;
            player.piece_x = piece_x;
            player.piece_y = piece_y;

            NextPieces.Enqueue(PieceDefinition.GetRandomPiece());
            return true;
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


        enum SoundState
        {
            None = 0,
            AddToGrid = 1,
            ClearedLines = 2,
            NextLevel = 3,
            Lose = 4,
        }

        public void Update(GameTime gameTime)
        {
            SoundState ss = SoundState.None;

            //allow players to move pieces
            foreach (var player in Players)
            {
                if (player.CurrentPiece == null)
                {
                    if (!SetupPlayerPiece(player))
                    {
                        ss = SoundState.Lose;
                        NewGame();
                        return;
                    }
                }

                player.Update(gameTime);
            }


            //time to drop pieces
            if (GridDropTime.UpdateAndCheck(gameTime))
            {
                foreach (var player in Players)
                {
                    if (player.CurrentPiece == null)
                        continue;

                    if (player.Grid.CheckPiece(player.CurrentPiece,
                        player.piece_x, player.piece_y + 1))
                    {
                        player.piece_y++;
                        ss = SoundState.None;
                    }
                    else
                    {
                        ss = SoundState.AddToGrid;

                        player.Grid.AddPieceToGrid(player.CurrentPiece,
                            player.piece_x, player.piece_y);
                        player.CurrentPiece = null;
                        var lines = ClearFilledLines();
                        if (lines > 0)
                        {
                            LineCount += lines;
                            player.Score += (lines * 50);

                            ss = SoundState.ClearedLines;

                            if ((LineCount > 0 ) && (LineCount % 5 == 0))
                            {
                                Level++;
                                if (Level < LevelSpeeds.Length)
                                    GridDropTime.SetDelay(LevelSpeeds[Level]);

                                ss = SoundState.NextLevel;
                            }
                        }
                    }

                    switch (ss)
                    {
                        case SoundState.AddToGrid:
                            //soundDrop.Play();
                            break;

                        case SoundState.ClearedLines:
                            //soundLine.Play();
                            break;

                        case SoundState.NextLevel:
                            //soundLevel.Play();
                            break;
                    }
                }
            }
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

                    if (BlockIndex > Blocks.Rows - 1)
                        BlockIndex = Blocks.Rows - 1;

                    Blocks.Draw(spriteBatch, 0, BlockIndex,
                        new Vector2(
                            ScreenRect.X + (BlockPixelSize * x),
                            ScreenRect.Y + (BlockPixelSize * y)));
                }
            }

            var YOffset = 0;

            foreach (var piece in NextPieces)
            {
                piece.Draw(spriteBatch, Blocks, BlockPixelSize,
                    ScreenRect.Right + BlockPixelSize,
                    ScreenRect.Y + (YOffset * BlockPixelSize)
                    );

                YOffset += piece.Height + 1;
            }

            foreach (var player in Players)
            {
                player.Draw(spriteBatch, Blocks, BlockPixelSize);
            }
        }
    }
}
