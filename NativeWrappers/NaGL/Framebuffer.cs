namespace OpenGL
{
    public sealed class Framebuffer : UIntHandle
    {
        public static void Unbind()
        {
            GL.BindFramebufferEXT(GL.GL_FRAMEBUFFER_EXT, 0);
        }

        public Texture Texture
        {
            set
            {
                Bind();
                GL.FramebufferTexture(GL.GL_FRAMEBUFFER_EXT, GL.GL_COLOR_ATTACHMENT0_EXT, value.Handle, 0);
                GL.DrawBuffers(1, new[] { GL.GL_COLOR_ATTACHMENT0_EXT });
                GL.Viewport(0, 0, value.Width, value.Height);
            }
        }

        public uint Status => GL.CheckFramebufferStatusEXT(Handle);

        public Framebuffer()
            : base(GL.GenFramebufferEXT, GL.DeleteFramebufferEXT)
        {
        }

        public void Bind()
        {
            GL.BindFramebufferEXT(GL.GL_FRAMEBUFFER_EXT, Handle);
        }
    }
}
