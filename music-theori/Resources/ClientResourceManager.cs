using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
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

                m_resourceManager.m_resources[m_resourcePath] = m_resultTexture;

                m_bitmap.Dispose();
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
            var resultTexture = Texture.CreateUninitialized2D();
            m_loaders.Add(new AsyncTextureLoader(this, resourcePath, resultTexture));
            return resultTexture;
        }

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

        public AudioTrack AquireAudioTrack(string resourcePath) => Aquire(resourcePath, LoadRawAudioTrack);
        public AudioSample AquireAudioSample(string resourcePath) => Aquire(resourcePath, LoadRawAudioSample);
        public Texture AquireTexture(string resourcePath) => Aquire(resourcePath, LoadRawTexture);
        public Material AquireMaterial(string resourcePath) => Aquire(resourcePath, LoadRawMaterial);

        public AudioTrack LoadRawAudioTrack(string resourcePath)
        {
            var audioStream = m_locator.OpenAudioStream(resourcePath, out string fileExtension);
            if (audioStream == null)
                throw new ArgumentException($"Could not find the specified audio track resource \"{ resourcePath }\".", nameof(resourcePath));

            // audio tracks own their stream
            return AudioTrack.FromStream(fileExtension, audioStream);
        }

        public AudioSample LoadRawAudioSample(string resourcePath)
        {
            var audioStream = m_locator.OpenAudioStream(resourcePath, out string fileExtension);
            if (audioStream == null)
                throw new ArgumentException($"Could not find the specified audio sample resource \"{ resourcePath }\".", nameof(resourcePath));

            // audio samples own their stream
            return AudioSample.FromStream(fileExtension, audioStream);
        }

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
