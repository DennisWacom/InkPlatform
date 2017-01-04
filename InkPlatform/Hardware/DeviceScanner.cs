using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wgssSTU;
using InkPlatform.Hardware.Wacom;

namespace InkPlatform.Hardware
{
    public class DeviceScanner
    {
        private WacomDeviceScanner _wacomScanner;

        public DeviceScanner()
        {
            _wacomScanner = new WacomDeviceScanner();
        }
        
        public List<ushort> SupportedVidList()
        {
            List<ushort> results = new List<ushort>();
            results.Add(_wacomScanner.SupportedVid());
            return results;
        }

        public List<PenDevice> Scan()
        {
            List<PenDevice> results = new List<PenDevice>();
            
            results.AddRange(_wacomScanner.Scan());

            return results;   
        }
    }
}
