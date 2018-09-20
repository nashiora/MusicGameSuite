using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenRM.Convert;
using theori.Audio;
using theori.Graphics;
using theori.Gui;
using theori.Platform;

namespace theori.Game.States
{
    class VoltexChartSelect_KSH : State
    {
        private Panel foreUiRoot;

        public override void Init()
        {
            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                }
            };

            Keyboard.KeyPress += Keyboard_KeyPress;
        }

        private void Keyboard_KeyPress(KeyInfo key)
        {
            if (key.KeyCode == KeyCode.O)
            {
                if (RuntimeInfo.IsWindows)
                {
                    var dialog = new OpenFileDialogDesc("Open Chart",
                                     new[] { new FileFilter("K-Shoot MANIA Files", "ksh") });

                    var result = FileSystem.ShowOpenFileDialog(dialog);
                    if (result.DialogResult == DialogResult.OK)
                    {
                        string kshChart = result.FilePath;

                        string fileDir = Directory.GetParent(kshChart).FullName;
                        var ksh = KShootMania.Chart.CreateFromFile(kshChart);

                        string audioFile = Path.Combine(fileDir, ksh.Metadata.MusicFileNoFx ?? ksh.Metadata.MusicFile);

                        var audio = AudioTrack.FromFile(audioFile);
                        audio.Channel = Host.Mixer.MasterChannel;
                        audio.Volume = ksh.Metadata.MusicVolume / 100.0f;

                        var voltex = new VoltexGameplay(ksh.ToVoltex(), audio);
                        Host.PushState(voltex);

                        return;
                    }
                }
            }
        }

        public override void Update()
        {
            foreUiRoot.Update();
        }

        public override void Render()
        {
            void DrawUiRoot(Panel root)
            {
                if (root == null) return;

                var viewportSize = new Vector2(Window.Width, Window.Height);
                using (var grq = new GuiRenderQueue(viewportSize))
                {
                    root.Position = Vector2.Zero;
                    root.RelativeSizeAxes = Axes.None;
                    root.Size = viewportSize;
                    root.Rotation = 0;
                    root.Scale = Vector2.One;
                    root.Origin = Vector2.Zero;

                    root.Render(grq);
                }
            }

            DrawUiRoot(foreUiRoot);
        }
    }
}
