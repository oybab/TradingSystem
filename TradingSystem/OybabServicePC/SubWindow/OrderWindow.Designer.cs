namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class OrderWindow
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
            this.krpcmOrderId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRoomNo = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmMemberName = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmMemberPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmTotalPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmBorrowPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmKeepPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmFinishAdminName = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmStartTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmEndTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmState = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmLang = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmAddTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmFinishTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPhone = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmName0 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmName1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmName2 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
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
            this.krpContextMenu = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuItems3 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAdd = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemShow = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.krptbEndTime = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.krplEndTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krptbStartTime = new ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker();
            this.krplAddTime = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbOrderType = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.krplOrderType = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcbIsDisplayAll = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbMultipleLanguage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbOrderType)).BeginInit();
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
            this.krpcmOrderId,
            this.krpcmRoomNo,
            this.krpcmMemberName,
            this.krpcmTotalPrice,
            this.krpcmMemberPaidPrice,
            this.krpcmPaidPrice,
            this.krpcmTotalPaidPrice,
            this.krpcmBorrowPrice,
            this.krpcmKeepPrice,
            this.krpcmFinishAdminName,
            this.krpcmStartTime,
            this.krpcmEndTime,
            this.krpcmState,
            this.krpcmLang,
            this.krpcmAddTime,
            this.krpcmFinishTime,
            this.krpcmPhone,
            this.krpcmName0,
            this.krpcmName1,
            this.krpcmName2,
            this.krpcmRemark});
            this.krpdgList.Location = new System.Drawing.Point(12, 35);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.krpdgList.Size = new System.Drawing.Size(959, 225);
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
            this.krpdgList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.krpdgList_MouseDoubleClick);
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
            // krpcmOrderId
            // 
            this.krpcmOrderId.Frozen = true;
            this.krpcmOrderId.HeaderText = "OrderId";
            this.krpcmOrderId.Name = "krpcmOrderId";
            this.krpcmOrderId.ReadOnly = true;
            this.krpcmOrderId.Width = 150;
            // 
            // krpcmRoomNo
            // 
            this.krpcmRoomNo.HeaderText = "RoomNo";
            this.krpcmRoomNo.Name = "krpcmRoomNo";
            this.krpcmRoomNo.ReadOnly = true;
            this.krpcmRoomNo.Width = 150;
            // 
            // krpcmMemberName
            // 
            this.krpcmMemberName.HeaderText = "MemberName";
            this.krpcmMemberName.Name = "krpcmMemberName";
            this.krpcmMemberName.ReadOnly = true;
            this.krpcmMemberName.Visible = false;
            this.krpcmMemberName.Width = 210;
            // 
            // krpcmTotalPrice
            // 
            this.krpcmTotalPrice.HeaderText = "TotalPrice";
            this.krpcmTotalPrice.Name = "krpcmTotalPrice";
            this.krpcmTotalPrice.ReadOnly = true;
            this.krpcmTotalPrice.Width = 150;
            // 
            // krpcmMemberPaidPrice
            // 
            this.krpcmMemberPaidPrice.HeaderText = "MemberPaidPrice";
            this.krpcmMemberPaidPrice.Name = "krpcmMemberPaidPrice";
            this.krpcmMemberPaidPrice.ReadOnly = true;
            this.krpcmMemberPaidPrice.Width = 160;
            // 
            // krpcmPaidPrice
            // 
            this.krpcmPaidPrice.HeaderText = "PaidPrice";
            this.krpcmPaidPrice.Name = "krpcmPaidPrice";
            this.krpcmPaidPrice.ReadOnly = true;
            this.krpcmPaidPrice.Width = 150;
            // 
            // krpcmTotalPaidPrice
            // 
            this.krpcmTotalPaidPrice.HeaderText = "TotalPaidPrice";
            this.krpcmTotalPaidPrice.Name = "krpcmTotalPaidPrice";
            this.krpcmTotalPaidPrice.ReadOnly = true;
            this.krpcmTotalPaidPrice.Width = 180;
            // 
            // krpcmBorrowPrice
            // 
            this.krpcmBorrowPrice.HeaderText = "BorrowPrice";
            this.krpcmBorrowPrice.Name = "krpcmBorrowPrice";
            this.krpcmBorrowPrice.ReadOnly = true;
            this.krpcmBorrowPrice.Width = 150;
            // 
            // krpcmKeepPrice
            // 
            this.krpcmKeepPrice.HeaderText = "KeepPrice";
            this.krpcmKeepPrice.Name = "krpcmKeepPrice";
            this.krpcmKeepPrice.ReadOnly = true;
            this.krpcmKeepPrice.Width = 150;
            // 
            // krpcmFinishAdminName
            // 
            this.krpcmFinishAdminName.HeaderText = "FinishAdminName";
            this.krpcmFinishAdminName.Name = "krpcmFinishAdminName";
            this.krpcmFinishAdminName.ReadOnly = true;
            this.krpcmFinishAdminName.Width = 210;
            // 
            // krpcmStartTime
            // 
            this.krpcmStartTime.HeaderText = "StartTime";
            this.krpcmStartTime.Name = "krpcmStartTime";
            this.krpcmStartTime.ReadOnly = true;
            this.krpcmStartTime.Width = 150;
            // 
            // krpcmEndTime
            // 
            this.krpcmEndTime.HeaderText = "EndTime";
            this.krpcmEndTime.Name = "krpcmEndTime";
            this.krpcmEndTime.ReadOnly = true;
            this.krpcmEndTime.Width = 150;
            // 
            // krpcmState
            // 
            this.krpcmState.HeaderText = "State";
            this.krpcmState.Name = "krpcmState";
            this.krpcmState.ReadOnly = true;
            this.krpcmState.Width = 150;
            // 
            // krpcmLang
            // 
            this.krpcmLang.HeaderText = "Lang";
            this.krpcmLang.Name = "krpcmLang";
            this.krpcmLang.ReadOnly = true;
            this.krpcmLang.Width = 150;
            // 
            // krpcmAddTime
            // 
            this.krpcmAddTime.HeaderText = "AddTime";
            this.krpcmAddTime.Name = "krpcmAddTime";
            this.krpcmAddTime.ReadOnly = true;
            this.krpcmAddTime.Width = 150;
            // 
            // krpcmFinishTime
            // 
            this.krpcmFinishTime.HeaderText = "FinishTime";
            this.krpcmFinishTime.Name = "krpcmFinishTime";
            this.krpcmFinishTime.ReadOnly = true;
            this.krpcmFinishTime.Width = 150;
            // 
            // krpcmPhone
            // 
            this.krpcmPhone.HeaderText = "Phone";
            this.krpcmPhone.Name = "krpcmPhone";
            this.krpcmPhone.ReadOnly = true;
            this.krpcmPhone.Width = 150;
            // 
            // krpcmName0
            // 
            this.krpcmName0.HeaderText = "Name0";
            this.krpcmName0.Name = "krpcmName0";
            this.krpcmName0.ReadOnly = true;
            this.krpcmName0.Width = 210;
            // 
            // krpcmName1
            // 
            this.krpcmName1.HeaderText = "Name1";
            this.krpcmName1.Name = "krpcmName1";
            this.krpcmName1.ReadOnly = true;
            this.krpcmName1.Width = 210;
            // 
            // krpcmName2
            // 
            this.krpcmName2.HeaderText = "Name2";
            this.krpcmName2.Name = "krpcmName2";
            this.krpcmName2.ReadOnly = true;
            this.krpcmName2.Width = 210;
            // 
            // krpcmRemark
            // 
            this.krpcmRemark.HeaderText = "Remark";
            this.krpcmRemark.Name = "krpcmRemark";
            this.krpcmRemark.ReadOnly = true;
            this.krpcmRemark.Width = 150;
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(946, 263);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(901, 266);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(834, 268);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 4;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(788, 268);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(764, 263);
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
            this.krpbNextPage.Location = new System.Drawing.Point(740, 263);
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
            this.krpbPrewPage.Location = new System.Drawing.Point(716, 263);
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
            this.krpbBeginPage.Location = new System.Drawing.Point(692, 263);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krpbSearch
            // 
            this.krpbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSearch.Location = new System.Drawing.Point(881, 5);
            this.krpbSearch.Name = "krpbSearch";
            this.krpbSearch.Size = new System.Drawing.Size(90, 25);
            this.krpbSearch.TabIndex = 2;
            this.krpbSearch.Values.Text = "Search";
            this.krpbSearch.Click += new System.EventHandler(this.krpbSearch_Click);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(879, 266);
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
            this.kryptonContextMenuItemShow});
            this.kryptonContextMenuItems3.StandardStyle = false;
            // 
            // kryptonContextMenuItemAdd
            // 
            this.kryptonContextMenuItemAdd.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.kryptonContextMenuItemAdd.Text = "Menu Item";
            this.kryptonContextMenuItemAdd.Visible = false;
            // 
            // kryptonContextMenuItemShow
            // 
            this.kryptonContextMenuItemShow.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.kryptonContextMenuItemShow.Text = "Menu Item";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.krptbEndTime);
            this.flowLayoutPanel1.Controls.Add(this.krplEndTime);
            this.flowLayoutPanel1.Controls.Add(this.krptbStartTime);
            this.flowLayoutPanel1.Controls.Add(this.krplAddTime);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(225, 5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(650, 28);
            this.flowLayoutPanel1.TabIndex = 10;
            // 
            // krptbEndTime
            // 
            this.krptbEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptbEndTime.CalendarShowToday = false;
            this.krptbEndTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.krptbEndTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.krptbEndTime.Location = new System.Drawing.Point(477, 3);
            this.krptbEndTime.Name = "krptbEndTime";
            this.krptbEndTime.Size = new System.Drawing.Size(170, 21);
            this.krptbEndTime.TabIndex = 7;
            this.krptbEndTime.ValueNullable = new System.DateTime(((long)(0)));
            this.krptbEndTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // krplEndTime
            // 
            this.krplEndTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplEndTime.Location = new System.Drawing.Point(452, 3);
            this.krplEndTime.Name = "krplEndTime";
            this.krplEndTime.Size = new System.Drawing.Size(19, 20);
            this.krplEndTime.TabIndex = 6;
            this.krplEndTime.Values.Text = " - ";
            // 
            // krptbStartTime
            // 
            this.krptbStartTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krptbStartTime.CalendarShowToday = false;
            this.krptbStartTime.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.krptbStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.krptbStartTime.Location = new System.Drawing.Point(276, 3);
            this.krptbStartTime.Name = "krptbStartTime";
            this.krptbStartTime.Size = new System.Drawing.Size(170, 21);
            this.krptbStartTime.TabIndex = 5;
            this.krptbStartTime.ValueNullable = new System.DateTime(((long)(0)));
            this.krptbStartTime.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Enter_KeyDown);
            // 
            // krplAddTime
            // 
            this.krplAddTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krplAddTime.Location = new System.Drawing.Point(210, 3);
            this.krplAddTime.Name = "krplAddTime";
            this.krplAddTime.Size = new System.Drawing.Size(60, 20);
            this.krplAddTime.TabIndex = 4;
            this.krplAddTime.Values.Text = "AddTime";
            // 
            // krpcbOrderType
            // 
            this.krpcbOrderType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.krpcbOrderType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.krpcbOrderType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.krpcbOrderType.DropDownWidth = 149;
            this.krpcbOrderType.Location = new System.Drawing.Point(92, 8);
            this.krpcbOrderType.Name = "krpcbOrderType";
            this.krpcbOrderType.Size = new System.Drawing.Size(102, 21);
            this.krpcbOrderType.TabIndex = 15;
            this.krpcbOrderType.SelectedIndexChanged += new System.EventHandler(this.krpcbOrderType_SelectedIndexChanged);
            // 
            // krplOrderType
            // 
            this.krplOrderType.Location = new System.Drawing.Point(12, 8);
            this.krplOrderType.Name = "krplOrderType";
            this.krplOrderType.Size = new System.Drawing.Size(56, 20);
            this.krplOrderType.TabIndex = 14;
            this.krplOrderType.Values.Text = "Bill Type";
            // 
            // krpcbIsDisplayAll
            // 
            this.krpcbIsDisplayAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpcbIsDisplayAll.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbIsDisplayAll.Location = new System.Drawing.Point(382, 263);
            this.krpcbIsDisplayAll.Name = "krpcbIsDisplayAll";
            this.krpcbIsDisplayAll.Size = new System.Drawing.Size(85, 20);
            this.krpcbIsDisplayAll.TabIndex = 25;
            this.krpcbIsDisplayAll.Text = "IsDisplayAll";
            this.krpcbIsDisplayAll.Values.Text = "IsDisplayAll";
            this.krpcbIsDisplayAll.Visible = false;
            this.krpcbIsDisplayAll.CheckedChanged += new System.EventHandler(this.krpcbIsDisplayAll_CheckedChanged);
            // 
            // krpcbMultipleLanguage
            // 
            this.krpcbMultipleLanguage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.krpcbMultipleLanguage.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbMultipleLanguage.Location = new System.Drawing.Point(12, 266);
            this.krpcbMultipleLanguage.Name = "krpcbMultipleLanguage";
            this.krpcbMultipleLanguage.Size = new System.Drawing.Size(122, 20);
            this.krpcbMultipleLanguage.TabIndex = 26;
            this.krpcbMultipleLanguage.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.Values.Text = "MultipleLanguage";
            this.krpcbMultipleLanguage.CheckedChanged += new System.EventHandler(this.krpcbMultipleLanguage_CheckedChanged);
            // 
            // OrderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(983, 292);
            this.Controls.Add(this.krpcbMultipleLanguage);
            this.Controls.Add(this.krpcbIsDisplayAll);
            this.Controls.Add(this.krpcbOrderType);
            this.Controls.Add(this.krplOrderType);
            this.Controls.Add(this.flowLayoutPanel1);
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
            this.Name = "OrderWindow";
            this.Text = "OrderWindow";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpcbOrderType)).EndInit();
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
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems3;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemShow;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAdd;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;


        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplAddTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker krptbStartTime;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDateTimePicker krptbEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonComboBox krpcbOrderType;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplOrderType;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbIsDisplayAll;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbMultipleLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEdit;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOrderId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRoomNo;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmMemberName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmMemberPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmTotalPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmBorrowPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmKeepPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmFinishAdminName;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmStartTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmState;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmLang;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmAddTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmFinishTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPhone;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmName0;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmName1;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmName2;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRemark;
    }
}