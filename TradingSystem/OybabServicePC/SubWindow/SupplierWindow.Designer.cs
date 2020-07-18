namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class SupplierWindow
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
            this.krpcmSupplierId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmSupplierNo = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmCardNo = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmSupplierName0 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmSupplierName1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmSupplierName2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmSex = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewComboBoxColumn();
            this.krpcmOccupation = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewComboBoxColumn();
            this.krpcmMobile = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPhone = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmIDNumber = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAddress0 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAddress1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAddress2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmOfferRate = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewNumericUpDownColumn();
            this.krpcmBalancePrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmSpendPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmFavorablePrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmIsAllowBorrow = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmExpiredTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmIsEnable = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmRemark = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
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
            this.krplSupplierNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAdd = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemSave = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemPay = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemChangeTime = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemDelete = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.krptSupplierNo = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
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
            this.krpcmSupplierId,
            this.krpcmSupplierNo,
            this.krpcmCardNo,
            this.krpcmSupplierName0,
            this.krpcmSupplierName1,
            this.krpcmSupplierName2,
            this.krpcmSex,
            this.krpcmOccupation,
            this.krpcmMobile,
            this.krpcmPhone,
            this.krpcmIDNumber,
            this.krpcmAddress0,
            this.krpcmAddress1,
            this.krpcmAddress2,
            this.krpcmOfferRate,
            this.krpcmBalancePrice,
            this.krpcmSpendPrice,
            this.krpcmFavorablePrice,
            this.krpcmIsAllowBorrow,
            this.krpcmExpiredTime,
            this.krpcmIsEnable,
            this.krpcmRemark});
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
            // krpcmSupplierId
            // 
            this.krpcmSupplierId.Frozen = true;
            this.krpcmSupplierId.HeaderText = "SupplierId";
            this.krpcmSupplierId.Name = "krpcmSupplierId";
            this.krpcmSupplierId.Visible = false;
            this.krpcmSupplierId.Width = 150;
            // 
            // krpcmSupplierNo
            // 
            this.krpcmSupplierNo.Frozen = true;
            this.krpcmSupplierNo.HeaderText = "SupplierNo";
            this.krpcmSupplierNo.Name = "krpcmSupplierNo";
            this.krpcmSupplierNo.Width = 210;
            // 
            // krpcmCardNo
            // 
            this.krpcmCardNo.HeaderText = "CardNo";
            this.krpcmCardNo.Name = "krpcmCardNo";
            this.krpcmCardNo.Width = 150;
            this.krpcmCardNo.ReadOnly = true;

            // 
            // krpcmSupplierNameZH
            // 
            this.krpcmSupplierName0.HeaderText = "SupplierName0";
            this.krpcmSupplierName0.Name = "krpcmSupplierName0";
            this.krpcmSupplierName0.Width = 210;
            // 
            // krpcmSupplierNameUG
            // 
            this.krpcmSupplierName1.HeaderText = "SupplierName1";
            this.krpcmSupplierName1.Name = "krpcmSupplierName1";
            this.krpcmSupplierName1.Width = 210;
            // 
            // krpcmSupplierNameEN
            // 
            this.krpcmSupplierName2.HeaderText = "SupplierName2";
            this.krpcmSupplierName2.Name = "krpcmSupplierName2";
            this.krpcmSupplierName2.Width = 210;
            // 
            // krpcmSex
            // 
            this.krpcmSex.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcmSex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcmSex.DropDownWidth = 90;
            this.krpcmSex.HeaderText = "Sex";
            this.krpcmSex.Name = "krpcmSex";
            this.krpcmSex.Width = 100;
            // 
            // krpcmOccupation
            // 
            this.krpcmOccupation.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcmOccupation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcmOccupation.DropDownWidth = 121;
            this.krpcmOccupation.HeaderText = "Occupation";
            this.krpcmOccupation.Name = "krpcmOccupation";
            this.krpcmOccupation.Visible = false;
            this.krpcmOccupation.Width = 150;
            // 
            // krpcmMobile
            // 
            this.krpcmMobile.HeaderText = "Mobile";
            this.krpcmMobile.Name = "krpcmMobile";
            this.krpcmMobile.Width = 150;
            // 
            // krpcmPhone
            // 
            this.krpcmPhone.HeaderText = "Phone";
            this.krpcmPhone.Name = "krpcmPhone";
            this.krpcmPhone.Width = 210;
            // 
            // krpcmIDNumber
            // 
            this.krpcmIDNumber.HeaderText = "krpcmID";
            this.krpcmIDNumber.Name = "krpcmIDNumber";
            this.krpcmIDNumber.Width = 210;
            // 
            // krpcmAddressZH
            // 
            this.krpcmAddress0.HeaderText = "Address0";
            this.krpcmAddress0.Name = "krpcmAddress0";
            this.krpcmAddress0.Width = 210;
            // 
            // krpcmAddressUG
            // 
            this.krpcmAddress1.HeaderText = "Address1";
            this.krpcmAddress1.Name = "krpcmAddress1";
            this.krpcmAddress1.Width = 210;
            // 
            // krpcmAddressEN
            // 
            this.krpcmAddress2.HeaderText = "Address2";
            this.krpcmAddress2.Name = "krpcmAddress2";
            this.krpcmAddress2.Width = 210;
            // 
            // krpcmOfferRate
            // 
            this.krpcmOfferRate.HeaderText = "OfferRate";
            this.krpcmOfferRate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.krpcmOfferRate.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.krpcmOfferRate.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.krpcmOfferRate.Name = "krpcmOfferRate";
            this.krpcmOfferRate.Width = 150;
            // 
            // krpcmBalancePrice
            // 
            this.krpcmBalancePrice.HeaderText = "BalancePrice";
            this.krpcmBalancePrice.Name = "krpcmBalancePrice";
            this.krpcmBalancePrice.ReadOnly = true;
            this.krpcmBalancePrice.Width = 210;
            // 
            // krpcmSpendPrice
            // 
            this.krpcmSpendPrice.HeaderText = "SpendPrice";
            this.krpcmSpendPrice.Name = "krpcmSpendPrice";
            this.krpcmSpendPrice.ReadOnly = true;
            this.krpcmSpendPrice.Width = 210;
            // 
            // krpcmFavorablePrice
            // 
            this.krpcmFavorablePrice.HeaderText = "FavorablePrice";
            this.krpcmFavorablePrice.Name = "krpcmFavorablePrice";
            this.krpcmFavorablePrice.ReadOnly = true;
            this.krpcmFavorablePrice.Width = 210;
            // 
            // krpcmIsAllowBorrow
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = false;
            this.krpcmIsAllowBorrow.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmIsAllowBorrow.FalseValue = "0";
            this.krpcmIsAllowBorrow.HeaderText = "IsAllowBorrow";
            this.krpcmIsAllowBorrow.IndeterminateValue = null;
            this.krpcmIsAllowBorrow.Name = "krpcmIsAllowBorrow";
            this.krpcmIsAllowBorrow.TrueValue = "1";
            this.krpcmIsAllowBorrow.Width = 150;
            // 
            // krpcmExpiredTime
            // 
            this.krpcmExpiredTime.HeaderText = "ExpiredTime";
            this.krpcmExpiredTime.Name = "krpcmExpiredTime";
            this.krpcmExpiredTime.ReadOnly = true;
            this.krpcmExpiredTime.Width = 210;
            // 
            // krpcmIsEnable
            // 
            this.krpcmIsEnable.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmIsEnable.FalseValue = "0";
            this.krpcmIsEnable.HeaderText = "IsEnable";
            this.krpcmIsEnable.IndeterminateValue = null;
            this.krpcmIsEnable.Name = "krpcmIsEnable";
            this.krpcmIsEnable.TrueValue = "1";
            this.krpcmIsEnable.Width = 150;
            // 
            // krpcmRemark
            // 
            this.krpcmRemark.HeaderText = "Remark";
            this.krpcmRemark.Name = "krpcmRemark";
            this.krpcmRemark.Width = 210;
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
            // krplSupplierNo
            // 
            this.krplSupplierNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSupplierNo.Location = new System.Drawing.Point(313, 7);
            this.krplSupplierNo.Name = "krplSupplierNo";
            this.krplSupplierNo.Size = new System.Drawing.Size(72, 20);
            this.krplSupplierNo.TabIndex = 10;
            this.krplSupplierNo.Values.Text = "SupplierNo";
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
            this.kryptonContextMenuItemPay,
            this.kryptonContextMenuItemChangeTime,
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
            // kryptonContextMenuItemPay
            // 
            this.kryptonContextMenuItemPay.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.kryptonContextMenuItemPay.Text = "Menu Item";
            // 
            // kryptonContextMenuItemChangeTime
            // 
            this.kryptonContextMenuItemChangeTime.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.kryptonContextMenuItemChangeTime.Text = "Menu Item";
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
            // krptSupplierNo
            // 
            this.krptSupplierNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptSupplierNo.Location = new System.Drawing.Point(391, 7);
            this.krptSupplierNo.Name = "krptSupplierNo";
            this.krptSupplierNo.Size = new System.Drawing.Size(135, 23);
            this.krptSupplierNo.TabIndex = 0;
            this.krptSupplierNo.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // krpcbIsDisplayAll
            // 
            this.krpcbIsDisplayAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpcbIsDisplayAll.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbIsDisplayAll.Location = new System.Drawing.Point(12, 236);
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
            this.krpcbMultipleLanguage.Location = new System.Drawing.Point(148, 236);
            this.krpcbMultipleLanguage.Name = "krpcbMultipleLanguage";
            this.krpcbMultipleLanguage.Size = new System.Drawing.Size(122, 20);
            this.krpcbMultipleLanguage.TabIndex = 25;
            this.krpcbMultipleLanguage.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.Values.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.CheckedChanged += new System.EventHandler(this.krpcbMultipleLanguage_CheckedChanged);
            // 
            // SupplierWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F); this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(634, 262);
            this.Controls.Add(this.krpcbMultipleLanguage);
            this.Controls.Add(this.krpcbIsDisplayAll);
            this.Controls.Add(this.krptSupplierNo);
            this.Controls.Add(this.krplSupplierNo);
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
            this.Name = "SupplierWindow";
            this.Text = "SupplierWindow";
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
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptSupplierNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSupplierNo;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems3;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemSave;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemDelete;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemChangeTime;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemPay;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmSupplierId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmSupplierNo;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmCardNo;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmSupplierName0;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmSupplierName1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmSupplierName2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewComboBoxColumn krpcmSex;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewComboBoxColumn krpcmOccupation;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmMobile;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPhone;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmIDNumber;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddress0;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddress1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddress2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewNumericUpDownColumn krpcmOfferRate;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmBalancePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmSpendPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmFavorablePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmExpiredTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsAllowBorrow;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsEnable;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRemark;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbIsDisplayAll;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbMultipleLanguage;
        
    }
}