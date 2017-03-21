using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTU1141 : WacomWintabDevice
    {
        public static new ushort PID = 0x0336;
        public static new string PRODUCT_MODEL = "DTU-1141";
        public static string PID_STRING = "0336";

        public DTU1141() : base()
        {
            _pid = 0x0336;
            _productModel = "DTU-1141";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
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
