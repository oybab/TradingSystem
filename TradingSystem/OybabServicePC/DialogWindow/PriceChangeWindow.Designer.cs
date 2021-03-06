﻿namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PriceChangeWindow
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
            this.krplChangeMark = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.SuspendLayout();
            // 
            // krpbSave
            // 
            this.krpbSave.Location = new System.Drawing.Point(81, 192);
            this.krpbSave.Name = "krpbSave";
            this.krpbSave.Size = new System.Drawing.Size(90, 25);
            this.krpbSave.TabIndex = 4;
            this.krpbSave.Values.Text = "Save";
            this.krpbSave.Click += new System.EventHandler(this.krpbSave_Click);
            // 
            // krplTotalPrice
            // 
            this.krplTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPrice.Location = new System.Drawing.Point(46, 27);
            this.krplTotalPrice.Name = "krplTotalPrice";
            this.krplTotalPrice.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPrice.TabIndex = 1;
            this.krplTotalPrice.Values.Text = "TotalPrice";
            // 
            // krplPaidPrice
            // 
            this.krplPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPaidPrice.Location = new System.Drawing.Point(50, 65);
            this.krplPaidPrice.Name = "krplPaidPrice";
            this.krplPaidPrice.Size = new System.Drawing.Size(60, 20);
            this.krplPaidPrice.TabIndex = 1;
            this.krplPaidPrice.Values.Text = "PaidPrice";
            // 
            // krplChangePrice
            // 
            this.krplChangePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplChangePrice.Location = new System.Drawing.Point(31, 102);
            this.krplChangePrice.Name = "krplChangePrice";
            this.krplChangePrice.Size = new System.Drawing.Size(79, 20);
            this.krplChangePrice.TabIndex = 1;
            this.krplChangePrice.Values.Text = "ChangePrice";
            // 
            // krptChangePrice
            // 
            this.krptChangePrice.Location = new System.Drawing.Point(154, 102);
            this.krptChangePrice.MaxLength = 12;
            this.krptChangePrice.Name = "krptChangePrice";
            this.krptChangePrice.Size = new System.Drawing.Size(57, 20);
            this.krptChangePrice.TabIndex = 3;
            this.krptChangePrice.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptChangePrice_KeyUp);
            this.krptChangePrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(108, 27);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(108, 65);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(108, 102);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 1;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // krplTotalPriceValue
            // 
            this.krplTotalPriceValue.Location = new System.Drawing.Point(154, 27);
            this.krplTotalPriceValue.Name = "krplTotalPriceValue";
            this.krplTotalPriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplTotalPriceValue.TabIndex = 1;
            this.krplTotalPriceValue.Values.Text = "0000000";
            // 
            // krplPaidPriceValue
            // 
            this.krplPaidPriceValue.Location = new System.Drawing.Point(154, 65);
            this.krplPaidPriceValue.Name = "krplPaidPriceValue";
            this.krplPaidPriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplPaidPriceValue.TabIndex = 1;
            this.krplPaidPriceValue.Values.Text = "0000000";
            // 
            // krplBalancePrice
            // 
            this.krplBalancePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBalancePrice.Location = new System.Drawing.Point(31, 146);
            this.krplBalancePrice.Name = "krplBalancePrice";
            this.krplBalancePrice.Size = new System.Drawing.Size(79, 20);
            this.krplBalancePrice.TabIndex = 1;
            this.krplBalancePrice.Values.Text = "BalancePrice";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(108, 146);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel4.TabIndex = 1;
            this.kryptonLabel4.Values.Text = ":";
            // 
            // krplBalancePriceValue
            // 
            this.krplBalancePriceValue.Location = new System.Drawing.Point(154, 146);
            this.krplBalancePriceValue.Name = "krplBalancePriceValue";
            this.krplBalancePriceValue.Size = new System.Drawing.Size(57, 20);
            this.krplBalancePriceValue.TabIndex = 1;
            this.krplBalancePriceValue.Values.Text = "0000000";
            // 
            // krplChangeMark
            // 
            this.krplChangeMark.Location = new System.Drawing.Point(127, 102);
            this.krplChangeMark.Name = "krplChangeMark";
            this.krplChangeMark.Size = new System.Drawing.Size(19, 20);
            this.krplChangeMark.TabIndex = 1;
            this.krplChangeMark.Values.Text = "+";
            // 
            // PriceChangeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(252, 229);
            this.Controls.Add(this.krptChangePrice);
            this.Controls.Add(this.krplBalancePrice);
            this.Controls.Add(this.krplChangePrice);
            this.Controls.Add(this.krplPaidPrice);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplBalancePriceValue);
            this.Controls.Add(this.krplChangeMark);
            this.Controls.Add(this.krplPaidPriceValue);
            this.Controls.Add(this.krplTotalPriceValue);
            this.Controls.Add(this.krplTotalPrice);
            this.Controls.Add(this.krpbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PriceChangeWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PriceChangeWindow";
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
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplChangeMark;
    }
}