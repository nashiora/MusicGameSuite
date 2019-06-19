namespace theori.IO
{
    public enum MouseButton : uint
    {
        Unknown = 0,

        Left = SDL2.SDL.SDL_BUTTON_LEFT,
        Middle = SDL2.SDL.SDL_BUTTON_MIDDLE,
        Right = SDL2.SDL.SDL_BUTTON_RIGHT,
        X1 = SDL2.SDL.SDL_BUTTON_X1,
        X2 = SDL2.SDL.SDL_BUTTON_X2,
    }
}
