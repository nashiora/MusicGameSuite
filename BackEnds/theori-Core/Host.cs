using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CSCore;
using CSCore.Codecs;
using OpenGL;
using theori.Audio;
using theori.Audio.NVorbis;
using theori.Configuration;
using theori.Game;
using theori.Graphics;
using theori.Input;
using theori.Platform;

namespace theori
{
    public static class Host
    {
        public static IPlatform Platform { get; private set; }

        public static Mixer Mixer { get; private set; }
        
        public static GameInput GameInput { get; private set; }
        public static GameConfig GameConfig { get; private set; }

        internal static ProgramPipeline Pipeline { get; private set; }

        private static readonly Stack<State> states = new Stack<State>();
        private static State State => states.Count == 0 ? null : states.Peek();

        public static void PushState(State state)
        {
            states.Push(state);
            // TODO(local): queue up inits better
            state.Init();
        }

        public static State PopState()
        {
            return states.Pop();
        }

        public static void Init(IPlatform platformImpl)
        {
            Platform = platformImpl;

            GameConfig = new GameConfig();
            // TODO(local): load config
            GameInput = new GameInput(GameConfig.GetInt(GameConfigKey.Controller_DeviceID));

            Window.Create();
            Window.VSync = VSyncMode.Off;
            Logger.Log($"Window VSync: { Window.VSync }");

            Window.ClientSizeChanged += (w, h) =>
            {
                State.ClientSizeChanged(w, h);
            };
            
            Window.Update();
            InputManager.Initialize();

            Pipeline = new ProgramPipeline();
            Pipeline.Bind();
            
            CodecFactory.Instance.Register("ogg-vorbis", new CodecFactoryEntry(s => new NVorbisSource(s).ToWaveSource(), ".ogg"));
            Mixer = new Mixer(2);

            #if DEBUG
            string cd = Environment.CurrentDirectory;
            while (!Directory.Exists(Path.Combine(cd, "InstallDir")))
                cd = Directory.GetParent(cd).FullName;
            Environment.CurrentDirectory = Path.Combine(cd, "InstallDir");
            #endif
        }

        public static void Start(State initialState)
        {
            PushState(initialState);

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
                    
                    Keyboard.Update();
                    Mouse.Update();
                    Window.Update();

                    if (Window.ShouldExitApplication)
                    {
                        Quit();
                        return;
                    }

                    State.Update();

                    if (Window.Width > 0 && Window.Height > 0)
                    {
                        GL.ClearColor(0, 0, 0, 1);
                        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                        State.Render();

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
