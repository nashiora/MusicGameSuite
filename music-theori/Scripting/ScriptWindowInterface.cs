using MoonSharp.Interpreter;

using theori.Graphics;

using static MoonSharp.Interpreter.DynValue;

namespace theori.Scripting
{
    [MoonSharpUserData]
    public sealed class ScriptWindowInterface
    {
        public DynValue GetClientSize() => NewTuple(NewNumber(Window.Width), NewNumber(Window.Height));
    }
}
