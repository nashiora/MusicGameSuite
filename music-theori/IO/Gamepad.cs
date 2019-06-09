using System;
using System.Collections.Generic;
using static SDL2.SDL;

namespace theori.IO
{
    public class ButtonInfo
    {
        public int DeviceIndex;

        public uint Button;
    }

    public class AnalogInfo
    {
        public int DeviceIndex;

        public uint Axis;
        public float Value;
    }

    public class Gamepad : Disposable
    {
        public static bool operator true (Gamepad g) => g.joystick != IntPtr.Zero;
        public static bool operator false(Gamepad g) => g.joystick == IntPtr.Zero;

        private static readonly Dictionary<int, Gamepad> openGamepads = new Dictionary<int, Gamepad>();
        
        public static event Action<int> Connect;
        public static event Action<int> Disconnect;

        internal static void Destroy()
        {
            openGamepads.Clear();
        }

        public static int NumConnected => SDL_NumJoysticks();

        public static string NameOf(int deviceIndex)
        {
            return SDL_JoystickNameForIndex(deviceIndex);
        }

        public static Gamepad Open(int deviceIndex)
        {
            if (!openGamepads.TryGetValue(deviceIndex, out var gamepad))
            {
                gamepad = new Gamepad(deviceIndex);
                if (gamepad)
                {
                    openGamepads[gamepad.InstanceId] = gamepad;
                    SDL_JoystickEventState(SDL_ENABLE);
                }
            }
            return gamepad;
        }

        internal static void HandleAddedEvent(int deviceIndex)
        {
            string name = SDL_JoystickNameForIndex(deviceIndex);
            Logger.Log($"Joystick Added: Device [{ deviceIndex }] \"{ name }\"", LogPriority.Verbose);

            Connect?.Invoke(deviceIndex);
        }

        internal static void HandleRemovedEvent(int instanceId)
        {
            Logger.Log($"Joystick Removed: Instance [{ instanceId }]", LogPriority.Verbose);
            
            Disconnect?.Invoke(instanceId);
            openGamepads.Remove(instanceId);
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
        public readonly int InstanceId;

        private IntPtr joystick;
        
        private readonly uint[] buttonStates;
        private readonly float[] axisStates;

        public int ButtonCount => buttonStates.Length;
        public int AxisCount => axisStates.Length;

        public string DeviceName => SDL_JoystickName(joystick);

        private Gamepad(int deviceIndex)
        {
            joystick = SDL_JoystickOpen(deviceIndex);
            DeviceIndex = deviceIndex;

            if (joystick == IntPtr.Zero)
            {
                InstanceId = 0;
                Logger.Log($"Failed to open joystick { deviceIndex }.");
            }
            else
            {
                InstanceId = SDL_JoystickInstanceID(joystick);

                buttonStates = new uint[SDL_JoystickNumButtons(joystick)];
                axisStates = new float[SDL_JoystickNumAxes(joystick)];
                
                Logger.Log($"Opened joystick { deviceIndex } ({ DeviceName }) with { ButtonCount } buttons and { AxisCount } axes.");
            }
        }

        public void Close()
        {
            if (joystick != IntPtr.Zero) SDL_JoystickClose(joystick);
            joystick = IntPtr.Zero;

            openGamepads.Remove(DeviceIndex);
        }

        protected override void DisposeUnmanaged()
        {
            // TODO(local): throws access violation all the fkin time, but it's only at exit so it's like fine?
            //Close();
        }

        public bool ButtonDown(uint buttonIndex) => buttonStates[buttonIndex] != 0;
        public float GetAxis(uint axisIndex) => axisStates[axisIndex];

        internal void HandleInputEvent(uint buttonIndex, uint newState)
        {
            var info = new ButtonInfo()
            {
                DeviceIndex = DeviceIndex,
                Button = buttonIndex,
            };

            buttonStates[buttonIndex] = newState;
            if (newState == 1)
            {
                Host.ButtonPressed(info);
                ButtonReleased?.Invoke(buttonIndex);
            }
            else
            {
                Host.ButtonReleased(info);
                ButtonPressed?.Invoke(buttonIndex);
            }
        }

        internal void HandleAxisEvent(uint axisIndex, short newValue)
        {
            float value = newValue / (float)0x7FFF;
            axisStates[axisIndex] = value;
            Host.AxisChanged(new AnalogInfo()
            {
                DeviceIndex = DeviceIndex,
                Axis = axisIndex,
                Value = value,
            });
        }
    }
}
