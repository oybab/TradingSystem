namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class CheckoutWindow
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
            this.krplTotalTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRemainingTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel6 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRemainingTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
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
            this.krplMemberPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel12 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel11 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBorrowPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel16 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBorrowPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplKeepPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel19 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplKeepPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.krplRoomNoValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplMemberPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pnPrice = new System.Windows.Forms.Panel();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpbCheckout
            // 
            this.krpbCheckout.AutoSize = true;
            this.krpbCheckout.Location = new System.Drawing.Point(182, 384);
            this.krpbCheckout.Name = "krpbCheckout";
            this.krpbCheckout.Size = new System.Drawing.Size(94, 25);
            this.krpbCheckout.TabIndex = 20;
            this.krpbCheckout.Values.Text = "Checkout";
            this.krpbCheckout.Click += new System.EventHandler(this.krpbCheckout_Click);
            // 
            // krplTotalTime
            // 
            this.krplTotalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalTime.Location = new System.Drawing.Point(91, 56);
            this.krplTotalTime.Name = "krplTotalTime";
            this.krplTotalTime.Size = new System.Drawing.Size(64, 20);
            this.krplTotalTime.TabIndex = 1;
            this.krplTotalTime.Values.Text = "TotalTime";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(177, 56);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel4.TabIndex = 1;
            this.kryptonLabel4.Values.Text = ":";
            // 
            // krplTotalTimeValue
            // 
            this.krplTotalTimeValue.Location = new System.Drawing.Point(215, 56);
            this.krplTotalTimeValue.Name = "krplTotalTimeValue";
            this.krplTotalTimeValue.Size = new System.Drawing.Size(35, 20);
            this.krplTotalTimeValue.TabIndex = 1;
            this.krplTotalTimeValue.Values.Text = "??:??";
            // 
            // krplRemainingTimeValue
            // 
            this.krplRemainingTimeValue.Location = new System.Drawing.Point(216, 87);
            this.krplRemainingTimeValue.Name = "krplRemainingTimeValue";
            this.krplRemainingTimeValue.Size = new System.Drawing.Size(35, 20);
            this.krplRemainingTimeValue.TabIndex = 1;
            this.krplRemainingTimeValue.Values.Text = "??:??";
            // 
            // kryptonLabel6
            // 
            this.kryptonLabel6.Location = new System.Drawing.Point(177, 87);
            this.kryptonLabel6.Name = "kryptonLabel6";
            this.kryptonLabel6.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel6.TabIndex = 1;
            this.kryptonLabel6.Values.Text = ":";
            // 
            // krplRemainingTime
            // 
            this.krplRemainingTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRemainingTime.Location = new System.Drawing.Point(60, 87);
            this.krplRemainingTime.Name = "krplRemainingTime";
            this.krplRemainingTime.Size = new System.Drawing.Size(95, 20);
            this.krplRemainingTime.TabIndex = 1;
            this.krplRemainingTime.Values.Text = "RemainingTime";
            // 
            // krplRoomNo
            // 
            this.krplRoomNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRoomNo.Location = new System.Drawing.Point(95, 12);
            this.krplRoomNo.Name = "krplRoomNo";
            this.krplRoomNo.Size = new System.Drawing.Size(60, 20);
            this.krplRoomNo.TabIndex = 1;
            this.krplRoomNo.Values.Text = "RoomNo";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(177, 12);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 1;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(177, 127);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 8;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // krplTotalPriceValue
            // 
            this.krplTotalPriceValue.Location = new System.Drawing.Point(215, 127);
            this.krplTotalPriceValue.Name = "krplTotalPriceValue";
            this.krplTotalPriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplTotalPriceValue.TabIndex = 10;
            this.krplTotalPriceValue.Values.Text = "0000000";
            // 
            // krplTotalPrice
            // 
            this.krplTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPrice.Location = new System.Drawing.Point(91, 127);
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
            this.flowLayoutPanel2.Location = new System.Drawing.Point(212, 214);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(200, 30);
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
            this.krpbPaidPriceFinish.TabIndex = 5;
            this.krpbPaidPriceFinish.Values.Text = "=";
            this.krpbPaidPriceFinish.Visible = false;
            this.krpbPaidPriceFinish.Click += new System.EventHandler(this.krpbPaidPriceFinish_Click);
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(177, 219);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel7.TabIndex = 22;
            this.kryptonLabel7.Values.Text = ":";
            // 
            // krplPaidPrice
            // 
            this.krplPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPaidPrice.Location = new System.Drawing.Point(95, 219);
            this.krplPaidPrice.Name = "krplPaidPrice";
            this.krplPaidPrice.Size = new System.Drawing.Size(60, 20);
            this.krplPaidPrice.TabIndex = 23;
            this.krplPaidPrice.Values.Text = "PaidPrice";
            this.krplPaidPrice.MouseClick += new System.Windows.Forms.MouseEventHandler(this.krplPaidPrice_MouseClick);
            // 
            // krplMemberPaidPrice
            // 
            this.krplMemberPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplMemberPaidPrice.Location = new System.Drawing.Point(49, 182);
            this.krplMemberPaidPrice.Name = "krplMemberPaidPrice";
            this.krplMemberPaidPrice.Size = new System.Drawing.Size(107, 20);
            this.krplMemberPaidPrice.TabIndex = 31;
            this.krplMemberPaidPrice.Values.Text = "MemberPaidPrice";
            // 
            // kryptonLabel12
            // 
            this.kryptonLabel12.Location = new System.Drawing.Point(177, 180);
            this.kryptonLabel12.Name = "kryptonLabel12";
            this.kryptonLabel12.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel12.TabIndex = 30;
            this.kryptonLabel12.Values.Text = ":";
            // 
            // krplTotalPaidPrice
            // 
            this.krplTotalPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPaidPrice.Location = new System.Drawing.Point(68, 255);
            this.krplTotalPaidPrice.Name = "krplTotalPaidPrice";
            this.krplTotalPaidPrice.Size = new System.Drawing.Size(88, 20);
            this.krplTotalPaidPrice.TabIndex = 31;
            this.krplTotalPaidPrice.Values.Text = "TotalPaidPrice";
            // 
            // kryptonLabel11
            // 
            this.kryptonLabel11.Location = new System.Drawing.Point(177, 255);
            this.kryptonLabel11.Name = "kryptonLabel11";
            this.kryptonLabel11.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel11.TabIndex = 30;
            this.kryptonLabel11.Values.Text = ":";
            // 
            // krplTotalPaidPriceValue
            // 
            this.krplTotalPaidPriceValue.Location = new System.Drawing.Point(215, 250);
            this.krplTotalPaidPriceValue.Name = "krplTotalPaidPriceValue";
            this.krplTotalPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPaidPriceValue.TabIndex = 29;
            this.krplTotalPaidPriceValue.Values.Text = "00000000";
            // 
            // krplBorrowPrice
            // 
            this.krplBorrowPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBorrowPrice.Location = new System.Drawing.Point(79, 301);
            this.krplBorrowPrice.Name = "krplBorrowPrice";
            this.krplBorrowPrice.Size = new System.Drawing.Size(76, 20);
            this.krplBorrowPrice.TabIndex = 31;
            this.krplBorrowPrice.Values.Text = "BorrowPrice";
            // 
            // kryptonLabel16
            // 
            this.kryptonLabel16.Location = new System.Drawing.Point(177, 301);
            this.kryptonLabel16.Name = "kryptonLabel16";
            this.kryptonLabel16.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel16.TabIndex = 30;
            this.kryptonLabel16.Values.Text = ":";
            // 
            // krplBorrowPriceValue
            // 
            this.krplBorrowPriceValue.Location = new System.Drawing.Point(215, 304);
            this.krplBorrowPriceValue.Name = "krplBorrowPriceValue";
            this.krplBorrowPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplBorrowPriceValue.TabIndex = 29;
            this.krplBorrowPriceValue.Values.Text = "00000000";
            // 
            // krplKeepPrice
            // 
            this.krplKeepPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplKeepPrice.Location = new System.Drawing.Point(92, 330);
            this.krplKeepPrice.Name = "krplKeepPrice";
            this.krplKeepPrice.Size = new System.Drawing.Size(64, 20);
            this.krplKeepPrice.TabIndex = 31;
            this.krplKeepPrice.Values.Text = "KeepPrice";
            // 
            // kryptonLabel19
            // 
            this.kryptonLabel19.Location = new System.Drawing.Point(177, 330);
            this.kryptonLabel19.Name = "kryptonLabel19";
            this.kryptonLabel19.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel19.TabIndex = 30;
            this.kryptonLabel19.Values.Text = ":";
            // 
            // krplKeepPriceValue
            // 
            this.krplKeepPriceValue.Location = new System.Drawing.Point(216, 330);
            this.krplKeepPriceValue.Name = "krplKeepPriceValue";
            this.krplKeepPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplKeepPriceValue.TabIndex = 29;
            this.krplKeepPriceValue.Values.Text = "00000000";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.krplRoomNoValue);
            this.flowLayoutPanel4.Controls.Add(this.krplRoomPriceValue);
            this.flowLayoutPanel4.Location = new System.Drawing.Point(212, 10);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(253, 30);
            this.flowLayoutPanel4.TabIndex = 24;
            // 
            // krplRoomNoValue
            // 
            this.krplRoomNoValue.Location = new System.Drawing.Point(3, 3);
            this.krplRoomNoValue.Name = "krplRoomNoValue";
            this.krplRoomNoValue.Size = new System.Drawing.Size(44, 20);
            this.krplRoomNoValue.TabIndex = 2;
            this.krplRoomNoValue.Values.Text = "00000";
            // 
            // krplRoomPriceValue
            // 
            this.krplRoomPriceValue.Location = new System.Drawing.Point(53, 3);
            this.krplRoomPriceValue.Name = "krplRoomPriceValue";
            this.krplRoomPriceValue.Size = new System.Drawing.Size(51, 20);
            this.krplRoomPriceValue.TabIndex = 3;
            this.krplRoomPriceValue.Values.Text = "(00000)";
            // 
            // krplMemberPaidPriceValue
            // 
            this.krplMemberPaidPriceValue.Location = new System.Drawing.Point(215, 180);
            this.krplMemberPaidPriceValue.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplMemberPaidPriceValue.Name = "krplMemberPaidPriceValue";
            this.krplMemberPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplMemberPaidPriceValue.TabIndex = 32;
            this.krplMemberPaidPriceValue.Values.Text = "00000000";
            // 
            // pnPrice
            // 
            this.pnPrice.Location = new System.Drawing.Point(353, 56);
            this.pnPrice.Name = "pnPrice";
            this.pnPrice.Size = new System.Drawing.Size(387, 342);
            this.pnPrice.TabIndex = 10;
            // 
            // CheckoutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(752, 434);
            this.Controls.Add(this.pnPrice);
            this.Controls.Add(this.krplMemberPaidPriceValue);
            this.Controls.Add(this.krplKeepPriceValue);
            this.Controls.Add(this.krplBorrowPriceValue);
            this.Controls.Add(this.krplTotalPaidPriceValue);
            this.Controls.Add(this.kryptonLabel12);
            this.Controls.Add(this.kryptonLabel19);
            this.Controls.Add(this.kryptonLabel16);
            this.Controls.Add(this.kryptonLabel11);
            this.Controls.Add(this.krplMemberPaidPrice);
            this.Controls.Add(this.krplKeepPrice);
            this.Controls.Add(this.krplBorrowPrice);
            this.Controls.Add(this.krplTotalPaidPrice);
            this.Controls.Add(this.flowLayoutPanel4);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.kryptonLabel7);
            this.Controls.Add(this.krplPaidPrice);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplTotalPriceValue);
            this.Controls.Add(this.krplTotalPrice);
            this.Controls.Add(this.krplRemainingTime);
            this.Controls.Add(this.krplRoomNo);
            this.Controls.Add(this.krplTotalTime);
            this.Controls.Add(this.kryptonLabel6);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.krplRemainingTimeValue);
            this.Controls.Add(this.krplTotalTimeValue);
            this.Controls.Add(this.krpbCheckout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckoutWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CheckoutWindow";
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

      


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbCheckout;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemainingTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel6;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemainingTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPrice;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnPaidPriceAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnPaidPriceSub;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel12;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel11;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBorrowPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel16;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBorrowPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplKeepPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel19;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplKeepPriceValue;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNoValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberPaidPriceValue;
        private System.Windows.Forms.Panel pnPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPaidPriceFinish;
    }
}