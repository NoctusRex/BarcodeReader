using NoRe.Core;
using System.Windows.Forms;

namespace BarcodeReader.Misc
{
    public class Settings : Configuration
    {

        public bool StartAtStartup { get; set; }
        public bool StartAsNotifyIcon { get; set; }
        public Keys ScanHotkey { get; set; }
        public Keys RescanHotkey { get; set; }
        public string BarcodeEngine { get; set; }
        public bool UseCopyPaste { get; set; }
        public bool SimulateKeyPress { get; set; }

        public override void Read()
        {
            Settings temp = Read<Settings>();
            StartAsNotifyIcon = temp.StartAsNotifyIcon;
            StartAtStartup = temp.StartAtStartup;
            ScanHotkey = temp.ScanHotkey;
            RescanHotkey = temp.RescanHotkey;
            BarcodeEngine = temp.BarcodeEngine;
            UseCopyPaste = temp.UseCopyPaste;
            SimulateKeyPress = temp.SimulateKeyPress;
        }

    }
}
