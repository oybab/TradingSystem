namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PasswordWindow
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
            this.krpbChangePassword = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbClose = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplPassword = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewPassword = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplConfirmPassword = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptPassword = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krptNewPassword = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krptConfirmPassword = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.SuspendLayout();
            // 
            // krpbChangePassword
            // 
            this.krpbChangePassword.Location = new System.Drawing.Point(55, 147);
            this.krpbChangePassword.Name = "krpbChangePassword";
            this.krpbChangePassword.Size = new System.Drawing.Size(90, 25);
            this.krpbChangePassword.TabIndex = 4;
            this.krpbChangePassword.Values.Text = "";
            this.krpbChangePassword.Click += new System.EventHandler(this.krpbChangePassword_Click);
            // 
            // krpbClose
            // 
            this.krpbClose.Location = new System.Drawing.Point(233, 147);
            this.krpbClose.Name = "krpbClose";
            this.krpbClose.Size = new System.Drawing.Size(90, 25);
            this.krpbClose.TabIndex = 5;
            this.krpbClose.Values.Text = "";
            this.krpbClose.Click += new System.EventHandler(this.krpbReset_Click);
            // 
            // krplPassword
            // 
            this.krplPassword.Location = new System.Drawing.Point(31, 27);
            this.krplPassword.Name = "krplPassword";
            this.krplPassword.Size = new System.Drawing.Size(17, 20);
            this.krplPassword.TabIndex = 1;
            this.krplPassword.Values.Text = "1";
            // 
            // krplNewPassword
            // 
            this.krplNewPassword.Location = new System.Drawing.Point(31, 65);
            this.krplNewPassword.Name = "krplNewPassword";
            this.krplNewPassword.Size = new System.Drawing.Size(17, 20);
            this.krplNewPassword.TabIndex = 1;
            this.krplNewPassword.Values.Text = "2";
            // 
            // krplConfirmPassword
            // 
            this.krplConfirmPassword.Location = new System.Drawing.Point(31, 102);
            this.krplConfirmPassword.Name = "krplConfirmPassword";
            this.krplConfirmPassword.Size = new System.Drawing.Size(17, 20);
            this.krplConfirmPassword.TabIndex = 1;
            this.krplConfirmPassword.Values.Text = "3";
            // 
            // krptPassword
            // 
            this.krptPassword.Location = new System.Drawing.Point(207, 27);
            this.krptPassword.MaxLength = 32;
            this.krptPassword.Name = "krptPassword";
            this.krptPassword.PasswordChar = '●';
            this.krptPassword.Size = new System.Drawing.Size(159, 23);
            this.krptPassword.TabIndex = 1;
            this.krptPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptConfirmPassword_KeyDown);
            // 
            // krptNewPassword
            // 
            this.krptNewPassword.Location = new System.Drawing.Point(207, 65);
            this.krptNewPassword.MaxLength = 16;
            this.krptNewPassword.Name = "krptNewPassword";
            this.krptNewPassword.PasswordChar = '●';
            this.krptNewPassword.Size = new System.Drawing.Size(159, 23);
            this.krptNewPassword.TabIndex = 2;
            this.krptNewPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptConfirmPassword_KeyDown);
            // 
            // krptConfirmPassword
            // 
            this.krptConfirmPassword.Location = new System.Drawing.Point(207, 102);
            this.krptConfirmPassword.MaxLength = 16;
            this.krptConfirmPassword.Name = "krptConfirmPassword";
            this.krptConfirmPassword.PasswordChar = '●';
            this.krptConfirmPassword.Size = new System.Drawing.Size(159, 23);
            this.krptConfirmPassword.TabIndex = 3;
            this.krptConfirmPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptConfirmPassword_KeyDown);
            // 
            // PasswordWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(401, 184);
            this.Controls.Add(this.krptConfirmPassword);
            this.Controls.Add(this.krptNewPassword);
            this.Controls.Add(this.krptPassword);
            this.Controls.Add(this.krplConfirmPassword);
            this.Controls.Add(this.krplNewPassword);
            this.Controls.Add(this.krplPassword);
            this.Controls.Add(this.krpbClose);
            this.Controls.Add(this.krpbChangePassword);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PasswordWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PasswordWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChangePassword;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbClose;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplConfirmPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptNewPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptConfirmPassword;
    }
}