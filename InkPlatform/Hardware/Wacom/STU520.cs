using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using wgssSTU;

namespace InkPlatform.Hardware.Wacom
{
    public class STU520 : WacomSignpad
    {
        public static new ushort PID = 0x00a3;
        public static new string PRODUCT_MODEL = "STU-520";

        public STU520(IUsbDevice usbDevice) : base(usbDevice)
        {
            _pid = 0x00a3;
            _productModel = "STU-520";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.SIGNPAD;
            ConnectionMode = CONNECTION_MODE.USB;
            _screenDimension = new Size(800, 480);
        }
    }
}
