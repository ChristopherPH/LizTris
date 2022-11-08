//-----------------------------------------------------------------------------
// InputManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Common
{
    /// <summary>
    /// This class handles all keyboard and gamepad actions in the game.
    /// </summary>
    public class InputManager<T> where T : struct, IConvertible
    {
        #region Support Types


        /// <summary>
        /// GamePad controls expressed as one type, unified with button semantics.
        /// </summary>
        public enum GamePadButtons
        {
            Start,
            Back,
            A,
            B,
            X,
            Y,
            Up,
            Down,
            Left,
            Right,
            LeftShoulder,
            RightShoulder,
            LeftTrigger,
            RightTrigger,
        }

        public enum MouseButtons
        {
            Left,
            Right,
            Middle,
            X1,
            X2
        }


        /// <summary>
        /// A combination of gamepad and keyboard keys mapped to a particular action.
        /// </summary>
        public class ActionMap
        {
            /// <summary>
            /// List of GamePad controls to be mapped to a given action.
            /// </summary>
            public List<GamePadButtons> gamePadButtons = new List<GamePadButtons>();


            /// <summary>
            /// List of Keyboard controls to be mapped to a given action.
            /// </summary>
            public List<Keys> keyboardKeys = new List<Keys>();


            public List<MouseButtons> mouseButtons = new List<MouseButtons>();
        }


        #endregion


        #region Constants


        /// <summary>
        /// The value of an analog control that reads as a "pressed button".
        /// </summary>
        const float analogLimit = 0.5f;


        public KeyboardState currentKeyboardState { get; private set; }
        private KeyboardState previousKeyboardState;


        /// <summary>
        /// Check if a key is pressed.
        /// </summary>
        public bool IsKeyPressed(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }


        /// <summary>
        /// Check if a key was just pressed in the most recent update.
        /// </summary>
        public bool IsKeyTriggered(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key)) &&
                (!previousKeyboardState.IsKeyDown(key));
        }


        #endregion


        #region GamePad Data


        public GamePadState currentGamePadState { get; private set; }
        private GamePadState previousGamePadState;


        #region GamePadButton Pressed Queries


        /// <summary>
        /// Check if the gamepad's Start button is pressed.
        /// </summary>
        public bool IsGamePadStartPressed()
        {
            return (currentGamePadState.Buttons.Start == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's Back button is pressed.
        /// </summary>
        public bool IsGamePadBackPressed()
        {
            return (currentGamePadState.Buttons.Back == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's A button is pressed.
        /// </summary>
        public bool IsGamePadAPressed()
        {
            return (currentGamePadState.Buttons.A == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's B button is pressed.
        /// </summary>
        public bool IsGamePadBPressed()
        {
            return (currentGamePadState.Buttons.B == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's X button is pressed.
        /// </summary>
        public bool IsGamePadXPressed()
        {
            return (currentGamePadState.Buttons.X == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's Y button is pressed.
        /// </summary>
        public bool IsGamePadYPressed()
        {
            return (currentGamePadState.Buttons.Y == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's LeftShoulder button is pressed.
        /// </summary>
        public bool IsGamePadLeftShoulderPressed()
        {
            return (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed);
        }


        /// <summary>
        /// <summary>
        /// Check if the gamepad's RightShoulder button is pressed.
        /// </summary>
        public bool IsGamePadRightShoulderPressed()
        {
            return (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if Up on the gamepad's directional pad is pressed.
        /// </summary>
        public bool IsGamePadDPadUpPressed()
        {
            return (currentGamePadState.DPad.Up == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if Down on the gamepad's directional pad is pressed.
        /// </summary>
        public bool IsGamePadDPadDownPressed()
        {
            return (currentGamePadState.DPad.Down == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if Left on the gamepad's directional pad is pressed.
        /// </summary>
        public bool IsGamePadDPadLeftPressed()
        {
            return (currentGamePadState.DPad.Left == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if Right on the gamepad's directional pad is pressed.
        /// </summary>
        public bool IsGamePadDPadRightPressed()
        {
            return (currentGamePadState.DPad.Right == ButtonState.Pressed);
        }


        /// <summary>
        /// Check if the gamepad's left trigger is pressed.
        /// </summary>
        public bool IsGamePadLeftTriggerPressed()
        {
            return (currentGamePadState.Triggers.Left > analogLimit);
        }


        /// <summary>
        /// Check if the gamepad's right trigger is pressed.
        /// </summary>
        public bool IsGamePadRightTriggerPressed()
        {
            return (currentGamePadState.Triggers.Right > analogLimit);
        }


        /// <summary>
        /// Check if Up on the gamepad's left analog stick is pressed.
        /// </summary>
        public bool IsGamePadLeftStickUpPressed()
        {
            return (currentGamePadState.ThumbSticks.Left.Y > analogLimit);
        }


        /// <summary>
        /// Check if Down on the gamepad's left analog stick is pressed.
        /// </summary>
        public bool IsGamePadLeftStickDownPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Left.Y > analogLimit);
        }


        /// <summary>
        /// Check if Left on the gamepad's left analog stick is pressed.
        /// </summary>
        public bool IsGamePadLeftStickLeftPressed()
        {
            return (-1f * currentGamePadState.ThumbSticks.Left.X > analogLimit);
        }


        /// <summary>
        /// Check if Right on the gamepad's left analog stick is pressed.
        /// </summary>
        public bool IsGamePadLeftStickRightPressed()
        {
            return (currentGamePadState.ThumbSticks.Left.X > analogLimit);
        }


        /// <summary>
        /// Check if the GamePadKey value specified is pressed.
        /// </summary>
        private bool IsGamePadButtonPressed(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartPressed();

                case GamePadButtons.Back:
                    return IsGamePadBackPressed();

                case GamePadButtons.A:
                    return IsGamePadAPressed();

                case GamePadButtons.B:
                    return IsGamePadBPressed();

                case GamePadButtons.X:
                    return IsGamePadXPressed();

                case GamePadButtons.Y:
                    return IsGamePadYPressed();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderPressed();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderPressed();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerPressed();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerPressed();

                case GamePadButtons.Up:
                    return IsGamePadDPadUpPressed() ||
                        IsGamePadLeftStickUpPressed();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownPressed() ||
                        IsGamePadLeftStickDownPressed();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftPressed() ||
                        IsGamePadLeftStickLeftPressed();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightPressed() ||
                        IsGamePadLeftStickRightPressed();
            }

            return false;
        }


        #endregion


        #region GamePadButton Triggered Queries


        /// <summary>
        /// Check if the gamepad's Start button was just pressed.
        /// </summary>
        public bool IsGamePadStartTriggered()
        {
            return ((currentGamePadState.Buttons.Start == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.Start == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's Back button was just pressed.
        /// </summary>
        public bool IsGamePadBackTriggered()
        {
            return ((currentGamePadState.Buttons.Back == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.Back == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's A button was just pressed.
        /// </summary>
        public bool IsGamePadATriggered()
        {
            return ((currentGamePadState.Buttons.A == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.A == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's B button was just pressed.
        /// </summary>
        public bool IsGamePadBTriggered()
        {
            return ((currentGamePadState.Buttons.B == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.B == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's X button was just pressed.
        /// </summary>
        public bool IsGamePadXTriggered()
        {
            return ((currentGamePadState.Buttons.X == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.X == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's Y button was just pressed.
        /// </summary>
        public bool IsGamePadYTriggered()
        {
            return ((currentGamePadState.Buttons.Y == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.Y == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's LeftShoulder button was just pressed.
        /// </summary>
        public bool IsGamePadLeftShoulderTriggered()
        {
            return (
                (currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.LeftShoulder == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's RightShoulder button was just pressed.
        /// </summary>
        public bool IsGamePadRightShoulderTriggered()
        {
            return (
                (currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed) &&
                (previousGamePadState.Buttons.RightShoulder == ButtonState.Released));
        }


        /// <summary>
        /// Check if Up on the gamepad's directional pad was just pressed.
        /// </summary>
        public bool IsGamePadDPadUpTriggered()
        {
            return ((currentGamePadState.DPad.Up == ButtonState.Pressed) &&
                (previousGamePadState.DPad.Up == ButtonState.Released));
        }


        /// <summary>
        /// Check if Down on the gamepad's directional pad was just pressed.
        /// </summary>
        public bool IsGamePadDPadDownTriggered()
        {
            return ((currentGamePadState.DPad.Down == ButtonState.Pressed) &&
                (previousGamePadState.DPad.Down == ButtonState.Released));
        }


        /// <summary>
        /// Check if Left on the gamepad's directional pad was just pressed.
        /// </summary>
        public bool IsGamePadDPadLeftTriggered()
        {
            return ((currentGamePadState.DPad.Left == ButtonState.Pressed) &&
                (previousGamePadState.DPad.Left == ButtonState.Released));
        }


        /// <summary>
        /// Check if Right on the gamepad's directional pad was just pressed.
        /// </summary>
        public bool IsGamePadDPadRightTriggered()
        {
            return ((currentGamePadState.DPad.Right == ButtonState.Pressed) &&
                (previousGamePadState.DPad.Right == ButtonState.Released));
        }


        /// <summary>
        /// Check if the gamepad's left trigger was just pressed.
        /// </summary>
        public bool IsGamePadLeftTriggerTriggered()
        {
            return ((currentGamePadState.Triggers.Left > analogLimit) &&
                (previousGamePadState.Triggers.Left < analogLimit));
        }


        /// <summary>
        /// Check if the gamepad's right trigger was just pressed.
        /// </summary>
        public bool IsGamePadRightTriggerTriggered()
        {
            return ((currentGamePadState.Triggers.Right > analogLimit) &&
                (previousGamePadState.Triggers.Right < analogLimit));
        }


        /// <summary>
        /// Check if Up on the gamepad's left analog stick was just pressed.
        /// </summary>
        public bool IsGamePadLeftStickUpTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Left.Y > analogLimit) &&
                (previousGamePadState.ThumbSticks.Left.Y < analogLimit));
        }


        /// <summary>
        /// Check if Down on the gamepad's left analog stick was just pressed.
        /// </summary>
        public bool IsGamePadLeftStickDownTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Left.Y > analogLimit) &&
                (-1f * previousGamePadState.ThumbSticks.Left.Y < analogLimit));
        }


        /// <summary>
        /// Check if Left on the gamepad's left analog stick was just pressed.
        /// </summary>
        public bool IsGamePadLeftStickLeftTriggered()
        {
            return ((-1f * currentGamePadState.ThumbSticks.Left.X > analogLimit) &&
                (-1f * previousGamePadState.ThumbSticks.Left.X < analogLimit));
        }


        /// <summary>
        /// Check if Right on the gamepad's left analog stick was just pressed.
        /// </summary>
        public bool IsGamePadLeftStickRightTriggered()
        {
            return ((currentGamePadState.ThumbSticks.Left.X > analogLimit) &&
                (previousGamePadState.ThumbSticks.Left.X < analogLimit));
        }


        /// <summary>
        /// Check if the GamePadKey value specified was pressed this frame.
        /// </summary>
        private bool IsGamePadButtonTriggered(GamePadButtons gamePadKey)
        {
            switch (gamePadKey)
            {
                case GamePadButtons.Start:
                    return IsGamePadStartTriggered();

                case GamePadButtons.Back:
                    return IsGamePadBackTriggered();

                case GamePadButtons.A:
                    return IsGamePadATriggered();

                case GamePadButtons.B:
                    return IsGamePadBTriggered();

                case GamePadButtons.X:
                    return IsGamePadXTriggered();

                case GamePadButtons.Y:
                    return IsGamePadYTriggered();

                case GamePadButtons.LeftShoulder:
                    return IsGamePadLeftShoulderTriggered();

                case GamePadButtons.RightShoulder:
                    return IsGamePadRightShoulderTriggered();

                case GamePadButtons.LeftTrigger:
                    return IsGamePadLeftTriggerTriggered();

                case GamePadButtons.RightTrigger:
                    return IsGamePadRightTriggerTriggered();

                case GamePadButtons.Up:
                    return IsGamePadDPadUpTriggered() ||
                        IsGamePadLeftStickUpTriggered();

                case GamePadButtons.Down:
                    return IsGamePadDPadDownTriggered() ||
                        IsGamePadLeftStickDownTriggered();

                case GamePadButtons.Left:
                    return IsGamePadDPadLeftTriggered() ||
                        IsGamePadLeftStickLeftTriggered();

                case GamePadButtons.Right:
                    return IsGamePadDPadRightTriggered() ||
                        IsGamePadLeftStickRightTriggered();
            }

            return false;
        }


        #endregion


        #endregion


        #region MouseState Data

        public MouseState CurrentMouseState { get; private set; }
        private MouseState previousMouseState;

        public bool IsMouseLeftButtonPressed()
        {
            return (CurrentMouseState.LeftButton == ButtonState.Pressed);
        }

        public bool IsMouseRightButtonPressed()
        {
            return (CurrentMouseState.RightButton == ButtonState.Pressed);
        }

        public bool IsMouseMiddleButtonPressed()
        {
            return (CurrentMouseState.MiddleButton == ButtonState.Pressed);
        }

        public bool IsMouseX1ButtonPressed()
        {
            return (CurrentMouseState.XButton1 == ButtonState.Pressed);
        }

        public bool IsMouseX2ButtonPressed()
        {
            return (CurrentMouseState.XButton2 == ButtonState.Pressed);
        }



        private bool IsMouseButtonPressed(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return IsMouseLeftButtonPressed();
                case MouseButtons.Right:
                    return IsMouseRightButtonPressed();
                case MouseButtons.Middle:
                    return IsMouseMiddleButtonPressed();
                case MouseButtons.X1:
                    return IsMouseX1ButtonPressed();
                case MouseButtons.X2:
                    return IsMouseX2ButtonPressed();
            }
            return false;
        }

        public bool IsMouseLeftButtonTriggered()
        {
            return ((CurrentMouseState.LeftButton == ButtonState.Pressed) &&
                    (previousMouseState.LeftButton == ButtonState.Released));
        }

        public bool IsMouseRightButtonTriggered()
        {
            return ((CurrentMouseState.RightButton == ButtonState.Pressed) &&
                    (previousMouseState.RightButton == ButtonState.Released));
        }

        public bool IsMouseMiddleButtonTriggered()
        {
            return ((CurrentMouseState.MiddleButton == ButtonState.Pressed) &&
                    (previousMouseState.MiddleButton == ButtonState.Released));
        }

        public bool IsMouseX1ButtonTriggered()
        {
            return ((CurrentMouseState.XButton1 == ButtonState.Pressed) &&
                    (previousMouseState.XButton1 == ButtonState.Released));
        }

        public bool IsMouseX2ButtonTriggered()
        {
            return ((CurrentMouseState.XButton2 == ButtonState.Pressed) &&
                    (previousMouseState.XButton2 == ButtonState.Released));
        }

        /// <summary>
        /// Check if the GamePadKey value specified was pressed this frame.
        /// </summary>
        private bool IsMouseButtonTriggered(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return IsMouseLeftButtonTriggered();
                case MouseButtons.Right:
                    return IsMouseRightButtonTriggered();
                case MouseButtons.Middle:
                    return IsMouseMiddleButtonTriggered();
                case MouseButtons.X1:
                    return IsMouseX1ButtonTriggered();
                case MouseButtons.X2:
                    return IsMouseX2ButtonTriggered();
            }

            return false;
        }


        #endregion

        #region Action Mapping


        /// <summary>
        /// The action mappings for the game.
        /// </summary>
        private Dictionary<T, ActionMap> actionMaps = new Dictionary<T, ActionMap>();


        public void AddAction(T Action, Keys Key)
        {
            if (!actionMaps.ContainsKey(Action))
                actionMaps[Action] = new ActionMap();

            foreach (var a in actionMaps.Values)
                if (a.keyboardKeys.Contains(Key))
                    throw new ArgumentException("Key already exists in map");

            actionMaps[Action].keyboardKeys.Add(Key);
        }

        public void AddAction(T Action, GamePadButtons Button)
        {
            if (!actionMaps.ContainsKey(Action))
                actionMaps[Action] = new ActionMap();

            foreach (var a in actionMaps.Values)
                if (a.gamePadButtons.Contains(Button))
                    throw new ArgumentException("Button already exists in map");

            actionMaps[Action].gamePadButtons.Add(Button);
        }

        public void AddAction(T Action, MouseButtons Button)
        {
            if (!actionMaps.ContainsKey(Action))
                actionMaps[Action] = new ActionMap();

            foreach (var a in actionMaps.Values)
                if (a.mouseButtons.Contains(Button))
                    throw new ArgumentException("Button already exists in map");

            actionMaps[Action].mouseButtons.Add(Button);
        }

        /// <summary>
        /// Check if an action has been pressed.
        /// </summary>
        public bool IsActionPressed(T action)
        {
            if (!actionMaps.ContainsKey(action))
                return false;

            return IsActionMapPressed(actionMaps[action]);
        }


        /// <summary>
        /// Check if an action was just performed in the most recent update.
        /// </summary>
        public bool IsActionTriggered(T action)
        {
            if (!actionMaps.ContainsKey(action))
                return false;

            return IsActionMapTriggered(actionMaps[action]);
        }


        /// <summary>
        /// Check if an action map has been pressed.
        /// </summary>
        private bool IsActionMapPressed(ActionMap actionMap)
        {
            foreach (var key in actionMap.keyboardKeys)
                if (IsKeyPressed(key))
                    return true;

            foreach (var button in actionMap.mouseButtons)
                if (IsMouseButtonPressed(button))
                    return true;

            if (currentGamePadState.IsConnected)
            {
                foreach (var button in actionMap.gamePadButtons)
                    if (IsGamePadButtonPressed(button))
                        return true;
            }
            return false;
        }


        /// <summary>
        /// Check if an action map has been triggered this frame.
        /// </summary>
        private bool IsActionMapTriggered(ActionMap actionMap)
        {
            foreach (var key in actionMap.keyboardKeys)
                if (IsKeyTriggered(key))
                    return true;

            foreach (var button in actionMap.mouseButtons)
                if (IsMouseButtonTriggered(button))
                    return true;

            if (currentGamePadState.IsConnected)
            {
                foreach (var button in actionMap.gamePadButtons)
                    if (IsGamePadButtonTriggered(button))
                        return true;
            }
            return false;
        }


        #endregion


        #region Updating


        /// <summary>
        /// Updates the keyboard and gamepad control states.
        /// </summary>
        public void Update(PlayerIndex index)
        {
            // update the keyboard state
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            // update the keyboard state
            previousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            // update the gamepad state
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(index);
        }


        #endregion
    }
}

