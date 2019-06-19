using System;
using System.Collections.Generic;

namespace theori.IO
{
    public static class Keyboard
    {
	    internal const int SCANCODE_MASK = 1 << 30;
        
        private static readonly HashSet<KeyCode> lastHeldKeys = new HashSet<KeyCode>();
        private static readonly HashSet<KeyCode> heldKeys = new HashSet<KeyCode>();
        
        public static event Action<KeyInfo> KeyPress;
        public static event Action<KeyInfo> KeyRelease;

        internal static void Update()
        {
            lastHeldKeys.Clear();
            foreach (var button in heldKeys)
                lastHeldKeys.Add(button);
        }

	    public static KeyCode ToKeyCode(ScanCode code)
	    {
		    return (KeyCode)((int)code | SCANCODE_MASK);
	    }
        
        public static bool IsDown(KeyCode key) => heldKeys.Contains(key);
        public static bool IsUp(KeyCode key) => !heldKeys.Contains(key);
        
        public static bool IsPressed(KeyCode key) => heldKeys.Contains(key) && !lastHeldKeys.Contains(key);
        public static bool IsReleased(KeyCode key) => !heldKeys.Contains(key) && lastHeldKeys.Contains(key);

        internal static void InvokePress(KeyInfo info)
        {
            //System.Diagnostics.Debug.Assert(heldKeys.Add(info.KeyCode), "added a key which was pressed");
            if (!heldKeys.Add(info.KeyCode)) return;

            Host.KeyPressed(info);
            KeyPress?.Invoke(info);
        }

        internal static void InvokeRelease(KeyInfo info)
        {
            bool removed = heldKeys.Remove(info.KeyCode);
            System.Diagnostics.Debug.Assert(removed, "removed a key which wasn't pressed");

            Host.KeyReleased(info);
            KeyRelease?.Invoke(info);
        }
    }
}
