using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using OpenGL;
using static OpenGL.GL;

namespace theori.Graphics
{
    public struct ShaderCreateInfo
    {
        public string VertexShader;
        public string GeometryShader;
        public string FragmentShader;
    }

    public class ShaderBase : IDisposable
    {

        #region IDisposable Support

        private bool isDisposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                }
                
                //program.Delete();

                isDisposed = true;
            }
        }

        ~ShaderBase()
        {
            //Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
