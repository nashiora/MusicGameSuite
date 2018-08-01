namespace OpenGL
{
    public sealed class ProgramPipeline : UIntHandle
    {
        public ProgramPipeline()
            : base(GL.GenProgramPipeline, GL.DeleteProgramPipeline)
        {
        }

        public void Bind() => GL.BindProgramPipeline(Handle);
    }
}
