namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class RegCountWindow
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
            this.krpbReg = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplRequestCode = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRegCode = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptRegCode = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplRequestCodeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbHelp = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // krpbReg
            // 
            this.krpbReg.Location = new System.Drawing.Point(76, 106);
            this.krpbReg.Name = "krpbReg";
            this.krpbReg.Size = new System.Drawing.Size(90, 25);
            this.krpbReg.TabIndex = 4;
            this.krpbReg.Values.Text = "Reg";
            this.krpbReg.Click += new System.EventHandler(this.krpbReg_Click);
            // 
            // krpbClose
            // 
            this.krpbClose.Location = new System.Drawing.Point(172, 106);
            this.krpbClose.Name = "krpbClose";
            this.krpbClose.Size = new System.Drawing.Size(90, 25);
            this.krpbClose.TabIndex = 5;
            this.krpbClose.Values.Text = "Close";
            this.krpbClose.Click += new System.EventHandler(this.krpbClose_Click);
            // 
            // krplRequestCode
            // 
            this.krplRequestCode.Location = new System.Drawing.Point(31, 24);
            this.krplRequestCode.Name = "krplRequestCode";
            this.krplRequestCode.Size = new System.Drawing.Size(74, 20);
            this.krplRequestCode.TabIndex = 1;
            this.krplRequestCode.Values.Text = "Request No";
            // 
            // krplRegCode
            // 
            this.krplRegCode.Location = new System.Drawing.Point(31, 65);
            this.krplRegCode.Name = "krplRegCode";
            this.krplRegCode.Size = new System.Drawing.Size(75, 20);
            this.krplRegCode.TabIndex = 1;
            this.krplRegCode.Values.Text = "Register No";
            // 
            // krptRegCode
            // 
            this.krptRegCode.Location = new System.Drawing.Point(139, 65);
            this.krptRegCode.MaxLength = 64;
            this.krptRegCode.Name = "krptRegCode";
            this.krptRegCode.Size = new System.Drawing.Size(261, 20);
            this.krptRegCode.TabIndex = 2;
            this.krptRegCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptRegCode_KeyDown);
            // 
            // krplRequestCodeValue
            // 
            this.krplRequestCodeValue.Location = new System.Drawing.Point(139, 24);
            this.krplRequestCodeValue.Name = "krplRequestCodeValue";
            this.krplRequestCodeValue.Size = new System.Drawing.Size(50, 20);
            this.krplRequestCodeValue.TabIndex = 1;
            this.krplRequestCodeValue.Values.Text = "000000";
            // 
            // krpbHelp
            // 
            this.krpbHelp.Location = new System.Drawing.Point(268, 106);
            this.krpbHelp.Name = "krpbHelp";
            this.krpbHelp.Size = new System.Drawing.Size(90, 25);
            this.krpbHelp.TabIndex = 6;
            this.krpbHelp.Values.Text = "Help";
            this.krpbHelp.Click += new System.EventHandler(this.krpbHelp_Click);
            // 
            // RegCountWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(432, 143);
            this.Controls.Add(this.krptRegCode);
            this.Controls.Add(this.krplRegCode);
            this.Controls.Add(this.krplRequestCodeValue);
            this.Controls.Add(this.krplRequestCode);
            this.Controls.Add(this.krpbHelp);
            this.Controls.Add(this.krpbClose);
            this.Controls.Add(this.krpbReg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RegCountWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RegWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbReg;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbClose;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRequestCode;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRegCode;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptRegCode;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRequestCodeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbHelp;
    }
}