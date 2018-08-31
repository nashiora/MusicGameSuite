using System;
using System.Collections.Generic;

namespace theori
{
    public static class Keyboard
    {
	    internal const int SCANCODE_MASK = 1 << 30;

        private static readonly HashSet<KeyCode> heldKeys = new HashSet<KeyCode>();
        
        public static event Action<KeyInfo> KeyPress;
        public static event Action<KeyInfo> KeyRelease;

	    public static KeyCode ToKeyCode(ScanCode code)
	    {
		    return (KeyCode)((int)code | SCANCODE_MASK);
	    }
        
        public static bool IsDown(KeyCode key) => heldKeys.Contains(key);
        public static bool IsUp(KeyCode key) => !heldKeys.Contains(key);

        internal static void InvokePress(KeyInfo info)
        {
            System.Diagnostics.Debug.Assert(heldKeys.Add(info.KeyCode), "added a key which was pressed");
            KeyPress?.Invoke(info);
        }

        internal static void InvokeRelease(KeyInfo info)
        {
            System.Diagnostics.Debug.Assert(heldKeys.Remove(info.KeyCode), "removed a key which wasn't pressed");
            KeyRelease?.Invoke(info);
        }
    }
}
