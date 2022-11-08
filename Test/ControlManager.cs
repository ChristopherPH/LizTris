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

namespace Test.ControllerManager
{
    public enum Controls
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
    }

    [Flags]
    public enum Controllers : uint
    {

        GamePad1 = 0x01,
        GamePad2 = 0x02,
        GamePad3 = 0x04,
        GamePad4 = 0x08,

        Keyboard1 = 0x10,
        Keyboard2 = 0x20,
        Keyboard3 = 0x40,
        Keyboard4 = 0x80,

        GamePads = GamePad1 | GamePad2 | GamePad3 | GamePad4,
        Keyboards = Keyboard1 | Keyboard2 | Keyboard3 | Keyboard4,
        Any = Keyboards | GamePads,
        None = 0x00
    }

    public class ControlManager<T> where T : struct, IConvertible
    {
        InputManager<T>[] inputManager;

        const int KeyboardCount = 2;
        const int ControlCount = 10;

        Keys[][][] KeyboardMap = new Keys[KeyboardCount][][]
        {
            /*
            new Keys[ControlCount][] {
                new Keys[] { Keys.Enter, Keys.Space },
                new Keys[] { Keys.Escape, Keys.Back },
                new Keys[] { Keys.A },
                new Keys[] { Keys.S },
                new Keys[] { Keys.X },
                new Keys[] { Keys.Z },
                new Keys[] { Keys.Up },
                new Keys[] { Keys.Down },
                new Keys[] { Keys.Left },
                new Keys[] { Keys.Right }
            },*/

            new Keys[ControlCount][] {
                new Keys[] { Keys.Enter, },
                new Keys[] { Keys.Back },
                new Keys[] { Keys.N },
                new Keys[] { Keys.M },
                new Keys[] { Keys.OemComma },
                new Keys[] { Keys.OemPeriod },
                new Keys[] { Keys.Up },
                new Keys[] { Keys.Down },
                new Keys[] { Keys.Left },
                new Keys[] { Keys.Right }
            },

            new Keys[ControlCount][] {
                new Keys[] { Keys.Space, },
                new Keys[] { Keys.Escape },
                new Keys[] { Keys.X },
                new Keys[] { Keys.Z },
                new Keys[] { Keys.C},
                new Keys[] { Keys.V},
                new Keys[] { Keys.W },
                new Keys[] { Keys.S },
                new Keys[] { Keys.A },
                new Keys[] { Keys.D }
            }
        };



        public ControlManager()
        {
            inputManager = new InputManager<T>[GamePad.MaximumGamePadCount + KeyboardCount];

            for (int i = 0; i < GamePad.MaximumGamePadCount + KeyboardCount; i++)
                inputManager[i] = new InputManager<T>();

            Update();
        }

        public void Update()
        {
            int GamePadNumber = 0;
            for (GamePadNumber = 0; GamePadNumber < GamePad.MaximumGamePadCount; GamePadNumber++)
            {
                inputManager[GamePadNumber].Update((PlayerIndex)GamePadNumber);
            }

            for (int i = 0; i < KeyboardCount; i++)
            {
                inputManager[GamePadNumber + i].Update(PlayerIndex.One);
            }
        }

        public void MapControl(Controls control, T Action)
        {
            int GamePadNumber = 0;
            for (GamePadNumber = 0; GamePadNumber < GamePad.MaximumGamePadCount; GamePadNumber++)
            {
                inputManager[GamePadNumber].AddAction(Action,
                    (InputManager<T>.GamePadButtons)control);
            }

            for (int i = 0; i < KeyboardCount; i++)
            {
                inputManager[GamePadNumber + i].Update(PlayerIndex.One);
            }

            for (int i = 0; i < KeyboardCount; i++)
            {
                var keys = KeyboardMap[i][(int)control];
                if (keys != null)
                    foreach (var k in keys)
                        inputManager[GamePadNumber + i].AddAction(Action, k);
            }
        }

        public bool IsActionTriggered(T action)
        {
            return IsActionTriggered(action, Controllers.Any, out var Controller);
        }

        public bool IsActionTriggered(T action, out Controllers Controller)
        {
            return IsActionTriggered(action, Controllers.Any, out Controller);
        }

        public bool IsActionTriggered(T action, Controllers ControllerMask)
        {
            return IsActionTriggered(action, ControllerMask, out var Controller);
        }

        public bool IsActionTriggered(T action, Controllers ControllerMask, out Controllers Controller)
        {
            if (ControllerMask != Controllers.None)
            {
                for (int i = 0; i < GamePad.MaximumGamePadCount + KeyboardCount; i++)
                {
                    var ControllerBit = (Controllers)(1 << i);

                    if (!ControllerMask.HasFlag(ControllerBit))
                        continue;

                    if (!inputManager[i].IsActionTriggered(action))
                        continue;

                    Controller = ControllerBit;
                    return true;
                }
            }

            Controller = Controllers.None;
            return false;
        }
    }
}
