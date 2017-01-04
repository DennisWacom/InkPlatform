namespace InkPlatform.UserControls
{
    partial class SignpadWindow
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
            this.signpadControl = new InkPlatform.UserControls.SignpadControl();
            this.SuspendLayout();
            // 
            // signpadControl
            // 
            this.signpadControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.signpadControl.BackColor = System.Drawing.Color.White;
            this.signpadControl.DefaultInkWidth = 0.7F;
            this.signpadControl.DefaultPenColor = System.Drawing.Color.DarkBlue;
            this.signpadControl.InkingOnButton = false;
            this.signpadControl.Location = new System.Drawing.Point(0, 0);
            this.signpadControl.Logging = false;
            this.signpadControl.Name = "signpadControl";
            this.signpadControl.ResizeCondition = InkPlatform.UserControls.SignpadControl.RESIZE_CONDITION.ACTUAL_SIZE;
            this.signpadControl.Size = new System.Drawing.Size(283, 260);
            this.signpadControl.TabIndex = 0;
            this.signpadControl.SizeChanged += new System.EventHandler(this.signpadControl_SizeChanged);
            // 
            // SignpadWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.signpadControl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SignpadWindow";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SignpadWindow";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SignpadWindow_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private SignpadControl signpadControl;
    }
}