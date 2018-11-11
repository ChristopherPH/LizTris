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

        const int GridXPerPlayer = 8;
        const int GridY = 13;
        const int BlockPixelSize = 32;
        const int BlocksBetweenGrids = 4;
        const int BlocksAboveBottom = 1;

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
            SetupGame(2, true);
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
                var grid = new Grid(GridXPerPlayer * PlayerCount, GridY, LevelSpeeds);
                Grids.Add(grid);

                for (int i = 0; i < PlayerCount; i++)
                {
                    var player = new Player(grid, (PlayerIndex)i, null);
                    grid.Players.Add(player);
                }
            }
            else
            {
                for (int i = 0; i < PlayerCount; i++)
                {
                    var grid = new Grid(GridXPerPlayer, GridY, LevelSpeeds);
                    Grids.Add(grid);

                    var player = new Player(grid, (PlayerIndex)i, null);
                    grid.Players.Add(player);
                }
            }

            //setup screen and grid locations
            var GridPad = (BlockPixelSize * BlocksBetweenGrids);
            var HeightPad = (BlockPixelSize * BlocksAboveBottom);

            var TotalGridPixelWidth = Grids.Sum(grid => (BlockPixelSize * grid.WidthInBlocks)) + ((Grids.Count - 1) * GridPad);
            var StartOffsetX = ((GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - (TotalGridPixelWidth / 2));

            var MaxGridPixelHeight = Grids.Max(grid => (BlockPixelSize * grid.HeightInBlocks)) + HeightPad;
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

            foreach (var grid in Grids)
                grid.Update(gameTime);
        }


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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
