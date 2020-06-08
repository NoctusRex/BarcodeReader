using NoRe.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeReader.Misc
{
    public class Settings : Configuration
    {

        public bool StartAtStartup { get; set; }
        public bool StartAsNotifyIcon { get; set; }

        public override void Read()
        {
            Settings temp = Read<Settings>();
            StartAsNotifyIcon = temp.StartAsNotifyIcon;
            StartAtStartup = temp.StartAtStartup;
        }

    }
}
