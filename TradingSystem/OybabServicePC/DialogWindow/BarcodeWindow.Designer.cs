namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class BarcodeWindow
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
            this.krpgBarcodeInput = new ComponentFactory.Krypton.Toolkit.KryptonGroup();
            this.krptBarcodeNo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplBarcodeNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpgProductInfo = new ComponentFactory.Krypton.Toolkit.KryptonGroup();
            this.krpbCancel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krptCount = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplProductName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krpgBarcodeInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpgBarcodeInput.Panel)).BeginInit();
            this.krpgBarcodeInput.Panel.SuspendLayout();
            this.krpgBarcodeInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpgProductInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpgProductInfo.Panel)).BeginInit();
            this.krpgProductInfo.Panel.SuspendLayout();
            this.krpgProductInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpgBarcodeInput
            // 
            this.krpgBarcodeInput.Location = new System.Drawing.Point(12, 29);
            this.krpgBarcodeInput.Name = "krpgBarcodeInput";
            // 
            // krpgBarcodeInput.Panel
            // 
            this.krpgBarcodeInput.Panel.Controls.Add(this.krptBarcodeNo);
            this.krpgBarcodeInput.Panel.Controls.Add(this.krplBarcodeNo);
            this.krpgBarcodeInput.Size = new System.Drawing.Size(356, 35);
            this.krpgBarcodeInput.TabIndex = 0;
            // 
            // krptBarcodeNo
            // 
            this.krptBarcodeNo.Location = new System.Drawing.Point(143, 6);
            this.krptBarcodeNo.MaxLength = 20;
            this.krptBarcodeNo.Name = "krptBarcodeNo";
            this.krptBarcodeNo.Size = new System.Drawing.Size(187, 20);
            this.krptBarcodeNo.TabIndex = 1;
            this.krptBarcodeNo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptBarcodeNo_KeyUp);
            // 
            // krplBarcodeNo
            // 
            this.krplBarcodeNo.Location = new System.Drawing.Point(10, 6);
            this.krplBarcodeNo.Name = "krplBarcodeNo";
            this.krplBarcodeNo.Size = new System.Drawing.Size(80, 20);
            this.krplBarcodeNo.TabIndex = 0;
            this.krplBarcodeNo.Values.Text = "Bar Code No";
            // 
            // krpgProductInfo
            // 
            this.krpgProductInfo.Location = new System.Drawing.Point(12, 12);
            this.krpgProductInfo.Name = "krpgProductInfo";
            // 
            // krpgProductInfo.Panel
            // 
            this.krpgProductInfo.Panel.Controls.Add(this.krpbCancel);
            this.krpgProductInfo.Panel.Controls.Add(this.krpbAdd);
            this.krpgProductInfo.Panel.Controls.Add(this.krptCount);
            this.krpgProductInfo.Panel.Controls.Add(this.krplProductName);
            this.krpgProductInfo.Size = new System.Drawing.Size(356, 76);
            this.krpgProductInfo.TabIndex = 0;
            this.krpgProductInfo.Visible = false;
            // 
            // krpbCancel
            // 
            this.krpbCancel.Location = new System.Drawing.Point(197, 43);
            this.krpbCancel.Name = "krpbCancel";
            this.krpbCancel.Size = new System.Drawing.Size(67, 25);
            this.krpbCancel.TabIndex = 2;
            this.krpbCancel.Values.Text = "Cancel";
            this.krpbCancel.Click += new System.EventHandler(this.krpbCancel_Click);
            // 
            // krpbAdd
            // 
            this.krpbAdd.Location = new System.Drawing.Point(94, 43);
            this.krpbAdd.Name = "krpbAdd";
            this.krpbAdd.Size = new System.Drawing.Size(67, 25);
            this.krpbAdd.TabIndex = 2;
            this.krpbAdd.Values.Text = "Add";
            this.krpbAdd.Click += new System.EventHandler(this.krpbAdd_Click);
            // 
            // krptCount
            // 
            this.krptCount.Location = new System.Drawing.Point(287, 8);
            this.krptCount.MaxLength = 4;
            this.krptCount.Name = "krptCount";
            this.krptCount.Size = new System.Drawing.Size(43, 20);
            this.krptCount.TabIndex = 1;
            this.krptCount.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCount_KeyUp);
            this.krptCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // krplProductName
            // 
            this.krplProductName.Location = new System.Drawing.Point(10, 8);
            this.krplProductName.Name = "krplProductName";
            this.krplProductName.Size = new System.Drawing.Size(86, 20);
            this.krplProductName.TabIndex = 0;
            this.krplProductName.Values.Text = "ProductName";
            // 
            // BarcodeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(380, 98);
            this.Controls.Add(this.krpgBarcodeInput);
            this.Controls.Add(this.krpgProductInfo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BarcodeWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BarcodeWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpgBarcodeInput.Panel)).EndInit();
            this.krpgBarcodeInput.Panel.ResumeLayout(false);
            this.krpgBarcodeInput.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpgBarcodeInput)).EndInit();
            this.krpgBarcodeInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpgProductInfo.Panel)).EndInit();
            this.krpgProductInfo.Panel.ResumeLayout(false);
            this.krpgProductInfo.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpgProductInfo)).EndInit();
            this.krpgProductInfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonGroup krpgBarcodeInput;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptBarcodeNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBarcodeNo;
        private ComponentFactory.Krypton.Toolkit.KryptonGroup krpgProductInfo;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbCancel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptCount;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplProductName;

    }
}