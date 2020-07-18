namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class ImportCheckoutWindow
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
            this.krpbCheckout = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.krplPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnPaidPriceAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnPaidPriceSub = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbPaidPriceFinish = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBorrowPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel16 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBorrowPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplKeepPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel19 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplKeepPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel12 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplSupplierPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel11 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplSupplierPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pnPrice = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpbCheckout
            // 
            this.krpbCheckout.Location = new System.Drawing.Point(192, 314);
            this.krpbCheckout.Name = "krpbCheckout";
            this.krpbCheckout.Size = new System.Drawing.Size(90, 25);
            this.krpbCheckout.TabIndex = 20;
            this.krpbCheckout.Values.Text = "Checkout";
            this.krpbCheckout.Click += new System.EventHandler(this.krpbCheckout_Click);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(176, 24);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 8;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // krplTotalPriceValue
            // 
            this.krplTotalPriceValue.Location = new System.Drawing.Point(211, 24);
            this.krplTotalPriceValue.Name = "krplTotalPriceValue";
            this.krplTotalPriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplTotalPriceValue.TabIndex = 10;
            this.krplTotalPriceValue.Values.Text = "0000000";
            // 
            // krplTotalPrice
            // 
            this.krplTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPrice.Location = new System.Drawing.Point(91, 24);
            this.krplTotalPrice.Name = "krplTotalPrice";
            this.krplTotalPrice.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPrice.TabIndex = 11;
            this.krplTotalPrice.Values.Text = "TotalPrice";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.krplPaidPriceValue);
            this.flowLayoutPanel2.Controls.Add(this.btnPaidPriceAdd);
            this.flowLayoutPanel2.Controls.Add(this.btnPaidPriceSub);
            this.flowLayoutPanel2.Controls.Add(this.krpbPaidPriceFinish);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(207, 120);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(199, 30);
            this.flowLayoutPanel2.TabIndex = 24;
            // 
            // krplPaidPriceValue
            // 
            this.krplPaidPriceValue.Location = new System.Drawing.Point(3, 6);
            this.krplPaidPriceValue.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplPaidPriceValue.Name = "krplPaidPriceValue";
            this.krplPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplPaidPriceValue.TabIndex = 10;
            this.krplPaidPriceValue.Values.Text = "00000000";
            // 
            // btnPaidPriceAdd
            // 
            this.btnPaidPriceAdd.Location = new System.Drawing.Point(73, 3);
            this.btnPaidPriceAdd.Name = "btnPaidPriceAdd";
            this.btnPaidPriceAdd.Size = new System.Drawing.Size(28, 25);
            this.btnPaidPriceAdd.TabIndex = 4;
            this.btnPaidPriceAdd.Values.Text = "+";
            this.btnPaidPriceAdd.Visible = false;
            this.btnPaidPriceAdd.Click += new System.EventHandler(this.btnPaidPriceAdd_Click);
            // 
            // btnPaidPriceSub
            // 
            this.btnPaidPriceSub.Location = new System.Drawing.Point(107, 3);
            this.btnPaidPriceSub.Name = "btnPaidPriceSub";
            this.btnPaidPriceSub.Size = new System.Drawing.Size(28, 25);
            this.btnPaidPriceSub.TabIndex = 5;
            this.btnPaidPriceSub.Values.Text = "-";
            this.btnPaidPriceSub.Visible = false;
            this.btnPaidPriceSub.Click += new System.EventHandler(this.btnPaidPriceSub_Click);
            // 
            // krpbPaidPriceFinish
            // 
            this.krpbPaidPriceFinish.Location = new System.Drawing.Point(141, 3);
            this.krpbPaidPriceFinish.Name = "krpbPaidPriceFinish";
            this.krpbPaidPriceFinish.Size = new System.Drawing.Size(28, 25);
            this.krpbPaidPriceFinish.TabIndex = 1;
            this.krpbPaidPriceFinish.Values.Text = "=";
            this.krpbPaidPriceFinish.Visible = false;
            this.krpbPaidPriceFinish.Click += new System.EventHandler(this.krpbPaidPriceFinish_Click);
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(176, 127);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel7.TabIndex = 22;
            this.kryptonLabel7.Values.Text = ":";
            // 
            // krplPaidPrice
            // 
            this.krplPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPaidPrice.Location = new System.Drawing.Point(95, 127);
            this.krplPaidPrice.Name = "krplPaidPrice";
            this.krplPaidPrice.Size = new System.Drawing.Size(60, 20);
            this.krplPaidPrice.TabIndex = 23;
            this.krplPaidPrice.Values.Text = "PaidPrice";
            this.krplPaidPrice.MouseClick += new System.Windows.Forms.MouseEventHandler(this.krplPaidPrice_MouseClick);
            // 
            // krplBorrowPrice
            // 
            this.krplBorrowPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBorrowPrice.Location = new System.Drawing.Point(79, 229);
            this.krplBorrowPrice.Name = "krplBorrowPrice";
            this.krplBorrowPrice.Size = new System.Drawing.Size(76, 20);
            this.krplBorrowPrice.TabIndex = 31;
            this.krplBorrowPrice.Values.Text = "BorrowPrice";
            // 
            // kryptonLabel16
            // 
            this.kryptonLabel16.Location = new System.Drawing.Point(176, 229);
            this.kryptonLabel16.Name = "kryptonLabel16";
            this.kryptonLabel16.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel16.TabIndex = 30;
            this.kryptonLabel16.Values.Text = ":";
            // 
            // krplBorrowPriceValue
            // 
            this.krplBorrowPriceValue.Location = new System.Drawing.Point(210, 229);
            this.krplBorrowPriceValue.Name = "krplBorrowPriceValue";
            this.krplBorrowPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplBorrowPriceValue.TabIndex = 29;
            this.krplBorrowPriceValue.Values.Text = "00000000";
            // 
            // krplKeepPrice
            // 
            this.krplKeepPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplKeepPrice.Location = new System.Drawing.Point(91, 258);
            this.krplKeepPrice.Name = "krplKeepPrice";
            this.krplKeepPrice.Size = new System.Drawing.Size(64, 20);
            this.krplKeepPrice.TabIndex = 31;
            this.krplKeepPrice.Values.Text = "KeepPrice";
            // 
            // kryptonLabel19
            // 
            this.kryptonLabel19.Location = new System.Drawing.Point(176, 258);
            this.kryptonLabel19.Name = "kryptonLabel19";
            this.kryptonLabel19.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel19.TabIndex = 30;
            this.kryptonLabel19.Values.Text = ":";
            // 
            // krplKeepPriceValue
            // 
            this.krplKeepPriceValue.Location = new System.Drawing.Point(210, 255);
            this.krplKeepPriceValue.Name = "krplKeepPriceValue";
            this.krplKeepPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplKeepPriceValue.TabIndex = 29;
            this.krplKeepPriceValue.Values.Text = "00000000";
            // 
            // kryptonLabel12
            // 
            this.kryptonLabel12.Location = new System.Drawing.Point(176, 82);
            this.kryptonLabel12.Name = "kryptonLabel12";
            this.kryptonLabel12.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel12.TabIndex = 40;
            this.kryptonLabel12.Values.Text = ":";
            // 
            // krplSupplierPaidPrice
            // 
            this.krplSupplierPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSupplierPaidPrice.Location = new System.Drawing.Point(50, 84);
            this.krplSupplierPaidPrice.Name = "krplSupplierPaidPrice";
            this.krplSupplierPaidPrice.Size = new System.Drawing.Size(105, 20);
            this.krplSupplierPaidPrice.TabIndex = 41;
            this.krplSupplierPaidPrice.Values.Text = "SupplierPaidPrice";
            // 
            // krplTotalPaidPriceValue
            // 
            this.krplTotalPaidPriceValue.Location = new System.Drawing.Point(211, 170);
            this.krplTotalPaidPriceValue.Name = "krplTotalPaidPriceValue";
            this.krplTotalPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPaidPriceValue.TabIndex = 42;
            this.krplTotalPaidPriceValue.Values.Text = "00000000";
            // 
            // kryptonLabel11
            // 
            this.kryptonLabel11.Location = new System.Drawing.Point(176, 170);
            this.kryptonLabel11.Name = "kryptonLabel11";
            this.kryptonLabel11.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel11.TabIndex = 43;
            this.kryptonLabel11.Values.Text = ":";
            // 
            // krplTotalPaidPrice
            // 
            this.krplTotalPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPaidPrice.Location = new System.Drawing.Point(67, 170);
            this.krplTotalPaidPrice.Name = "krplTotalPaidPrice";
            this.krplTotalPaidPrice.Size = new System.Drawing.Size(88, 20);
            this.krplTotalPaidPrice.TabIndex = 44;
            this.krplTotalPaidPrice.Values.Text = "TotalPaidPrice";
            // 
            // krplSupplierPaidPriceValue
            // 
            this.krplSupplierPaidPriceValue.Location = new System.Drawing.Point(210, 82);
            this.krplSupplierPaidPriceValue.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplSupplierPaidPriceValue.Name = "krplSupplierPaidPriceValue";
            this.krplSupplierPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplSupplierPaidPriceValue.TabIndex = 45;
            this.krplSupplierPaidPriceValue.Values.Text = "00000000";
            // 
            // pnPrice
            // 
            this.pnPrice.Location = new System.Drawing.Point(353, 6);
            this.pnPrice.Name = "pnPrice";
            this.pnPrice.Size = new System.Drawing.Size(387, 342);
            this.pnPrice.TabIndex = 10;
            // 
            // ImportCheckoutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(752, 360);
            this.Controls.Add(this.pnPrice);
            this.Controls.Add(this.krplSupplierPaidPriceValue);
            this.Controls.Add(this.krplTotalPaidPriceValue);
            this.Controls.Add(this.kryptonLabel11);
            this.Controls.Add(this.krplTotalPaidPrice);
            this.Controls.Add(this.kryptonLabel12);
            this.Controls.Add(this.krplSupplierPaidPrice);
            this.Controls.Add(this.krplKeepPriceValue);
            this.Controls.Add(this.krplBorrowPriceValue);
            this.Controls.Add(this.kryptonLabel19);
            this.Controls.Add(this.kryptonLabel16);
            this.Controls.Add(this.krplKeepPrice);
            this.Controls.Add(this.krplBorrowPrice);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.kryptonLabel7);
            this.Controls.Add(this.krplPaidPrice);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplTotalPriceValue);
            this.Controls.Add(this.krplTotalPrice);
            this.Controls.Add(this.krpbCheckout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportCheckoutWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ImportCheckoutWindow";
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbCheckout;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPrice;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnPaidPriceAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnPaidPriceSub;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBorrowPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel16;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBorrowPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplKeepPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel19;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplKeepPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPaidPriceFinish;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel12;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSupplierPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel11;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSupplierPaidPriceValue;
        private System.Windows.Forms.Panel pnPrice;
    }
}