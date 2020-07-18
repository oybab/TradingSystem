namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class FastGridWindow
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.krpdgList = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.krpcmEdit = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductName = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krpcmPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmCount = new Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn();
            this.krpcmTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpbClickToPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplPageCount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptCurrentPage = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplPage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbEngPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbNextPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbPrewPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbBeginPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplSeperate = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.krpcbProductType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplProductTypeName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpcbOnlyDisplaySelected = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krplIndexName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptIndexName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbProductType)).BeginInit();
            this.SuspendLayout();
            // 
            // krpdgList
            // 
            this.krpdgList.AllowUserToAddRows = false;
            this.krpdgList.AllowUserToDeleteRows = false;
            this.krpdgList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.krpdgList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.krpdgList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.krpcmEdit,
            this.krpcmProductId,
            this.krpcmProductName,
            this.krpcmPrice,
            this.krpcmCount,
            this.krpcmTotalPrice});
            this.krpdgList.Location = new System.Drawing.Point(12, 39);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.Size = new System.Drawing.Size(610, 410);
            this.krpdgList.TabIndex = 6;
            this.krpdgList.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.krpdgList_CellBeginEdit);
            this.krpdgList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.krpdgList_CellClick);
            this.krpdgList.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.krpdgList_CellEndEdit);
            this.krpdgList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.krpdgList_CellMouseClick);
            this.krpdgList.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.krpdgList_CellValidating);
            this.krpdgList.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.krpdgList_RowPostPaint);
            this.krpdgList.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.krpdgList_RowStateChanged);
            this.krpdgList.SelectionChanged += new System.EventHandler(this.krpdgList_SelectionChanged);
            this.krpdgList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krpdgList_KeyDown);
            this.krpdgList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krpdgList_KeyUp);
            this.krpdgList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.krpdgList_MouseClick);
            // 
            // krpcmEdit
            // 
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Red;
            this.krpcmEdit.DefaultCellStyle = dataGridViewCellStyle1;
            this.krpcmEdit.Frozen = true;
            this.krpcmEdit.HeaderText = "*";
            this.krpcmEdit.Name = "krpcmEdit";
            this.krpcmEdit.ReadOnly = true;
            this.krpcmEdit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.krpcmEdit.Width = 20;
            // 
            // krpcmProductId
            // 
            this.krpcmProductId.HeaderText = "krpcmProductId";
            this.krpcmProductId.Name = "krpcmProductId";
            this.krpcmProductId.ReadOnly = true;
            this.krpcmProductId.Visible = false;
            this.krpcmProductId.Width = 150;
            // 
            // krpcmProductName
            // 
            this.krpcmProductName.HeaderText = "ProductName";
            this.krpcmProductName.Name = "krpcmProductName";
            this.krpcmProductName.ReadOnly = true;
            this.krpcmProductName.Width = 210;
            // 
            // krpcmPrice
            // 
            this.krpcmPrice.HeaderText = "krpcmPrice";
            this.krpcmPrice.Name = "krpcmPrice";
            this.krpcmPrice.ReadOnly = true;
            this.krpcmPrice.Width = 115;
            // 
            // krpcmCount
            // 
            this.krpcmCount.DecimalPlaces = 3;
            this.krpcmCount.HeaderText = "Count";
            this.krpcmCount.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.krpcmCount.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.krpcmCount.Name = "krpcmCount";
            this.krpcmCount.Width = 100;
            // 
            // krpcmTotalPrice
            // 
            this.krpcmTotalPrice.HeaderText = "TotalPrice";
            this.krpcmTotalPrice.Name = "krpcmTotalPrice";
            this.krpcmTotalPrice.ReadOnly = true;
            this.krpcmTotalPrice.Width = 120;
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(597, 422);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(552, 425);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(485, 427);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 2;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(439, 427);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(415, 422);
            this.krpbEngPage.Name = "krpbEngPage";
            this.krpbEngPage.Size = new System.Drawing.Size(18, 25);
            this.krpbEngPage.TabIndex = 8;
            this.krpbEngPage.Values.Text = "";
            this.krpbEngPage.Click += new System.EventHandler(this.krpbEngPage_Click);
            // 
            // krpbNextPage
            // 
            this.krpbNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbNextPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.krpbNextPage.Location = new System.Drawing.Point(391, 422);
            this.krpbNextPage.Name = "krpbNextPage";
            this.krpbNextPage.Size = new System.Drawing.Size(18, 25);
            this.krpbNextPage.TabIndex = 7;
            this.krpbNextPage.Values.Text = "";
            this.krpbNextPage.Click += new System.EventHandler(this.krpbNextPage_Click);
            // 
            // krpbPrewPage
            // 
            this.krpbPrewPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbPrewPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.krpbPrewPage.Location = new System.Drawing.Point(367, 422);
            this.krpbPrewPage.Name = "krpbPrewPage";
            this.krpbPrewPage.Size = new System.Drawing.Size(18, 25);
            this.krpbPrewPage.TabIndex = 6;
            this.krpbPrewPage.Values.Text = "";
            this.krpbPrewPage.Click += new System.EventHandler(this.krpbPrewPage_Click);
            // 
            // krpbBeginPage
            // 
            this.krpbBeginPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbBeginPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.krpbBeginPage.Location = new System.Drawing.Point(343, 422);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(530, 425);
            this.krplSeperate.Name = "krplSeperate";
            this.krplSeperate.Size = new System.Drawing.Size(15, 20);
            this.krplSeperate.TabIndex = 8;
            this.krplSeperate.Values.Text = "/";
            // 
            // kryptonContextMenuItem1
            // 
            this.kryptonContextMenuItem1.Text = "Menu Item";
            // 
            // krpcbProductType
            // 
            this.krpcbProductType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpcbProductType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.krpcbProductType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbProductType.DropDownWidth = 149;
            this.krpcbProductType.Location = new System.Drawing.Point(454, 12);
            this.krpcbProductType.Name = "krpcbProductType";
            this.krpcbProductType.Size = new System.Drawing.Size(168, 21);
            this.krpcbProductType.TabIndex = 13;
            this.krpcbProductType.SelectedIndexChanged += new System.EventHandler(this.krpcbProductType_SelectedIndexChanged);
            // 
            // krplProductTypeName
            // 
            this.krplProductTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplProductTypeName.Location = new System.Drawing.Point(336, 12);
            this.krplProductTypeName.Name = "krplProductTypeName";
            this.krplProductTypeName.Size = new System.Drawing.Size(112, 20);
            this.krplProductTypeName.TabIndex = 12;
            this.krplProductTypeName.Values.Text = "ProductTypeName";
            // 
            // krpbAdd
            // 
            this.krpbAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpbAdd.Location = new System.Drawing.Point(253, 455);
            this.krpbAdd.Name = "krpbAdd";
            this.krpbAdd.Size = new System.Drawing.Size(90, 25);
            this.krpbAdd.TabIndex = 14;
            this.krpbAdd.Values.Text = "Add";
            this.krpbAdd.Click += new System.EventHandler(this.krpbAdd_Click);
            // 
            // krpcbOnlyDisplaySelected
            // 
            this.krpcbOnlyDisplaySelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpcbOnlyDisplaySelected.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbOnlyDisplaySelected.Location = new System.Drawing.Point(12, 456);
            this.krpcbOnlyDisplaySelected.Name = "krpcbOnlyDisplaySelected";
            this.krpcbOnlyDisplaySelected.Size = new System.Drawing.Size(134, 20);
            this.krpcbOnlyDisplaySelected.TabIndex = 15;
            this.krpcbOnlyDisplaySelected.Text = "OnlyDisplaySelected";
            this.krpcbOnlyDisplaySelected.Values.Text = "OnlyDisplaySelected";
            this.krpcbOnlyDisplaySelected.CheckedChanged += new System.EventHandler(this.krpcbOnlyDisplaySelected_CheckedChanged);
            // 
            // krplIndexName
            // 
            this.krplIndexName.Location = new System.Drawing.Point(12, 12);
            this.krplIndexName.Name = "krplIndexName";
            this.krplIndexName.Size = new System.Drawing.Size(76, 20);
            this.krplIndexName.TabIndex = 16;
            this.krplIndexName.Values.Text = "Index Name";
            // 
            // krptIndexName
            // 
            this.krptIndexName.Location = new System.Drawing.Point(107, 12);
            this.krptIndexName.Name = "krptIndexName";
            this.krptIndexName.Size = new System.Drawing.Size(127, 23);
            this.krptIndexName.TabIndex = 17;
            this.krptIndexName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptIndexName_KeyUp);
            // 
            // FastGridWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(634, 486);
            this.Controls.Add(this.krptIndexName);
            this.Controls.Add(this.krplIndexName);
            this.Controls.Add(this.krpcbOnlyDisplaySelected);
            this.Controls.Add(this.krpbAdd);
            this.Controls.Add(this.krpcbProductType);
            this.Controls.Add(this.krplProductTypeName);
            this.Controls.Add(this.krpdgList);
            this.Controls.Add(this.krplSeperate);
            this.Controls.Add(this.krplPage);
            this.Controls.Add(this.krptCurrentPage);
            this.Controls.Add(this.krplPageCount);
            this.Controls.Add(this.krpbBeginPage);
            this.Controls.Add(this.krpbPrewPage);
            this.Controls.Add(this.krpbNextPage);
            this.Controls.Add(this.krpbEngPage);
            this.Controls.Add(this.krpbClickToPage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 300);
            this.Name = "FastGridWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FastGridWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbProductType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView krpdgList;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbClickToPage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPageCount;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptCurrentPage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbEngPage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbNextPage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPrewPage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbBeginPage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSeperate;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbProductType;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplProductTypeName;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmProductId;
        private Tools.ComTextColumn krpcmProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPrice;
        private Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn krpcmCount;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbOnlyDisplaySelected;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplIndexName;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptIndexName;
        
    }
}