using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liztris
{
    public class Player
    {
        public enum Actions
        {
            Left,
            Right,
            Drop,
            Rotate,
            RotateCounter,
        }

        public Player(Grid grid, PlayerIndex Player, InputManager<Actions> InputManager)
        {
            this.Grid = grid;
            this.inputManager = InputManager;
            this.playerIndex = Player;

            inputManager = new InputManager<Actions>();

            inputManager.AddAction(Actions.Left, Keys.Left);
            inputManager.AddAction(Actions.Left, InputManager<Actions>.GamePadButtons.Left);

            inputManager.AddAction(Actions.Right, Keys.Right);
            inputManager.AddAction(Actions.Right, InputManager<Actions>.GamePadButtons.Right);

            inputManager.AddAction(Actions.Drop, Keys.Down);
            inputManager.AddAction(Actions.Drop, InputManager<Actions>.GamePadButtons.Down);

            inputManager.AddAction(Actions.Rotate, Keys.Up);
            inputManager.AddAction(Actions.Rotate, InputManager<Actions>.GamePadButtons.A);
            inputManager.AddAction(Actions.Rotate, InputManager<Actions>.GamePadButtons.X);

            inputManager.AddAction(Actions.RotateCounter, InputManager<Actions>.GamePadButtons.B);
            inputManager.AddAction(Actions.RotateCounter, InputManager<Actions>.GamePadButtons.Y);
            inputManager.AddAction(Actions.RotateCounter, Keys.Space);
        }

        private InputManager<Actions> inputManager;
        private PlayerIndex playerIndex;
        private Grid Grid;

        public int Score { get; private set; }

        //TODO: Implement
        public string Name;
        public Color Color;

        public enum Speed
        {
            Slowest,
            Slow,
            Normal,
            Fast,
            Fastest
        }

        public void NewGame()
        {
            Score = 0;
            CurrentPiece = null;
            NextPiece = null;
        }

        

        public Piece CurrentPiece;
        public Piece NextPiece;

        public int piece_x;
        public int piece_y;


        public void MovePieceLeft()
        {
            if ((CurrentPiece == null) || (Grid == null))
                return;

            if (Grid.CheckPiece(CurrentPiece, piece_x - 1, piece_y))
                piece_x--;
        }

        public void MovePieceRight()
        {
            if ((CurrentPiece == null) || (Grid == null))
                return;

            if (Grid.CheckPiece(CurrentPiece, piece_x + 1, piece_y))
                piece_x++;
        }

        public void MovePieceDown()
        {
            if ((CurrentPiece == null) || (Grid == null))
                return;

            if (Grid.CheckPiece(CurrentPiece, piece_x, piece_y + 1))
                piece_y++;
        }

        public void RotatePieceCounterClockwise()
        {
            if ((CurrentPiece == null) || (Grid == null))
                return;

            CurrentPiece.RotateCounterClockwise();
            if (!Grid.CheckPiece(CurrentPiece, piece_x, piece_y))
                CurrentPiece.RotateClockwise();
        }

        public void RotatePieceClockwise()
        {
            if ((CurrentPiece == null) || (Grid == null)) 
                return;

            CurrentPiece.RotateClockwise();
            if (!Grid.CheckPiece(CurrentPiece, piece_x, piece_y))
                CurrentPiece.RotateCounterClockwise();
        }


        public void Update(GameTime gameTime)
        {
            if (NextPiece == null)
                NextPiece = PieceDefinition.GetRandomPiece();

            if (CurrentPiece == null)
            {
                CurrentPiece = NextPiece;
                NextPiece = PieceDefinition.GetRandomPiece();
            }

            inputManager.Update(playerIndex);

            if (inputManager.IsActionTriggered(Actions.Left))
            {
                MovePieceLeft();
            }
            else if (inputManager.IsActionTriggered(Actions.Right))
            {
                MovePieceRight();
            }
            else if (inputManager.IsActionTriggered(Actions.Drop))
            {
                MovePieceDown();
            }
            else if (inputManager.IsActionTriggered(Actions.Rotate))
            {
                RotatePieceClockwise();
            }
            else if (inputManager.IsActionTriggered(Actions.RotateCounter))
            {
                RotatePieceCounterClockwise();
            }
        }

        SpriteSheet Bricks;


        public void LoadContent(ContentManager Content)
        {
            Texture2D tex = Content.Load<Texture2D>("Bricks");
            Bricks = new SpriteSheet(tex, 4, 6);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            int offsetx = 0;
            int offsety = 15;

            if (CurrentPiece != null)
                for (int x = 0; x < CurrentPiece.Width; x++)
                    for (int y = 0; y < CurrentPiece.Height ; y++)
                        if (CurrentPiece.BlockMap[x, y] > 0)
                            Bricks.Draw(spriteBatch, 0, CurrentPiece.BlockMap[x, y] - 1, 
                                new Vector2(offsetx + 32 * (piece_x + x), 
                                offsety + 32 * (piece_y + y)));
        }



        public bool NewPiece()
        {/*
            var newPiece = Piece.GetPiece();

            piece_x = (Grid.BlocksX - newPiece.Width) / 2;
            piece_y = -newPiece.Height;

            int y = 0;
            for (y = 0; y <= newPiece.Height; y++)
            {
                if (Grid.CheckPiece(newPiece, piece_x, piece_y))
                    break;

                piece_y++;
            }
            if (y == newPiece.Height + 1)
            {
                return false;
            }


            CurrentPiece = newPiece;*/
            return true;
        }
    }
}
