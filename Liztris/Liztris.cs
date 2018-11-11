using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Liztris
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Liztris : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteSheet Bricks;
        Texture2D Background;
        SpriteFont fontScore, fontTitle;
        SoundEffect soundLine, soundLevel, soundLose, soundMusic, soundDrop;
        SoundEffectInstance soundMusicInstance;


        public Liztris()
        {
            graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            pieces = new int[PieceCount, PieceSize, PieceSize] { 
            {
                {0,1,1,0},
                {0,1,1,0},
                {0,0,0,0},
                {0,0,0,0},
            },
            {
                {0,2,0,0},
                {0,2,0,0},
                {0,2,2,0},
                {0,0,0,0},
            },
                        {
                {0,0,3,0},
                {0,0,3,0},
                {0,3,3,0},
                {0,0,0,0},
            },
                        {
                {0,4,0,0},
                {0,4,0,0},
                {0,4,0,0},
                {0,4,0,0},
            },
                        {
                {0,5,0,0},
                {0,5,5,0},
                {0,0,5,0},
                {0,0,0,0},
            },
                        {
                {0,0,6,0},
                {0,6,6,0},
                {0,6,0,0},
                {0,0,0,0},
            },
            };

            timer = 0;
            grid = new int[gridx, gridy];
            OldKeyboardState = Keyboard.GetState();
            soundMusicInstance.Volume = 0.20f;
            soundMusicInstance.IsLooped = true;
            soundMusicInstance.Play();
            NewGame();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            Texture2D tex = Content.Load<Texture2D>("Bricks");
            Bricks = new SpriteSheet(tex, 4, 6);

            Background = new Texture2D(GraphicsDevice, 1, 1);
            Background.SetData(new[] { Color.White });

            fontScore = Content.Load<SpriteFont>("Score");
            fontTitle = Content.Load<SpriteFont>("Title");

            soundLine = Content.Load<SoundEffect>("Line");
            soundLevel = Content.Load<SoundEffect>("Level");
            soundLose = Content.Load<SoundEffect>("Lose");
            soundMusic = Content.Load<SoundEffect>("Music");
            soundDrop = Content.Load<SoundEffect>("Drop");

            soundMusicInstance = soundMusic.CreateInstance();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Background.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            // next piece
            if (current_piece == null)
            {
                if (NewPiece() == false)
                {
                    soundLose.Play();
                    NewGame();
                }
            }

            //keyboard
            KeyboardState CurrentState = Keyboard.GetState();
            if (CurrentState.IsKeyDown(Keys.Left) && OldKeyboardState.IsKeyUp(Keys.Left))
            {
                if (CheckPiece(piece_x - 1, piece_y))
                    piece_x--;
            }
            else if (CurrentState.IsKeyDown(Keys.Right) && OldKeyboardState.IsKeyUp(Keys.Right))
            {
                if (CheckPiece(piece_x + 1, piece_y))
                     piece_x++;
            }
            else if (CurrentState.IsKeyDown(Keys.Space) && OldKeyboardState.IsKeyUp(Keys.Space))
            {
                if (CheckPiece(piece_x, piece_y + 1))
                    piece_y++;
            }
            else if (CurrentState.IsKeyDown(Keys.Down) && OldKeyboardState.IsKeyUp(Keys.Down))
            {
                //rotate
                //rotate
                int[,] new_piece = new int[PieceSize, PieceSize];

                for (int x = PieceSize - 1; x >= 0; x--)
                    for (int y = 0; y < PieceSize; y++)
                        new_piece[PieceSize - 1 - x, y] = current_piece[y, x];

                int[,] t = current_piece;
                current_piece = new_piece;
                if (CheckPiece(piece_x, piece_y) == false)
                    current_piece = t;
            }
            else if (CurrentState.IsKeyDown(Keys.Up) && OldKeyboardState.IsKeyUp(Keys.Up))
            {
                //rotate
                int[,] new_piece = new int[PieceSize, PieceSize];

                for (int x = PieceSize - 1; x >= 0; x--)
                    for (int y = 0; y < PieceSize; y++)
                        new_piece[y, PieceSize - 1 - x] = current_piece[x, y];

                int[,] t = current_piece;
                current_piece = new_piece;
                if (CheckPiece(piece_x, piece_y) == false)
                    current_piece = t;
            }

            OldKeyboardState = CurrentState;

            //drop time?
            timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (timer < GameDelay)
                return;

            timer -= GameDelay;

            int gamevent = 0;

            //game logic
            if (CheckPiece(piece_x, piece_y + 1))
            {
                piece_y++;
                gamevent = 0;
            }
            else
            {
                gamevent = 1;

                for (int x = 0; x < PieceSize; x++)
                    for (int y = 0; y < PieceSize; y++)
                    {
                        //piece has no block, ignore
                        if (current_piece[x, y] == 0)
                            continue;

                        //copy piece to grid
                        grid[piece_x + x, piece_y + y] = current_piece[x, y];
                    }

                current_piece = null;


                for (int y = gridy - 1; y >= 0; y--)
                {
                    int x;
                    for (x = 0; x < gridx; x++)
                    {
                        if (grid[x, y] == 0)
                            break;
                    }

                    if (x == gridx)
                    {
                        for (int y2 = y; y2 > 0; y2--)
                            for (x = 0; x < gridx; x++)
                                grid[x, y2] = grid[x, y2 - 1];

                        for (x = 0; x < gridx; x++)
                            grid[x, 0] = 0;

                        y++;
                        LineCount++;
                        gamevent = 2;

                        if ((LineCount > 0) && (LineCount % 5 == 0))
                        {
                            Level++;
                            GameDelay -= 100;
                            if (GameDelay < 250)
                                GameDelay = 250;

                            gamevent = 3;
                        }

                    }

                }
            }

            if (gamevent == 1)
                soundDrop.Play();
            else if (gamevent == 2)
                soundLine.Play();
            else if (gamevent == 3)
                soundLevel.Play();

                   
        }

        void NewGame()
        {
            GameDelay = 1000.0f;
            LineCount = 0;
            Level = 1;

            for (int x = 0; x < gridx; x++)
                for (int y = 0; y < gridy; y++)
                    grid[x, y] = 0;

            NewPiece();
        }

        bool NewPiece()
        {
            Random r = new Random();
            int t = r.Next(0, PieceCount); // Returns a random number from 0-99
            piece_x = (gridx - PieceSize) / 2;

            current_piece = new int[PieceSize, PieceSize];
            for (int x = 0; x < PieceSize; x++)
                for (int y = 0; y < PieceSize; y++)
                    current_piece[x,y] = pieces[t, x, y];

            piece_y = -PieceSize;
            int i = 0;
            for (i = 0; i <= PieceSize; i++)
            {
                if (CheckPiece(piece_x, piece_y))
                    break;
                piece_y++;
            }
            if (i == PieceSize + 1)
            {
                return false;
            }

            return true;
        }

        bool CheckPiece(int new_x, int new_y)
        {
            if (current_piece == null)
                return false;

            for (int x = 0; x < PieceSize; x++)
                for (int y = 0; y < PieceSize; y++)
                {
                    //piece has no block, ignore
                    if (current_piece[x, y] == 0)
                        continue;

                    //where is this block?
                    int tmp_x = new_x + x;
                    int tmp_y = new_y + y;

                    //block out of bounds
                    if ((tmp_x < 0) || (tmp_x >= gridx))
                        return false;

                    if ((tmp_y < 0) || (tmp_y >= gridy))
                        return false;

                    //block over grid block
                    if (grid[tmp_x, tmp_y] > 0)
                        return false;
                }

            return true;
        }

        KeyboardState OldKeyboardState;
        double timer;
        const int blocksize = 32;
        const int gridx = 8;
        const int gridy = 14;
        int piece_x = 0;
        int piece_y = 0;
        double GameDelay = 0;
        const int PieceSize = 4;
        const int PieceCount = 6;
        int LineCount;
        int Level;

        int[, ,] pieces;
        int[,] grid;
        int[,] current_piece = null;
   
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //draw game grid
            
            int offsetx = ((GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - ( (blocksize * gridx) / 2)) ;
            int offsety = 15;
            int border = 4;
            spriteBatch.Draw(Background, new Rectangle(offsetx - border, offsety - border, blocksize * gridx + border*2, blocksize * gridy + border*2), Color.LightGray);
            spriteBatch.Draw(Background, new Rectangle(offsetx, offsety, blocksize * gridx, blocksize * gridy), Color.DarkSlateGray);

            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 10), Color.Black);
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 8), Color.Red);
            spriteBatch.DrawString(fontScore, "Level: " + Level.ToString(), new Vector2(offsetx + blocksize * gridx + 60, 15), Color.Black);
            spriteBatch.DrawString(fontScore, "Lines: " + LineCount.ToString(), new Vector2(offsetx + blocksize * gridx + 60, 50), Color.Black);

            spriteBatch.End();

            for (int x = 0; x < gridx; x++)
                for (int y = 0; y < gridy; y++)
                    if (grid[x, y] > 0)
                        Bricks.Draw(spriteBatch, 0, grid[x, y] - 1, new Vector2(offsetx + blocksize * x, offsety + blocksize * y));


            //draw piece
            if (current_piece != null)
                for (int x = 0; x < PieceSize; x++)
                    for (int y = 0; y < PieceSize; y++)
                        if (current_piece[x, y] > 0)
                            Bricks.Draw(spriteBatch, 0, current_piece[x, y] - 1, new Vector2(offsetx + blocksize * (piece_x + x), offsety + blocksize * (piece_y + y)));

   
            base.Draw(gameTime);
        }
    }
}
