using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTU2231 : WacomWintabDevice
    {
        public static new ushort PID = 0x00ce;
        public static new string PRODUCT_MODEL = "DTU-2231";
        public static string PID_STRING = "00CE";

        public DTU2231() : base()
        {
            _pid = 0x00CE;
            _productModel = "DTU-2231";
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
