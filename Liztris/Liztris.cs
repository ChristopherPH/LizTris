using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Liztris
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Liztris : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D Background;
        SpriteSheet Blocks;
        SpriteFont fontScore, fontTitle;
        SoundEffect soundLine, soundLevel, soundLose, soundMusic, soundDrop;
        SoundEffectInstance soundMusicInstance;

        enum GlobalCommands
        {
            Pause,
            MenuSelect,
            MenuUp,
            MenuDown,
        }

        InputManager<GlobalCommands> inputManager = new InputManager<GlobalCommands>();

        List<Grid> Grids = new List<Grid>();
        List<Player> Players = new List<Player>();

        const int GridXPerPlayer = 8;
        const int GridY = 7;
        const int BlockPixelSize = 32;
        const int BlocksBetweenGrids = 4;

        public int[] LevelSpeeds = { 1000, 900, 800, 700, 600, 500, 400, 350, 300, 250 };

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

            inputManager.AddAction(GlobalCommands.Pause, Keys.Escape);
            inputManager.AddAction(GlobalCommands.Pause, InputManager<GlobalCommands>.GamePadButtons.Start);
            inputManager.AddAction(GlobalCommands.Pause, InputManager<GlobalCommands>.GamePadButtons.Back);

            inputManager.AddAction(GlobalCommands.MenuSelect, Keys.Enter);
            inputManager.AddAction(GlobalCommands.MenuSelect, InputManager<GlobalCommands>.GamePadButtons.A);

            inputManager.AddAction(GlobalCommands.MenuUp, Keys.Up);
            inputManager.AddAction(GlobalCommands.MenuUp, InputManager<GlobalCommands>.GamePadButtons.Up);

            inputManager.AddAction(GlobalCommands.MenuDown, Keys.Down);
            inputManager.AddAction(GlobalCommands.MenuDown, InputManager<GlobalCommands>.GamePadButtons.Down);

            soundMusicInstance.Volume = 0.20f;
            soundMusicInstance.IsLooped = true;
            soundMusicInstance.Play();
            SetupGame(1, false);
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
            Background = new Texture2D(GraphicsDevice, 1, 1);
            Background.SetData(new[] { Color.White });

            Texture2D tex = Content.Load<Texture2D>("Bricks");
            Blocks = new SpriteSheet(tex, 4, 6);

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


        public void SetupGame(int PlayerCount, bool SharedGrid)
        {
            if (SharedGrid)
            {
                var g = new Grid(GridXPerPlayer * PlayerCount, GridY);
                Grids.Add(g);

                for (int i = 0; i < PlayerCount; i++)
                    Players.Add(new Player(g, (PlayerIndex)i, null));
            }
            else
            {
                for (int i = 0; i < PlayerCount; i++)
                {
                    var g = new Grid(GridXPerPlayer, GridY);
                    Grids.Add(g);

                    Players.Add(new Player(g, (PlayerIndex)i, null));
                }
            }

            //setup screen and grid locations
            var GridPad = (BlockPixelSize * BlocksBetweenGrids);

            var TotalGridPixelWidth = Grids.Sum(grid => (BlockPixelSize * grid.WidthInBlocks)) + ((Grids.Count - 1) * GridPad);
            var StartOffsetX = ((GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - (TotalGridPixelWidth / 2));

            var MaxGridPixelHeight = Grids.Max(grid => (BlockPixelSize * grid.HeightInBlocks)) + (BlockPixelSize * 2);
            //int StartOffsetY = ((GraphicsDevice.PresentationParameters.BackBufferHeight / 2) - (MaxGridPixelHeight / 2));
            var StartOffsetY = GraphicsDevice.PresentationParameters.BackBufferHeight - MaxGridPixelHeight;

            var XOffset = StartOffsetX;
            var YOffset = StartOffsetY;

            foreach (var grid in Grids)
            {
                //draw grid border
                grid.ScreenRect = new Rectangle(XOffset, YOffset,
                    BlockPixelSize * grid.WidthInBlocks,
                    BlockPixelSize * grid.HeightInBlocks);

                XOffset += BlockPixelSize * (grid.WidthInBlocks + BlocksBetweenGrids);
            }

            NewGame();
        }


        public void NewGame()
        {
            foreach (var grid in Grids)
                grid.NewGame();

            foreach (var player in Players)
                player.NewGame();
        }

        enum PieceState
        {
            None = 0,
            AddToGrid = 1,
            ClearedLines = 2,
            NextLevel = 3,
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);

            inputManager.Update(PlayerIndex.One);

            if (inputManager.IsActionPressed(GlobalCommands.Pause))
                Exit();

            foreach (var player in Players)
                player.Update(gameTime);
            /*
            foreach (var grid in Grids)
                grid.Update(gameTime);

            // next piece
            if (Player1.CurrentPiece == null)
            {
                if (Player1.NewPiece() == false)
                {
                    soundLose.Play();
                    NewGame();
                }
            }
            

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

                Player1.Grid.AddPieceToGrid(Player1.CurrentPiece, Player1.piece_x, Player1.piece_y);
                Player1.CurrentPiece = null;
                var clearedLines = Player1.Grid.ClearFilledLines();
                
                if (clearedLines > 0)
                {
                    Player1.LineCount += clearedLines;
                    gamevent = 2;

                    if ((Player1.LineCount > 0) && (Player1.LineCount % 5 == 0))
                    {
                        Player1.Level++;
                        GameDelay -= 100;
                        if (GameDelay < 250)
                            GameDelay = 250;

                        gamevent = 3;
                    }
                }
            }

            if (gamevent == 1)
                soundDrop.Play();
            else if (gamevent == 2)
                soundLine.Play();
            else if (gamevent == 3)
                soundLevel.Play();
                */
        }


        public enum PlayerAction
        {
            Pause,
            RestartAllPlayers,
            HelpPlayers,
            HinderPlayers,
        }


        KeyboardState OldKeyboardState;
        double timer;
        double GameDelay = 0;

        

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            

            //draw
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 10), Color.Black);
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 8), Color.Red);

            foreach (var grid in Grids)
            {
                //draw grid info on upper left of grid
                spriteBatch.DrawString(fontScore, "Lines: " + grid.LineCount.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 120), Color.Black);
                spriteBatch.DrawString(fontScore, "Level: " + grid.Level.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 80), Color.Black);

                //draw grid border
                var borderRect = grid.ScreenRect;

                borderRect.Inflate(BlockPixelSize/2, BlockPixelSize/2);
                spriteBatch.Draw(Background, borderRect, Color.LightGray);
                spriteBatch.Draw(Background, grid.ScreenRect, Color.DarkSlateGray);

                //draw grid
                grid.Draw(spriteBatch, Blocks, BlockPixelSize);
            }

            foreach (var player in Players)
            {
                player.Draw(spriteBatch, Blocks, BlockPixelSize);
            }
   
            base.Draw(gameTime);

            spriteBatch.End();
        }
    }
}
