using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wgssSTU;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InkPlatform.Hardware.Wacom
{
    /// <summary>
    /// Class to detect and identify wacom devices connected
    /// </summary>
    /// <seealso cref="InkPlatform.Hardware.Wacom.IDeviceScanner" />
    public class WacomDeviceScanner : IDeviceScanner
    {
        /// <summary>
        /// The vendor id for wacom device : 056A (Hex) or 1386 (Dec)
        /// </summary>
        public static int WACOM_VID = 1386;

        /// <summary>
        /// Structure to hold COM information for serial device connection
        /// </summary>
        public struct COM_CONNECTION
        {
            public string COM_NO;
            public string COM_TYPE;
        }

        public ushort SupportedVid()
        {
            return WacomSignpad.VID;
        }

        /// <summary>
        /// List to contain all recognized devices from the Hardware folder
        /// </summary>
        List<PenDevice> _wacomPenDevices;

        /// <summary>
        /// Initializes a new instance of the <see cref="WacomDeviceScanner"/> class. This will call the 
        /// LoadRecognizedDevices to load the json files from the Hardware folder
        /// </summary>
        public WacomDeviceScanner()
        {
            _wacomPenDevices = LoadRecognizedDevices();
        }

        /// <summary>
        /// Loads the json files in the Hardware\[Vendor] folder. 
        /// </summary>
        /// <returns>List of pen devices recognized</returns>
        public List<PenDevice> LoadRecognizedDevices()
        {
            List<PenDevice> result = new List<PenDevice>();

            try
            {
                string execFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string hardwareFolder = Path.Combine(execFolder, "Hardware");
                string[] vendorFolders = Directory.GetDirectories(hardwareFolder);
                foreach (string vendorFolder in vendorFolders)
                {
                    string[] files = Directory.GetFiles(vendorFolder, "*.json");
                    foreach (string file in files)
                    {
                        string json = File.ReadAllText(file);
                        try
                        {
                            JObject o = JObject.Parse(json);
                            int vid = o["Vid"].Value<int>();
                            int deviceType = o["DeviceType"].Value<int>();

                            if (vid == WacomDeviceScanner.WACOM_VID)
                            {
                                if (deviceType == (int)DEVICE_TYPE.SIGNPAD)
                                {
                                    WacomSignpad signpad = JsonConvert.DeserializeObject<WacomSignpad>(json);
                                    result.Add(signpad);
                                }
                                else
                                {
                                    WacomWintabDevice wintab = JsonConvert.DeserializeObject<WacomWintabDevice>(json);
                                    result.Add(wintab);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.Write(ex.Message);
                        }

                    }
                }
            }
            catch (Exception)
            {

            }
            
            return result;
        }

        /// <summary>
        /// Scans for any plugged in pen devices, looking for usb signpad, serial signpad, and wintab devices
        /// </summary>
        /// <returns>List of pen devices connected</returns>
        public List<PenDevice> Scan()
        {
            List<PenDevice> result = new List<PenDevice>();

            result.AddRange(ScanUsbSignpad());
            result.AddRange(ScanSerialSignpad());
            result.AddRange(ScanWintabDevice());

            return result;
        }

        /// <summary>
        /// Gets the wacom pen device for the specific vid and pid
        /// </summary>
        /// <param name="vid">The vid.</param>
        /// <param name="pid">The pid.</param>
        /// <returns>Pendevice connected</returns>
        public PenDevice GetWacomPenDevice(ushort vid, ushort pid)
        {
            foreach(PenDevice device in _wacomPenDevices)
            {
                if(device.Vid == vid && device.Pid == pid)
                {
                    return device;
                }
            }
            return null;
        }

        /// <summary>
        /// Identifies the wacom signpad for the specified vid and pid
        /// </summary>
        /// <param name="vid">The vid.</param>
        /// <param name="pid">The pid.</param>
        /// <returns>Wacom signpad connected</returns>
        public WacomSignpad IdentifyWacomSignpad(ushort vid, ushort pid)
        {

            WacomSignpad result = (WacomSignpad)GetWacomPenDevice(vid, pid);

            if(result != null)
            {
                return result;
            }

            if (vid != WacomSignpad.VID) return null;

            if (pid == STU300.PID) return new STU300();
            if (pid == STU430.PID) return new STU430();
            if (pid == STU430V.PID) return new STU430V();
            if (pid == STU500.PID) return new STU500();
            if (pid == STU520.PID) return new STU520();
            if (pid == STU530.PID) return new STU530();
            if (pid == STU540.PID) return new STU540();

            return null;
        }

        /// <summary>
        /// Identifies the wacom pen display by the deviceid string as gotten from the win32_pnpentity
        /// </summary>
        /// <param name="DeviceIdString">The device identifier string.</param>
        /// <returns>Wintab device connected</returns>
        public WintabDevice IdentifyWacomPenDisplay(string DeviceIdString)
        {
            // USB\VID_056A&PID_00FB\3CZQ003595
            if (DeviceIdString.Substring(0, 3) != strings.USB) return null;

            try
            {
                string vidString = DeviceIdString.Substring(8, 4);
                string pidString = DeviceIdString.Substring(17, 4);
                ushort vid = ushort.Parse(vidString, System.Globalization.NumberStyles.HexNumber);
                ushort pid = ushort.Parse(pidString, System.Globalization.NumberStyles.HexNumber);
                WintabDevice device = (WintabDevice)GetWacomPenDevice(vid, pid);
                if(device != null)
                {
                    return device;
                }
            }
            catch (Exception)
            {

            }
            
            if (DeviceIdString.Substring(8, 4) == WacomSignpad.VID_STRING)
            {
                if (DeviceIdString.Substring(17, 4) == DTU1031.PID_STRING) return new DTU1031();
                if (DeviceIdString.Substring(17, 4) == DTU1031X.PID_STRING) return new DTU1031X();
                if (DeviceIdString.Substring(17, 4) == DTU1141.PID_STRING) return new DTU1141();
                if (DeviceIdString.Substring(17, 4) == DTU1631.PID_STRING) return new DTU1631();
                if (DeviceIdString.Substring(17, 4) == DTU1931.PID_STRING) return new DTU1931();
                if (DeviceIdString.Substring(17, 4) == DTU2231.PID_STRING) return new DTU2231();
                if (DeviceIdString.Substring(17, 4) == DTK1651.PID_STRING) return new DTK1651();
                if (DeviceIdString.Substring(17, 4) == DTK2241.PID_STRING) return new DTK2241();
            }

            return null;
        }

        /// <summary>
        /// Identifies the wacom signpad with the usbDevice (from STU SDK)
        /// </summary>
        /// <param name="usbDevice">The usb device.</param>
        /// <returns>Wacom signpad connected</returns>
        public WacomSignpad IdentifyWacomSignpad(IUsbDevice usbDevice)
        {
            if (usbDevice == null) return null;
            WacomSignpad signpad = (WacomSignpad)GetWacomPenDevice(usbDevice.idVendor, usbDevice.idProduct);
            if(signpad != null)
            {
                return signpad;
            }
            
            if (usbDevice.idVendor != WacomSignpad.VID) return null;

            if (usbDevice.idProduct == STU300.PID) return new STU300(usbDevice);
            if (usbDevice.idProduct == STU430.PID) return new STU430(usbDevice);
            if (usbDevice.idProduct == STU430V.PID) return new STU430V(usbDevice);
            if (usbDevice.idProduct == STU500.PID) return new STU500(usbDevice);
            if (usbDevice.idProduct == STU520.PID) return new STU520(usbDevice);
            if (usbDevice.idProduct == STU530.PID) return new STU530(usbDevice);
            if (usbDevice.idProduct == STU540.PID) return new STU540(usbDevice);

            return null;
        }

        /// <summary>
        /// Scans the usb signpad, using the UsbDevices class from Wacom's STU SDK
        /// </summary>
        /// <returns>list of wacom stu signpads connected</returns>
        public List<PenDevice> ScanUsbSignpad()
        {
            List<PenDevice> result = new List<PenDevice>();

            UsbDevices usbDevices = new UsbDevices();
            if (usbDevices == null) return result;

            for(int i=0; i<usbDevices.Count; i++)
            {
                WacomSignpad signpad = IdentifyWacomSignpad(usbDevices[i]);
                if(signpad != null)
                {
                    result.Add(signpad);
                }
            }
            
            return result;
        }

        /// <summary>
        /// Scans for any serial signpad.
        /// </summary>
        /// <returns>List of connected serial signpads</returns>
        public List<PenDevice> ScanSerialSignpad()
        {
            List<COM_CONNECTION> coms = new List<COM_CONNECTION>();
            try
            {
                coms = DetectSerialPorts();
            }
            catch (Exception)
            {

            }

            List<PenDevice> result = new List<PenDevice>();
            return result;
        }

        /// <summary>
        /// Scans for any pen displays using the Win32_PnpEntity from Windows function
        /// </summary>
        /// <returns>List of pen displays connected</returns>
        public List<PenDevice> ScanWintabDevice()
        {
            List<PenDevice> result = new List<PenDevice>();

            try
            {
                ManagementScope scope = new ManagementScope(@"\\" + Environment.MachineName + @"\root\CIMV2");
                SelectQuery query = new SelectQuery(@"Select * from Win32_PnPEntity");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                using (ManagementObjectCollection collection = searcher.Get())
                {
                    foreach (var device in collection)
                    {
                        string deviceId = (string)device.GetPropertyValue("DeviceID");
                        if (deviceId.Contains(WacomSignpad.VID_STRING))
                        {
                            WintabDevice wtDevice = IdentifyWacomPenDisplay(deviceId);
                            if (wtDevice != null)
                            {
                                result.Add(wtDevice);
                            }

                            //Console.WriteLine((string)device.GetPropertyValue("DeviceID"));
                            //Console.WriteLine((string)device.GetPropertyValue("Name"));
                            //Console.WriteLine((string)device.GetPropertyValue("Manufacturer"));
                            //Console.WriteLine((string)device.GetPropertyValue("Description"));
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
            
            return result;
        }

        /// <summary>
        /// Scans serial ports for COM connections - serial sign pads
        /// </summary>
        /// <returns>List of serial sign pad connected</returns>
        /// <exception cref="Exception"></exception>
        public List<COM_CONNECTION> DetectSerialPorts()
        {
            string utilitiesFolder = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "Utilities");
            string getSerialPortsPath = Path.Combine(utilitiesFolder, "getSerialPorts.exe");
            if (!File.Exists(getSerialPortsPath))
            {
                throw new Exception(strings.GetSerialPortsNotFound);
            }
            Process getSerialPorts = new Process();
            getSerialPorts.StartInfo.FileName = getSerialPortsPath;
            getSerialPorts.StartInfo.UseShellExecute = false;
            getSerialPorts.StartInfo.CreateNoWindow = true;
            getSerialPorts.StartInfo.RedirectStandardOutput = true;
            getSerialPorts.Start();

            string output = getSerialPorts.StandardOutput.ReadToEnd();
            getSerialPorts.WaitForExit();

            List<COM_CONNECTION> result = new List<COM_CONNECTION>();

            if (output != null && output.Length > 0)
            {
                string[] connections = output.Split(Environment.NewLine.ToCharArray());
                foreach (string conn in connections)
                {
                    string[] items = conn.Split(' ');
                    if (items.Length < 2) continue;
                    try
                    {
                        COM_CONNECTION com = new COM_CONNECTION();
                        com.COM_NO = items[0];
                        com.COM_TYPE = items[items.Length - 1];
                        result.Add(com);
                    }
                    catch (Exception) { }
                }
            }

            return result;
        }
    }
}
