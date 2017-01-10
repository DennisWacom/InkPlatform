namespace InkPlatformTest
{
    partial class Form1
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
            this.btnScanDevice = new System.Windows.Forms.Button();
            this.cboDevices = new System.Windows.Forms.ComboBox();
            this.btnGetSignature = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnLayout = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnColorSignature = new System.Windows.Forms.Button();
            this.btnBox = new System.Windows.Forms.Button();
            this.cboInitialFile = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.signpadControl1 = new InkPlatform.UserControls.SignpadControl();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnScanDevice
            // 
            this.btnScanDevice.Location = new System.Drawing.Point(1, 12);
            this.btnScanDevice.Name = "btnScanDevice";
            this.btnScanDevice.Size = new System.Drawing.Size(95, 23);
            this.btnScanDevice.TabIndex = 0;
            this.btnScanDevice.Text = "Scan Device";
            this.btnScanDevice.UseVisualStyleBackColor = true;
            this.btnScanDevice.Click += new System.EventHandler(this.button1_Click);
            // 
            // cboDevices
            // 
            this.cboDevices.FormattingEnabled = true;
            this.cboDevices.Location = new System.Drawing.Point(102, 14);
            this.cboDevices.Name = "cboDevices";
            this.cboDevices.Size = new System.Drawing.Size(131, 21);
            this.cboDevices.TabIndex = 2;
            this.cboDevices.SelectedIndexChanged += new System.EventHandler(this.cboDevices_SelectedIndexChanged);
            // 
            // btnGetSignature
            // 
            this.btnGetSignature.Location = new System.Drawing.Point(242, 41);
            this.btnGetSignature.Name = "btnGetSignature";
            this.btnGetSignature.Size = new System.Drawing.Size(78, 23);
            this.btnGetSignature.TabIndex = 4;
            this.btnGetSignature.Text = "Sign";
            this.btnGetSignature.UseVisualStyleBackColor = true;
            this.btnGetSignature.Click += new System.EventHandler(this.btnGetSignature_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(1, 253);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(423, 122);
            this.txtLog.TabIndex = 5;
            this.txtLog.Text = "";
            // 
            // btnLayout
            // 
            this.btnLayout.Location = new System.Drawing.Point(2, 70);
            this.btnLayout.Name = "btnLayout";
            this.btnLayout.Size = new System.Drawing.Size(78, 23);
            this.btnLayout.TabIndex = 6;
            this.btnLayout.Text = "Browse";
            this.btnLayout.UseVisualStyleBackColor = true;
            this.btnLayout.Click += new System.EventHandler(this.btnLayout_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            // 
            // btnColorSignature
            // 
            this.btnColorSignature.Location = new System.Drawing.Point(2, 41);
            this.btnColorSignature.Name = "btnColorSignature";
            this.btnColorSignature.Size = new System.Drawing.Size(106, 23);
            this.btnColorSignature.TabIndex = 7;
            this.btnColorSignature.Text = "Color Signature";
            this.btnColorSignature.UseVisualStyleBackColor = true;
            this.btnColorSignature.Click += new System.EventHandler(this.btnColorSignature_Click);
            // 
            // btnBox
            // 
            this.btnBox.Location = new System.Drawing.Point(114, 41);
            this.btnBox.Name = "btnBox";
            this.btnBox.Size = new System.Drawing.Size(119, 23);
            this.btnBox.TabIndex = 8;
            this.btnBox.Text = "Box Layout";
            this.btnBox.UseVisualStyleBackColor = true;
            this.btnBox.Click += new System.EventHandler(this.btnBox_Click);
            // 
            // cboInitialFile
            // 
            this.cboInitialFile.FormattingEnabled = true;
            this.cboInitialFile.Location = new System.Drawing.Point(89, 70);
            this.cboInitialFile.Name = "cboInitialFile";
            this.cboInitialFile.Size = new System.Drawing.Size(144, 21);
            this.cboInitialFile.TabIndex = 9;
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(242, 68);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(78, 23);
            this.btnLoad.TabIndex = 10;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // signpadControl1
            // 
            this.signpadControl1.BackColor = System.Drawing.Color.White;
            this.signpadControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.signpadControl1.DefaultInkWidth = 0.7F;
            this.signpadControl1.DefaultPenColor = System.Drawing.Color.DarkBlue;
            this.signpadControl1.InkingOnButton = true;
            this.signpadControl1.Location = new System.Drawing.Point(1, 97);
            this.signpadControl1.Logging = true;
            this.signpadControl1.Name = "signpadControl1";
            this.signpadControl1.ResizeCondition = InkPlatform.UserControls.SignpadControl.RESIZE_CONDITION.ACTUAL_SIZE;
            this.signpadControl1.Size = new System.Drawing.Size(424, 150);
            this.signpadControl1.TabIndex = 3;
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(242, 12);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(78, 23);
            this.btnDisconnect.TabIndex = 11;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 383);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.cboInitialFile);
            this.Controls.Add(this.btnBox);
            this.Controls.Add(this.btnColorSignature);
            this.Controls.Add(this.btnLayout);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnGetSignature);
            this.Controls.Add(this.signpadControl1);
            this.Controls.Add(this.cboDevices);
            this.Controls.Add(this.btnScanDevice);
            this.Name = "Form1";
            this.Text = "InkPlatform Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnScanDevice;
        private System.Windows.Forms.ComboBox cboDevices;
        private InkPlatform.UserControls.SignpadControl signpadControl1;
        private System.Windows.Forms.Button btnGetSignature;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Button btnLayout;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnColorSignature;
        private System.Windows.Forms.Button btnBox;
        private System.Windows.Forms.ComboBox cboInitialFile;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnDisconnect;
    }
}

