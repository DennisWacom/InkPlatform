using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTK2241 : WacomWintabDevice
    {
        public static new ushort PID = 0x0057;
        public static new string PRODUCT_MODEL = "DTK-2241";
        public static string PID_STRING = "0057";

        public DTK2241() : base()
        {
            _pid = 0x0057;
            _productModel = "DTK-2241";
            _maxPressureLevels = 1024;
            _maxReportRate = 197;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.PEN_DISPLAY;
            _screenDimension = new Size(1920, 1080);

            ConnectionMode = CONNECTION_MODE.USB;
        }
    }
}
