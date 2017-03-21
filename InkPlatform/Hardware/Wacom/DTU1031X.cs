using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTU1031X : WacomWintabDevice
    {
        public static new ushort PID = 0x032f;
        public static new string PRODUCT_MODEL = "DTU-1031X";
        public static string PID_STRING = "032F";

        public DTU1031X() : base()
        {
            _pid = 0x032f;
            _productModel = "DTU-1031X";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.PEN_DISPLAY;
            _screenDimension = new Size(1024, 600);

            ConnectionMode = CONNECTION_MODE.USB;
        }
    }
}
