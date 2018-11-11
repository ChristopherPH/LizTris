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

            inputManager.AddAction(Actions.Left, InputManager<Actions>.GamePadButtons.Left);
            inputManager.AddAction(Actions.Right, InputManager<Actions>.GamePadButtons.Right);
            inputManager.AddAction(Actions.Drop, InputManager<Actions>.GamePadButtons.Down);
            inputManager.AddAction(Actions.Rotate, InputManager<Actions>.GamePadButtons.A);
            inputManager.AddAction(Actions.Rotate, InputManager<Actions>.GamePadButtons.X);
            inputManager.AddAction(Actions.RotateCounter, InputManager<Actions>.GamePadButtons.B);
            inputManager.AddAction(Actions.RotateCounter, InputManager<Actions>.GamePadButtons.Y);

            switch (Player)
            {
                case PlayerIndex.One:
                    inputManager.AddAction(Actions.Left, Keys.A);
                    inputManager.AddAction(Actions.Right, Keys.D);
                    inputManager.AddAction(Actions.Drop, Keys.S);
                    inputManager.AddAction(Actions.Rotate, Keys.W);
                    break;

                case PlayerIndex.Two:
                    inputManager.AddAction(Actions.Left, Keys.Left);
                    inputManager.AddAction(Actions.Right, Keys.Right);
                    inputManager.AddAction(Actions.Drop, Keys.Down);
                    inputManager.AddAction(Actions.Rotate, Keys.Up);
                    break;

                case PlayerIndex.Three:
                    inputManager.AddAction(Actions.Left, Keys.J);
                    inputManager.AddAction(Actions.Right, Keys.L);
                    inputManager.AddAction(Actions.Drop, Keys.K);
                    inputManager.AddAction(Actions.Rotate, Keys.I);
                    break;

                case PlayerIndex.Four:
                    inputManager.AddAction(Actions.Left, Keys.NumPad4);
                    inputManager.AddAction(Actions.Right, Keys.NumPad6);
                    inputManager.AddAction(Actions.Drop, Keys.NumPad5);
                    inputManager.AddAction(Actions.Rotate, Keys.NumPad8);
                    break;
            }


        }

        public InputManager<Actions> inputManager;
        public PlayerIndex playerIndex;
        public Grid Grid { get; private set; }

        public Piece CurrentPiece;
        public int piece_x;
        public int piece_y;

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


        /*
        public enum PlayerAction
        {
            Pause,
            RestartAllPlayers,
            HelpPlayers,
            HinderPlayers,
        }
        */


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
            else if (inputManager.IsActionTriggered(Actions.Drop))
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
                var drop = inputManager.IsActionPressed(Actions.Drop);

                if (!right && !left && !drop)
                    repeatTimer = null;

                if (repeatTimer != null)
                    if (repeatTimer.UpdateAndCheck(gameTime))
                    {
                        if (left)
                            MovePieceLeft();
                        else if (right)
                            MovePieceRight();
                        else if (drop)
                            MovePieceDown();
                    }
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
