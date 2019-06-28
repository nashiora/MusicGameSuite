using System;
using System.Collections.Generic;
using OpenGL;

namespace theori.Graphics
{
    public class RenderQueue : Disposable
    {
        protected readonly RenderState m_state;
        protected readonly List<SimpleDrawCall> m_orderedCommands = new List<SimpleDrawCall>(50);

        public RenderQueue(RenderState state)
        {
            m_state = state;
        }

        public virtual void Process(bool clear)
        {
		    bool scissorEnabled = false;
		    bool blendEnabled = false;
		    var activeBlendMode = (BlendMode)(-1);

		    var initializedShaders = new HashSet<Material>();
		    Material currentMaterial = null;
		    Mesh currentMesh = null;
            
            // TODO(local): For the time being, the only things using this
            //  are rendered in a pre-determined order and REQUIRE there to be no depth testing.
            // In future, this should be configurable!
            //GL.Enable(GL.GL_DEPTH_TEST);
            //GL.DepthFunc(DepthFunction.LessThanOrEqual);

            foreach (var item in m_orderedCommands)
            {
                void SetUpMaterial(Material mat, MaterialParams p)
                {
                    if (currentMaterial == mat)
                        mat.ApplyParams(p, m_state.WorldTransform);
                    else
                    {
                        if (initializedShaders.Contains(mat))
                        {
                            mat.ApplyParams(p, m_state.WorldTransform);
                            mat.BindToContext();
                        }
                        else
                        {
                            mat.Bind(m_state, p);
                            initializedShaders.Add(mat);
                        }
                        currentMaterial = mat;
                    }

                    if (mat.Opaque)
                    {
					    if (blendEnabled)
					    {
						    GL.Disable(GL.GL_BLEND);
						    blendEnabled = false;
					    }
                    }
                    else
                    {
					    if (!blendEnabled)
					    {
						    GL.Enable(GL.GL_BLEND);
						    blendEnabled = true;
					    }

					    if (activeBlendMode != mat.BlendMode)
					    {
						    switch(activeBlendMode = mat.BlendMode)
						    {
						        case BlendMode.Normal: GL.BlendFuncSeparate(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA, GL.GL_ONE, GL.GL_ONE); break;
						        case BlendMode.Additive: GL.BlendFunc(GL.GL_ONE, GL.GL_ONE); break;
						        case BlendMode.Multiply: GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_SRC_COLOR); break;
						    }
					    }
                    }
                }

                void DrawMesh(Mesh mesh)
                {
                    if (currentMesh == mesh)
                        mesh.Redraw();
                    else
                    {
                        mesh.Draw();
                        currentMesh = mesh;
                    }
                }

                if (item is SimpleDrawCall sdc)
                {
                    m_state.WorldTransform = sdc.WorldTransform;
                    SetUpMaterial(sdc.Material, sdc.Params);

                    bool useScissor = sdc.Scissor.Width >= 0;
                    if (useScissor)
                    {
                        if (!scissorEnabled)
                        {
                            GL.Enable(GL.GL_SCISSOR_TEST);
                            scissorEnabled = true;
                        }

                        float scissorY = m_state.ViewportSize.Y - sdc.Scissor.Bottom;
                        GL.Scissor((int)sdc.Scissor.Left, (int)scissorY,
                                   (int)sdc.Scissor.Width, (int)sdc.Scissor.Height);
                    }
                    else
                    {
                        if (scissorEnabled)
                        {
                            GL.Disable(GL.GL_SCISSOR_TEST);
                            scissorEnabled = false;
                        }
                    }

                    DrawMesh(sdc.Mesh);
                }
            }
            
            GL.Disable(GL.GL_BLEND);
            GL.Disable(GL.GL_SCISSOR_TEST);
            //GL.Disable(GL.GL_DEPTH_TEST);

            if (clear) m_orderedCommands.Clear();
        }

        public virtual void Draw(Transform world, Mesh mesh, Material mat, MaterialParams p)
        {
            var sdc = new SimpleDrawCall()
            {
                Mesh = mesh,
                Material = mat,
                Params = p,
                WorldTransform = world,
                Scissor = Rect.EmptyScissor,
            };
            m_orderedCommands.Add(sdc);
        }

        public virtual void Draw(Rect scissor, Transform world, Mesh mesh, Material mat, MaterialParams p)
        {
            var sdc = new SimpleDrawCall()
            {
                Mesh = mesh,
                Material = mat,
                Params = p,
                WorldTransform = world,
                Scissor = scissor,
            };
            m_orderedCommands.Add(sdc);
        }

        protected override void DisposeManaged()
        {
            Process(true);
        }
    }

    public class SimpleDrawCall
    {
        public Mesh Mesh;
        public Material Material;
        public MaterialParams Params;

        public Transform WorldTransform;
        public Rect Scissor;
    }
}
