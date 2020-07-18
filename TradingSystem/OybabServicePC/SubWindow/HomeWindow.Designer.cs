namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class HomeWindow
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
            this.kryptonContextMenuOrder = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemNewOrder = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemShowOrder = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemCheckoutOrder = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemCancelOrder = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemReplaceRoom = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemRestart = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItemShutdown = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.krpContextMenuTakeout = new ComponentFactory.Krypton.Toolkit.KryptonContextMenu();
            this.kryptonContextMenuTakeout = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItemAddTakeout = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems2 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItem1 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem();
            this.kryptonContextMenuItems4 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonContextMenuItems5 = new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems();
            this.kryptonTaskDialog = new ComponentFactory.Krypton.Toolkit.KryptonTaskDialog();
            this.krpbSendFireAlarm = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewCallValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewCall = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewOrder = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krplNewOrderValue = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.krpcmRemark = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmLang = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmMemberPaidPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmBalancePrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmTotalPrice = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmState = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmEndTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmStartTime = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRoomNo = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmRoomId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmOrderId = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmOrderSession = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmOrder = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpcmCall = new ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn();
            this.krpdgList = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.krpbImageMode = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbListMode = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbAddTakeout = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpbPos = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.tlpTakeout = new System.Windows.Forms.TableLayoutPanel();
            this.flpRooms = new Oybab.ServicePC.Tools.CustomFlowLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).BeginInit();
            this.SuspendLayout();
            // 
            // krpbClickToPage
            // 
            this.krpbClickToPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbClickToPage.Location = new System.Drawing.Point(597, 223);
            this.krpbClickToPage.Name = "krpbClickToPage";
            this.krpbClickToPage.Size = new System.Drawing.Size(25, 25);
            this.krpbClickToPage.TabIndex = 4;
            this.krpbClickToPage.Values.Text = "";
            this.krpbClickToPage.Visible = false;
            this.krpbClickToPage.Click += new System.EventHandler(this.krpbClickToPage_Click);
            // 
            // krplPageCount
            // 
            this.krplPageCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPageCount.Location = new System.Drawing.Point(552, 226);
            this.krplPageCount.Name = "krplPageCount";
            this.krplPageCount.Size = new System.Drawing.Size(44, 20);
            this.krplPageCount.TabIndex = 4;
            this.krplPageCount.Values.Text = "99999";
            this.krplPageCount.Visible = false;
            // 
            // krptCurrentPage
            // 
            this.krptCurrentPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krptCurrentPage.Location = new System.Drawing.Point(485, 228);
            this.krptCurrentPage.MaxLength = 4;
            this.krptCurrentPage.Name = "krptCurrentPage";
            this.krptCurrentPage.Size = new System.Drawing.Size(30, 23);
            this.krptCurrentPage.TabIndex = 2;
            this.krptCurrentPage.Visible = false;
            this.krptCurrentPage.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krptCurrentPage_KeyUp);
            // 
            // krplPage
            // 
            this.krplPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplPage.Location = new System.Drawing.Point(439, 228);
            this.krplPage.Name = "krplPage";
            this.krplPage.Size = new System.Drawing.Size(40, 20);
            this.krplPage.TabIndex = 7;
            this.krplPage.Values.Text = "Page:";
            this.krplPage.Visible = false;
            // 
            // krpbEngPage
            // 
            this.krpbEngPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbEngPage.Location = new System.Drawing.Point(415, 223);
            this.krpbEngPage.Name = "krpbEngPage";
            this.krpbEngPage.Size = new System.Drawing.Size(18, 25);
            this.krpbEngPage.TabIndex = 8;
            this.krpbEngPage.Values.Text = "";
            this.krpbEngPage.Visible = false;
            this.krpbEngPage.Click += new System.EventHandler(this.krpbEngPage_Click);
            // 
            // krpbNextPage
            // 
            this.krpbNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbNextPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.krpbNextPage.Location = new System.Drawing.Point(391, 223);
            this.krpbNextPage.Name = "krpbNextPage";
            this.krpbNextPage.Size = new System.Drawing.Size(18, 25);
            this.krpbNextPage.TabIndex = 7;
            this.krpbNextPage.Values.Text = "";
            this.krpbNextPage.Visible = false;
            this.krpbNextPage.Click += new System.EventHandler(this.krpbNextPage_Click);
            // 
            // krpbPrewPage
            // 
            this.krpbPrewPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbPrewPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.krpbPrewPage.Location = new System.Drawing.Point(367, 223);
            this.krpbPrewPage.Name = "krpbPrewPage";
            this.krpbPrewPage.Size = new System.Drawing.Size(18, 25);
            this.krpbPrewPage.TabIndex = 6;
            this.krpbPrewPage.Values.Text = "";
            this.krpbPrewPage.Visible = false;
            this.krpbPrewPage.Click += new System.EventHandler(this.krpbPrewPage_Click);
            // 
            // krpbBeginPage
            // 
            this.krpbBeginPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbBeginPage.Cursor = System.Windows.Forms.Cursors.Default;
            this.krpbBeginPage.Location = new System.Drawing.Point(343, 223);
            this.krpbBeginPage.Name = "krpbBeginPage";
            this.krpbBeginPage.Size = new System.Drawing.Size(18, 25);
            this.krpbBeginPage.TabIndex = 5;
            this.krpbBeginPage.Values.Text = "";
            this.krpbBeginPage.Visible = false;
            this.krpbBeginPage.Click += new System.EventHandler(this.krpbBeginPage_Click);
            // 
            // krplSeperate
            // 
            this.krplSeperate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.krplSeperate.Location = new System.Drawing.Point(530, 226);
            this.krplSeperate.Name = "krplSeperate";
            this.krplSeperate.Size = new System.Drawing.Size(15, 20);
            this.krplSeperate.TabIndex = 8;
            this.krplSeperate.Values.Text = "/";
            this.krplSeperate.Visible = false;
            // 
            // krpContextMenu
            // 
            this.krpContextMenu.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuOrder});
            // 
            // kryptonContextMenuOrder
            // 
            this.kryptonContextMenuOrder.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItemNewOrder,
            this.kryptonContextMenuItemShowOrder,
            this.kryptonContextMenuItemCheckoutOrder,
            this.kryptonContextMenuItemCancelOrder,
            this.kryptonContextMenuItemReplaceRoom,
            this.kryptonContextMenuItemRestart,
            this.kryptonContextMenuItemShutdown});
            this.kryptonContextMenuOrder.StandardStyle = false;
            // 
            // kryptonContextMenuItemNewOrder
            // 
            this.kryptonContextMenuItemNewOrder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.kryptonContextMenuItemNewOrder.Text = "NewOrder";
            // 
            // kryptonContextMenuItemShowOrder
            // 
            this.kryptonContextMenuItemShowOrder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
            this.kryptonContextMenuItemShowOrder.Text = "ShowOrder";
            // 
            // kryptonContextMenuItemCheckoutOrder
            // 
            this.kryptonContextMenuItemCheckoutOrder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.kryptonContextMenuItemCheckoutOrder.Text = "CheckoutOrder";
            // 
            // kryptonContextMenuItemCancelOrder
            // 
            this.kryptonContextMenuItemCancelOrder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.kryptonContextMenuItemCancelOrder.Text = "CancelOrder";
            // 
            // kryptonContextMenuItemReplaceRoom
            // 
            this.kryptonContextMenuItemReplaceRoom.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.kryptonContextMenuItemReplaceRoom.Text = "ReplaceRoom";
            // 
            // kryptonContextMenuItemRestart
            // 
            this.kryptonContextMenuItemRestart.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.kryptonContextMenuItemRestart.Text = "Restart";
            this.kryptonContextMenuItemRestart.Visible = false;
            // 
            // kryptonContextMenuItemShutdown
            // 
            this.kryptonContextMenuItemShutdown.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U)));
            this.kryptonContextMenuItemShutdown.Text = "Shutdown";
            this.kryptonContextMenuItemShutdown.Visible = false;
            // 
            // krpContextMenuTakeout
            // 
            this.krpContextMenuTakeout.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuTakeout});
            // 
            // kryptonContextMenuTakeout
            // 
            this.kryptonContextMenuTakeout.Items.AddRange(new ComponentFactory.Krypton.Toolkit.KryptonContextMenuItemBase[] {
            this.kryptonContextMenuItemAddTakeout});
            this.kryptonContextMenuTakeout.StandardStyle = false;
            // 
            // kryptonContextMenuItemAddTakeout
            // 
            this.kryptonContextMenuItemAddTakeout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.kryptonContextMenuItemAddTakeout.Text = "AddTakeout";
            // 
            // kryptonContextMenuItem1
            // 
            this.kryptonContextMenuItem1.Text = "Menu Item";
            // 
            // kryptonTaskDialog
            // 
            this.kryptonTaskDialog.CheckboxText = null;
            this.kryptonTaskDialog.Content = null;
            this.kryptonTaskDialog.DefaultRadioButton = null;
            this.kryptonTaskDialog.FooterHyperlink = null;
            this.kryptonTaskDialog.FooterText = null;
            this.kryptonTaskDialog.MainInstruction = null;
            this.kryptonTaskDialog.Tag = null;
            this.kryptonTaskDialog.WindowTitle = null;
            // 
            // krpbSendFireAlarm
            // 
            this.krpbSendFireAlarm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbSendFireAlarm.Location = new System.Drawing.Point(413, 4);
            this.krpbSendFireAlarm.Name = "krpbSendFireAlarm";
            this.krpbSendFireAlarm.Size = new System.Drawing.Size(103, 25);
            this.krpbSendFireAlarm.TabIndex = 10;
            this.krpbSendFireAlarm.Values.Text = "Send Fire Alarm";
            this.krpbSendFireAlarm.Visible = false;
            this.krpbSendFireAlarm.Click += new System.EventHandler(this.krpbSendFireAlarm_Click);
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(343, 9);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel1.TabIndex = 11;
            this.kryptonLabel1.Values.Text = ":";
            this.kryptonLabel1.Visible = false;
            // 
            // krplNewCallValue
            // 
            this.krplNewCallValue.Location = new System.Drawing.Point(357, 9);
            this.krplNewCallValue.Name = "krplNewCallValue";
            this.krplNewCallValue.Size = new System.Drawing.Size(24, 20);
            this.krplNewCallValue.TabIndex = 12;
            this.krplNewCallValue.Values.Text = "00";
            this.krplNewCallValue.Visible = false;
            this.krplNewCallValue.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.krplNewCallValue_MouseDoubleClick);
            // 
            // krplNewCall
            // 
            this.krplNewCall.Location = new System.Drawing.Point(291, 9);
            this.krplNewCall.Name = "krplNewCall";
            this.krplNewCall.Size = new System.Drawing.Size(55, 20);
            this.krplNewCall.TabIndex = 13;
            this.krplNewCall.Values.Text = "NewCall";
            this.krplNewCall.Visible = false;
            // 
            // krplNewOrder
            // 
            this.krplNewOrder.Location = new System.Drawing.Point(181, 9);
            this.krplNewOrder.Name = "krplNewOrder";
            this.krplNewOrder.Size = new System.Drawing.Size(67, 20);
            this.krplNewOrder.TabIndex = 13;
            this.krplNewOrder.Values.Text = "NewOrder";
            this.krplNewOrder.Visible = false;
            // 
            // krplNewOrderValue
            // 
            this.krplNewOrderValue.Location = new System.Drawing.Point(258, 9);
            this.krplNewOrderValue.Name = "krplNewOrderValue";
            this.krplNewOrderValue.Size = new System.Drawing.Size(24, 20);
            this.krplNewOrderValue.TabIndex = 12;
            this.krplNewOrderValue.Values.Text = "00";
            this.krplNewOrderValue.Visible = false;
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(244, 9);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(13, 20);
            this.kryptonLabel4.TabIndex = 11;
            this.kryptonLabel4.Values.Text = ":";
            this.kryptonLabel4.Visible = false;
            // 
            // krpcmRemark
            // 
            this.krpcmRemark.HeaderText = "Remark";
            this.krpcmRemark.Name = "krpcmRemark";
            this.krpcmRemark.ReadOnly = true;
            this.krpcmRemark.Width = 150;
            // 
            // krpcmLang
            // 
            this.krpcmLang.HeaderText = "Lang";
            this.krpcmLang.Name = "krpcmLang";
            this.krpcmLang.ReadOnly = true;
            this.krpcmLang.Width = 100;
            // 
            // krpcmPaidPrice
            // 
            this.krpcmPaidPrice.HeaderText = "PaidPrice";
            this.krpcmPaidPrice.Name = "krpcmPaidPrice";
            this.krpcmPaidPrice.ReadOnly = true;
            this.krpcmPaidPrice.Width = 150;
            // 
            // krpcmMemberPaidPrice
            // 
            this.krpcmMemberPaidPrice.HeaderText = "MemberPaidPrice";
            this.krpcmMemberPaidPrice.Name = "krpcmMemberPaidPrice";
            this.krpcmMemberPaidPrice.ReadOnly = true;
            this.krpcmMemberPaidPrice.Visible = false;
            this.krpcmMemberPaidPrice.Width = 150;
            // 
            // krpcmBalancePrice
            // 
            this.krpcmBalancePrice.HeaderText = "BalancePrice";
            this.krpcmBalancePrice.Name = "krpcmBalancePrice";
            this.krpcmBalancePrice.ReadOnly = true;
            this.krpcmBalancePrice.Width = 150;
            // 
            // krpcmTotalPrice
            // 
            this.krpcmTotalPrice.HeaderText = "TotalPrice";
            this.krpcmTotalPrice.Name = "krpcmTotalPrice";
            this.krpcmTotalPrice.ReadOnly = true;
            this.krpcmTotalPrice.Width = 150;
            // 
            // krpcmState
            // 
            this.krpcmState.HeaderText = "State";
            this.krpcmState.Name = "krpcmState";
            this.krpcmState.ReadOnly = true;
            this.krpcmState.Visible = false;
            this.krpcmState.Width = 150;
            // 
            // krpcmEndTime
            // 
            this.krpcmEndTime.HeaderText = "EndTime";
            this.krpcmEndTime.Name = "krpcmEndTime";
            this.krpcmEndTime.ReadOnly = true;
            this.krpcmEndTime.Width = 150;
            // 
            // krpcmStartTime
            // 
            this.krpcmStartTime.HeaderText = "StartTime";
            this.krpcmStartTime.Name = "krpcmStartTime";
            this.krpcmStartTime.ReadOnly = true;
            this.krpcmStartTime.Width = 150;
            // 
            // krpcmRoomNo
            // 
            this.krpcmRoomNo.Frozen = true;
            this.krpcmRoomNo.HeaderText = "RoomNo";
            this.krpcmRoomNo.Name = "krpcmRoomNo";
            this.krpcmRoomNo.ReadOnly = true;
            this.krpcmRoomNo.Width = 120;
            // 
            // krpcmRoomId
            // 
            this.krpcmRoomId.HeaderText = "RoomId";
            this.krpcmRoomId.Name = "krpcmRoomId";
            this.krpcmRoomId.Visible = false;
            this.krpcmRoomId.Width = 20;
            // 
            // krpcmOrderId
            // 
            this.krpcmOrderId.HeaderText = "OrderId";
            this.krpcmOrderId.Name = "krpcmOrderId";
            this.krpcmOrderId.Visible = false;
            this.krpcmOrderId.Width = 20;
            // 
            // krpcmOrderSession
            // 
            this.krpcmOrderSession.HeaderText = "OrderSession";
            this.krpcmOrderSession.Name = "krpcmOrderSession";
            this.krpcmOrderSession.Visible = false;
            this.krpcmOrderSession.Width = 20;
            // 
            // krpcmOrder
            // 
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Red;
            this.krpcmOrder.DefaultCellStyle = dataGridViewCellStyle1;
            this.krpcmOrder.Frozen = true;
            this.krpcmOrder.HeaderText = "Order";
            this.krpcmOrder.Name = "krpcmOrder";
            this.krpcmOrder.ReadOnly = true;
            this.krpcmOrder.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.krpcmOrder.Visible = false;
            this.krpcmOrder.Width = 70;
            // 
            // krpcmCall
            // 
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Red;
            this.krpcmCall.DefaultCellStyle = dataGridViewCellStyle2;
            this.krpcmCall.Frozen = true;
            this.krpcmCall.HeaderText = "Call";
            this.krpcmCall.Name = "krpcmCall";
            this.krpcmCall.ReadOnly = true;
            this.krpcmCall.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.krpcmCall.Visible = false;
            this.krpcmCall.Width = 70;
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
            this.krpcmCall,
            this.krpcmOrder,
            this.krpcmOrderSession,
            this.krpcmOrderId,
            this.krpcmRoomId,
            this.krpcmRoomNo,
            this.krpcmStartTime,
            this.krpcmEndTime,
            this.krpcmState,
            this.krpcmTotalPrice,
            this.krpcmPaidPrice,
            this.krpcmMemberPaidPrice,
            this.krpcmBalancePrice,
            this.krpcmLang,
            this.krpcmRemark});
            this.krpdgList.Location = new System.Drawing.Point(12, 35);
            this.krpdgList.MultiSelect = false;
            this.krpdgList.Name = "krpdgList";
            this.krpdgList.RowTemplate.Height = 23;
            this.krpdgList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.krpdgList.Size = new System.Drawing.Size(610, 215);
            this.krpdgList.TabIndex = 9;
            this.krpdgList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.krpdgList_CellMouseClick);
            this.krpdgList.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.krpdgList_RowPostPaint);
            this.krpdgList.RowStateChanged += new System.Windows.Forms.DataGridViewRowStateChangedEventHandler(this.krpdgList_RowStateChanged);
            this.krpdgList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.krpdgList_KeyDown);
            this.krpdgList.KeyUp += new System.Windows.Forms.KeyEventHandler(this.krpdgList_KeyUp);
            this.krpdgList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.krpdgList_MouseClick);
            this.krpdgList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.krpdgList_MouseDoubleClick);
            // 
            // krpbImageMode
            // 
            this.krpbImageMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbImageMode.Location = new System.Drawing.Point(590, 1);
            this.krpbImageMode.Name = "krpbImageMode";
            this.krpbImageMode.Size = new System.Drawing.Size(32, 32);
            this.krpbImageMode.TabIndex = 24;
            this.krpbImageMode.Values.Text = "";
            this.krpbImageMode.Click += new System.EventHandler(this.krpbMode_Click);
            // 
            // krpbListMode
            // 
            this.krpbListMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.krpbListMode.Enabled = false;
            this.krpbListMode.Location = new System.Drawing.Point(552, 1);
            this.krpbListMode.Name = "krpbListMode";
            this.krpbListMode.Size = new System.Drawing.Size(32, 32);
            this.krpbListMode.TabIndex = 25;
            this.krpbListMode.Values.Text = "";
            this.krpbListMode.Click += new System.EventHandler(this.krpbMode_Click);
            // 
            // krpbAddTakeout
            // 
            this.krpbAddTakeout.Location = new System.Drawing.Point(12, 1);
            this.krpbAddTakeout.Name = "krpbAddTakeout";
            this.krpbAddTakeout.Size = new System.Drawing.Size(32, 32);
            this.krpbAddTakeout.TabIndex = 26;
            this.krpbAddTakeout.Values.Text = "";
            this.krpbAddTakeout.Click += new System.EventHandler(this.krpbAddTakeout_Click);
            // 
            // krpbPos
            // 
            this.krpbPos.Location = new System.Drawing.Point(50, 1);
            this.krpbPos.Name = "krpbPos";
            this.krpbPos.Size = new System.Drawing.Size(32, 32);
            this.krpbPos.TabIndex = 26;
            this.krpbPos.Values.Text = "";
            this.krpbPos.Click += new System.EventHandler(this.krpbPos_Click);
            // 
            // tlpTakeout
            // 
            this.tlpTakeout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpTakeout.ColumnCount = 1;
            this.tlpTakeout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpTakeout.Location = new System.Drawing.Point(12, 35);
            this.tlpTakeout.Name = "tlpTakeout";
            this.tlpTakeout.RowCount = 1;
            this.tlpTakeout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpTakeout.Size = new System.Drawing.Size(610, 215);
            this.tlpTakeout.TabIndex = 27;
            // 
            // flpRooms
            // 
            this.flpRooms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flpRooms.AutoScroll = true;
            this.flpRooms.Location = new System.Drawing.Point(12, 35);
            this.flpRooms.Name = "flpRooms";
            this.flpRooms.Size = new System.Drawing.Size(610, 215);
            this.flpRooms.TabIndex = 15;
            // 
            // HomeWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(634, 262);
            this.Controls.Add(this.tlpTakeout);
            this.Controls.Add(this.krpbPos);
            this.Controls.Add(this.krpbAddTakeout);
            this.Controls.Add(this.flpRooms);
            this.Controls.Add(this.krpbImageMode);
            this.Controls.Add(this.krpbListMode);
            this.Controls.Add(this.kryptonLabel4);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.krplNewOrderValue);
            this.Controls.Add(this.krplNewCallValue);
            this.Controls.Add(this.krplNewOrder);
            this.Controls.Add(this.krplNewCall);
            this.Controls.Add(this.krpbSendFireAlarm);
            this.Controls.Add(this.krplSeperate);
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
            this.Name = "HomeWindow";
            this.Text = "HomeWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpdgList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenu krpContextMenuTakeout;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuTakeout;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems2;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItem1;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems4;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItems kryptonContextMenuItems5;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemShowOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemNewOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemReplaceRoom;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemCheckoutOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemCancelOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemShutdown;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemAddTakeout;
        private ComponentFactory.Krypton.Toolkit.KryptonContextMenuItem kryptonContextMenuItemRestart;
        private ComponentFactory.Krypton.Toolkit.KryptonTaskDialog kryptonTaskDialog;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbSendFireAlarm;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewCallValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewCall;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel krplNewOrderValue;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRemark;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmLang;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmMemberPaidPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmBalancePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmTotalPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmState;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmEndTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmStartTime;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRoomNo;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmRoomId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOrderId;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOrderSession;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridViewTextBoxColumn krpcmCall;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView krpdgList;
        private Oybab.ServicePC.Tools.CustomFlowLayoutPanel flpRooms;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbImageMode;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbListMode;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbAddTakeout;
        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbPos;
        private System.Windows.Forms.TableLayoutPanel tlpTakeout;
    }
}