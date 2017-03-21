using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTK1651 : WacomWintabDevice
    {
        public static new ushort PID = 0x0343;
        public static new string PRODUCT_MODEL = "DTK-1651";
        public static string PID_STRING = "0343";

        public DTK1651() : base()
        {
            _pid = 0x0343;
            _productModel = "DTK-1651";
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
