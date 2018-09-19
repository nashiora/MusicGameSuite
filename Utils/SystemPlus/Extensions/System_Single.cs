using System.Runtime.CompilerServices;

namespace System
{
    public static class System_Single_Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxEq(this float a, float b, float eps = 1e-20f)
        {
            return Math.Abs(a - b) < eps;
        }
    }
}
