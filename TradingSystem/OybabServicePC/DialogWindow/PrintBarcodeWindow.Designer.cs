namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PrintBarcodeWindow
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
            this.krpbPrint = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpcLanguage = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplLanguage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplSize = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcSize = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplCount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcCount = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcSize)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbPrint
            // 
            this.krpbPrint.Location = new System.Drawing.Point(113, 104);
            this.krpbPrint.Name = "krpbPrint";
            this.krpbPrint.Size = new System.Drawing.Size(90, 25);
            this.krpbPrint.TabIndex = 50;
            this.krpbPrint.Values.Text = "Print";
            this.krpbPrint.Click += new System.EventHandler(this.krpbChange_Click);
            // 
            // krpcLanguage
            // 
            this.krpcLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcLanguage.DropDownWidth = 121;
            this.krpcLanguage.Location = new System.Drawing.Point(176, 17);
            this.krpcLanguage.Name = "krpcLanguage";
            this.krpcLanguage.Size = new System.Drawing.Size(121, 21);
            this.krpcLanguage.TabIndex = 1;
            this.krpcLanguage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krpcLanguage_KeyDown);
            // 
            // krplLanguage
            // 
            this.krplLanguage.Location = new System.Drawing.Point(34, 17);
            this.krplLanguage.Name = "krplLanguage";
            this.krplLanguage.Size = new System.Drawing.Size(64, 20);
            this.krplLanguage.TabIndex = 5;
            this.krplLanguage.Values.Text = "Language";
            // 
            // krplSize
            // 
            this.krplSize.Location = new System.Drawing.Point(34, 56);
            this.krplSize.Name = "krplSize";
            this.krplSize.Size = new System.Drawing.Size(32, 20);
            this.krplSize.TabIndex = 5;
            this.krplSize.Values.Text = "Size";
            // 
            // krpcSize
            // 
            this.krpcSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcSize.DropDownWidth = 121;
            this.krpcSize.Location = new System.Drawing.Point(176, 55);
            this.krpcSize.Name = "krpcSize";
            this.krpcSize.Size = new System.Drawing.Size(121, 21);
            this.krpcSize.TabIndex = 10;
            this.krpcSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krpcLanguage_KeyDown);
            // 
            // krplCount
            // 
            this.krplCount.Location = new System.Drawing.Point(34, 95);
            this.krplCount.Name = "krplCount";
            this.krplCount.Size = new System.Drawing.Size(44, 20);
            this.krplCount.TabIndex = 5;
            this.krplCount.Values.Text = "Count";
            this.krplCount.Visible = false;
            // 
            // krpcCount
            // 
            this.krpcCount.Location = new System.Drawing.Point(177, 93);
            this.krpcCount.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.krpcCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcCount.Name = "krpcCount";
            this.krpcCount.Size = new System.Drawing.Size(120, 22);
            this.krpcCount.TabIndex = 20;
            this.krpcCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcCount.Visible = false;
            // 
            // PrintBarcodeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(340, 140);
            this.Controls.Add(this.krpbPrint);
            this.Controls.Add(this.krpcCount);
            this.Controls.Add(this.krpcSize);
            this.Controls.Add(this.krpcLanguage);
            this.Controls.Add(this.krplCount);
            this.Controls.Add(this.krplSize);
            this.Controls.Add(this.krplLanguage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintBarcodeWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PrintBarcodeWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcLanguage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPrint;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSize;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcSize;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplCount;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown krpcCount;
    }
}