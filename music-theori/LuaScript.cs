using System;
using MoonSharp.Interpreter;

namespace theori
{
    public class LuaScript
    {
        private readonly Script m_script;

        public object this[string globalKey]
        {
            get => m_script.Globals[globalKey];
            set => m_script.Globals[globalKey] = value;
        }

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

        public DynValue Call(string name, params object[] args)
        {
            return m_script.Call(this[name], args);
        }

        public DynValue Call(object val, params object[] args)
        {
            return m_script.Call(val, args);
        }
    }
}
