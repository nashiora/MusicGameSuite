using System.IO;

using theori;
using theori.Audio;
using theori.Charting.Conversions;
using theori.IO;
using theori.Resources;

using NeuroSonic.Startup;
using NeuroSonic.GamePlay;
using System.Collections.Generic;
using System;
using theori.Charting;

namespace NeuroSonic.ChartSelect
{
    internal class ChartManagerLayer : BaseMenuLayer
    {
        protected override string Title => "Chart Manager";

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(ItemIndex, "Open KSH Chart Directly", OpenKSH));
            AddMenuItem(new MenuItem(ItemIndex, "Convert KSH Charts to Theori Set", ConvertKSH));
            AddMenuItem(new MenuItem(ItemIndex, "Convert KSH Charts and Open Selected", ConvertKSHAndOpen));
        }

        private void OpenKSH()
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

                var game = new GameLayer(Plugin.DefaultResourceLocator, chart, audio, autoPlay);
                Host.PushLayer(new GenericTransitionLayer(game, Plugin.DefaultResourceLocator));
            }
        }

        private void ConvertKSH()
        {
            var dialog = new OpenFileDialogDesc("Open Chart",
                                new[] { new FileFilter("K-Shoot MANIA Files", "ksh") });

            var dialogResult = FileSystem.ShowOpenFileDialog(dialog);
            if (dialogResult.DialogResult == DialogResult.OK)
            {
                string primaryKshChart = dialogResult.FilePath;

                Charting.KShootMania.ChartMetadata primaryKshMeta;
                using (var reader = new StreamReader(File.OpenRead(primaryKshChart)))
                    primaryKshMeta = Charting.KShootMania.ChartMetadata.Create(reader);

                string setDir = Directory.GetParent(primaryKshChart).FullName;
                string setName = Path.GetFileName(setDir);

                List<(string, Charting.KShootMania.ChartMetadata)> chartFiles =
                    new List<(string, Charting.KShootMania.ChartMetadata)> { (primaryKshChart, primaryKshMeta) };

                foreach (string kshChart in Directory.EnumerateFiles(setDir, "*.ksh"))
                {
                    // we're filtering out invalid charts, as :theori will only support one song per set.
                    // skipping files with non-matching meta and logging the issue for now.
                    if (Path.GetFileName(kshChart) == Path.GetFileName(primaryKshChart)) continue;

                    Charting.KShootMania.ChartMetadata kshMeta;
                    using (var reader = new StreamReader(File.OpenRead(kshChart)))
                        kshMeta = Charting.KShootMania.ChartMetadata.Create(reader);

                    if ((kshMeta.MusicFile, kshMeta.MusicFileNoFx, kshMeta.Title, kshMeta.Artist) !=
                       (primaryKshMeta.MusicFile, primaryKshMeta.MusicFileNoFx, primaryKshMeta.Title, primaryKshMeta.Artist))
                    {
                        Logger.Log($"Skipping '{ Path.GetFileName(kshChart) }' chart file in the set '{ setDir }'.\n:theori and NeuroSonic only support a single song for each set, and the chosen set does not comply.\nOnly charts of the same song will be added to this converted set.");
                        continue;
                    }

                    chartFiles.Add( (kshChart, kshMeta) );
                }

                var chartSetInfo = new ChartSetInfo()
                {
                };

                foreach (var (kshChart, kshMeta) in chartFiles)
                {
                    var chartInfo = new ChartInfo()
                    {
                    };
                }
            }
        }

        private void ConvertKSHAndOpen()
        {
        }
    }
}
