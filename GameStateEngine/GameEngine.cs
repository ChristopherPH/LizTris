using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace GameStateEngine
{
    /// <summary>
    /// Common XNA/Monogame game, with:
    /// - Game State Manager
    /// - Independant Resolutions
    /// - Saved / Loaded Video settings
    ///     - Resolution
    ///     - Windowed / Windowed Fullscreen / Fullscreen
    /// - Saved / Loaded Audio settings
    ///     - Music level (in 10% increments), On/Off
    ///     - Sound level (in 10% increments), On/Off
    /// - Extensible singleton / global shared data between states
    /// - FPS Display
    /// 
    /// Thoughts:
    /// - Input Manager / remapper
    /// - Mouse / Gamepad / keyboard support
    /// </summary>
    public abstract class GameEngine : Game
    {
        protected abstract int GameResolutionWidth { get; }
        protected abstract int GameResolutionHeight { get; }
        protected abstract GameState InitialGameState { get; }
        protected abstract string DefaultSpriteFontContentPath { get; }

        protected virtual string SettingsFile { get; } = "settings.xml";
        protected GameEngineSettings GameEngineSettings { get; } 
        protected virtual bool StartEndSpriteBatchInDraw { get; } = true;
        protected Rectangle GameRectangle { get; private set; }
        protected SpriteFont DefaultFont { get; private set; }

        GraphicsDeviceManager graphics;
        protected ExtendedSpriteBatch spriteBatch;
        private Stack<GameState> gameStates = new Stack<GameState>();
        private int gameRenderStates = 0;

        Timer fpsTimer = new Timer(500);
        double framerate = 0;

        public GameEngine()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            GameEngineSettings = GameEngineSettings.LoadSettings(SettingsFile);

            IndependentResolutionRendering.Resolution.Init(ref graphics);

            //Virtual Resolution (Game Resolution): 
            //  Amount of pixels for Draw() to draw on. This will be scaled to the Window resolution for display.
            //  Note: VirtualResolution doesn't need to be a valid resolution
            IndependentResolutionRendering.Resolution.SetVirtualResolution(GameResolutionWidth, GameResolutionHeight);

            GameRectangle = new Rectangle(0, 0, GameResolutionWidth, GameResolutionHeight);

            SetResolution(GameEngineSettings.Video.Width,
                GameEngineSettings.Video.Height,
                GameEngineSettings.Video.WindowMode,
                GameEngineSettings.Video.VSync);

            InitialGameState.SetServiceProvider(this.Services);
            gameStates.Push(InitialGameState);
            gameRenderStates = 1;
            InitialGameState.OnStateStarted(false);
        }

        protected void SaveResolution(int Width, int Height, VideoSettings.WindowModeTypes windowMode, bool VSync)
        {
            GameEngineSettings.Video.Width = Width;
            GameEngineSettings.Video.Height = Height;
            GameEngineSettings.Video.WindowMode = windowMode;
            GameEngineSettings.Video.VSync = VSync;

            GameEngineSettings.SaveSettings(SettingsFile);

            SetResolution(Width, Height, windowMode, VSync);
        }

        public void SetResolution(int Width, int Height, VideoSettings.WindowModeTypes WindowMode, bool VSync)
        {
            this.IsFixedTimeStep = VSync;
            graphics.SynchronizeWithVerticalRetrace = VSync;

            switch (WindowMode)
            {
                case VideoSettings.WindowModeTypes.Windowed:
                    Window.IsBorderless = false;

                    var x = (GraphicsDevice.DisplayMode.Width - Width) / 2;
                    var y = (GraphicsDevice.DisplayMode.Height - Height) / 2;
                    Window.Position = new Point(x, y);

                    IndependentResolutionRendering.Resolution.SetResolution(Width, Height, false);
                    break;


                case VideoSettings.WindowModeTypes.Fullscreen:
                    Window.IsBorderless = false;
                    IndependentResolutionRendering.Resolution.SetResolution(Width, Height, true);
                    break;


                case VideoSettings.WindowModeTypes.WindowedFullscreen:
                    Window.IsBorderless = true;
                    Window.Position = new Point(0, 0);
                    IndependentResolutionRendering.Resolution.SetResolution(
                        GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height, false);
                    break;
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
            DefaultFont = Content.Load<SpriteFont>(DefaultSpriteFontContentPath);

            foreach (var state in gameStates)
                state.SetServiceProvider(Services);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameStates.Count == 0)
            {
                gameRenderStates = 0;
                Exit();
                return;
            }

            var CurrentState = gameStates.Peek();
            GameStateOperation Operation = null;

            CurrentState.Update(gameTime, ref Operation);

            //save state time, run state update logic
            CurrentState.stateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Operation != null)
            {
                if (Operation.CompleteCurrentState)
                {
                    //complete the state and remove it
                    CurrentState.OnStateStopped(false);
                    gameStates.Pop();
                    CurrentState.Dispose();

                    //go back to prev state if not moving to next state
                    if ((Operation.StateToPush == null) && (gameStates.Count > 0))
                        gameStates.Peek().OnStateStarted(true);
                }
                else if (Operation.StateToPush != null)
                {
                    //state is still running and we are moving to next state, pause current state
                    CurrentState.OnStateStopped(true);
                }

                //move to next state
                if (Operation.StateToPush != null)
                {
                    gameStates.Push(Operation.StateToPush);
                    Operation.StateToPush.SetServiceProvider(this.Services);
                    Operation.StateToPush.OnStateStarted(false);
                }

                //our states have changed, determine how many states to render
                gameRenderStates = 0;

                foreach (var state in gameStates)
                {
                    gameRenderStates++;
                    if (!state.RenderPreviousState)
                        break;
                }
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (fpsTimer.UpdateAndCheck(gameTime))
                framerate = 1 / gameTime.ElapsedGameTime.TotalSeconds;
            
            if (gameRenderStates > 0)
            {
                IndependentResolutionRendering.Resolution.BeginDraw();

                if (StartEndSpriteBatchInDraw)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        null, null, null, null, DrawMatrix);

                    spriteBatch.FillRectangle(GameRectangle, Color.CornflowerBlue);

                    spriteBatch.DrawString(DefaultFont,
                        //string.Format("{0}x{1}", GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height),
                        //string.Format("{0}x{1} {2}", graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, graphics.IsFullScreen),
                        //string.Format("FPS: {0:N0}", framerate),
                        string.Format("{0}x{1} {2} FPS:{3:N5}", graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, graphics.IsFullScreen, framerate),
                        GameRectangle,
                        ExtendedSpriteBatch.Alignment.Top | ExtendedSpriteBatch.Alignment.Right,
                        Color.Black);
                }

                //draw states back to front
                foreach (var state in gameStates.Take(gameRenderStates).Reverse())
                {
                    state.Draw(gameTime, spriteBatch, GameRectangle);
                }

                if (StartEndSpriteBatchInDraw)
                    spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        protected Matrix DrawMatrix => IndependentResolutionRendering.Resolution.getTransformationMatrix();


        protected Vector2 TranslateWindowToGame(Vector2 Location)
        {
            var vp = new Vector2(
                IndependentResolutionRendering.Resolution.ViewportX, 
                IndependentResolutionRendering.Resolution.ViewportY);

            return Vector2.Transform(Location - vp, Matrix.Invert(DrawMatrix));
        }
    }
}
