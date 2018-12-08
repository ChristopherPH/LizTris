using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace GameStateEngine
{
    public abstract class GameEngine : Game
    {
        protected abstract int GameResolutionWidth { get; }
        protected abstract int GameResolutionHeight { get; }

        protected virtual string SettingsFile { get; } = "settings.xml";
        protected GameEngineSettings GameEngineSettings { get; }
        protected virtual bool StartEndSpriteBatchInDraw { get; } = true;
        protected Rectangle GameRectangle { get; private set; }

        GraphicsDeviceManager graphics;
        ExtendedSpriteBatch spriteBatch;
        private Stack<GameState> gameStates = new Stack<GameState>();

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

        protected void SetResolution(int Width, int Height, VideoSettings.WindowModeTypes WindowMode, bool VSync)
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
                Exit();
                return;
            }

            var CurrentState = gameStates.Peek();

            //save state time, run state update logic
            CurrentState.stateTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            var rc = CurrentState.Update(gameTime);

            switch (rc)
            {
                default:
                case GameState.StateOperation.StateRunning:
                    break;

                case GameState.StateOperation.StateCompleted:
                    //complete the state and remove it
                    CurrentState.OnCompleted();
                    gameStates.Pop();

                    //if the completed state has another state to move to, add it
                    var NextState = CurrentState.NextState;
                    if (NextState != null)
                    {
                        NextState.SetServiceProvider(this.Services);
                        gameStates.Push(NextState);
                        NextState.OnInit();
                    }
                    else if (gameStates.Count > 0) //go back to prev state
                    {
                        gameStates.Peek().OnResume();
                    }

                    CurrentState.Dispose();
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (gameStates.Count > 0)
            {
                IndependentResolutionRendering.Resolution.BeginDraw();

                if (StartEndSpriteBatchInDraw)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        null, null, null, null, DrawMatrix);

                    spriteBatch.FillRectangle(GameRectangle, Color.CornflowerBlue);
                }

                gameStates.Peek().Draw(gameTime, spriteBatch);

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

            var invert = Matrix.Invert(
                IndependentResolutionRendering.Resolution.getTransformationMatrix());

            return Vector2.Transform(Location - vp, invert);
        }

        public void AddState(GameState newState)
        {
            if (newState != null)
            {
                newState.SetServiceProvider(this.Services);

                if (gameStates.Count > 0)
                    gameStates.Peek().OnPause();

                gameStates.Push(newState);
                newState.OnInit();
            }
        }
    }
}
