using System;
using System.Collections.Generic;
using System.Numerics;
using static SDL2.SDL;

namespace theori.IO
{
    public static class Mouse
    {
        internal static int x, y;
        internal static int dx, dy;
        internal static int sx, sy;

        internal static bool relative;

        public static int X => x;
        public static int Y => y;

        public static int DeltaX => dx;
        public static int DeltaY => dy;

        public static Vector2 Position => new Vector2(x, y);
        public static Vector2 Delta => new Vector2(dx, dy);
        
        public static event Action<MouseButton> ButtonPress;
        public static event Action<MouseButton> ButtonRelease;
        
        public static event Action<int, int> Move;
        public static event Action<int, int> Scroll;
        
        private static readonly HashSet<MouseButton> lastHeldButtons = new HashSet<MouseButton>();
        private static readonly HashSet<MouseButton> heldButtons = new HashSet<MouseButton>();

        public static bool Relative
        {
            get => relative;
            set => SDL_SetRelativeMouseMode((relative = value) ? SDL_bool.SDL_TRUE : SDL_bool.SDL_FALSE);
        }

        internal static void Update()
        {
            lastHeldButtons.Clear();
            foreach (var button in heldButtons)
                lastHeldButtons.Add(button);
        }
        
        public static bool IsDown(MouseButton button) => heldButtons.Contains(button);
        public static bool IsUp(MouseButton button) => !heldButtons.Contains(button);
        
        public static bool IsPressed(MouseButton button) => heldButtons.Contains(button) && !lastHeldButtons.Contains(button);
        public static bool IsReleased(MouseButton button) => !heldButtons.Contains(button) && lastHeldButtons.Contains(button);

        internal static void InvokePress(MouseButton button)
        {
            System.Diagnostics.Debug.Assert(heldButtons.Add(button), "added a button which was pressed");
            ButtonPress?.Invoke(button);
        }

        internal static void InvokeRelease(MouseButton button)
        {
            System.Diagnostics.Debug.Assert(heldButtons.Remove(button), "removed a button which wasn't pressed");
            ButtonRelease?.Invoke(button);
        }

        internal static void InvokeMove(int mx, int my)
        {
            dx = mx - x; dy = my - y;
            Move?.Invoke(x = mx, y = my);
        }

        internal static void InvokeScroll(int x, int y)
        {
            Scroll?.Invoke(sx = x, sy = y);
        }
    }
}
