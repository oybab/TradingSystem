namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class BarcodeScales
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
            this.krpbSend = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplIPAddress = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptIPAddress = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krpcLanguage = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplLanguage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbSend
            // 
            this.krpbSend.Location = new System.Drawing.Point(127, 110);
            this.krpbSend.Name = "krpbSend";
            this.krpbSend.Size = new System.Drawing.Size(120, 25);
            this.krpbSend.TabIndex = 4;
            this.krpbSend.Values.Text = "Send Data";
            this.krpbSend.Click += new System.EventHandler(this.krpbAdd_Click);
            // 
            // krplIPAddress
            // 
            this.krplIPAddress.Location = new System.Drawing.Point(25, 58);
            this.krplIPAddress.Name = "krplIPAddress";
            this.krplIPAddress.Size = new System.Drawing.Size(68, 20);
            this.krplIPAddress.TabIndex = 1;
            this.krplIPAddress.Values.Text = "IP Address";
            // 
            // krptIPAddress
            // 
            this.krptIPAddress.Location = new System.Drawing.Point(142, 58);
            this.krptIPAddress.MaxLength = 20;
            this.krptIPAddress.Name = "krptIPAddress";
            this.krptIPAddress.Size = new System.Drawing.Size(203, 23);
            this.krptIPAddress.TabIndex = 2;
            this.krptIPAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptMemberNo_KeyDown);
            // 
            // krpcLanguage
            // 
            this.krpcLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage.DropDownWidth = 121;
            this.krpcLanguage.Location = new System.Drawing.Point(142, 17);
            this.krpcLanguage.Name = "krpcLanguage";
            this.krpcLanguage.Size = new System.Drawing.Size(121, 21);
            this.krpcLanguage.TabIndex = 1;
            // 
            // krplLanguage
            // 
            this.krplLanguage.Location = new System.Drawing.Point(29, 17);
            this.krplLanguage.Name = "krplLanguage";
            this.krplLanguage.Size = new System.Drawing.Size(64, 20);
            this.krplLanguage.TabIndex = 7;
            this.krplLanguage.Values.Text = "Language";
            // 
            // BarcodeScales
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(373, 147);
            this.Controls.Add(this.krpcLanguage);
            this.Controls.Add(this.krplLanguage);
            this.Controls.Add(this.krptIPAddress);
            this.Controls.Add(this.krplIPAddress);
            this.Controls.Add(this.krpbSend);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BarcodeScales";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BarcodeScalesWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSend;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplIPAddress;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptIPAddress;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplLanguage;
    }
}