using OpenGL;

namespace theori.Graphics
{
    public class Drawable3D
    {
        public Mesh Mesh;
        public Texture Texture;

        public Material Material;
        public MaterialParams Params = new MaterialParams();

        public void DrawToQueue(RenderQueue queue, Transform transform)
        {
            Params["MainTexture"] = Texture;
            queue.Draw(transform, Mesh, Material, Params);
        }
    }
}
