using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;

using theori.Graphics;

using static MoonSharp.Interpreter.DynValue;

namespace theori.Scripting
{
    [MoonSharpUserData]
    public sealed class ScriptWindowInterface
    {
        [MoonSharpVisible(true)]
        private DynValue GetClientSize() => NewTuple(NewNumber(Window.Width), NewNumber(Window.Height));
    }
}
