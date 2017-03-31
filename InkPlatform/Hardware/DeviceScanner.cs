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
    /// <summary>
    /// Class to scan for any connected signpads and pen displays
    /// </summary>
    public class DeviceScanner
    {
        /// <summary>
        /// The scanner class which specifically only scans for wacom devices
        /// </summary>
        private WacomDeviceScanner _wacomScanner;

        public delegate List<PenDevice> ScanDeviceDelegate();
        public ScanDeviceDelegate ScanAsync;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceScanner"/> class.
        /// </summary>
        public DeviceScanner()
        {
            _wacomScanner = new WacomDeviceScanner();
            ScanAsync = Scan;
        }

        /// <summary>
        /// Gets the list of supported vendor ids.
        /// </summary>
        /// <returns>List of supported vendor ids</returns>
        public List<ushort> SupportedVidList()
        {
            List<ushort> results = new List<ushort>();
            results.Add(_wacomScanner.SupportedVid());
            return results;
        }

        /// <summary>
        /// Identifies the pen device.
        /// </summary>
        /// <param name="vid">The vendor id</param>
        /// <param name="pid">The product id</param>
        /// <returns></returns>
        public PenDevice IdentifyPenDevice(ushort vid, ushort pid)
        {
            PenDevice wacomPenDevice = _wacomScanner.IdentifyWacomSignpad(vid, pid);
            if(wacomPenDevice != null) { return wacomPenDevice; }

            return null;
        }

        /// <summary>
        /// Scans for any connected pen device
        /// </summary>
        /// <returns>List of connected pen devices</returns>
        public List<PenDevice> Scan()
        {
            List<PenDevice> results = new List<PenDevice>();
            
            results.AddRange(_wacomScanner.Scan());

            return results;   
        }
        
    }
}
