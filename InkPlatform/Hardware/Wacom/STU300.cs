using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using wgssSTU;

namespace InkPlatform.Hardware.Wacom
{
    public class STU300 : WacomSignpad
    {
        public static new ushort PID = 0x00a2;
        public static new string PRODUCT_MODEL = "STU-300";

        public STU300(IUsbDevice usbDevice) : base(usbDevice)
        {
            _pid = 0x00a2;
            _productModel = "STU-300";
            _maxPressureLevels = 1024;
            _maxReportRate = 200;
            _hasScreen = true;
            _supportUsb = true;
            _supportSerial = false;
            _supportColor = false;
            _deviceType = DEVICE_TYPE.SIGNPAD;
            _screenDimension = new Size(396, 100);

            ConnectionMode = CONNECTION_MODE.USB;
        }

        
    }
}
