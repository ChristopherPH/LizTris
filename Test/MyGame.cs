using Common;
using Common.MenuSystem;
using Common.Misc;
using Common.Interpolator;
using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using Test.ControllerManager;

namespace Test
{
    public class MyGame : GameEngine
    {
        protected override int GameResolutionWidth => 1280;
        protected override int GameResolutionHeight => 720;
        protected override string DefaultSpriteFontContentPath => "Font";
        protected override GameState InitialGameState => new IntroState();
    }

    public class IntroState : GameState
    {
        InputManager<int> inputManager = new InputManager<int>();
        Timer introTimer = new Timer(StateDuration + StatePause);
        SpriteFont _font;

        ColorInterpolator ci;
        Vector2Interpolator vi;
        FloatInterpolator fi;
        CurveInterpolator curi;
        InterpolatorCollection Interpolators;

        const int StateDuration = 5000;
        const int StatePause = 1000;
        const int StateSlice = 5000 / 3;

        public IntroState()
        {
            inputManager.AddAction(0, Keys.Escape);
            inputManager.AddAction(0, InputManager<int>.GamePadButtons.Start);

            ci = new ColorInterpolator(Color.Transparent, Color.Wheat, StateDuration);
            fi = new FloatInterpolator(2.5f, -0.5f, StateDuration);
            vi = new Vector2Interpolator(new Vector2(900, -150), new Vector2(28, 248), StateDuration);

            curi = new CurveInterpolator(new Vector2(100, 300), new Vector2(500, 300), StateDuration);
            curi.AddPoint(new Vector2(200, 000), StateSlice);
            curi.AddPoint(new Vector2(400, 600), StateSlice + StateSlice);

            Interpolators = new InterpolatorCollection(
                new IInterpolator[] { ci, fi, vi, curi });
        }


        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
            var textcolor = ci.Interpolate();
            var textshadow = Color.FromNonPremultiplied(Color.Black.R, Color.Black.G, Color.Black.B, textcolor.A);
            var pos = vi.Interpolate();
            var rot = fi.Interpolate();

            var ci_val = ci.Interpolate();

            spriteBatch.DrawString(_font, "LIZTRIS", pos + new Vector2(102, 2), textshadow,
                rot, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "LIZTRIS", pos + new Vector2(100, 0), textcolor,
                rot, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            var curvepos = curi.Interpolate();
            //spriteBatch.DrawEquilateralTriangle(curvepos, 20, Color.Black, -30, 5);
            // spriteBatch.DrawString(_font, string.Format("{0} {1}", (int)curvepos.X, (int)curvepos.Y), 
            //    curvepos + new Vector2(40, -10), 
            //    Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            spriteBatch.DrawString(_font, "LizTris", curvepos + new Vector2(2, 2), textshadow, rot, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "LizTris", curvepos, Color.Wheat, rot, Vector2.Zero, 1f, SpriteEffects.None, 0f);

#if FOO
            spriteBatch.DrawString(_font, "XXXXXXXXXXX", new Vector2(0, 400), Color.Green,
                    0.0f, Vector2.Zero, 5f, SpriteEffects.None, 0f);

            //no premult is good
            var c_start = new Color(Color.Red, 63);
            var c_end = new Color(Color.Blue, 191);

            //pre lerp is good
            //var c_start = new Color(Color.Red, 191);
            //var c_end = new Color(Color.Blue, 191);

            //pre lerp is good
            //var c_start = Color.Transparent;
            //var c_end = Color.Wheat;

            DrawFade(spriteBatch, 25, 400, 100, "No Premultiply", c_start, c_end, false);
            DrawFade(spriteBatch, 25, 510, 100, "Premultiplied (pre lerp)", 
                Color.FromNonPremultiplied(c_start.ToVector4()), 
                Color.FromNonPremultiplied(c_end.ToVector4()), false);
            DrawFade(spriteBatch, 25, 620, 100, "Premultiplied (post lerp)", c_start, c_end, true);



            /*
            spriteBatch.DrawString(_font, "TESTING", new Vector2(0, 200), Color.Green,
                    0.55f, Vector2.Zero, 4f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "TESTING", new Vector2(0, 600), Color.Green,
                    -0.55f, Vector2.Zero, 4f, SpriteEffects.None, 0f);

            spriteBatch.FillRectangle(new Rectangle(0, 200, 200, 200), Color.FromNonPremultiplied(new Color(Color.Red, 63).ToVector4()));
            spriteBatch.FillRectangle(new Rectangle(400, 200, 200, 200), Color.FromNonPremultiplied(new Color(Color.Blue, 192).ToVector4()));
              spriteBatch.FillRectangle(new Rectangle(200, 200, 200, 200), Color.Lerp(
                  Color.FromNonPremultiplied(new Color(Color.Red, 64).ToVector4()),
                  Color.FromNonPremultiplied(new Color(Color.Blue, 192).ToVector4()),
                  0.5f));

              //spriteBatch.FillRectangle(new Rectangle(0, 400, 200, 200), new Color(Color.Red, 64));
              //spriteBatch.FillRectangle(new Rectangle(400, 400, 200, 200), new Color(Color.Blue, 192));
              //spriteBatch.FillRectangle(new Rectangle(200, 400, 200, 200), Color.Lerp(new Color(Color.Red, 64), new Color(Color.Blue, 192), 0.5f));

              spriteBatch.FillRectangle(new Rectangle(0, 400, 200, 200), new Color(Color.Red, 64));
              spriteBatch.FillRectangle(new Rectangle(400, 400, 200, 200), new Color(Color.Blue, 191));

              var xx = Color.Lerp(new Color(Color.Red, 64), new Color(Color.Blue, 192), 0.25f);
              var xxg = Color.Lerp(new Color(Color.Red, 64), new Color(Color.Blue, 192), 0.5f);
              var xxgg = Color.Lerp(new Color(Color.Red, 64), new Color(Color.Blue, 192), 0.75f);

              var xx2 = Color.Lerp(Color.Red * .25f, Color.Blue * .75f, 0.25f);
              var xx21 = Color.Lerp(Color.Red * .25f, Color.Blue * .75f, 0.5f);
              var xx22 = Color.Lerp(Color.Red * .25f, Color.Blue * .75f, 0.75f);

              var tmp4 = Color.FromNonPremultiplied(new Color(Color.Red, 64).ToVector4());
              var tmp25 = Color.Red * 0.25f;

              spriteBatch.FillRectangle(new Rectangle(200, 400, 200, 200), 
                  Color.FromNonPremultiplied(
                  Color.Lerp(new Color(Color.Red, 64), new Color(Color.Blue, 192), 0.5f).ToVector4()));
            
            spriteBatch.FillRectangle(new Rectangle(0, 600, 200, 200), Color.Red * .25f);
            spriteBatch.FillRectangle(new Rectangle(400, 600, 200, 200), Color.Blue * .75f);
            spriteBatch.FillRectangle(new Rectangle(200, 600, 200, 200), Color.Lerp(Color.Red * .25f, Color.Blue * .75f, 0.5f));
            */
#endif
        }


        void DrawFade(ExtendedSpriteBatch spriteBatch, int steps, int y, int h, string s, Color c_start, Color c_end, bool premult)
        {
            spriteBatch.DrawString(_font, s,
                 new Vector2(0, y), Color.White,
                 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            for (int x = 0; x <= steps; x++)
            {
                var w = Engine.GameRectangle.Width / (steps + 1);
                var r = new Rectangle(x * w, y, w, h);
                var p = (float)x / (float)steps;

                var c = Color.Lerp(c_start, c_end, p);

                if (premult)
                    c = Color.FromNonPremultiplied(c.ToVector4());

                spriteBatch.FillRectangle(r, c);
            }
        }

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            inputManager.Update(PlayerIndex.One);
            
            if ((this.stateTimer > introTimer.Delay) || inputManager.IsActionTriggered(0))
                Operation = GameStateOperation.ChangeToState(new TitleState());

            Interpolators.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }
    }




    public class TitleState : GameState
    {
        public enum TitleActions
        {
            Go
        }

        ControlManager<TitleActions> controls = new ControlManager<TitleActions>();
        SpriteFont _font;

        float _scale = 1;
        bool _scaleReverse = false;
        Timer AnimationTimer = new Timer(20);

        public TitleState()
        {
            controls.MapControl(Controls.Start, TitleActions.Go);
            controls.MapControl(Controls.Back, TitleActions.Go);
        }

        public override void OnStateStarted(bool Resumed)
        {
            controls.Update();
        }


        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
            spriteBatch.DrawString(_font, "LizTris", new Vector2(28, 248), Color.Wheat, -0.5f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "Press a button to start",
                new Rectangle(0, GameRectangle.Bottom - 100, GameRectangle.Width, 100),
                ExtendedSpriteBatch.Alignment.Center, Color.Black, _scale);
        }

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            AnimationTimer.UpdateAndCheck(gameTime, () =>
            {
                if (!_scaleReverse)
                {
                    _scale += 0.02f;
                    if (_scale >= 1.2)
                        _scaleReverse = true;
                }
                else
                {
                    _scale -= 0.02f;
                    if (_scale <= 1)
                        _scaleReverse = false;
                }
            });

            controls.Update();

            if (controls.IsActionTriggered(TitleActions.Go, out var controller))
            {
                System.Diagnostics.Debug.Print("Player 1 is at {0}", controller);
                var m = new MenuState();
                Operation = GameStateOperation.ChangeToState(m);
            }
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }
    }

    public class ThisIsTheGame : GameState
    {
        InputManager<int> inputManager = new InputManager<int>();
        PauseMenuState pauseState = new PauseMenuState();

        public ThisIsTheGame()
        {
            inputManager.AddAction(0, Keys.Escape);
            inputManager.AddAction(0, InputManager<int>.GamePadButtons.Start);
        }

        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
            //spriteBatch.FillRectangle(GameRectangle, Color.Green);
            spriteBatch.DrawString(_font, "Playing Game", GameRectangle,
                ExtendedSpriteBatch.Alignment.Center, Color.Black, 3.0f);
        }

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            inputManager.Update(PlayerIndex.One);

            if (inputManager.IsActionTriggered(0))
            {
                Operation = GameStateOperation.AddState(pauseState);
            }

            if (pauseState.QuitGame)
                Operation = GameStateOperation.CompleteState;
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }

        SpriteFont _font;

        public override bool RenderPreviousState => true;
    }


    public class MenuState : GameState
    {
        public enum GameMenuOptions
        {
            QuitGame,
            NewGame,
            TestState,
            Exit,
            Options,
            StartMusic,
            StartMP3,
            StopMusic,

            ShowScores
        }

        public static SimpleMenu MainMenu = new SimpleMenu(string.Empty, new MenuItem[]
        {
            new MenuItem("New Game") { DoAction = GameMenuOptions.NewGame },
            new MenuItem("TestState") { DoAction = GameMenuOptions.TestState },
            new MenuItem("Options") { DoAction = GameMenuOptions.Options },
            new MenuItem("Stop") { DoAction = GameMenuOptions.StopMusic },
            new MenuItem("Music") { DoAction = GameMenuOptions.StartMusic },
            new MenuItem("MP3") { DoAction = GameMenuOptions.StartMP3 },
            new MenuItem("High Scores") { DoAction = GameMenuOptions.ShowScores },
            new OpenMenu("Quit") { Menu = new SubMenu("Really Quit?", new MenuItem[]
            {
                new Choice("Quit:", new MenuItem[]
                {
                    new MenuItem("Yes") { DoAction = GameMenuOptions.Exit },
                    new CloseMenu("No"),
                }) { DefaultIndex = 1 },
            }) },
        })
        { CloseOnBack = false };

        public override void OnStateStarted(bool Resumed)
        {
            MainMenu.ResetMenu();
            MainMenu.ResetInputs();
        }

        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
             pm.Draw(gameTime, spriteBatch);
           MainMenu.Draw(spriteBatch, _font, new Rectangle(40, 40, 600, 600), true);

        }

        ParticleManager pm = new ParticleManager();
        Random rand = new Random();

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            pm.Update(gameTime);

            for (int i = 0; i < 2; i++) // 2 particles per tick
            {
                var speed = 1.0f;
                var rad = (double)(rand.Next(0, 359)) * (3.1415926 / 180.0);
                var dx = Math.Cos(rad) * speed;
                var dy = Math.Sin(rad) * speed;

                pm.AddParticle(
                    new Vector2(350, this.Engine.GameRectangle.Center.Y), 
                        
                    new Vector2((float)dx, (float)dy),
                    Color.Red, 2,
                    100, true);
            }

            
            var rc = MainMenu.Update(gameTime, out object MenuAction);

            if (rc.HasFlag(MenuBase.MenuResult.PerformedAction) && (MenuAction != null))
            { 
                switch ((GameMenuOptions)MenuAction)
                {
                    case GameMenuOptions.NewGame:
                        Operation = GameStateOperation.AddState(new ThisIsTheGame());
                        MainMenu.CloseMenu();
                        break;

                    case GameMenuOptions.TestState:
                        Operation = GameStateOperation.AddState(new TestState());
                        break;

                    case GameMenuOptions.Options:
                        Operation = GameStateOperation.AddState(new GameStateEngineSettings());
                        break;

                    case GameMenuOptions.StopMusic:
                        Engine.StopMusic();
                        break;

                    case GameMenuOptions.StartMP3:
                        Engine.PlayMusic(Content.Load<Microsoft.Xna.Framework.Media.Song>("MusicMP3"));
                        break;

                    case GameMenuOptions.StartMusic:
                        Engine.PlayMusic(Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Music"));
                        break;

                    case GameMenuOptions.Exit:
                        Operation = GameStateOperation.CompleteState;
                        break;
                }
                MenuAction = null;
            }
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }

        SpriteFont _font;
    }



    public class PauseMenuState : GameState
    {
        public bool QuitGame { get; set; }


        public override bool RenderPreviousState => true;
        public enum GameMenuOptions
        {
            QuitGame,
            ResumeGame,
            Options,
        }

        public static SimpleMenu MainMenu = new SimpleMenu(string.Empty, new MenuItem[]
        {
            new MenuItem("Resume") { DoAction = GameMenuOptions.ResumeGame },
            new MenuItem("Options") { DoAction = GameMenuOptions.Options },
            new MenuItem("Quit Game") { DoAction = GameMenuOptions.QuitGame },
        });

        public override void OnStateStarted(bool Resumed)
        {
            QuitGame = false;
            MainMenu.ResetMenu();
            MainMenu.ResetInputs();
        }

        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
            MainMenu.Draw(spriteBatch, _font, new Rectangle(40, 40, 600, 600), true);
        }

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            var rc = MainMenu.Update(gameTime, out object MenuAction);

            if (rc.HasFlag(MenuBase.MenuResult.PerformedAction) && (MenuAction != null))
            {
                switch ((GameMenuOptions)MenuAction)
                {
                    case GameMenuOptions.ResumeGame:
                        MainMenu.CloseMenu();
                        break;

                    case GameMenuOptions.Options:
                        Operation = GameStateOperation.AddState(new GameStateEngineSettings());
                        break;

                    case GameMenuOptions.QuitGame:
                        QuitGame = true;
                        MainMenu.CloseMenu();
                        break;
                }
                MenuAction = null;
            }

            if (!MainMenu.IsMenuActive)
                Operation = GameStateOperation.CompleteState;
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }

        SpriteFont _font;
    }
}
