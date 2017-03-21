using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public class DTU1931 : WacomWintabDevice
    {
        public static new ushort PID = 0x00c7;
        public static new string PRODUCT_MODEL = "DTU-1931";
        public static string PID_STRING = "00C7";

        public DTU1931() : base()
        {
            _pid = 0x00C7;
            _productModel = "DTU-1931";
            _maxPressureLevels = 1024;
            _maxReportRate = 197;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.PEN_DISPLAY;
            _screenDimension = new Size(1280, 1024);

            ConnectionMode = CONNECTION_MODE.USB;
        }
    }
}
