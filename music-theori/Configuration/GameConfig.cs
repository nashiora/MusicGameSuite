namespace theori.Configuration
{
    public class GameConfig : Config<GameConfigKey>
    {
        protected override void SetDefaults()
        {
	        Set(GameConfigKey.ScreenWidth, 1280);
	        Set(GameConfigKey.ScreenHeight, 720);
	        Set(GameConfigKey.FullScreenWidth, -1);
	        Set(GameConfigKey.FullScreenHeight, -1);
	        Set(GameConfigKey.Fullscreen, false);
            Set(GameConfigKey.FullscreenMonitorIndex, 0);
            Set(GameConfigKey.Maximized, false);
            Set(GameConfigKey.MasterVolume, 0.65f);
	        Set(GameConfigKey.ScreenX, -1);
	        Set(GameConfigKey.ScreenY, -1);
	        Set(GameConfigKey.VSync, 0);
	        Set(GameConfigKey.FPSTarget, 0);
            Set(GameConfigKey.Controller_DeviceID, 0);
        }
    }

    public enum GameConfigKey
    {
	    ScreenWidth,
	    ScreenHeight,
	    FullScreenWidth,
	    FullScreenHeight,
	    ScreenX,
	    ScreenY,
	    Fullscreen,
	    FullscreenMonitorIndex,
	    VSync,
        Maximized,

        MasterVolume,
        FPSTarget,

        Controller_DeviceID,
    }
}
