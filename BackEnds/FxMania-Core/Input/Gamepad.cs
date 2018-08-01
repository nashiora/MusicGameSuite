using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace FxMania.Input
{
    public class Gamepad : Disposable
    {
        public static bool operator true (Gamepad g) => g.joystick != IntPtr.Zero;
        public static bool operator false(Gamepad g) => g.joystick == IntPtr.Zero;

        private static readonly Dictionary<int, Gamepad> openGamepads = new Dictionary<int, Gamepad>();

        internal static void Destroy()
        {
            foreach (var g in openGamepads.Values)
                g.Dispose();
        }

        public static Gamepad OpenGamepad(int deviceIndex)
        {
            if (!openGamepads.TryGetValue(deviceIndex, out var gamepad))
            {
                gamepad = new Gamepad(deviceIndex);
                if (gamepad)
                {
                    openGamepads[deviceIndex] = gamepad;
                    SDL_JoystickEventState(SDL_ENABLE);
                }
            }
            return gamepad;
        }

        internal static void HandleInputEvent(int deviceIndex, uint buttonIndex, uint newState)
        {
            if (openGamepads.TryGetValue(deviceIndex, out var gamepad))
                gamepad.HandleInputEvent(buttonIndex, newState);
        }

        internal static void HandleAxisEvent(int deviceIndex, uint axisIndex, short newValue)
        {
            if (openGamepads.TryGetValue(deviceIndex, out var gamepad))
                gamepad.HandleAxisEvent(axisIndex, newValue);
        }
        
        public event Action<uint> ButtonPressed;
        public event Action<uint> ButtonReleased;
        
        public readonly int DeviceIndex;
        private IntPtr joystick;
        
        private readonly uint[] buttonStates;
        private readonly float[] axisStates;

        public int ButtonCount => buttonStates.Length;
        public int AxisCount => axisStates.Length;

        public string DeviceName => SDL_JoystickName(joystick);

        private Gamepad(int deviceIndex)
        {
            DeviceIndex = deviceIndex;
            joystick = SDL_JoystickOpen(deviceIndex);

            if (joystick == IntPtr.Zero)
                Logger.Log($"Failed to open joystick { deviceIndex }.");
            else
            {
                buttonStates = new uint[SDL_JoystickNumButtons(joystick)];
                axisStates = new float[SDL_JoystickNumAxes(joystick)];
                
                Logger.Log($"Opened joystick { deviceIndex } ({ DeviceName }) with { ButtonCount } buttons and { AxisCount } axes.");
            }
        }

        protected override void DisposeUnmanaged()
        {
            SDL_JoystickClose(joystick);
        }

        public bool ButtonDown(uint buttonIndex) => buttonStates[buttonIndex] != 0;
        public float GetAxis(uint axisIndex) => axisStates[axisIndex];

        internal void HandleInputEvent(uint buttonIndex, uint newState)
        {
            buttonStates[buttonIndex] = newState;
            if (newState == 0)
                ButtonReleased?.Invoke(buttonIndex);
            else ButtonPressed?.Invoke(buttonIndex);
        }

        internal void HandleAxisEvent(uint axisIndex, short newValue)
        {
            axisStates[axisIndex] = newValue / (float)0x7FFF;
        }
    }
}
