using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerMenuTest
{

    /// <summary>
    /// Map a game action to a button press
    /// </summary>
    public abstract class ControlProfile<T> where T : struct, IConvertible
    {
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
            All = Keyboards | GamePads,
            None = 0x00
        }

        public ControlProfile()
        {
            Update();
        }

        public void Update()
        {

        }

        public virtual Controllers AllowedControllers => Controllers.All;
    }

    public interface IController
    {

    }

    public class GamePad2 : IController
    {
        public enum Buttons
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
    }

    public class Mouse : IController
    {
        public enum Buttons
        {
            Left,
            Right,
            Middle,
            X1,
            X2
        }
    }

    /* Gamepads will all have the same mappings (actu
     * 
     * 
     */


    public class MenuControl : ControlProfile<MenuControl.MenuActions>
    {



        [Flags]
        public enum Device : uint
        {

            GamePad1 = 0x01,
            GamePad2 = 0x02,
            GamePad3 = 0x04,
            GamePad4 = 0x08,

            Keyboard1 = 0x10,
            Keyboard2 = 0x20,
            Keyboard3 = 0x40,
            Keyboard4 = 0x80,
        }

        [Flags]
        public enum DeviceGroup : uint
        {
            GamePad1 = Device.GamePad1,
            GamePad2 = Device.GamePad2,
            GamePad3 = Device.GamePad3,
            GamePad4 = Device.GamePad4,

            Keyboard1 = 0x10,
            Keyboard2 = 0x20,
            Keyboard3 = 0x40,
            Keyboard4 = 0x80,

            GamePads = GamePad1 | GamePad2 | GamePad3 | GamePad4,
            Keyboards = Keyboard1 | Keyboard2 | Keyboard3 | Keyboard4,
            Any = Keyboards | GamePads,
            None = 0x00
        }


        public enum MenuActions
        {
            Up,
            Down,
            Left,
            Right,
            Select,
            Back
        }


        public class KeyMap
        {
            public void AddPlayer(DeviceType )

            public enum DeviceType
            {
                None,
                Keyboard,
                Gamepad,
            }


            DeviceType[] _PlayerDeviceType;
            int[] _PlayerDeviceNumber;
            GamePadState[] _GamePadState;
            KeyboardState _KeyboardState;

            public int MaxPlayers => MaxPlayersGamePad + MaxPlayersKeyboard;

            private int MaxPlayersKeyboard = 2;
            private int MaxPlayersGamePad => GamePad.MaximumGamePadCount;


            public KeyMap()
            {
                _PlayerDeviceType = new DeviceType[MaxPlayers];
                _PlayerDeviceNumber = new int[MaxPlayers];

                for (int i = 0; i < MaxPlayers; i++)
                {
                    _PlayerDeviceType[i] = DeviceType.None;
                    _PlayerDeviceNumber[i] = -1;
                }

                _GamePadState = new GamePadState[MaxPlayersGamePad];

                for (int i = 0; i < MaxPlayersGamePad; i++)
                    _GamePadState[i] = GamePad.GetState(i);

                _KeyboardState = Keyboard.GetState();
            }

            bool IsPressed(int Player, Buttons button)
            {
                var ix = _PlayerDeviceNumber[Player];

                switch (_PlayerDeviceType[Player])
                {
                    case DeviceType.Gamepad:
                        var 
                        return _GamePadState[ix].IsButtonDown(button);

                    case DeviceType.Keyboard:
                        return _KeyboardState[]
                }
            }



        }



        public MenuControl()
        {
            var x = Microsoft.Xna.Framework.Input.Buttons.A;


            var di = new Dictionary<Microsoft.Xna.Framework.Input.Buttons, Microsoft.Xna.Framework.Input.Keys>();

            di[Microsoft.Xna.Framework.Input.Buttons.A] = Microsoft.Xna.Framework.Input.Keys.A;
            

            var y = Microsoft.Xna.Framework.Input.MouseState;
            y.

            Actions.Add(MenuActions.Select, new Mouse(Mouse.Buttons.Left));

            Player[0].Controller = Device.GamePad1;

            Controls.Players.Add(Gamepad1);
            Controls.Players.Add(Gamepad2);
            Controls.Players.Add(Keyboard);
            Controls.Players.Add(Gamepad4);


            Controls.Update();

            /*
             * Single Player
            action[OK] = Gamepad1/A, GamePad1/Start, Keyboard.Enter, Keyboard.Space
            Action[BACK] = Gamebad1/B, GamePad1/Back, Keyboard.ESC

             * 2 Players
            action[OK] = Gamepad1/A, GamePad1/Start, Gamepad2/A, GamePad2/Start, Keyboard.Enter, Keyboard.Space
            Action[BACK] = Gamebad1/B, GamePad1/Back, Gamebad2/B, GamePad2/Back, Keyboard.ESC

            CheckAction(OK, out Device)

            Remap(Gamepad1, A) = B

            foreach (player)
                if (player.device.ispressed(actions.JUMP))


            [Liztris]
                [Liz]
                    [Keys]
                       
            foreach (var dev in controls.devices.where(x => !players.contains(x)))
            {
                if (dev.ispressed(actions.START)
                    players[playercount++] = device;

            }




            */

        public interface IControllerManager<T> where T : struct, IConvertible
        {

        }

        public abstract class ControllerManager<T>
        {
            public abstract bool IsActionTriggered(T action);
            public abstract bool IsActionPressed(T action);
        }

        public class KeyboardController<T> : ControllerManager<T>
        {
            public void Add(T action, Keys key)
            {

            }

            public List<Tuple<T, Keys>> Mapping {get;set;}
        }

        public class GamepadController<T> : ControllerManager<T>
        {
            public GamepadController(int GamepadNumber)
            {

            }

            public void Add(T action, Buttons button)
            {

            }

            public List<Tuple<T, Buttons>> Mapping { get; set; }
        }

        enum TestActions
        {
            Select,
            Back
        }

        ControllerManager<TestActions> input1 = new KeyboardController<TestActions>()
        {
            Mapping = new List<Tuple<TestActions, Keys>>()
            {
                new Tuple<TestActions, Keys>(TestActions.Select, Keys.Enter),
                new Tuple<TestActions, Keys>(TestActions.Select, Keys.Space),
                new Tuple<TestActions, Keys>(TestActions.Back, Keys.Escape),
            }
        };

        ControllerManager<TestActions> input2 = new GamepadController<TestActions>(0)
        {
            Mapping = new List<Tuple<TestActions, Buttons>>()
            {
                new Tuple<TestActions, Buttons>(TestActions.Select, Buttons.Start),
                new Tuple<TestActions, Buttons>(TestActions.Select, Buttons.A),
                new Tuple<TestActions, Buttons>(TestActions.Back, Buttons.Back),
                new Tuple<TestActions, Buttons>(TestActions.Back, Buttons.B),
            }
        };

        void test()
        {
            if (player1.IsActionTriggered(TestActions.Select))
            {
                ...
            }

            foreach (var x in new ControllerManager<TestActions>[] { input1, input2 })
                if (x.IsActionTriggered(TestActions.Select))
                {
                    var player1 = x;
                }

        }




        Inputter inp = new Inputter<ActionList>(GamePad1);

            inp.SetAction(ActionList.Jump, A);
            inp.SetAction(ActionList.Jump, X);

            if (inp.CheckAction(ActionList.Jump))
            {
                ...
            }




            PhysicalDevice x = GamePad1;

            x.SetMapping(2ndProfile);
            x.Map[A] = B;
            x.Map[B] = A;

            if (x.CheckAction(jump))
            {

            }






            if (Controls.CheckAction(TitleActions.OK, out Device device))
            {
                Player[0].Controller = device;
            }

            foreach (Player)
                if (Controls.CheckAction(Player.device, TitleActions.OK))


            for (var i = 0; i < Players.Count; i++)
            {
                if (Players[i].InMenu)
                {

                }
                else
                {

                }
            }

            DeviceMap[Device.GamePad1] = Player0;

            if (Controls.CheckAction(Actions.ShowPlayerMenu, out int[] PlayerNumbers))
            {
                foreach (var i in PlayerNumbers)
                    Players[i].InMenu = true;
            }

            if (Controls.CheckAction(Actions.))
        }
    }

}
