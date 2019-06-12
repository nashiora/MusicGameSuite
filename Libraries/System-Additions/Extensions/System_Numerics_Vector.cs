namespace System.Numerics
{
    public static class VectorExt
    {
        #region Swizzles

        public static Vector3 XYZ(this Vector4 v) => new Vector3(v.X, v.Y, v.Z);

        #endregion
    }
}
