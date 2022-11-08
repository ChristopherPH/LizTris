//using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerMenuTest
{
    public interface IController<Actions>
        where Actions : struct, IConvertible//, IEquatable<Action>
    {
        void Update();

        bool IsActionTriggered(Actions Action);

        bool IsActionPressed(Actions Action);
    }


    public abstract class Controller<Actions, Inputs> : IController<Actions>
        where Actions : struct, IConvertible//, IEquatable<Action>
        where Inputs : struct, IConvertible//, IEquatable<Inputs>
    {
        public Controller() 
        {
            Init();
        }

        public ActionMap[] Map { get; set; }

        protected abstract void Init();
        public abstract void Update();

        public bool IsActionTriggered(Actions Action)
        {
            if (Map == null)
                throw new MissingFieldException(nameof(Controller<Actions, Inputs>), nameof(Map));

            foreach (var entry in Map)
                if (entry.Action.Equals(Action))
                    if (IsInputTriggered(entry.Input))
                        return true;

            return false;
        }

        public bool IsActionPressed(Actions Action)
        {
            if (Map == null)
                throw new MissingFieldException(nameof(Controller<Actions, Inputs>), nameof(Map));

            foreach (var entry in Map)
                if (entry.Action.Equals(Action))
                    if (IsInputPressed(entry.Input))
                        return true;

            return false;
        }

        public abstract bool IsInputTriggered(Inputs Input);
        public abstract bool IsInputPressed(Inputs Input);


        public class ActionMap
        {
            public ActionMap() { }
            public ActionMap(Actions Action, Inputs Input)
            {
                this.Action = Action;
                this.Input = Input;
            }

            public Actions Action { get; set; }
            public Inputs Input { get; set; }
        }
    }

    public class KeyboardController<Actions> : Controller<Actions, Microsoft.Xna.Framework.Input.Keys>
        where Actions : struct, IConvertible//, IEquatable<Action>
    {
        public Microsoft.Xna.Framework.Input.KeyboardState CurrentState { get; private set; }
        Microsoft.Xna.Framework.Input.KeyboardState _prevState;

        protected override void Init()
        {
            CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            _prevState = CurrentState;
        }

        public override void Update()
        {
            _prevState = CurrentState;
            CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public override bool IsInputPressed(Microsoft.Xna.Framework.Input.Keys Input)
        {
            return CurrentState.IsKeyDown(Input);
        }

        public override bool IsInputTriggered(Microsoft.Xna.Framework.Input.Keys Input)
        {
            return (CurrentState.IsKeyDown(Input)) &&
                (!_prevState.IsKeyDown(Input));
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }

    public class XInputController<Actions> : Controller<Actions, Microsoft.Xna.Framework.Input.Buttons>
        where Actions : struct, IConvertible//, IEquatable<Action>
    {
        /// <summary>
        /// 0 Based gamepad index
        /// </summary>
        public int GamepadNumber { get; private set; } = -1;
        public Microsoft.Xna.Framework.Input.GamePadState CurrentState { get; private set; }
        Microsoft.Xna.Framework.Input.GamePadState _prevState;

        public XInputController(int GamepadNumber) : base()
        {
            if (GamepadNumber < 0 || GamepadNumber >= Microsoft.Xna.Framework.Input.GamePad.MaximumGamePadCount)
                throw new ArgumentException();

            this.GamepadNumber = GamepadNumber;
        }

        protected override void Init()
        {
            CurrentState = Microsoft.Xna.Framework.Input.GamePad.GetState(GamepadNumber);
            _prevState = CurrentState;
        }

        public override void Update()
        {
            _prevState = CurrentState;
            CurrentState = Microsoft.Xna.Framework.Input.GamePad.GetState(GamepadNumber);
        }

        public override bool IsInputTriggered(Microsoft.Xna.Framework.Input.Buttons Input)
        {
            if (!CurrentState.IsConnected)
                return false;

            return CurrentState.IsButtonDown(Input);
        }

        public override bool IsInputPressed(Microsoft.Xna.Framework.Input.Buttons Input)
        {
            if (!CurrentState.IsConnected)
                return false;

            return (CurrentState.IsButtonDown(Input)) &&
                (!_prevState.IsButtonDown(Input));
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + GamepadNumber;
        }
    }

    public class SimpleGamepad<Actions> : Controller<Actions, SimpleGamepad<Actions>.SimpleGamepadButtons>
        where Actions : struct, IConvertible//, IEquatable<Action>
    {
        public enum SimpleGamepadButtons
        {
            Start,
            Back,
            A,
            B,
            X,
            Y,
            Up,     //combine all sticks and dpad
            Down,
            Left,
            Right,
            LeftShoulderTrigger,
            RightShoulderTrigger,
        }

        /// <summary>
        /// The value of an analog control that reads as a "pressed button".
        /// </summary>
        const float analogLimit = 0.5f;

        /// <summary>
        /// 0 Based gamepad index
        /// </summary>
        public int GamepadNumber { get; private set; } = -1;
        public Microsoft.Xna.Framework.Input.GamePadState CurrentState { get; private set; }
        Microsoft.Xna.Framework.Input.GamePadState _prevState;

        public SimpleGamepad(int GamepadNumber) : base()
        {
            if (GamepadNumber < 0 || GamepadNumber >= Microsoft.Xna.Framework.Input.GamePad.MaximumGamePadCount)
                throw new ArgumentException();

            this.GamepadNumber = GamepadNumber;
        }

        protected override void Init()
        {
            CurrentState = Microsoft.Xna.Framework.Input.GamePad.GetState(GamepadNumber);
            _prevState = CurrentState;
        }

        public override void Update()
        {
            _prevState = CurrentState;
            CurrentState = Microsoft.Xna.Framework.Input.GamePad.GetState(GamepadNumber);
        }

        public override bool IsInputTriggered(SimpleGamepadButtons Input)
        {
            return (IsButtonDown(CurrentState, Input)) &&
                (!IsButtonDown(_prevState, Input));
        }

        public override bool IsInputPressed(SimpleGamepadButtons Input)
        {
            return IsButtonDown(CurrentState, Input);
        }

        public override string ToString()
        {
            return this.GetType().Name + ": " + GamepadNumber;
        }

        private static bool IsButtonDown(Microsoft.Xna.Framework.Input.GamePadState CurrentState, SimpleGamepadButtons Input)
        {
            if ((CurrentState == null) || !CurrentState.IsConnected)
                return false;

            switch (Input)
            {
                case SimpleGamepadButtons.Start:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Start);
                case SimpleGamepadButtons.Back:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Back);

                case SimpleGamepadButtons.A:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.A);
                case SimpleGamepadButtons.B:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.B);
                case SimpleGamepadButtons.X:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.X);
                case SimpleGamepadButtons.Y:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.Y);

                case SimpleGamepadButtons.Left:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadLeft) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickLeft) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightThumbstickLeft);
                        //(-1f * CurrentState.ThumbSticks.Left.X > analogLimit) ||
                        //(-1f * CurrentState.ThumbSticks.Right.X > analogLimit);

                case SimpleGamepadButtons.Right:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadRight) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickRight) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightThumbstickRight);
                        //(CurrentState.ThumbSticks.Left.X > analogLimit) ||
                        //(CurrentState.ThumbSticks.Right.X > analogLimit);

                case SimpleGamepadButtons.Up:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadUp) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickUp) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightThumbstickUp);
                        //(CurrentState.ThumbSticks.Left.Y > analogLimit) ||
                        //(CurrentState.ThumbSticks.Right.Y > analogLimit);

                case SimpleGamepadButtons.Down:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.DPadDown) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickDown) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightThumbstickDown);
                        //(-1f * CurrentState.ThumbSticks.Left.Y > analogLimit) ||
                        //(-1f * CurrentState.ThumbSticks.Right.Y > analogLimit);

                case SimpleGamepadButtons.LeftShoulderTrigger:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftShoulder) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftTrigger) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.LeftThumbstickDown);
                        //(CurrentState.Triggers.Left > analogLimit);

                case SimpleGamepadButtons.RightShoulderTrigger:
                    return CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightShoulder) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightTrigger) ||
                        CurrentState.IsButtonDown(Microsoft.Xna.Framework.Input.Buttons.RightThumbstickDown);
                        //(CurrentState.Triggers.Left > analogLimit);

                default:
                    return false;
            }
        }
    }

    public class ControllerUtility
    {
        public enum ActionStart
        {
            Start
        }

        List<IController<ActionStart>> Controllers = new List<IController<ActionStart>>();

        public ControllerUtility()
        {
            for (int i = 0; i < Microsoft.Xna.Framework.Input.GamePad.MaximumGamePadCount; i++)
                Controllers.Add(new SimpleGamepad<ActionStart>(i)
                {
                    Map = new Controller<ActionStart, SimpleGamepad<ActionStart>.SimpleGamepadButtons>.ActionMap[]
                    {
                        new Controller<ActionStart, SimpleGamepad<ActionStart>.SimpleGamepadButtons>.ActionMap(ActionStart.Start, SimpleGamepad<ActionStart>.SimpleGamepadButtons.A),
                        new Controller<ActionStart, SimpleGamepad<ActionStart>.SimpleGamepadButtons>.ActionMap(ActionStart.Start, SimpleGamepad<ActionStart>.SimpleGamepadButtons.Start),
                    }
                });

            Controllers.Add(new KeyboardController<ActionStart>()
            {
                Map = new Controller<ActionStart, Microsoft.Xna.Framework.Input.Keys>.ActionMap[]
                {
                    new Controller<ActionStart, Microsoft.Xna.Framework.Input.Keys>.ActionMap(ActionStart.Start, Microsoft.Xna.Framework.Input.Keys.Enter),
                    new Controller<ActionStart, Microsoft.Xna.Framework.Input.Keys>.ActionMap(ActionStart.Start, Microsoft.Xna.Framework.Input.Keys.Space),
                }
            });
        }

        public IController<ActionStart> GetController()
        {
            foreach (var c in Controllers)
            {
                c.Update();

                if (c.IsActionPressed(ActionStart.Start))
                    return c;
            }

            return null;
        }
    }
}

