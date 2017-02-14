using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InkPlatform;
using InkPlatform.Hardware;
using InkPlatform.Hardware.Wacom;
using InkPlatform.Ink;
using InkPlatform.UserControls;
using InkPlatform.UserInterface;
using System.IO;

namespace InkPlatformTest
{
    public partial class Form2 : Form
    {
        List<PenDevice> penDevices = new List<PenDevice>();
        PenDevice currentPenDevice = null;
        List<string> _layoutFiles;

        public Form2()
        {
            InitializeComponent();
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

        private void Log(string msg)
        {
            txtLog.Text = txtLog.Text + msg + Environment.NewLine;
            txtLog.SelectionStart = txtLog.Text.Length;
        }

        private void Log(string logprefix, string msg, int alertType)
        {
            if (alertType % 2 == 0) return;

            string prefix = "SignpadControl: ";
            if (alertType >= 10)
            {
                prefix = "WacomSignpad: ";
            }

            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.SelectionLength = 0;

            if (alertType == 1 || alertType == 11)
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
            }
            else
            {
                currentPenDevice = null;
            }
        }

        private void btnLayout_Click(object sender, EventArgs e)
        {
            OpenLayoutFiles();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cboInitialFile.Items.Count == 0)
            {
                MessageBox.Show("Initial file empty");
                return;
            }
            
            try
            {
                List<Layout> layoutList = LayoutManager.ReadLayoutFiles(_layoutFiles);
                SignpadWindow spw = new SignpadWindow();
                spw.DonePressed = DonePressed;
                spw.CancelPressed = CancelPressed;
                spw.ClearPressed = ClearPressed;
                spw.Logging = true;
                spw.LogFunction = Log;
                spw.DisplayLayoutsDialog(layoutList, currentPenDevice, cboInitialFile.SelectedIndex, this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private bool ClearPressed(object sender, InkPlatform.UserInterface.LayoutEventArgs e)
        {
            Log("Clear pressed on Layout - " + e.Layout.Name);
            return true;
        } 

        private bool CancelPressed(object sender, InkPlatform.UserInterface.LayoutEventArgs e)
        {
            Log("Cancel pressed on Layout - " + e.Layout.Name);
            return true;
        }

        private bool DonePressed(object sender, InkPlatform.UserInterface.LayoutEventArgs e)
        {
           
            Log("Done pressed on Layout - " + e.Layout.Name);

            bool CollectSignature = false;
            if (sender.GetType() == typeof(ElementButton))
            {
                ElementButton btn = (ElementButton)sender;
                if (btn.NextScreenName == null || btn.NextScreenName == "")
                {
                    CollectSignature = true;
                }
            }
            else if (sender.GetType() == typeof(ElementImage))
            {
                ElementImage img = (ElementImage)sender;
                if (img.NextScreenName == null || img.NextScreenName == "")
                {
                    CollectSignature = true;
                }
            }

            if (CollectSignature)
            {
                if(e.PenData != null && e.PenData.Count > 0)
                {
                    try
                    {
                        string base64 = InkProcessor.Base64Encode(e.PenData);
                        Log(base64);
                        List<InkData> data = InkProcessor.Base64Decode(base64);
                        string json = InkProcessor.SerializeInkDataListToJson(data);
                        Log(json);
                        List<InkData> data2 = InkProcessor.DeserializeJsonToInkDataList(json);
                        Log((data2 == null ? "null" : data2.Count.ToString()));
                    }
                    catch(Exception ex)
                    {
                        Log(ex.Message);
                    }
                    
                }
            }

            return true;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            scan();
        }

        private void btnScanDevice_Click(object sender, EventArgs e)
        {
            scan();
        }
    }
}
