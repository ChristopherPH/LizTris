using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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
        public int BestLineCount { get; private set; } = 0;
        public int Score { get; private set; } = 0;
        public int BestScore { get; private set; } = 0;

        public int WidthInBlocks { get; private set; }
        public int HeightInBlocks { get; private set; }

        public int[,] BlockMap { get; private set; }

        public Rectangle ScreenRect { get; set; } = Rectangle.Empty;

        int[] LevelSpeeds;
        int[] LevelLines;
        int[] ScoreMultiplier;
        public List<Player> Players { get; } = new List<Player>();
        Timer GridDropTime = new Timer();
        Queue<Piece> NextPieces = new Queue<Piece>();

        public Grid(int WidthInBlocks, int HeightInBlocks, int[] LevelSpeeds, int[] LevelLines, int [] ScoreMultiplier)
        {
            this.WidthInBlocks = WidthInBlocks;
            this.HeightInBlocks = HeightInBlocks;
            this.LevelSpeeds = LevelSpeeds;
            this.LevelLines = LevelLines;
            this.ScoreMultiplier = ScoreMultiplier;

            BlockMap = new int[HeightInBlocks, WidthInBlocks];
        }

        public void NewGame()
        {
            Level = 1;
            LineCount = 0;
            Score = 0;
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
            if (!CheckPieceInGrid(p, px, py))
                return false;

            //get rectagle location on the grid
            var r = new Rectangle(px, py, p.Width, p.Height);

            foreach (var player in Players)
            {
                if (player.CurrentPiece == null)
                    continue;
                if (player.CurrentPiece == p)
                    continue;

                var r2 = new Rectangle(player.piece_x, player.piece_y, 
                    player.CurrentPiece.Width, player.CurrentPiece.Height);

                if (!r.Intersects(r2))
                    continue;

                //rectangles overlap, check where
                var inter = Rectangle.Intersect(r, r2);

                for (int x = inter.Left; x < inter.Right; x++)
                { 
                    for (int y = inter.Top; y < inter.Bottom; y++)
                    {
                        var x1 = x - px;
                        var y1 = y - py;

                        var x2 = x - player.piece_x;
                        var y2 = y - player.piece_y;

                        if ((p.BlockMap[y1, x1] > 0) &&
                            (player.CurrentPiece.BlockMap[y2, x2] > 0))
                            return false;
                    }
                }
            }

            return true;
        }

        private bool CheckPieceInGrid(Piece p, int px, int py)
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
            if (NextPieces.Count == 0)
                return false;

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
            Tetris = 4,
            Lose = 5,
        }

        public SoundEffect soundLine { get; set; }
        public SoundEffect soundLevel { get; set; }
        public SoundEffect soundLose { get; set; }
        public SoundEffect soundDrop { get; set; }
        public SoundEffect soundTetris { get; set; }

        Timer GameOverTimer = null;
        bool CanResetGameOver = false;

        public void Update(GameTime gameTime)
        {
            if (GameOverTimer != null)
            {
                if (GameOverTimer.UpdateAndCheck(gameTime))
                {
                    GameOverTimer = null;
                    CanResetGameOver = true;
                }

                return;
            }

            if (CanResetGameOver)
            {
                foreach (var player in Players)
                {
                    player.inputManager.Update(player.playerIndex);

                    if (player.inputManager.IsActionTriggered(Player.Actions.Rotate) ||
                        player.inputManager.IsActionTriggered(Player.Actions.RotateCounter) ||
                        player.inputManager.IsActionTriggered(Player.Actions.Drop))
                    {
                        NewGame();
                        CanResetGameOver = false;
                        break;
                    }
                }

                return;
            }

            SoundState ss = SoundState.None;

            //allow players to move pieces
            foreach (var player in Players)
            {
                if (player.CurrentPiece == null)
                {
                    if (!SetupPlayerPiece(player))
                    {
                        ss = SoundState.Lose;
                        soundLose.Play();
                        GameOverTimer = new Timer(3000);

                        Toasts.AddToast(
                            new Rectangle(ScreenRect.X, 
                                ScreenRect.Y + (ScreenRect.Height/2), 
                                ScreenRect.Width, 100),
                            2000,
                            "Game Over",
                            Color.Red, 1f);

                        for (int x = 0; x < WidthInBlocks; x++)
                        {
                            for (int y = 0; y < HeightInBlocks; y++)
                            {
                                if (BlockMap[y, x] > 0)
                                {
                                    BlockMap[y, x] = 1;
                                }
                            }
                        }

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

                    if (CheckPieceInGrid(player.CurrentPiece,
                        player.piece_x, player.piece_y + 1))
                    {
                        player.piece_y++;
                        ss = SoundState.None;
                    }
                    else
                    {
                        ss = SoundState.AddToGrid;

                        AddPieceToGrid(player.CurrentPiece,
                            player.piece_x, player.piece_y);

                        var lines = ClearFilledLines();
                        if (lines > 0)
                        {
                            LineCount += lines;
                            ss = SoundState.ClearedLines;

                            int ix;
                            for (ix = 0; ix < LevelLines.Length; ix++)
                            {
                                if ((LevelLines[ix] >= LineCount) || (ix == LevelLines.Length - 1))
                                {
                                    if (Level < ix)
                                    {
                                        Level = ix;
                                        GridDropTime.SetDelay(LevelSpeeds[Level]);
                                        ss = SoundState.NextLevel;
                                    }
                                    break;
                                }
                            }

                            var c = Color.Gold;

                            if (lines >= ScoreMultiplier.Length)
                            {
                                lines = ScoreMultiplier.Length - 1;
                                c = Color.Aqua;
                                ss = SoundState.Tetris;
                            }

                            Score += ScoreMultiplier[lines - 1];

                            var r = new Rectangle(
                                this.ScreenRect.X + (player.piece_x * 32),
                                this.ScreenRect.Y + (player.piece_y * 32),
                                player.CurrentPiece.Width * 32,
                                player.CurrentPiece.Height * 32
                                );

                            Toasts.AddToast(r, 1500,
                                ScoreMultiplier[lines - 1].ToString(),
                                c, lines);
  
                            if (LineCount > BestLineCount)
                                BestLineCount = LineCount;
                            if (Score > BestScore)
                                BestScore = Score;
                        }

                        player.CurrentPiece = null;
                        player.repeatTimer = null;
                    }

                    switch (ss)
                    {
                        case SoundState.AddToGrid:
                            soundDrop.Play();
                            break;

                        case SoundState.ClearedLines:
                            soundLine.Play();
                            break;

                        case SoundState.NextLevel:
                            soundLevel.Play();
                            break;

                        case SoundState.Tetris:
                            soundTetris.Play();
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

                    //if (BlockIndex > Blocks. - 1)
                    //    BlockIndex = Blocks.Rows - 1;

                    Blocks.Draw(spriteBatch, /*0,*/ BlockIndex,
                        new Vector2(
                            ScreenRect.X + (BlockPixelSize * x),
                            ScreenRect.Y + (BlockPixelSize * y)));
                }
            }

            var XOffset = 5;
            var YOffset = 4;

            foreach (var piece in NextPieces)
            {
                piece.Draw(spriteBatch, Blocks, BlockPixelSize,
                    ScreenRect.X + (XOffset * BlockPixelSize),
                    ScreenRect.Y - (YOffset * BlockPixelSize) - 8 //8 to get above border
                    );

                XOffset += piece.Width + 1;
            }

            foreach (var player in Players)
            {
                player.Draw(spriteBatch, Blocks, BlockPixelSize);
            }
        }
    }
}
