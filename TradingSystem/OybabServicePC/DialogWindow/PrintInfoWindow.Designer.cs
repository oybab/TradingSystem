namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PrintInfoWindow
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
            this.flOther = new System.Windows.Forms.FlowLayoutPanel();
            this.fl0 = new System.Windows.Forms.FlowLayoutPanel();
            this.krpt0 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpl0 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.fl1 = new System.Windows.Forms.FlowLayoutPanel();
            this.krpt1 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpl1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.fl2 = new System.Windows.Forms.FlowLayoutPanel();
            this.krpt2 = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.label3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpl2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptPhoneNo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.KrplOrderPhoneNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbMultipleLanguage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krplPrintMessage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpOther = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krplPageHeight = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcPageHeight = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplIsPrintAfterCheckout = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcIsPrintAfterCheckout = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krplIsPrintAfterBuy = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcIsPrintAfterBuy = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.lbPageHeightDescription = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flOther.SuspendLayout();
            this.fl0.SuspendLayout();
            this.fl1.SuspendLayout();
            this.fl2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpOther)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpOther.Panel)).BeginInit();
            this.krpOther.Panel.SuspendLayout();
            this.krpOther.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpcPageHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbChange
            // 
            this.krpbChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpbChange.Location = new System.Drawing.Point(227, 524);
            this.krpbChange.Name = "krpbChange";
            this.krpbChange.Size = new System.Drawing.Size(90, 25);
            this.krpbChange.TabIndex = 4;
            this.krpbChange.Values.Text = "Change";
            this.krpbChange.Click += new System.EventHandler(this.krpbChange_Click);
            // 
            // flOther
            // 
            this.flOther.BackColor = System.Drawing.Color.Transparent;
            this.flOther.Controls.Add(this.fl0);
            this.flOther.Controls.Add(this.fl1);
            this.flOther.Controls.Add(this.fl2);
            this.flOther.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flOther.Location = new System.Drawing.Point(3, 49);
            this.flOther.Name = "flOther";
            this.flOther.Size = new System.Drawing.Size(489, 267);
            this.flOther.TabIndex = 5;
            // 
            // fl0
            // 
            this.fl0.BackColor = System.Drawing.Color.Transparent;
            this.fl0.Controls.Add(this.krpt0);
            this.fl0.Controls.Add(this.label2);
            this.fl0.Controls.Add(this.krpl0);
            this.fl0.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.fl0.Location = new System.Drawing.Point(3, 3);
            this.fl0.Name = "fl0";
            this.fl0.Size = new System.Drawing.Size(486, 80);
            this.fl0.TabIndex = 0;
            // 
            // krpt0
            // 
            this.krpt0.Location = new System.Drawing.Point(134, 3);
            this.krpt0.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.krpt0.MaxLength = 3000;
            this.krpt0.Multiline = true;
            this.krpt0.Name = "krpt0";
            this.krpt0.Size = new System.Drawing.Size(322, 75);
            this.krpt0.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(109, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(13, 20);
            this.label2.TabIndex = 13;
            this.label2.Values.Text = ":";
            // 
            // krpl0
            // 
            this.krpl0.Location = new System.Drawing.Point(20, 9);
            this.krpl0.Margin = new System.Windows.Forms.Padding(9);
            this.krpl0.Name = "krpl0";
            this.krpl0.Size = new System.Drawing.Size(71, 20);
            this.krpl0.TabIndex = 11;
            this.krpl0.Values.Text = "Language0";
            // 
            // fl1
            // 
            this.fl1.BackColor = System.Drawing.Color.Transparent;
            this.fl1.Controls.Add(this.krpt1);
            this.fl1.Controls.Add(this.label1);
            this.fl1.Controls.Add(this.krpl1);
            this.fl1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.fl1.Location = new System.Drawing.Point(3, 89);
            this.fl1.Name = "fl1";
            this.fl1.Size = new System.Drawing.Size(486, 80);
            this.fl1.TabIndex = 0;
            // 
            // krpt1
            // 
            this.krpt1.Location = new System.Drawing.Point(133, 3);
            this.krpt1.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.krpt1.MaxLength = 3000;
            this.krpt1.Multiline = true;
            this.krpt1.Name = "krpt1";
            this.krpt1.Size = new System.Drawing.Size(323, 75);
            this.krpt1.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(108, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(13, 20);
            this.label1.TabIndex = 0;
            this.label1.Values.Text = ":";
            // 
            // krpl1
            // 
            this.krpl1.Location = new System.Drawing.Point(19, 9);
            this.krpl1.Margin = new System.Windows.Forms.Padding(9);
            this.krpl1.Name = "krpl1";
            this.krpl1.Size = new System.Drawing.Size(71, 20);
            this.krpl1.TabIndex = 0;
            this.krpl1.Values.Text = "Language1";
            // 
            // fl2
            // 
            this.fl2.BackColor = System.Drawing.Color.Transparent;
            this.fl2.Controls.Add(this.krpt2);
            this.fl2.Controls.Add(this.label3);
            this.fl2.Controls.Add(this.krpl2);
            this.fl2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.fl2.Location = new System.Drawing.Point(3, 175);
            this.fl2.Name = "fl2";
            this.fl2.Size = new System.Drawing.Size(486, 80);
            this.fl2.TabIndex = 0;
            // 
            // krpt2
            // 
            this.krpt2.Location = new System.Drawing.Point(134, 3);
            this.krpt2.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.krpt2.MaxLength = 3000;
            this.krpt2.Multiline = true;
            this.krpt2.Name = "krpt2";
            this.krpt2.Size = new System.Drawing.Size(322, 75);
            this.krpt2.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(109, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(13, 20);
            this.label3.TabIndex = 13;
            this.label3.Values.Text = ":";
            // 
            // krpl2
            // 
            this.krpl2.Location = new System.Drawing.Point(20, 9);
            this.krpl2.Margin = new System.Windows.Forms.Padding(9);
            this.krpl2.Name = "krpl2";
            this.krpl2.Size = new System.Drawing.Size(71, 20);
            this.krpl2.TabIndex = 11;
            this.krpl2.Values.Text = "Language2";
            // 
            // krptPhoneNo
            // 
            this.krptPhoneNo.Location = new System.Drawing.Point(282, 12);
            this.krptPhoneNo.MaxLength = 30;
            this.krptPhoneNo.Name = "krptPhoneNo";
            this.krptPhoneNo.Size = new System.Drawing.Size(215, 23);
            this.krptPhoneNo.TabIndex = 10;
            this.krptPhoneNo.Text = "0903-111222333";
            // 
            // KrplOrderPhoneNo
            // 
            this.KrplOrderPhoneNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.KrplOrderPhoneNo.Location = new System.Drawing.Point(66, 12);
            this.KrplOrderPhoneNo.Name = "KrplOrderPhoneNo";
            this.KrplOrderPhoneNo.Size = new System.Drawing.Size(100, 20);
            this.KrplOrderPhoneNo.TabIndex = 9;
            this.KrplOrderPhoneNo.Values.Text = "Order Phone No";
            // 
            // krpcbMultipleLanguage
            // 
            this.krpcbMultipleLanguage.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbMultipleLanguage.Location = new System.Drawing.Point(299, 15);
            this.krpcbMultipleLanguage.Name = "krpcbMultipleLanguage";
            this.krpcbMultipleLanguage.Size = new System.Drawing.Size(122, 20);
            this.krpcbMultipleLanguage.TabIndex = 11;
            this.krpcbMultipleLanguage.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.Values.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.CheckedChanged += new System.EventHandler(this.krpcbMultipleLanguage_CheckedChanged);
            // 
            // krplPrintMessage
            // 
            this.krplPrintMessage.Location = new System.Drawing.Point(63, 15);
            this.krplPrintMessage.Name = "krplPrintMessage";
            this.krplPrintMessage.Size = new System.Drawing.Size(89, 20);
            this.krplPrintMessage.TabIndex = 9;
            this.krplPrintMessage.Values.Text = "PrintMessages";
            // 
            // krpOther
            // 
            this.krpOther.Location = new System.Drawing.Point(12, 157);
            this.krpOther.Name = "krpOther";
            // 
            // krpOther.Panel
            // 
            this.krpOther.Panel.Controls.Add(this.flOther);
            this.krpOther.Panel.Controls.Add(this.krplPrintMessage);
            this.krpOther.Panel.Controls.Add(this.krpcbMultipleLanguage);
            this.krpOther.Size = new System.Drawing.Size(527, 352);
            this.krpOther.TabIndex = 18;
            this.krpOther.Text = "Other";
            this.krpOther.Values.Heading = "Other";
            // 
            // krplPageHeight
            // 
            this.krplPageHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageHeight.Location = new System.Drawing.Point(88, 126);
            this.krplPageHeight.Name = "krplPageHeight";
            this.krplPageHeight.Size = new System.Drawing.Size(78, 20);
            this.krplPageHeight.TabIndex = 20;
            this.krplPageHeight.Values.Text = "Page Height";
            // 
            // krpcPageHeight
            // 
            this.krpcPageHeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcPageHeight.DropDownWidth = 121;
            this.krpcPageHeight.Items.AddRange(new object[] {
            "93",
            "140",
            "280"});
            this.krpcPageHeight.Location = new System.Drawing.Point(282, 126);
            this.krpcPageHeight.Name = "krpcPageHeight";
            this.krpcPageHeight.Size = new System.Drawing.Size(121, 21);
            this.krpcPageHeight.TabIndex = 21;
            // 
            // krplIsPrintAfterCheckout
            // 
            this.krplIsPrintAfterCheckout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplIsPrintAfterCheckout.Location = new System.Drawing.Point(45, 86);
            this.krplIsPrintAfterCheckout.Name = "krplIsPrintAfterCheckout";
            this.krplIsPrintAfterCheckout.Size = new System.Drawing.Size(121, 20);
            this.krplIsPrintAfterCheckout.TabIndex = 40;
            this.krplIsPrintAfterCheckout.Values.Text = "Print After Checkout";
            // 
            // krpcIsPrintAfterCheckout
            // 
            this.krpcIsPrintAfterCheckout.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcIsPrintAfterCheckout.Location = new System.Drawing.Point(282, 86);
            this.krpcIsPrintAfterCheckout.Name = "krpcIsPrintAfterCheckout";
            this.krpcIsPrintAfterCheckout.Size = new System.Drawing.Size(42, 20);
            this.krpcIsPrintAfterCheckout.TabIndex = 43;
            this.krpcIsPrintAfterCheckout.Text = "Yes";
            this.krpcIsPrintAfterCheckout.Values.Text = "Yes";
            // 
            // krplIsPrintAfterBuy
            // 
            this.krplIsPrintAfterBuy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplIsPrintAfterBuy.Location = new System.Drawing.Point(76, 51);
            this.krplIsPrintAfterBuy.Name = "krplIsPrintAfterBuy";
            this.krplIsPrintAfterBuy.Size = new System.Drawing.Size(90, 20);
            this.krplIsPrintAfterBuy.TabIndex = 41;
            this.krplIsPrintAfterBuy.Values.Text = "Print After Buy";
            // 
            // krpcIsPrintAfterBuy
            // 
            this.krpcIsPrintAfterBuy.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcIsPrintAfterBuy.Location = new System.Drawing.Point(282, 51);
            this.krpcIsPrintAfterBuy.Name = "krpcIsPrintAfterBuy";
            this.krpcIsPrintAfterBuy.Size = new System.Drawing.Size(42, 20);
            this.krpcIsPrintAfterBuy.TabIndex = 42;
            this.krpcIsPrintAfterBuy.Text = "Yes";
            this.krpcIsPrintAfterBuy.Values.Text = "Yes";
            // 
            // lbPageHeightDescription
            // 
            this.lbPageHeightDescription.Location = new System.Drawing.Point(409, 127);
            this.lbPageHeightDescription.Name = "lbPageHeightDescription";
            this.lbPageHeightDescription.Size = new System.Drawing.Size(32, 20);
            this.lbPageHeightDescription.TabIndex = 20;
            this.lbPageHeightDescription.Values.Text = "mm";
            // 
            // PrintInfoWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(551, 561);
            this.Controls.Add(this.krplIsPrintAfterCheckout);
            this.Controls.Add(this.krpcIsPrintAfterCheckout);
            this.Controls.Add(this.krplIsPrintAfterBuy);
            this.Controls.Add(this.krpcIsPrintAfterBuy);
            this.Controls.Add(this.lbPageHeightDescription);
            this.Controls.Add(this.krplPageHeight);
            this.Controls.Add(this.krpcPageHeight);
            this.Controls.Add(this.krpOther);
            this.Controls.Add(this.krptPhoneNo);
            this.Controls.Add(this.KrplOrderPhoneNo);
            this.Controls.Add(this.krpbChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintInfoWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PrintInfoWindow";
            this.flOther.ResumeLayout(false);
            this.fl0.ResumeLayout(false);
            this.fl0.PerformLayout();
            this.fl1.ResumeLayout(false);
            this.fl1.PerformLayout();
            this.fl2.ResumeLayout(false);
            this.fl2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpOther.Panel)).EndInit();
            this.krpOther.Panel.ResumeLayout(false);
            this.krpOther.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpOther)).EndInit();
            this.krpOther.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpcPageHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChange;
        private System.Windows.Forms.FlowLayoutPanel flOther;
        private System.Windows.Forms.FlowLayoutPanel fl2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptPhoneNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel KrplOrderPhoneNo;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbMultipleLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPrintMessage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krpl2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krpt2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label3;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpOther;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPageHeight;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcPageHeight;
        private System.Windows.Forms.FlowLayoutPanel fl1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krpt1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krpl1;
        private System.Windows.Forms.FlowLayoutPanel fl0;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krpt0;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel label2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krpl0;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplIsPrintAfterCheckout;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcIsPrintAfterCheckout;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplIsPrintAfterBuy;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcIsPrintAfterBuy;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lbPageHeightDescription;
    }
}