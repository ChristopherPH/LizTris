//#define SHOWSTUFF
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Liztris
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Liztris : GameResolution
    {
        GraphicsDeviceManager graphics;
        ExtendedSpriteBatch spriteBatch;
        Texture2D transparentDarkTexture;
        Texture2D[] Background;
        SpriteSheet Blocks;
        SpriteFont fontScore, fontTitle, fontMenu, fontTitleHuge;
        SoundEffect soundLine, soundLevel, soundLose, musicDefault, soundDrop, soundTetris;
        SoundEffectInstance musicDefaultInstance;
        int BackgroundIndex = 0;
        bool ShowHighScores = false;

        enum GlobalCommands
        {
            Menu,
        }

        InputManager<GlobalCommands> inputManager = new InputManager<GlobalCommands>();

        List<Grid> Grids = new List<Grid>();

        const int GridXPerPlayer = 8;
        const int GridY = 17;
        const int BlockPixelSize = 32;
        const int BlocksBetweenGrids = 8;
        const int BlocksAboveBottom = 1;

        public int[][] LevelSpeeds = { //Slow, Normal, Fast
            new int[] { 1500, 1400, 1300, 1200, 1100, 1000, 900, 800, 750, 700, 650, 600, 550, 500, 500 },
            new int[] { 1000, 900,  800,  700,  600,  500,  400, 350, 300, 250, 225, 200, 175, 150, 125 },
            new int[] { 500,  450,  400,  350,  300,  250,  200, 175, 150, 125, 120, 115, 110, 105, 100 } };
        public int[] LevelLines =  
                      { 0,    10,   20,   30,   40,   50,   60,  70,  80,  90,  100, 125, 150, 175, 200 };
        public int[] ScoreMultiplier = { 100, 250, 500, 1000 };

        protected override int WantedGameResolutionWidth => 1280;
        protected override int WantedGameResolutionHeight => 720;

        protected override int WindowWidth => Program.Settings.Video.Width;
        protected override int WindowHeight => Program.Settings.Video.Height;
        protected override bool WindowFullScreen =>
            Program.Settings.Video.WindowMode == VideoSettings.WindowModeTypes.Fullscreen;

        public Liztris()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            GameMenus.MainMenu.Options["Fullscreen"] =
                Program.Settings.Video.WindowMode == VideoSettings.WindowModeTypes.Fullscreen;
            GameMenus.MainMenu.Options["VSync"] = Program.Settings.Video.VSync;

            GameMenus.MainMenu.Options["Resolution"] = 
                string.Format("{0}x{1}", Program.Settings.Video.Width, Program.Settings.Video.Height);

            GameMenus.MainMenu.Options["Music"] = Program.Settings.Audio.Music;
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

            if (Program.Settings.Audio.Music)
                musicDefaultInstance.Play();

#if SHOWSTUFF


#if FREE_FPS_LPS //fps and lps are in sync, as fast as possible, no vsync
            this.IsFixedTimeStep = false; //default is true
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
#endif

#if MONITOR_FPS_LPS //fps and lps are in sync, hovers around monitor refresh rate
            this.IsFixedTimeStep = false; //default is true
#endif

#if FIXED_LPS_LIMITED_TO_MONIOR_FPS //120 lps, 60 fps
            var targetFPS = 120;
            TargetElapsedTime = TimeSpan.FromTicks(10000000 / targetFPS);
#endif

#if FIXED_LPS_FPS //30 fps, 30 lps
            var targetFPS = 30;
            TargetElapsedTime = TimeSpan.FromTicks(10000000 / targetFPS);
#endif

#if FIXED_LPS_FIXED_FPS //120 lps, 120 fps, no vsync
            var targetFPS = 120;
            TargetElapsedTime = TimeSpan.FromTicks(10000000 / targetFPS);
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
#endif

#if FIXED_LPS_FPS //30 fps, 30 lps, no vsync
            var targetFPS = 30;
            TargetElapsedTime = TimeSpan.FromTicks(10000000 / targetFPS);
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();
#endif

#if DEFAULT //60 fps, 60 lps, vsync

#endif

#endif

            GameMenus.MainMenu.ActionHandler = (object Action) =>
            {
                switch ((GameMenus.GameMenuOptions)Action)
                {
                    case GameMenus.GameMenuOptions.Exit:
                        musicDefaultInstance.Stop();
                        Exit();
                        break;

                    case GameMenus.GameMenuOptions.NewGame:
                        var players = (int)GameMenus.MainMenu.Options["Players"];
                        var speed = (int)GameMenus.MainMenu.Options["Speed"];
                        var sharedgrid = (bool)GameMenus.MainMenu.Options["SharedGrid"];
                        NewGame(players, speed, sharedgrid);
                        GameMenus.MainMenu.ExitMenu();
                        break;

                    case GameMenus.GameMenuOptions.ApplyGraphics:
                        if ((bool)GameMenus.MainMenu.Options["Fullscreen"])
                            Program.Settings.Video.WindowMode = VideoSettings.WindowModeTypes.Fullscreen;
                        else
                            Program.Settings.Video.WindowMode = VideoSettings.WindowModeTypes.Windowed;

                        Program.Settings.Video.VSync = (bool)GameMenus.MainMenu.Options["VSync"];

                        var resolution = (string)GameMenus.MainMenu.Options["Resolution"];

                        switch (resolution)
                        {
                            case "1280x720": Program.Settings.Video.Width = 1280; Program.Settings.Video.Height = 720; break;
                            case "1366x768": Program.Settings.Video.Width = 1366; Program.Settings.Video.Height = 768; break;
                            case "1600x900": Program.Settings.Video.Width = 1600; Program.Settings.Video.Height = 900; break;
                            case "1920x1080": Program.Settings.Video.Width = 1920; Program.Settings.Video.Height = 1080; break;
                        }

                        IndependentResolutionRendering.Resolution.SetResolution(
                            Program.Settings.Video.Width, Program.Settings.Video.Height,
                            Program.Settings.Video.WindowMode == VideoSettings.WindowModeTypes.Fullscreen);

                        this.IsFixedTimeStep = Program.Settings.Video.VSync;
                        graphics.SynchronizeWithVerticalRetrace = Program.Settings.Video.VSync;
                        graphics.ApplyChanges();
                        Program.Settings.SaveSettings();
                        break;

                    case GameMenus.GameMenuOptions.ChangeAudio:
                        //var sound = (bool)GameMenus.MainMenu.Options["Sound"];
                        Program.Settings.Audio.Music = (bool)GameMenus.MainMenu.Options["Music"];

                        if (Program.Settings.Audio.Music)
                            musicDefaultInstance.Play();
                        else
                            musicDefaultInstance.Stop();

                        Program.Settings.SaveSettings();
                        break;

                    case GameMenus.GameMenuOptions.ShowScores:
                        ShowHighScores = true;
                        break;
                }
            };

            GameMenus.MainMenu.ShowMenu();

            GameMenus.PauseMenu.ActionHandler = (object Action) =>
            {
                switch ((GameMenus.GameMenuOptions)Action)
                {
                    case GameMenus.GameMenuOptions.QuitGame:
                        //HACK: leaving current state, reset input manager of new state
                        GameMenus.MainMenu.ShowMenu();
                        GameMenus.MainMenu.ResetInputs();
                        break;
                }
            };

            Intro.IsActive = true;


            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                System.Diagnostics.Debug.Print("{0}x{1}   asp={2}  format={3}   safe={4}",
                    mode.Width, mode.Height, mode.AspectRatio, mode.Format, mode.TitleSafeArea);
                //mode.whatever (and use any of avaliable information)
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new ExtendedSpriteBatch(GraphicsDevice);

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
            fontTitleHuge = Content.Load<SpriteFont>("Fonts/TitleHuge");

            Toasts.spriteFont = Content.Load<SpriteFont>("Fonts/Toast");

            musicDefault = Content.Load<SoundEffect>("Music/Music");

            soundLine = Content.Load<SoundEffect>("Sounds/Line");
            soundLevel = Content.Load<SoundEffect>("Sounds/Level");
            soundLose = Content.Load<SoundEffect>("Sounds/Lose");
            soundDrop = Content.Load<SoundEffect>("Sounds/Drop");
            soundTetris = Content.Load<SoundEffect>("Sounds/Tetris");

            musicDefaultInstance = musicDefault.CreateInstance();
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


        public void NewGame(int PlayerCount, int SpeedType, bool SharedGrid)
        {
            foreach (var grid in Grids)
                grid.Players.Clear();
            Grids.Clear();

            if (SharedGrid)
            {
                var grid = new Grid(GridXPerPlayer * PlayerCount, GridY, LevelSpeeds[SpeedType], LevelLines, ScoreMultiplier);
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
                    var grid = new Grid(GridXPerPlayer, GridY, LevelSpeeds[SpeedType], LevelLines, ScoreMultiplier);
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

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here

            base.Update(gameTime);

#if SHOWSTUFF
            if (logicTimer.UpdateAndCheck(gameTime))
                logic = 1 / gameTime.ElapsedGameTime.TotalSeconds;

            mousePos = TranslateLocation(Mouse.GetState().Position.ToVector2());
#endif

            if (!toggle)
            {
                if (toggleTimer.UpdateAndCheck(gameTime))
                    toggle = !toggle;
            }
            else if (toggle2Timer.UpdateAndCheck(gameTime))
                toggle = !toggle;

            //Gamestate: Intro
            if (Intro.IsActive)
            {
                Intro.Update(gameTime);
                return;
            }

            if (ShowHighScores)
            {
                inputManager.Update(PlayerIndex.One);
                if (inputManager.IsActionTriggered(GlobalCommands.Menu))
                {
                    ShowHighScores = false;
                }
                return;
            }

            //Gamestate: MainMenu
            if (GameMenus.MainMenu.IsMenuActive)
            {
                GameMenus.MainMenu.Update(gameTime);
                return;
            }

            //Gamestate: PauseMenu
            if (GameMenus.PauseMenu.IsMenuActive)
            {
                GameMenus.PauseMenu.Update(gameTime);

                if (!GameMenus.PauseMenu.IsMenuActive)
                {
                    //HACK: leaving current state, reset input manager of new state
                    foreach (var grid in Grids)
                        foreach (var player in grid.Players)
                            player.inputManager.Update(player.playerIndex);
                }
                return;
            }

            //Gamestate: Playing game
            inputManager.Update(PlayerIndex.One);
            if (inputManager.IsActionTriggered(GlobalCommands.Menu))
            {
                //HACK: leaving current state, reset input manager of new state
                GameMenus.PauseMenu.ShowMenu();
                GameMenus.PauseMenu.ResetInputs();
                return;
            }

            foreach (var grid in Grids)
                grid.Update(gameTime);

            Toasts.Update(gameTime);
        }


        Timer toggleTimer = new Timer(9000);
        Timer toggle2Timer = new Timer(1000);
        bool toggle = false;

#if SHOWSTUFF
        Timer fpsTimer = new Timer(100);
        double framerate = 0;
        Timer logicTimer = new Timer(100);
        double logic = 0;

        Vector2 mousePos;
#endif

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

            //Gamestate: Intro
            if (Intro.IsActive)
            {
                Intro.Draw(spriteBatch, fontTitleHuge);

                spriteBatch.End();
                return;
            }

            if (ShowHighScores)
            {
                spriteBatch.DrawString(fontTitleHuge, "LIZTRIS", new Vector2(30, 250), Color.Black,
                    -0.5f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(fontTitleHuge, "LIZTRIS", new Vector2(28, 248), Color.Wheat,
                    -0.5f, Vector2.Zero, 1f, SpriteEffects.None, 0f);


                var ScoreRect = new Rectangle(
                    (GamePixelWidth / 8) * 3,
                    GamePixelHeight / 4,
                    GamePixelWidth / 4,
                    GamePixelHeight / 2);

                

                spriteBatch.DrawRectangle(ScoreRect, Color.Teal, 3, false);

                spriteBatch.Draw(transparentDarkTexture, ScoreRect, Color.White * 0.5f);

                var r = new Rectangle(ScoreRect.Left, ScoreRect.Top + 5, ScoreRect.Width, 50);
                spriteBatch.DrawString(fontTitle, "HIGH SCORES", r, ExtendedSpriteBatch.Alignment.Center, Color.Black);
                r.Offset(2, 2);
                spriteBatch.DrawString(fontTitle, "HIGH SCORES", r, ExtendedSpriteBatch.Alignment.Center, Color.Wheat);
                r.Offset(-2, -2);
                r.Offset(0, 40);

                foreach (var score in Program.Settings.Game.HighScores)
                {
                    spriteBatch.DrawString(fontScore, score.ToString("N0"),
                        r, ExtendedSpriteBatch.Alignment.Center, Color.Wheat);
                    r.Offset(0, 30);
                }

                spriteBatch.End();
                return;
            }

            //Gamestate: MainMenu / PauseMenu
            if (GameMenus.MainMenu.IsMenuActive || GameMenus.PauseMenu.IsMenuActive)
            {
                spriteBatch.DrawString(fontTitleHuge, "LIZTRIS", new Vector2(30, 250), Color.Black,
                    -0.5f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                spriteBatch.DrawString(fontTitleHuge, "LIZTRIS", new Vector2(28, 248), Color.Wheat,
                    -0.5f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

                var MenuRect = new Rectangle(
                    (GamePixelWidth / 8) * 3, 
                    GamePixelHeight / 4, 
                    GamePixelWidth / 4, 
                    GamePixelHeight / 2);

                spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

                spriteBatch.Draw(transparentDarkTexture, MenuRect, Color.White * 0.5f);

                if (GameMenus.MainMenu.IsMenuActive)
                    GameMenus.MainMenu.Draw(spriteBatch, fontMenu, MenuRect, true);
                else  if (GameMenus.PauseMenu.IsMenuActive)
                    GameMenus.PauseMenu.Draw(spriteBatch, fontMenu, MenuRect, true);

#if SHOWSTUFF
                spriteBatch.DrawString(fontMenu, "Mouse: " + 
                    (int)mousePos.X + " " + (int)mousePos.Y, new Vector2(), Color.White);

                if (fpsTimer.UpdateAndCheck(gameTime))
                    framerate = 1 / gameTime.ElapsedGameTime.TotalSeconds;

                spriteBatch.DrawString(fontMenu, "FPS: " +
                    framerate.ToString("N2"), new Vector2(0, 60), Color.White);
                spriteBatch.DrawString(fontMenu, "LPS: " +
                    logic.ToString("N2"), new Vector2(0, 120), Color.White);
#endif

                spriteBatch.End();
                return;
            }

            //Gamestate: Playing game

            //draw
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(30, 2), Color.Black);
            spriteBatch.DrawString(fontTitle, "LIZTRIS", new Vector2(28, 0), Color.Wheat);

            foreach (var grid in Grids)
            {
                //draw grid info on upper left of grid
                if (!toggle ||
                    ((grid.LineCount == grid.BestLineCount) && 
                     (grid.Score == grid.BestScore)))
                {
                    spriteBatch.DrawString(fontScore, "Level: " + grid.Level.ToString(),
                        new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 140), Color.Gold);
                    spriteBatch.DrawString(fontScore, "Lines: " + grid.LineCount.ToString(),
                        new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 100), Color.Gold);
                    spriteBatch.DrawString(fontScore, "Pts: " + grid.Score.ToString(),
                        new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 60), Color.Gold);
                }
                else
                {
                    spriteBatch.DrawString(fontScore, "Best:",
                        new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 140), Color.Gold);
                    spriteBatch.DrawString(fontScore, "Lines: " + grid.BestLineCount.ToString(),
                        new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 100), Color.Gold);
                    spriteBatch.DrawString(fontScore, "Pts: " + grid.BestScore.ToString(),
                        new Vector2(grid.ScreenRect.X, grid.ScreenRect.Y - 60), Color.Gold);
                }

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
