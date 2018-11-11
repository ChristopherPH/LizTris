using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        Texture2D transparentDarkTexture;
        Texture2D[] Background;
        SpriteSheet Blocks;
        SpriteFont fontScore, fontTitle, fontMenu;
        SoundEffect soundLine, soundLevel, soundLose, musicDefault, soundDrop, soundTetris;
        SoundEffectInstance musicDefaultInstance;
        int BackgroundIndex = 0;

        enum GlobalCommands
        {
            Menu,
        }

        enum GlobalMenuItems
        {
            Resume,
            NewGame,
            Options,
            Quit,

            OnePlayer,
            TwoPlayer,
            ThreePlayer,
            FourPlayer,

            GridPerPlayer,
            SharedGrid,

            BackToGameMenu,
            BackToPlayerCountMenu,
        }

        Menu<GlobalMenuItems> StartMenu = null;
        Menu<GlobalMenuItems> GameMenu = null;
        Menu<GlobalMenuItems> PlayerCountMenu = null;
        Menu<GlobalMenuItems> SharedGrid = null;

        Menu<GlobalMenuItems> CurrentGameMenu = null;
        Menu<GlobalMenuItems> CurrentMenu = null;

        InputManager<GlobalCommands> inputManager = new InputManager<GlobalCommands>();

        List<Grid> Grids = new List<Grid>();

        const int GridXPerPlayer = 8;
        const int GridY = 17;
        const int BlockPixelSize = 32;
        const int BlocksBetweenGrids = 8;
        const int BlocksAboveBottom = 1;
        const int PlayerCount = 2;
        const bool ShareGrid = true;

        public int[] LevelSpeeds = { 1000, 900, 800, 700, 600, 500, 400, 350, 300, 250, 225, 200, 175, 150, 125 };
        public int[] LevelLines =  { 0,    10,  20,  30,  40,  50,  60,  70,  80,  90,  100, 125, 150, 175, 200 };
        public int[] ScoreMultiplier = { 50, 250, 500, 1000 };

        protected override int WantedGameResolutionWidth => 1280;
        protected override int WantedGameResolutionHeight => 720;
        protected override int WindowWidth => 1600;
        protected override int WindowHeight => 900;
        protected override bool WindowFullScreen => false;

        public Liztris()
        {
            Common.GameMenu.MenuTest.Savemenu();

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

            inputManager.AddAction(GlobalCommands.Menu, Keys.Escape);
            inputManager.AddAction(GlobalCommands.Menu, InputManager<GlobalCommands>.GamePadButtons.Start);
            inputManager.AddAction(GlobalCommands.Menu, InputManager<GlobalCommands>.GamePadButtons.Back);

            musicDefaultInstance.Volume = 0.20f;
            musicDefaultInstance.IsLooped = true;
            musicDefaultInstance.Play();

            CurrentGameMenu = StartMenu;
            CurrentMenu = CurrentGameMenu;
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
            transparentDarkTexture = new Texture2D(GraphicsDevice, 1, 1);
            transparentDarkTexture.SetData(new[] { Color.DarkSlateGray });

            Texture2D tex = Content.Load<Texture2D>("Graphics/Blocks");
            Blocks = new SpriteSheet(tex, 5, 4);

            Background = new Texture2D[3];
            Background[0] = Content.Load<Texture2D>("Backgrounds/Beach"); 
            Background[1] = Content.Load<Texture2D>("Backgrounds/Ocean"); 
            Background[2] = Content.Load<Texture2D>("Backgrounds/Ruins");
            BackgroundIndex = 0;

            fontScore = Content.Load<SpriteFont>("Fonts/Score");
            fontTitle = Content.Load<SpriteFont>("Fonts/Title");
            fontMenu = Content.Load<SpriteFont>("Fonts/Menu");

            Toasts.spriteFont = Content.Load<SpriteFont>("Fonts/Toast");

            musicDefault = Content.Load<SoundEffect>("Music/Music");

            soundLine = Content.Load<SoundEffect>("Sounds/Line");
            soundLevel = Content.Load<SoundEffect>("Sounds/Level");
            soundLose = Content.Load<SoundEffect>("Sounds/Lose");
            soundDrop = Content.Load<SoundEffect>("Sounds/Drop");
            soundTetris = Content.Load<SoundEffect>("Sounds/Tetris");

            musicDefaultInstance = musicDefault.CreateInstance();


            StartMenu = new Menu<GlobalMenuItems>(new Menu<GlobalMenuItems>.MenuItem[] {
                new Menu<GlobalMenuItems>.MenuItem("New Game", GlobalMenuItems.NewGame),
                new Menu<GlobalMenuItems>.MenuItem("Full Screen", GlobalMenuItems.Options),
                new Menu<GlobalMenuItems>.MenuItem("Quit", GlobalMenuItems.Quit),
            }, fontMenu);

            GameMenu = new Menu<GlobalMenuItems>(new Menu<GlobalMenuItems>.MenuItem[] {
                new Menu<GlobalMenuItems>.MenuItem("Resume", GlobalMenuItems.Resume),
                new Menu<GlobalMenuItems>.MenuItem("New Game", GlobalMenuItems.NewGame),
                new Menu<GlobalMenuItems>.MenuItem("Full Screen", GlobalMenuItems.Options),
                new Menu<GlobalMenuItems>.MenuItem("Quit", GlobalMenuItems.Quit),
            }, fontMenu, 0, GlobalMenuItems.Resume);

            PlayerCountMenu = new Menu<GlobalMenuItems>(new Menu<GlobalMenuItems>.MenuItem[] {
                new Menu<GlobalMenuItems>.MenuItem("One Player", GlobalMenuItems.OnePlayer),
                new Menu<GlobalMenuItems>.MenuItem("Two Players", GlobalMenuItems.TwoPlayer),
                new Menu<GlobalMenuItems>.MenuItem("Three Players", GlobalMenuItems.ThreePlayer),
                new Menu<GlobalMenuItems>.MenuItem("Four Players", GlobalMenuItems.FourPlayer),
                new Menu<GlobalMenuItems>.MenuItem("Back", GlobalMenuItems.BackToGameMenu),
            }, fontMenu, 0, GlobalMenuItems.BackToGameMenu);

            SharedGrid = new Menu<GlobalMenuItems>(new Menu<GlobalMenuItems>.MenuItem[] {
                new Menu<GlobalMenuItems>.MenuItem("Grid Per Player", GlobalMenuItems.GridPerPlayer),
                new Menu<GlobalMenuItems>.MenuItem("Shared Grid", GlobalMenuItems.SharedGrid),
                new Menu<GlobalMenuItems>.MenuItem("Back", GlobalMenuItems.BackToPlayerCountMenu),
            }, fontMenu, 0, GlobalMenuItems.BackToPlayerCountMenu);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
            // TODO: Unload any non ContentManager content here
            transparentDarkTexture.Dispose();
        }


        public void NewGame(int PlayerCount, bool SharedGrid)
        {
            foreach (var grid in Grids)
                grid.Players.Clear();
            Grids.Clear();

            if (SharedGrid)
            {
                var grid = new Grid(GridXPerPlayer * PlayerCount, GridY, LevelSpeeds, LevelLines, ScoreMultiplier);
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
                    var grid = new Grid(GridXPerPlayer, GridY, LevelSpeeds, LevelLines, ScoreMultiplier);
                    Grids.Add(grid);

                    var player = new Player(grid, (PlayerIndex)i, null);
                    grid.Players.Add(player);
                }
            }

            //setup screen and grid locations
            var BlocksBetweenGridsPerPlayer = BlocksBetweenGrids / PlayerCount;
            var GridPad = (BlockPixelSize * BlocksBetweenGridsPerPlayer);
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

                XOffset += BlockPixelSize * (grid.WidthInBlocks + BlocksBetweenGridsPerPlayer);

                grid.soundDrop = soundDrop;
                grid.soundLine = soundLine;
                grid.soundLose = soundLose;
                grid.soundLevel = soundLevel;
                grid.soundTetris = soundTetris;
            }

            BackgroundIndex = new Random().Next(3);

            foreach (var grid in Grids)
                grid.NewGame();
        }

        private int mnuPlayers = 0;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);

            if (CurrentMenu != null)
            {
                if (CurrentMenu.Update(gameTime, out GlobalMenuItems? Choice))
                {
                    if (Choice != null)
                    {
                        switch (Choice.Value)
                        {
                            case GlobalMenuItems.Quit:
                                musicDefaultInstance.Stop();
                                Exit();
                                break;

                            case GlobalMenuItems.Resume:
                                CurrentMenu = null;
                                break;

                            case GlobalMenuItems.NewGame:
                                CurrentMenu = PlayerCountMenu;
                                CurrentMenu.ResetMenu();
                                mnuPlayers = 0;
                                break;

                            case GlobalMenuItems.OnePlayer:
                                mnuPlayers = 1;
                                CurrentMenu = SharedGrid;
                                CurrentMenu.ResetMenu();
                                break;

                            case GlobalMenuItems.TwoPlayer:
                                mnuPlayers = 2;
                                CurrentMenu = SharedGrid;
                                CurrentMenu.ResetMenu();
                                break;

                            case GlobalMenuItems.ThreePlayer:
                                mnuPlayers = 3;
                                CurrentMenu = SharedGrid;
                                CurrentMenu.ResetMenu();
                                break;

                            case GlobalMenuItems.FourPlayer:
                                mnuPlayers = 4;
                                CurrentMenu = SharedGrid;
                                CurrentMenu.ResetMenu();
                                break;

                            case GlobalMenuItems.GridPerPlayer:
                                NewGame(mnuPlayers, false);
                                CurrentMenu = null;
                                break;

                            case GlobalMenuItems.SharedGrid:
                                NewGame(mnuPlayers, true);
                                CurrentMenu = null;
                                break;

                            case GlobalMenuItems.BackToGameMenu:
                                CurrentMenu = CurrentGameMenu;
                                CurrentMenu.ResetMenu();
                                break;

                            case GlobalMenuItems.BackToPlayerCountMenu:
                                CurrentMenu = PlayerCountMenu;
                                CurrentMenu.ResetMenu();
                                mnuPlayers = 0;
                                break;

                            case GlobalMenuItems.Options:
                                IndependentResolutionRendering.Resolution.SetResolution(1920, 1080, true);
                                break;
                        }
                    }
                }

                return;
            }

            inputManager.Update(PlayerIndex.One);
            if (inputManager.IsActionTriggered(GlobalCommands.Menu))
            {
                CurrentGameMenu = GameMenu; //change to resumable menu

                CurrentMenu = CurrentGameMenu;
                CurrentMenu.ResetMenu();
                
                return;
            }

            foreach (var grid in Grids)
                grid.Update(gameTime);

            Toasts.Update(gameTime);
        }

       


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

            spriteBatch.Draw(Background[BackgroundIndex], GameRectangle, Color.White);

            if (CurrentMenu != null)
            {
                spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 250), Color.Black,
                    -0.5f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 248), Color.Red,
                    -0.5f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);

                var MenuRect = new Rectangle(
                    (GamePixelWidth / 8) * 3, 
                    GamePixelHeight / 4, 
                    GamePixelWidth / 4, 
                    GamePixelHeight / 2);

                spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

                spriteBatch.Draw(transparentDarkTexture, MenuRect, Color.White * 0.5f);

                CurrentMenu.Draw(spriteBatch, MenuRect);
                spriteBatch.End();
                return;
            }

            //draw
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 2), Color.Black);
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 0), Color.Red);

            foreach (var grid in Grids)
            {
                //draw grid info on upper left of grid
                spriteBatch.DrawString(fontScore, "Level: " + grid.Level.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 140), Color.Black);
                spriteBatch.DrawString(fontScore, "Lines: " + grid.LineCount.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 100), Color.Black);
                spriteBatch.DrawString(fontScore, "Score: " + grid.Score.ToString(),
                    new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 60), Color.Black);

                //draw grid border
                var borderRect = grid.ScreenRect;

                //spriteBatch.DrawRectangle(borderRect, Color.Gray, 6, false);
                spriteBatch.DrawRectangle(borderRect, Color.Teal, 3, false);

                spriteBatch.Draw(transparentDarkTexture, grid.ScreenRect, Color.White * 0.5f);

                //draw grid
                grid.Draw(spriteBatch, Blocks, BlockPixelSize);
            }

            Toasts.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
