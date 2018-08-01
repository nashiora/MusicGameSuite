using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

using OpenGL;

using FxMania.Configuration;
using FxMania.Game;
using FxMania.Graphics;
using FxMania.Input;
using FxMania.Gui;
using OpenRM.Voltex;
using OpenRM;

namespace FxMania
{
    public static class Application
    {
        public static GameConfig GameConfig { get; private set; }

        internal static ProgramPipeline Pipeline { get; private set; }

        public static void Start()
        {
            GameConfig = new GameConfig();
            // TODO(local): load config

            Window.Create();
            Window.VSync = VSyncMode.Off;
            Logger.Log($"Window VSync: { Window.VSync }");

            Window.ClientSizeChanged += (w, h) =>
            {
                highwayView.Camera.AspectRatio = Window.Aspect;
            };
            
            Window.Update();
            InputManager.Initialize();

            Pipeline = new ProgramPipeline();
            Pipeline.Bind();

            TEMP_Init();

            var timer = Stopwatch.StartNew();

            long lastFrameStart = timer.ElapsedMilliseconds;
            long targetFrameTimeMillis = 1_000 / 240;

            while (!Window.ShouldExitApplication)
            {
                long currentTime = timer.ElapsedMilliseconds;
                long elapsedTime = currentTime - lastFrameStart;

                if (elapsedTime > targetFrameTimeMillis)
                {
                    currentTime = timer.ElapsedMilliseconds;
			        long actualDeltaTime = currentTime - lastFrameStart;

                    lastFrameStart = currentTime;

                    Time.Delta = actualDeltaTime / 1_000.0f;
                    Time.Total = lastFrameStart / 1_000.0f;
                    
                    Window.Update();
                    if (Window.ShouldExitApplication)
                    {
                        Quit();
                        return;
                    }

                    Tick();
                    DrawFrame();
                }
            }
        }

        static float leftVol = 0, rightVol = 0;
        static int actionKind = 0;

        private static void KeyboardButtonPress(KeyInfo key)
        {
            switch (key.KeyCode)
            {
                case KeyCode.D1: actionKind = 0; break;
                case KeyCode.D2: actionKind = 1; break;
                case KeyCode.D3: actionKind = 2; break;
            }
        }

        private static void GamepadButtonPressed(uint btn)
        {
            int dir;
            switch (btn)
            {
                case 1: dir = -1; break;
                case 4: dir =  1; break;
                default: return;
            }

            switch (actionKind)
            {
                case 0: control.ApplySpin(new SpinParams()
                {
                    Direction = (AngularDirection)dir,
                    Duration = 2.0,
                }); break;
                    
                case 1: control.ApplySwing(new SwingParams()
                {
                    Direction = (AngularDirection)dir,
                    Duration = 1.0,
                    Amplitude = 45,
                }); break;
                    
                case 2: control.ApplyWobble(new WobbleParams()
                {
                    Direction = (LinearDirection)dir,
                    Duration = 1.0,
                    Amplitude = 1,
                    Decay = Decay.On,
                    Frequency = 3,
                }); break;
            }
        }

        private static void Tick()
        {
            var gp = InputManager.Gamepad;
            if (gp)
            {
                const float LASER_SPEED = 600.0f;

                if (gp.ButtonDown(2))
                    leftVol = MathL.Min(1, leftVol + Time.Delta * LASER_SPEED);
                else leftVol = 0;
                
                if (gp.ButtonDown(3))
                    rightVol = MathL.Min(1, rightVol + Time.Delta * LASER_SPEED);
                else rightVol = 0;

                if (gp.ButtonDown(0))
                    control.LeftLaserParams = new LaserParams()
                    {
                        Function = LaserFunction.OneMinusSource,
                    };
            }

            control.LeftLaserInput = leftVol;
            control.RightLaserInput = rightVol;

            control.Update();
            control.ApplyToView(highwayView);

            highwayView.Update();
            
            critRoot.LaserRoll = highwayView.LaserRoll;
            critRoot.AddRoll = highwayView.TargetBaseRoll;
            critRoot.AddOffset = highwayView.TargetOffset * 2;
            critRoot.HorizonHeight = highwayView.HorizonHeight;
            critRoot.CriticalHeight = highwayView.CriticalHeight;

            rootPanel.Update();
            
            if (m_square.ContainsScreenPoint(Mouse.Position))
                m_square.Color = new Vector4(1, 1, 0, 1);
            else m_square.Color = Vector4.One;
        }

        private static void DrawFrame()
        {
            if (Window.Width > 0 && Window.Height > 0)
            {
                GL.ClearColor(0, 0, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                highwayView.Render();
                if (rootPanel != null)
                {
                    var viewportSize = new Vector2(Window.Width, Window.Height);
                    using (var grq = new GuiRenderQueue(viewportSize))
                    {
                        rootPanel.Position = Vector2.Zero;
                        rootPanel.RelativeSizeAxes = Axes.None;
                        rootPanel.Size = viewportSize;
                        rootPanel.Rotation = 0;
                        rootPanel.Scale = Vector2.One;
                        rootPanel.Origin = Vector2.Zero;

                        rootPanel.Render(grq);
                    }
                }

                Window.SwapBuffer();
            }
        }

        public static void Quit(int code = 0)
        {
            Gamepad.Destroy();
            Window.Destroy();

            Environment.Exit(code);
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        [VertexType(VertexData.Vector3, VertexData.Vector2)]
        public struct VertexP3T2
        {
            public Vector3 Position;
            public Vector2 TexCoords;

            public VertexP3T2(Vector3 pos, Vector2 coords)
            {
                Position = pos;
                TexCoords = coords;
            }
        }
        
        static HighwayControl control;
        static HighwayView highwayView;
        static Panel rootPanel;
        static CriticalLine critRoot;
        static Sprite m_square;

        private static void TEMP_Init()
        {
            var gp = InputManager.Gamepad;
            if (gp)
            {
                gp.ButtonPressed += GamepadButtonPressed;
            }

            Keyboard.KeyPress += KeyboardButtonPress;

            highwayView = new HighwayView();
            control = new HighwayControl();

            rootPanel = new Panel()
            {
                Children = new GuiElement[]
                {
                    critRoot = new CriticalLine(),
                    m_square = new Sprite(Texture.Empty)
                    {
                        Size = new Vector2(50, 50),
                        Origin = new Vector2(50, 50) / 2,
                        Position = new Vector2(50, 50) * 2,
                        Rotation = 30,
                        Scale = Vector2.One * 2,
                    },
                }
            };
        }
    }
}
