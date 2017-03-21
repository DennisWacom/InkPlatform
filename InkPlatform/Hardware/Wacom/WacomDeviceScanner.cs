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
    public class WacomDeviceScanner : IDeviceScanner
    {
        public static int WACOM_VID = 1386;

        public struct COM_CONNECTION
        {
            public string COM_NO;
            public string COM_TYPE;
        }

        public ushort SupportedVid()
        {
            return WacomSignpad.VID;
        }

        List<PenDevice> _wacomPenDevices;

        public WacomDeviceScanner()
        {
            _wacomPenDevices = LoadRecognizedDevices();
        }

        public List<PenDevice> LoadRecognizedDevices()
        {
            List<PenDevice> result = new List<PenDevice>();

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

            return result;
        }

        public List<PenDevice> Scan()
        {
            List<PenDevice> result = new List<PenDevice>();

            result.AddRange(ScanUsbSignpad());
            result.AddRange(ScanSerialSignpad());
            result.AddRange(ScanWintabDevice());

            return result;
        }
        
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

        public WacomSignpad IdentifyWacomSignpad(ushort vid, ushort pid)
        {

            return (WacomSignpad)GetWacomPenDevice(vid, pid);

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
                return (WintabDevice)GetWacomPenDevice(vid, pid);
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

        public WacomSignpad IdentifyWacomSignpad(IUsbDevice usbDevice)
        {
            if (usbDevice == null) return null;
            return (WacomSignpad)GetWacomPenDevice(usbDevice.idVendor, usbDevice.idProduct);

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

        public List<PenDevice> ScanWintabDevice()
        {
            List<PenDevice> result = new List<PenDevice>();

            ManagementObjectCollection collection;
            using(var searcher = new ManagementObjectSearcher(@"Select * from Win32_PnPEntity"))
            {
                collection = searcher.Get();

                foreach (var device in collection)
                {
                    string deviceId = (string)device.GetPropertyValue("DeviceID");
                    if (deviceId.Contains(WacomSignpad.VID_STRING))
                    {
                        WintabDevice wtDevice = IdentifyWacomPenDisplay(deviceId);
                        if(wtDevice != null)
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
            

            return result;
        }

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
