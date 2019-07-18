using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;

using MoonSharp.Interpreter;
using theori.Graphics;
using theori.Resources;

namespace theori.Scripting
{
    public class LuaScript : Disposable
    {
        private static ClientResourceLocator scriptLocator;

        public static void RegisterType<T>() => UserData.RegisterType<T>();
        public static void RegisterType(Type type) => UserData.RegisterType(type);

        public static void RegisterAssembly(Assembly assembly = null, bool includeExtensionTypes = false) =>
            UserData.RegisterAssembly(assembly, includeExtensionTypes);

        static LuaScript()
        {
            scriptLocator = new ClientResourceLocator(null, null);
            scriptLocator.AddManifestResourceLoader(ManifestResourceLoader.GetResourceLoader(typeof(LuaScript).Assembly, "theori.Resources"));
        }

        private readonly Script m_script;

        private readonly Dictionary<Type, DynValue> m_luaConverters = new Dictionary<Type, DynValue>();

        private ClientResourceManager m_resources;
        private BasicSpriteRenderer m_renderer;

#if false
        public DynValue this[string globalKey]
        {
            get => m_script.Globals.Get(globalKey);
            set => m_script.Globals.Set(globalKey, value);
        }
#else
        public object this[string globalKey]
        {
            get => m_script.Globals[globalKey];
            set
            {
                if (value != null && m_luaConverters.TryGetValue(value.GetType(), out var converter))
                    m_script.Globals[globalKey] = m_script.Call(converter, value);
                else m_script.Globals[globalKey] = value;
            }
        }
#endif

        public LuaScript()
        {
            m_script = new Script( CoreModules.Basic
                                 | CoreModules.String
                                 | CoreModules.Bit32
                                 | CoreModules.Coroutine
                                 | CoreModules.Debug
                                 | CoreModules.ErrorHandling
                                 | CoreModules.GlobalConsts
                                 | CoreModules.Json
                                 | CoreModules.Math
                                 | CoreModules.Metatables
                                 | CoreModules.Table
                                 | CoreModules.TableIterators);

            m_script.Globals.Get("math").Table["sign"] = (Func<double, int>)Math.Sign;
            m_script.Globals.Get("math").Table["clamp"] = (Func<double, double, double, double>)MathL.Clamp;

            InitTheoriLibrary();
        }

        protected override void DisposeManaged()
        {
            m_resources?.Dispose();
            m_renderer?.Dispose();

            m_resources = null;
            this["res"] = null;

            m_renderer = null;
            this["g2d"] = null;
        }

        public void InitTheoriLibrary()
        {
            this["Anchor"] = typeof(Anchor);

            Vector2 NewVec2(float x, float y) => new Vector2(x, y);
            this["vec2"] = (Func<float, float, Vector2>)NewVec2;

            Vector3 NewVec3(float x, float y, float z) => new Vector3(x, y, z);
            this["vec3"] = (Func<float, float, float, Vector3>)NewVec3;

            Vector4 NewVec4(float x, float y, float z, float w) => new Vector4(x, y, z, w);
            this["vec4"] = (Func<float, float, float, float, Vector4>)NewVec4;

            LoadFile(scriptLocator.OpenFileStream("scripts/lib/standard.lua"));
            var converters = LoadFile(scriptLocator.OpenFileStream("scripts/lib/vectors.lua"));

            m_luaConverters[typeof(Vector2)] = (DynValue)converters.Tuple.GetValue(0);
            m_luaConverters[typeof(Vector3)] = (DynValue)converters.Tuple.GetValue(1);
            m_luaConverters[typeof(Vector4)] = (DynValue)converters.Tuple.GetValue(2);
        }

        public void InitResourceLoading(ClientResourceLocator locator)
        {
            m_resources = new ClientResourceManager(locator);
            this["res"] = m_resources;
        }

        public void InitSpriteRenderer(ClientResourceLocator locator = null, Vector2? viewportSize = null)
        {
            m_renderer = new BasicSpriteRenderer(locator, viewportSize);
            this["g2d"] = m_renderer;
        }

        public bool LuaAsyncLoad()
        {
            var result = CallIfExists("AsyncLoad");
            if (result == null)
                return true;

            if (!result.CastToBool())
                return false;

            if (!m_resources.LoadAll())
                return false;

            return true;
        }

        public bool LuaAsyncFinalize()
        {
            var result = CallIfExists("AsyncFinalize");
            if (result == null)
                return true;

            if (!result.CastToBool())
                return false;

            if (!m_resources.FinalizeLoad())
                return false;

            return true;
        }

        public void Update(float delta, float total)
        {
            CallIfExists("Update", delta, total);
        }

        public void Draw()
        {
            if (m_renderer != null)
            {
                m_renderer.BeginFrame();
                CallIfExists("Draw");
                m_renderer.Flush();
                m_renderer.EndFrame();
            }
        }

        public DynValue DoString(string code) => m_script.DoString(code);

        /// <summary>
        /// Takes ownership of the file stream.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public DynValue LoadFile(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                string code = reader.ReadToEnd();
                return DoString(code);
            }
        }

        public async Task<DynValue> LoadFileAsync(Stream fileStream)
        {
            using (var reader = new StreamReader(fileStream))
            {
                string code = await reader.ReadToEndAsync();
                return DoString(code);
            }
        }

        public DynValue Call(string name, params object[] args)
        {
            return m_script.Call(this[name], args);
        }

        public DynValue CallIfExists(string name, params object[] args)
        {
            var target = this[name];
            if (target is Closure || target is CallbackFunction)
                return m_script.Call(target, args);
            else return null;
        }

        public DynValue Call(object val, params object[] args)
        {
            return m_script.Call(val, args);
        }

        #region New

        public Table NewTable() => new Table(m_script);

        #endregion
    }
}
