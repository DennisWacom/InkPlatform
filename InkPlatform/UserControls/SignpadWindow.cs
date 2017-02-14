using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InkPlatform.UserInterface;
using InkPlatform.Hardware;
using InkPlatform.Ink;

namespace InkPlatform.UserControls
{
    public delegate void PrefixedSendLog(string prefix, string msg, int alertType);

    public partial class SignpadWindow: Form
    {
        public SignpadWindow()
        {
            InitializeComponent();
            signpadControl.Logging = true;
            signpadControl.LogFunction = ReceivedSignpadControlLog;
            signpadControl.DonePressed = Done;
            signpadControl.CancelPressed = Cancel;
            signpadControl.ClearPressed = Clear;
        }

        public ButtonEvent DonePressed;
        public ButtonEvent CancelPressed;
        public ButtonEvent ClearPressed;

        private bool _closeWindowOnDone = true;
        public bool CloseWindowOnDone
        {
            get { return _closeWindowOnDone; }
            set { _closeWindowOnDone = value; }
        }

        private bool _closeWindowOnCancel = true;
        public bool CloseWindowOnCancel
        {
            get { return _closeWindowOnCancel; }
            set { _closeWindowOnCancel = value; }
        }

        public PrefixedSendLog LogFunction;
        public string LogPrefix = "";
        
        private bool _logging = true;
        public bool Logging
        {
            get { return _logging; }
            set
            {
                _logging = value;
                signpadControl.Logging = value;
                signpadControl.LogFunction = ReceivedSignpadControlLog;
            }
        }

        protected void Log(string msg)
        {
            Log(msg, 0);
        }

        protected void Log(string msg, int alertType)
        {
            if(LogFunction != null && _logging)
            {
                LogFunction(LogPrefix, msg, alertType);
            }   
        }

        protected void ReceivedSignpadControlLog(string msg, int alertType)
        {
            Log("[SignpadControl] " + msg, alertType + 10);
        }

        public Color DefaultPenColor
        {
            get { return signpadControl.DefaultPenColor; }
            set { signpadControl.DefaultPenColor = value; }
        }

        public float DefaultInkWidth
        {
            get { return signpadControl.DefaultInkWidth; }
            set { signpadControl.DefaultInkWidth = value; }
        }

        public bool InkingOnButton
        {
            get { return signpadControl.InkingOnButton; }
            set { signpadControl.InkingOnButton = value; }
        }

        public List<InkData> PenData
        {
            get { return signpadControl.PenData; }
        }

        public ContextPenData ContextPenData
        {
            get { return signpadControl.ContextPenData; }
        }

        public Layout CurrentLayout
        {
            get { return signpadControl.CurrentLayout; }
        }   

        public List<Layout> AllLayouts
        {
            get { return signpadControl.AllLayouts; }
        }

        public Bitmap CurrentBitmap
        {
            get { return signpadControl.CurrentBitmap; }
        }

        public void SetInking(bool inking)
        {
            signpadControl.SetInking(inking);
        }

        public bool Cancel(object sender, UserInterface.LayoutEventArgs e)
        {
            Log("Cancel event handler");
            bool carryOn = true;
            if (CancelPressed != null)
            {
                carryOn = CancelPressed(sender, e);
            }

            if (_closeWindowOnCancel)
            {
                Log("Set dialog result = cancel");
                DialogResult = DialogResult.Cancel;
                //this.Close();
                return false;
            }

            return carryOn;
        }

        public bool Clear(object sender, UserInterface.LayoutEventArgs e)
        {
            Log("Clear event handler");
            
            bool carryOn = true;
            if (ClearPressed != null)
            {
                carryOn = ClearPressed(sender, e);
            }
            return carryOn;
        }

        public bool Done(object sender, UserInterface.LayoutEventArgs e)
        {
            Log("Done event handler");
            
            bool carryOn = true;
            if(DonePressed != null)
            {
                carryOn = DonePressed(sender, e);
            }
            if (_closeWindowOnDone && TestDoneFinish(sender))
            {
                Log("Set DialogResult = OK");
                DialogResult = DialogResult.OK;
                return false;
            }

            return carryOn;
        }

        public bool TestDoneFinish(object eventSender)
        {
            if (eventSender.GetType() == typeof(ElementButton))
            {
                ElementButton btn = (ElementButton)eventSender;
                if (btn.NextScreenName == null || btn.NextScreenName == "")
                {
                    return true;
                }
            }
            else if(eventSender.GetType() == typeof(ElementImage))
            {
                ElementImage img = (ElementImage)eventSender;
                if(img.NextScreenName == null || img.NextScreenName == "")
                {
                    return true;
                }
            }
            return false;
        }

        public int ShowDialog(int result, IWin32Window owner)
        {
            if (result != (int)PEN_DEVICE_ERROR.NONE)
            {
                return result;
            }

            DialogResult dr = this.ShowDialog(owner);
            if (dr == DialogResult.Cancel)
            {
                return (int)PEN_DEVICE_ERROR.USER_CANCELLED;
            }
            else
            {
                return (int)PEN_DEVICE_ERROR.NONE;
            }
        }

        public int CaptureSignature(string who, string why, PenDevice penDevice)
        {
            int result = signpadControl.CaptureSignature(who, why, penDevice);
            return result;
        }

        public int CaptureSignatureDialog(string who, string why, PenDevice penDevice, IWin32Window owner)
        {
            int result = CaptureSignature(who, why, penDevice);
            return ShowDialog(result, owner);
        }

        public int DisplayBitmap(Bitmap bitmap, PenDevice penDevice)
        {
            return signpadControl.DisplayBitmap(bitmap, penDevice);
        }

        public int DisplayBitmapDialog(Bitmap bitmap, PenDevice penDevice, IWin32Window owner)
        {
            int result = DisplayBitmap(bitmap, penDevice);
            return ShowDialog(result, owner);
        }

        public int DisplayLayouts(List<Layout> layoutList, PenDevice penDevice, int initialLayout)
        {
            return signpadControl.DisplayLayouts(layoutList, penDevice, initialLayout);
        }

        public int DisplayLayoutsDialog(List<Layout> layoutList, PenDevice penDevice, int initialLayout, IWin32Window owner)
        {
            int result = DisplayLayouts(layoutList, penDevice, initialLayout);
            return ShowDialog(result, owner);
        }

        public int DisplayLayout(Layout layout, PenDevice penDevice)
        {
            return signpadControl.DisplayLayout(layout, penDevice);
        }

        public int DisplayLayoutDialog(Layout layout, PenDevice penDevice, IWin32Window owner)
        {
            int result = signpadControl.DisplayLayout(layout, penDevice);
            return ShowDialog(result, owner);
        }

        public void Disconnect()
        {
            signpadControl.Disconnect();
        }

        public int Connect(PenDevice penDevice)
        {
            return signpadControl.Connect(penDevice);
        }

        public void Reset()
        {
            signpadControl.Reset();
        }

        public void RefreshScreen()
        {
            signpadControl.RefreshScreen();
        }

        public void ClearScreen()
        {
            signpadControl.ClearScreen();
        }

        private void SignpadWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClearScreen();
            Disconnect();
        }

        private void signpadControl_SizeChanged(object sender, EventArgs e)
        {
            ClientSize = signpadControl.Size;
        }
    }
}
