using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using OpenGL;

using theori.Audio;
using theori.Graphics;

namespace theori.Resources
{
    public sealed class ClientResourceManager : Disposable
    {
        abstract class AsyncResourceLoader
        {
            protected readonly ClientResourceManager m_resourceManager;
            protected readonly string m_resourcePath;

            protected AsyncResourceLoader(ClientResourceManager resourceManager, string resourcePath)
            {
                m_resourceManager = resourceManager;
                m_resourcePath = resourcePath;
            }

            /// <summary>
            /// Started in a separate thread to load the necessary data
            /// </summary>
            public abstract bool Load();
            /// <summary>
            /// Started on the main thread to finalize the loaded data
            /// </summary>
            /// <returns></returns>
            public abstract bool Finalize();
        }

        sealed class AsyncTextureLoader : AsyncResourceLoader
        {
            private readonly Texture m_resultTexture;
            private Bitmap m_bitmap;

            public AsyncTextureLoader(ClientResourceManager resourceManager, string resourcePath, Texture resultTexture)
                : base(resourceManager, resourcePath)
            {
                m_resultTexture = resultTexture;
            }

            public override bool Load()
            {
                var textureStream = m_resourceManager.m_locator.OpenTextureStream(m_resourcePath, out string fileExtension);
                if (textureStream == null)
                    return false;

                using (textureStream)
                    m_bitmap = new Bitmap(Image.FromStream(textureStream));
                return true;
            }

            public override bool Finalize()
            {
                m_resultTexture.GenerateHandle();
                m_resultTexture.Create2DFromBitmap(m_bitmap);

                //m_resourceManager.m_resources[m_resourcePath] = m_resultTexture;

                m_bitmap.Dispose();
                return true;
            }
        }

        sealed class AsyncMaterialLoader : AsyncResourceLoader
        {
            private readonly Material m_resultMaterial;
            private readonly string[] m_sources = new string[3];

            public AsyncMaterialLoader(ClientResourceManager resourceManager, string resourcePath, Material resultMaterial)
                : base(resourceManager, resourcePath)
            {
                m_resultMaterial = resultMaterial;
            }

            public override bool Load()
            {
                // materials do NOT own their stream
                using (var vertexStream = m_resourceManager.m_locator.OpenShaderStream(m_resourcePath, ".vs", out bool missingVertex))
                using (var fragmentStream = m_resourceManager.m_locator.OpenShaderStream(m_resourcePath, ".fs", out bool missingFragment))
                using (var geometryStream = m_resourceManager.m_locator.OpenShaderStream(m_resourcePath, ".gs", out bool missingGeometry))
                {
                    if ((missingVertex || vertexStream == null) && (missingFragment || fragmentStream == null))
                        return false;

                    m_sources[0] = new StreamReader(vertexStream).ReadToEnd();
                    m_sources[1] = new StreamReader(fragmentStream).ReadToEnd();
                    if (geometryStream != null)
                        m_sources[2] = new StreamReader(geometryStream).ReadToEnd();
                }

                return true;
            }

            public override bool Finalize()
            {
                m_resultMaterial.CreatePipeline();

                bool CreateShader(ShaderType type, int sourceIndex)
                {
                    var program = new ShaderProgram(type, m_sources[sourceIndex]);
                    if (!program || !program.Linked)
                        return false;
                    m_resultMaterial.AssignShader(program);
                    return true;
                }

                if (!CreateShader(ShaderType.Vertex, 0)) return false;
                if (!CreateShader(ShaderType.Fragment, 1)) return false;

                if (m_sources[2] != null)
                {
                    if (!CreateShader(ShaderType.Geometry, 2))
                        return false;
                }

                return true;
            }
        }

        sealed class AsyncAudioLoader<T> : AsyncResourceLoader
            where T : AudioTrack
        {
            private readonly T m_resultAudio;

            public AsyncAudioLoader(ClientResourceManager resourceManager, string resourcePath, T resultAudio)
                : base(resourceManager, resourcePath)
            {
                m_resultAudio = resultAudio;
            }

            public override bool Load()
            {
                var stream = m_resourceManager.m_locator.OpenAudioStream(m_resourcePath, out string fileExtension);
                if (stream == null) return false;

                m_resultAudio.SetSourceFromStream(stream, fileExtension);
                return true;
            }

            public override bool Finalize()
            {
                // all the work is already done, no main-thread specific stuff so we do it all up front
                return true;
            }
        }

        private readonly ClientResourceLocator m_locator;

        private readonly List<AsyncResourceLoader> m_loaders = new List<AsyncResourceLoader>();
        private readonly Dictionary<string, Disposable> m_resources = new Dictionary<string, Disposable>();

        public ClientResourceManager(ClientResourceLocator locator)
        {
            m_locator = locator;
        }

        protected override void DisposeManaged()
        {
            foreach (var resource in m_resources.Values)
                resource.Dispose();
            m_resources.Clear();
        }

        public Texture QueueTextureLoad(string resourcePath)
        {
            if (m_resources.TryGetValue(resourcePath, out var resource))
                return resource as Texture;

            var resultTexture = Texture.CreateUninitialized2D();
            m_resources[resourcePath] = resultTexture;

            m_loaders.Add(new AsyncTextureLoader(this, resourcePath, resultTexture));
            return resultTexture;
        }

        public Material QueueMaterialLoad(string resourcePath)
        {
            if (m_resources.TryGetValue(resourcePath, out var resource))
                return resource as Material;

            var resultMaterial = Material.CreateUninitialized();
            m_resources[resourcePath] = resultMaterial;

            m_loaders.Add(new AsyncMaterialLoader(this, resourcePath, resultMaterial));
            return resultMaterial;
        }

        private T QueueAudioLoad<T>(string resourcePath, T resultAudio)
            where T : AudioTrack
        {
            if (m_resources.TryGetValue(resourcePath, out var resource))
                return resource as T;

            m_resources[resourcePath] = resultAudio;

            m_loaders.Add(new AsyncAudioLoader<T>(this, resourcePath, resultAudio));
            return resultAudio;
        }

        public AudioTrack QueueAudioTrackLoad(string resourcePath) =>
            QueueAudioLoad(resourcePath, AudioTrack.CreateUninitialized());

        public AudioSample QueueAudioSampleLoad(string resourcePath) =>
            QueueAudioLoad(resourcePath, AudioSample.CreateUninitialized());

        public bool LoadAll()
        {
            bool success = true;
            foreach (var loader in m_loaders)
            {
                if (!loader.Load())
                {
                    success = false;
                }
            }
            return success;
        }

        public bool FinalizeLoad()
        {
            bool success = true;
            foreach (var loader in m_loaders)
            {
                if (!loader.Finalize())
                {
                    success = false;
                }
            }
            m_loaders.Clear();
            return success;
        }

        public Texture GetTexture(string resourcePath)
        {
            if (!m_resources.TryGetValue(resourcePath, out var resource) || resource.IsDisposed)
                return null;
            return resource as Texture;
        }

        public Material GetMaterial(string resourcePath)
        {
            if (!m_resources.TryGetValue(resourcePath, out var resource) || resource.IsDisposed)
                return null;
            return resource as Material;
        }

        private T GetAudio<T>(string resourcePath)
            where T : AudioTrack
        {
            if (!m_resources.TryGetValue(resourcePath, out var resource) || resource.IsDisposed)
                return null;
            return resource as T;
        }

        public AudioTrack GetAudioTrack(string resourcePath) => GetAudio<AudioTrack>(resourcePath);
        public AudioSample GetAudioSample(string resourcePath) => GetAudio<AudioSample>(resourcePath);

        private T Aquire<T>(string resourcePath, Func<string, T> loader)
            where T : Disposable
        {
            // read: if it doesn't yet exist or has already been disposed then recreate it
            //if (!m_resources.TryGetValue(resourcePath, out var handle) || !handle.TryGetTarget(out var resource) || resource.IsDisposed)
            if (!m_resources.TryGetValue(resourcePath, out var resource) || resource.IsDisposed)
            {
                resource = loader(resourcePath);
                //m_resources[resourcePath] = new WeakReference<Disposable>(resource);
                m_resources[resourcePath] = resource;
            }

            return resource as T;
        }

        public Texture AquireTexture(string resourcePath) => Aquire(resourcePath, LoadRawTexture);
        public Material AquireMaterial(string resourcePath) => Aquire(resourcePath, LoadRawMaterial);

        public Texture LoadRawTexture(string resourcePath)
        {
            var textureStream = m_locator.OpenTextureStream(resourcePath, out string fileExtension);
            if (textureStream == null)
                throw new ArgumentException($"Could not find the specified texture resource \"{ resourcePath }\".", nameof(resourcePath));

            // textures do NOT own their stream
            using (textureStream)
                return Texture.FromStream2D(textureStream);
        }

        public Material LoadRawMaterial(string resourcePath)
        {
            // materials do NOT own their stream
            using (var vertexStream = m_locator.OpenShaderStream(resourcePath, ".vs", out bool missingVertex))
            using (var fragmentStream = m_locator.OpenShaderStream(resourcePath, ".fs", out bool missingFragment))
            using (var geometryStream = m_locator.OpenShaderStream(resourcePath, ".gs", out bool missingGeometry))
            {
                if ((missingVertex || vertexStream == null) && (missingFragment || fragmentStream == null))
                    throw new ArgumentException($"Could not find the specified material resource \"{ resourcePath }\".", nameof(resourcePath));

                return new Material(vertexStream, fragmentStream, geometryStream);
            }
        }
    }
}
