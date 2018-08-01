namespace System
{
    public static unsafe class Reinterpret
    {
        public static int  CastToInt (float from) => *( int*)&from;
        public static uint CastToUInt(float from) => *(uint*)&from;
        
        public static long  CastToLong (double from) => *( long*)&from;
        public static ulong CastToULong(double from) => *(ulong*)&from;

        public static float CastToFloat(int  from) => *(float*)&from;
        public static float CastToFloat(uint from) => *(float*)&from;
        
        public static double CastToDouble(long  from) => *(double*)&from;
        public static double CastToDouble(ulong from) => *(double*)&from;
    }
}
