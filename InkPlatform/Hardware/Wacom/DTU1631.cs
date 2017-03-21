using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTU1631 : WacomWintabDevice
    {
        public static new ushort PID = 0x000f0;
        public static new string PRODUCT_MODEL = "DTU-1631";
        public static string PID_STRING = "00f0";

        public DTU1631() : base()
        {
            _pid = 0x00f0;
            _productModel = "DTU-1631";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.PEN_DISPLAY;
            _screenDimension = new Size(1366, 768);

            ConnectionMode = CONNECTION_MODE.USB;
        }
    }
}
