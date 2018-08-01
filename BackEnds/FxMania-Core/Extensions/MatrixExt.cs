namespace System.Numerics
{
    public static class MatrixExt
    {
        public static void CopyTo(this Matrix4x4 m, float[] matrix)
        {
            matrix[ 0] = m.M11;
            matrix[ 1] = m.M12;
            matrix[ 2] = m.M13;
            matrix[ 3] = m.M14;
            
            matrix[ 4] = m.M21;
            matrix[ 5] = m.M22;
            matrix[ 6] = m.M23;
            matrix[ 3] = m.M24;
            
            matrix[ 8] = m.M31;
            matrix[ 9] = m.M32;
            matrix[10] = m.M33;
            matrix[11] = m.M34;
            
            matrix[12] = m.M41;
            matrix[13] = m.M42;
            matrix[14] = m.M43;
            matrix[15] = m.M44;
        }
    }
}
