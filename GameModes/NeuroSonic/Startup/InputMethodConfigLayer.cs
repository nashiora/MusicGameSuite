using System;

using theori.IO;

namespace NeuroSonic.Startup
{
    public sealed class InputMethodConfigLayer : BaseMenuLayer
    {
        protected override string Title => "Input Methods";

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(ItemIndex, "Keyboard Only", () => SelectKeyboard(false)));
            AddMenuItem(new MenuItem(ItemIndex, "Keyboard and Mouse", () => SelectKeyboard(true)));

            for (int id = 0; id < Gamepad.NumConnected; id++)
            {
                string name = Gamepad.NameOf(id);
                Logger.Log($"Connected Controller { id }: { name }");

                int gpId = id;
                AddMenuItem(new MenuItem(ItemIndex, name, () => SelectGamepad(gpId)));
            }
        }

        private void SelectKeyboard(bool andMouse)
        {
            Logger.Log($"Selected Keyboard{ (andMouse ? " and Mouse" : "") }");
        }

        private void SelectGamepad(int id)
        {
            Logger.Log($"Selected Gamepad { id }");
        }
    }
}
