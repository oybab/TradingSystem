namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class GlobalSettingWindow
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
            this.krpbChange = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krptPriceSymbol = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplPriceSymbol = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpCompanyName = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpt1 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpt2 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpl1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpl2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpt0 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpl0 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpSystemLanguage = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpcLanguage2 = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcLanguage1 = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcLanguage0 = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplsecond = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplthird = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplMain = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krpCompanyName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpCompanyName.Panel)).BeginInit();
            this.krpCompanyName.Panel.SuspendLayout();
            this.krpCompanyName.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystemLanguage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystemLanguage.Panel)).BeginInit();
            this.krpSystemLanguage.Panel.SuspendLayout();
            this.krpSystemLanguage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage0)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbChange
            // 
            this.krpbChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpbChange.Location = new System.Drawing.Point(227, 455);
            this.krpbChange.Name = "krpbChange";
            this.krpbChange.Size = new System.Drawing.Size(90, 25);
            this.krpbChange.TabIndex = 4;
            this.krpbChange.Values.Text = "Change";
            this.krpbChange.Click += new System.EventHandler(this.krpbChange_Click);
            // 
            // krptPriceSymbol
            // 
            this.krptPriceSymbol.Location = new System.Drawing.Point(287, 405);
            this.krptPriceSymbol.MaxLength = 30;
            this.krptPriceSymbol.Name = "krptPriceSymbol";
            this.krptPriceSymbol.Size = new System.Drawing.Size(215, 23);
            this.krptPriceSymbol.TabIndex = 10;
            this.krptPriceSymbol.Text = "￥";
            // 
            // krplPriceSymbol
            // 
            this.krplPriceSymbol.Location = new System.Drawing.Point(51, 405);
            this.krplPriceSymbol.Name = "krplPriceSymbol";
            this.krplPriceSymbol.Size = new System.Drawing.Size(81, 20);
            this.krplPriceSymbol.TabIndex = 9;
            this.krplPriceSymbol.Values.Text = "Price Symbol";
            // 
            // krpCompanyName
            // 
            this.krpCompanyName.Location = new System.Drawing.Point(12, 195);
            this.krpCompanyName.Name = "krpCompanyName";
            // 
            // krpCompanyName.Panel
            // 
            this.krpCompanyName.Panel.Controls.Add(this.krpt1);
            this.krpCompanyName.Panel.Controls.Add(this.label1);
            this.krpCompanyName.Panel.Controls.Add(this.krpt2);
            this.krpCompanyName.Panel.Controls.Add(this.label3);
            this.krpCompanyName.Panel.Controls.Add(this.krpl1);
            this.krpCompanyName.Panel.Controls.Add(this.krpl2);
            this.krpCompanyName.Panel.Controls.Add(this.krpt0);
            this.krpCompanyName.Panel.Controls.Add(this.label2);
            this.krpCompanyName.Panel.Controls.Add(this.krpl0);
            this.krpCompanyName.Size = new System.Drawing.Size(527, 175);
            this.krpCompanyName.TabIndex = 18;
            this.krpCompanyName.Text = "Company Name";
            this.krpCompanyName.Values.Heading = "Company Name";
            // 
            // krpt1
            // 
            this.krpt1.Location = new System.Drawing.Point(151, 61);
            this.krpt1.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.krpt1.MaxLength = 3000;
            this.krpt1.Name = "krpt1";
            this.krpt1.Size = new System.Drawing.Size(323, 23);
            this.krpt1.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(126, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 20);
            this.label1.TabIndex = 0;
            this.label1.Values.Text = ":";
            // 
            // krpt2
            // 
            this.krpt2.Location = new System.Drawing.Point(151, 103);
            this.krpt2.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.krpt2.MaxLength = 3000;
            this.krpt2.Name = "krpt2";
            this.krpt2.Size = new System.Drawing.Size(322, 23);
            this.krpt2.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(126, 106);
            this.label3.Margin = new System.Windows.Forms.Padding(9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 20);
            this.label3.TabIndex = 19;
            this.label3.Values.Text = ":";
            // 
            // krpl1
            // 
            this.krpl1.Location = new System.Drawing.Point(37, 62);
            this.krpl1.Margin = new System.Windows.Forms.Padding(9);
            this.krpl1.Name = "krpl1";
            this.krpl1.Size = new System.Drawing.Size(71, 20);
            this.krpl1.TabIndex = 0;
            this.krpl1.Values.Text = "Language1";
            // 
            // krpl2
            // 
            this.krpl2.Location = new System.Drawing.Point(37, 106);
            this.krpl2.Margin = new System.Windows.Forms.Padding(9);
            this.krpl2.Name = "krpl2";
            this.krpl2.Size = new System.Drawing.Size(71, 20);
            this.krpl2.TabIndex = 17;
            this.krpl2.Values.Text = "Language2";
            // 
            // krpt0
            // 
            this.krpt0.Location = new System.Drawing.Point(151, 20);
            this.krpt0.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.krpt0.MaxLength = 3000;
            this.krpt0.Name = "krpt0";
            this.krpt0.Size = new System.Drawing.Size(322, 23);
            this.krpt0.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(124, 20);
            this.label2.Margin = new System.Windows.Forms.Padding(9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 20);
            this.label2.TabIndex = 16;
            this.label2.Values.Text = ":";
            // 
            // krpl0
            // 
            this.krpl0.Location = new System.Drawing.Point(37, 20);
            this.krpl0.Margin = new System.Windows.Forms.Padding(9);
            this.krpl0.Name = "krpl0";
            this.krpl0.Size = new System.Drawing.Size(71, 20);
            this.krpl0.TabIndex = 14;
            this.krpl0.Values.Text = "Language0";
            // 
            // krpSystemLanguage
            // 
            this.krpSystemLanguage.Location = new System.Drawing.Point(17, 14);
            this.krpSystemLanguage.Name = "krpSystemLanguage";
            // 
            // krpSystemLanguage.Panel
            // 
            this.krpSystemLanguage.Panel.Controls.Add(this.krpcLanguage2);
            this.krpSystemLanguage.Panel.Controls.Add(this.krpcLanguage1);
            this.krpSystemLanguage.Panel.Controls.Add(this.krpcLanguage0);
            this.krpSystemLanguage.Panel.Controls.Add(this.kryptonLabel1);
            this.krpSystemLanguage.Panel.Controls.Add(this.kryptonLabel2);
            this.krpSystemLanguage.Panel.Controls.Add(this.krplsecond);
            this.krpSystemLanguage.Panel.Controls.Add(this.krplthird);
            this.krpSystemLanguage.Panel.Controls.Add(this.kryptonLabel5);
            this.krpSystemLanguage.Panel.Controls.Add(this.krplMain);
            this.krpSystemLanguage.Size = new System.Drawing.Size(527, 168);
            this.krpSystemLanguage.TabIndex = 18;
            this.krpSystemLanguage.Text = "System Language";
            this.krpSystemLanguage.Values.Heading = "System Language";
            // 
            // krpcLanguage2
            // 
            this.krpcLanguage2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage2.DropDownWidth = 121;
            this.krpcLanguage2.Location = new System.Drawing.Point(151, 104);
            this.krpcLanguage2.Name = "krpcLanguage2";
            this.krpcLanguage2.Size = new System.Drawing.Size(323, 21);
            this.krpcLanguage2.TabIndex = 20;
            this.krpcLanguage2.SelectedIndexChanged += new System.EventHandler(this.krpcLanguage0_SelectedIndexChanged);
            // 
            // krpcLanguage1
            // 
            this.krpcLanguage1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage1.DropDownWidth = 121;
            this.krpcLanguage1.Location = new System.Drawing.Point(151, 61);
            this.krpcLanguage1.Name = "krpcLanguage1";
            this.krpcLanguage1.Size = new System.Drawing.Size(323, 21);
            this.krpcLanguage1.TabIndex = 20;
            this.krpcLanguage1.SelectedIndexChanged += new System.EventHandler(this.krpcLanguage0_SelectedIndexChanged);
            // 
            // krpcLanguage0
            // 
            this.krpcLanguage0.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage0.DropDownWidth = 121;
            this.krpcLanguage0.Location = new System.Drawing.Point(151, 19);
            this.krpcLanguage0.Name = "krpcLanguage0";
            this.krpcLanguage0.Size = new System.Drawing.Size(323, 21);
            this.krpcLanguage0.TabIndex = 20;
            this.krpcLanguage0.SelectedIndexChanged += new System.EventHandler(this.krpcLanguage0_SelectedIndexChanged);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(126, 62);
            this.kryptonLabel1.Margin = new System.Windows.Forms.Padding(9);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(126, 104);
            this.kryptonLabel2.Margin = new System.Windows.Forms.Padding(9);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 19;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // krplsecond
            // 
            this.krplsecond.Location = new System.Drawing.Point(37, 62);
            this.krplsecond.Margin = new System.Windows.Forms.Padding(9);
            this.krplsecond.Name = "krplsecond";
            this.krplsecond.Size = new System.Drawing.Size(17, 20);
            this.krplsecond.TabIndex = 0;
            this.krplsecond.Values.Text = "2";
            // 
            // krplthird
            // 
            this.krplthird.Location = new System.Drawing.Point(37, 104);
            this.krplthird.Margin = new System.Windows.Forms.Padding(9);
            this.krplthird.Name = "krplthird";
            this.krplthird.Size = new System.Drawing.Size(17, 20);
            this.krplthird.TabIndex = 17;
            this.krplthird.Values.Text = "3";
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(124, 20);
            this.kryptonLabel5.Margin = new System.Windows.Forms.Padding(9);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel5.TabIndex = 16;
            this.kryptonLabel5.Values.Text = ":";
            // 
            // krplMain
            // 
            this.krplMain.Location = new System.Drawing.Point(37, 20);
            this.krplMain.Margin = new System.Windows.Forms.Padding(9);
            this.krplMain.Name = "krplMain";
            this.krplMain.Size = new System.Drawing.Size(38, 20);
            this.krplMain.TabIndex = 14;
            this.krplMain.Values.Text = "Main";
            // 
            // GlobalSettingWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(551, 492);
            this.Controls.Add(this.krpSystemLanguage);
            this.Controls.Add(this.krpCompanyName);
            this.Controls.Add(this.krptPriceSymbol);
            this.Controls.Add(this.krplPriceSymbol);
            this.Controls.Add(this.krpbChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlobalSettingWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "GlobalSettingWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpCompanyName.Panel)).EndInit();
            this.krpCompanyName.Panel.ResumeLayout(false);
            this.krpCompanyName.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpCompanyName)).EndInit();
            this.krpCompanyName.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpSystemLanguage.Panel)).EndInit();
            this.krpSystemLanguage.Panel.ResumeLayout(false);
            this.krpSystemLanguage.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystemLanguage)).EndInit();
            this.krpSystemLanguage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage0)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChange;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptPriceSymbol;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPriceSymbol;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpCompanyName;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krpt1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krpt2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krpl1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krpl2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krpt0;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krpl0;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpSystemLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplsecond;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplthird;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMain;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage2;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage1;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage0;
    }
}