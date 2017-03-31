using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkPlatform.Hardware.Wacom
{
    /// <summary>
    /// Interface for device scanner. To create another device scanner for another vendor.
    /// </summary>
    public interface IDeviceScanner
    {
        List<PenDevice> Scan();
        ushort SupportedVid();
    }
}
