using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserControls;
using InkPlatform.UserInterface;

namespace InkPlatformTest
{
    class PenDeviceTester
    {
        public void TestPenDeviceEquals()
        {
            DeviceScanner _deviceScanner = new DeviceScanner();
            List<PenDevice> pdList = _deviceScanner.Scan();
            Console.Write("Test same device equals: ");
            if(pdList.Count > 0)
            {
                Console.WriteLine(pdList[0].IsSameDevice(pdList[0])?"Pass":"Fail");
            }
            else
            {
                Console.WriteLine("Unavailable");
            }

            Console.Write("Test different device equals: ");
            if (pdList.Count > 1)
            {
                Console.WriteLine(pdList[0].IsSameDevice(pdList[1]) ? "Fail" : "Pass");
            }
            else
            {
                Console.WriteLine("Unavailable");
            }

        }

    }
}
