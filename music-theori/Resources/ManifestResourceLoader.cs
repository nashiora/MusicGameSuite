using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace theori.Resources
{
    public sealed class ManifestResourceLoader
    {
        private static readonly Dictionary<(Assembly Assembly, string Namespace), ManifestResourceLoader> loaders =
            new Dictionary<(Assembly Assembly, string Namespace), ManifestResourceLoader>();

        public static ManifestResourceLoader GetResourceLoader(Assembly assembly, string rootNamespace)
        {
            var key = (assembly, rootNamespace);
            if (!loaders.TryGetValue(key, out var loader))
            {
                loader = new ManifestResourceLoader(assembly, rootNamespace);
                loaders[key] = loader;
            }
            return loader;
        }

        public static string ResourcePathToManifestLocation(string resourcePath, string rootNamespace)
        {
            return $"{ rootNamespace }.{ resourcePath.Replace('/', '.') }";
        }

        public Assembly Assembly { get; private set; }
        public string Namespace { get; private set; }

        private ManifestResourceLoader(Assembly assembly, string rootNamespace)
        {
            Assembly = assembly;
            Namespace = rootNamespace;
        }

        public bool ContainsResource(string resourcePath)
        {
            string manifestResourcePath = ResourcePathToManifestLocation(resourcePath, Namespace);

            var info = Assembly.GetManifestResourceInfo(manifestResourcePath);
            return info != null;
        }

        public Stream OpenResourceStream(string resourcePath)
        {
            return Assembly.GetManifestResourceStream(ResourcePathToManifestLocation(resourcePath, Namespace));
        }
    }
}
