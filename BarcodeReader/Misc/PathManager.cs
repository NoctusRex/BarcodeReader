using NoRe.Core;

namespace BarcodeReader.Misc
{
    public static class PathManager
    {
        public static string DatabasePath => System.IO.Path.Combine(Pathmanager.StartupDirectory, "History.db");
        public static string SettingsPath => System.IO.Path.Combine(Pathmanager.StartupDirectory, "Settings.json");
        public static string BarcodeEngineDirectory => System.IO.Path.Combine(Pathmanager.StartupDirectory, "BarcodeEngines");
    }
}
