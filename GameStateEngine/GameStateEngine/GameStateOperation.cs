using System;
using System.Collections.Generic;
using System.Text;

namespace GameStateEngine
{
    public class GameStateOperation
    {
        private GameStateOperation(bool CompleteCurrentState, GameState StateToPush)
        {
            this.CompleteCurrentState = CompleteCurrentState;
            this.StateToPush = StateToPush;
        }

        public bool CompleteCurrentState { get; }
        public GameState StateToPush { get; }

        public static GameStateOperation CompleteState =>
            new GameStateOperation(true, null);

        public static GameStateOperation ChangeToState(GameState State)
        {
            if (State == null)
                throw new InvalidOperationException("Invalid state");

            return new GameStateOperation(true, State);
        }

        public static GameStateOperation AddState(GameState State)
        {
            if (State == null)
                throw new InvalidOperationException("Invalid state");

            return new GameStateOperation(false, State);
        }
    }
}
