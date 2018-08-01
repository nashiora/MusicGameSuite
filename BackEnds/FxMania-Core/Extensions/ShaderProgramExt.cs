using System.Numerics;

namespace OpenGL
{
    public static class ShaderProgramExt
    {
        private static readonly float[] matrix = new float[16];

        public static void SetUniform(this ShaderProgram s, string name, Matrix4x4 m, bool transpose = false)
        {
            m.CopyTo(matrix);
            s.SetUniformMatrix4(name, 1, transpose, matrix);
        }

        public static void SetUniform(this ShaderProgram s, int location, Matrix4x4 m, bool transpose = false)
        {
            m.CopyTo(matrix);
            s.SetUniformMatrix4(location, 1, transpose, matrix);
        }

        public static void SetUniform(this ShaderProgram s, string  name, Vector2 v) => s.SetUniform(name    , v.X, v.Y);
        public static void SetUniform(this ShaderProgram s, int location, Vector2 v) => s.SetUniform(location, v.X, v.Y);

        public static void SetUniform(this ShaderProgram s, string  name, Vector3 v) => s.SetUniform(name    , v.X, v.Y, v.Z);
        public static void SetUniform(this ShaderProgram s, int location, Vector3 v) => s.SetUniform(location, v.X, v.Y, v.Z);

        public static void SetUniform(this ShaderProgram s, string  name, Vector4 v) => s.SetUniform(name    , v.X, v.Y, v.Z, v.W);
        public static void SetUniform(this ShaderProgram s, int location, Vector4 v) => s.SetUniform(location, v.X, v.Y, v.Z, v.W);
    }
}
