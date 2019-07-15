using System;
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
        public static void RegisterType<T>() => UserData.RegisterType<T>();
        public static void RegisterType(Type type) => UserData.RegisterType(type);

        public static void RegisterAssembly(Assembly assembly = null, bool includeExtensionTypes = false) =>
            UserData.RegisterAssembly(assembly, includeExtensionTypes);

        private readonly Script m_script;

        private ClientResourceLocator m_locator;
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
            set => m_script.Globals[globalKey] = value;
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

            m_locator = null;
            m_resources = null;
            this["res"] = null;

            m_renderer = null;
            this["g2d"] = null;
        }

        public void InitTheoriLibrary()
        {
            this["window"] = new ScriptWindowInterface();
        }

        public void InitResourceLoading(ClientResourceLocator locator)
        {
            m_locator = locator;
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
    }
}
