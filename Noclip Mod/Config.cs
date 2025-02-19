using OMP.LSWTSS.CApi1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMP.LSWTSS;

public static class Config
{
    private const string configPath = @"mods/noclip/config.ini";

    private static INIFile configFile;

    public static float HorizontalSpeed = 1f;
    public static float VerticalSpeed = 0.5f;

    public static int ToggleKey = (int)PInvoke.User32.VirtualKey.VK_DELETE;
    public static int ReloadKey = (int)PInvoke.User32.VirtualKey.VK_END;

    public static int ForwardsKey = (int)PInvoke.User32.VirtualKey.VK_W;
    public static int BackwardsKey = (int)PInvoke.User32.VirtualKey.VK_S;
    public static int LeftKey = (int)PInvoke.User32.VirtualKey.VK_A;
    public static int RightKey = (int)PInvoke.User32.VirtualKey.VK_D;

    public static int UpKey = (int)PInvoke.User32.VirtualKey.VK_PRIOR;
    public static int DownKey = (int)PInvoke.User32.VirtualKey.VK_NEXT;

    public static void LoadConfig()
    {
        configFile = new INIFile(configPath);

        float.TryParse(configFile.IniReadValue("Config", "HorizontalSpeed"), out HorizontalSpeed);
        float.TryParse(configFile.IniReadValue("Config", "VerticalSpeed"), out VerticalSpeed);

        Int32.TryParse(configFile.IniReadValue("Inputs", "ToggleKey"), out ToggleKey);
        Int32.TryParse(configFile.IniReadValue("Inputs", "ReloadKey"), out ReloadKey);

        Int32.TryParse(configFile.IniReadValue("Inputs", "ForwardsKey"), out ForwardsKey);
        Int32.TryParse(configFile.IniReadValue("Inputs", "BackwardsKey"), out BackwardsKey);
        Int32.TryParse(configFile.IniReadValue("Inputs", "LeftKey"), out LeftKey);
        Int32.TryParse(configFile.IniReadValue("Inputs", "RightKey"), out RightKey);

        Int32.TryParse(configFile.IniReadValue("Inputs", "UpKey"), out UpKey);
        Int32.TryParse(configFile.IniReadValue("Inputs", "DownKey"), out DownKey);
    }
}
