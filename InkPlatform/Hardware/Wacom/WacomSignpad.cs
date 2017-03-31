using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InkPlatform.UserInterface;
using InkPlatform.Ink;
using wgssSTU;

namespace InkPlatform.Hardware.Wacom
{

    /// <summary>
    /// Represents a Wacom Signpad
    /// </summary>
    /// <seealso cref="InkPlatform.Hardware.Signpad" />
    public class WacomSignpad : Signpad
    {
        /// <summary>
        /// The default baud rate for wacom sign pads
        /// </summary>
        public static int DEFAULT_BAUD_RATE = 128000;
        /// <summary>
        /// The vendor id for wacom devices
        /// </summary>
        public static new ushort VID = 0x056a;
        /// <summary>
        /// The vendor name - Wacom
        /// </summary>
        public static new string VENDOR_NAME = strings.WACOM;
        /// <summary>
        /// The vendor id for wacom devices (in string)
        /// </summary>
        public static string VID_STRING = "056A";

        /// <summary>
        /// The number of times to retry connecting if an attempt to connect is unsuccessful
        /// </summary>
        int _retry_times = 5;
        /// <summary>
        /// Deprecated. The wait time in milliseconds between each retry. Changed to random
        /// </summary>
        int _retry_wait = 2000;

        IUsbDevice _usbDevice;
        Tablet _tablet;
        encodingMode _encodingMode;
        Bitmap _lastBitmap;

        /// <summary>
        /// Initializes a new instance of the <see cref="WacomSignpad"/> class.
        /// </summary>
        /// <param name="usbDevice">The usb device (from Wacom STU SDK)</param>
        public WacomSignpad(IUsbDevice usbDevice)
        {
            Log("WacomSignpad created");
            _vid = 0x056a;
            _vendorName = strings.WACOM;
            _sensorResolution = 2540;
            _usbDevice = usbDevice;
            initTablet();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WacomSignpad"/> class.
        /// </summary>
        public WacomSignpad()
        {
            Log("WacomSignpad created");
            _vid = 0x056a;
            _vendorName = strings.WACOM;
            _sensorResolution = 2540;
            initTablet();
        }

        /// <summary>
        /// Initializes the tablet class (in STU SDK) and the pen data events
        /// </summary>
        private void initTablet()
        {
            _tablet = new Tablet();
            _tablet.onPenData += _tablet_onPenData;
            _tablet.onPenDataTimeCountSequence += _tablet_onPenDataTimeCountSequence;
            _tablet.onGetReportException += _tablet_onGetReportException;
        }

        /// <summary>
        /// Event handler when exception occurs (from Wacom STU SDK)
        /// </summary>
        /// <param name="pException">The p exception.</param>
        private void _tablet_onGetReportException(ITabletEventsException pException)
        {
            try
            {
                pException.getException();
            }
            catch (Exception)
            {
                if(OnDeviceException != null)
                {
                    OnDeviceException();
                }
            }
        }

        /// <summary>
        /// Currently not supported. Connects serial sign pads
        /// </summary>
        /// <returns></returns>
        private int ConnectSerial()
        {
            Log("ConnectSerial");
            return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;
        }

        /// <summary>
        /// Connects usb sign pad.
        /// </summary>
        /// <returns></returns>
        private int ConnectUsb()
        {
            return ConnectUsb(true, _retry_times, _retry_wait);
        }

        /// <summary>
        /// Connects usb signpad.
        /// </summary>
        /// <param name="exclusive">if set to <c>true</c> [exclusive].</param>
        /// <param name="retryTimes">The retry times.</param>
        /// <param name="retryWait">Deprecated. The retry wait is now changed to random</param>
        /// <returns></returns>
        private int ConnectUsb(bool exclusive, int retryTimes, int retryWait)
        {
            Log("ConnectUsb " + (exclusive ? "exclusive " : " ") + "retry " + retryTimes + " wait random max " + retryWait + "ms");
            if(_usbDevice == null)
            {
                resetUsbDevice();
            }
            
            try
            {
                Log("Tablet usb connect");
                if (_tablet == null)
                {
                    initTablet();
                }
                IErrorCode ec = _tablet.usbConnect(_usbDevice, exclusive);
                Log("Return " + ec.value.ToString() + " - " + GetConnectUsbErrorMessage(ec.value));

                //Retry if the error code is wrong state (Meaning device busy)
                if (ec.value != (int)ErrorCode.ErrorCode_None)
                {
                    Log("Connection attempt failed, pending retry");
                    Random random = new Random();
                    int retryCount = 0;
                    while (retryCount < retryTimes)
                    {
                        int randomWait = random.Next(100, retryWait);
                        Log("Wait " + randomWait + "ms");
                        Thread.Sleep(randomWait);
                        Log("Tablet usb connect");
                        ec = _tablet.usbConnect(_usbDevice, true);
                        Log("Return " + ec.value.ToString() + " - " + GetConnectUsbErrorMessage(ec.value));

                        if (ec.value == (int)ErrorCode.ErrorCode_None)
                        {
                            break;
                        }

                        retryCount++;
                    }

                    if(ec.value != (int)ErrorCode.ErrorCode_None)
                    {
                        Log(PenDeviceErrorMessage(PEN_DEVICE_ERROR.DEVICE_BUSY), 1);
                        return (int)PEN_DEVICE_ERROR.DEVICE_BUSY;
                    }
                }
                
            }
            catch (Exception)
            {
                Log(PenDeviceErrorMessage(PEN_DEVICE_ERROR.CANNOT_CONNECT), 1);
                return (int)PEN_DEVICE_ERROR.CANNOT_CONNECT;
            }

            return InitWacomSignpad(CONNECTION_MODE.USB);
        }

        /// <summary>
        /// Initializes the wacom signpad, select the encoding mode, get serial no, set color support,
        /// get tablet dimension, and set the connection id
        /// </summary>
        /// <param name="connectionMode">The connection mode.</param>
        /// <returns></returns>
        private int InitWacomSignpad(CONNECTION_MODE connectionMode)
        {
            Log("Init Wacom Signpad");

            ConnectionMode = connectionMode;

            try
            {
                _encodingMode = TestEncodingMode();
            }
            catch (Exception)
            {
                if(ProductModel == "STU-530" || ProductModel == "STU-540")
                {
                    _encodingMode = encodingMode.EncodingMode_24bit_Bulk;
                }else if(ProductModel == "STU-520")
                {
                    _encodingMode = encodingMode.EncodingMode_16bit_Bulk;
                }else if(ProductModel == "STU-300" || ProductModel == "STU-430V")
                {
                    _encodingMode = encodingMode.EncodingMode_1bit;
                }
                else
                {
                    _encodingMode = encodingMode.EncodingMode_16bit;
                }
            }

            try
            {
                SupportColor = TestSupportColor();
            }
            catch (Exception)
            {
                if(ProductModel == "STU-530" || ProductModel == "STU-520" || ProductModel == "STU-540")
                {
                    SupportColor = true;
                }
                else
                {
                    SupportColor = false;
                }
            }

            try
            {
                _serialNo = GetSerialNo();
            }
            catch (Exception)
            {
                _serialNo = "";
            }

            try
            {
                _tabletDimension = GetTabletDimension();

            }
            catch (Exception ex)
            {
                Log("Fail: " + ex.Message, 1);
                return (int)PEN_DEVICE_ERROR.INIT_FAIL;
            }

            _connectionId = System.IO.Path.GetRandomFileName();
            
            return (int)PEN_DEVICE_ERROR.NONE;
        }

        /// <summary>
        /// Check if the tablet supports color (STU SDK)
        /// </summary>
        /// <returns>boolean.</returns>
        private bool TestSupportColor()
        {
            if (SupportColor && _tablet.supportsWrite())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Tests the encoding mode of the sign pad (STU SDK)
        /// </summary>
        /// <returns>encoding mode for the stu</returns>
        private encodingMode TestEncodingMode()
        {
            if (_tablet == null)
            {
                initTablet();
            }

            wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();
            wgssSTU.encodingFlag encodingFlag = (wgssSTU.encodingFlag)protocolHelper.simulateEncodingFlag(Pid, 0);
            
            if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_24bit) != 0)
            {
                return _tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_24bit_Bulk : wgssSTU.encodingMode.EncodingMode_24bit;
            }
            else if ((encodingFlag & wgssSTU.encodingFlag.EncodingFlag_16bit) != 0)
            {
                return _tablet.supportsWrite() ? wgssSTU.encodingMode.EncodingMode_16bit_Bulk : wgssSTU.encodingMode.EncodingMode_16bit;
            }
            else
            {
                // assumes 1bit is available
                return wgssSTU.encodingMode.EncodingMode_1bit;
            }
        }

        /// <summary>
        /// Gets the serial no of the connected sign pad (STU SDK)
        /// </summary>
        /// <returns>serial number of the connected signpad</returns>
        private string GetSerialNo()
        {
            Log("Get Serial No");
           if(_tablet != null)
            {
                try
                {
                    return _tablet.getUid2();
                }
                catch (Exception ex)
                {
                    Log("GetUid2 Failed: " + ex.Message, 1);
                }

                try
                {
                    uint result = _tablet.getUid();
                    return result.ToString();
                }
                catch (Exception ex)
                {
                    Log("Get Uid Failed: " + ex.Message, 1);
                }

                return "";
            }
            return "";
        }

        /// <summary>
        /// Gets the error message from the error code in STU SDK
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>The error message</returns>
        private string GetConnectUsbErrorMessage(int errorCode)
        {
            switch ((ErrorCode)errorCode)
            {
                case ErrorCode.ErrorCode_None:
                    return strings.SUCCESS;
                case ErrorCode.ErrorCode_WrongReportId:
                    return strings.WRONG_REPORT_ID;
                case ErrorCode.ErrorCode_WrongState:
                    return strings.WRONG_STATE;
                case ErrorCode.ErrorCode_CRC:
                    return strings.CRC;
                case ErrorCode.ErrorCode_GraphicsWrongEncodingType:
                    return strings.GRAPHICS_WRONG_ENCODING_TYPE;
                case ErrorCode.ErrorCode_GraphicsImageTooLong:
                    return strings.GRAPHICS_IMAGE_TOO_LONG;
                case ErrorCode.ErrorCode_GraphicsZlibError:
                    return strings.GRAPHICS_ZLIB_ERROR;
                case ErrorCode.ErrorCode_GraphicsWrongParameters:
                    return strings.GRAPHICS_WRONG_PARAMETERS;
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets the tablet dimension of the signpad
        /// </summary>
        /// <value>
        /// The tablet dimension.
        /// </value>
        public override Size TabletDimension
        {
            get
            {
                _tabletDimension = GetTabletDimension();
                return _tabletDimension;
            }
            //set
            //{
                //throw new NotSupportedException();
            //}
        }

        /// <summary>
        /// Gets the tablet dimension from the icapability of the tablet in STU SDK
        /// </summary>
        /// <returns></returns>
        private Size GetTabletDimension()
        {
            try
            {
                return new Size((int)_tablet.getCapability().tabletMaxX, (int)_tablet.getCapability().tabletMaxY);
            }
            catch (Exception)
            {
                return new Size(0, 0);
            }
            
        }

        /// <summary>
        /// Resets the usb device.
        /// </summary>
        private void resetUsbDevice()
        {
            Log("Reset usb device");
            UsbDevices usbDevices = new UsbDevices();
            if(usbDevices != null && usbDevices.Count > 0)
            {
                foreach(IUsbDevice usbDevice in usbDevices)
                {
                    if(usbDevice.idVendor == _vid && usbDevice.idProduct == _pid)
                    {
                        Log("Usb device found for " + VendorName + " " + ProductModel);
                        _usbDevice = usbDevice;
                        return;
                    }
                }
            }
            Log("Fail", 1);
        }

        /// <summary>
        /// Converts the pen data from STU SDK to InkData format
        /// </summary>
        /// <param name="penData">The pen data.</param>
        /// <returns>InkData</returns>
        private InkData convertInkData(IPenData penData)
        {
            InkData ink = new InkData();
            ink.x = penData.x;
            ink.y = penData.y;
            ink.p = penData.pressure;
            ink.contact = penData.sw == 1 ? true : false;
            ink.proximity = penData.rdy;

            return ink;
        }

        /// <summary>
        /// Converts the pen data from STU SDK to InkData format
        /// </summary>
        /// <param name="penData">The pen data.</param>
        /// <returns>InkData</returns>
        private InkData convertInkData(IPenDataTimeCountSequence penData)
        {
            InkData ink = new InkData();
            ink.x = penData.x;
            ink.y = penData.y;
            ink.p = penData.pressure;
            ink.contact = penData.sw == 1 ? true : false;
            ink.proximity = penData.rdy;
            ink.t = penData.timeCount;
            ink.seq = penData.sequence;

            return ink;
        }

        /// <summary>
        /// Connects this signpad.
        /// </summary>
        /// <returns>Error code. 0 = no error</returns>
        public override int Connect()
        {
            Log("Connect");
            int connectError = (int)PEN_DEVICE_ERROR.NONE;

            if(ConnectionMode == CONNECTION_MODE.USB)
            {
                connectError = ConnectUsb();
            }
            else if(ConnectionMode == CONNECTION_MODE.SERIAL)
            {
                connectError = ConnectSerial();
            }
            
            return connectError;
            
        }

        /// <summary>
        /// Checks whether the signpad is connected
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsConnected()
        {
            Log("IsConnected");
            if (_tablet == null)
            {
                Log("Tablet is null");
                return false;
            }
            if (_connectionId == null || _connectionId == "")
            {
                Log("Connection Id is empty");
                return false;
            }
            try
            {
                int status = _tablet.getStatus().statusCode;
                Log(GetConnectUsbErrorMessage(status));
                if (status != (int)statusCode.StatusCode_Ready)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                Log("Get tablet status failed", 1);
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Converts the bitmap to data for the signpad to show
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>byte array</returns>
        private Byte[] convertBitmapToData(Bitmap bitmap)
        {
            Log("Convert bitmap to data");
            // Now the bitmap has been created, it needs to be converted to device-native
            // format.

            // Unfortunately it is not possible for the native COM component to
            // understand .NET bitmaps. We have therefore convert the .NET bitmap
            // into a memory blob that will be understood by COM.
            try
            {
                wgssSTU.ProtocolHelper protocolHelper = new wgssSTU.ProtocolHelper();

                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Byte[] _bitmapData = (byte[])protocolHelper.resizeAndFlatten(stream.ToArray(), 0, 0, (uint)bitmap.Width, (uint)bitmap.Height, (ushort)ScreenDimension.Width, (ushort)ScreenDimension.Height, (byte)_encodingMode, wgssSTU.Scale.Scale_Fit, 0, 0);

                protocolHelper = null;
                stream.Dispose();

                return _bitmapData;
            }
            catch (Exception)
            {
                Log("Fail", 1);
                return null;
            }
            
        }

        /// <summary>
        /// Displays the bitmap onto the signpad screen
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns>error code</returns>
        public override int DisplayBitmap(Bitmap bitmap)
        {
            Log("Display bitmap");
            
            if (!HasScreen) return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;
            if (bitmap == null) return (int)PEN_DEVICE_ERROR.NULL_PARAM;
            if (_connectionId == null || _connectionId == "") return (int)PEN_DEVICE_ERROR.NOT_CONNECTED;
            if (!IsConnected()) { return (int)PEN_DEVICE_ERROR.DEVICE_BUSY; }

            //Convert bitmap to data bytes
            Byte[] bmpData = convertBitmapToData(bitmap);

            //Send to signpad
            try
            {
                Log("Tablet write image");
                _tablet.writeImage((byte)_encodingMode, bmpData);
            }
            catch (Exception)
            {
                Log(PenDeviceErrorMessage(PEN_DEVICE_ERROR.DISPLAY_FAIL), 1);
                return (int)PEN_DEVICE_ERROR.DISPLAY_FAIL;
            }
            
            _lastBitmap = bitmap;

            return (int)PEN_DEVICE_ERROR.NONE;
        }

        /// <summary>
        /// Event handler of the signpad pendata event which returns pen data with time and sequence
        /// </summary>
        /// <param name="penDataTimeCountSequence">The pen data time count sequence.</param>
        private void _tablet_onPenDataTimeCountSequence(IPenDataTimeCountSequence penDataTimeCountSequence)
        {
            Log("PDTCS: " + penDataTimeCountSequence.x.ToString() + "," + penDataTimeCountSequence.y.ToString(), 2);
            if(OnPenData != null)
            {
                OnPenData(convertInkData(penDataTimeCountSequence));
            }
        }

        /// <summary>
        /// Event handler of the signpad pen data event which returns pen data without the time and sequence
        /// </summary>
        /// <param name="penData">The pen data.</param>
        private void _tablet_onPenData(IPenData penData)
        {
            Log("PD: " + penData.x.ToString() + "," + penData.y.ToString(), 2);
            if(OnPenData != null)
            {
                OnPenData(convertInkData(penData));
            }
        }

        /// <summary>
        /// Refreshes the screen.
        /// </summary>
        /// <returns>error code if any</returns>
        public override int RefreshScreen()
        {
            Log("Refresh Screen");
            if(_lastBitmap != null)
            {
                return DisplayBitmap(_lastBitmap);
            }
            else
            {
                return ClearScreen();
            }
            
        }

        /// <summary>
        /// Clears the screen.
        /// </summary>
        /// <returns>error code if any</returns>
        public override int ClearScreen()
        {
            Log("Clear Screen");
            try
            {
                _tablet.setClearScreen();
                _lastBitmap = null;

                return (int)PEN_DEVICE_ERROR.NONE;
            }
            catch (Exception)
            {
                Log(PenDeviceErrorMessage(PEN_DEVICE_ERROR.NOT_CONNECTED), 1);
                return (int)PEN_DEVICE_ERROR.CANNOT_CONNECT;
            }
            
        }

        /// <summary>
        /// Resets the signpad
        /// </summary>
        public override void Reset()
        {
            Log("Reset");
            try
            {
                _tablet.reset();
            }
            catch (Exception) {
                Log("Reset fail", 1);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether inking on the <see cref="WacomSignpad"/> is enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if inking; otherwise, <c>false</c>.
        /// </value>
        public override bool Inking
        {
            get
            {
                return base.Inking;
            }

            set
            {
                base.Inking = value;
                try
                {
                    if (value)
                    {
                        Log("Tablet set inking mode = yes");
                        _tablet.setInkingMode(0x01);
                    }
                    else
                    {
                        Log("Tablet set inking mode = no");
                        _tablet.setInkingMode(0x00);
                    }
                }
                catch (Exception) {
                    Log("Tablet set inking mode failed", 1);
                }
            }
        }

        /// <summary>
        /// Disconnects the signpad.
        /// </summary>
        /// <returns></returns>
        public override int Disconnect()
        {
            Log("Disconnect");
            //ClearScreen();
            try
            {
                _tablet.disconnect();
                _tablet = null;
                Log("Tablet disconnected");
                _connectionId = "";
                return (int)PEN_DEVICE_ERROR.NONE;
            }
            catch (Exception ex)
            {
                Log("Disconnect fail", 1);
                Log(ex.Message);
                return (int)PEN_DEVICE_ERROR.NOT_CONNECTED;
            }
            
            
        }

    }
}
