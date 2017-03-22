using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InkPlatform.UserInterface;
using InkPlatform.Hardware;
using InkPlatform.Ink;

namespace InkPlatform.UserControls
{
    public delegate bool ButtonEvent(object sender, UserInterface.LayoutEventArgs e);
    public delegate void SendLog(string msg, int alertType);

    public partial class SignpadControl : UserControl
    {
        public enum RESIZE_CONDITION
        {
            NONE, ACTUAL_SIZE, ASPECT_RATIO_WIDTH, ASPECT_RATIO_HEIGHT
        }

        public SendLog LogFunction;
        
        Layout _currentLayout;
        List<Layout> _allLayouts = new List<UserInterface.Layout>();
        Bitmap _currentBitmap;
        PenDevice _currentPenDevice;

        protected PenDevice CurrentPenDevice
        {
            get { return _currentPenDevice; }
            set
            {
                if(_currentPenDevice != null)
                {
                    _currentPenDevice.OnPenData = null;
                    this.MouseMove -= this.SignpadControl_MouseMove;
                    this.MouseClick -= this.SignpadControl_MouseClick;
                }
                
                _currentPenDevice = value;
                
                if(_currentPenDevice != null)
                {
                    this.MouseMove += this.SignpadControl_MouseMove;
                    this.MouseClick += this.SignpadControl_MouseClick;
                }
            }
        }
        
        Graphics gfx;
        bool _inking = true;
        string _connectionId = "";
        Pen DefaultPen;

        InkData CurrPenStatus = null;
        InkData PrevPenStatus = null;

        List<InkData> _penData = new List<InkData>();
        public List<InkData> PenData
        {
            get { return _penData; }
        }

        ContextPenData _contextPenData = null;
        public ContextPenData ContextPenData
        {
            get { return _contextPenData; }
        }

        private bool _inkingOnButton = false;
        public bool InkingOnButton
        {
            get { return _inkingOnButton; }
            set { _inkingOnButton = value; }
        }

        public float _defaultInkWidth = 0.7F;
        public float DefaultInkWidth
        {
            get { return _defaultInkWidth; }
            set { _defaultInkWidth = value; }
        }

        public Color _defaultPenColor = Color.DarkBlue;
        public Color DefaultPenColor
        {
            get { return _defaultPenColor; }
            set { _defaultPenColor = value; }
        }
        
        RESIZE_CONDITION _resizeCondition = RESIZE_CONDITION.ACTUAL_SIZE;
        public RESIZE_CONDITION ResizeCondition
        {
            get { return _resizeCondition; }
            set { _resizeCondition = value; }
        }

        public Layout CurrentLayout
        {
            get { return _currentLayout; }
        }

        public List<Layout> AllLayouts
        {
            get { return _allLayouts; }
        }

        public Bitmap CurrentBitmap
        {
            get { return _currentBitmap; }
        }
        
        public ButtonEvent DonePressed;
        public ButtonEvent CancelPressed;
        public ButtonEvent ClearPressed;
        
        private bool _logging = true;
        public bool Logging
        {
            get { return _logging; }
            set
            {
                _logging = value;
                if(_currentPenDevice != null)
                {
                    if (_logging)
                    {
                        _currentPenDevice.LogFunction = ReceivePenDeviceLog;
                    }
                    else
                    {
                        _currentPenDevice.LogFunction = null;
                    }
                }
            }
        }
        
        public SignpadControl()
        {
            InitializeComponent();
        }
        
        protected bool OnDeviceException()
        {
            //return true to get device to disconnect
            Log("Device exception");
            return true;
        }
        
        public int CaptureSignature(string who, string why, PenDevice penDevice)
        {
            SignatureLayout layout = new SignatureLayout("Sign", who, why);
            int result = DisplayLayout(layout, penDevice);

            return result; 
        }
        
        public void ReceivePenDeviceLog(string msg, int alertType)
        {
            //we add 10 to the alert type for those log passed over by the pen device
            Log("[PenDevice] " + msg, alertType + 10);
        }

        public void Log(string msg, int alertType)
        {
            if(LogFunction != null && _logging)
            {
                if(_currentPenDevice != null)
                {
                    _currentPenDevice.LogFunction = ReceivePenDeviceLog;
                }
                LogFunction(msg, alertType);
            }
        }

        public void Log(string msg)
        {
            Log(msg, 0);
        }

        public bool IsConnected(PenDevice penDevice)
        {
            if (penDevice == null) return false;
            if (penDevice.ConnectionId == null || penDevice.ConnectionId == "") return false;
            if (_connectionId == null || _connectionId == "") return false;
            if (penDevice.ConnectionId != _connectionId) return false;
            return true;
        }

        public int Connect(PenDevice penDevice)
        {
            Log("Connect");
            penDevice.LogFunction = ReceivePenDeviceLog;

            if (IsConnected(penDevice))
            {
                Log("Already connected");
                return (int)PEN_DEVICE_ERROR.NONE;
            }
            else
            {
                int result = penDevice.Connect();
                if(result == (int)PEN_DEVICE_ERROR.NONE)
                {
                    _connectionId = penDevice.ConnectionId;
                    Log("Set current pen device to " + penDevice.ProductModel);
                    CurrentPenDevice = penDevice;
                    
                    if(CurrentPenDevice.DeviceType == DEVICE_TYPE.SIGNPAD)
                    {
                        _resizeCondition = RESIZE_CONDITION.ACTUAL_SIZE;
                    }
                    else
                    {
                        _resizeCondition = RESIZE_CONDITION.ASPECT_RATIO_WIDTH;
                    }
                }
                return result;
            }
        }
        
        public void SetInking(bool inking)
        {
            Log("Set inking = " + (inking ? "yes" : "no"));
            _inking = inking;

            if(CurrentPenDevice != null)
            {
                CurrentPenDevice.Inking = inking;
            }
        }

        public void Reset()
        {
            Log("Reset");
            if(CurrentPenDevice != null)
            {
                CurrentPenDevice.Reset();
            }

            Log("Clear pen data");
            _penData.Clear();
            Log("Set background = white");
            BackgroundImage = null;
            BackColor = Color.White;
        }
        
        protected Bitmap ResizeBitmap(Bitmap original, Size newSize)
        {
            Bitmap result = new Bitmap(newSize.Width, newSize.Height);
            Graphics g = Graphics.FromImage(result);
            
            g.DrawImage(original, new Rectangle(0, 0, newSize.Width, newSize.Height), 
                0, 0, original.Width, original.Height, 
                GraphicsUnit.Pixel);

            return result;
        }

        public int DisplayBitmap(Bitmap bitmap, PenDevice penDevice)
        {
            Log("Display bitmap");
            //Allow signpad control to display regardless if the pen device has screen or not
            //If the pen device does not has screen, still display on the computer screen, but not on the pen device
            /* 
            if (!penDevice.HasScreen)
            {
                Log("Pen device does not support display bitmap");
                return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;
            }
            */

            //make sure pendevice is connected
            if (Connect(penDevice) != (int)PEN_DEVICE_ERROR.NONE)
            {
                return (int)PEN_DEVICE_ERROR.CANNOT_CONNECT;
            }

            Log("Clear pen device onPenData event handler");
            penDevice.OnPenData = null;
            SetInking(false);

            //Initialise the pendata for this layout
            Log("Reset pen data");
            _penData = new List<InkData>();
            _contextPenData = null;


            int result = (int)PEN_DEVICE_ERROR.NONE;
            //Only call pen device to display bitmap if it is a sign pad with screen
            if (penDevice.DeviceType == DEVICE_TYPE.SIGNPAD && penDevice.HasScreen == true)
            {
                result = penDevice.DisplayBitmap(bitmap);
            }

            if (result != (int)PEN_DEVICE_ERROR.NONE)
            {
                return result;
            }

            this.SuspendLayout();

            //Create the bitmap from the layout
            Log("Set background image to bitmap");
            
            BackgroundImage = bitmap;
            BackgroundImageLayout = ImageLayout.Stretch;
            _currentBitmap = bitmap;

            //this.Size = bitmap.Size;

            //Resize control according to aspect ratio of pen device screen
            //Deprecated. Control will resize according to size of bitmap
            /*
            if ((penDevice.DeviceType == DEVICE_TYPE.SIGNPAD && penDevice.HasScreen == true))
            {
                resizeControl(penDevice);
            }
            else if(penDevice.DeviceType == DEVICE_TYPE.PEN_TABLET)
            {
                ResizeCondition = RESIZE_CONDITION.ASPECT_RATIO_WIDTH;
                resizeControl(penDevice);
            }
            */
            
            this.ResumeLayout();

            SetInking(true);
            Log("Set pen device onPenData event handler");
            penDevice.OnPenData = processPenData;

            return (int)PEN_DEVICE_ERROR.NONE;

        }
        
        public int DisplayLayouts(List<Layout> layoutList, PenDevice penDevice, int initialLayout)
        {
            Log("Display layout list");
            //if (!penDevice.HasScreen) return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;

            if (layoutList != null || layoutList.Count > 0)
            {
                _allLayouts = layoutList;
                return DisplayLayout(layoutList[initialLayout], penDevice);
            }
            else
            {
                Log("No layouts available");
                return (int)PEN_DEVICE_ERROR.NULL_PARAM;
            }
        }

        public int DisplayLayouts(List<Layout> layoutList, PenDevice penDevice)
        {
            return DisplayLayouts(layoutList, penDevice, 0);
        }
        
        public int DisplayLayout(Layout layout, PenDevice penDevice)
        {
            Log("Display layout: " + layout.Name);
            //if (!penDevice.HasScreen) return (int)PEN_DEVICE_ERROR.NOT_SUPPORTED;

            _currentLayout = layout;
            CurrentPenDevice = penDevice;

            if(penDevice.DeviceType == DEVICE_TYPE.SIGNPAD || penDevice.DeviceType == DEVICE_TYPE.PEN_TABLET)
            {
                resizeControl(penDevice);
            }
            
            Size bmpDimension = this.Size;
            
            Bitmap bmp = LayoutManager.CreateBitmap(layout, bmpDimension.Width, bmpDimension.Height, penDevice.SupportColor);
            ReassignClickEvents();
            return DisplayBitmap(bmp, penDevice);
            
        }
        
        public void resizeControl(PenDevice penDevice)
        {
            Log("Resize control");

            Size controlDimension = penDevice.TabletDimension;
            if(penDevice.DeviceType == DEVICE_TYPE.SIGNPAD && penDevice.HasScreen)
            {
                controlDimension = penDevice.ScreenDimension;
            }

            if(_resizeCondition == RESIZE_CONDITION.ACTUAL_SIZE)
            {
                Log("Actual size");
                this.Width = controlDimension.Width;
                this.Height = controlDimension.Height;
            }
            else if (_resizeCondition == RESIZE_CONDITION.ASPECT_RATIO_WIDTH)
            {
                Log("Aspect ratio width");
                float ratio = (float)controlDimension.Height / (float)controlDimension.Width;
                this.Height = (int)(ratio * Width);
            }
            else if (_resizeCondition == RESIZE_CONDITION.ASPECT_RATIO_HEIGHT)
            {
                Log("Aspect ration height");
                float ratio = (float)controlDimension.Width / (float)controlDimension.Height;
                this.Width = (int)(ratio * Height);
            }

            resetGraphics();
        }
        
        public void Cancel(object sender, EventArgs e)
        {
            Log("Cancel event handler");
            bool carryOn = true;
            if(CancelPressed != null)
            {
                carryOn = CancelPressed(sender, new UserInterface.LayoutEventArgs(_currentLayout, _penData));
            }

            if (carryOn)
            {
                Log("Carry on");
                ClearScreen();
            }
        }

        public void Clear(object sender, EventArgs e)
        {
            Log("Clear event handler");
            bool carryOn = true;
            if(ClearPressed != null)
            {
                carryOn = ClearPressed(sender, new UserInterface.LayoutEventArgs(_currentLayout, _penData));
            }

            if (!carryOn) return;

            this.Invalidate();
            if(CurrentPenDevice != null)
            {
                RefreshScreen();
            }
        }
        
        public void Done(object sender, EventArgs e)
        {
            Log("Done event handler");

            Log("Save Context Pen Data");
            _contextPenData = new ContextPenData(CurrentPenDevice, _penData, _currentLayout);

            bool carryOn = true;
            if(DonePressed != null)
            {
                carryOn = DonePressed(sender, new UserInterface.LayoutEventArgs(_currentLayout, _penData));
            }

            if (!carryOn) return;
            Log("Carry on");

            string nextScreenName = "";
            if(sender.GetType() == typeof(ElementButton))
            {
                ElementButton btn = (ElementButton)sender;
                if(btn.NextScreenName != null && btn.NextScreenName != "")
                {
                    Log("Button:" + btn.Name + " identified");
                    nextScreenName = btn.NextScreenName;
                }
            }
            else if(sender.GetType() == typeof(ElementImage))
            {
                ElementImage img = (ElementImage)sender;
                if(img.NextScreenName != null && img.NextScreenName != "")
                {
                    Log("Image:" + img.Name + " identified");
                    nextScreenName = img.NextScreenName;
                }
            }
            if(nextScreenName != "")
            {
                LoadNextLayout(nextScreenName);
            }
            else
            {
                ClearScreen();
            }
            
        }

        public void RefreshScreen()
        {
            if(CurrentPenDevice != null)
            {
                if(_currentBitmap != null)
                {
                    DisplayBitmap(_currentBitmap, CurrentPenDevice);
                }
            }
        }

        public void Disconnect()
        {
            Log("Disconnect");
            ClearScreen();
            if(CurrentPenDevice != null)
            {
                _connectionId = "";
                CurrentPenDevice.Disconnect();
                CurrentPenDevice = null;
            }
        }

        public void ClearScreen()
        {
            Log("Clear Screen");
            if(CurrentPenDevice != null)
            {
                CurrentPenDevice.ClearScreen();
            }
            _currentLayout = null;
            BackgroundImage = null;
            BackColor = Color.White;
        }

        public int LoadNextLayout(string layoutName)
        {
            Log("Load next layout: " + layoutName);

            if (_allLayouts == null || _allLayouts.Count == 0)
            {
                Log("Layout list is empty");
                return (int)PEN_DEVICE_ERROR.LAYOUT_NOT_FOUND;
            }

            if (CurrentPenDevice == null)
            {
                Log("Current pen device is null");
                return (int)PEN_DEVICE_ERROR.CANNOT_CONNECT;
            }

            foreach(Layout layout in _allLayouts)
            {
                if(layout.Name == layoutName)
                {
                    return DisplayLayout(layout, CurrentPenDevice);
                }
            }

            return (int)PEN_DEVICE_ERROR.LAYOUT_NOT_FOUND;
        }

        public Pen GetDefaultPen()
        {
            Log("Get default pen");
            
            // Calculate the size and cache the inking pen.
            SizeF s = this.AutoScaleDimensions;

            //Pen _penInk = new Pen(DefaultPenColor, DefaultInkWidth / 25.4F * ((s.Width + s.Height) / 2F));
            Pen _penInk = new Pen(DefaultPenColor, DefaultInkWidth);
            _penInk.StartCap = _penInk.EndCap = System.Drawing.Drawing2D.LineCap.Round;
            _penInk.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

            return _penInk;
        }

        private void drawInk(Pen pen, Point fromPoint, Point toPoint)
        {
            Log("Draw ink", 2);
            // Draw a line from the previous down point to this down point.
            // This is the simplist thing you can do; a more sophisticated program
            // can perform higher quality rendering than this!

            //skip if this is the first dot
            if (PrevPenStatus == null)
            {
                Log("No prev ink status");
                return;
            }

            if(gfx == null)
            {
                resetGraphics();
            }

            try
            {
                if (fromPoint.X < 0) fromPoint.X = 0;
                if (fromPoint.Y < 0) fromPoint.Y = 0;
                if (fromPoint.X > Size.Width) fromPoint.X = Size.Width;
                if (fromPoint.Y > Size.Height) fromPoint.Y = Size.Height;

                if (toPoint.X < 0) toPoint.X = 0;
                if (toPoint.Y < 0) toPoint.Y = 0;
                if (toPoint.X > Size.Width) toPoint.X = Size.Width;
                if (toPoint.Y > Size.Height) toPoint.Y = Size.Height;

                gfx.DrawLine(pen, fromPoint, toPoint);
            }
            catch (Exception)
            {
                Log("Draw ink failed", 1);
            }
            
            
        }

        private void processPenData(InkData inkData)
        {
            Log("Process pen data", 2);
            if (CurrentPenDevice == null) return;
            if (_currentLayout == null) return;
            if (!_inking) return;

            if(CurrentPenDevice.DeviceType == DEVICE_TYPE.SIGNPAD)
            {
                CurrPenStatus = inkData.Duplicate(CurrentPenDevice.TabletDimension, CurrentPenDevice.ScreenDimension);
            }
            else
            {
                CurrPenStatus = inkData.Duplicate();
                Point currPenPosition = new Point((int)CurrPenStatus.x, (int)CurrPenStatus.y);
                currPenPosition = this.PointToClient(currPenPosition);
                if(currPenPosition.X < 0 || currPenPosition.Y < 0 || currPenPosition.X > this.Width || currPenPosition.Y > this.Height)
                {
                    //return;
                }
                CurrPenStatus.x = (uint)currPenPosition.X;
                CurrPenStatus.y = (uint)currPenPosition.Y;
            }

            
            ElementButton button = _currentLayout.GetButtonOnPoint(CurrPenStatus.coordinates);
            if (button != null)
            {
                Log("Pen on button:" + button.Name);
                CurrPenStatus.tag = button.Name;
            }
            ElementImage image = _currentLayout.GetImageOnPoint(CurrPenStatus.coordinates);
            if (image != null)
            {
                if(image.Action != Element.DEFAULT_ACTIONS.None || image.Click != null)
                {
                    Log("Pen on image:" + image.Name);
                    CurrPenStatus.tag = image.Name;
                }   
            }
            
            if(DefaultPen == null)
            {
                DefaultPen = GetDefaultPen();
            }

            if (CurrPenStatus.tag != null && CurrPenStatus.tag != "")
            {
                //pen is on button/image and InkingOnButton is true, then draw ink, else do nothing
                if (InkingOnButton)
                {
                    //Change to save only screen dimension as of the control only                   
                    //_penData.Add(CurrPenStatus.Duplicate(_currentPenDevice.ScreenDimension, _currentPenDevice.TabletDimension));
                    _penData.Add(CurrPenStatus);
                    if (CurrPenStatus.contact && PrevPenStatus != null)
                    {
                        drawInk(DefaultPen, PrevPenStatus.coordinates, CurrPenStatus.coordinates);
                    }
                }
            }
            else
            {
                //pen is not on button/image, just draw ink
                ////Change to save only screen dimension as of the control only 
                //_penData.Add(CurrPenStatus.Duplicate(_currentPenDevice.ScreenDimension, _currentPenDevice.TabletDimension));
                _penData.Add(CurrPenStatus);
                if (CurrPenStatus.contact && PrevPenStatus != null)
                {
                    drawInk(DefaultPen, PrevPenStatus.coordinates, CurrPenStatus.coordinates);
                }
            }

            //Processed button pressed
            if (CurrPenStatus.contact == false && PrevPenStatus != null)
            {
                if (PrevPenStatus.contact && PrevPenStatus.tag != null && PrevPenStatus.tag != "")
                {
                    Log("Click button:" + PrevPenStatus.tag);
                    _currentLayout.ClickButton(PrevPenStatus.tag);
                }
            }

            PrevPenStatus = CurrPenStatus.Duplicate();
        }

        public void ReassignClickEvents()
        {
            Log("Reassign click events");
            if (_currentLayout == null) return;
            if(_currentLayout.ButtonList != null && _currentLayout.ButtonList.Count > 0)
            {
                foreach (ElementButton btn in _currentLayout.ButtonList)
                {
                    if (btn.Action == Element.DEFAULT_ACTIONS.Done)
                    {
                        Log("Button:" + btn.Name + " = Done");
                        btn.Click = Done;
                    }
                    else if (btn.Action == Element.DEFAULT_ACTIONS.Refresh)
                    {
                        Log("Button:" + btn.Name + " = Clear");
                        btn.Click = Clear;
                    }
                    else if (btn.Action == Element.DEFAULT_ACTIONS.Cancel)
                    {
                        Log("Button:" + btn.Name + " = Cancel");
                        btn.Click = Cancel;
                    }
                }
            }

            if(_currentLayout.ImageList != null && _currentLayout.ImageList.Count > 0)
            {
                foreach (ElementImage img in _currentLayout.ImageList)
                {
                    if (img.Action == Element.DEFAULT_ACTIONS.Done)
                    {
                        Log("Image:" + img.Name + " = Done");
                        img.Click = Done;
                    }
                    else if (img.Action == Element.DEFAULT_ACTIONS.Refresh)
                    {
                        Log("Image:" + img.Name + " = Clear");
                        img.Click = Clear;
                    }
                    else if (img.Action == Element.DEFAULT_ACTIONS.Cancel)
                    {
                        Log("Image:" + img.Name + " = Cancel");
                        img.Click = Cancel;
                    }
                }
            }
            
        }

        private void SignpadControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (CurrentPenDevice == null) return;
            if (_currentLayout == null) return;

            if (CurrentPenDevice.DeviceType != DEVICE_TYPE.SIGNPAD) return;

            Point pt = e.Location;
            if(CurrentPenDevice.DeviceType == DEVICE_TYPE.SIGNPAD)
            {
                pt = InkProcessor.ConvertCoordinate(e.Location, ClientSize, CurrentPenDevice.ScreenDimension);
            }
            
            if (_currentLayout != null && _currentLayout.ButtonList != null)
            {
                foreach (ElementButton btn in _currentLayout.ButtonList)
                {
                    if (btn.Bounds.Contains(pt))
                    {
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                }
            }
            
            if (_currentLayout != null && _currentLayout.ImageList != null)
            {
                foreach (ElementImage img in _currentLayout.ImageList)
                {
                    if (img.Bounds.Contains(pt) && img.IsButton)
                    {
                        this.Cursor = Cursors.Hand;
                        return;
                    }
                }
            }
            
            this.Cursor = Cursors.Default;
        }

        private void SignpadControl_MouseClick(object sender, MouseEventArgs e)
        {
            Log("SignpadControl mouse click");
            if (CurrentPenDevice == null) return;
            if (_currentLayout == null) return;

            if (CurrentPenDevice.DeviceType != DEVICE_TYPE.SIGNPAD) return;

            Point pt = e.Location;
            if(CurrentPenDevice.DeviceType == DEVICE_TYPE.SIGNPAD)
            {
                pt = InkProcessor.ConvertCoordinate(e.Location, ClientSize, CurrentPenDevice.ScreenDimension);
            }
            
            if (_currentLayout != null && _currentLayout.ButtonList != null)
            {
                foreach (ElementButton btn in _currentLayout.ButtonList)
                {
                    if (btn.Bounds.Contains(pt))
                    {
                        Log("Button:" + btn.Name + " perform click");
                        btn.PerformClick();
                        break;
                    }
                }
            }
            

            if (_currentLayout != null && _currentLayout.ImageList != null)
            {
                foreach (ElementImage img in _currentLayout.ImageList)
                {
                    if (img.Bounds.Contains(pt))
                    {
                        Log("Image:" + img.Name + " perform click");
                        img.PerformClick();
                    }
                }
            }
            
        }

        private void resetGraphics()
        {
            Log("Reset graphics");
            gfx = this.CreateGraphics();
            gfx.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            gfx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            gfx.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        }

        private void SignpadControl_Paint(object sender, PaintEventArgs e)
        {
            Log("Repaint");
            if (CurrentPenDevice == null) return;

            try
            {
                if (_penData.Count != 0)
                {
                    // Redraw all the pen data up until now!
                    if (gfx == null)
                    {
                        resetGraphics();
                    }

                    bool isDown = false;
                    Point prev = new Point();
                    for (int i = 0; i < _penData.Count; ++i)
                    {
                        if (_penData[i].contact)
                        {
                            if (!isDown)
                            {
                                isDown = true;
                                prev = InkProcessor.ConvertCoordinate(_penData[i].coordinates, CurrentPenDevice.TabletDimension, ClientSize);
                            }
                            else
                            {
                                Point curr = InkProcessor.ConvertCoordinate(_penData[i].coordinates, CurrentPenDevice.TabletDimension, ClientSize);
                                gfx.DrawLine(DefaultPen, prev, curr);
                                prev = curr;
                            }
                        }
                        else
                        {
                            isDown = false;
                        }
                    }
                }
            }
            catch (Exception) { }
            
        }

        private void SignpadControl_LocationChanged(object sender, EventArgs e)
        {
            if (CurrentPenDevice == null) return;
            if (CurrentPenDevice.DeviceType == DEVICE_TYPE.SIGNPAD || 
                CurrentPenDevice.DeviceType == DEVICE_TYPE.PEN_TABLET)
                    return;

            if (Screen.AllScreens.Count() > 1)
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.Bounds.Contains(Location))
                    {
                        CurrentPenDevice.ScreenDimension = screen.Bounds.Size;
                        return;
                    }
                }
            }
            else
            {
                CurrentPenDevice.ScreenDimension = Screen.PrimaryScreen.Bounds.Size;
            }
        }

        private void SignpadControl_SizeChanged(object sender, EventArgs e)
        {
            this.resetGraphics();
            SignpadControl_Paint(this, null);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            Disconnect();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
