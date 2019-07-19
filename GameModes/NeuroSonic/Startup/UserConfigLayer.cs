using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using theori;
using theori.Graphics;
using theori.Gui;
using theori.IO;

namespace NeuroSonic.Startup
{
    public class UserConfigLayer : BaseMenuLayer
    {
        class Nav
        {
            public Direction2D Dir;
            public bool Modified;

            public Nav(KeyInfo key)
            {
            }
        }

        protected override string Title => "Config";

        private int m_hsKindIndex, m_hsValueIndex;
        private int m_voIndex, m_ioIndex;
        private int m_llHue, m_rlHue;

        private Action<Nav>[] m_configActions;

        private HiSpeedMod m_hiSpeedKind;
        private TextLabel[] m_hsKinds;

        private float m_hiSpeed, m_modSpeed;
        private TextLabel m_hs;

        private int m_voffValue;
        private TextLabel m_voff;

        private int m_ioffValue;
        private TextLabel m_ioff;

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
            m_hiSpeed = Plugin.Config.GetFloat(NscConfigKey.HiSpeed);
            m_modSpeed = Plugin.Config.GetFloat(NscConfigKey.ModSpeed);
            m_voffValue = Plugin.Config.GetInt(NscConfigKey.VideoOffset);
            m_ioffValue = Plugin.Config.GetInt(NscConfigKey.InputOffset);

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

                    new Panel()
                    {
                        Position = new Vector2(0, SPACING),
                        Children = new TextLabel[]
                        {
                            m_hs = new TextLabel(Font.Default24, "0")   { Position = new Vector2(0, 0) },
                        }
                    },

                    new Panel()
                    {
                        Position = new Vector2(0, SPACING * 2),
                        Children = new TextLabel[]
                        {
                            m_voff = new TextLabel(Font.Default24, "0")   { Position = new Vector2(0, 0) },
                        }
                    },

                    new Panel()
                    {
                        Position = new Vector2(0, SPACING * 3),
                        Children = new TextLabel[]
                        {
                            m_ioff = new TextLabel(Font.Default24, "0")   { Position = new Vector2(0, 0) },
                        }
                    },
                }
            });

            m_configActions = new Action<KeyInfo>[]
            {
                KeyPressed_HiSpeedKind,
                KeyPressed_HiSpeed,
            };
        }

        private void KeyPressed_HiSpeedKind(Nav key)
        {
            KeyCode code = key.KeyCode;
            if (code != KeyCode.LEFT && code != KeyCode.RIGHT) return;
            int dir = code == KeyCode.LEFT ? -1 : 1;

            m_hiSpeedKind = (HiSpeedMod)((int)(m_hiSpeedKind + dir + 3) % 3);
            Plugin.Config.Set(NscConfigKey.HiSpeedModKind, m_hiSpeedKind);
        }

        private void KeyPressed_HiSpeed(Nav key)
        {
            KeyCode code = key.KeyCode;
            if (code != KeyCode.LEFT && code != KeyCode.RIGHT) return;
            int dir = code == KeyCode.LEFT ? -1 : 1;

            bool ctrl = key.Mods.HasFlag(KeyMod.LCTRL) || key.Mods.HasFlag(KeyMod.RCTRL);

            switch (m_hiSpeedKind)
            {
                case HiSpeedMod.Default:
                {
                    float amt = ctrl ? 1 : 5;
                    m_hiSpeed = MathL.Round(((m_hiSpeed * 10) + dir * amt) / amt) * amt / 10;
                    Plugin.Config.Set(NscConfigKey.HiSpeed, m_hiSpeed);
                } break;

                case HiSpeedMod.MMod:
                case HiSpeedMod.CMod:
                {
                    float amt = ctrl ? 5 : 25;
                    m_modSpeed = MathL.Round((m_modSpeed + dir * amt) / amt) * amt;
                    Plugin.Config.Set(NscConfigKey.ModSpeed, m_modSpeed);
                } break;
            }
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

        private void UpdateHiSpeed()
        {
            bool active = m_activeIndex == m_hsValueIndex;
            m_hs.Color = active ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 1);

            switch (m_hiSpeedKind)
            {
                case HiSpeedMod.Default: m_hs.Text = $"{m_hiSpeed:F1}"; break;

                case HiSpeedMod.MMod:
                case HiSpeedMod.CMod: m_hs.Text = $"{(int)m_modSpeed}"; break;
            }
        }

        private void UpdateVideoOffset()
        {
            bool active = m_activeIndex == m_voIndex;
            m_voff.Color = active ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 1);
            m_voff.Text = $"{m_voffValue}";
        }

        private void UpdateInputOffset()
        {
            bool active = m_activeIndex == m_ioIndex;
            m_ioff.Color = active ? new Vector4(1, 1, 1, 1) : new Vector4(0.5f, 0.5f, 0.5f, 1);
            m_voff.Text = $"{m_ioffValue}";
        }

        public override bool KeyPressed(KeyInfo key)
        {
            if (m_activeIndex >= 0)
            {
                switch (key.KeyCode)
                {
                    case KeyCode.RETURN:
                    case KeyCode.ESCAPE: m_activeIndex = -1; break;

                    default: m_configActions[m_activeIndex](new Nav(key)); break;
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
            UpdateHiSpeed();
            UpdateVideoOffset();
            UpdateInputOffset();
        }
    }
}
