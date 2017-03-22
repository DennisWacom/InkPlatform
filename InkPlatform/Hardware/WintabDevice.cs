using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WintabDN;
using InkPlatform.Ink;

namespace InkPlatform.Hardware
{
    public class WintabDevice : PenDevice
    {
        private CWintabContext _context = null;
        private CWintabData _data = null;
        
        public WintabDevice()
        {
            Log("WintabDevice created");
            //_vid = 0x056a;
            //_vendorName = strings.WINTAB_DEVICE;
            //_sensorResolution = 2540;
            //initTablet();
        }

        public override int MaxPressureLevels
        {
            get
            {
                if(_maxPressureLevels == 0)
                {
                    _maxPressureLevels = CWintabInfo.GetMaxPressure();
                }
                return _maxPressureLevels;
            }
        }

        public override Size TabletDimension
        {
            get
            {
                int maxX = CWintabInfo.GetDeviceAxis(0, EAxisDimension.AXIS_X).axMax;
                int maxY = CWintabInfo.GetDeviceAxis(0, EAxisDimension.AXIS_Y).axMax;

                _tabletDimension = new Size(maxX, maxY);

                return _tabletDimension;
            }
            
        }

        public override int DisplayBitmap(Bitmap bmp)
        {
            Log("Display Bitmap");
            return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;
        }

        public override int RefreshScreen()
        {
            Log("Refresh Screen");
            return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;
        }

        public override int ClearScreen()
        {
            Log("Clear Screen");
            return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;
        }

        public override int Connect()
        {
            Log("Connect");
            try
            {
                _context = OpenQueryDigitizerContext(true);
                return (int)PEN_DEVICE_ERROR.NONE;
            }
            catch (Exception)
            {
                return (int)PEN_DEVICE_ERROR.CANNOT_CONNECT;
            }
        }
        public override int Disconnect()
        {
            Log("Disconnect");
            try
            {
                CloseCurrentContext();
                return (int)PEN_DEVICE_ERROR.NONE;
            }
            catch (Exception)
            {
                return (int)PEN_DEVICE_ERROR.UNSPECIFIED;
            }
        }

        public override bool IsConnected()
        {
            return _context == null ? false : true;
        }

        public override void Reset()
        {
            Log("Reset");
            Disconnect();
            Connect();
        }

        private CWintabContext OpenQueryDigitizerContext(bool enable)
        {
            bool status = false;
            CWintabContext logContext = null;
            
            if (enable)
            {
                logContext = CWintabInfo.GetDefaultSystemContext();
                // In Wintab, the tablet origin is lower left.  Move origin to upper left
                // so that it coincides with screen origin.
                logContext.OutExtY = -logContext.OutExtY;
            }
            else
            {
                logContext = CWintabInfo.GetDefaultDigitizingContext(ECTXOptionValues.CXO_MESSAGES);
                logContext.OutExtX = logContext.SysExtX;
                logContext.OutExtY = -logContext.SysExtY;
            }

            if (logContext == null)
            {
                throw new Exception();
            }

            logContext.Name = "Context";

            int deviceCount = (int)CWintabInfo.GetNumberOfDevices();

            string info = CWintabInfo.GetDeviceInfo();
            
            status = logContext.Open();
            _data = new CWintabData(logContext);
            _data.SetWTPacketEventHandler(PacketHandler);
            _connectionId = DateTime.Now.ToString("HHmmss");
            
            return logContext;
        }

        private void PacketHandler(Object sender, MessageReceivedEventArgs args)
        {
            try
            {
                uint pktId = (uint)args.Message.WParam;
                WintabPacket pkt = _data.GetDataPacket(pktId);


                if (pkt.pkContext == _context.HCtx)
                {
                    //lblX.Text = "X: " + pkt.pkX.ToString();
                    //lblY.Text = "Y: " + pkt.pkY.ToString();
                    if (OnPenData != null)
                    {
                        OnPenData(convertInkData(pkt));
                    }
                }
            }
            catch (Exception)
            {

            }
            
        }

        private InkData convertInkData(WintabPacket pkt)
        {
            InkData ink = new InkData();
            ink.x = (uint)pkt.pkX;
            ink.y = (uint)pkt.pkY;
            ink.p = (uint)pkt.pkNormalPressure;
            ink.contact = pkt.pkNormalPressure > 0 ? true : false;
            ink.proximity = ((byte)pkt.pkStatus & (byte)EWintabPacketStatusValue.TPS_PROXIMITY) == 1? true : false;
            ink.t = pkt.pkTime;
            ink.seq = pkt.pkSerialNumber;

            return ink;
        }

        private void CloseCurrentContext()
        {
            if(_data != null)
            {
                _data.ClearWTPacketEventHandler();
                _data = null;
            }

            if (_context != null)
            {
                _context.Close();
                _context = null;

            }
            
        }

    }
}
