using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using theori.Graphics;
using theori.Gui;
using theori.IO;

namespace NeuroSonic.Startup
{
    public class UserConfigLayer : BaseMenuLayer
    {
        protected override string Title => "Config";

        private int m_hsKindIndex, m_hsValueIndex;
        private int m_voIndex, m_ioIndex;
        private int m_llHue, m_rlHue;

        private Action<KeyCode>[] m_configActions;

        private HiSpeedMod m_hiSpeedKind;
        private TextLabel[] m_hsKinds;

        private int m_activeIndex = -1;

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(m_hsKindIndex = NextOffset, "HiSpeed Kind", null));
            AddMenuItem(new MenuItem(m_hsValueIndex = NextOffset, "HiSpeed Value", null));
            AddMenuItem(new MenuItem(m_voIndex = NextOffset, "Video Offset", null));
            AddMenuItem(new MenuItem(m_ioIndex = NextOffset, "Input Offset", null));
            AddMenuItem(new MenuItem(m_llHue = NextOffset, "Left Laser Hue", null));
            AddMenuItem(new MenuItem(m_rlHue = NextOffset, "Right Laser Hue", null));
        }

        public override void Init()
        {
            base.Init();

            const int ALIGN = 300;
            const int SPACING = MenuItem.SPACING;

            m_hiSpeedKind = Plugin.Config.GetEnum<HiSpeedMod>(NscConfigKey.HiSpeedModKind);

            ForegroundGui.AddChild(new Panel()
            {
                Position = new Vector2(ALIGN, 100),

                Children = new GuiElement[]
                {
                    new Panel()
                    {
                        Position = new Vector2(0, 0),

                        Children = m_hsKinds = new TextLabel[]
                        {
                            new TextLabel(Font.Default24, "Multiplier")   { Position = new Vector2(0, 0) },
                            new TextLabel(Font.Default24, "Mode Mod")     { Position = new Vector2(150, 0) },
                            new TextLabel(Font.Default24, "Constant Mod") { Position = new Vector2(300, 0) },
                        }
                    },
                }
            });

            m_configActions = new Action<KeyCode>[]
            {
                KeyPressed_HiSpeedKind,
            };
        }

        private void KeyPressed_HiSpeedKind(KeyCode key)
        {
            if (key != KeyCode.LEFT && key != KeyCode.RIGHT) return;
            int dir = key == KeyCode.LEFT ? -1 : 1;

            m_hiSpeedKind = (HiSpeedMod)((int)(m_hiSpeedKind + dir + 3) % 3);
            Plugin.Config.Set(NscConfigKey.HiSpeedModKind, m_hiSpeedKind);
        }

        private void UpdateHiSpeedKinds()
        {
            bool active = m_activeIndex == m_hsKindIndex;

            Vector4 nCol = active ? new Vector4(0.5f, 0.5f, 0.5f, 1) : new Vector4(0.25f, 0.25f, 0.25f, 1.0f);
            Vector4 aCol = active ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

            for (int i = 0; i < m_hsKinds.Length; i++)
            {
                if (i == (int)m_hiSpeedKind)
                    m_hsKinds[i].Color = aCol;
                else m_hsKinds[i].Color = nCol;
            }
        }

        public override bool KeyPressed(KeyInfo key)
        {
            if (m_activeIndex >= 0)
            {
                switch (key.KeyCode)
                {
                    case KeyCode.RETURN:
                    case KeyCode.ESCAPE: m_activeIndex = -1; break;

                    default: m_configActions[m_activeIndex](key.KeyCode); break;
                }

                return true;
            }
            else
            {
                if (key.KeyCode == KeyCode.RETURN)
                {
                    m_activeIndex = ItemIndex;
                    return true;
                }
            }

            return base.KeyPressed(key);
        }

        protected internal override bool ControllerButtonPressed(ControllerInput input)
        {


            return base.ControllerButtonPressed(input);
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);

            UpdateHiSpeedKinds();
        }
    }
}
