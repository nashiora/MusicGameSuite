using MoonSharp.Interpreter;

using theori.Graphics;

namespace theori.Scripting
{
    [MoonSharpUserData]
    public sealed class ScriptWindowInterface
    {
        public DynValue GetClientSize() =>
            DynValue.NewTuple(DynValue.NewNumber(Window.Width), DynValue.NewNumber(Window.Height));
    }
}
