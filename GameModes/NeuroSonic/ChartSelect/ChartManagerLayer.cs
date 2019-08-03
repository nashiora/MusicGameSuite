using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

using theori;
using theori.Audio;
using theori.Charting;
using theori.Charting.Serialization;
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
            //AddMenuItem(new MenuItem(NextOffset, "Open Theori Chart Directly", () => CreateThread(OpenTheori)));
            AddSpacing();
            //AddMenuItem(new MenuItem(NextOffset, "Convert KSH Charts to Theori Set", () => CreateThread(ConvertKSH)));
            //AddMenuItem(new MenuItem(NextOffset, "Convert KSH Charts and Open Selected", () => CreateThread(ConvertKSHAndOpen)));

            void CreateThread(ThreadStart function)
            {
                m_loadThread = new Thread(function);
                m_loadThread.SetApartmentState(ApartmentState.STA);
                m_loadThread.Start();
            }
        }

        private void OpenKSH()
        {
            AutoPlay autoPlay = Keyboard.IsDown(KeyCode.LCTRL) || Keyboard.IsDown(KeyCode.RCTRL) ? AutoPlay.ButtonsAndLasers : AutoPlay.None;
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

                var loader = new GameLoadingLayer(Plugin.DefaultResourceLocator, chart, audio, autoPlay);
                m_nextLayer = loader;
            }
        }

        private void OpenTheori()
        {
            AutoPlay autoPlay = Keyboard.IsDown(KeyCode.LCTRL) || Keyboard.IsDown(KeyCode.RCTRL) ? AutoPlay.ButtonsAndLasers : AutoPlay.None;
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

                string fullChartsDir = Path.GetFullPath(m_chartsDir);
                string setDirectory = Path.GetFileName(fullChartsDir);
                if (theoriDirectory.Contains(fullChartsDir))
                    setDirectory = setFile.Substring(theoriDirectory.Length + 1);

                var setSerializer = new ChartSetSerializer();
                ChartSetInfo setInfo = setSerializer.LoadFromFile(m_chartsDir, theoriDirectory, setFile);

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

                var loader = new GameLoadingLayer(Plugin.DefaultResourceLocator, selected, autoPlay);
                m_nextLayer = loader;
            }
        }

        private ChartSetInfo ConvertKSHAndSave(string primaryKshFile, out ChartInfo selected)
        {
            var primaryKshChart = KshChart.CreateFromFile(primaryKshFile);
            var primaryChart = primaryKshChart.ToVoltex();

            string setDir = Directory.GetParent(primaryKshFile).FullName;
            // Since we can't know where the parent 'charts' directory might be for this chart is,
            //  or even if it exists, when converting this way we only care that the first directory
            //  up is the name of the set directory rather than the whole path thru a 'charts' directory.
            // A more feature-complete converter will instead ask where the root charts directory is and do this more accurately.
            string setName = Path.GetFileName(setDir);

            var chartFiles = new List<(string, Chart)> { (primaryKshFile, primaryChart) };
            foreach (string kshChartFile in Directory.EnumerateFiles(setDir, "*.ksh"))
            {
                if (Path.GetFileName(kshChartFile) == Path.GetFileName(primaryKshFile)) continue;

                KshChartMetadata kshMeta;
                using (var reader = new StreamReader(File.OpenRead(kshChartFile)))
                    kshMeta = KshChartMetadata.Create(reader);

                var kshChart = KshChart.CreateFromFile(kshChartFile);
                var chart = kshChart.ToVoltex();

                chartFiles.Add((kshChartFile, chart));
            }

            var chartSetInfo = new ChartSetInfo()
            {
                ID = 0, // no database ID, it's not in the database yet
                OnlineID = null, // no online stuff, it's not uploaded

                FilePath = setName,
                FileName = ".theori-set",
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

            var s = new ChartSerializer(m_chartsDir, NeuroSonicGameMode.Instance);
            foreach (var (_, chart) in chartFiles)
                s.SaveToFile(chart);

            var setSerializer = new ChartSetSerializer();
            setSerializer.SaveToFile(m_chartsDir, chartSetInfo);

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
            AutoPlay autoPlay = Keyboard.IsDown(KeyCode.LCTRL) || Keyboard.IsDown(KeyCode.RCTRL) ? AutoPlay.ButtonsAndLasers : AutoPlay.None;
            var dialog = new OpenFileDialogDesc("Open KSH Chart",
                                new[] { new FileFilter("K-Shoot MANIA Files", "ksh") });

            var dialogResult = FileSystem.ShowOpenFileDialog(dialog);
            if (dialogResult.DialogResult == DialogResult.OK)
            {
                string primaryKshFile = dialogResult.FilePath;
                var chartSetInfo = ConvertKSHAndSave(primaryKshFile, out ChartInfo selected);

                var loader = new GameLoadingLayer(Plugin.DefaultResourceLocator, selected, autoPlay);
                m_nextLayer = loader;
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
