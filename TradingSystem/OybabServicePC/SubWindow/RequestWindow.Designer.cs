namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class RequestWindow
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.krpdgList = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.krpcmEdit = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRequestId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRequestName0 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRequestName1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRequestName2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmOrder = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewNumericUpDownColumn();
            this.krpcmIsEnable = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpbClickToPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplPageCount = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptCurrentPage = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.krplPage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbEngPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbNextPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbPrewPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbBeginPage = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbSearch = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplSeperate = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRequestName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAdd = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemSave = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemDelete = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.krptRequestName = new Oybab.ServicePC.Tools.ComTextBox();
            this.krpcbIsDisplayAll = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbMultipleLanguage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
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
            this.krpcmRequestId,
            this.krpcmRequestName0,
            this.krpcmRequestName1,
            this.krpcmRequestName2,
            this.krpcmOrder,
            this.krpcmIsEnable});
            this.krpdgList.Location = new System.Drawing.Point(12, 35);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.Size = new System.Drawing.Size(610, 195);
            this.krpdgList.TabIndex = 9;
            this.krpdgList.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.krpdgList_CellBeginEdit);
            this.krpdgList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.krpdgList_CellClick);
            this.krpdgList.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.krpdgList_CellEndEdit);
            this.krpdgList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.krpdgList_CellMouseClick);
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
            // krpcmRequestId
            // 
            this.krpcmRequestId.Frozen = true;
            this.krpcmRequestId.HeaderText = "RequestId";
            this.krpcmRequestId.Name = "krpcmRequestId";
            this.krpcmRequestId.Visible = false;
            this.krpcmRequestId.Width = 150;
            // 
            // krpcmRequestName0
            // 
            this.krpcmRequestName0.Frozen = true;
            this.krpcmRequestName0.HeaderText = "RequestName0";
            this.krpcmRequestName0.Name = "krpcmRequestName0";
            this.krpcmRequestName0.Width = 210;
            // 
            // krpcmRequestName1
            // 
            this.krpcmRequestName1.Frozen = true;
            this.krpcmRequestName1.HeaderText = "RequestName1";
            this.krpcmRequestName1.Name = "krpcmRequestName1";
            this.krpcmRequestName1.Width = 210;
            // 
            // krpcmRequestName2
            // 
            this.krpcmRequestName2.Frozen = true;
            this.krpcmRequestName2.HeaderText = "RequestName2";
            this.krpcmRequestName2.Name = "krpcmRequestName2";
            this.krpcmRequestName2.Width = 210;
            // 
            // krpcmOrder
            // 
            this.krpcmOrder.HeaderText = "Order";
            this.krpcmOrder.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmOrder.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.krpcmOrder.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.krpcmOrder.Name = "krpcmOrder";
            this.krpcmOrder.Width = 100;
            // 
            // krpcmIsEnable
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = false;
            this.krpcmIsEnable.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmIsEnable.FalseValue = "0";
            this.krpcmIsEnable.HeaderText = "IsEnable";
            this.krpcmIsEnable.IndeterminateValue = null;
            this.krpcmIsEnable.Name = "krpcmIsEnable";
            this.krpcmIsEnable.TrueValue = "1";
            this.krpcmIsEnable.Width = 150;
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(597, 233);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(552, 236);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(485, 238);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 2;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(439, 238);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(415, 233);
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
            this.krpbNextPage.Location = new System.Drawing.Point(391, 233);
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
            this.krpbPrewPage.Location = new System.Drawing.Point(367, 233);
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
            this.krpbBeginPage.Location = new System.Drawing.Point(343, 233);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krpbSearch
            // 
            this.krpbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSearch.Location = new System.Drawing.Point(532, 5);
            this.krpbSearch.Name = "krpbSearch";
            this.krpbSearch.Size = new System.Drawing.Size(90, 25);
            this.krpbSearch.TabIndex = 2;
            this.krpbSearch.Values.Text = "Search";
            this.krpbSearch.Click += new System.EventHandler(this.krpbSearch_Click);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(530, 236);
            this.krplSeperate.Name = "krplSeperate";
            this.krplSeperate.Size = new System.Drawing.Size(15, 20);
            this.krplSeperate.TabIndex = 8;
            this.krplSeperate.Values.Text = "/";
            // 
            // krplRequestName
            // 
            this.krplRequestName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRequestName.Location = new System.Drawing.Point(298, 7);
            this.krplRequestName.Name = "krplRequestName";
            this.krplRequestName.Size = new System.Drawing.Size(87, 20);
            this.krplRequestName.TabIndex = 10;
            this.krplRequestName.Values.Text = "RequestName";
            // 
            // krpContextMenu
            // 
            this.krpContextMenu.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItems3});
            // 
            // kryptonContextMenuItems3
            // 
            this.kryptonContextMenuItems3.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItemAdd,
            this.kryptonContextMenuItemSave,
            this.kryptonContextMenuItemDelete});
            this.kryptonContextMenuItems3.StandardStyle = false;
            // 
            // kryptonContextMenuItemAdd
            // 
            this.kryptonContextMenuItemAdd.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.kryptonContextMenuItemAdd.Text = "Menu Item";
            // 
            // kryptonContextMenuItemSave
            // 
            this.kryptonContextMenuItemSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.kryptonContextMenuItemSave.Text = "Menu Item";
            // 
            // kryptonContextMenuItemDelete
            // 
            this.kryptonContextMenuItemDelete.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.kryptonContextMenuItemDelete.Text = "Menu Item";
            // 
            // kryptonContextMenuItem1
            // 
            this.kryptonContextMenuItem1.Text = "Menu Item";
            // 
            // krptRequestName
            // 
            this.krptRequestName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptRequestName.Location = new System.Drawing.Point(391, 7);
            this.krptRequestName.Name = "krptRequestName";
            this.krptRequestName.Size = new System.Drawing.Size(135, 23);
            this.krptRequestName.TabIndex = 0;
            this.krptRequestName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // krpcbIsDisplayAll
            // 
            this.krpcbIsDisplayAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpcbIsDisplayAll.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbIsDisplayAll.Location = new System.Drawing.Point(12, 238);
            this.krpcbIsDisplayAll.Name = "krpcbIsDisplayAll";
            this.krpcbIsDisplayAll.Size = new System.Drawing.Size(85, 20);
            this.krpcbIsDisplayAll.TabIndex = 24;
            this.krpcbIsDisplayAll.Text = "IsDisplayAll";
            this.krpcbIsDisplayAll.Values.Text = "IsDisplayAll";
            this.krpcbIsDisplayAll.CheckedChanged += new System.EventHandler(this.krpcbIsDisplayAll_CheckedChanged);
            // 
            // krpcbMultipleLanguage
            // 
            this.krpcbMultipleLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpcbMultipleLanguage.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbMultipleLanguage.Location = new System.Drawing.Point(148, 238);
            this.krpcbMultipleLanguage.Name = "krpcbMultipleLanguage";
            this.krpcbMultipleLanguage.Size = new System.Drawing.Size(122, 20);
            this.krpcbMultipleLanguage.TabIndex = 25;
            this.krpcbMultipleLanguage.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.Values.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.CheckedChanged += new System.EventHandler(this.krpcbMultipleLanguage_CheckedChanged);
            // 
            // RequestWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(634, 262);
            this.Controls.Add(this.krpcbMultipleLanguage);
            this.Controls.Add(this.krpcbIsDisplayAll);
            this.Controls.Add(this.krptRequestName);
            this.Controls.Add(this.krplRequestName);
            this.Controls.Add(this.krplSeperate);
            this.Controls.Add(this.krpbSearch);
            this.Controls.Add(this.krplPage);
            this.Controls.Add(this.krptCurrentPage);
            this.Controls.Add(this.krplPageCount);
            this.Controls.Add(this.krpbBeginPage);
            this.Controls.Add(this.krpbPrewPage);
            this.Controls.Add(this.krpbNextPage);
            this.Controls.Add(this.krpbEngPage);
            this.Controls.Add(this.krpbClickToPage);
            this.Controls.Add(this.krpdgList);
            this.MinimumSize = new System.Drawing.Size(650, 300);
            this.Name = "RequestWindow";
            this.Text = "RequestWindow";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
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
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSearch;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSeperate;
        private Oybab.ServicePC.Tools.ComTextBox krptRequestName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRequestName;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems3;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemSave;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemDelete;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRequestId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRequestName0;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRequestName1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRequestName2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewNumericUpDownColumn krpcmOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsEnable;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbIsDisplayAll;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbMultipleLanguage;
        
        
    }
}