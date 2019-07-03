using System;
using System.Collections.Generic;
using System.IO;

using theori;
using theori.Audio;
using theori.Charting;
using theori.IO;

using NeuroSonic.Startup;
using NeuroSonic.GamePlay;
using NeuroSonic.Charting.KShootMania;
using NeuroSonic.Charting.Conversions;
using theori.Gui;
using System.Numerics;
using theori.Graphics;
using System.Diagnostics;
using theori.Charting.IO;

namespace NeuroSonic.ChartSelect
{
    internal class ChartManagerLayer : BaseMenuLayer
    {
        protected override string Title => "Chart Manager";

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(NextOffset, "Open KSH Chart Directly", OpenKSH));
            AddMenuItem(new MenuItem(NextOffset, "Convert KSH Charts to Theori Set", ConvertKSH));
            AddMenuItem(new MenuItem(NextOffset, "Convert KSH Charts and Open Selected", ConvertKSHAndOpen));
        }

        public override void Init()
        {
            base.Init();

            ForegroundGui.AddChild(new Panel()
            {
                RelativeSizeAxes = Axes.X,
                RelativePositionAxes = Axes.Both,

                Position = new Vector2(0, 1),
                Size = new Vector2(1, 0),

                Children = new GuiElement[]
                {
                    new Panel()
                    {
                        RelativePositionAxes = Axes.X,

                        Position = new Vector2(0.5f, 0),

                        Children = new GuiElement[]
                        {
                            new TextLabel(Font.Default16, "Only charts with the same primary audio file will be added to the same set.")
                            {
                                TextAlignment = Anchor.BottomCenter,
                                Position = new Vector2(-10, -70),
                            },

                            new TextLabel(Font.Default16, "For old charts and most converts this means that only the selected diff will be added to the set!")
                            {
                                TextAlignment = Anchor.BottomCenter,
                                Position = new Vector2(-10, -40),
                            },
                        }
                    },
                }
            });
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
                var ksh = KshChart.CreateFromFile(kshChart);

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

        private ChartSetInfo ConvertKSHAndSave(string primaryKshFile)
        {
            var primaryKshChart = KshChart.CreateFromFile(primaryKshFile);
            var primaryKshMeta = primaryKshChart.Metadata;

            var primaryChart = primaryKshChart.ToVoltex();

            string setDir = Directory.GetParent(primaryKshFile).FullName;
            string setName = Path.GetFileName(setDir);

            List<(string, Chart)> chartFiles =
                new List<(string, Chart)> { (primaryKshFile, primaryChart) };

            foreach (string kshChartFile in Directory.EnumerateFiles(setDir, "*.ksh"))
            {
                // we're filtering out invalid charts, as :theori will only support one song per set.
                // skipping files with non-matching meta and logging the issue for now.
                if (Path.GetFileName(kshChartFile) == Path.GetFileName(primaryKshFile)) continue;

                KshChartMetadata kshMeta;
                using (var reader = new StreamReader(File.OpenRead(kshChartFile)))
                    kshMeta = KshChartMetadata.Create(reader);

                // don't worry about checking the nofx one, as we'll only keep the primary file anyway.
                if ((kshMeta.MusicFile, kshMeta.Title, kshMeta.Artist) !=
                    (primaryKshMeta.MusicFile, primaryKshMeta.Title, primaryKshMeta.Artist))
                {
                    Logger.Log($"Skipping '{ Path.GetFileName(kshChartFile) }' chart file in the set '{ setDir }'.\n:theori and NeuroSonic only support a single song for each set, and the chosen set does not comply.\nOnly charts of the same song will be added to this converted set.");
                    continue;
                }

                var kshChart = KshChart.CreateFromFile(kshChartFile);
                var chart = kshChart.ToVoltex();

                chartFiles.Add((kshChartFile, chart));
            }

            var chartSetInfo = new ChartSetInfo()
            {
                ID = 0, // no database ID, it's not in the database yet
                OnlineID = null, // no online stuff, it's not uploaded

                FilePath = setName,

                SongTitle = primaryKshMeta.Title,
                SongArtist = primaryKshMeta.Artist,
                SongFileName = primaryKshMeta.MusicFileNoFx,
            };

            foreach (var (kshChartFile, chart) in chartFiles)
            {
                var chartInfo = new ChartInfo()
                {
                    ID = 0, // No database ID, it's not in the database yet
                    Set = chartSetInfo,

                    FileName = $"{ Path.GetFileNameWithoutExtension(kshChartFile) }.tchart",

                    Charter = chart.Metadata.Charter,
                    JacketFileName = chart.Metadata.JacketFileName,
                    JacketArtist = chart.Metadata.JacketArtist,
                    BackgroundFileName = chart.Metadata.BackgroundFileName,
                    BackgroundArtist = chart.Metadata.BackgroundArtist,
                    DifficultyLevel = chart.Metadata.DifficultyLevel,
                    DifficultyIndex = chart.Metadata.DifficultyIndex,
                    DifficultyName = chart.Metadata.DifficultyName,
                    DifficultyNameShort = chart.Metadata.DifficultyNameShort,
                    DifficultyColor = chart.Metadata.DifficultyColor,
                };

                chart.Info = chartInfo;
                chartSetInfo.Charts.Add(chartInfo);
            }

            string nscChartDirectory = Path.Combine("charts", setName);
            if (!Directory.Exists(nscChartDirectory))
                Directory.CreateDirectory(nscChartDirectory);

            var serializer = ChartSerializer.GetSerializerFor(NeuroSonicGameMode.Instance);

            using (var setInfoStream = File.Open(Path.Combine(nscChartDirectory, ".tset"), FileMode.Create))
                serializer.SerializeSetInfo(chartSetInfo, setInfoStream);

            foreach (var (_, chart) in chartFiles)
            {
                var chartInfo = chart.Info;
                using (var chartInfoStream = File.Open(Path.Combine(nscChartDirectory, chartInfo.FileName), FileMode.Create))
                    serializer.SerializeChart(chart, chartInfoStream);
            }

            return chartSetInfo;
        }

        private void ConvertKSH()
    {
            var dialog = new OpenFileDialogDesc("Open KSH Chart",
                                new[] { new FileFilter("K-Shoot MANIA Files", "ksh") });

            var dialogResult = FileSystem.ShowOpenFileDialog(dialog);
            if (dialogResult.DialogResult == DialogResult.OK)
            {
                string primaryKshFile = dialogResult.FilePath;
                var chartSetInfo = ConvertKSHAndSave(primaryKshFile);

                Process.Start(Path.Combine("charts", chartSetInfo.FilePath));
            }
        }

        private void ConvertKSHAndOpen()
        {
            var dialog = new OpenFileDialogDesc("Open KSH Chart",
                                new[] { new FileFilter("K-Shoot MANIA Files", "ksh") });

            var dialogResult = FileSystem.ShowOpenFileDialog(dialog);
            if (dialogResult.DialogResult == DialogResult.OK)
            {
                string primaryKshFile = dialogResult.FilePath;
                var chartSetInfo = ConvertKSHAndSave(primaryKshFile);
            }
        }
    }
}
