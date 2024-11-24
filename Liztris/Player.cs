using Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Liztris
{
    public class Player
    {
        public enum Actions
        {
            Left,
            Right,
            SoftDrop,
            Rotate,
            RotateCounter,
        }

        public Player(Grid grid, PlayerIndex Player, string Name, InputManager<Actions> InputManager)
        {
            this.Grid = grid;
            this.inputManager = InputManager;
            this.playerIndex = Player;
            this.Name = Name;

            inputManager = new InputManager<Actions>();

            inputManager.AddAction(Actions.Left, InputManager<Actions>.GamePadButtons.Left);
            inputManager.AddAction(Actions.Right, InputManager<Actions>.GamePadButtons.Right);
            inputManager.AddAction(Actions.SoftDrop, InputManager<Actions>.GamePadButtons.Down);
            inputManager.AddAction(Actions.Rotate, InputManager<Actions>.GamePadButtons.A);
            inputManager.AddAction(Actions.Rotate, InputManager<Actions>.GamePadButtons.X);
            inputManager.AddAction(Actions.RotateCounter, InputManager<Actions>.GamePadButtons.B);
            inputManager.AddAction(Actions.RotateCounter, InputManager<Actions>.GamePadButtons.Y);

            switch (Player)
            {
                case PlayerIndex.One:
                    inputManager.AddAction(Actions.Left, Keys.A);
                    inputManager.AddAction(Actions.Right, Keys.D);
                    inputManager.AddAction(Actions.SoftDrop, Keys.S);
                    inputManager.AddAction(Actions.Rotate, Keys.W);
                    break;

                case PlayerIndex.Two:
                    inputManager.AddAction(Actions.Left, Keys.Left);
                    inputManager.AddAction(Actions.Right, Keys.Right);
                    inputManager.AddAction(Actions.SoftDrop, Keys.Down);
                    inputManager.AddAction(Actions.Rotate, Keys.Up);
                    break;

                case PlayerIndex.Three:
                    inputManager.AddAction(Actions.Left, Keys.J);
                    inputManager.AddAction(Actions.Right, Keys.L);
                    inputManager.AddAction(Actions.SoftDrop, Keys.K);
                    inputManager.AddAction(Actions.Rotate, Keys.I);
                    break;

                case PlayerIndex.Four:
                    inputManager.AddAction(Actions.Left, Keys.NumPad4);
                    inputManager.AddAction(Actions.Right, Keys.NumPad6);
                    inputManager.AddAction(Actions.SoftDrop, Keys.NumPad5);
                    inputManager.AddAction(Actions.Rotate, Keys.NumPad8);
                    break;
            }
        }

        public InputManager<Actions> inputManager;
        public PlayerIndex playerIndex;
        public Grid Grid { get; private set; }
        public string Name { get; private set; }

        public Piece CurrentPiece;
        public int piece_x;
        public int piece_y;

        public void NewGame()
        {
            CurrentPiece = null;
        }

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

        private bool PlacePiece(Piece CurrentPiece, ref int piece_x, ref int piece_y)
        {
            //piece fits, great!
            if (Grid.CheckPiece(CurrentPiece, piece_x, piece_y))
            {
                return true;
            }

            //wall kicks
            if (Grid.CheckPiece(CurrentPiece, piece_x + 1, piece_y))
            {
                piece_x += 1;
                return true;
            }

            if (Grid.CheckPiece(CurrentPiece, piece_x - 1, piece_y))
            {
                piece_x -= 1;
                return true;
            }

            if (Grid.CheckPiece(CurrentPiece, piece_x + 2, piece_y))
            {
                piece_x += 2;
                return true;
            }

            if (Grid.CheckPiece(CurrentPiece, piece_x - 2, piece_y))
            {
                piece_x -= 2;
                return true;
            }

            //allow rotate from top line by pushing down one
            if (piece_y - CurrentPiece.Height <= 0)
            {
                if (Grid.CheckPiece(CurrentPiece, piece_x, piece_y + 1))
                {
                    piece_y += 1;
                    return true;
                }
            }

            return false;
        }

        public void RotatePieceCounterClockwise()
        {
            if ((CurrentPiece == null) || (Grid == null))
                return;

            CurrentPiece.RotateCounterClockwise();

            if (!PlacePiece(CurrentPiece, ref piece_x, ref piece_y))
            {
                //put back
                CurrentPiece.RotateClockwise();
            }
        }

        public void RotatePieceClockwise()
        {
            if ((CurrentPiece == null) || (Grid == null)) 
                return;

            CurrentPiece.RotateClockwise();

            if (!PlacePiece(CurrentPiece, ref piece_x, ref piece_y))
            {
                //put back
                CurrentPiece.RotateCounterClockwise();
            }
        }

        public Timer repeatTimer = null;
        const int moveRepeatTime = 150;
        const int dropRepeatTime = 100;

        public void Update(GameTime gameTime)
        {
            inputManager.Update(playerIndex);

            if (inputManager.IsActionTriggered(Actions.Left))
            {
                MovePieceLeft();
                repeatTimer = new Timer(moveRepeatTime);
            }
            else if (inputManager.IsActionTriggered(Actions.Right))
            {
                MovePieceRight();
                repeatTimer = new Timer(moveRepeatTime);
            }
            else if (inputManager.IsActionTriggered(Actions.SoftDrop))
            {
                MovePieceDown();
                repeatTimer = new Timer(dropRepeatTime);
            }
            else if (inputManager.IsActionTriggered(Actions.Rotate))
            {
                RotatePieceClockwise();
            }
            else if (inputManager.IsActionTriggered(Actions.RotateCounter))
            {
                RotatePieceCounterClockwise();
            }
            else
            {
                var left = inputManager.IsActionPressed(Actions.Left);
                var right = inputManager.IsActionPressed(Actions.Right);
                var drop = inputManager.IsActionPressed(Actions.SoftDrop);

                if (!right && !left && !drop)
                    repeatTimer = null;

                if (repeatTimer != null)
                    repeatTimer.UpdateAndCheck(gameTime, () =>
                    {
                        if (left)
                            MovePieceLeft();
                        else if (right)
                            MovePieceRight();
                        else if (drop)
                            MovePieceDown();
                    });
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteSheet Blocks, int BlockPixelSize)
        {
            if (CurrentPiece == null)
                return;

            CurrentPiece.Draw(spriteBatch, Blocks, BlockPixelSize,
                Grid.ScreenRect.X + (BlockPixelSize * piece_x),
                Grid.ScreenRect.Y + (BlockPixelSize * piece_y));
        }
    }
}
