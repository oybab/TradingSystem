namespace Oybab.ServicePC.DialogWindow.ConditionWindow
{
    internal sealed partial class ProductConditionWindow
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
            this.krpbSearch = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplProductName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptProductName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplBarcode = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptBarcode = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplWarn = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbCount = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbTime = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krplHideType = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcmHideType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.krpcmHideType)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbSearch
            // 
            this.krpbSearch.Location = new System.Drawing.Point(138, 204);
            this.krpbSearch.Name = "krpbSearch";
            this.krpbSearch.Size = new System.Drawing.Size(90, 25);
            this.krpbSearch.TabIndex = 99;
            this.krpbSearch.Values.Text = "Search";
            this.krpbSearch.Click += new System.EventHandler(this.krpbSearch_Click);
            // 
            // krplProductName
            // 
            this.krplProductName.Location = new System.Drawing.Point(21, 24);
            this.krplProductName.Name = "krplProductName";
            this.krplProductName.Size = new System.Drawing.Size(89, 20);
            this.krplProductName.TabIndex = 1;
            this.krplProductName.Values.Text = "Product Name";
            // 
            // krptProductName
            // 
            this.krptProductName.Location = new System.Drawing.Point(138, 24);
            this.krptProductName.MaxLength = 12;
            this.krptProductName.Name = "krptProductName";
            this.krptProductName.Size = new System.Drawing.Size(203, 23);
            this.krptProductName.TabIndex = 1;
            this.krptProductName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptMemberNo_KeyDown);
            // 
            // krplBarcode
            // 
            this.krplBarcode.Location = new System.Drawing.Point(21, 62);
            this.krplBarcode.Name = "krplBarcode";
            this.krplBarcode.Size = new System.Drawing.Size(55, 20);
            this.krplBarcode.TabIndex = 1;
            this.krplBarcode.Values.Text = "Barcode";
            // 
            // krptBarcode
            // 
            this.krptBarcode.Location = new System.Drawing.Point(138, 62);
            this.krptBarcode.MaxLength = 12;
            this.krptBarcode.Name = "krptBarcode";
            this.krptBarcode.Size = new System.Drawing.Size(203, 23);
            this.krptBarcode.TabIndex = 5;
            this.krptBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krptMemberNo_KeyDown);
            // 
            // krplWarn
            // 
            this.krplWarn.Location = new System.Drawing.Point(21, 150);
            this.krplWarn.Name = "krplWarn";
            this.krplWarn.Size = new System.Drawing.Size(40, 20);
            this.krplWarn.TabIndex = 1;
            this.krplWarn.Values.Text = "Warn";
            // 
            // krpcbCount
            // 
            this.krpcbCount.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbCount.Location = new System.Drawing.Point(138, 150);
            this.krpcbCount.Name = "krpcbCount";
            this.krpcbCount.Size = new System.Drawing.Size(57, 20);
            this.krpcbCount.TabIndex = 10;
            this.krpcbCount.Text = "Count";
            this.krpcbCount.Values.Text = "Count";
            // 
            // krpcbTime
            // 
            this.krpcbTime.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbTime.Location = new System.Drawing.Point(264, 150);
            this.krpcbTime.Name = "krpcbTime";
            this.krpcbTime.Size = new System.Drawing.Size(50, 20);
            this.krpcbTime.TabIndex = 15;
            this.krpcbTime.Text = "Time";
            this.krpcbTime.Values.Text = "Time";
            // 
            // krplHideType
            // 
            this.krplHideType.Location = new System.Drawing.Point(21, 105);
            this.krplHideType.Name = "krplHideType";
            this.krplHideType.Size = new System.Drawing.Size(66, 20);
            this.krplHideType.TabIndex = 1;
            this.krplHideType.Values.Text = "Hide Type";
            // 
            // krpcmHideType
            // 
            this.krpcmHideType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpcmHideType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.krpcmHideType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcmHideType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcmHideType.DropDownWidth = 149;
            this.krpcmHideType.Location = new System.Drawing.Point(138, 105);
            this.krpcmHideType.Name = "krpcmHideType";
            this.krpcmHideType.Size = new System.Drawing.Size(203, 21);
            this.krpcmHideType.TabIndex = 100;
            // 
            // ProductConditionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(373, 245);
            this.Controls.Add(this.krpcmHideType);
            this.Controls.Add(this.krpcbTime);
            this.Controls.Add(this.krpcbCount);
            this.Controls.Add(this.krptBarcode);
            this.Controls.Add(this.krptProductName);
            this.Controls.Add(this.krplWarn);
            this.Controls.Add(this.krplHideType);
            this.Controls.Add(this.krplBarcode);
            this.Controls.Add(this.krplProductName);
            this.Controls.Add(this.krpbSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProductConditionWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ProductConditionWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpcmHideType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSearch;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBarcode;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptBarcode;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplWarn;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbCount;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplHideType;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcmHideType;
    }
}