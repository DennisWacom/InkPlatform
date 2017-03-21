using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class WacomWintabDevice : WintabDevice
    {
        public static new ushort VID = 0x056a;
        public static new string VENDOR_NAME = strings.WACOM;
        public static string VID_STRING = "056A";

        public WacomWintabDevice() : base()
        {
            Log("Wacom Wintab device created");
            _vid = 0x056a;
            _vendorName = strings.WACOM;
            _sensorResolution = 2540;
        }

    }
}
