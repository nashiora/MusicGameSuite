using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace theori.Resources
{
    public sealed class ManifestResourceLoader : Disposable
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

#if false && DEBUG
            foreach (string m in Assembly.GetManifestResourceNames())
                Logger.Log($"Manifest resource found: { m }");
#endif
        }

        public bool ContainsResource(string resourcePath)
        {
            string manifestResourcePath = ResourcePathToManifestLocation(resourcePath, Namespace);

            var info = Assembly.GetManifestResourceInfo(manifestResourcePath);
            return info != null;
        }

        public Stream OpenResourceStream(string resourcePath)
        {
            if (IsDisposed) throw new ObjectDisposedException(nameof(ManifestResourceLoader));
            return Assembly.GetManifestResourceStream(ResourcePathToManifestLocation(resourcePath, Namespace));
        }

        protected override void DisposeManaged()
        {
            var key = (Assembly, Namespace);
            loaders.Remove(key);

            // make this instance useless

            Assembly = null;
            Namespace = null;
        }
    }
}
