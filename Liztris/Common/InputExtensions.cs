using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Common
{
    public static class InputExtensions
    {
        public static bool KeyPressed(this KeyboardState currentState, KeyboardState oldState, Keys key)
        {
            if (currentState.IsKeyDown(key) && (oldState != null) && !oldState.IsKeyDown(key))
                return true;

            return false;
        }

        public static bool KeyReleased(this KeyboardState currentState, KeyboardState oldState, Keys key)
        {
            if (!currentState.IsKeyDown(key) && (oldState != null) && oldState.IsKeyDown(key))
                return true;

            return false;
        }

        public static bool LeftMouseButtonPressed(this MouseState currentState, MouseState oldState)
        {
            if ((currentState.LeftButton == ButtonState.Pressed) && (oldState != null) && 
                (oldState.LeftButton == ButtonState.Released))
                return true;

            return false;
        }

        public static bool LeftMouseButtonReleased(this MouseState currentState, MouseState oldState)
        {
            if ((currentState.LeftButton == ButtonState.Released) && (oldState != null) && 
                (oldState.LeftButton == ButtonState.Pressed))
                return true;

            return false;
        }

        public static bool RightMouseButtonPressed(this MouseState currentState, MouseState oldState)
        {
            if ((currentState.RightButton == ButtonState.Pressed) && (oldState != null) &&
                (oldState.RightButton == ButtonState.Released))
                return true;

            return false;
        }

        public static bool RightMouseButtonReleased(this MouseState currentState, MouseState oldState)
        {
            if ((currentState.RightButton == ButtonState.Released) && (oldState != null) &&
                (oldState.RightButton == ButtonState.Pressed))
                return true;

            return false;
        }

        public static bool MouseMoved(this MouseState currentState, MouseState oldState, out int DeltaX, out int DeltaY)
        {
            if (oldState == null)
            {
                DeltaX = DeltaY = 0;
                return false;
            }

            DeltaX = currentState.X - oldState.X;
            DeltaY = currentState.Y - oldState.Y;

            if (DeltaX == 0 && DeltaY == 0)
                return false;

            return true;
        }
    }
}
