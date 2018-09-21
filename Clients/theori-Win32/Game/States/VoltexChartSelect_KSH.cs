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
    class TempUiManager
    {
        private Panel root;

        private GuiElement currentHover;

        public TempUiManager(Panel root)
        {
            this.root = root;
        }

        // TODO(local): move to update instead plz
        private void Mouse_ButtonPress(MouseButton button)
        {
            
        }

        public void Update()
        {
            var underCursor = new SortedList<float, GuiElement>();
            var mousePos = Mouse.Position;
            
            ScanChildren(root);
            void ScanChildren(Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child.ContainsScreenPoint(mousePos))
                        underCursor.Add(0, child);

                    if (child is Panel childPanel)
                        ScanChildren(childPanel);
                }
            }

            var targetChild = underCursor.FirstOrDefault().Value;
            if (currentHover != targetChild)
            {
                if (currentHover != null)
                {
                    Logger.Log("Just unhovered old thing");
                }
                currentHover = targetChild;
                if (currentHover != null)
                {
                    Logger.Log("Just hovered new thing");
                }
            }

            if (Mouse.IsPressed(MouseButton.Left))
            {
                if (currentHover != null) currentHover.OnMouseButtonPress(MouseButton.Left);
            }
        }
    }

    class TempButtonTest : Panel
    {
        private Sprite image;

        public Action Pressed;

        public TempButtonTest()
        {
            Children = new GuiElement[]
            {
                image = new Sprite(OpenGL.Texture.Empty)
                {
                    RelativeSizeAxes = Axes.Both,
                    Size = Vector2.One,
                },
            };
        }

        public override void Update()
        {
            base.Update();

            if (ContainsScreenPoint(Mouse.Position))
                image.Color = new Vector4(1, 1, 0, 1);
            else image.Color = Vector4.One;
        }

        public override bool OnMouseButtonPress(MouseButton button)
        {
            Pressed?.Invoke();
            Logger.Log("TempButtonTest pressed");
            return true;
        }
    }

    class VoltexChartSelect_KSH : State
    {
        private TempUiManager uiManager;
        private Panel foreUiRoot;
        private TempButtonTest openButton;

        public override void Init()
        {
            foreUiRoot = new Panel()
            {
                Children = new GuiElement[]
                {
                    openButton = new TempButtonTest()
                    {
                        Size = new Vector2(200, 50),
                        Position = new Vector2(50, 50),

                        Pressed = OpenChart,
                    },
                }
            };

            uiManager = new TempUiManager(foreUiRoot);

            Keyboard.KeyPress += Keyboard_KeyPress;
        }

        private void OpenChart()
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

                    string audioFile = Path.Combine(fileDir, ksh.Metadata.MusicFile ?? ksh.Metadata.MusicFileNoFx);

                    var audio = AudioTrack.FromFile(audioFile);
                    audio.Channel = Host.Mixer.MasterChannel;
                    audio.Volume = ksh.Metadata.MusicVolume / 100.0f;

                    var voltex = new VoltexGameplay(ksh.ToVoltex(), audio);
                    Host.PushState(voltex);
                }
            }
        }

        private void Keyboard_KeyPress(KeyInfo key)
        {
            if (key.KeyCode == KeyCode.O)
            {
                OpenChart();
            }
        }

        public override void Update()
        {
            foreUiRoot.Update();
            uiManager.Update();
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
