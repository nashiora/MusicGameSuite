using System.Runtime.CompilerServices;

namespace System
{
    public static class System_Double_Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxEq(this double a, double b, double eps = 1e-20)
        {
            return Math.Abs(a - b) < eps;
        }
    }
}
