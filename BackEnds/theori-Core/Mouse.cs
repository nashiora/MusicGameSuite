using System;
using System.Numerics;

namespace theori
{
    public static class Mouse
    {
        internal static int x, y;
        internal static int dx, dy;
        internal static int sx, sy;

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

        internal static void InvokePress(MouseButton button)
        {
            ButtonPress?.Invoke(button);
        }

        internal static void InvokeRelease(MouseButton button)
        {
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
