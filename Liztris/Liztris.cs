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
    public class Liztris : Common.GameResolution
    {
        GraphicsDeviceManager graphics;
        Common.ExtendedSpriteBatch spriteBatch;
        Texture2D BlankTexture;
        Texture2D tmpTexture;
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
        const int GridY = 18;
        const int BlockPixelSize = 32;
        const int BlocksBetweenGrids = 4;
        const int BlocksAboveBottom = 1;
        const int PlayerCount = 2;
        const bool ShareGrid = true;

        public int[] LevelSpeeds = { 1000, 900, 800, 700, 600, 500, 400, 350, 300, 250 };

        protected override int WantedGameResolutionWidth => 1280;
        protected override int WantedGameResolutionHeight => 720;
        protected override int WindowWidth => 1600;
        protected override int WindowHeight => 900;
        protected override bool WindowFullScreen => false;


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

            SetupGameResolution(ref graphics);
            
            //Window.IsBorderless = true;
            //Window.Position = new Point(0, 0);

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
            //soundMusicInstance.Play();
            SetupGame(PlayerCount, ShareGrid);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new Common.ExtendedSpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            BlankTexture = new Texture2D(GraphicsDevice, 1, 1);
            BlankTexture.SetData(new[] { Color.White });

            tmpTexture = new Texture2D(GraphicsDevice, 1, 1);
            tmpTexture.SetData(new[] { Color.DarkSlateGray });

            //Texture2D tex = Content.Load<Texture2D>("Bricks");
            //Blocks = new SpriteSheet(tex, 4, 6);

            Texture2D tex = Content.Load<Texture2D>("Blocks");
            Blocks = new SpriteSheet(tex, 5, 4);

            Background = Content.Load<Texture2D>("Backgrounds/Beach");

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
            BlankTexture.Dispose();
            tmpTexture.Dispose();
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
            var StartOffsetX = ((GamePixelWidth / 2) - (TotalGridPixelWidth / 2));

            var MaxGridPixelHeight = Grids.Max(grid => (BlockPixelSize * grid.HeightInBlocks)) + HeightPad;
            var StartOffsetY = GamePixelHeight - MaxGridPixelHeight;

            var XOffset = StartOffsetX;
            var YOffset = StartOffsetY;

            foreach (var grid in Grids)
            {
                grid.ScreenRect = new Rectangle(XOffset, YOffset,
                    BlockPixelSize * grid.WidthInBlocks,
                    BlockPixelSize * grid.HeightInBlocks);

                XOffset += BlockPixelSize * (grid.WidthInBlocks + BlocksBetweenGrids);
            }

            NewGame();

            MainMenu = new Menu(new string[]
            {
                "New Game",
                "Options",
                "Quit"
            }, fontScore);
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

            if (MainMenu != null)
            {
                if (MainMenu.Update(gameTime, out int Choice))
                {
                    //do something
                    switch (Choice)
                    {
                        case 0:
                            NewGame();
                            MainMenu = null;
                            break;
                        case 1: break;
                        case 2: Exit(); break;
                        case 3:
                            MainMenu = null;
                            break;
                    }
                }

                return;
            }

            inputManager.Update(PlayerIndex.One);

            if (inputManager.IsActionTriggered(GlobalCommands.Pause))
            {
                MainMenu = new Menu(new string[]
                {
                "New Game",
                "Options",
                "Quit",
                "Resume",
                }, fontScore, 3);
            }

            foreach (var grid in Grids)
                grid.Update(gameTime);
        }

        Menu MainMenu = null;


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            var drawMatrix = GameToMonitorMatrix;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                null, null, null, null, drawMatrix);

            spriteBatch.FillRectangle(GameRectangle, Color.CornflowerBlue);

            spriteBatch.Draw(Background, GameRectangle, Color.White);

            if (MainMenu != null)
            {
                spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 150), Color.Black,
                    -0.4f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 148), Color.Red,
                    -0.4f, Vector2.Zero, 2f, SpriteEffects.None, 0f);

                var MenuRect = new Rectangle(
                    (GamePixelWidth / 8) * 3, 
                    GamePixelHeight / 4, 
                    GamePixelWidth / 4, 
                    GamePixelHeight / 2);

                spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

                spriteBatch.Draw(tmpTexture, MenuRect, Color.White * 0.5f);

                MainMenu.Draw(spriteBatch, MenuRect);
                spriteBatch.End();
                return;
            }

            //draw
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 10), Color.Black);
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 8), Color.Red);

            foreach (var grid in Grids)
            {
                //draw grid info on upper left of grid
                spriteBatch.DrawString(fontScore, "Lines: " + grid.LineCount.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 100), Color.Black);
                spriteBatch.DrawString(fontScore, "Level: " + grid.Level.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 60), Color.Black);

                //draw grid border
                var borderRect = grid.ScreenRect;

                //spriteBatch.DrawRectangle(borderRect, Color.Gray, 6, false);
                spriteBatch.DrawRectangle(borderRect, Color.Teal, 3, false);

                spriteBatch.Draw(tmpTexture, grid.ScreenRect, Color.White * 0.5f);

                //draw grid
                grid.Draw(spriteBatch, Blocks, BlockPixelSize);
            }

            spriteBatch.End();
        }
    }
}
