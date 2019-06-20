using System.IO;

using OpenRM.Convert;

using theori;
using theori.Audio;
using theori.IO;
using theori.Resources;

using NeuroSonic.GamePlay;

namespace NeuroSonic.ChartSelect
{
    public abstract class ChartSelectLayer : NscLayer
    {
        protected readonly ClientResourceManager m_skin;

        public ChartSelectLayer(ClientResourceManager skin)
        {
            m_skin = skin;
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        public override void Suspended()
        {
        }

        public override void Resumed()
        {
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
                        var ksh = KShootMania.Chart.CreateFromFile(kshChart);

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

                        var game = new GameLayer(m_skin, chart, audio, autoPlay);
                        Host.PushLayer(game);
                    }
                } break;

                default: return false;
            }

            return true;
        }

        protected internal override bool ControllerButtonReleased(ControllerInput input)
        {
            switch (input)
            {
                default: return false;
            }

            return true;
        }

        protected internal override bool ControllerAxisChanged(ControllerInput input, float delta)
        {
            switch (input)
            {
                default: return false;
            }

            return true;
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);
        }

        public override void Render()
        {
        }
    }
}
