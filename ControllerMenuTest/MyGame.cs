using Common;
using GameStateEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ControllerMenuTest
{
    public class MyGame : GameEngine
    {
        protected override int GameResolutionWidth => 1280;
        protected override int GameResolutionHeight => 720;
        protected override string DefaultSpriteFontContentPath => "Font";
        protected override GameState InitialGameState => new TitleScreen();
    }

    public class TitleScreen : GameState
    {
        SpriteFont _font;
        ControllerUtility controllerUtility = new ControllerUtility();
        string s;


        public override void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch, Rectangle GameRectangle)
        {
            spriteBatch.DrawString(_font, "Press a button", new Vector2(102, 2), Color.Black);

            if (s != null)
                spriteBatch.DrawString(_font, s, new Vector2(102, 80), Color.Black);
        }

        public override void Update(GameTime gameTime, ref GameStateOperation Operation)
        {
            var c = controllerUtility.GetController();
            if (c != null)
            {
                s = c.ToString();
            }
        }

        protected override void LoadContent()
        {
            _font = Content.Load<SpriteFont>("Font");
        }
    }
}
