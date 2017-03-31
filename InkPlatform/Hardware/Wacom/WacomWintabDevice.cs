using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    /// <summary>
    /// Represents a Wacom pen device, default with the wacom vendor id
    /// </summary>
    /// <seealso cref="InkPlatform.Hardware.WintabDevice" />
    public class WacomWintabDevice : WintabDevice
    {
        public static new ushort VID = 0x056a;
        public static new string VENDOR_NAME = strings.WACOM;
        public static string VID_STRING = "056A";

        /// <summary>
        /// Initializes a new instance of the <see cref="WacomWintabDevice"/> class.
        /// </summary>
        public WacomWintabDevice() : base()
        {
            Log("Wacom Wintab device created");
            _vid = 0x056a;
            _vendorName = strings.WACOM;
            _sensorResolution = 2540;
        }

    }
}
