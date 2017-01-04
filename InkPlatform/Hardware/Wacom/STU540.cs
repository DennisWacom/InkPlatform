using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using wgssSTU;

namespace InkPlatform.Hardware.Wacom
{
    public class STU540 : WacomSignpad
    {
        public static new ushort PID = 0x00a7;
        public static new string PRODUCT_MODEL = "STU-540";

        public STU540(IUsbDevice usbDevice) : base(usbDevice)
        {
            _pid = 0x00a7;
            _productModel = "STU-540";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = true;
            _supportColor = true;
            _deviceType = DEVICE_TYPE.SIGNPAD;
            _screenDimension = new Size(800, 480);
        }
    }
}
