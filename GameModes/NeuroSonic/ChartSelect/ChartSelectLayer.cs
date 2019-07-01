using System.IO;

using theori;
using theori.Audio;
using theori.Charting.Conversions;
using theori.IO;
using theori.Resources;

using NeuroSonic.GamePlay;

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
                case ControllerInput.Start:
                {
                    var dialog = new OpenFileDialogDesc("Open Chart",
                                        new[] { new FileFilter("K-Shoot MANIA Files", "ksh") });

                    var dialogResult = FileSystem.ShowOpenFileDialog(dialog);
                    if (dialogResult.DialogResult == DialogResult.OK)
                    {
                        string kshChart = dialogResult.FilePath;

                        string fileDir = Directory.GetParent(kshChart).FullName;
                        var ksh = Charting.KShootMania.Chart.CreateFromFile(kshChart);

                        string audioFileFx = Path.Combine(fileDir, ksh.Metadata.MusicFile ?? "");
                        string audioFileNoFx = Path.Combine(fileDir, ksh.Metadata.MusicFileNoFx ?? "");

                        string audioFile = audioFileNoFx;
                        if (File.Exists(audioFileFx))
                            audioFile = audioFileFx;

                        var audio = AudioTrack.FromFile(audioFile);
                        audio.Channel = Host.Mixer.MasterChannel;
                        audio.Volume = ksh.Metadata.MusicVolume / 100.0f;

                        var chart = ksh.ToVoltex();

                        AutoPlay autoPlay = AutoPlay.None;
                        if (Keyboard.IsDown(KeyCode.LCTRL) || Keyboard.IsDown(KeyCode.RCTRL))
                            autoPlay = AutoPlay.ButtonsAndLasers;

                        var game = new GameLayer(m_locator, chart, audio, autoPlay);
                        Host.PushLayer(new GenericTransitionLayer(game, m_locator));
                    }
                } break;

                default: return false;
            }

            return true;
        }
    }
}
