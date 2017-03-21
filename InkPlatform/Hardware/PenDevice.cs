using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using InkPlatform.UserInterface;
using InkPlatform.Ink;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Windows.Forms;

namespace InkPlatform.Hardware
{
    public enum DEVICE_TYPE { SIGNPAD, PEN_TABLET, PEN_DISPLAY, PEN_COMPUTER };
    public enum CONNECTION_MODE { USB, SERIAL, WIRELESS, BLUETOOTH };
    
    public enum PEN_DEVICE_ERROR { NONE, DEVICE_BUSY, CANNOT_CONNECT, INIT_FAIL, NULL_PARAM, LAYOUT_NOT_FOUND, NOT_SUPPORTED, ALREADY_CONNECTED, NOT_CONNECTED, DISPLAY_FAIL, USER_CANCELLED, LAYOUT_FAIL, UNSPECIFIED};

    public abstract class PenDevice
    {
        public delegate void InkDataReceived(InkData ink);
        public delegate void SendLog(string msg, int alertType);
        public delegate bool DeviceException();

        public InkDataReceived OnPenData;
        public DeviceException OnDeviceException;

        public static ushort VID;
        public static ushort PID;
        public static string VENDOR_NAME;
        public static string PRODUCT_MODEL;

        protected string _connectionId = "";

        protected ushort _vid;
        protected ushort _pid;
        protected string _vendorName;
        protected string _productModel;
        
        protected int _maxPressureLevels;
        protected int _maxReportRate;
        protected bool _hasScreen;
        protected bool _supportUsb;
        protected bool _supportSerial;
        protected Size _screenDimension;
        protected DEVICE_TYPE _deviceType;
        protected int _sensorResolution;
        protected bool _inking;
        
        public SendLog LogFunction;

        protected string _serialNo;
        protected CONNECTION_MODE _connectionMode;
        protected Size _tabletDimension;
        protected bool _supportColor;

        public ushort Vid
        {
            get { return _vid; }
            set { _vid = value; }
        }
        public ushort Pid
        {
            get { return _pid; }
            set { _pid = value; }
        }
        public string VendorName
        {
            get { return _vendorName; }
            set { _vendorName = value; }
        }
        public string ProductModel
        {
            get { return _productModel; }
            set { _productModel = value; }
        }

        public virtual int MaxPressureLevels
        {
            get { return _maxPressureLevels; }
            set { _maxPressureLevels = value; }
        }
        public virtual int MaxReportRate
        {
            get { return _maxReportRate; }
            set { _maxReportRate = value; }
        }
        public virtual bool HasScreen
        {
            get { return _hasScreen; }
            set { _hasScreen = value; }
        }
        public virtual bool SupportUsb
        {
            get { return _supportUsb; }
            set { _supportUsb = value; }
        }
        public virtual bool SupportSerial
        {
            get { return _supportSerial; }
            set { _supportSerial = value; }
        }
        public virtual Size ScreenDimension
        {
            get { return _screenDimension; }
            set { _screenDimension = value; }
        }
        public DEVICE_TYPE DeviceType
        {
            get { return _deviceType; }
            set { _deviceType = value; }
        }
        public virtual int SensorResolution
        {
            get { return _sensorResolution; }
            set { _sensorResolution = value; }
        }
        public virtual bool Inking
        {
            get { return _inking; }
            set { _inking = value; }
        }

        public override string ToString()
        {
            string result = "{";

            result = result + "\"Vid\":" + _vid.ToString() + ",";
            result = result + "\"Pid\":" + _pid.ToString() + ",";
            result = result + "\"VendorName\":\"" + _vendorName + "\",";
            result = result + "\"ProductModel\":\"" + _productModel + "\",";

            result = result + "\"MaxPressureLevels\":" + _maxPressureLevels.ToString() + ",";
            result = result + "\"MaxReportRate\":" + _maxReportRate.ToString() + ",";
            result = result + "\"HasScreen\":" + (_hasScreen?"true":"false") + ",";
            result = result + "\"SupportUsb\":" + (_supportUsb ? "true" : "false") + ",";
            result = result + "\"SupportSerial\":" + (_supportSerial ? "true" : "false") + ",";
            result = result + "\"SupportColor\":" + (_supportColor ? "true" : "false") + ",";
            if(_screenDimension != null)
            {
                result = result + "\"ScreenWidth\":" + _screenDimension.Width + ",";
                result = result + "\"ScreenHeight\":" + _screenDimension.Height + ",";
            }
            switch (DeviceType)
            {
                case DEVICE_TYPE.SIGNPAD:
                    result = result + "\"DeviceType\":\"Signpad\",";
                    break;
                case DEVICE_TYPE.PEN_TABLET:
                    result = result + "\"DeviceType\":\"PenTablet\",";
                    break;
                case DEVICE_TYPE.PEN_DISPLAY:
                    result = result + "\"DeviceType\":\"PenDisplay\",";
                    break;
                case DEVICE_TYPE.PEN_COMPUTER:
                    result = result + "\"DeviceType\":\"PenComputer\",";
                    break;
            }
            if(_sensorResolution > 0)
            {
                result = result + "\"SensorResolution\":" + _sensorResolution + ",";
            }
            result = result + "\"SerialNo\":\"" + _serialNo + "\",";
            switch (ConnectionMode)
            {
                case CONNECTION_MODE.BLUETOOTH:
                    result = result + "\"ConnectionMode\":\"Bluetooth\",";
                    break;
                case CONNECTION_MODE.SERIAL:
                    result = result + "\"ConnectionMode\":\"Serial\",";
                    break;
                case CONNECTION_MODE.USB:
                    result = result + "\"ConnectionMode\":\"USB\",";
                    break;
                case CONNECTION_MODE.WIRELESS:
                    result = result + "\"ConnectionMode\":\"Wireless\",";
                    break;
            }
            if(_tabletDimension != null)
            {
                result = result + "\"TabletWidth\":" + _tabletDimension.Width + ",";
                result = result + "\"TabletHeight\":" + _tabletDimension.Height;
            }
            
            result = result + "}";

            return result;
        }

        public virtual string SerialNo
        {
            get
            {
                return _serialNo;
            }

            set
            {
                _serialNo = value;
            }
        }
        public virtual CONNECTION_MODE ConnectionMode
        {
            get { return _connectionMode; }
            set { _connectionMode = value; }
        }
        public virtual Size TabletDimension
        {
            get { return _tabletDimension; }
            set { _tabletDimension = value; }
        }
        public virtual bool SupportColor
        {
            get { return _supportColor; }
            set { _supportColor = value; }
        }

        public string ConnectionId
        {
            get { return _connectionId; }
            set { _connectionId = value; }
        }

        //public abstract int DisplayLayout(Layout layout);
        public abstract int DisplayBitmap(Bitmap bmp);
        public abstract int RefreshScreen();
        public abstract int ClearScreen();
        public abstract int Connect();
        public abstract int Disconnect();
        
        public virtual bool IsConnected()
        {
            return _connectionId == "" ? false : true;
        }
        public abstract void Reset();

        public virtual void Log(string msg, int alertType)
        {
            //alertType
            //0 = information
            //1 = error
            //2 = bulky, for pen data
            if(LogFunction != null)
            {
                LogFunction(msg, alertType);
            }
        }
        public virtual void Log(string msg)
        {
            Log(msg, 0);
        }

        public virtual string PenDeviceErrorMessage(PEN_DEVICE_ERROR errorCode)
        {
            switch (errorCode)
            {
                case PEN_DEVICE_ERROR.NONE:
                    return strings.SUCCESS;
                case PEN_DEVICE_ERROR.DEVICE_BUSY:
                    return strings.DEVICE_BUSY;
                case PEN_DEVICE_ERROR.CANNOT_CONNECT:
                    return strings.CANNOT_CONNECT;
                case PEN_DEVICE_ERROR.INIT_FAIL:
                    return strings.INIT_FAIL;
                case PEN_DEVICE_ERROR.NULL_PARAM:
                    return strings.NULL_PARAM;
                case PEN_DEVICE_ERROR.LAYOUT_NOT_FOUND:
                    return strings.LAYOUT_NOT_FOUND;
                case PEN_DEVICE_ERROR.NOT_SUPPORTED:
                    return strings.NOT_SUPPORTED;
                case PEN_DEVICE_ERROR.ALREADY_CONNECTED:
                    return strings.ALREADY_CONNECTED;
                case PEN_DEVICE_ERROR.NOT_CONNECTED:
                    return strings.NOT_CONNECTED;
                case PEN_DEVICE_ERROR.DISPLAY_FAIL:
                    return strings.DISPLAY_FAIL;
                case PEN_DEVICE_ERROR.USER_CANCELLED:
                    return strings.USER_CANCELLED;
                case PEN_DEVICE_ERROR.LAYOUT_FAIL:
                    return strings.LAYOUT_FAIL;
                default:
                    return "";
            }
        }

        public virtual bool IsSameDevice(PenDevice other)
        {
            try
            {
                if (Vid == other.Vid && Pid == other.Pid)
                {
                    if (SerialNo != null && SerialNo != "" && SerialNo != "0" &&
                        other.SerialNo != null && other.SerialNo != "" && other.SerialNo != "0")
                    {
                        if (SerialNo == other.SerialNo)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public virtual SerializablePenDevice Serialize()
        {
            SerializablePenDevice device = new SerializablePenDevice();
            device.Vid = _vid;
            device.Pid = _pid;
            device.VendorName = _vendorName;
            device.ProductModel = _productModel;
            device.MaxPressureLevels = _maxPressureLevels;
            device.MaxReportRate = _maxReportRate;
            device.HasScreen = _hasScreen;
            device.SupportColor = _supportColor;
            device.SupportSerial = _supportSerial;
            device.SupportUsb = _supportUsb;
            if(_screenDimension != null)
            {
                device.ScreenWidth = _screenDimension.Width;
                device.ScreenHeight = _screenDimension.Height;
            }
            
            switch (_deviceType)
            {
                case DEVICE_TYPE.PEN_COMPUTER:
                    device.DeviceType = "PenComputer";
                    break;
                case DEVICE_TYPE.PEN_DISPLAY:
                    device.DeviceType = "PenDisplay";
                    break;
                case DEVICE_TYPE.PEN_TABLET:
                    device.DeviceType = "PenTablet";
                    break;
                case DEVICE_TYPE.SIGNPAD:
                    device.DeviceType = "Signpad";
                    break;
            }
            device.SensorResolution = _sensorResolution;
            device.SerialNo = _serialNo;
            switch (ConnectionMode)
            {
                case CONNECTION_MODE.BLUETOOTH:
                    device.ConnectionMode = "Bluetooth";
                    break;
                case CONNECTION_MODE.SERIAL:
                    device.ConnectionMode = "Serial";
                    break;
                case CONNECTION_MODE.USB:
                    device.ConnectionMode = "Usb";
                    break;
                case CONNECTION_MODE.WIRELESS:
                    device.ConnectionMode = "Wireless";
                    break;
            }
            if(_tabletDimension != null)
            {
                device.TabletWidth = _tabletDimension.Width;
                device.TabletHeight = _tabletDimension.Height;
            }
            

            return device;
        }
        
    }

    [DataContract]
    public class SerializablePenDevice
    {
        [DataMember]
        public ushort Vid;

        [DataMember]
        public ushort Pid;

        [DataMember]
        public string VendorName;

        [DataMember]
        public string ProductModel;

        [DataMember]
        public int MaxPressureLevels;

        [DataMember]
        public int MaxReportRate;

        [DataMember]
        public bool HasScreen;

        [DataMember]
        public bool SupportUsb;

        [DataMember]
        public bool SupportSerial;

        [DataMember]
        public bool SupportColor;

        [DataMember]
        public int ScreenWidth;

        [DataMember]
        public int ScreenHeight;

        [DataMember]
        public string DeviceType;

        [DataMember]
        public int SensorResolution;

        [DataMember]
        public string SerialNo;

        [DataMember]
        public string ConnectionMode;

        [DataMember]
        public int TabletWidth;

        [DataMember]
        public int TabletHeight;
        
        public string Serialize()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, this);
                ms.Position = 0;
                try
                {
                    StreamReader reader = new StreamReader(ms);
                    return reader.ReadToEnd();
                }
                catch(Exception)
                {
                    return null;
                }
            }
        }

        public SerializablePenDevice Deserialize(string json)
        {
            DataContractJsonSerializer deserializer = new DataContractJsonSerializer(GetType());
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(ms);
                writer.Write(json);
                writer.Flush();
                ms.Position = 0;

                try
                {
                    SerializablePenDevice idevice = (SerializablePenDevice)deserializer.ReadObject(ms);
                    return idevice;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public static string ErrorMessage(PEN_DEVICE_ERROR errorCode)
        {
            switch (errorCode)
            {
                case PEN_DEVICE_ERROR.NONE:
                    return strings.SUCCESS;
                case PEN_DEVICE_ERROR.DEVICE_BUSY:
                    return strings.DEVICE_BUSY;
                case PEN_DEVICE_ERROR.CANNOT_CONNECT:
                    return strings.CANNOT_CONNECT;
                case PEN_DEVICE_ERROR.INIT_FAIL:
                    return strings.INIT_FAIL;
                case PEN_DEVICE_ERROR.NULL_PARAM:
                    return strings.NULL_PARAM;
                case PEN_DEVICE_ERROR.LAYOUT_NOT_FOUND:
                    return strings.LAYOUT_NOT_FOUND;
                case PEN_DEVICE_ERROR.NOT_SUPPORTED:
                    return strings.NOT_SUPPORTED;
                case PEN_DEVICE_ERROR.ALREADY_CONNECTED:
                    return strings.ALREADY_CONNECTED;
                case PEN_DEVICE_ERROR.NOT_CONNECTED:
                    return strings.NOT_CONNECTED;
                case PEN_DEVICE_ERROR.DISPLAY_FAIL:
                    return strings.DISPLAY_FAIL;
                case PEN_DEVICE_ERROR.USER_CANCELLED:
                    return strings.USER_CANCELLED;
                case PEN_DEVICE_ERROR.LAYOUT_FAIL:
                    return strings.LAYOUT_FAIL;
                default:
                    return "";
            }
        }
    }
}
