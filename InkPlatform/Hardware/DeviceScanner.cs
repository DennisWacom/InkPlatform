using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wgssSTU;
using InkPlatform.Hardware.Wacom;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InkPlatform.Hardware
{
    public class DeviceScanner
    {
        private WacomDeviceScanner _wacomScanner;

        public delegate List<PenDevice> ScanDeviceDelegate();
        public ScanDeviceDelegate ScanAsync;

        public DeviceScanner()
        {
            _wacomScanner = new WacomDeviceScanner();
            ScanAsync = Scan;
        }
        
        public List<ushort> SupportedVidList()
        {
            List<ushort> results = new List<ushort>();
            results.Add(_wacomScanner.SupportedVid());
            return results;
        }

        public PenDevice IdentifyPenDevice(ushort vid, ushort pid)
        {
            PenDevice wacomPenDevice = _wacomScanner.IdentifyWacomSignpad(vid, pid);
            if(wacomPenDevice != null) { return wacomPenDevice; }

            return null;
        }

        public List<PenDevice> Scan()
        {
            List<PenDevice> results = new List<PenDevice>();
            
            results.AddRange(_wacomScanner.Scan());

            return results;   
        }
        
    }
}
