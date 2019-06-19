using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using OpenGL;

using theori.Audio;
using theori.Graphics;

namespace theori.Resources
{
    public sealed class ClientResourceManager
    {
        private readonly List<ManifestResourceLoader> m_resourceLoaders = new List<ManifestResourceLoader>();

        private readonly Dictionary<string, IDisposable> m_resources = new Dictionary<string, IDisposable>();

        public readonly string FileSearchDirectory;
        public readonly string FallbackMaterialName;

        public ClientResourceManager(string fileSearchDirectory, string fallbackMaterialName)
        {
            FileSearchDirectory = fileSearchDirectory;
            FallbackMaterialName = fallbackMaterialName;
        }

        public void AddManifestResourceLoader(ManifestResourceLoader loader)
        {
            if (m_resourceLoaders.Contains(loader)) return;
            m_resourceLoaders.Add(loader);
        }

        public AudioTrack AquireAudioTrack(string resourcePath)
        {
            if (!m_resources.TryGetValue(resourcePath, out IDisposable resource))
            {
                resource = LoadRawAudioTrack(resourcePath);
                m_resources[resourcePath] = resource;
            }

            return resource as AudioTrack;
        }

        public AudioSample AquireAudioSample(string resourcePath)
        {
            if (!m_resources.TryGetValue(resourcePath, out IDisposable resource))
            {
                resource = LoadRawAudioSample(resourcePath);
                m_resources[resourcePath] = resource;
            }

            return resource as AudioSample;
        }

        public Texture AquireTexture(string resourcePath)
        {
            if (!m_resources.TryGetValue(resourcePath, out IDisposable resource))
            {
                resource = LoadRawTexture(resourcePath);
                m_resources[resourcePath] = resource;
            }

            return resource as Texture;
        }

        public Material AquireMaterial(string resourcePath)
        {
            if (!m_resources.TryGetValue(resourcePath, out IDisposable resource))
            {
                resource = LoadRawMaterial(resourcePath);
                m_resources[resourcePath] = resource;
            }

            return resource as Material;
        }

        public AudioTrack LoadRawAudioTrack(string resourcePath)
        {
            string[] exts = { ".ogg", ".wav" };

            if (FileSearchDirectory != null)
            {
                foreach (var ext in exts)
                {
                    string fsResourcePath = Path.Combine(FileSearchDirectory, resourcePath + ext);
                    if (File.Exists(fsResourcePath))
                        return AudioTrack.FromFile(fsResourcePath);
                }
            }

            // first loader with the path loads it
            foreach (var loader in m_resourceLoaders)
            {
                foreach (var ext in exts)
                {
                    string manifestResourcePath = resourcePath + ext;
                    if (loader.ContainsResource(manifestResourcePath))
                    {
                        using (var stream = loader.GetResourceStream(manifestResourcePath))
                            return AudioTrack.FromStream(ext, stream);
                    }
                }
            }

            throw new ArgumentException("Could not find the specified audio track resource.", nameof(resourcePath));
        }

        public AudioSample LoadRawAudioSample(string resourcePath)
        {
            string[] exts = { ".ogg", ".wav" };

            if (FileSearchDirectory != null)
            {
                foreach (var ext in exts)
                {
                    string fsResourcePath = Path.Combine(FileSearchDirectory, resourcePath + ext);
                    if (File.Exists(fsResourcePath))
                        return AudioSample.FromFile(fsResourcePath);
                }
            }

            // first loader with the path loads it
            foreach (var loader in m_resourceLoaders)
            {
                foreach (var ext in exts)
                {
                    string manifestResourcePath = resourcePath + ext;
                    if (loader.ContainsResource(manifestResourcePath))
                        return AudioSample.FromStream(ext, loader.GetResourceStream(manifestResourcePath));
                }
            }

            throw new ArgumentException("Could not find the specified audio sample resource.", nameof(resourcePath));
        }

        public Texture LoadRawTexture(string resourcePath)
        {
            string[] exts = { ".png" };

            if (FileSearchDirectory != null)
            {
                foreach (var ext in exts)
                {
                    string fsResourcePath = Path.Combine(FileSearchDirectory, resourcePath + ext);
                    if (File.Exists(fsResourcePath))
                        return Texture.FromFile2D(fsResourcePath);
                }
            }

            // first loader with the path loads it
            foreach (var loader in m_resourceLoaders)
            {
                foreach (var ext in exts)
                {
                    string manifestResourcePath = resourcePath + ext;
                    if (loader.ContainsResource(manifestResourcePath))
                    {
                        using (var stream = loader.GetResourceStream(manifestResourcePath))
                            return Texture.FromStream2D(stream);
                    }
                }
            }

            throw new ArgumentException("Could not find the specified texture resource.", nameof(resourcePath));
        }

        public Material LoadRawMaterial(string resourcePath)
        {
            (int ResourceIndex, string ResourcePath) LocateShaderResource(string ext, out bool isFallback)
            {
                isFallback = false;

                // search for the right one first
                if (FileSearchDirectory != null)
                {
                    string fsPath = Path.Combine(FileSearchDirectory, resourcePath) + ext;
                    if (File.Exists(fsPath))
                        return (-1, fsPath);
                }

                for (int i = 0; i < m_resourceLoaders.Count; i++)
                {
                    var loader = m_resourceLoaders[i];

                    string loaderPath = resourcePath + ext;
                    if (loader.ContainsResource(loaderPath))
                        return (i, loaderPath);
                }

                // then search for the fallback
                isFallback = true;

                if (FileSearchDirectory != null)
                {
                    string correct = Path.Combine(FileSearchDirectory, FallbackMaterialName) + ext;
                    if (File.Exists(correct))
                        return (-1, correct);
                }

                for (int i = 0; i < m_resourceLoaders.Count; i++)
                {
                    var loader = m_resourceLoaders[i];

                    string loaderPath = FallbackMaterialName + ext;
                    if (loader.ContainsResource(loaderPath))
                        return (i, loaderPath);
                }

                //Debug.Assert(false, $"Unable to locate correct or fallback shader for { resourcePath }{ ext } (with fallback name { FallbackMaterialName })");
                return (-1, null);
            }

            Stream OpenStreamForShader((int Index, string Path) resourceInfo)
            {
                if (resourceInfo.Path == null) return null;

                if (resourceInfo.Index == -1)
                    return File.OpenRead(resourceInfo.Path);
                else return m_resourceLoaders[resourceInfo.Index].GetResourceStream(resourceInfo.Path);
            }

            (int, string) vertexLocation = LocateShaderResource(".vs", out bool missingVertex);
            (int, string) fragmentLocation = LocateShaderResource(".fs", out bool missingFragment);
            (int, string) geometryLocation = LocateShaderResource(".gs", out bool missingGeometry);

            if (missingVertex && missingFragment)
                throw new ArgumentException("Could not find the specified material resource.", nameof(resourcePath));

            using (var vertexStream = OpenStreamForShader(vertexLocation))
            using (var fragmentStream = OpenStreamForShader(fragmentLocation))
            using (var geometryStream = OpenStreamForShader(geometryLocation))
                return new Material(vertexStream, fragmentStream, geometryStream);
        }
    }
}
