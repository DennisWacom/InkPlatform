using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wgssSTU;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace InkPlatform.Hardware.Wacom
{
    public class WacomDeviceScanner : IDeviceScanner
    {
        public struct COM_CONNECTION
        {
            public string COM_NO;
            public string COM_TYPE;
        }

        public ushort SupportedVid()
        {
            return WacomSignpad.VID;
        }

        public List<PenDevice> Scan()
        {
            List<PenDevice> result = new List<PenDevice>();

            result.AddRange(ScanUsbSignpad());
            result.AddRange(ScanSerialSignpad());
            result.AddRange(ScanWintabDevice());

            return result;
        }

        public WacomSignpad IdentifyWacomSignpad(ushort vid, ushort pid)
        {
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
        
        public WacomSignpad IdentifyWacomSignpad(IUsbDevice usbDevice)
        {
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
