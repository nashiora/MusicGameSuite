using theori.Configuration;

namespace theori.Input
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

            int deviceIndex = Host.GameConfig.GetInt(GameConfigKey.Controller_DeviceID);
            Gamepad = Gamepad.Open(deviceIndex);
        }

        public static void Clear()
        {
        }
    }
}
