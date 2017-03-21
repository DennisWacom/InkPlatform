using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTU1031 : WacomWintabDevice
    {
        public static new ushort PID = 0x00fb;
        public static new string PRODUCT_MODEL = "DTU-1031";
        public static string PID_STRING = "00FB";

        public DTU1031() : base()
        {
            _pid = 0x00fb;
            _productModel = "DTU-1031";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.PEN_DISPLAY;
            _screenDimension = new Size(1280, 800);

            ConnectionMode = CONNECTION_MODE.USB;
        }
    }
}
