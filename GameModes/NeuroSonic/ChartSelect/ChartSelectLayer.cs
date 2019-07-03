using System.IO;

using theori;
using theori.Audio;
using theori.IO;
using theori.Resources;

using NeuroSonic.GamePlay;
using NeuroSonic.Charting.Conversions;

namespace NeuroSonic.ChartSelect
{
    public abstract class ChartSelectLayer : NscLayer
    {
        protected readonly ClientResourceLocator m_locator;

        public ChartSelectLayer(ClientResourceLocator locator)
        {
            m_locator = locator;
        }

        public override bool KeyPressed(KeyInfo info)
        {
            switch (info.KeyCode)
            {
                case KeyCode.ESCAPE:
                {
                    Host.PopToParent(this);
                } break;

                default: return false;
            }

            return true;
        }

        protected internal override bool ControllerButtonPressed(ControllerInput input)
        {
            switch (input)
            {
                default: return false;
            }

            return true;
        }
    }
}
