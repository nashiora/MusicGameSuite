using System;
using System.Runtime.InteropServices;

namespace OpenGL
{
    public static class Platform
    {
        static IPlatformLayer platform;

        public const string OpenGL32 = "opengl32.dll";

        static Platform()
        {
            platform = RuntimeInfo.IsWindows ? (IPlatformLayer)new Win32() : (IPlatformLayer)new Linux();
            IntPtr glLibrary = platform.LoadDynLib(OpenGL32);
        }

        public static IntPtr LoadDynLib(string name) => platform.LoadDynLib(name);
        public static IntPtr GlGetProcAddress(string name) => platform.GlGetProcAddress(name);
    }

    interface IPlatformLayer
    {
        IntPtr LoadDynLib(string name);
        IntPtr GlGetProcAddress(string name);
    }

    internal class Linux : IPlatformLayer
    {
        private const int RTLD_LAZY = 0x00001;
        private const int RTLD_NOW = 0x00002;

        [DllImport("libdl.so", SetLastError = true)]
        private static extern IntPtr dlopen(string filename, int flag);

        [DllImport("libEGL.so", SetLastError = true)]
        private static extern IntPtr eglGetProcAddress(string name);

        public IntPtr LoadDynLib(string name) => dlopen(name, RTLD_NOW);
        public IntPtr GlGetProcAddress(string name) => eglGetProcAddress(name);
    }

	public class Win32 : IPlatformLayer
    {
        private const string Kernel32 = "kernel32.dll";
        private const string OpenGL32 = "opengl32.dll";

        [DllImport(Kernel32, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(OpenGL32, SetLastError = true)]
        private static extern IntPtr wglGetProcAddress(string name);

        public IntPtr LoadDynLib(string name) => LoadLibrary(name);
        public IntPtr GlGetProcAddress(string name) => wglGetProcAddress(name);
    }
}
