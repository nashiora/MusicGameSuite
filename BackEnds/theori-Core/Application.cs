using System;
using System.Diagnostics;
using CSCore;
using CSCore.Codecs;
using OpenGL;
using theori.Audio;
using theori.Audio.NVorbis;
using theori.Configuration;
using theori.Game;
using theori.Game.States;
using theori.Graphics;
using theori.Input;

namespace theori
{
    public static class Application
    {
        public static Mixer Mixer { get; private set; }

        public static GameConfig GameConfig { get; private set; }

        internal static ProgramPipeline Pipeline { get; private set; }

        private static State state;

        public static void Start()
        {
            GameConfig = new GameConfig();
            // TODO(local): load config

            Window.Create();
            Window.VSync = VSyncMode.Off;
            Logger.Log($"Window VSync: { Window.VSync }");

            Window.ClientSizeChanged += (w, h) =>
            {
                state.ClientSizeChanged(w, h);
            };
            
            Window.Update();
            InputManager.Initialize();

            Pipeline = new ProgramPipeline();
            Pipeline.Bind();
            
            CodecFactory.Instance.Register("ogg-vorbis", new CodecFactoryEntry(s => new NVorbisSource(s).ToWaveSource(), ".ogg"));
            Mixer = new Mixer(2, 48000);

            state = new VoltexGameplay();

            state.Init();

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

                    state.Update();

                    
                    if (Window.Width > 0 && Window.Height > 0)
                    {
                        GL.ClearColor(0, 0, 0, 1);
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                        state.Render();

                        Window.SwapBuffer();
                    }
                }
            }
        }

        public static void Quit(int code = 0)
        {
            Gamepad.Destroy();
            Window.Destroy();

            Environment.Exit(code);
        }
    }
}
