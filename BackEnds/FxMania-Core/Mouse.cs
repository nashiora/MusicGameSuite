using System;
using System.Numerics;

namespace FxMania
{
    public static class Mouse
    {
        internal static int x, y;

        public static int X
        {
            get => x;
        }

        public static int Y
        {
            get => y;
        }

        public static Vector2 Position
        {
            get => new Vector2(x, y);
        }
        
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
            Move?.Invoke(mx, my);
        }

        internal static void InvokeScroll(int x, int y)
        {
            Scroll?.Invoke(x, y);
        }
    }
}
