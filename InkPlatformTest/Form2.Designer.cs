namespace InkPlatformTest
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnLayout = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.cboInitialFile = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.cboDevices = new System.Windows.Forms.ComboBox();
            this.btnScanDevice = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLayout
            // 
            this.btnLayout.Location = new System.Drawing.Point(10, 43);
            this.btnLayout.Name = "btnLayout";
            this.btnLayout.Size = new System.Drawing.Size(95, 23);
            this.btnLayout.TabIndex = 7;
            this.btnLayout.Text = "Browse";
            this.btnLayout.UseVisualStyleBackColor = true;
            this.btnLayout.Click += new System.EventHandler(this.btnLayout_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // cboInitialFile
            // 
            this.cboInitialFile.FormattingEnabled = true;
            this.cboInitialFile.Location = new System.Drawing.Point(111, 43);
            this.cboInitialFile.Name = "cboInitialFile";
            this.cboInitialFile.Size = new System.Drawing.Size(131, 21);
            this.cboInitialFile.TabIndex = 8;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(251, 43);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(78, 23);
            this.btnLoad.TabIndex = 11;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(2, 82);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(413, 114);
            this.txtLog.TabIndex = 12;
            this.txtLog.Text = "";
            // 
            // cboDevices
            // 
            this.cboDevices.FormattingEnabled = true;
            this.cboDevices.Location = new System.Drawing.Point(111, 12);
            this.cboDevices.Name = "cboDevices";
            this.cboDevices.Size = new System.Drawing.Size(131, 21);
            this.cboDevices.TabIndex = 14;
            // 
            // btnScanDevice
            // 
            this.btnScanDevice.Location = new System.Drawing.Point(10, 10);
            this.btnScanDevice.Name = "btnScanDevice";
            this.btnScanDevice.Size = new System.Drawing.Size(95, 23);
            this.btnScanDevice.TabIndex = 13;
            this.btnScanDevice.Text = "Scan Device";
            this.btnScanDevice.UseVisualStyleBackColor = true;
            this.btnScanDevice.Click += new System.EventHandler(this.btnScanDevice_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(416, 199);
            this.Controls.Add(this.cboDevices);
            this.Controls.Add(this.btnScanDevice);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cboInitialFile);
            this.Controls.Add(this.btnLayout);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnLayout;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox cboInitialFile;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.ComboBox cboDevices;
        private System.Windows.Forms.Button btnScanDevice;
    }
}