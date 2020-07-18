namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class ProductWindow
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.krpdgList = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.krpcmEdit = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductName0 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductName1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductName2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductTypeName = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krpcmIsScales = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmBarcode = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPriceChangeMode = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmCostPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmCostPriceChangeMode = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmBalanceCount = new Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn();
            this.krpcmWarningCount = new Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn();
            this.krpcmIsBindCount = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmExpiredTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductParentName = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krpcmProductParentCount = new Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn();
            this.krpcmImageName = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPrinters = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krpcmOrder = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewNumericUpDownColumn();
            this.krpcmHideType = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewComboBoxColumn();
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
            this.krplProductName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAdd = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemSave = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemChangeTime = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemPrint = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemDelete = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.krptProductName = new Oybab.ServicePC.Tools.ComTextBox();
            this.krplProductTypeName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbProductType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krpcbIsDisplayAll = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbMultipleLanguage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.krplAllPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPriceSperator = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplAllCostPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbProductType)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
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
            this.krpcmProductName0,
            this.krpcmProductName1,
            this.krpcmProductName2,
            this.krpcmProductTypeName,
            this.krpcmIsScales,
            this.krpcmBarcode,
            this.krpcmPrice,
            this.krpcmPriceChangeMode,
            this.krpcmCostPrice,
            this.krpcmCostPriceChangeMode,
            this.krpcmBalanceCount,
            this.krpcmWarningCount,
            this.krpcmIsBindCount,
            this.krpcmExpiredTime,
            this.krpcmProductParentName,
            this.krpcmProductParentCount,
            this.krpcmImageName,
            this.krpcmPrinters,
            this.krpcmOrder,
            this.krpcmHideType});
            this.krpdgList.Location = new System.Drawing.Point(12, 35);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.Size = new System.Drawing.Size(903, 195);
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
            // krpcmProductId
            // 
            this.krpcmProductId.Frozen = true;
            this.krpcmProductId.HeaderText = "ProductdId";
            this.krpcmProductId.Name = "krpcmProductId";
            this.krpcmProductId.Visible = false;
            this.krpcmProductId.Width = 150;
            // 
            // krpcmProductNameZH
            // 
            this.krpcmProductName0.Frozen = true;
            this.krpcmProductName0.HeaderText = "ProductName0";
            this.krpcmProductName0.Name = "krpcmProductName0";
            this.krpcmProductName0.Width = 210;
            // 
            // krpcmProductNameUG
            // 
            this.krpcmProductName1.Frozen = true;
            this.krpcmProductName1.HeaderText = "ProductName1";
            this.krpcmProductName1.Name = "krpcmProductName1";
            this.krpcmProductName1.Width = 210;
            // 
            // krpcmProductNameEN
            // 
            this.krpcmProductName2.Frozen = true;
            this.krpcmProductName2.HeaderText = "ProductName2";
            this.krpcmProductName2.Name = "krpcmProductName2";
            this.krpcmProductName2.Width = 210;
            // 
            // krpcmProductTypeName
            // 
            this.krpcmProductTypeName.HeaderText = "ProductTypeName";
            this.krpcmProductTypeName.Name = "krpcmProductTypeName";
            this.krpcmProductTypeName.Width = 210;
            // 
            // krpcmIsScales
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = false;
            this.krpcmIsScales.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmIsScales.FalseValue = "0";
            this.krpcmIsScales.HeaderText = "IsScales";
            this.krpcmIsScales.IndeterminateValue = null;
            this.krpcmIsScales.Name = "krpcmIsScales";
            this.krpcmIsScales.TrueValue = "1";
            this.krpcmIsScales.Width = 150;
            // 
            // krpcmBarcode
            // 
            this.krpcmBarcode.HeaderText = "BarCode";
            this.krpcmBarcode.MaxInputLength = 20;
            this.krpcmBarcode.Name = "krpcmBarcode";
            this.krpcmBarcode.Width = 150;
            // 
            // krpcmPrice
            // 
            this.krpcmPrice.HeaderText = "Price";
            this.krpcmPrice.MaxInputLength = 10;
            this.krpcmPrice.Name = "krpcmPrice";
            this.krpcmPrice.Width = 150;
            // 
            // krpcmPriceChangeMode
            // 
            this.krpcmPriceChangeMode.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmPriceChangeMode.FalseValue = "0";
            this.krpcmPriceChangeMode.HeaderText = "PriceChangeMode";
            this.krpcmPriceChangeMode.IndeterminateValue = null;
            this.krpcmPriceChangeMode.Name = "krpcmPriceChangeMode";
            this.krpcmPriceChangeMode.TrueValue = "1";
            this.krpcmPriceChangeMode.Width = 150;
            // 
            // krpcmCostPrice
            // 
            this.krpcmCostPrice.HeaderText = "CostPrice";
            this.krpcmCostPrice.MaxInputLength = 10;
            this.krpcmCostPrice.Name = "krpcmCostPrice";
            this.krpcmCostPrice.Width = 150;
            // 
            // krpcmCostPriceChangeMode
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = false;
            this.krpcmCostPriceChangeMode.DefaultCellStyle = dataGridViewCellStyle3;
            this.krpcmCostPriceChangeMode.FalseValue = "0";
            this.krpcmCostPriceChangeMode.HeaderText = "CostPriceChangeMode";
            this.krpcmCostPriceChangeMode.IndeterminateValue = null;
            this.krpcmCostPriceChangeMode.Name = "krpcmCostPriceChangeMode";
            this.krpcmCostPriceChangeMode.TrueValue = "1";
            this.krpcmCostPriceChangeMode.Width = 150;
            // 
            // krpcmBalanceCount
            // 
            this.krpcmBalanceCount.DecimalPlaces = 3;
            this.krpcmBalanceCount.HeaderText = "BalanceCount";
            this.krpcmBalanceCount.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmBalanceCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.krpcmBalanceCount.Minimum = new decimal(new int[] {
            9999,
            0,
            0,
            -2147483648});
            this.krpcmBalanceCount.Name = "krpcmBalanceCount";
            this.krpcmBalanceCount.Width = 150;
            // 
            // krpcmWarningCount
            // 
            this.krpcmWarningCount.DecimalPlaces = 3;
            this.krpcmWarningCount.HeaderText = "WarningCount";
            this.krpcmWarningCount.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmWarningCount.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.krpcmWarningCount.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.krpcmWarningCount.Name = "krpcmWarningCount";
            this.krpcmWarningCount.Width = 150;
            // 
            // krpcmIsBindCount
            // 
            this.krpcmIsBindCount.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmIsBindCount.FalseValue = "0";
            this.krpcmIsBindCount.HeaderText = "IsBindCount";
            this.krpcmIsBindCount.IndeterminateValue = null;
            this.krpcmIsBindCount.Name = "krpcmIsBindCount";
            this.krpcmIsBindCount.TrueValue = "1";
            this.krpcmIsBindCount.Width = 150;
            // 
            // krpcmExpiredTime
            // 
            this.krpcmExpiredTime.HeaderText = "ExpiredTime";
            this.krpcmExpiredTime.Name = "krpcmExpiredTime";
            this.krpcmExpiredTime.ReadOnly = true;
            this.krpcmExpiredTime.Width = 150;
            // 
            // krpcmProductParentName
            // 
            this.krpcmProductParentName.HeaderText = "ProductParentName";
            this.krpcmProductParentName.Name = "krpcmProductParentName";
            this.krpcmProductParentName.Width = 210;
            // 
            // krpcmProductParentCount
            // 
            this.krpcmProductParentCount.DecimalPlaces = 3;
            this.krpcmProductParentCount.HeaderText = "ProductParentCount";
            this.krpcmProductParentCount.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmProductParentCount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.krpcmProductParentCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmProductParentCount.Name = "krpcmProductParentCount";
            this.krpcmProductParentCount.Width = 150;
            // 
            // krpcmImageName
            // 
            this.krpcmImageName.HeaderText = "ImageName";
            this.krpcmImageName.Name = "krpcmImageName";
            this.krpcmImageName.Width = 150;
            // 
            // krpcmPrinters
            // 
            this.krpcmPrinters.HeaderText = "Printers";
            this.krpcmPrinters.Name = "krpcmPrinters";
            this.krpcmPrinters.Width = 150;
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
            // krpcmHideType
            // 
            this.krpcmHideType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcmHideType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcmHideType.DropDownWidth = 121;
            this.krpcmHideType.HeaderText = "HideType";
            this.krpcmHideType.Name = "krpcmHideType";
            this.krpcmHideType.Width = 150;
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(890, 233);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(845, 236);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(778, 238);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 2;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(732, 238);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(708, 233);
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
            this.krpbNextPage.Location = new System.Drawing.Point(684, 233);
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
            this.krpbPrewPage.Location = new System.Drawing.Point(660, 233);
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
            this.krpbBeginPage.Location = new System.Drawing.Point(636, 233);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krpbSearch
            // 
            this.krpbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSearch.Location = new System.Drawing.Point(825, 5);
            this.krpbSearch.Name = "krpbSearch";
            this.krpbSearch.Size = new System.Drawing.Size(90, 25);
            this.krpbSearch.TabIndex = 2;
            this.krpbSearch.Values.Text = "Search";
            this.krpbSearch.Click += new System.EventHandler(this.krpbSearch_Click);
            this.krpbSearch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.krpbSearch_MouseUp);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(823, 236);
            this.krplSeperate.Name = "krplSeperate";
            this.krplSeperate.Size = new System.Drawing.Size(15, 20);
            this.krplSeperate.TabIndex = 8;
            this.krplSeperate.Values.Text = "/";
            // 
            // krplProductName
            // 
            this.krplProductName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplProductName.Location = new System.Drawing.Point(592, 7);
            this.krplProductName.Name = "krplProductName";
            this.krplProductName.Size = new System.Drawing.Size(86, 20);
            this.krplProductName.TabIndex = 10;
            this.krplProductName.Values.Text = "ProductName";
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
            this.kryptonContextMenuItemChangeTime,
            this.kryptonContextMenuItemPrint,
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
            // kryptonContextMenuItemChangeTime
            // 
            this.kryptonContextMenuItemChangeTime.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.kryptonContextMenuItemChangeTime.Text = "Menu Item";
            // 
            // kryptonContextMenuItemPrint
            // 
            this.kryptonContextMenuItemPrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.kryptonContextMenuItemPrint.Text = "Menu Item";
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
            // krptProductName
            // 
            this.krptProductName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptProductName.Location = new System.Drawing.Point(684, 7);
            this.krptProductName.Name = "krptProductName";
            this.krptProductName.Size = new System.Drawing.Size(135, 23);
            this.krptProductName.TabIndex = 0;
            this.krptProductName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // krplProductTypeName
            // 
            this.krplProductTypeName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplProductTypeName.Location = new System.Drawing.Point(278, 5);
            this.krplProductTypeName.Name = "krplProductTypeName";
            this.krplProductTypeName.Size = new System.Drawing.Size(112, 20);
            this.krplProductTypeName.TabIndex = 10;
            this.krplProductTypeName.Values.Text = "ProductTypeName";
            // 
            // krpcbProductType
            // 
            this.krpcbProductType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpcbProductType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.krpcbProductType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbProductType.DropDownWidth = 149;
            this.krpcbProductType.Location = new System.Drawing.Point(396, 5);
            this.krpcbProductType.Name = "krpcbProductType";
            this.krpcbProductType.Size = new System.Drawing.Size(168, 21);
            this.krpcbProductType.TabIndex = 11;
            this.krpcbProductType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
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
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.krplAllPrice);
            this.flowLayoutPanel1.Controls.Add(this.krplPriceSperator);
            this.flowLayoutPanel1.Controls.Add(this.krplAllCostPrice);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(258, 25);
            this.flowLayoutPanel1.TabIndex = 26;
            // 
            // krplAllPrice
            // 
            this.krplAllPrice.Location = new System.Drawing.Point(3, 3);
            this.krplAllPrice.Name = "krplAllPrice";
            this.krplAllPrice.Size = new System.Drawing.Size(30, 20);
            this.krplAllPrice.TabIndex = 9;
            this.krplAllPrice.Values.Text = "￥0";
            // 
            // krplPriceSperator
            // 
            this.krplPriceSperator.Location = new System.Drawing.Point(39, 3);
            this.krplPriceSperator.Name = "krplPriceSperator";
            this.krplPriceSperator.Size = new System.Drawing.Size(15, 20);
            this.krplPriceSperator.TabIndex = 11;
            this.krplPriceSperator.Values.Text = "/";
            // 
            // krplAllCostPrice
            // 
            this.krplAllCostPrice.Location = new System.Drawing.Point(60, 3);
            this.krplAllCostPrice.Name = "krplAllCostPrice";
            this.krplAllCostPrice.Size = new System.Drawing.Size(30, 20);
            this.krplAllCostPrice.TabIndex = 10;
            this.krplAllCostPrice.Values.Text = "￥0";
            // 
            // ProductWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(927, 262);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.krpcbMultipleLanguage);
            this.Controls.Add(this.krpcbIsDisplayAll);
            this.Controls.Add(this.krpcbProductType);
            this.Controls.Add(this.krplProductTypeName);
            this.Controls.Add(this.krplProductName);
            this.Controls.Add(this.krptProductName);
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
            this.Name = "ProductWindow";
            this.Text = "ProductWindow";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbProductType)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
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
        private Oybab.ServicePC.Tools.ComTextBox krptProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems3;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemSave;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemDelete;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemChangeTime;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemPrint;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmProductId;
        private Oybab.ServicePC.Tools.ComTextColumn krpcmProductTypeName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmProductName0;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmProductName1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmProductName2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmBarcode;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmPriceChangeMode;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmCostPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmCostPriceChangeMode;
        private Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn krpcmBalanceCount;
        private Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn krpcmWarningCount;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsBindCount;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsScales;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmImageName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmExpiredTime;
        private Oybab.ServicePC.Tools.ComTextColumn krpcmProductParentName;
        private Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn krpcmProductParentCount;
        private Oybab.ServicePC.Tools.ComTextColumn krpcmPrinters;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewComboBoxColumn krpcmHideType;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewNumericUpDownColumn krpcmOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplProductTypeName;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbProductType;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbIsDisplayAll;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbMultipleLanguage;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAllPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAllCostPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPriceSperator;
    }
}