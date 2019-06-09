using theori.Configuration;

namespace theori.IO
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

        public static void ReopenGamepad()
        {
            Gamepad?.Close();

            int deviceIndex = Host.GameConfig.GetInt(GameConfigKey.Controller_DeviceID);
            Gamepad = Gamepad.Open(deviceIndex);
        }

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
