using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using wgssSTU;

namespace InkPlatform.Hardware.Wacom
{
    public class STU500 : WacomSignpad
    {
        public static new ushort PID = 0x00a1;
        public static new string PRODUCT_MODEL = "STU-500";

        public STU500(IUsbDevice usbDevice) : base(usbDevice)
        {
            _pid = 0x00a1;
            _productModel = "STU-500";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = true;
            _supportColor = false;
            _deviceType = DEVICE_TYPE.SIGNPAD;
            
            _screenDimension = new Size(640, 480);
        }
    }
}
