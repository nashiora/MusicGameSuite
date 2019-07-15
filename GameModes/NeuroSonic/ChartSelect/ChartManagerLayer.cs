using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

using theori;
using theori.Audio;
using theori.Charting;
using theori.Charting.IO;
using theori.IO;
using theori.Gui;
using theori.Graphics;

using NeuroSonic.Startup;
using NeuroSonic.GamePlay;
using NeuroSonic.Charting.KShootMania;
using NeuroSonic.Charting.Conversions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace NeuroSonic.ChartSelect
{
    internal class ChartManagerLayer : BaseMenuLayer
    {
        protected override string Title => "Chart Manager";

        private string m_chartsDir = Plugin.Config.GetString(NscConfigKey.StandaloneChartsDirectory);

        private Thread m_loadThread = null;
        private Layer m_nextLayer = null;

        protected override void GenerateMenuItems()
        {
            AddMenuItem(new MenuItem(NextOffset, "Open KSH Chart Directly", () => CreateThread(OpenKSH)));
            AddMenuItem(new MenuItem(NextOffset, "Open Theori Chart Directly", () => CreateThread(OpenTheori)));
            AddSpacing();
            AddMenuItem(new MenuItem(NextOffset, "Convert KSH Charts to Theori Set", () => CreateThread(ConvertKSH)));
            AddMenuItem(new MenuItem(NextOffset, "Convert KSH Charts and Open Selected", () => CreateThread(ConvertKSHAndOpen)));

            void CreateThread(ThreadStart function)
            {
                m_loadThread = new Thread(function);
                m_loadThread.SetApartmentState(ApartmentState.STA);
                m_loadThread.Start();
            }
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

                var loader = new GameLoadingLayer(Plugin.DefaultResourceLocator, chart, audio);
                m_nextLayer = loader;
            }
        }

        private void OpenTheori()
        {
            var dialog = new OpenFileDialogDesc("Open Theori Chart",
                                new[] { new FileFilter("music:theori Files", "theori") });

            var dialogResult = FileSystem.ShowOpenFileDialog(dialog);
            if (dialogResult.DialogResult == DialogResult.OK)
            {
                string theoriFile = dialogResult.FilePath;
                string theoriDirectory = Directory.GetParent(theoriFile).FullName;

                var setFiles = Directory.EnumerateFiles(theoriDirectory, "*.theori-set").ToArray();
                if (setFiles.Length == 0)
                {
                    Logger.Log("Failed to locate .theori-set file.");
                    return;
                }
                else if (setFiles.Length != 1)
                {
                    Logger.Log($"Too many .theori-set files, choosing the first ({ setFiles[0] }).");
                    return;
                }

                string setFile = setFiles[0];

                var setSerializer = new ChartSetSerializer();

                ChartSetInfo setInfo;
                using (var setStream = File.OpenRead(setFile))
                    setInfo = setSerializer.DeserializeChartSetInfo(setStream);
                setInfo.FilePath = theoriDirectory;

                var chartInfos = (from chartInfo in setInfo.Charts
                                  where chartInfo.FileName == Path.GetFileName(theoriFile)
                                  select chartInfo).ToArray();
                if (chartInfos.Length == 0)
                {
                    Logger.Log($"Set file { Path.GetFileName(setFile) } did not contain meta information for given chart { Path.GetFileName(theoriFile) }.");
                    return;
                }

                Debug.Assert(chartInfos.Length == 1, "Chart set deserialization returned multiple sets with the same file name!");
                var selected = chartInfos.Single();

                var loader = new GameLoadingLayer(Plugin.DefaultResourceLocator, selected);
                m_nextLayer = loader;

#if false
                var serializer = BinaryTheoriChartSerializer.GetSerializerFor(NeuroSonicGameMode.Instance);
                using (var stream = File.OpenRead(Path.Combine(m_chartsDir, setInfo.FilePath, selected.FileName)))
                {
                    var chart = serializer.DeserializeChart(selected, stream);
                    string audioFile = Path.Combine(m_chartsDir, setInfo.FilePath, chart.Info.SongFileName);

                    var audio = AudioTrack.FromFile(audioFile);
                    audio.Channel = Host.Mixer.MasterChannel;
                    audio.Volume = chart.Info.SongVolume / 100.0f;

                    AutoPlay autoPlay = AutoPlay.None;
                    if (Keyboard.IsDown(KeyCode.LCTRL) || Keyboard.IsDown(KeyCode.RCTRL))
                        autoPlay = AutoPlay.ButtonsAndLasers;

                    var game = new GameLayer(Plugin.DefaultResourceLocator, chart, audio, autoPlay);
                    Host.PushLayer(new GenericTransitionLayer(game, Plugin.DefaultResourceLocator));
                }
#endif
            }
        }

        private ChartSetInfo ConvertKSHAndSave(string primaryKshFile, out ChartInfo selected)
        {
            var primaryKshChart = KshChart.CreateFromFile(primaryKshFile);
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
                /*
                if ((kshMeta.MusicFile, kshMeta.Title, kshMeta.Artist) !=
                    (primaryKshMeta.MusicFile, primaryKshMeta.Title, primaryKshMeta.Artist))
                {
                    Logger.Log($"Skipping '{ Path.GetFileName(kshChartFile) }' chart file in the set '{ setDir }'.\n:theori and NeuroSonic only support a single song for each set, and the chosen set does not comply.\nOnly charts of the same song will be added to this converted set.");
                    continue;
                }
                */

                var kshChart = KshChart.CreateFromFile(kshChartFile);
                var chart = kshChart.ToVoltex();

                chartFiles.Add((kshChartFile, chart));
            }

            var chartSetInfo = new ChartSetInfo()
            {
                ID = 0, // no database ID, it's not in the database yet
                OnlineID = null, // no online stuff, it's not uploaded

                FilePath = setName,
            };

            string nscChartDirectory = Path.Combine(m_chartsDir, setName);
            if (!Directory.Exists(nscChartDirectory))
                Directory.CreateDirectory(nscChartDirectory);

            foreach (var (kshChartFile, chart) in chartFiles)
            {
                string audioFile = Path.Combine(setDir, chart.Info.SongFileName);
                if (File.Exists(audioFile))
                {
                    string audioFileDest = Path.Combine(m_chartsDir, setName, Path.GetFileName(audioFile));
                    if (File.Exists(audioFileDest))
                        File.Delete(audioFileDest);
                    File.Copy(audioFile, audioFileDest);
                }

                chart.Info.Set = chartSetInfo;
                chart.Info.FileName = $"{ Path.GetFileNameWithoutExtension(kshChartFile) }.theori";

                chartSetInfo.Charts.Add(chart.Info);
            }

            selected = primaryChart.Info;

            var setSerializer = new ChartSetSerializer();
            var serializer = BinaryTheoriChartSerializer.GetSerializerFor(NeuroSonicGameMode.Instance);

            using (var setInfoStream = File.Open(Path.Combine(nscChartDirectory, ".theori-set"), FileMode.Create))
                setSerializer.SerializeSetInfo(chartSetInfo, setInfoStream);

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
                var chartSetInfo = ConvertKSHAndSave(primaryKshFile, out _);

                Process.Start(Path.Combine(Plugin.Config.GetString(NscConfigKey.StandaloneChartsDirectory), chartSetInfo.FilePath));
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
                var chartSetInfo = ConvertKSHAndSave(primaryKshFile, out ChartInfo selected);

                var loader = new GameLoadingLayer(Plugin.DefaultResourceLocator, selected);
                m_nextLayer = loader;

#if false
                var serializer = BinaryTheoriChartSerializer.GetSerializerFor(NeuroSonicGameMode.Instance);
                using (var stream = File.OpenRead(Path.Combine(m_chartsDir, chartSetInfo.FilePath, selected.FileName)))
                {
                    var chart = serializer.DeserializeChart(selected, stream);
                    string audioFile = Path.Combine(m_chartsDir, chartSetInfo.FilePath, chart.Info.SongFileName);

                    var audio = AudioTrack.FromFile(audioFile);
                    audio.Channel = Host.Mixer.MasterChannel;
                    audio.Volume = chart.Info.SongVolume / 100.0f;

                    AutoPlay autoPlay = AutoPlay.None;
                    if (Keyboard.IsDown(KeyCode.LCTRL) || Keyboard.IsDown(KeyCode.RCTRL))
                        autoPlay = AutoPlay.ButtonsAndLasers;

                    var game = new GameLayer(Plugin.DefaultResourceLocator, chart, audio, autoPlay);
                    Host.PushLayer(new GenericTransitionLayer(game, Plugin.DefaultResourceLocator));
                }
#endif
            }
        }

        public override void Update(float delta, float total)
        {
            base.Update(delta, total);

            if (m_loadThread != null)
            {
                if (m_loadThread.ThreadState == System.Threading.ThreadState.Stopped)
                    m_loadThread = null;
                return;
            }

            if (m_nextLayer != null)
            {
                Host.PushLayer(m_nextLayer);
                m_nextLayer = null;
            }
        }
    }
}
