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
using InkPlatform.UserControls;
using wgssSTU;
using System.Reflection;
using System.IO;

namespace InkPlatformTest
{
    public partial class Form1 : Form
    {
        List<PenDevice> penDevices = new List<PenDevice>();
        PenDevice currentPenDevice = null;
        List<string> _layoutFiles;

        public Form1()
        {
            InitializeComponent();
        }

        private void Log(string msg, int alertType)
        {
            if (alertType == 2 || alertType == 12) return;

            string prefix = "SignpadControl: ";
            if(alertType >= 10)
            {
                prefix = "WacomSignpad: ";
            }

            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.SelectionLength = 0;

            if(alertType == 1 || alertType == 11)
            {
                txtLog.SelectionColor = Color.Red;
            }

            txtLog.AppendText(prefix + msg + Environment.NewLine);
            
            if (alertType == 1 || alertType == 11)
            {
                txtLog.SelectionColor = txtLog.ForeColor;
            }

            txtLog.ScrollToCaret();
            
        }

        private Layout GetColorSignatureLayout()
        {
            Layout layout = new Layout("ColorSignature");
            
            ElementImage screen = new ElementImage("Screen", Properties.Resources.signature_color);
            layout.AddElement(screen);

            ElementImage acceptButton = new ElementImage(
                    "Accept",
                    Properties.Resources.accept_button,
                    550,
                    384
                );
            acceptButton.Action = Element.DEFAULT_ACTIONS.Done;
            layout.AddElement(acceptButton);

            ElementImage cancelButton = new ElementImage("Cancel",
                    Properties.Resources.cancel_button,
                    550,
                    300
                );
            cancelButton.Action = Element.DEFAULT_ACTIONS.Cancel;
            layout.AddElement(cancelButton);
            
            layout.AddElement("who", "Name of Signer", 30, 460, 15);
            layout.AddElement("why", "Reason for Signing", 334, 460, 15);

            return layout;
        }

        private void Log(string msg)
        {
            txtLog.Text = txtLog.Text + msg + Environment.NewLine;
            txtLog.SelectionStart = txtLog.Text.Length;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            scan();
        }

        private void scan()
        {
            cboDevices.Items.Clear();
            DeviceScanner deviceScanner = new DeviceScanner();
            penDevices = deviceScanner.Scan();
            Log(penDevices.Count + " devices detected");
            foreach (PenDevice penDevice in penDevices)
            {
                Log(penDevice.ProductModel + " detected");
                cboDevices.Items.Add(penDevice.ProductModel);
            }

            if (cboDevices.Items.Count > 0)
            {
                cboDevices.SelectedIndex = 0;
                currentPenDevice = penDevices[0];
                resizeForm(currentPenDevice);
            }
            else
            {
                currentPenDevice = null;
            }
        }
        
        private void btnGetSignature_Click(object sender, EventArgs e)
        {
            if(currentPenDevice != null)
            {
                SignatureLayout layout = new SignatureLayout("layout1", "Name", "Reason");
                signpadControl1.DonePressed = SignatureDone;
                signpadControl1.SetInking(true);
                int result = signpadControl1.DisplayLayout(layout, currentPenDevice);

                if(result == (int)PEN_DEVICE_ERROR.NONE)
                {
                    
                }
            }
        }

        private void resizeForm(PenDevice signpad)
        {
            signpadControl1.Width = signpad.ScreenDimension.Width;
            signpadControl1.Height = signpad.ScreenDimension.Height;
            txtLog.Location = new Point(txtLog.Location.X, signpadControl1.Location.Y + signpadControl1.Size.Height + 10);
            txtLog.Width = signpadControl1.Width;
            ClientSize = new Size(signpadControl1.Width, txtLog.Location.Y + txtLog.Size.Height);
            //MessageBox.Show(ClientSize.Width.ToString() + " - " + signpadControl1.Width.ToString());
        }

        private bool SignatureDone(object sender, EventArgs e)
        {
            if(sender.GetType() == typeof(ElementButton))
            {
                ElementButton btn = (ElementButton)sender;
                if(btn.NextScreenName != null && btn.NextScreenName != "")
                {
                    return true;
                }
            }

            if (sender.GetType() == typeof(ElementImage))
            {
                ElementImage btn = (ElementImage)sender;
                if (btn.NextScreenName != null && btn.NextScreenName != "")
                {
                    return true;
                }
            }

            signpadControl1.SetInking(false);

            Bitmap bitmap = new Bitmap(currentPenDevice.ScreenDimension.Width, currentPenDevice.ScreenDimension.Height);
            if (InkProcessor.GenerateImageFromInkData(
                out bitmap,
                signpadControl1.PenData,
                currentPenDevice.TabletDimension,
                currentPenDevice.ScreenDimension, 
                signpadControl1.GetDefaultPen(),
                Color.White,
                true,
                true) == InkProcessor.GenerateImageResult.Successful     
            ){
                signpadControl1.DisplayBitmap(bitmap, currentPenDevice);

                return false;
            }
            else
            {
                return true;
            }
            
        }

        private void cboDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboDevices.Items.Count > 0)
            {
                currentPenDevice = penDevices[cboDevices.SelectedIndex];
                resizeForm(currentPenDevice);
            }
            else
            {
                currentPenDevice = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            signpadControl1.Logging = true;
            signpadControl1.LogFunction = Log;
            scan();
        }

        private void btnColorSignature_Click(object sender, EventArgs e)
        {
            if (currentPenDevice == null) return;
            Layout layout = GetColorSignatureLayout();
            
            int result = signpadControl1.DisplayLayout(layout, currentPenDevice);
            if(result != 0)
            {
                MessageBox.Show(currentPenDevice.PenDeviceErrorMessage((PEN_DEVICE_ERROR)result));
            }
        }

        private List<Layout> GetBoxLayoutShow()
        {
            BoxLayout doneLayout = new BoxLayout("box");
            doneLayout.Flow = BoxLayout.FLOW.DOWN;
            doneLayout.Spacing = 2;
            doneLayout.setHorizontalRatio(1, 5, 1);
            doneLayout.ShowGrid = true;
            ElementButton btnA = new ElementButton("A", "Button A");
            ElementButton btnB = new ElementButton("B", "Button B");
            ElementButton btnC = new ElementButton("C", "Button C");

            doneLayout.AddElement(btnA, BoxLayout.POSITION.MIDDLE_CENTRE);
            doneLayout.AddElement(btnB, BoxLayout.POSITION.MIDDLE_CENTRE);
            doneLayout.AddElement(btnC, BoxLayout.POSITION.MIDDLE_CENTRE);

            ElementButton btnBack = new ElementButton("Back", "Back");
            btnBack.NextScreenName = doneLayout.Name;
            btnBack.Action = Element.DEFAULT_ACTIONS.Done;
            
            ElementButton btnFinish = new ElementButton("Finish", "Finish");
            btnFinish.Action = Element.DEFAULT_ACTIONS.Done;

            BoxLayout layoutA = new BoxLayout("A");
            layoutA.setHorizontalRatio(1, 5, 1);
            layoutA.ShowGrid = true;
            ElementText txtA = layoutA.AddElement("You pressed button A", BoxLayout.POSITION.MIDDLE_CENTRE);
            txtA.FontSize = 30;
            layoutA.AddElement(btnBack, BoxLayout.POSITION.BOTTOM_LEFT);
            layoutA.AddElement(btnFinish, BoxLayout.POSITION.BOTTOM_RIGHT);

            BoxLayout layoutB = new BoxLayout("B");
            layoutB.setHorizontalRatio(1, 5, 1);
            layoutB.ShowGrid = true;
            ElementText txtB = layoutB.AddElement("You pressed button B", BoxLayout.POSITION.MIDDLE_CENTRE);
            txtB.FontSize = 30;
            layoutB.AddElement(btnBack, BoxLayout.POSITION.BOTTOM_LEFT);
            layoutB.AddElement(btnFinish, BoxLayout.POSITION.BOTTOM_RIGHT);

            BoxLayout layoutC = new BoxLayout("C");
            layoutC.setHorizontalRatio(1, 5, 1);
            layoutC.ShowGrid = true;
            ElementText txtC = layoutC.AddElement("You pressed button C", BoxLayout.POSITION.MIDDLE_CENTRE);
            txtC.FontSize = 30;
            layoutC.AddElement(btnBack, BoxLayout.POSITION.BOTTOM_LEFT);
            layoutC.AddElement(btnFinish, BoxLayout.POSITION.BOTTOM_RIGHT);

            btnA.NextScreenName = layoutA.Name;
            btnB.NextScreenName = layoutB.Name;
            btnC.NextScreenName = layoutC.Name;

            btnA.Action = Element.DEFAULT_ACTIONS.Done;
            btnB.Action = Element.DEFAULT_ACTIONS.Done;
            btnC.Action = Element.DEFAULT_ACTIONS.Done;

            List<Layout> result = new List<InkPlatform.UserInterface.Layout>();
            result.Add(doneLayout);
            result.Add(layoutA);
            result.Add(layoutB);
            result.Add(layoutC);

            string json = JSONSerializer.SerializeLayout(doneLayout);

            return result; 
        }

        private void btnBox_Click(object sender, EventArgs e)
        {
            if (currentPenDevice == null) return;

            List<Layout> boxLayoutList = GetBoxLayoutShow();
            signpadControl1.DisplayLayouts(boxLayoutList, currentPenDevice);

        }

        private void OpenLayoutFiles()
        {
            openFileDialog.Filter = "JSON files | *.json";
            DialogResult result = openFileDialog.ShowDialog();

            _layoutFiles = new List<string>();

            if (result == DialogResult.OK)
            {
                _layoutFiles.Clear();

                foreach (string file in openFileDialog.FileNames)
                {
                    _layoutFiles.Add(file);
                }
                
                cboInitialFile.Items.Clear();
                foreach (string file in _layoutFiles)
                {
                    cboInitialFile.Items.Add(Path.GetFileName(file));
                }
                cboInitialFile.SelectedIndex = 0;
            }
        }

        private void btnLayout_Click(object sender, EventArgs e)
        {
            OpenLayoutFiles();
        }
        
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if(cboInitialFile.Items.Count == 0)
            {
                MessageBox.Show("Initial file empty");
                return;
            }
            
            try
            {
                List<Layout> layoutList = LayoutManager.ReadLayoutFiles(_layoutFiles);
                signpadControl1.DonePressed = SignatureDone;
                signpadControl1.SetInking(true);
                signpadControl1.DisplayLayouts(layoutList, currentPenDevice, cboInitialFile.SelectedIndex);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        
    }
}
