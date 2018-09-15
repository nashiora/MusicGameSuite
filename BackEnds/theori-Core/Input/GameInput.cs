using System;
using theori.Configuration;

namespace theori.Input
{
    public enum GameButton : uint
    {
        Start = 0,
        BtA, BtB, BtC, BtD,
        FxL, FxR,
        Back,
    }

    public enum GameAxis : uint
    {
        VolL = 0, VolR,
    }

    public sealed class GameInput : Disposable
    {
        private readonly int gamepadIndex;
        private Gamepad gamepad;

        private InputDevice BtInputDevice => Application.GameConfig.GetEnum<InputDevice>(GameConfigKey.ButtonInputDevice);
        private InputDevice VolInputDevice => Application.GameConfig.GetEnum<InputDevice>(GameConfigKey.LaserInputDevice);

        public GameInput(int gamepadIndex)
        {
            this.gamepadIndex = gamepadIndex;
            gamepad = Gamepad.Open(gamepadIndex);
#pragma warning disable CS0642 // Possible mistaken empty statement
            if (gamepad); else gamepad = null;
#pragma warning restore CS0642 // Possible mistaken empty statement

            Gamepad.Connect += Gamepad_Connect;
            Gamepad.Disconnect += Gamepad_Disconnect;
        }

        /// <summary>
        /// Doesn't check the back button, which is only event-driven.
        /// </summary>
        public bool ButtonDown(GameButton button)
        {
            if (button == GameButton.Back) return false;
            
            if (gamepad != null && BtInputDevice == InputDevice.Controller)
            {
                GameConfigKey configKey = GameConfigKey.Controller_BTS + (int)button;
                return gamepad.ButtonDown((uint)Application.GameConfig.GetInt(configKey));
            }
            else
            {
                GameConfigKey configKey = GameConfigKey.Key_BTS + (int)button;
                return Keyboard.IsDown(Application.GameConfig.GetEnum<KeyCode>(configKey));
            }
        }

        public float AxisDir(GameAxis axis)
        {
            if (gamepad != null && VolInputDevice == InputDevice.Controller)
            {
                GameConfigKey configKey = GameConfigKey.Controller_Laser0Axis + (int)axis;
                return gamepad.GetAxis((uint)Application.GameConfig.GetInt(configKey));
            }
            else if (VolInputDevice == InputDevice.Keyboard)
            {
                float sens = Application.GameConfig.GetFloat(GameConfigKey.Key_Sensitivity);
                int axisIndex = Application.GameConfig.GetInt(GameConfigKey.Mouse_Laser0Axis + (int)axis);
                
                KeyCode pos = Application.GameConfig.GetEnum<KeyCode>(GameConfigKey.Key_Laser0Pos + (int)axis);
                KeyCode neg = Application.GameConfig.GetEnum<KeyCode>(GameConfigKey.Key_Laser0Neg + (int)axis);

                return (Keyboard.IsDown(pos) ? 1 : 0) + (Keyboard.IsDown(neg) ? -1 : 0);
            }
            else if (VolInputDevice == InputDevice.Mouse)
            {
                float sens = Application.GameConfig.GetFloat(GameConfigKey.Mouse_Sensitivity);
                int axisIndex = Application.GameConfig.GetInt(GameConfigKey.Mouse_Laser0Axis + (int)axis);
                
                if (axisIndex == 0) return Mouse.DeltaX * sens;
                if (axisIndex == 1) return Mouse.DeltaY * sens;
            }

            // ??
            return 0.0f;
        }

        private void Gamepad_Connect(int deviceIndex)
        {
            if (deviceIndex != gamepadIndex) return;
            Logger.Log($"GameInput device connect ({ gamepadIndex }).");
            if (gamepad == null)
                gamepad = Gamepad.Open(gamepadIndex);
        }

        private void Gamepad_Disconnect(int deviceIndex)
        {
            if (deviceIndex != gamepadIndex) return;
            Logger.Log($"GameInput device disconnect ({ gamepadIndex }).");
            gamepad = null;
        }

        protected override void DisposeManaged()
        {
            gamepad.Dispose();
            gamepad = null;

            Gamepad.Connect -= Gamepad_Connect;
            Gamepad.Disconnect -= Gamepad_Disconnect;
        }
    }
}
