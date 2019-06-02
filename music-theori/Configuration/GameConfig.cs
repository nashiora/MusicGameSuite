using theori.IO;

namespace theori.Configuration
{
    public class GameConfig : Config<GameConfigKey>
    {
        public object GetIntGameConfigKey { get; internal set; }

        protected override void SetDefaults()
        {
	        Set(GameConfigKey.ScreenWidth, 1280);
	        Set(GameConfigKey.ScreenHeight, 720);
	        Set(GameConfigKey.FullScreenWidth, -1);
	        Set(GameConfigKey.FullScreenHeight, -1);
	        Set(GameConfigKey.Fullscreen, false);
	        Set(GameConfigKey.FullscreenMonitorIndex, 0);
	        Set(GameConfigKey.MasterVolume, 1.0f);
	        Set(GameConfigKey.ScreenX, -1);
	        Set(GameConfigKey.ScreenY, -1);
	        Set(GameConfigKey.VSync, 0);
	        Set(GameConfigKey.HiSpeed, 1.0f);
	        Set(GameConfigKey.GlobalOffset, 0);
	        Set(GameConfigKey.InputOffset, 0);
	        Set(GameConfigKey.FPSTarget, 0);
	        Set(GameConfigKey.LaserAssistLevel, 1.5f);
	        Set(GameConfigKey.HiSpeedModKind, HiSpeedMod.Default);
	        Set(GameConfigKey.ModSpeed, 300.0f);
	        Set(GameConfigKey.ChartFolder, "charts");
	        Set(GameConfigKey.Skin, "Default");
	        Set(GameConfigKey.Laser0Color, 200.0f);
	        Set(GameConfigKey.Laser1Color, 330.0f);

	        // Input settings
	        Set(GameConfigKey.ButtonInputDevice, InputDevice.Controller);
	        Set(GameConfigKey.LaserInputDevice, InputDevice.Controller);

	        // Default keyboard bindings
	        Set(GameConfigKey.Key_BTS, KeyCode.D1); // Start button on Dao controllers
	        Set(GameConfigKey.Key_BT0, KeyCode.D);
	        Set(GameConfigKey.Key_BT1, KeyCode.F);
	        Set(GameConfigKey.Key_BT2, KeyCode.J);
	        Set(GameConfigKey.Key_BT3, KeyCode.K);
	        Set(GameConfigKey.Key_BT0Alt, -1);
	        Set(GameConfigKey.Key_BT1Alt, -1);
	        Set(GameConfigKey.Key_BT2Alt, -1);
	        Set(GameConfigKey.Key_BT3Alt, -1);
	        Set(GameConfigKey.Key_FX0, KeyCode.C);
	        Set(GameConfigKey.Key_FX1, KeyCode.M);
	        Set(GameConfigKey.Key_FX0Alt, -1);
	        Set(GameConfigKey.Key_FX1Alt, -1);
	        Set(GameConfigKey.Key_Laser0Neg, KeyCode.W);
	        Set(GameConfigKey.Key_Laser0Pos, KeyCode.E);
	        Set(GameConfigKey.Key_Laser1Neg, KeyCode.O);
	        Set(GameConfigKey.Key_Laser1Pos, KeyCode.P);
	        Set(GameConfigKey.Key_Sensitivity, 3.0f);
	        Set(GameConfigKey.Key_LaserReleaseTime, 0.0f);

	        // Default controller settings
	        Set(GameConfigKey.Controller_DeviceID, 0); // First device
	        Set(GameConfigKey.Controller_BTS, 0);
	        Set(GameConfigKey.Controller_BT0, 1);
	        Set(GameConfigKey.Controller_BT1, 2);
	        Set(GameConfigKey.Controller_BT2, 3);
	        Set(GameConfigKey.Controller_BT3, 4);
	        Set(GameConfigKey.Controller_FX0, 5);
	        Set(GameConfigKey.Controller_FX1, 6);
	        Set(GameConfigKey.Controller_Laser0Axis, 0);
	        Set(GameConfigKey.Controller_Laser1Axis, 1);
	        Set(GameConfigKey.Controller_Sensitivity, 1.0f);
	        Set(GameConfigKey.Controller_Deadzone, 0.0f);

	        // Default mouse settings
	        Set(GameConfigKey.Mouse_Laser0Axis, 0);
	        Set(GameConfigKey.Mouse_Laser1Axis, 1);
	        Set(GameConfigKey.Mouse_Sensitivity, 1.0f);
        }
    }
    
    public enum HiSpeedMod
    {
        Default = 0,
        MMod, CMod,
    }

    public enum GameConfigKey
    {
	    // Screen settings
	    ScreenWidth,
	    ScreenHeight,
	    FullScreenWidth,
	    FullScreenHeight,
	    ScreenX,
	    ScreenY,
	    Fullscreen,
	    FullscreenMonitorIndex,
	    MasterVolume,
	    VSync,

	    // Game settings
	    HiSpeed,
        HiSpeedModKind,
        ModSpeed,
	    GlobalOffset,
	    InputOffset,
	    ChartFolder,
	    Skin,
	    Laser0Color,
	    Laser1Color,
	    FPSTarget,
	    LaserAssistLevel,

	    // Input device setting per element
	    LaserInputDevice,
	    ButtonInputDevice,
        
	    // Mouse settings (primary axes are x=0, y=1)
	    Mouse_Laser0Axis,
	    Mouse_Laser1Axis,
	    Mouse_Sensitivity,

	    // Key bindings
	    Key_BTS,
	    Key_BT0,
	    Key_BT1,
	    Key_BT2,
	    Key_BT3,
	    Key_BT0Alt,
	    Key_BT1Alt,
	    Key_BT2Alt,
	    Key_BT3Alt,
	    Key_FX0,
	    Key_FX1,
	    Key_FX0Alt,
	    Key_FX1Alt,
	    Key_Laser0Pos,
	    Key_Laser1Pos,
	    Key_Laser0Neg,
	    Key_Laser1Neg,
	    Key_Sensitivity,
	    Key_LaserReleaseTime,

	    // Controller bindings
	    Controller_DeviceID,
	    Controller_BTS,
	    Controller_BT0,
	    Controller_BT1,
	    Controller_BT2,
	    Controller_BT3,
	    Controller_FX0,
	    Controller_FX1,
	    Controller_Laser0Axis,
	    Controller_Laser1Axis,
	    Controller_Deadzone,
	    Controller_Sensitivity
    }
}
