namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class BalanceTransferWindow
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
            this.krplOldBalanceName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplOldBalanceNameValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewBalanceName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbNewBalanceName = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krptChangePrice = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplChangePrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbNewBalanceName)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbSave
            // 
            this.krpbSave.Location = new System.Drawing.Point(105, 210);
            this.krpbSave.Name = "krpbSave";
            this.krpbSave.Size = new System.Drawing.Size(90, 25);
            this.krpbSave.TabIndex = 4;
            this.krpbSave.Values.Text = "Save";
            this.krpbSave.Click += new System.EventHandler(this.krpbSave_Click);
            // 
            // krplOldBalanceName
            // 
            this.krplOldBalanceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplOldBalanceName.Location = new System.Drawing.Point(38, 32);
            this.krplOldBalanceName.Name = "krplOldBalanceName";
            this.krplOldBalanceName.Size = new System.Drawing.Size(85, 20);
            this.krplOldBalanceName.TabIndex = 5;
            this.krplOldBalanceName.Values.Text = "BalanceName";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(133, 32);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 6;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // krplOldBalanceNameValue
            // 
            this.krplOldBalanceNameValue.Location = new System.Drawing.Point(169, 32);
            this.krplOldBalanceNameValue.Name = "krplOldBalanceNameValue";
            this.krplOldBalanceNameValue.Size = new System.Drawing.Size(37, 20);
            this.krplOldBalanceNameValue.TabIndex = 7;
            this.krplOldBalanceNameValue.Values.Text = "0000";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(133, 92);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 6;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // krplNewBalanceName
            // 
            this.krplNewBalanceName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplNewBalanceName.Location = new System.Drawing.Point(38, 92);
            this.krplNewBalanceName.Name = "krplNewBalanceName";
            this.krplNewBalanceName.Size = new System.Drawing.Size(85, 20);
            this.krplNewBalanceName.TabIndex = 5;
            this.krplNewBalanceName.Values.Text = "BalanceName";
            // 
            // krpcbNewBalanceName
            // 
            this.krpcbNewBalanceName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbNewBalanceName.DropDownWidth = 149;
            this.krpcbNewBalanceName.Location = new System.Drawing.Point(169, 92);
            this.krpcbNewBalanceName.Name = "krpcbNewBalanceName";
            this.krpcbNewBalanceName.Size = new System.Drawing.Size(114, 21);
            this.krpcbNewBalanceName.TabIndex = 12;
            // 
            // krptChangePrice
            // 
            this.krptChangePrice.Location = new System.Drawing.Point(169, 147);
            this.krptChangePrice.MaxLength = 12;
            this.krptChangePrice.Name = "krptChangePrice";
            this.krptChangePrice.Size = new System.Drawing.Size(114, 23);
            this.krptChangePrice.TabIndex = 14;
            this.krptChangePrice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptChangePrice_KeyDown);
            this.krptChangePrice.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptChangePrice_KeyUp);
            // 
            // krplChangePrice
            // 
            this.krplChangePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplChangePrice.Location = new System.Drawing.Point(38, 150);
            this.krplChangePrice.Name = "krplChangePrice";
            this.krplChangePrice.Size = new System.Drawing.Size(79, 20);
            this.krplChangePrice.TabIndex = 13;
            this.krplChangePrice.Values.Text = "ChangePrice";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(133, 150);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 6;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // BalanceTransferWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(310, 247);
            this.Controls.Add(this.krptChangePrice);
            this.Controls.Add(this.krplChangePrice);
            this.Controls.Add(this.krpcbNewBalanceName);
            this.Controls.Add(this.krplNewBalanceName);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.krplOldBalanceName);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.krplOldBalanceNameValue);
            this.Controls.Add(this.krpbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BalanceTransferWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BalanceTransferWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcbNewBalanceName)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSave;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplOldBalanceName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplOldBalanceNameValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewBalanceName;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbNewBalanceName;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptChangePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplChangePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}