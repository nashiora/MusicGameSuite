using System.Runtime.CompilerServices;

namespace System
{
    public static class NumericExt
    {
        #region Float

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ApproxEq(this float a, float b)
        {
            return Math.Abs(a - b) < float.Epsilon;
        }

        #endregion
    }
}
