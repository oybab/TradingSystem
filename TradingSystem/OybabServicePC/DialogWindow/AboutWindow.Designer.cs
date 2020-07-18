namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class AboutWindow
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
            this.krppImage = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.krplSoft = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbDonate = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplLicense = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.krplVersionNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplVersion = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.krplRemainingTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRemainingTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplDeviceCount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplDeviceCountValue = new Oybab.ServicePC.DialogWindow.RLabel();
            this.krplTableCount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTableCountValue = new Oybab.ServicePC.DialogWindow.RLabel();
            this.krpbDisable = new Oybab.ServicePC.DialogWindow.RButton();
            this.bevelLine1 = new Oybab.ServicePC.Tools.BevelLine();
            this.krpbSourceCode = new Oybab.ServicePC.DialogWindow.RButton();
            this.krplEmail = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            this.krplCompanyName = new ComponentFactory.Krypton.Toolkit.KryptonLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krppImage)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // krppImage
            // 
            this.krppImage.Location = new System.Drawing.Point(28, 12);
            this.krppImage.Name = "krppImage";
            this.krppImage.Size = new System.Drawing.Size(265, 90);
            this.krppImage.TabIndex = 0;
            // 
            // krplSoft
            // 
            this.krplSoft.Location = new System.Drawing.Point(12, 132);
            this.krplSoft.Name = "krplSoft";
            this.krplSoft.Size = new System.Drawing.Size(17, 20);
            this.krplSoft.TabIndex = 1;
            this.krplSoft.Values.Text = "1";
            this.krplSoft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.krplSoft_MouseDown);
            // 
            // krpbDonate
            // 
            this.krpbDonate.Location = new System.Drawing.Point(233, 251);
            this.krpbDonate.Name = "krpbDonate";
            this.krpbDonate.Size = new System.Drawing.Size(60, 25);
            this.krpbDonate.TabIndex = 0;
            this.krpbDonate.Values.Text = "Donate";
            this.krpbDonate.Click += new System.EventHandler(this.krpbDonate_Click);
            // 
            // krplLicense
            // 
            this.krplLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplLicense.Location = new System.Drawing.Point(243, 132);
            this.krplLicense.Name = "krplLicense";
            this.krplLicense.Size = new System.Drawing.Size(50, 20);
            this.krplLicense.TabIndex = 2;
            this.krplLicense.Values.Text = "License";
            this.krplLicense.LinkClicked += new System.EventHandler(this.krplLicense_LinkClicked);
            // 
            // krplVersionNo
            // 
            this.krplVersionNo.Location = new System.Drawing.Point(12, 158);
            this.krplVersionNo.Name = "krplVersionNo";
            this.krplVersionNo.Size = new System.Drawing.Size(17, 20);
            this.krplVersionNo.TabIndex = 1;
            this.krplVersionNo.Values.Text = "1";
            // 
            // krplVersion
            // 
            this.krplVersion.Location = new System.Drawing.Point(57, 158);
            this.krplVersion.Name = "krplVersion";
            this.krplVersion.Size = new System.Drawing.Size(17, 20);
            this.krplVersion.TabIndex = 1;
            this.krplVersion.Values.Text = "1";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.krplRemainingTime);
            this.flowLayoutPanel1.Controls.Add(this.krplRemainingTimeValue);
            this.flowLayoutPanel1.Controls.Add(this.krplDeviceCount);
            this.flowLayoutPanel1.Controls.Add(this.krplDeviceCountValue);
            this.flowLayoutPanel1.Controls.Add(this.krplTableCount);
            this.flowLayoutPanel1.Controls.Add(this.krplTableCountValue);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(11, 185);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(311, 25);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // krplRemainingTime
            // 
            this.krplRemainingTime.Location = new System.Drawing.Point(0, 3);
            this.krplRemainingTime.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.krplRemainingTime.Name = "krplRemainingTime";
            this.krplRemainingTime.Size = new System.Drawing.Size(14, 20);
            this.krplRemainingTime.StateCommon.Padding = new System.Windows.Forms.Padding(-1, -1, 0, -1);
            this.krplRemainingTime.TabIndex = 1;
            this.krplRemainingTime.Values.Text = "0";
            // 
            // krplRemainingTimeValue
            // 
            this.krplRemainingTimeValue.Location = new System.Drawing.Point(14, 3);
            this.krplRemainingTimeValue.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.krplRemainingTimeValue.Name = "krplRemainingTimeValue";
            this.krplRemainingTimeValue.Size = new System.Drawing.Size(11, 20);
            this.krplRemainingTimeValue.StateCommon.Padding = new System.Windows.Forms.Padding(0, -1, 0, -1);
            this.krplRemainingTimeValue.TabIndex = 1;
            this.krplRemainingTimeValue.Values.Text = "0";
            // 
            // krplDeviceCount
            // 
            this.krplDeviceCount.Location = new System.Drawing.Point(25, 3);
            this.krplDeviceCount.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.krplDeviceCount.Name = "krplDeviceCount";
            this.krplDeviceCount.Size = new System.Drawing.Size(11, 20);
            this.krplDeviceCount.StateCommon.Padding = new System.Windows.Forms.Padding(0, -1, 0, -1);
            this.krplDeviceCount.TabIndex = 1;
            this.krplDeviceCount.Values.Text = "0";
            // 
            // krplDeviceCountValue
            // 
            this.krplDeviceCountValue.Location = new System.Drawing.Point(36, 3);
            this.krplDeviceCountValue.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.krplDeviceCountValue.Name = "krplDeviceCountValue";
            this.krplDeviceCountValue.Size = new System.Drawing.Size(11, 20);
            this.krplDeviceCountValue.StateCommon.Padding = new System.Windows.Forms.Padding(0, -1, 0, -1);
            this.krplDeviceCountValue.TabIndex = 1;
            this.krplDeviceCountValue.Values.Text = "0";
            // 
            // krplTableCount
            // 
            this.krplTableCount.Location = new System.Drawing.Point(47, 3);
            this.krplTableCount.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.krplTableCount.Name = "krplTableCount";
            this.krplTableCount.Size = new System.Drawing.Size(11, 20);
            this.krplTableCount.StateCommon.Padding = new System.Windows.Forms.Padding(0, -1, 0, -1);
            this.krplTableCount.TabIndex = 1;
            this.krplTableCount.Values.Text = "0";
            // 
            // krplTableCountValue
            // 
            this.krplTableCountValue.Location = new System.Drawing.Point(58, 3);
            this.krplTableCountValue.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.krplTableCountValue.Name = "krplTableCountValue";
            this.krplTableCountValue.Size = new System.Drawing.Size(11, 20);
            this.krplTableCountValue.StateCommon.Padding = new System.Windows.Forms.Padding(0, -1, 0, -1);
            this.krplTableCountValue.TabIndex = 1;
            this.krplTableCountValue.Values.Text = "0";
            // 
            // krpbDisable
            // 
            this.krpbDisable.Location = new System.Drawing.Point(36, 251);
            this.krpbDisable.Name = "krpbDisable";
            this.krpbDisable.Size = new System.Drawing.Size(91, 25);
            this.krpbDisable.TabIndex = 1;
            this.krpbDisable.Values.Text = "Disable";
            this.krpbDisable.Visible = false;
            this.krpbDisable.Click += new System.EventHandler(this.krpbDisable_Click);
            // 
            // bevelLine1
            // 
            this.bevelLine1.Angle = 90;
            this.bevelLine1.Location = new System.Drawing.Point(6, 116);
            this.bevelLine1.Name = "bevelLine1";
            this.bevelLine1.Size = new System.Drawing.Size(305, 2);
            this.bevelLine1.TabIndex = 3;
            // 
            // krpbSourceCode
            // 
            this.krpbSourceCode.Location = new System.Drawing.Point(133, 251);
            this.krpbSourceCode.Name = "krpbSourceCode";
            this.krpbSourceCode.Size = new System.Drawing.Size(84, 25);
            this.krpbSourceCode.TabIndex = 1;
            this.krpbSourceCode.Values.Text = "Source Code";
            this.krpbSourceCode.Click += new System.EventHandler(this.krpbHelp_Click);
            // 
            // krplEmail
            // 
            this.krplEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplEmail.Location = new System.Drawing.Point(194, 216);
            this.krplEmail.Name = "krplEmail";
            this.krplEmail.Size = new System.Drawing.Size(114, 20);
            this.krplEmail.TabIndex = 2;
            this.krplEmail.Values.Text = "service@oybab.net";
            this.krplEmail.LinkClicked += new System.EventHandler(this.krplUrl_LinkClicked);
            // 
            // krplCompanyName
            // 
            this.krplCompanyName.Location = new System.Drawing.Point(13, 218);
            this.krplCompanyName.Name = "krplCompanyName";
            this.krplCompanyName.Size = new System.Drawing.Size(94, 20);
            this.krplCompanyName.TabIndex = 2;
            this.krplCompanyName.Values.Text = "Oybab Co., Ltd.";
            this.krplCompanyName.LinkClicked += new System.EventHandler(this.krplCompanyWebsite_LinkClicked);
            // 
            // AboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(320, 287);
            this.Controls.Add(this.krpbDisable);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.krplCompanyName);
            this.Controls.Add(this.krplEmail);
            this.Controls.Add(this.krplLicense);
            this.Controls.Add(this.bevelLine1);
            this.Controls.Add(this.krpbSourceCode);
            this.Controls.Add(this.krpbDonate);
            this.Controls.Add(this.krplVersion);
            this.Controls.Add(this.krplVersionNo);
            this.Controls.Add(this.krplSoft);
            this.Controls.Add(this.krppImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krppImage)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel krppImage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSoft;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbDonate;
        private Oybab.ServicePC.Tools.BevelLine bevelLine1;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel krplLicense;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplVersionNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplVersion;
        private RButton krpbSourceCode;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemainingTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemainingTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplDeviceCount;
        private RLabel krplDeviceCountValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTableCount;
        private RLabel krplTableCountValue;
        private RButton krpbDisable;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel krplEmail;
        private ComponentFactory.Krypton.Toolkit.KryptonLinkLabel krplCompanyName;
    }
}