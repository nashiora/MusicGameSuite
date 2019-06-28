using System;
using System.Collections.Generic;
using System.IO;

namespace theori.Resources
{
    public sealed class ClientResourceLocator
    {
        public static readonly ClientResourceLocator Default;

        static ClientResourceLocator()
        {
            Default = new ClientResourceLocator(null, "materials/basic");
            Default.AddManifestResourceLoader(ManifestResourceLoader.GetResourceLoader(typeof(ClientResourceLocator).Assembly, "theori.Resources"));
        }

        private readonly List<ManifestResourceLoader> m_resourceLoaders = new List<ManifestResourceLoader>();

        public readonly string FileSearchDirectory;
        public readonly string FallbackMaterialName;

        public ClientResourceLocator(string fileSearchDirectory, string fallbackMaterialName)
        {
            FileSearchDirectory = fileSearchDirectory;
            FallbackMaterialName = fallbackMaterialName;
        }

        public ClientResourceLocator Clone(string newFileSearchDirectory = null)
        {
            var result = new ClientResourceLocator(newFileSearchDirectory ?? FileSearchDirectory, FallbackMaterialName);
            foreach (var loader in m_resourceLoaders)
                result.AddManifestResourceLoader(loader);
            return result;
        }

        public void AddManifestResourceLoader(ManifestResourceLoader loader)
        {
            if (m_resourceLoaders.Contains(loader)) return;
            m_resourceLoaders.Add(loader);
        }

        private Stream OpenStreamWithExtension(string resourcePath, string[] exts, out string fileExtension)
        {
            if (FileSearchDirectory != null)
            {
                foreach (var ext in exts)
                {
                    string fsResourcePath = Path.Combine(FileSearchDirectory, resourcePath + ext);
                    if (File.Exists(fsResourcePath))
                    {
                        fileExtension = ext;
                        return File.OpenRead(fsResourcePath);
                    }
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
                        fileExtension = ext;
                        return loader.OpenResourceStream(manifestResourcePath);
                    }
                }
            }

            fileExtension = null;
            return null;
        }

        public Stream OpenFileStream(string resourcePath)
        {
            if (FileSearchDirectory != null)
            {
                string fsResourcePath = Path.Combine(FileSearchDirectory, resourcePath);
                if (File.Exists(fsResourcePath))
                    return File.OpenRead(fsResourcePath);
            }

            // first loader with the path loads it
            foreach (var loader in m_resourceLoaders)
            {
                if (loader.ContainsResource(resourcePath))
                    return loader.OpenResourceStream(resourcePath);
            }

            return null;
        }

        public Stream OpenAudioStream(string resourcePath, out string fileExtension)
        {
            string[] exts = { ".ogg", ".wav" };
            return OpenStreamWithExtension(resourcePath, exts, out fileExtension);
        }

        public Stream OpenTextureStream(string resourcePath, out string fileExtension)
        {
            string[] exts = { ".png" };
            return OpenStreamWithExtension(resourcePath, exts, out fileExtension);
        }

        public Stream OpenShaderStream(string resourcePath, string fileExtension, out bool usedFallback)
        {
            usedFallback = false;

            // search for the right one first
            if (FileSearchDirectory != null)
            {
                string fsResourcePath = Path.Combine(FileSearchDirectory, resourcePath) + fileExtension;
                if (File.Exists(fsResourcePath))
                    return File.OpenRead(fsResourcePath);
            }

            for (int i = 0; i < m_resourceLoaders.Count; i++)
            {
                var loader = m_resourceLoaders[i];

                string manifestResourcePath = resourcePath + fileExtension;
                if (loader.ContainsResource(manifestResourcePath))
                    return loader.OpenResourceStream(manifestResourcePath);
            }

            // then search for the fallback
            usedFallback = true;

            if (FileSearchDirectory != null)
            {
                string fsResourcePath = Path.Combine(FileSearchDirectory, FallbackMaterialName) + fileExtension;
                if (File.Exists(fsResourcePath))
                    return File.OpenRead(fsResourcePath);
            }

            for (int i = 0; i < m_resourceLoaders.Count; i++)
            {
                var loader = m_resourceLoaders[i];

                string manifestResourcePath = FallbackMaterialName + fileExtension;
                if (loader.ContainsResource(manifestResourcePath))
                    return loader.OpenResourceStream(manifestResourcePath);
            }

            return null;
        }
    }
}
