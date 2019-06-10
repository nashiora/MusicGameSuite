using theori;
using theori.Gui;

using theori.Graphics;
using System.Numerics;

namespace NeuroSonic.GamePlay
{
    public class ComboDisplay : Panel
    {
        private int m_combo;
        private float m_lastDisplayTime;

        private TextLabel m_text;

        public int Combo
        {
            get => m_combo;
            set
            {
                if (value < m_combo)
                    m_text.Color = Vector4.One;

                if (value != 0)
                    m_lastDisplayTime = Time.Total;
                m_combo = value;

                m_text.Text = $"{m_combo:D4}";
            }
        }

        public ComboDisplay()
        {
            Children = new GuiElement[]
            {
                m_text = new TextLabel(Font.Default32, "0000")
                {
                    TextAlignment = TextAlignment.MiddleCenter,
                    Color = new Vector4(1, 1, 0, 1),
                },
            };
        }

        public override void Render(GuiRenderQueue rq)
        {
            if (m_combo > 0 && Time.Total - m_lastDisplayTime <= 0.75)
            {
                base.Render(rq);
            }
        }
    }
}
