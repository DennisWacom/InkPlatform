using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    public interface IDeviceScanner
    {
        List<PenDevice> Scan();
        ushort SupportedVid();
    }
}
