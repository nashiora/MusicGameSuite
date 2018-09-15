using System;
using System.Runtime.InteropServices;
using System.Text;
using OpenGL;
using theori.Input;
using static SDL2.SDL;

namespace theori.Graphics
{
    public enum VSyncMode
    {
        Adaptive = -1,
        Off = 0,
        On = 1,
    }

    public static class Window
    {
        public static bool ShouldExitApplication { get; private set; }

        private static IntPtr window, context;
        
        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static float Aspect => (float)Width / Height;

        public static event Action<int, int> ClientSizeChanged;

        private static VSyncMode vsync;
        public static VSyncMode VSync
        {
            get => vsync;
            set
            {
                if (SDL_GL_SetSwapInterval((int)value) == -1)
                    vsync = (VSyncMode)SDL_GL_GetSwapInterval();
                else vsync = value;
            }
        }

        public static void Create()
        {
            if (Window.window != IntPtr.Zero)
                throw new InvalidOperationException("Only one Window can be created at a time.");

            if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_JOYSTICK) != 0)
            {
                string err = SDL_GetError();
                Logger.Log(err, LogCategory.System, LogPriority.Error);
                // can't continue, sorry
                Application.Quit(1);
            }
            
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, (int)SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 3);

            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);

            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_MULTISAMPLEBUFFERS, 1);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, 16);

            window = SDL_CreateWindow("theori", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, Width = 1280, Height = 720, 
                SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_SHOWN);

            SDL_DisableScreenSaver();

            if (window == IntPtr.Zero)
            {
                string err = SDL_GetError();
                Logger.Log(err, LogCategory.System, LogPriority.Error);
                // can't continue, sorry
                Application.Quit(1);
            }

            context = SDL_GL_CreateContext(window);
            SDL_GL_MakeCurrent(window, context);
            
            if (SDL_GL_SetSwapInterval(1) == -1)
            {
                string err = SDL_GetError();
                Logger.Log(err, LogCategory.System, LogPriority.Error);
            }
            vsync = (VSyncMode)SDL_GL_GetSwapInterval();

		    Logger.Log($"OpenGL Version: { GL.GetString(GL.GL_VERSION) }");
		    Logger.Log($"OpenGL Shading Language Version: { GL.GetString(GL.GL_SHADING_LANGUAGE_VERSION) }");
		    Logger.Log($"OpenGL Renderer: { GL.GetString(GL.GL_RENDERER) }");
		    Logger.Log($"OpenGL Vendor: { GL.GetString(GL.GL_VENDOR) }");

            GL.Enable(GL.GL_MULTISAMPLE);

            GL.Enable(GL.GL_BLEND);
            //GL.Enable(GL.GL_DEPTH_TEST);
            GL.BlendFunc(BlendingSourceFactor.SourceAlpha, BlendingDestinationFactor.OneMinusSourceAlpha);
        }

        internal static void Destroy()
        {
            SDL_EnableScreenSaver();

            SDL_GL_DeleteContext(context);
            SDL_DestroyWindow(window);

            SDL_Quit();
        }

        internal static void SwapBuffer()
        {
            SDL_GL_SwapWindow(window);
        }

        internal static unsafe void Update()
        {
            SDL_GL_MakeCurrent(window, context);

            while (SDL_PollEvent(out var evt) != 0)
            {
                switch (evt.type)
                {
                    case SDL_EventType.SDL_QUIT: ShouldExitApplication = true; break;
                        
                    case SDL_EventType.SDL_KEYDOWN:
                    case SDL_EventType.SDL_KEYUP:
                        {
                            var sym = evt.key.keysym;
                            var info = new KeyInfo()
                            {
                                ScanCode = (ScanCode)sym.scancode,
                                KeyCode = (KeyCode)sym.sym,
                                Mods = (KeyMod)sym.mod,
                            };

                            if (evt.type == SDL_EventType.SDL_KEYDOWN)
                                Keyboard.InvokePress(info);
                            else Keyboard.InvokeRelease(info);
                        }
                        break;

                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        {
                            Mouse.x = evt.button.x;
                            Mouse.y = evt.button.y;
                            
                            if (evt.type == SDL_EventType.SDL_MOUSEBUTTONDOWN)
                                Mouse.InvokePress((MouseButton)evt.button.which);
                            else Mouse.InvokeRelease((MouseButton)evt.button.which);
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEMOTION:
                        {
                            Mouse.x = evt.motion.x;
                            Mouse.y = evt.motion.y;

                            Mouse.InvokeMove(evt.motion.xrel, evt.motion.yrel);
                        }
                        break;
                    case SDL_EventType.SDL_MOUSEWHEEL:
                    {
                        int y = evt.wheel.y;
                        if (evt.wheel.direction != (uint)SDL_MouseWheelDirection.SDL_MOUSEWHEEL_FLIPPED)
                            y = -y;
                        Mouse.InvokeScroll(evt.wheel.x, y);
                    } break;
                        
                    case SDL_EventType.SDL_TEXTEDITING:
                    {
                        SDL_Rect rect;
                        SDL_GetWindowPosition(window, out rect.x, out rect.y);
                        SDL_GetWindowSize(window, out rect.w, out rect.h);
                        SDL_SetTextInputRect(ref rect);

                        byte[] bytes = new byte[32];
                        Marshal.Copy(new IntPtr(evt.edit.text), bytes, 0, 32);

                        string composition = Encoding.UTF8.GetString(bytes, 0, Array.IndexOf<byte>(bytes, 0));
                        int cursor = evt.edit.start;
                        int selectionLength = evt.edit.length;
                    } break;
                    case SDL_EventType.SDL_TEXTINPUT:
                    {
                        byte[] bytes = new byte[32];
                        Marshal.Copy(new IntPtr(evt.edit.text), bytes, 0, 32);

                        string composition = Encoding.UTF8.GetString(bytes, 0, Array.IndexOf<byte>(bytes, 0));
                    } break;
                        
                    case SDL_EventType.SDL_CONTROLLERDEVICEADDED:
                    {
                        int id = evt.cdevice.which;
                        string name = SDL_GameControllerNameForIndex(id);

                        Logger.Log($"Constroller Added: [{ id }] { name }", LogCategory.System, LogPriority.Verbose); 
                    } break;
                    case SDL_EventType.SDL_CONTROLLERDEVICEREMOVED:
                    {
                        int id = evt.cdevice.which;
                        string name = SDL_GameControllerNameForIndex(id);

                        Logger.Log($"Constroller Removed: [{ id }] { name }", LogCategory.System, LogPriority.Verbose);
                    } break;
                    case SDL_EventType.SDL_CONTROLLERDEVICEREMAPPED:
                    {
                        int id = evt.cdevice.which;
                        string name = SDL_GameControllerNameForIndex(id);

                        Logger.Log($"Constroller Remapped: [{ id }] { name }", LogCategory.System, LogPriority.Verbose);
                    } break;
                    case SDL_EventType.SDL_CONTROLLERAXISMOTION: break;
                    case SDL_EventType.SDL_CONTROLLERBUTTONDOWN: break;
                    case SDL_EventType.SDL_CONTROLLERBUTTONUP: break;

                    case SDL_EventType.SDL_JOYDEVICEADDED: Gamepad.HandleAddedEvent(evt.jdevice.which); break;
                    case SDL_EventType.SDL_JOYDEVICEREMOVED: Gamepad.HandleRemovedEvent(evt.jdevice.which); break;

                    case SDL_EventType.SDL_JOYAXISMOTION:
                    {
                        Logger.Log($"Joystick[{ evt.jaxis.which }].Axis{ evt.jaxis.axis } = { evt.jaxis.axisValue }");
                        Gamepad.HandleAxisEvent(evt.jaxis.which, evt.jaxis.axis, evt.jaxis.axisValue);
                    } break;
                    case SDL_EventType.SDL_JOYBALLMOTION: break;
                    case SDL_EventType.SDL_JOYBUTTONDOWN:
                    {
                        Gamepad.HandleInputEvent(evt.jbutton.which, evt.jaxis.axis, 1);
                    } break;
                    case SDL_EventType.SDL_JOYBUTTONUP:
                    {
                        Gamepad.HandleInputEvent(evt.jbutton.which, evt.jaxis.axis, 0);
                    } break;
                    case SDL_EventType.SDL_JOYHATMOTION: break;
                        
                    case SDL_EventType.SDL_DROPBEGIN: break;
                    case SDL_EventType.SDL_DROPCOMPLETE: break;
                    case SDL_EventType.SDL_DROPFILE: break;
                    case SDL_EventType.SDL_DROPTEXT: break;

                    case SDL_EventType.SDL_AUDIODEVICEADDED: Logger.Log("Audio Device Added: " + evt.adevice.which, LogCategory.System, LogPriority.Verbose); break;
                    case SDL_EventType.SDL_AUDIODEVICEREMOVED: Logger.Log("Audio Device Removed: " + evt.adevice.which, LogCategory.System, LogPriority.Verbose); break;

                    case SDL_EventType.SDL_CLIPBOARDUPDATE: break;
                        
                    case SDL_EventType.SDL_WINDOWEVENT:
                        switch (evt.window.windowEvent)
                        {
                            case SDL_WindowEventID.SDL_WINDOWEVENT_CLOSE: ShouldExitApplication = true; break;
                                
                            case SDL_WindowEventID.SDL_WINDOWEVENT_TAKE_FOCUS: SDL_SetWindowInputFocus(window); break;
                                
                            case SDL_WindowEventID.SDL_WINDOWEVENT_ENTER: break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_LEAVE: break;
                                
                            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED: break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST: break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_SIZE_CHANGED:
                                Width = evt.window.data1;
                                Height = evt.window.data2;
                                break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                                Width = evt.window.data1;
                                Height = evt.window.data2;
                                GL.Viewport(0, 0, Width, Height);
                                ClientSizeChanged?.Invoke(Width, Height);
                                break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_MOVED: break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_HIDDEN: break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_SHOWN: break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_MAXIMIZED: break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_MINIMIZED: break;
                            case SDL_WindowEventID.SDL_WINDOWEVENT_RESTORED: break;

                            case SDL_WindowEventID.SDL_WINDOWEVENT_EXPOSED: break;
                        }
                        break;
                        
                    case SDL_EventType.SDL_RENDER_DEVICE_RESET: break;
                    case SDL_EventType.SDL_RENDER_TARGETS_RESET: break;
                        
                    case SDL_EventType.SDL_SYSWMEVENT: break;

                    case SDL_EventType.SDL_USEREVENT: break;

                    default: break;
                }
            }
        }
    }
}
