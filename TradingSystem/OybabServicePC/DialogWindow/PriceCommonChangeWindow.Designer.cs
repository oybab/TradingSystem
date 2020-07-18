namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PriceCommonChangeWindow
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
            this.krpbSave = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplChangePrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptChangePrice = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBalancePrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBalancePriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBalancePay = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbBalancePay = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpbAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplMemberName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel10 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flpMember = new System.Windows.Forms.FlowLayoutPanel();
            this.krplMemberNameValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnMemberAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnMemberRemove = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krprbAdd = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            this.krprbSub = new ComponentFactory.Krypton.Toolkit.KryptonRadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbBalancePay)).BeginInit();
            this.flpMember.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpbSave
            // 
            this.krpbSave.Location = new System.Drawing.Point(174, 266);
            this.krpbSave.Name = "krpbSave";
            this.krpbSave.Size = new System.Drawing.Size(90, 25);
            this.krpbSave.TabIndex = 25;
            this.krpbSave.Values.Text = "Save";
            this.krpbSave.Click += new System.EventHandler(this.krpbSave_Click);
            // 
            // krplTotalPrice
            // 
            this.krplTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPrice.Location = new System.Drawing.Point(53, 55);
            this.krplTotalPrice.Name = "krplTotalPrice";
            this.krplTotalPrice.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPrice.TabIndex = 1;
            this.krplTotalPrice.Values.Text = "TotalPrice";
            // 
            // krplPaidPrice
            // 
            this.krplPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPaidPrice.Location = new System.Drawing.Point(57, 93);
            this.krplPaidPrice.Name = "krplPaidPrice";
            this.krplPaidPrice.Size = new System.Drawing.Size(60, 20);
            this.krplPaidPrice.TabIndex = 1;
            this.krplPaidPrice.Values.Text = "PaidPrice";
            // 
            // krplChangePrice
            // 
            this.krplChangePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplChangePrice.Location = new System.Drawing.Point(38, 175);
            this.krplChangePrice.Name = "krplChangePrice";
            this.krplChangePrice.Size = new System.Drawing.Size(79, 20);
            this.krplChangePrice.TabIndex = 1;
            this.krplChangePrice.Values.Text = "ChangePrice";
            this.krplChangePrice.MouseClick += new System.Windows.Forms.MouseEventHandler(this.krplChangePrice_MouseClick);
            // 
            // krptChangePrice
            // 
            this.krptChangePrice.Location = new System.Drawing.Point(261, 175);
            this.krptChangePrice.MaxLength = 12;
            this.krptChangePrice.Name = "krptChangePrice";
            this.krptChangePrice.Size = new System.Drawing.Size(90, 23);
            this.krptChangePrice.TabIndex = 12;
            this.krptChangePrice.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptChangePrice_KeyUp);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(128, 55);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(128, 93);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(128, 175);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 1;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // krplTotalPriceValue
            // 
            this.krplTotalPriceValue.Location = new System.Drawing.Point(174, 55);
            this.krplTotalPriceValue.Name = "krplTotalPriceValue";
            this.krplTotalPriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplTotalPriceValue.TabIndex = 1;
            this.krplTotalPriceValue.Values.Text = "0000000";
            // 
            // krplPaidPriceValue
            // 
            this.krplPaidPriceValue.Location = new System.Drawing.Point(174, 93);
            this.krplPaidPriceValue.Name = "krplPaidPriceValue";
            this.krplPaidPriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplPaidPriceValue.TabIndex = 1;
            this.krplPaidPriceValue.Values.Text = "0000000";
            // 
            // krplBalancePrice
            // 
            this.krplBalancePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBalancePrice.Location = new System.Drawing.Point(38, 220);
            this.krplBalancePrice.Name = "krplBalancePrice";
            this.krplBalancePrice.Size = new System.Drawing.Size(79, 20);
            this.krplBalancePrice.TabIndex = 1;
            this.krplBalancePrice.Values.Text = "BalancePrice";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(128, 220);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel4.TabIndex = 1;
            this.kryptonLabel4.Values.Text = ":";
            // 
            // krplBalancePriceValue
            // 
            this.krplBalancePriceValue.Location = new System.Drawing.Point(174, 220);
            this.krplBalancePriceValue.Name = "krplBalancePriceValue";
            this.krplBalancePriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplBalancePriceValue.TabIndex = 1;
            this.krplBalancePriceValue.Values.Text = "0000000";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(128, 130);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel6.TabIndex = 1;
            this.kryptonLabel6.Values.Text = ":";
            // 
            // krplBalancePay
            // 
            this.krplBalancePay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBalancePay.Location = new System.Drawing.Point(45, 130);
            this.krplBalancePay.Name = "krplBalancePay";
            this.krplBalancePay.Size = new System.Drawing.Size(72, 20);
            this.krplBalancePay.TabIndex = 1;
            this.krplBalancePay.Values.Text = "BalancePay";
            // 
            // krpcbBalancePay
            // 
            this.krpcbBalancePay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpcbBalancePay.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.krpcbBalancePay.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcbBalancePay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbBalancePay.DropDownWidth = 149;
            this.krpcbBalancePay.Location = new System.Drawing.Point(174, 130);
            this.krpcbBalancePay.Name = "krpcbBalancePay";
            this.krpcbBalancePay.Size = new System.Drawing.Size(177, 21);
            this.krpcbBalancePay.TabIndex = 15;
            this.krpcbBalancePay.SelectedIndexChanged += new System.EventHandler(this.krpcbBalancePay_SelectedIndexChanged);
            // 
            // krpbAdd
            // 
            this.krpbAdd.Location = new System.Drawing.Point(62, 266);
            this.krpbAdd.Name = "krpbAdd";
            this.krpbAdd.Size = new System.Drawing.Size(63, 25);
            this.krpbAdd.TabIndex = 20;
            this.krpbAdd.Values.Text = "Add";
            this.krpbAdd.Click += new System.EventHandler(this.krpbAdd_Click);
            // 
            // krplMemberName
            // 
            this.krplMemberName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplMemberName.Location = new System.Drawing.Point(28, 16);
            this.krplMemberName.Name = "krplMemberName";
            this.krplMemberName.Size = new System.Drawing.Size(89, 20);
            this.krplMemberName.TabIndex = 28;
            this.krplMemberName.Values.Text = "MemberName";
            this.krplMemberName.MouseClick += new System.Windows.Forms.MouseEventHandler(this.krplMemberName_MouseClick);
            // 
            // kryptonLabel10
            // 
            this.kryptonLabel10.Location = new System.Drawing.Point(128, 16);
            this.kryptonLabel10.Name = "kryptonLabel10";
            this.kryptonLabel10.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel10.TabIndex = 29;
            this.kryptonLabel10.Values.Text = ":";
            // 
            // flpMember
            // 
            this.flpMember.Controls.Add(this.krplMemberNameValue);
            this.flpMember.Controls.Add(this.btnMemberAdd);
            this.flpMember.Controls.Add(this.btnMemberRemove);
            this.flpMember.Location = new System.Drawing.Point(168, 12);
            this.flpMember.Name = "flpMember";
            this.flpMember.Size = new System.Drawing.Size(253, 30);
            this.flpMember.TabIndex = 27;
            // 
            // krplMemberNameValue
            // 
            this.krplMemberNameValue.Location = new System.Drawing.Point(3, 6);
            this.krplMemberNameValue.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplMemberNameValue.Name = "krplMemberNameValue";
            this.krplMemberNameValue.Size = new System.Drawing.Size(50, 20);
            this.krplMemberNameValue.TabIndex = 10;
            this.krplMemberNameValue.Values.Text = "000000";
            // 
            // btnMemberAdd
            // 
            this.btnMemberAdd.Location = new System.Drawing.Point(59, 3);
            this.btnMemberAdd.Name = "btnMemberAdd";
            this.btnMemberAdd.Size = new System.Drawing.Size(28, 25);
            this.btnMemberAdd.TabIndex = 30;
            this.btnMemberAdd.Values.Text = "+";
            this.btnMemberAdd.Click += new System.EventHandler(this.btnMemberAdd_Click);
            // 
            // btnMemberRemove
            // 
            this.btnMemberRemove.Location = new System.Drawing.Point(93, 3);
            this.btnMemberRemove.Name = "btnMemberRemove";
            this.btnMemberRemove.Size = new System.Drawing.Size(28, 25);
            this.btnMemberRemove.TabIndex = 35;
            this.btnMemberRemove.Values.Text = "-";
            this.btnMemberRemove.Click += new System.EventHandler(this.btnMemberRemove_Click);
            // 
            // krprbAdd
            // 
            this.krprbAdd.Location = new System.Drawing.Point(174, 175);
            this.krprbAdd.Name = "krprbAdd";
            this.krprbAdd.Size = new System.Drawing.Size(31, 20);
            this.krprbAdd.TabIndex = 45;
            this.krprbAdd.Values.Text = "+";
            this.krprbAdd.Click += new System.EventHandler(this.krprbAdd_Click);
            // 
            // krprbSub
            // 
            this.krprbSub.Location = new System.Drawing.Point(211, 175);
            this.krprbSub.Name = "krprbSub";
            this.krprbSub.Size = new System.Drawing.Size(27, 20);
            this.krprbSub.TabIndex = 50;
            this.krprbSub.Values.Text = "-";
            this.krprbSub.Click += new System.EventHandler(this.krprbAdd_Click);
            // 
            // PriceCommonChangeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(363, 303);
            this.Controls.Add(this.krprbSub);
            this.Controls.Add(this.krprbAdd);
            this.Controls.Add(this.krplMemberName);
            this.Controls.Add(this.kryptonLabel10);
            this.Controls.Add(this.flpMember);
            this.Controls.Add(this.krpcbBalancePay);
            this.Controls.Add(this.krptChangePrice);
            this.Controls.Add(this.krplBalancePrice);
            this.Controls.Add(this.krplBalancePay);
            this.Controls.Add(this.krplChangePrice);
            this.Controls.Add(this.krplPaidPrice);
            this.Controls.Add(this.kryptonLabel6);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplBalancePriceValue);
            this.Controls.Add(this.krplPaidPriceValue);
            this.Controls.Add(this.krplTotalPriceValue);
            this.Controls.Add(this.krplTotalPrice);
            this.Controls.Add(this.krpbAdd);
            this.Controls.Add(this.krpbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PriceCommonChangeWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PriceCommonChangeWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcbBalancePay)).EndInit();
            this.flpMember.ResumeLayout(false);
            this.flpMember.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSave;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplChangePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptChangePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBalancePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBalancePriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBalancePay;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbBalancePay;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel10;
        private System.Windows.Forms.FlowLayoutPanel flpMember;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberNameValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnMemberAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnMemberRemove;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton krprbAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonRadioButton krprbSub;
    }
}