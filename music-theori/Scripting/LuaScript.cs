using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using MoonSharp.Interpreter;

namespace theori.Scripting
{
    public class LuaScript
    {
        public static void RegisterType<T>() => UserData.RegisterType<T>();
        public static void RegisterType(Type type) => UserData.RegisterType(type);

        public static void RegisterAssembly(Assembly assembly = null, bool includeExtensionTypes = false) =>
            UserData.RegisterAssembly(assembly, includeExtensionTypes);

        private readonly Script m_script;

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
            else return DynValue.Nil;
        }

        public DynValue Call(object val, params object[] args)
        {
            return m_script.Call(val, args);
        }
    }
}
