using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserInterface;

namespace InkPlatform.UserControls
{
    public partial class SignpadWindow : Form
    {
        private bool _logging = false;
        public bool Logging
        {
            get { return _logging; }
            set { _logging = value; }
        }

        public SendLog LogFunction
        {
            get { return signpadControl.LogFunction; }
            set
            {
                signpadControl.Logging = (value == null ? false: true);
                signpadControl.LogFunction = value;
            }
        }
        public ButtonEvent DonePressed;
        public ButtonEvent CancelPressed;
        public ButtonEvent ClearPressed;
        
        public PenDevice CurrentPenDevice
        {
            get { return signpadControl.CurrentPenDevice; }
        }

        public bool InkingOnButton
        {
            get { return signpadControl.InkingOnButton; }
            set { signpadControl.InkingOnButton = value; }
        }

        public SignpadWindow()
        {
            InitializeComponent();
            signpadControl.DonePressed = Done;
            signpadControl.CancelPressed = Cancel;
            signpadControl.ClearPressed = Clear;
        }

        public List<InkData> PenData
        {
            get { return signpadControl.PenData; }
        }

        public ContextPenData ContextPenData
        {
            get { return signpadControl.ContextPenData; }
        }

        public int CaptureSignature(string who, string why, PenDevice penDevice)
        {
            int result = signpadControl.CaptureSignature(who, why, penDevice);
            return result; 
        }

        public int CaptureSignature(string who, string why)
        {
            int result = signpadControl.CaptureSignature(who, why);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }

            return result;
        }

        public bool Clear(object sender, UserInterface.LayoutEventArgs e)
        {
            bool carryOn = true;
            if (ClearPressed != null)
            {
                carryOn = ClearPressed(sender, e);
            }

            if (!carryOn) return false;
            
            return true;
        }

        public bool Cancel(object sender, UserInterface.LayoutEventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            bool carryOn = true;
            if(CancelPressed != null)
            {
                carryOn = CancelPressed(sender, e);
            }

            if (!carryOn) return false;

            this.Close();

            return true;
        }

        public bool Done(object sender, UserInterface.LayoutEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            bool carryOn = true;
            if(DonePressed != null)
            {
                carryOn = DonePressed(sender, e);
            }

            if (!carryOn) return false;

            if(sender.GetType() == typeof(ElementButton))
            {
                ElementButton btn = (ElementButton)sender;
                if(btn.NextScreenName == null || btn.NextScreenName == "")
                {
                    signpadControl.ClearScreen();
                    this.Close();
                    return false;
                }
            }else if(sender.GetType() == typeof(ElementImage))
            {
                ElementImage img = (ElementImage)sender;
                if(img.NextScreenName == null || img.NextScreenName == "")
                {
                    signpadControl.ClearScreen();
                    this.Close();
                    return false;
                }
            }

            return true;

        }

        public int DisplayBitmap(Bitmap bitmap)
        {
            int result = signpadControl.DisplayBitmap(bitmap);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        public int DisplayBitmap(Bitmap bitmap, PenDevice penDevice)
        {
            int result = signpadControl.DisplayBitmap(bitmap, penDevice);
            if(result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        public int DisplayLayouts(List<Layout> layoutList)
        {
            int result = signpadControl.DisplayLayouts(layoutList);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        public int DisplayLayouts(List<Layout> layoutList, PenDevice penDevice, int initialLayout)
        {
            int result = signpadControl.DisplayLayouts(layoutList, penDevice, initialLayout);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        public int DisplayLayouts(List<Layout> layoutList, PenDevice penDevice)
        {
            int result = signpadControl.DisplayLayouts(layoutList, penDevice);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        public int DisplayLayout(Layout layout)
        {
            int result = signpadControl.DisplayLayout(layout);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        public int DisplayLayout(Layout layout, PenDevice penDevice)
        {
            int result = signpadControl.DisplayLayout(layout, penDevice);
            if (result == (int)PEN_DEVICE_ERROR.NONE)
            {
                this.ShowDialog();
            }
            return result;
        }

        private void signpadControl_SizeChanged(object sender, EventArgs e)
        {
            ClientSize = new Size(signpadControl.Width, signpadControl.Height);
            
        }

        private void SignpadWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                signpadControl.TurnOff();
            }
            catch (Exception)
            {
                
            }
        }

        private void SignpadWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult = DialogResult.Cancel;
            }
           
        }
    }
}
