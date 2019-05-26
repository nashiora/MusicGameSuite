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

        // Re-implementation of System.Numeric.Matrix4x4.CreatePerspectiveFieldOfView
        // literally fuck you Microsoft why doesn't this behave as expected fuck off
        public static Transform CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            if (fieldOfView <= 0.0f || fieldOfView >= Math.PI)
                throw new ArgumentOutOfRangeException("fieldOfView");

            if (nearPlaneDistance <= 0.0f)
                throw new ArgumentOutOfRangeException("nearPlaneDistance");

            if (farPlaneDistance <= 0.0f)
                throw new ArgumentOutOfRangeException("farPlaneDistance");

            if (nearPlaneDistance >= farPlaneDistance)
                throw new ArgumentOutOfRangeException("nearPlaneDistance");

            Matrix4x4 result;

            float yScale = 1.0f / (float)Math.Tan(fieldOfView * 0.5f);
            float xScale = yScale / aspectRatio;

            result.M11 = xScale;
            result.M12 = result.M13 = result.M14 = 0.0f;

            result.M22 = yScale;
            result.M21 = result.M23 = result.M24 = 0.0f;

            result.M31 = result.M32 = 0.0f;
            result.M33 = -(nearPlaneDistance + farPlaneDistance) / (farPlaneDistance - nearPlaneDistance);
            result.M34 = -1.0f;

            result.M41 = result.M42 = result.M44 = 0.0f;
            result.M43 = -(2 * nearPlaneDistance * farPlaneDistance) / (farPlaneDistance - nearPlaneDistance);

            return (Transform)result;
        }

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
