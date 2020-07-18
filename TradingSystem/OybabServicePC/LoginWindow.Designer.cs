namespace Oybab.ServicePC
{
    partial class LoginWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.krpManagerLogin = new ComponentFactory.Krypton.Toolkit.KryptonManager(this.components);
            this.krplAdminNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPassword = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptPassword = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krpbLogin = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbExit = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krptAdminNo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krpbSetting = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // krplAdminNo
            // 
            this.krplAdminNo.Location = new System.Drawing.Point(31, 35);
            this.krplAdminNo.Name = "krplAdminNo";
            this.krplAdminNo.Size = new System.Drawing.Size(17, 20);
            this.krplAdminNo.TabIndex = 0;
            this.krplAdminNo.Values.Text = "1";
            // 
            // krplPassword
            // 
            this.krplPassword.Location = new System.Drawing.Point(31, 84);
            this.krplPassword.Name = "krplPassword";
            this.krplPassword.Size = new System.Drawing.Size(17, 20);
            this.krplPassword.TabIndex = 0;
            this.krplPassword.Values.Text = "2";
            // 
            // krptPassword
            // 
            this.krptPassword.Location = new System.Drawing.Point(151, 84);
            this.krptPassword.MaxLength = 32;
            this.krptPassword.Name = "krptPassword";
            this.krptPassword.PasswordChar = '●';
            this.krptPassword.Size = new System.Drawing.Size(209, 23);
            this.krptPassword.TabIndex = 1;
            this.krptPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptPassword_KeyDown);
            // 
            // krpbLogin
            // 
            this.krpbLogin.Location = new System.Drawing.Point(87, 138);
            this.krpbLogin.Name = "krpbLogin";
            this.krpbLogin.Size = new System.Drawing.Size(90, 25);
            this.krpbLogin.TabIndex = 2;
            this.krpbLogin.Values.Text = "";
            this.krpbLogin.Click += new System.EventHandler(this.krpbLogin_Click);
            // 
            // krpbExit
            // 
            this.krpbExit.Location = new System.Drawing.Point(225, 138);
            this.krpbExit.Name = "krpbExit";
            this.krpbExit.Size = new System.Drawing.Size(90, 25);
            this.krpbExit.TabIndex = 2;
            this.krpbExit.Values.Text = "";
            this.krpbExit.Click += new System.EventHandler(this.krpbExit_Click);
            // 
            // krptAdminNo
            // 
            this.krptAdminNo.Location = new System.Drawing.Point(151, 35);
            this.krptAdminNo.MaxLength = 32;
            this.krptAdminNo.Name = "krptAdminNo";
            this.krptAdminNo.Size = new System.Drawing.Size(209, 23);
            this.krptAdminNo.TabIndex = 0;
            this.krptAdminNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptPassword_KeyDown);
            // 
            // krpbSetting
            // 
            this.krpbSetting.Location = new System.Drawing.Point(366, 138);
            this.krpbSetting.Name = "krpbSetting";
            this.krpbSetting.Size = new System.Drawing.Size(24, 24);
            this.krpbSetting.TabIndex = 2;
            this.krpbSetting.Values.Text = "";
            this.krpbSetting.Click += new System.EventHandler(this.krpbSetting_Click);
            // 
            // LoginWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(405, 175);
            this.Controls.Add(this.krpbSetting);
            this.Controls.Add(this.krpbExit);
            this.Controls.Add(this.krpbLogin);
            this.Controls.Add(this.krptAdminNo);
            this.Controls.Add(this.krptPassword);
            this.Controls.Add(this.krplPassword);
            this.Controls.Add(this.krplAdminNo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "LoginWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonManager krpManagerLogin;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAdminNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptPassword;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbLogin;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbExit;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptAdminNo;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSetting;
    }
}

