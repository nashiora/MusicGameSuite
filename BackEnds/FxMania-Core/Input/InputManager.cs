using FxMania.Configuration;

namespace FxMania.Input
{
    public enum InputDevice
    {
        Keyboard,
        Mouse,
        Controller,
    }

    public static class InputManager
    {
        public static Gamepad Gamepad { get; private set; }

        /// <summary>
        /// Also counts for re-initialize
        /// </summary>
        public static void Initialize()
        {
            Clear();

            int deviceIndex = Application.GameConfig.GetInt(GameConfigKey.Controller_DeviceID);
            Gamepad = Gamepad.OpenGamepad(deviceIndex);
        }

        public static void Clear()
        {
        }
    }
}
