using System;
using System.Numerics;

namespace theori
{
    public struct Transform
    {
        public static Transform Identity => new Transform(Matrix4x4.Identity);
        
        public static explicit operator Transform(Matrix4x4 m) => new Transform(m);
        public static explicit operator Matrix4x4(Transform t) => t.Matrix;

        public static Transform operator *(Transform a, Transform b) => new Transform(a.Matrix * b.Matrix);

        public static Vector4 operator *(Transform a, Vector4 b) => Vector4.Transform(b, a.Matrix);
        
        public static Transform Translation(float x, float y, float z) => new Transform(Matrix4x4.CreateTranslation(x, y, z));
        public static Transform Translation(Vector3 translation) => new Transform(Matrix4x4.CreateTranslation(translation));

        public static Transform RotationX(float xDeg) => new Transform(Matrix4x4.CreateRotationX(MathL.ToRadians(xDeg)));
        public static Transform RotationY(float yDeg) => new Transform(Matrix4x4.CreateRotationY(MathL.ToRadians(yDeg)));
        public static Transform RotationZ(float zDeg) => new Transform(Matrix4x4.CreateRotationZ(MathL.ToRadians(zDeg)));
        
        public static Transform Scale(float x, float y, float z) => new Transform(Matrix4x4.CreateScale(x, y, z));
        public static Transform Scale(Vector3 scale) => new Transform(Matrix4x4.CreateScale(scale));

        public readonly Matrix4x4 Matrix;

        public Transform(Matrix4x4 matrix)
        {
            this.Matrix = matrix;
        }

        public Transform(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            Matrix = Matrix4x4.CreateScale(scale) *
                     Matrix4x4.CreateFromQuaternion(rotation) *
                     Matrix4x4.CreateTranslation(translation);
        }
    }
}
