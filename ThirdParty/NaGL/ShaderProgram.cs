using System.Text;

namespace OpenGL
{
    public struct UniformInfo
    {
        public int Size;
        public GLType Type;
        public string Name;
        public int Location;
    }

    public sealed class ShaderProgram : UIntHandle
    {
        public readonly ShaderStage Stage;

        public string InfoLog => GL.GetProgramInfoLog(Handle);
        public bool Linked => GL.GetProgram(Handle, GL.GL_LINK_STATUS) != 0;

        public UniformInfo[] ActiveUniforms
        {
            get
            {
                int count = GL.GetProgram(Handle, GL.GL_ACTIVE_UNIFORMS);
                var result = new UniformInfo[count];

                var nameBuilder = new StringBuilder(64);
                for (int i = 0; i < count; i++)
                {
                    nameBuilder.Clear();
                    GL.GetActiveUniform(Handle, (uint)i, out int nameLen, out int size, out GLType type, nameBuilder);
                    string name = nameBuilder.ToString();
                    int loc = GL.GetUniformLocation(Handle, name);

                    result[i] = new UniformInfo()
                    {
                        Size = size,
                        Type = type,
                        Name = name,
                        Location = loc,
                    };
                }

                return result;
            }
        }

        public ShaderProgram(ShaderType shader, string source)
            : base(() => GL.CreateShaderProgram((uint)shader, source),
                   GL.DeleteProgram)
        {
            switch (shader)
            {
                case ShaderType.Vertex: Stage = ShaderStage.Vertex; break;
                case ShaderType.Fragment: Stage = ShaderStage.Fragment; break;
                case ShaderType.Geometry: Stage = ShaderStage.Geometry; break;
            }
        }

        public void Use(ProgramPipeline pipeline)
        {
            GL.UseProgramStages(pipeline.Handle, (uint)Stage, Handle);
        }

        public void SetUniform(string name, int value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniform1(Handle, location, value);
        }

        public void SetUniform(int location, int value)
        {
            GL.ProgramUniform1(Handle, location, value);
        }

        public void SetUniform(string name, float value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniform1(Handle, location, value);
        }

        public void SetUniform(int location, float value)
        {
            GL.ProgramUniform1(Handle, location, value);
        }

        public void SetUniform(string name, float v0, float v1)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniform2(Handle, location, v0, v1);
        }

        public void SetUniform(int location, float v0, float v1)
        {
            GL.ProgramUniform2(Handle, location, v0, v1);
        }

        public void SetUniform(string name, float v0, float v1, float v2)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniform3(Handle, location, v0, v1, v2);
        }

        public void SetUniform(int location, float v0, float v1, float v2)
        {
            GL.ProgramUniform3(Handle, location, v0, v1, v2);
        }

        public void SetUniform(string name, float v0, float v1, float v2, float v3)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniform4(Handle, location, v0, v1, v2, v3);
        }

        public void SetUniform(int location, float v0, float v1, float v2, float v3)
        {
            GL.ProgramUniform4(Handle, location, v0, v1, v2, v3);
        }

        public void SetUniformMatrix4(string name, int count, bool transpose, float[] data)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.ProgramUniformMatrix4(Handle, location, count, transpose, data);
        }

        public void SetUniformMatrix4(int location, int count, bool transpose, float[] data)
        {
            GL.ProgramUniformMatrix4(Handle, location, count, transpose, data);
        }
    }
}
