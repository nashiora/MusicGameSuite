using System;

using theori;
using theori.Configuration;
using theori.IO;

namespace NeuroSonic.Startup
{
    public sealed class InputMethodConfigLayer : BaseMenuLayer
    {
        protected override string Title => "Input Methods";

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(NextOffset, "Keyboard Only", () => SelectKeyboard(false)));
            AddMenuItem(new MenuItem(NextOffset, "Keyboard and Mouse", () => SelectKeyboard(true)));

            for (int id = 0; id < Gamepad.NumConnected; id++)
            {
                string name = Gamepad.NameOf(id);
                Logger.Log($"Connected Controller { id }: { name }");

                int gpId = id;
                AddMenuItem(new MenuItem(NextOffset, name, () => SelectGamepad(gpId)));
            }
        }

        private void SelectKeyboard(bool andMouse)
        {
            Plugin.Config.Set(NscConfigKey.ButtonInputDevice, InputDevice.Keyboard);
            Plugin.Config.Set(NscConfigKey.LaserInputDevice, andMouse ? InputDevice.Mouse : InputDevice.Keyboard);
            Plugin.SaveNscConfig();

            Host.PopToParent(this);
        }

        private void SelectGamepad(int id)
        {
            Plugin.Config.Set(NscConfigKey.ButtonInputDevice, InputDevice.Controller);
            Plugin.Config.Set(NscConfigKey.LaserInputDevice, InputDevice.Controller);
            Plugin.SwitchActiveGamepad(id); // Saves the config anyway

            Host.PopToParent(this);
        }
    }
}
