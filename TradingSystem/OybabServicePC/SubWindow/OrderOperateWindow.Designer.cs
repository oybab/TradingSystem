namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class OrderOperateWindow
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
            this.krpcmOrderDetailId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductName = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krpcmPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmCount = new Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn();
            this.krpcmTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmIsPack = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmState = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRequest = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krpcmAddTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
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
            this.kryptonContextMenuItems3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAdd = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemRequest = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemDelete = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemHistory = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.krplTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBalancePrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplSperator2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBalancePriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplEndTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRemark = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptbRemark = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplEndTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnEndTimeAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnEndTimeSub = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplEndTimeChange = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPriceChange = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnPaidPriceAdd = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnPaidPriceSub = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbConfirm = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplRemarkChange = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbSave = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplLanguage = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbLanguage = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel5 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.krpbCheckout = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krplRoomPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel8 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbPackage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpbAddByBarcode = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbAddByFastGrid = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.comTextColumn1 = new Oybab.ServicePC.Tools.ComTextColumn();
            this.krplRoomNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel7 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomNoValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpbRefresh = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbLanguage)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
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
            this.krpcmOrderDetailId,
            this.krpcmProductName,
            this.krpcmPrice,
            this.krpcmCount,
            this.krpcmTotalPrice,
            this.krpcmIsPack,
            this.krpcmState,
            this.krpcmRequest,
            this.krpcmAddTime});
            this.krpdgList.Location = new System.Drawing.Point(12, 116);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.Size = new System.Drawing.Size(929, 290);
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
            // krpcmOrderDetailId
            // 
            this.krpcmOrderDetailId.HeaderText = "OrderDetailId";
            this.krpcmOrderDetailId.Name = "krpcmOrderDetailId";
            this.krpcmOrderDetailId.ReadOnly = true;
            this.krpcmOrderDetailId.Visible = false;
            this.krpcmOrderDetailId.Width = 150;
            // 
            // krpcmProductName
            // 
            this.krpcmProductName.HeaderText = "ProductName";
            this.krpcmProductName.Name = "krpcmProductName";
            this.krpcmProductName.Width = 210;
            // 
            // krpcmPrice
            // 
            this.krpcmPrice.HeaderText = "krpcmPrice";
            this.krpcmPrice.Name = "krpcmPrice";
            this.krpcmPrice.Width = 110;
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
            // krpcmIsPack
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = false;
            this.krpcmIsPack.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmIsPack.FalseValue = "0";
            this.krpcmIsPack.HeaderText = "IsPack";
            this.krpcmIsPack.IndeterminateValue = null;
            this.krpcmIsPack.Name = "krpcmIsPack";
            this.krpcmIsPack.TrueValue = "1";
            // 
            // krpcmState
            // 
            this.krpcmState.HeaderText = "State";
            this.krpcmState.Name = "krpcmState";
            this.krpcmState.ReadOnly = true;
            this.krpcmState.Visible = false;
            this.krpcmState.Width = 120;
            // 
            // krpcmRequest
            // 
            this.krpcmRequest.HeaderText = "Request";
            this.krpcmRequest.Name = "krpcmRequest";
            this.krpcmRequest.Width = 120;
            // 
            // krpcmAddTime
            // 
            this.krpcmAddTime.HeaderText = "AddTime";
            this.krpcmAddTime.Name = "krpcmAddTime";
            this.krpcmAddTime.ReadOnly = true;
            this.krpcmAddTime.Width = 150;
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(914, 368);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(869, 371);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(802, 373);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 2;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(756, 373);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(732, 368);
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
            this.krpbNextPage.Location = new System.Drawing.Point(708, 368);
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
            this.krpbPrewPage.Location = new System.Drawing.Point(684, 368);
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
            this.krpbBeginPage.Location = new System.Drawing.Point(660, 368);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(847, 371);
            this.krplSeperate.Name = "krplSeperate";
            this.krplSeperate.Size = new System.Drawing.Size(15, 20);
            this.krplSeperate.TabIndex = 8;
            this.krplSeperate.Values.Text = "/";
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
            this.kryptonContextMenuItemRequest,
            this.kryptonContextMenuItemDelete,
            this.kryptonContextMenuItemHistory});
            this.kryptonContextMenuItems3.StandardStyle = false;
            // 
            // kryptonContextMenuItemAdd
            // 
            this.kryptonContextMenuItemAdd.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.kryptonContextMenuItemAdd.Text = "Menu Item";
            // 
            // kryptonContextMenuItemRequest
            // 
            this.kryptonContextMenuItemRequest.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.kryptonContextMenuItemRequest.Text = "Menu Item";
            // 
            // kryptonContextMenuItemDelete
            // 
            this.kryptonContextMenuItemDelete.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.kryptonContextMenuItemDelete.Text = "Menu Item";
            // 
            // kryptonContextMenuItemHistory
            // 
            this.kryptonContextMenuItemHistory.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.kryptonContextMenuItemHistory.Text = "Menu Item";
            
            // 
            // kryptonContextMenuItem1
            // 
            this.kryptonContextMenuItem1.Text = "Menu Item";
            // 
            // krplTotalPrice
            // 
            this.krplTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPrice.Location = new System.Drawing.Point(44, 10);
            this.krplTotalPrice.Name = "krplTotalPrice";
            this.krplTotalPrice.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPrice.TabIndex = 10;
            this.krplTotalPrice.Values.Text = "TotalPrice";
            // 
            // krplTotalPriceValue
            // 
            this.krplTotalPriceValue.Location = new System.Drawing.Point(118, 10);
            this.krplTotalPriceValue.Name = "krplTotalPriceValue";
            this.krplTotalPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPriceValue.TabIndex = 10;
            this.krplTotalPriceValue.Values.Text = "00000000";
            // 
            // krplPaidPrice
            // 
            this.krplPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPaidPrice.Location = new System.Drawing.Point(268, 10);
            this.krplPaidPrice.Name = "krplPaidPrice";
            this.krplPaidPrice.Size = new System.Drawing.Size(60, 20);
            this.krplPaidPrice.TabIndex = 10;
            this.krplPaidPrice.Values.Text = "PaidPrice";
            // 
            // krplBalancePrice
            // 
            this.krplBalancePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBalancePrice.Location = new System.Drawing.Point(566, 10);
            this.krplBalancePrice.Name = "krplBalancePrice";
            this.krplBalancePrice.Size = new System.Drawing.Size(79, 20);
            this.krplBalancePrice.TabIndex = 10;
            this.krplBalancePrice.Values.Text = "BalancePrice";
            // 
            // krplSperator2
            // 
            this.krplSperator2.Location = new System.Drawing.Point(326, 80);
            this.krplSperator2.Name = "krplSperator2";
            this.krplSperator2.Size = new System.Drawing.Size(13, 20);
            this.krplSperator2.TabIndex = 10;
            this.krplSperator2.Values.Text = ":";
            // 
            // krplBalancePriceValue
            // 
            this.krplBalancePriceValue.Location = new System.Drawing.Point(654, 10);
            this.krplBalancePriceValue.Name = "krplBalancePriceValue";
            this.krplBalancePriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplBalancePriceValue.TabIndex = 10;
            this.krplBalancePriceValue.Values.Text = "00000000";
            // 
            // krplEndTime
            // 
            this.krplEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplEndTime.Location = new System.Drawing.Point(271, 80);
            this.krplEndTime.Name = "krplEndTime";
            this.krplEndTime.Size = new System.Drawing.Size(58, 20);
            this.krplEndTime.TabIndex = 13;
            this.krplEndTime.Values.Text = "EndTime";
            // 
            // krplRemark
            // 
            this.krplRemark.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRemark.Location = new System.Drawing.Point(588, 77);
            this.krplRemark.Name = "krplRemark";
            this.krplRemark.Size = new System.Drawing.Size(52, 20);
            this.krplRemark.TabIndex = 10;
            this.krplRemark.Values.Text = "Remark";
            // 
            // krptbRemark
            // 
            this.krptbRemark.Location = new System.Drawing.Point(651, 77);
            this.krptbRemark.Name = "krptbRemark";
            this.krptbRemark.Size = new System.Drawing.Size(189, 23);
            this.krptbRemark.TabIndex = 5;
            this.krptbRemark.TextChanged += new System.EventHandler(this.krptbRemark_TextChanged);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(635, 77);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 10;
            this.kryptonLabel1.Values.Text = ":";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(105, 10);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel2.TabIndex = 10;
            this.kryptonLabel2.Values.Text = ":";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(327, 10);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel3.TabIndex = 10;
            this.kryptonLabel3.Values.Text = ":";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(641, 10);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel4.TabIndex = 10;
            this.kryptonLabel4.Values.Text = ":";
            // 
            // krplEndTimeValue
            // 
            this.krplEndTimeValue.Location = new System.Drawing.Point(3, 6);
            this.krplEndTimeValue.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplEndTimeValue.Name = "krplEndTimeValue";
            this.krplEndTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplEndTimeValue.TabIndex = 17;
            this.krplEndTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // btnEndTimeAdd
            // 
            this.btnEndTimeAdd.Location = new System.Drawing.Point(124, 3);
            this.btnEndTimeAdd.Name = "btnEndTimeAdd";
            this.btnEndTimeAdd.Size = new System.Drawing.Size(28, 25);
            this.btnEndTimeAdd.TabIndex = 3;
            this.btnEndTimeAdd.Values.Text = "+";
            this.btnEndTimeAdd.Click += new System.EventHandler(this.btnEndTimeAdd_Click);
            // 
            // btnEndTimeSub
            // 
            this.btnEndTimeSub.Location = new System.Drawing.Point(158, 3);
            this.btnEndTimeSub.Name = "btnEndTimeSub";
            this.btnEndTimeSub.Size = new System.Drawing.Size(28, 25);
            this.btnEndTimeSub.TabIndex = 4;
            this.btnEndTimeSub.Values.Text = "-";
            this.btnEndTimeSub.Click += new System.EventHandler(this.btnEndTimeSub_Click);
            // 
            // krplEndTimeChange
            // 
            this.krplEndTimeChange.Location = new System.Drawing.Point(102, 6);
            this.krplEndTimeChange.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplEndTimeChange.Name = "krplEndTimeChange";
            this.krplEndTimeChange.Size = new System.Drawing.Size(16, 20);
            this.krplEndTimeChange.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.krplEndTimeChange.TabIndex = 10;
            this.krplEndTimeChange.Values.Text = "*";
            this.krplEndTimeChange.Visible = false;
            // 
            // krplPaidPriceChange
            // 
            this.krplPaidPriceChange.Location = new System.Drawing.Point(73, 6);
            this.krplPaidPriceChange.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
            this.krplPaidPriceChange.Name = "krplPaidPriceChange";
            this.krplPaidPriceChange.Size = new System.Drawing.Size(16, 20);
            this.krplPaidPriceChange.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.krplPaidPriceChange.TabIndex = 10;
            this.krplPaidPriceChange.Values.Text = "*";
            this.krplPaidPriceChange.Visible = false;
            // 
            // btnPaidPriceAdd
            // 
            this.btnPaidPriceAdd.Location = new System.Drawing.Point(95, 3);
            this.btnPaidPriceAdd.Name = "btnPaidPriceAdd";
            this.btnPaidPriceAdd.Size = new System.Drawing.Size(28, 25);
            this.btnPaidPriceAdd.TabIndex = 0;
            this.btnPaidPriceAdd.Values.Text = "+";
            this.btnPaidPriceAdd.Click += new System.EventHandler(this.btnPaidPriceAdd_Click);
            // 
            // btnPaidPriceSub
            // 
            this.btnPaidPriceSub.Location = new System.Drawing.Point(129, 3);
            this.btnPaidPriceSub.Name = "btnPaidPriceSub";
            this.btnPaidPriceSub.Size = new System.Drawing.Size(28, 25);
            this.btnPaidPriceSub.TabIndex = 1;
            this.btnPaidPriceSub.Values.Text = "-";
            this.btnPaidPriceSub.Click += new System.EventHandler(this.btnPaidPriceSub_Click);
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
            // krpbConfirm
            // 
            this.krpbConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbConfirm.Location = new System.Drawing.Point(851, 7);
            this.krpbConfirm.Name = "krpbConfirm";
            this.krpbConfirm.Size = new System.Drawing.Size(90, 25);
            this.krpbConfirm.TabIndex = 11;
            this.krpbConfirm.Values.Text = "Confirm";
            this.krpbConfirm.Visible = false;
            this.krpbConfirm.Click += new System.EventHandler(this.krpbConfirm_Click);
            // 
            // krplRemarkChange
            // 
            this.krplRemarkChange.Location = new System.Drawing.Point(843, 77);
            this.krplRemarkChange.Name = "krplRemarkChange";
            this.krplRemarkChange.Size = new System.Drawing.Size(16, 20);
            this.krplRemarkChange.StateCommon.ShortText.Color1 = System.Drawing.Color.Red;
            this.krplRemarkChange.TabIndex = 10;
            this.krplRemarkChange.Values.Text = "*";
            this.krplRemarkChange.Visible = false;
            // 
            // krpbSave
            // 
            this.krpbSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSave.Location = new System.Drawing.Point(851, 7);
            this.krpbSave.Name = "krpbSave";
            this.krpbSave.Size = new System.Drawing.Size(90, 25);
            this.krpbSave.TabIndex = 11;
            this.krpbSave.Values.Text = "Save";
            this.krpbSave.Click += new System.EventHandler(this.krpbSave_Click);
            // 
            // krplLanguage
            // 
            this.krplLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplLanguage.Location = new System.Drawing.Point(44, 80);
            this.krplLanguage.Name = "krplLanguage";
            this.krplLanguage.Size = new System.Drawing.Size(64, 20);
            this.krplLanguage.TabIndex = 10;
            this.krplLanguage.Values.Text = "Language";
            // 
            // krpcbLanguage
            // 
            this.krpcbLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpcbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbLanguage.DropDownWidth = 149;
            this.krpcbLanguage.Location = new System.Drawing.Point(124, 80);
            this.krpcbLanguage.Name = "krpcbLanguage";
            this.krpcbLanguage.Size = new System.Drawing.Size(99, 21);
            this.krpcbLanguage.TabIndex = 2;
            this.krpcbLanguage.SelectedIndexChanged += new System.EventHandler(this.krpcbLanguage_SelectedIndexChanged);
            // 
            // kryptonLabel5
            // 
            this.kryptonLabel5.Location = new System.Drawing.Point(105, 81);
            this.kryptonLabel5.Name = "kryptonLabel5";
            this.kryptonLabel5.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel5.TabIndex = 10;
            this.kryptonLabel5.Values.Text = ":";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.krplEndTimeValue);
            this.flowLayoutPanel1.Controls.Add(this.krplEndTimeChange);
            this.flowLayoutPanel1.Controls.Add(this.btnEndTimeAdd);
            this.flowLayoutPanel1.Controls.Add(this.btnEndTimeSub);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(340, 72);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(245, 30);
            this.flowLayoutPanel1.TabIndex = 20;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.krplPaidPriceValue);
            this.flowLayoutPanel2.Controls.Add(this.krplPaidPriceChange);
            this.flowLayoutPanel2.Controls.Add(this.btnPaidPriceAdd);
            this.flowLayoutPanel2.Controls.Add(this.btnPaidPriceSub);
            this.flowLayoutPanel2.Location = new System.Drawing.Point(339, 5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(200, 30);
            this.flowLayoutPanel2.TabIndex = 21;
            // 
            // krpbCheckout
            // 
            this.krpbCheckout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbCheckout.Location = new System.Drawing.Point(851, 7);
            this.krpbCheckout.Name = "krpbCheckout";
            this.krpbCheckout.Size = new System.Drawing.Size(90, 25);
            this.krpbCheckout.TabIndex = 11;
            this.krpbCheckout.Values.Text = "Checkout";
            this.krpbCheckout.Visible = false;
            this.krpbCheckout.Click += new System.EventHandler(this.krpbCheckout_Click);
            // 
            // krplRoomPrice
            // 
            this.krplRoomPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRoomPrice.Location = new System.Drawing.Point(39, 44);
            this.krplRoomPrice.Name = "krplRoomPrice";
            this.krplRoomPrice.Size = new System.Drawing.Size(69, 20);
            this.krplRoomPrice.TabIndex = 10;
            this.krplRoomPrice.Values.Text = "RoomPrice";
            // 
            // krplRoomPriceValue
            // 
            this.krplRoomPriceValue.Location = new System.Drawing.Point(118, 44);
            this.krplRoomPriceValue.Name = "krplRoomPriceValue";
            this.krplRoomPriceValue.Size = new System.Drawing.Size(17, 20);
            this.krplRoomPriceValue.TabIndex = 10;
            this.krplRoomPriceValue.Values.Text = "0";
            // 
            // kryptonLabel8
            // 
            this.kryptonLabel8.Location = new System.Drawing.Point(105, 44);
            this.kryptonLabel8.Name = "kryptonLabel8";
            this.kryptonLabel8.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel8.TabIndex = 10;
            this.kryptonLabel8.Values.Text = ":";
            // 
            // krpcbPackage
            // 
            this.krpcbPackage.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbPackage.Location = new System.Drawing.Point(176, 44);
            this.krpcbPackage.Name = "krpcbPackage";
            this.krpcbPackage.Size = new System.Drawing.Size(69, 20);
            this.krpcbPackage.TabIndex = 22;
            this.krpcbPackage.Text = "Package";
            this.krpcbPackage.Values.Text = "Package";
            this.krpcbPackage.Visible = false;
            // 
            // krpbAddByBarcode
            // 
            this.krpbAddByBarcode.Location = new System.Drawing.Point(850, 38);
            this.krpbAddByBarcode.Name = "krpbAddByBarcode";
            this.krpbAddByBarcode.Size = new System.Drawing.Size(32, 32);
            this.krpbAddByBarcode.TabIndex = 23;
            this.krpbAddByBarcode.Values.Text = "";
            this.krpbAddByBarcode.Click += new System.EventHandler(this.ktpbAddByBarcode_Click);
            // 
            // krpbAddByFastGrid
            // 
            this.krpbAddByFastGrid.Location = new System.Drawing.Point(907, 38);
            this.krpbAddByFastGrid.Name = "krpbAddByFastGrid";
            this.krpbAddByFastGrid.Size = new System.Drawing.Size(32, 32);
            this.krpbAddByFastGrid.TabIndex = 23;
            this.krpbAddByFastGrid.Values.Text = "";
            this.krpbAddByFastGrid.Click += new System.EventHandler(this.krpbAddByFastGrid_Click);
            // 
            // comTextColumn1
            // 
            this.comTextColumn1.HeaderText = "ProductName";
            this.comTextColumn1.Name = "comTextColumn1";
            this.comTextColumn1.Width = 210;
            // 
            // krplRoomNo
            // 
            this.krplRoomNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRoomNo.Location = new System.Drawing.Point(271, 44);
            this.krplRoomNo.Name = "krplRoomNo";
            this.krplRoomNo.Size = new System.Drawing.Size(60, 20);
            this.krplRoomNo.TabIndex = 10;
            this.krplRoomNo.Values.Text = "RoomNo";
            // 
            // kryptonLabel7
            // 
            this.kryptonLabel7.Location = new System.Drawing.Point(327, 44);
            this.kryptonLabel7.Name = "kryptonLabel7";
            this.kryptonLabel7.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel7.TabIndex = 10;
            this.kryptonLabel7.Values.Text = ":";
            // 
            // krplRoomNoValue
            // 
            this.krplRoomNoValue.Location = new System.Drawing.Point(340, 44);
            this.krplRoomNoValue.Name = "krplRoomNoValue";
            this.krplRoomNoValue.Size = new System.Drawing.Size(64, 20);
            this.krplRoomNoValue.TabIndex = 10;
            this.krplRoomNoValue.Values.Text = "00000000";
            // 
            // krpbRefresh
            // 
            this.krpbRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbRefresh.Location = new System.Drawing.Point(851, 7);
            this.krpbRefresh.Name = "krpbRefresh";
            this.krpbRefresh.Size = new System.Drawing.Size(90, 25);
            this.krpbRefresh.StateCommon.Content.ShortText.Color1 = System.Drawing.Color.Red;
            this.krpbRefresh.TabIndex = 11;
            this.krpbRefresh.Values.Text = "Refresh";
            this.krpbRefresh.Visible = false;
            this.krpbRefresh.Click += new System.EventHandler(this.krpbRefresh_Click);
            // 
            // OrderOperateWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(953, 418);
            this.Controls.Add(this.krpbAddByFastGrid);
            this.Controls.Add(this.krpbAddByBarcode);
            this.Controls.Add(this.krpcbPackage);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.krpcbLanguage);
            this.Controls.Add(this.krptbRemark);
            this.Controls.Add(this.krplEndTime);
            this.Controls.Add(this.krpbRefresh);
            this.Controls.Add(this.krpbCheckout);
            this.Controls.Add(this.krpbConfirm);
            this.Controls.Add(this.krplRemarkChange);
            this.Controls.Add(this.krplRoomNoValue);
            this.Controls.Add(this.krplBalancePriceValue);
            this.Controls.Add(this.kryptonLabel7);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.kryptonLabel5);
            this.Controls.Add(this.kryptonLabel8);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplSperator2);
            this.Controls.Add(this.krplRoomPriceValue);
            this.Controls.Add(this.krplTotalPriceValue);
            this.Controls.Add(this.krplRoomNo);
            this.Controls.Add(this.krplBalancePrice);
            this.Controls.Add(this.krplPaidPrice);
            this.Controls.Add(this.krplLanguage);
            this.Controls.Add(this.krplRemark);
            this.Controls.Add(this.krplRoomPrice);
            this.Controls.Add(this.krplTotalPrice);
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
            this.Controls.Add(this.krpbSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 300);
            this.Name = "OrderOperateWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OrderOperateWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbLanguage)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
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
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems3;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemRequest;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBalancePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplSperator2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBalancePriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemark;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox krptbRemark;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOrderDetailId;
        private Tools.ComTextColumn krpcmProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPrice;
        private Oybab.ServicePC.Tools.ComTextBoxNumericUpDownColumn krpcmCount;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsPack;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmState;
        private Tools.ComTextColumn krpcmRequest;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddTime;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemDelete;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemHistory;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnEndTimeAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnEndTimeSub;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTimeChange;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPriceChange;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnPaidPriceAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnPaidPriceSub;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbConfirm;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRemarkChange;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSave;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel5;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbCheckout;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel8;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbPackage;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAddByBarcode;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAddByFastGrid;
        private Tools.ComTextColumn comTextColumn1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel7;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNoValue;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbRefresh;
    }
}