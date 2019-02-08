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
using Microsoft.Xna.Framework.Media;

namespace Liztris
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Liztris : GameResolution
    {
        GraphicsDeviceManager graphics;
        ExtendedSpriteBatch spriteBatch;
        Texture2D[] Background;
        SpriteSheet Blocks;
        SpriteSheet Patterns;
        SpriteFont fontScore, fontScoreBig, fontTitle, fontMenu, fontTitleHuge;
        SoundEffect soundLine, soundLevel, soundLose, soundDrop, soundTetris, soundMenuSelect;
        SoundEffect musicDefault;
        SoundEffectInstance musicDefaultInstance;
        Song musicMP3;
        int BackgroundIndex = 0;
        bool ShowHighScores = false;

        enum GlobalCommands
        {
            Menu,
            ExitHighScore
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
        public int[] LevelPattern =
                      { 0,     6,   19,    7,    4,   22,   16,  35,  12,  18,   19,  11,  28,  31,   4 };
        public Color[] LevelTint =
                      { Color.Teal, Color.Green, Color.DarkMagenta, Color.Olive, Color.Crimson, Color.DarkGoldenrod,
                        Color.LightSkyBlue,  Color.Gray,  Color.Plum,  Color.Coral,
                        Color.CornflowerBlue, Color.CadetBlue, Color.DarkSalmon, Color.Olive, Color.Red };

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

            GameMenus.MainMenu.SetPropertyValue("WindowMode", Program.Settings.Video.WindowMode);
            GameMenus.MainMenu.SetPropertyValue("VSync", Program.Settings.Video.VSync);

            GameMenus.MainMenu.SetPropertyValue("Resolution",
                string.Format("{0}x{1}", Program.Settings.Video.Width, Program.Settings.Video.Height));

            GameMenus.MainMenu.SetPropertyValue("MasterVolume", Program.Settings.Audio.MasterVolume);
            GameMenus.MainMenu.SetPropertyValue("MusicVolume", Program.Settings.Audio.MusicVolume);
            GameMenus.MainMenu.SetPropertyValue("UseMP3", Program.Settings.Audio.UseMP3);

            //ensure default profiles are created
            foreach (var s in new string[] { "Liz", "Chris", "Gwen", "Guest" })
                if (!Program.Settings.Game.Profiles.Where(x => x.Name == s).Any())
                    Program.Settings.Game.Profiles.Add(new Profile() { Name = s });
        }

        private void MediaPlayer_MediaStateChanged(object sender, EventArgs e)
        {
            if ((MediaPlayer.State == MediaState.Stopped) &&
                (Program.Settings.Audio.UseMP3) &&
                (Program.Settings.Audio.MusicVolume > 0))
            {
                MediaPlayer.Play(musicMP3);
            }
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

            this.IsFixedTimeStep = Program.Settings.Video.VSync;
            graphics.SynchronizeWithVerticalRetrace = Program.Settings.Video.VSync;

            SetupGameResolution(ref graphics);

            switch (Program.Settings.Video.WindowMode)
            {
                case VideoSettings.WindowModeTypes.Windowed:
                    Window.IsBorderless = false;

                    var x = (GraphicsDevice.DisplayMode.Width - WindowWidth) / 2;
                    var y = (GraphicsDevice.DisplayMode.Height - WindowHeight) / 2;
                    Window.Position = new Point(x, y);
                    break;

                case VideoSettings.WindowModeTypes.Fullscreen:
                    Window.IsBorderless = false;
                    break;


                case VideoSettings.WindowModeTypes.WindowedFullscreen:
                    Window.IsBorderless = true;
                    Window.Position = new Point(0, 0);
                    IndependentResolutionRendering.Resolution.SetResolution(
                        GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height, false);
                    break;
            }

            inputManager.AddAction(GlobalCommands.Menu, Keys.Escape);
            inputManager.AddAction(GlobalCommands.Menu, InputManager<GlobalCommands>.GamePadButtons.Start);
            inputManager.AddAction(GlobalCommands.Menu, InputManager<GlobalCommands>.GamePadButtons.Back);
            inputManager.AddAction(GlobalCommands.ExitHighScore, InputManager<GlobalCommands>.GamePadButtons.A);
            inputManager.AddAction(GlobalCommands.ExitHighScore, InputManager<GlobalCommands>.GamePadButtons.B);
            inputManager.AddAction(GlobalCommands.ExitHighScore, InputManager<GlobalCommands>.GamePadButtons.X);
            inputManager.AddAction(GlobalCommands.ExitHighScore, InputManager<GlobalCommands>.GamePadButtons.Y);

            musicDefaultInstance.Volume = (float)Program.Settings.Audio.MusicVolume / 100;
            musicDefaultInstance.IsLooped = true;
            MediaPlayer.Volume = (float)Program.Settings.Audio.MusicVolume / 100;
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

            if (Program.Settings.Audio.MusicVolume > 0)
            {
                if (Program.Settings.Audio.UseMP3)
                    MediaPlayer.Play(musicMP3);
                else
                    musicDefaultInstance.Play();
            }

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
                        var sharedgrid = (bool)GameMenus.MainMenu.Options["SharedGrid"];

                        int[] speeds;
                        if (sharedgrid)
                        {
                            speeds = new int[1];
                            speeds[0] = (int)GameMenus.MainMenu.Options["Speed0"];
                        }
                        else
                        {
                            speeds = new int[players];
                            for (int i = 0; i < players; i++)
                                speeds[i] = (int)GameMenus.MainMenu.Options["Speed" + i.ToString()];
                        }

                        string[] names = new string[players];
                        for (int i = 0; i < players; i++)
                            names[i] = (string)GameMenus.MainMenu.Options["Profile" + i.ToString()];

                        NewGame(players, names, speeds, sharedgrid);
                        GameMenus.MainMenu.ExitMenu();
                        break;

                    case GameMenus.GameMenuOptions.ApplyGraphics:
                        AdjustVideoSettings();
                        break;

                    case GameMenus.GameMenuOptions.ChangeAudio:
                        //set up audio parameters
                        //var sound = (bool)GameMenus.MainMenu.Options["Sound"];
                        Program.Settings.Audio.MusicVolume = (int)GameMenus.MainMenu.Options["MusicVolume"];
                        Program.Settings.Audio.MasterVolume = (int)GameMenus.MainMenu.Options["MasterVolume"];
                        Program.Settings.Audio.UseMP3 = (bool)GameMenus.MainMenu.Options["UseMP3"];

                        AdjustAudioSettings();
                        break;

                    case GameMenus.GameMenuOptions.ShowScores:
                        ShowHighScores = true;
                        break;
                }
            };

            GameMenus.MainMenu.ResetMenu();

            GameMenus.PauseMenu.ActionHandler = (object Action) =>
            {
                switch ((GameMenus.GameMenuOptions)Action)
                {
                    case GameMenus.GameMenuOptions.QuitGame:
                        //HACK: leaving current state, reset input manager of new state
                        GameMenus.MainMenu.ResetMenu();
                        GameMenus.MainMenu.ResetInputs();
                        break;

                    case GameMenus.GameMenuOptions.ChangeAudio:
                        //set up audio parameters
                        //var sound = (bool)GameMenus.PauseMenu.Options["Sound"];
                        Program.Settings.Audio.MusicVolume = (int)GameMenus.PauseMenu.Options["MusicVolume"];
                        Program.Settings.Audio.MasterVolume = (int)GameMenus.PauseMenu.Options["MasterVolume"];
                        Program.Settings.Audio.UseMP3 = (bool)GameMenus.PauseMenu.Options["UseMP3"];

                        AdjustAudioSettings();

                        //HACK: fix up audio settings on main menu
                        GameMenus.MainMenu.SetPropertyValue("MasterVolume", Program.Settings.Audio.MasterVolume);
                        GameMenus.MainMenu.SetPropertyValue("MusicVolume", Program.Settings.Audio.MusicVolume);
                        GameMenus.MainMenu.SetPropertyValue("UseMP3", Program.Settings.Audio.UseMP3);
                        break;
                }
            };

            GameMenus.MainMenu.MenuChanged = () => { soundMenuSelect.Play(); };
            GameMenus.PauseMenu.MenuChanged = () => { soundMenuSelect.Play(); };

            Intro.IsActive = true;
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
            Texture2D tex = Content.Load<Texture2D>("Graphics/Blocks");
            Blocks = new SpriteSheet(tex, 5, 4);

            tex = Content.Load<Texture2D>("Graphics/Patterns");
            Patterns = new SpriteSheet(tex, 6, 6);

            Background = new Texture2D[3];
            Background[0] = Content.Load<Texture2D>("Backgrounds/Beach"); 
            Background[1] = Content.Load<Texture2D>("Backgrounds/Ocean"); 
            Background[2] = Content.Load<Texture2D>("Backgrounds/Ruins");
            BackgroundIndex = 0;

            fontScore = Content.Load<SpriteFont>("Fonts/Score");
            fontScoreBig = Content.Load<SpriteFont>("Fonts/ScoreBig");
            fontTitle = Content.Load<SpriteFont>("Fonts/Title");
            fontMenu = Content.Load<SpriteFont>("Fonts/Menu");
            fontTitleHuge = Content.Load<SpriteFont>("Fonts/TitleHuge");

            Toasts.spriteFont = Content.Load<SpriteFont>("Fonts/Toast");

            musicDefault = Content.Load<SoundEffect>("Music/Music");
            musicMP3 = Content.Load<Song>("Music/musicMP3");

            soundLine = Content.Load<SoundEffect>("Sounds/Line");
            soundLevel = Content.Load<SoundEffect>("Sounds/Level");
            soundLose = Content.Load<SoundEffect>("Sounds/Lose");
            soundDrop = Content.Load<SoundEffect>("Sounds/Drop");
            soundTetris = Content.Load<SoundEffect>("Sounds/Tetris");

            soundMenuSelect = Content.Load<SoundEffect>("Sounds/MenuSelect");

            musicDefaultInstance = musicDefault.CreateInstance();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
            // TODO: Unload any non ContentManager content here
        }


        public void NewGame(int PlayerCount, string[] Names, int[] SpeedTypes, bool SharedGrid)
        {
            foreach (var grid in Grids)
                grid.Players.Clear();
            Grids.Clear();

            if (SharedGrid)
            {
                var grid = new Grid(GridXPerPlayer * PlayerCount, GridY, LevelSpeeds[SpeedTypes[0]], 
                    LevelLines, ScoreMultiplier, LevelPattern, LevelTint);
                Grids.Add(grid);

                for (int i = 0; i < PlayerCount; i++)
                {
                    var player = new Player(grid, (PlayerIndex)i, Names[i], null);
                    grid.Players.Add(player);
                }
            }
            else
            {
                for (int i = 0; i < PlayerCount; i++)
                {
                    var grid = new Grid(GridXPerPlayer, GridY, LevelSpeeds[SpeedTypes[i]], LevelLines, 
                        ScoreMultiplier, LevelPattern, LevelTint);
                    Grids.Add(grid);

                    var player = new Player(grid, (PlayerIndex)i, Names[i], null);
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
                if (inputManager.IsActionTriggered(GlobalCommands.ExitHighScore) ||
                    inputManager.IsActionTriggered(GlobalCommands.Menu))
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
                //HACK: Set up audio levels in different menu
                GameMenus.PauseMenu.SetPropertyValue("MasterVolume", Program.Settings.Audio.MasterVolume);
                GameMenus.PauseMenu.SetPropertyValue("MusicVolume", Program.Settings.Audio.MusicVolume);
                GameMenus.PauseMenu.SetPropertyValue("UseMP3", Program.Settings.Audio.UseMP3);

                //HACK: leaving current state, reset input manager of new state
                GameMenus.PauseMenu.ResetMenu();
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

            spriteBatch.Draw(Background[BackgroundIndex], GameRectangle);

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


                var scoreWidth = GamePixelWidth * 0.5;
                var scoreHeight = GamePixelHeight * 0.75;

                var ScoreRect = new Rectangle(
                    (GamePixelWidth / 2) - (int)(scoreWidth / 2),
                    (GamePixelHeight / 2) - (int)(scoreHeight / 2),
                    (int)scoreWidth, (int)scoreHeight);

                spriteBatch.DrawRectangle(ScoreRect, Color.Teal, 3, false);

                spriteBatch.FillRectangle(ScoreRect, Color.Teal, 0.25f);

                var r = new Rectangle(ScoreRect.Left, ScoreRect.Top + 5, ScoreRect.Width, 50);
                spriteBatch.DrawString(fontTitle, "HIGH SCORES", r, ExtendedSpriteBatch.Alignment.Center, Color.Black);
                r.Offset(2, 2);
                spriteBatch.DrawString(fontTitle, "HIGH SCORES", r, ExtendedSpriteBatch.Alignment.Center, Color.Wheat);
                r.Offset(-2, -2);
                r.Offset(0, 60);

                foreach (var hs in Program.Settings.Game.HighScores)
                {
                    spriteBatch.DrawString(fontScore, 
                        hs.Name + ": " + hs.Score.ToString("N0") + " and " + hs.Lines + " Lines",
                        r, ExtendedSpriteBatch.Alignment.Center, Color.Wheat);
                    r.Offset(0, 50);
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

                var winWidth = GamePixelWidth * 0.34;
                var winHeight = GamePixelHeight * 0.5;

                var MenuRect = new Rectangle(
                    (GamePixelWidth / 2) - (int)(winWidth / 2),
                    (GamePixelHeight / 2) - (int)(winHeight / 2),
                    (int)winWidth, (int)winHeight);

                spriteBatch.DrawRectangle(MenuRect, Color.Teal, 3, false);

                spriteBatch.FillRectangle(MenuRect, Color.Teal, 0.25f);

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


            //draw scores
            if ((Grids.Count <= 2) && (Grids[0].Players.Count <= 2))
            {
                //spriteBatch.FillRectangle(new Rectangle(5, 5, 405, 100), Color.Teal * 0.25f);
                spriteBatch.DrawString(fontTitleHuge, "LIZTRIS", new Vector2(3, -20), Color.Black);
                spriteBatch.DrawString(fontTitleHuge, "LIZTRIS", new Vector2(0, -23), Color.Wheat);

                foreach (var grid in Grids)
                {
                    var sX = grid.ScreenRect.X - 300;
                    if (Grids.IndexOf(grid) == 1)
                        sX = grid.ScreenRect.Right + 25;

                    var sY = grid.ScreenRect.Y - 15;
                    var sC = Color.Black;

                    spriteBatch.DrawString(fontScoreBig, grid.PlayerNames, new Vector2(sX, sY), sC);
                    spriteBatch.DrawString(fontScoreBig, "Level: " + grid.Level, new Vector2(sX, sY + 100), sC);
                    spriteBatch.DrawString(fontScoreBig, "Lines: " + grid.LineCount, new Vector2(sX, sY + 175), sC);
                    spriteBatch.DrawString(fontScoreBig, grid.Score.ToString("N0"), new Vector2(sX, sY + 250), sC);
                    spriteBatch.DrawString(fontScoreBig, "High Score", new Vector2(sX, sY + 350), sC);
                    spriteBatch.DrawString(fontScoreBig, "Lines: " + grid.BestLineCount, new Vector2(sX, sY + 425), sC);
                    spriteBatch.DrawString(fontScoreBig, grid.BestScore.ToString("N0"), new Vector2(sX, sY + 500), sC);

                    sX -= 3;
                    sY -= 3;
                    sC = Color.White; 
                    spriteBatch.DrawString(fontScoreBig, grid.PlayerNames, new Vector2(sX, sY), sC);

                    sC = Color.Cyan;
                    spriteBatch.DrawString(fontScoreBig, "Level: " + grid.Level, new Vector2(sX, sY + 100), sC);
                    sC = Color.LimeGreen;
                    spriteBatch.DrawString(fontScoreBig, "Lines: " + grid.LineCount, new Vector2(sX, sY + 175), sC);
                    spriteBatch.DrawString(fontScoreBig, grid.Score.ToString("N0"), new Vector2(sX, sY + 250), sC);

                    sC = Color.White;
                    spriteBatch.DrawString(fontScoreBig, "High Score", new Vector2(sX, sY + 350), sC);
                    sC = Color.Yellow;
                    spriteBatch.DrawString(fontScoreBig, "Lines: " + grid.BestLineCount, new Vector2(sX, sY + 425), sC);
                    spriteBatch.DrawString(fontScoreBig, grid.BestScore.ToString("N0"), new Vector2(sX, sY + 500), sC);
                }
            }
            else
            {
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

                    spriteBatch.DrawString(fontScore, grid.PlayerNames,
                        new Rectangle(grid.ScreenRect.Left, grid.ScreenRect.Bottom - 7,
                        grid.ScreenRect.Width, 50), ExtendedSpriteBatch.Alignment.Center, Color.Gold);
                }
            }


            //draw grids

            //this line stops bleed from the neighboring patterns on the right and bottom
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var grid in Grids)
                grid.Draw(spriteBatch, Blocks, Patterns, BlockPixelSize);

            Toasts.Draw(spriteBatch);

            spriteBatch.End();
        }

        void AdjustVideoSettings()
        {
            var resString = (string)GameMenus.MainMenu.Options["Resolution"];
            if (string.IsNullOrWhiteSpace(resString))
            {
                System.Diagnostics.Debug.Print("Invalid Resolution");
                return;
            }

            var resStrings = resString.Split('x');
            if ((resStrings == null) || (resStrings.Length != 2))
            {
                System.Diagnostics.Debug.Print("Invalid Resolution {0}", resString);
                return;
            }

            if ((int.TryParse(resStrings[0], out int tmpWidth) == false) || (tmpWidth <= 0))
            {
                System.Diagnostics.Debug.Print("Invalid Resolution Width {0}", resStrings[0]);
                return;
            }

            if ((int.TryParse(resStrings[1], out int tmpHeight) == false) || (tmpHeight <= 0))
            {
                System.Diagnostics.Debug.Print("Invalid Resolution Height {0}", resStrings[1]);
                return;
            }

            Program.Settings.Video.WindowMode = (VideoSettings.WindowModeTypes)GameMenus.MainMenu.Options["WindowMode"];
            Program.Settings.Video.VSync = (bool)GameMenus.MainMenu.Options["VSync"];
            Program.Settings.Video.Width = tmpWidth;
            Program.Settings.Video.Height = tmpHeight;


            this.IsFixedTimeStep = Program.Settings.Video.VSync;
            graphics.SynchronizeWithVerticalRetrace = Program.Settings.Video.VSync;

            switch (Program.Settings.Video.WindowMode)
            {
                case VideoSettings.WindowModeTypes.Windowed:
                    Window.IsBorderless = false;

                    var x = (GraphicsDevice.DisplayMode.Width - WindowWidth) / 2;
                    var y = (GraphicsDevice.DisplayMode.Height - WindowHeight) / 2;
                    Window.Position = new Point(x, y);

                    IndependentResolutionRendering.Resolution.SetResolution(
                        Program.Settings.Video.Width, Program.Settings.Video.Height,
                        false);
                    break;

                case VideoSettings.WindowModeTypes.Fullscreen:
                    Window.IsBorderless = false;

                    IndependentResolutionRendering.Resolution.SetResolution(
                        Program.Settings.Video.Width, Program.Settings.Video.Height, true);
                    break;

                case VideoSettings.WindowModeTypes.WindowedFullscreen:
                    Window.IsBorderless = true;
                    Window.Position = new Point(0, 0);
                    IndependentResolutionRendering.Resolution.SetResolution(
                        GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height, false);
                    break;
            }

            graphics.ApplyChanges();
            Program.Settings.SaveSettings();
        }

        void AdjustAudioSettings()
        {
            SoundEffect.MasterVolume = (float)Program.Settings.Audio.MasterVolume / 100;
            musicDefaultInstance.Volume = (float)Program.Settings.Audio.MusicVolume / 100;
            MediaPlayer.Volume = (float)Program.Settings.Audio.MusicVolume / 100;

            if (Program.Settings.Audio.MusicVolume > 0)
            {
                //pause and play to kick the volume change
                if (musicDefaultInstance.State == SoundState.Playing)
                    musicDefaultInstance.Pause();

                if (Program.Settings.Audio.UseMP3)
                {
                    //play mp3 if not playing already, other music is already paused or stopped
                    if (MediaPlayer.State != MediaState.Playing)
                        MediaPlayer.Play(musicMP3);
                }
                else
                {
                    //play other music, stop the mp3 if playing
                    if (MediaPlayer.State == MediaState.Playing)
                        MediaPlayer.Stop();

                    musicDefaultInstance.Play();
                }
            }
            else //no volume
            {
                //stop all music
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Stop();

                if (musicDefaultInstance.State == SoundState.Playing)
                    musicDefaultInstance.Stop();
            }

            Program.Settings.SaveSettings();
        }
    }
}
