using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware
{
    public class Signpad : PenDevice
    {
        public Signpad() : base()
        {
            _deviceType = DEVICE_TYPE.SIGNPAD;
        }

        public override int ClearScreen()
        {
            throw new NotImplementedException();
        }

        public override int Connect()
        {
            throw new NotImplementedException();
        }

        public override int Disconnect()
        {
            throw new NotImplementedException();
        }

        public override int DisplayBitmap(Bitmap bmp)
        {
            throw new NotImplementedException();
        }

        public override int RefreshScreen()
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
