namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class OrderDetailsWindow
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
            this.krpcmOrderDetailId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmProductName = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmCount = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmIsPack = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn();
            this.krpcmState = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRequest = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAddTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAdmin = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
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
            this.kryptonContextMenuItemPrint = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemHistory = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.krplOrderNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplOrderNoValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomNo = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomNoValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplAddTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplAddTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplFinishTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplFinishTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplStartTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplStartTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplEndTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplEndTimeValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplState = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplStateValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplKeepPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBorrowPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplKeepPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplBorrowPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplMemberName = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplMemberNameValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplMemberPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplMemberPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplTotalPaidPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomPrice = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplRoomPriceValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpdgPayList = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.krpcmEdit2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPayId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmState2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmBalanceName = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmMemberName = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmOriginalPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRemovePrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmBalancePrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAddTime2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAdmin2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgPayList)).BeginInit();
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
            this.krpcmAddTime,
            this.krpcmAdmin});
            this.krpdgList.Location = new System.Drawing.Point(12, 120);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.krpdgList.Size = new System.Drawing.Size(923, 183);
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
            this.krpcmProductName.ReadOnly = true;
            this.krpcmProductName.Width = 210;
            // 
            // krpcmPrice
            // 
            this.krpcmPrice.HeaderText = "krpcmPrice";
            this.krpcmPrice.Name = "krpcmPrice";
            this.krpcmPrice.ReadOnly = true;
            this.krpcmPrice.Width = 110;
            // 
            // krpcmCount
            // 
            this.krpcmCount.HeaderText = "Count";
            this.krpcmCount.Name = "krpcmCount";
            this.krpcmCount.ReadOnly = true;
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
            this.krpcmIsPack.ReadOnly = true;
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
            this.krpcmRequest.ReadOnly = true;
            this.krpcmRequest.Width = 120;
            // 
            // krpcmAddTime
            // 
            this.krpcmAddTime.HeaderText = "AddTime";
            this.krpcmAddTime.Name = "krpcmAddTime";
            this.krpcmAddTime.ReadOnly = true;
            this.krpcmAddTime.Width = 150;
            // 
            // krpcmAdmin
            // 
            this.krpcmAdmin.HeaderText = "Admin";
            this.krpcmAdmin.Name = "krpcmAdmin";
            this.krpcmAdmin.ReadOnly = true;
            this.krpcmAdmin.Width = 120;
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(910, 416);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(865, 419);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(798, 421);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 2;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(752, 421);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(728, 416);
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
            this.krpbNextPage.Location = new System.Drawing.Point(704, 416);
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
            this.krpbPrewPage.Location = new System.Drawing.Point(680, 416);
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
            this.krpbBeginPage.Location = new System.Drawing.Point(656, 416);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(843, 419);
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
            this.kryptonContextMenuItemPrint,
            this.kryptonContextMenuItemHistory});
            this.kryptonContextMenuItems3.StandardStyle = false;
            // 
            // kryptonContextMenuItemPrint
            // 
            this.kryptonContextMenuItemPrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.kryptonContextMenuItemPrint.Text = "Menu Item";
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
            // krplOrderNo
            // 
            this.krplOrderNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplOrderNo.Location = new System.Drawing.Point(32, 8);
            this.krplOrderNo.Name = "krplOrderNo";
            this.krplOrderNo.Size = new System.Drawing.Size(59, 20);
            this.krplOrderNo.TabIndex = 10;
            this.krplOrderNo.Values.Text = "OrderNo";
            // 
            // krplOrderNoValue
            // 
            this.krplOrderNoValue.Location = new System.Drawing.Point(88, 8);
            this.krplOrderNoValue.Name = "krplOrderNoValue";
            this.krplOrderNoValue.Size = new System.Drawing.Size(64, 20);
            this.krplOrderNoValue.TabIndex = 10;
            this.krplOrderNoValue.Values.Text = "00000000";
            // 
            // krplRoomNo
            // 
            this.krplRoomNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRoomNo.Location = new System.Drawing.Point(221, 8);
            this.krplRoomNo.Name = "krplRoomNo";
            this.krplRoomNo.Size = new System.Drawing.Size(60, 20);
            this.krplRoomNo.TabIndex = 10;
            this.krplRoomNo.Values.Text = "RoomNo";
            // 
            // krplRoomNoValue
            // 
            this.krplRoomNoValue.Location = new System.Drawing.Point(278, 8);
            this.krplRoomNoValue.Name = "krplRoomNoValue";
            this.krplRoomNoValue.Size = new System.Drawing.Size(64, 20);
            this.krplRoomNoValue.TabIndex = 10;
            this.krplRoomNoValue.Values.Text = "00000000";
            // 
            // krplAddTime
            // 
            this.krplAddTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplAddTime.Location = new System.Drawing.Point(723, 89);
            this.krplAddTime.Name = "krplAddTime";
            this.krplAddTime.Size = new System.Drawing.Size(60, 20);
            this.krplAddTime.TabIndex = 10;
            this.krplAddTime.Values.Text = "AddTime";
            // 
            // krplAddTimeValue
            // 
            this.krplAddTimeValue.Location = new System.Drawing.Point(781, 89);
            this.krplAddTimeValue.Name = "krplAddTimeValue";
            this.krplAddTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplAddTimeValue.TabIndex = 10;
            this.krplAddTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // krplFinishTime
            // 
            this.krplFinishTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplFinishTime.Location = new System.Drawing.Point(680, 452);
            this.krplFinishTime.Name = "krplFinishTime";
            this.krplFinishTime.Size = new System.Drawing.Size(68, 20);
            this.krplFinishTime.TabIndex = 10;
            this.krplFinishTime.Values.Text = "FinishTime";
            // 
            // krplFinishTimeValue
            // 
            this.krplFinishTimeValue.Location = new System.Drawing.Point(743, 452);
            this.krplFinishTimeValue.Name = "krplFinishTimeValue";
            this.krplFinishTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplFinishTimeValue.TabIndex = 10;
            this.krplFinishTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // krplStartTime
            // 
            this.krplStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplStartTime.Location = new System.Drawing.Point(228, 89);
            this.krplStartTime.Name = "krplStartTime";
            this.krplStartTime.Size = new System.Drawing.Size(63, 20);
            this.krplStartTime.TabIndex = 10;
            this.krplStartTime.Values.Text = "StartTime";
            // 
            // krplStartTimeValue
            // 
            this.krplStartTimeValue.Location = new System.Drawing.Point(287, 89);
            this.krplStartTimeValue.Name = "krplStartTimeValue";
            this.krplStartTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplStartTimeValue.TabIndex = 10;
            this.krplStartTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // krplEndTime
            // 
            this.krplEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplEndTime.Location = new System.Drawing.Point(488, 89);
            this.krplEndTime.Name = "krplEndTime";
            this.krplEndTime.Size = new System.Drawing.Size(58, 20);
            this.krplEndTime.TabIndex = 10;
            this.krplEndTime.Values.Text = "EndTime";
            // 
            // krplEndTimeValue
            // 
            this.krplEndTimeValue.Location = new System.Drawing.Point(544, 89);
            this.krplEndTimeValue.Name = "krplEndTimeValue";
            this.krplEndTimeValue.Size = new System.Drawing.Size(93, 20);
            this.krplEndTimeValue.TabIndex = 10;
            this.krplEndTimeValue.Values.Text = "????-??-?? ??:??";
            // 
            // krplTotalPrice
            // 
            this.krplTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPrice.Location = new System.Drawing.Point(704, 8);
            this.krplTotalPrice.Name = "krplTotalPrice";
            this.krplTotalPrice.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPrice.TabIndex = 10;
            this.krplTotalPrice.Values.Text = "TotalPrice";
            // 
            // krplTotalPriceValue
            // 
            this.krplTotalPriceValue.Location = new System.Drawing.Point(767, 8);
            this.krplTotalPriceValue.Name = "krplTotalPriceValue";
            this.krplTotalPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPriceValue.TabIndex = 10;
            this.krplTotalPriceValue.Values.Text = "00000000";
            // 
            // krplState
            // 
            this.krplState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplState.Location = new System.Drawing.Point(38, 452);
            this.krplState.Name = "krplState";
            this.krplState.Size = new System.Drawing.Size(38, 20);
            this.krplState.TabIndex = 10;
            this.krplState.Values.Text = "State";
            // 
            // krplStateValue
            // 
            this.krplStateValue.Location = new System.Drawing.Point(87, 452);
            this.krplStateValue.Name = "krplStateValue";
            this.krplStateValue.Size = new System.Drawing.Size(64, 20);
            this.krplStateValue.TabIndex = 10;
            this.krplStateValue.Values.Text = "00000000";
            // 
            // krplKeepPriceValue
            // 
            this.krplKeepPriceValue.Location = new System.Drawing.Point(826, 50);
            this.krplKeepPriceValue.Name = "krplKeepPriceValue";
            this.krplKeepPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplKeepPriceValue.TabIndex = 11;
            this.krplKeepPriceValue.Values.Text = "00000000";
            // 
            // krplBorrowPriceValue
            // 
            this.krplBorrowPriceValue.Location = new System.Drawing.Point(663, 50);
            this.krplBorrowPriceValue.Name = "krplBorrowPriceValue";
            this.krplBorrowPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplBorrowPriceValue.TabIndex = 12;
            this.krplBorrowPriceValue.Values.Text = "00000000";
            // 
            // krplKeepPrice
            // 
            this.krplKeepPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplKeepPrice.Location = new System.Drawing.Point(762, 50);
            this.krplKeepPrice.Name = "krplKeepPrice";
            this.krplKeepPrice.Size = new System.Drawing.Size(64, 20);
            this.krplKeepPrice.TabIndex = 13;
            this.krplKeepPrice.Values.Text = "KeepPrice";
            // 
            // krplBorrowPrice
            // 
            this.krplBorrowPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplBorrowPrice.Location = new System.Drawing.Point(585, 50);
            this.krplBorrowPrice.Name = "krplBorrowPrice";
            this.krplBorrowPrice.Size = new System.Drawing.Size(76, 20);
            this.krplBorrowPrice.TabIndex = 14;
            this.krplBorrowPrice.Values.Text = "BorrowPrice";
            // 
            // krplPaidPriceValue
            // 
            this.krplPaidPriceValue.Location = new System.Drawing.Point(86, 50);
            this.krplPaidPriceValue.Name = "krplPaidPriceValue";
            this.krplPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplPaidPriceValue.TabIndex = 15;
            this.krplPaidPriceValue.Values.Text = "00000000";
            // 
            // krplPaidPrice
            // 
            this.krplPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPaidPrice.Location = new System.Drawing.Point(29, 50);
            this.krplPaidPrice.Name = "krplPaidPrice";
            this.krplPaidPrice.Size = new System.Drawing.Size(60, 20);
            this.krplPaidPrice.TabIndex = 17;
            this.krplPaidPrice.Values.Text = "PaidPrice";
            // 
            // krplMemberName
            // 
            this.krplMemberName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplMemberName.Location = new System.Drawing.Point(423, 8);
            this.krplMemberName.Name = "krplMemberName";
            this.krplMemberName.Size = new System.Drawing.Size(89, 20);
            this.krplMemberName.TabIndex = 10;
            this.krplMemberName.Values.Text = "MemberName";
            this.krplMemberName.Visible = false;
            // 
            // krplMemberNameValue
            // 
            this.krplMemberNameValue.Location = new System.Drawing.Point(508, 8);
            this.krplMemberNameValue.Name = "krplMemberNameValue";
            this.krplMemberNameValue.Size = new System.Drawing.Size(130, 20);
            this.krplMemberNameValue.TabIndex = 10;
            this.krplMemberNameValue.Values.Text = "000000000000000000";
            this.krplMemberNameValue.Visible = false;
            // 
            // krplMemberPaidPrice
            // 
            this.krplMemberPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplMemberPaidPrice.Location = new System.Drawing.Point(184, 50);
            this.krplMemberPaidPrice.Name = "krplMemberPaidPrice";
            this.krplMemberPaidPrice.Size = new System.Drawing.Size(107, 20);
            this.krplMemberPaidPrice.TabIndex = 10;
            this.krplMemberPaidPrice.Values.Text = "MemberPaidPrice";
            // 
            // krplMemberPaidPriceValue
            // 
            this.krplMemberPaidPriceValue.Location = new System.Drawing.Point(288, 50);
            this.krplMemberPaidPriceValue.Name = "krplMemberPaidPriceValue";
            this.krplMemberPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplMemberPaidPriceValue.TabIndex = 10;
            this.krplMemberPaidPriceValue.Values.Text = "00000000";
            // 
            // krplTotalPaidPrice
            // 
            this.krplTotalPaidPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplTotalPaidPrice.Location = new System.Drawing.Point(390, 50);
            this.krplTotalPaidPrice.Name = "krplTotalPaidPrice";
            this.krplTotalPaidPrice.Size = new System.Drawing.Size(88, 20);
            this.krplTotalPaidPrice.TabIndex = 18;
            this.krplTotalPaidPrice.Values.Text = "TotalPaidPrice";
            // 
            // krplTotalPaidPriceValue
            // 
            this.krplTotalPaidPriceValue.Location = new System.Drawing.Point(475, 50);
            this.krplTotalPaidPriceValue.Name = "krplTotalPaidPriceValue";
            this.krplTotalPaidPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplTotalPaidPriceValue.TabIndex = 16;
            this.krplTotalPaidPriceValue.Values.Text = "00000000";
            // 
            // krplRoomPrice
            // 
            this.krplRoomPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplRoomPrice.Location = new System.Drawing.Point(38, 89);
            this.krplRoomPrice.Name = "krplRoomPrice";
            this.krplRoomPrice.Size = new System.Drawing.Size(69, 20);
            this.krplRoomPrice.TabIndex = 17;
            this.krplRoomPrice.Values.Text = "RoomPrice";
            // 
            // krplRoomPriceValue
            // 
            this.krplRoomPriceValue.Location = new System.Drawing.Point(104, 89);
            this.krplRoomPriceValue.Name = "krplRoomPriceValue";
            this.krplRoomPriceValue.Size = new System.Drawing.Size(64, 20);
            this.krplRoomPriceValue.TabIndex = 15;
            this.krplRoomPriceValue.Values.Text = "00000000";
            // 
            // krpdgPayList
            // 
            this.krpdgPayList.AllowUserToAddRows = false;
            this.krpdgPayList.AllowUserToDeleteRows = false;
            this.krpdgPayList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.krpdgPayList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.krpdgPayList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.krpcmEdit2,
            this.krpcmPayId,
            this.krpcmState2,
            this.krpcmBalanceName,
            this.krpcmMemberName,
            this.krpcmOriginalPrice,
            this.krpcmRemovePrice,
            this.krpcmBalancePrice,
            this.krpcmAddTime2,
            this.krpcmAdmin2});
            this.krpdgPayList.Location = new System.Drawing.Point(12, 320);
            this.krpdgPayList.MultiSelect = false;
            this.krpdgPayList.Name = "krpdgPayList";
            this.krpdgPayList.RowTemplate.Height = 23;
            this.krpdgPayList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.krpdgPayList.Size = new System.Drawing.Size(923, 126);
            this.krpdgPayList.TabIndex = 9;
            this.krpdgPayList.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.krpdgPayList_RowPostPaint);
            // 
            // krpcmEdit2
            // 
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Red;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Red;
            this.krpcmEdit2.DefaultCellStyle = dataGridViewCellStyle3;
            this.krpcmEdit2.Frozen = true;
            this.krpcmEdit2.HeaderText = "*";
            this.krpcmEdit2.Name = "krpcmEdit2";
            this.krpcmEdit2.ReadOnly = true;
            this.krpcmEdit2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.krpcmEdit2.Width = 20;
            // 
            // krpcmPayId
            // 
            this.krpcmPayId.HeaderText = "PayId";
            this.krpcmPayId.Name = "krpcmPayId";
            this.krpcmPayId.ReadOnly = true;
            this.krpcmPayId.Visible = false;
            this.krpcmPayId.Width = 150;
            // 
            // krpcmState2
            // 
            this.krpcmState2.HeaderText = "State";
            this.krpcmState2.Name = "krpcmState2";
            this.krpcmState2.ReadOnly = true;
            this.krpcmState2.Visible = false;
            this.krpcmState2.Width = 100;
            // 
            // krpcmBalanceName
            // 
            this.krpcmBalanceName.HeaderText = "BalanceName";
            this.krpcmBalanceName.Name = "krpcmBalanceName";
            this.krpcmBalanceName.ReadOnly = true;
            this.krpcmBalanceName.Width = 130;
            // 
            // krpcmMemberName
            // 
            this.krpcmMemberName.HeaderText = "MemberName";
            this.krpcmMemberName.Name = "krpcmMemberName";
            this.krpcmMemberName.ReadOnly = true;
            this.krpcmMemberName.Width = 110;
            // 
            // krpcmOriginalPrice
            // 
            this.krpcmOriginalPrice.HeaderText = "OriginalPrice";
            this.krpcmOriginalPrice.Name = "krpcmOriginalPrice";
            this.krpcmOriginalPrice.ReadOnly = true;
            this.krpcmOriginalPrice.Width = 100;
            // 
            // krpcmRemovePrice
            // 
            this.krpcmRemovePrice.HeaderText = "RemovePrice";
            this.krpcmRemovePrice.Name = "krpcmRemovePrice";
            this.krpcmRemovePrice.ReadOnly = true;
            this.krpcmRemovePrice.Width = 120;
            // 
            // krpcmBalancePrice
            // 
            this.krpcmBalancePrice.HeaderText = "BalancePrice";
            this.krpcmBalancePrice.Name = "krpcmBalancePrice";
            this.krpcmBalancePrice.ReadOnly = true;
            this.krpcmBalancePrice.Width = 130;
            // 
            // krpcmAddTime2
            // 
            this.krpcmAddTime2.HeaderText = "AddTime";
            this.krpcmAddTime2.Name = "krpcmAddTime2";
            this.krpcmAddTime2.ReadOnly = true;
            this.krpcmAddTime2.Width = 150;
            // 
            // krpcmAdmin2
            // 
            this.krpcmAdmin2.HeaderText = "Admin";
            this.krpcmAdmin2.Name = "krpcmAdmin2";
            this.krpcmAdmin2.ReadOnly = true;
            this.krpcmAdmin2.Width = 120;
            // 
            // OrderDetailsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(947, 478);
            this.Controls.Add(this.krplKeepPriceValue);
            this.Controls.Add(this.krplBorrowPriceValue);
            this.Controls.Add(this.krplKeepPrice);
            this.Controls.Add(this.krplBorrowPrice);
            this.Controls.Add(this.krplRoomPriceValue);
            this.Controls.Add(this.krplPaidPriceValue);
            this.Controls.Add(this.krplTotalPaidPriceValue);
            this.Controls.Add(this.krplRoomPrice);
            this.Controls.Add(this.krplPaidPrice);
            this.Controls.Add(this.krplTotalPaidPrice);
            this.Controls.Add(this.krplEndTimeValue);
            this.Controls.Add(this.krplFinishTimeValue);
            this.Controls.Add(this.krplTotalPriceValue);
            this.Controls.Add(this.krplMemberPaidPriceValue);
            this.Controls.Add(this.krplRoomNoValue);
            this.Controls.Add(this.krplEndTime);
            this.Controls.Add(this.krplFinishTime);
            this.Controls.Add(this.krplTotalPrice);
            this.Controls.Add(this.krplMemberPaidPrice);
            this.Controls.Add(this.krplRoomNo);
            this.Controls.Add(this.krplMemberNameValue);
            this.Controls.Add(this.krplStartTimeValue);
            this.Controls.Add(this.krplAddTimeValue);
            this.Controls.Add(this.krplStateValue);
            this.Controls.Add(this.krplOrderNoValue);
            this.Controls.Add(this.krplMemberName);
            this.Controls.Add(this.krplStartTime);
            this.Controls.Add(this.krplAddTime);
            this.Controls.Add(this.krplState);
            this.Controls.Add(this.krplOrderNo);
            this.Controls.Add(this.krpdgPayList);
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
            this.Name = "OrderDetailsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "OrderDetailsWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgPayList)).EndInit();
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
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemPrint;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemHistory;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOrderDetailId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmProductName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmCount;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewCheckBoxColumn krpcmIsPack;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmState;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRequest;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAdmin;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplOrderNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplOrderNoValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNo;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomNoValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAddTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAddTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplFinishTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplFinishTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplStartTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplStartTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTimeValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplState;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplStateValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplKeepPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBorrowPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplKeepPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplBorrowPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberNameValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplMemberPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplTotalPaidPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplRoomPriceValue;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView krpdgPayList;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPayId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmState2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmBalanceName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmMemberName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOriginalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRemovePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmBalancePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddTime2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAdmin2;
    }
}