namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class MenuListWindow
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
            this.krpbChange = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.krpMenu = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpcbProductManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbProductTypeManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbRoomManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbMemberManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbExpenditureManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbOrderManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbInnerBill = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbChangeSet = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbOuterBill = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpSystem = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpcbAdminManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbDeviceManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbPrinterManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbBalance = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbFinanceLog = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbStatistic = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbRequest = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbAdminLog = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbSupplierManager = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbIncomeTradingManage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpIncomePermission = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpcbChangeLanguage = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbBindMemberByNo = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbReturnMoney = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbDecreaseProductCount = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbDeleteProduct = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbReplaceRoom = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbCancelOrder = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbTemporaryChangePrice = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbOpenCashDrawer = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpExpenditurePermission = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpcbBindSupplierByNo = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbChangeUnitPrice = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbChangeCostPrice = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krptPlatform = new ComponentFactory.Krypton.Toolkit.KryptonGroupBox();
            this.krpcbPhone = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbTablet = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.krpcbComputer = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.krpMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpMenu.Panel)).BeginInit();
            this.krpMenu.Panel.SuspendLayout();
            this.krpMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem.Panel)).BeginInit();
            this.krpSystem.Panel.SuspendLayout();
            this.krpSystem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpIncomePermission)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpIncomePermission.Panel)).BeginInit();
            this.krpIncomePermission.Panel.SuspendLayout();
            this.krpIncomePermission.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpExpenditurePermission)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krpExpenditurePermission.Panel)).BeginInit();
            this.krpExpenditurePermission.Panel.SuspendLayout();
            this.krpExpenditurePermission.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krptPlatform)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.krptPlatform.Panel)).BeginInit();
            this.krptPlatform.Panel.SuspendLayout();
            this.krptPlatform.SuspendLayout();
            this.SuspendLayout();
            // 
            // krpbChange
            // 
            this.krpbChange.Location = new System.Drawing.Point(358, 526);
            this.krpbChange.Name = "krpbChange";
            this.krpbChange.Size = new System.Drawing.Size(90, 25);
            this.krpbChange.TabIndex = 100;
            this.krpbChange.Values.Text = "Change";
            this.krpbChange.Click += new System.EventHandler(this.krpbChange_Click);
            // 
            // krpMenu
            // 
            this.krpMenu.Location = new System.Drawing.Point(22, 88);
            this.krpMenu.Name = "krpMenu";
            // 
            // krpMenu.Panel
            // 
            this.krpMenu.Panel.Controls.Add(this.krpcbProductManager);
            this.krpMenu.Panel.Controls.Add(this.krpcbProductTypeManager);
            this.krpMenu.Panel.Controls.Add(this.krpcbRoomManager);
            this.krpMenu.Panel.Controls.Add(this.krpcbMemberManager);
            this.krpMenu.Panel.Controls.Add(this.krpcbExpenditureManager);
            this.krpMenu.Panel.Controls.Add(this.krpcbOrderManager);
            this.krpMenu.Panel.Controls.Add(this.krpcbInnerBill);
            this.krpMenu.Panel.Controls.Add(this.krpcbChangeSet);
            this.krpMenu.Panel.Controls.Add(this.krpcbOuterBill);
            this.krpMenu.Size = new System.Drawing.Size(236, 423);
            this.krpMenu.TabIndex = 5;
            this.krpMenu.Text = "Menu";
            this.krpMenu.Values.Heading = "Menu";
            // 
            // krpcbProductManager
            // 
            this.krpcbProductManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbProductManager.Location = new System.Drawing.Point(24, 312);
            this.krpcbProductManager.Name = "krpcbProductManager";
            this.krpcbProductManager.Size = new System.Drawing.Size(115, 20);
            this.krpcbProductManager.TabIndex = 17;
            this.krpcbProductManager.Text = "ProductManager";
            this.krpcbProductManager.Values.Text = "ProductManager";
            // 
            // krpcbProductTypeManager
            // 
            this.krpcbProductTypeManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbProductTypeManager.Location = new System.Drawing.Point(24, 270);
            this.krpcbProductTypeManager.Name = "krpcbProductTypeManager";
            this.krpcbProductTypeManager.Size = new System.Drawing.Size(141, 20);
            this.krpcbProductTypeManager.TabIndex = 16;
            this.krpcbProductTypeManager.Text = "ProductTypeManager";
            this.krpcbProductTypeManager.Values.Text = "ProductTypeManager";
            // 
            // krpcbRoomManager
            // 
            this.krpcbRoomManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbRoomManager.Location = new System.Drawing.Point(24, 228);
            this.krpcbRoomManager.Name = "krpcbRoomManager";
            this.krpcbRoomManager.Size = new System.Drawing.Size(105, 20);
            this.krpcbRoomManager.TabIndex = 15;
            this.krpcbRoomManager.Text = "RoomManager";
            this.krpcbRoomManager.Values.Text = "RoomManager";
            // 
            // krpcbMemberManager
            // 
            this.krpcbMemberManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbMemberManager.Location = new System.Drawing.Point(24, 186);
            this.krpcbMemberManager.Name = "krpcbMemberManager";
            this.krpcbMemberManager.Size = new System.Drawing.Size(118, 20);
            this.krpcbMemberManager.TabIndex = 14;
            this.krpcbMemberManager.Text = "MemberManager";
            this.krpcbMemberManager.Values.Text = "MemberManager";
            // 
            // krpcbExpenditureManager
            // 
            this.krpcbExpenditureManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbExpenditureManager.Location = new System.Drawing.Point(24, 144);
            this.krpcbExpenditureManager.Name = "krpcbExpenditureManager";
            this.krpcbExpenditureManager.Size = new System.Drawing.Size(137, 20);
            this.krpcbExpenditureManager.TabIndex = 13;
            this.krpcbExpenditureManager.Text = "ExpenditureManager";
            this.krpcbExpenditureManager.Values.Text = "ExpenditureManager";
            // 
            // krpcbOrderManager
            // 
            this.krpcbOrderManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbOrderManager.Location = new System.Drawing.Point(24, 102);
            this.krpcbOrderManager.Name = "krpcbOrderManager";
            this.krpcbOrderManager.Size = new System.Drawing.Size(104, 20);
            this.krpcbOrderManager.TabIndex = 12;
            this.krpcbOrderManager.Text = "OrderManager";
            this.krpcbOrderManager.Values.Text = "OrderManager";
            // 
            // krpcbInnerBill
            // 
            this.krpcbInnerBill.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbInnerBill.Location = new System.Drawing.Point(24, 18);
            this.krpcbInnerBill.Name = "krpcbInnerBill";
            this.krpcbInnerBill.Size = new System.Drawing.Size(71, 20);
            this.krpcbInnerBill.TabIndex = 11;
            this.krpcbInnerBill.Text = "Inner Bill";
            this.krpcbInnerBill.Values.Text = "Inner Bill";
            // 
            // krpcbChangeSet
            // 
            this.krpcbChangeSet.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbChangeSet.Location = new System.Drawing.Point(24, 354);
            this.krpcbChangeSet.Name = "krpcbChangeSet";
            this.krpcbChangeSet.Size = new System.Drawing.Size(82, 20);
            this.krpcbChangeSet.TabIndex = 18;
            this.krpcbChangeSet.Text = "ChangeSet";
            this.krpcbChangeSet.Values.Text = "ChangeSet";
            // 
            // krpcbOuterBill
            // 
            this.krpcbOuterBill.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbOuterBill.Location = new System.Drawing.Point(24, 60);
            this.krpcbOuterBill.Name = "krpcbOuterBill";
            this.krpcbOuterBill.Size = new System.Drawing.Size(74, 20);
            this.krpcbOuterBill.TabIndex = 80;
            this.krpcbOuterBill.Text = "Outer Bill";
            this.krpcbOuterBill.Values.Text = "Outer Bill";
            // 
            // krpSystem
            // 
            this.krpSystem.Location = new System.Drawing.Point(281, 88);
            this.krpSystem.Name = "krpSystem";
            // 
            // krpSystem.Panel
            // 
            this.krpSystem.Panel.Controls.Add(this.krpcbAdminManager);
            this.krpSystem.Panel.Controls.Add(this.krpcbDeviceManager);
            this.krpSystem.Panel.Controls.Add(this.krpcbPrinterManager);
            this.krpSystem.Panel.Controls.Add(this.krpcbBalance);
            this.krpSystem.Panel.Controls.Add(this.krpcbFinanceLog);
            this.krpSystem.Panel.Controls.Add(this.krpcbStatistic);
            this.krpSystem.Panel.Controls.Add(this.krpcbRequest);
            this.krpSystem.Panel.Controls.Add(this.krpcbAdminLog);
            this.krpSystem.Panel.Controls.Add(this.krpcbSupplierManager);
            this.krpSystem.Size = new System.Drawing.Size(248, 423);
            this.krpSystem.TabIndex = 6;
            this.krpSystem.Text = "System";
            this.krpSystem.Values.Heading = "System";
            // 
            // krpcbAdminManager
            // 
            this.krpcbAdminManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbAdminManager.Location = new System.Drawing.Point(24, 18);
            this.krpcbAdminManager.Name = "krpcbAdminManager";
            this.krpcbAdminManager.Size = new System.Drawing.Size(108, 20);
            this.krpcbAdminManager.TabIndex = 21;
            this.krpcbAdminManager.Text = "AdminManager";
            this.krpcbAdminManager.Values.Text = "AdminManager";
            // 
            // krpcbDeviceManager
            // 
            this.krpcbDeviceManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbDeviceManager.Location = new System.Drawing.Point(24, 60);
            this.krpcbDeviceManager.Name = "krpcbDeviceManager";
            this.krpcbDeviceManager.Size = new System.Drawing.Size(108, 20);
            this.krpcbDeviceManager.TabIndex = 22;
            this.krpcbDeviceManager.Text = "DeviceManager";
            this.krpcbDeviceManager.Values.Text = "DeviceManager";
            // 
            // krpcbPrinterManager
            // 
            this.krpcbPrinterManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbPrinterManager.Location = new System.Drawing.Point(24, 102);
            this.krpcbPrinterManager.Name = "krpcbPrinterManager";
            this.krpcbPrinterManager.Size = new System.Drawing.Size(108, 20);
            this.krpcbPrinterManager.TabIndex = 23;
            this.krpcbPrinterManager.Text = "PrinterManager";
            this.krpcbPrinterManager.Values.Text = "PrinterManager";
            // 
            // krpcbBalance
            // 
            this.krpcbBalance.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbBalance.Location = new System.Drawing.Point(24, 270);
            this.krpcbBalance.Name = "krpcbBalance";
            this.krpcbBalance.Size = new System.Drawing.Size(65, 20);
            this.krpcbBalance.TabIndex = 28;
            this.krpcbBalance.Text = "Balance";
            this.krpcbBalance.Values.Text = "Balance";
            // 
            // krpcbFinanceLog
            // 
            this.krpcbFinanceLog.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbFinanceLog.Location = new System.Drawing.Point(24, 312);
            this.krpcbFinanceLog.Name = "krpcbFinanceLog";
            this.krpcbFinanceLog.Size = new System.Drawing.Size(85, 20);
            this.krpcbFinanceLog.TabIndex = 29;
            this.krpcbFinanceLog.Text = "FinanceLog";
            this.krpcbFinanceLog.Values.Text = "FinanceLog";
            // 
            // krpcbStatistic
            // 
            this.krpcbStatistic.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbStatistic.Location = new System.Drawing.Point(24, 354);
            this.krpcbStatistic.Name = "krpcbStatistic";
            this.krpcbStatistic.Size = new System.Drawing.Size(66, 20);
            this.krpcbStatistic.TabIndex = 30;
            this.krpcbStatistic.Text = "Statistic";
            this.krpcbStatistic.Values.Text = "Statistic";
            // 
            // krpcbRequest
            // 
            this.krpcbRequest.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbRequest.Location = new System.Drawing.Point(24, 186);
            this.krpcbRequest.Name = "krpcbRequest";
            this.krpcbRequest.Size = new System.Drawing.Size(116, 20);
            this.krpcbRequest.TabIndex = 26;
            this.krpcbRequest.Text = "RequestManager";
            this.krpcbRequest.Values.Text = "RequestManager";
            // 
            // krpcbAdminLog
            // 
            this.krpcbAdminLog.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbAdminLog.Location = new System.Drawing.Point(24, 228);
            this.krpcbAdminLog.Name = "krpcbAdminLog";
            this.krpcbAdminLog.Size = new System.Drawing.Size(80, 20);
            this.krpcbAdminLog.TabIndex = 27;
            this.krpcbAdminLog.Text = "AdminLog";
            this.krpcbAdminLog.Values.Text = "AdminLog";
            // 
            // krpcbSupplierManager
            // 
            this.krpcbSupplierManager.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbSupplierManager.Location = new System.Drawing.Point(24, 144);
            this.krpcbSupplierManager.Name = "krpcbSupplierManager";
            this.krpcbSupplierManager.Size = new System.Drawing.Size(117, 20);
            this.krpcbSupplierManager.TabIndex = 24;
            this.krpcbSupplierManager.Text = "SupplierManager";
            this.krpcbSupplierManager.Values.Text = "SupplierManager";
            // 
            // krpcbIncomeTradingManage
            // 
            this.krpcbIncomeTradingManage.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbIncomeTradingManage.Location = new System.Drawing.Point(20, 44);
            this.krpcbIncomeTradingManage.Name = "krpcbIncomeTradingManage";
            this.krpcbIncomeTradingManage.Size = new System.Drawing.Size(157, 20);
            this.krpcbIncomeTradingManage.TabIndex = 81;
            this.krpcbIncomeTradingManage.Text = "Income Trading Manage";
            this.krpcbIncomeTradingManage.Values.Text = "Income Trading Manage";
            // 
            // krpIncomePermission
            // 
            this.krpIncomePermission.Location = new System.Drawing.Point(558, 13);
            this.krpIncomePermission.Name = "krpIncomePermission";
            // 
            // krpIncomePermission.Panel
            // 
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbChangeLanguage);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbBindMemberByNo);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbReturnMoney);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbDecreaseProductCount);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbDeleteProduct);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbReplaceRoom);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbCancelOrder);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbTemporaryChangePrice);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbOpenCashDrawer);
            this.krpIncomePermission.Panel.Controls.Add(this.krpcbIncomeTradingManage);
            this.krpIncomePermission.Size = new System.Drawing.Size(233, 375);
            this.krpIncomePermission.TabIndex = 81;
            this.krpIncomePermission.Text = "Income Permission";
            this.krpIncomePermission.Values.Heading = "Income Permission";
            // 
            // krpcbChangeLanguage
            // 
            this.krpcbChangeLanguage.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbChangeLanguage.Location = new System.Drawing.Point(20, 11);
            this.krpcbChangeLanguage.Name = "krpcbChangeLanguage";
            this.krpcbChangeLanguage.Size = new System.Drawing.Size(122, 20);
            this.krpcbChangeLanguage.TabIndex = 70;
            this.krpcbChangeLanguage.Text = "Change Language";
            this.krpcbChangeLanguage.Values.Text = "Change Language";
            // 
            // krpcbBindMemberByNo
            // 
            this.krpcbBindMemberByNo.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbBindMemberByNo.Location = new System.Drawing.Point(20, 292);
            this.krpcbBindMemberByNo.Name = "krpcbBindMemberByNo";
            this.krpcbBindMemberByNo.Size = new System.Drawing.Size(134, 20);
            this.krpcbBindMemberByNo.TabIndex = 89;
            this.krpcbBindMemberByNo.Text = "Bind Member By No";
            this.krpcbBindMemberByNo.Values.Text = "Bind Member By No";
            // 
            // krpcbReturnMoney
            // 
            this.krpcbReturnMoney.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbReturnMoney.Location = new System.Drawing.Point(20, 261);
            this.krpcbReturnMoney.Name = "krpcbReturnMoney";
            this.krpcbReturnMoney.Size = new System.Drawing.Size(101, 20);
            this.krpcbReturnMoney.TabIndex = 88;
            this.krpcbReturnMoney.Text = "Return Money";
            this.krpcbReturnMoney.Values.Text = "Return Money";
            // 
            // krpcbDecreaseProductCount
            // 
            this.krpcbDecreaseProductCount.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbDecreaseProductCount.Location = new System.Drawing.Point(20, 230);
            this.krpcbDecreaseProductCount.Name = "krpcbDecreaseProductCount";
            this.krpcbDecreaseProductCount.Size = new System.Drawing.Size(156, 20);
            this.krpcbDecreaseProductCount.TabIndex = 87;
            this.krpcbDecreaseProductCount.Text = "Decrease Product Count";
            this.krpcbDecreaseProductCount.Values.Text = "Decrease Product Count";
            // 
            // krpcbDeleteProduct
            // 
            this.krpcbDeleteProduct.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbDeleteProduct.Location = new System.Drawing.Point(20, 199);
            this.krpcbDeleteProduct.Name = "krpcbDeleteProduct";
            this.krpcbDeleteProduct.Size = new System.Drawing.Size(105, 20);
            this.krpcbDeleteProduct.TabIndex = 86;
            this.krpcbDeleteProduct.Text = "Delete Product";
            this.krpcbDeleteProduct.Values.Text = "Delete Product";
            // 
            // krpcbReplaceRoom
            // 
            this.krpcbReplaceRoom.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbReplaceRoom.Location = new System.Drawing.Point(20, 137);
            this.krpcbReplaceRoom.Name = "krpcbReplaceRoom";
            this.krpcbReplaceRoom.Size = new System.Drawing.Size(102, 20);
            this.krpcbReplaceRoom.TabIndex = 84;
            this.krpcbReplaceRoom.Text = "Replace Room";
            this.krpcbReplaceRoom.Values.Text = "Replace Room";
            // 
            // krpcbCancelOrder
            // 
            this.krpcbCancelOrder.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbCancelOrder.Location = new System.Drawing.Point(20, 168);
            this.krpcbCancelOrder.Name = "krpcbCancelOrder";
            this.krpcbCancelOrder.Size = new System.Drawing.Size(95, 20);
            this.krpcbCancelOrder.TabIndex = 85;
            this.krpcbCancelOrder.Text = "Cancel Order";
            this.krpcbCancelOrder.Values.Text = "Cancel Order";
            // 
            // krpcbTemporaryChangePrice
            // 
            this.krpcbTemporaryChangePrice.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbTemporaryChangePrice.Location = new System.Drawing.Point(20, 106);
            this.krpcbTemporaryChangePrice.Name = "krpcbTemporaryChangePrice";
            this.krpcbTemporaryChangePrice.Size = new System.Drawing.Size(157, 20);
            this.krpcbTemporaryChangePrice.TabIndex = 83;
            this.krpcbTemporaryChangePrice.Text = "Temporary Change Price";
            this.krpcbTemporaryChangePrice.Values.Text = "Temporary Change Price";
            // 
            // krpcbOpenCashDrawer
            // 
            this.krpcbOpenCashDrawer.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbOpenCashDrawer.Location = new System.Drawing.Point(20, 75);
            this.krpcbOpenCashDrawer.Name = "krpcbOpenCashDrawer";
            this.krpcbOpenCashDrawer.Size = new System.Drawing.Size(125, 20);
            this.krpcbOpenCashDrawer.TabIndex = 82;
            this.krpcbOpenCashDrawer.Text = "Open Cash Drawer";
            this.krpcbOpenCashDrawer.Values.Text = "Open Cash Drawer";
            // 
            // krpExpenditurePermission
            // 
            this.krpExpenditurePermission.Location = new System.Drawing.Point(558, 394);
            this.krpExpenditurePermission.Name = "krpExpenditurePermission";
            // 
            // krpExpenditurePermission.Panel
            // 
            this.krpExpenditurePermission.Panel.Controls.Add(this.krpcbBindSupplierByNo);
            this.krpExpenditurePermission.Panel.Controls.Add(this.krpcbChangeUnitPrice);
            this.krpExpenditurePermission.Panel.Controls.Add(this.krpcbChangeCostPrice);
            this.krpExpenditurePermission.Size = new System.Drawing.Size(233, 117);
            this.krpExpenditurePermission.TabIndex = 81;
            this.krpExpenditurePermission.Text = "Expend Permission";
            this.krpExpenditurePermission.Values.Heading = "Expend Permission";
            // 
            // krpcbBindSupplierByNo
            // 
            this.krpcbBindSupplierByNo.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbBindSupplierByNo.Location = new System.Drawing.Point(20, 67);
            this.krpcbBindSupplierByNo.Name = "krpcbBindSupplierByNo";
            this.krpcbBindSupplierByNo.Size = new System.Drawing.Size(132, 20);
            this.krpcbBindSupplierByNo.TabIndex = 92;
            this.krpcbBindSupplierByNo.Text = "Bind Supplier By No";
            this.krpcbBindSupplierByNo.Values.Text = "Bind Supplier By No";
            // 
            // krpcbChangeUnitPrice
            // 
            this.krpcbChangeUnitPrice.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbChangeUnitPrice.Location = new System.Drawing.Point(20, 37);
            this.krpcbChangeUnitPrice.Name = "krpcbChangeUnitPrice";
            this.krpcbChangeUnitPrice.Size = new System.Drawing.Size(121, 20);
            this.krpcbChangeUnitPrice.TabIndex = 91;
            this.krpcbChangeUnitPrice.Text = "Change Unit Price";
            this.krpcbChangeUnitPrice.Values.Text = "Change Unit Price";
            // 
            // krpcbChangeCostPrice
            // 
            this.krpcbChangeCostPrice.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbChangeCostPrice.Location = new System.Drawing.Point(20, 6);
            this.krpcbChangeCostPrice.Name = "krpcbChangeCostPrice";
            this.krpcbChangeCostPrice.Size = new System.Drawing.Size(123, 20);
            this.krpcbChangeCostPrice.TabIndex = 90;
            this.krpcbChangeCostPrice.Text = "Change Cost Price";
            this.krpcbChangeCostPrice.Values.Text = "Change Cost Price";
            // 
            // krptPlatform
            // 
            this.krptPlatform.Location = new System.Drawing.Point(22, 13);
            this.krptPlatform.Name = "krptPlatform";
            // 
            // krptPlatform.Panel
            // 
            this.krptPlatform.Panel.Controls.Add(this.krpcbPhone);
            this.krptPlatform.Panel.Controls.Add(this.krpcbTablet);
            this.krptPlatform.Panel.Controls.Add(this.krpcbComputer);
            this.krptPlatform.Size = new System.Drawing.Size(507, 69);
            this.krptPlatform.TabIndex = 101;
            this.krptPlatform.Text = "Platform";
            this.krptPlatform.Values.Heading = "Platform";
            // 
            // krpcbPhone
            // 
            this.krpcbPhone.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbPhone.Location = new System.Drawing.Point(399, 11);
            this.krpcbPhone.Name = "krpcbPhone";
            this.krpcbPhone.Size = new System.Drawing.Size(58, 20);
            this.krpcbPhone.TabIndex = 3;
            this.krpcbPhone.Text = "Phone";
            this.krpcbPhone.Values.Text = "Phone";
            // 
            // krpcbTablet
            // 
            this.krpcbTablet.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbTablet.Location = new System.Drawing.Point(225, 11);
            this.krpcbTablet.Name = "krpcbTablet";
            this.krpcbTablet.Size = new System.Drawing.Size(57, 20);
            this.krpcbTablet.TabIndex = 2;
            this.krpcbTablet.Text = "Tablet";
            this.krpcbTablet.Values.Text = "Tablet";
            // 
            // krpcbComputer
            // 
            this.krpcbComputer.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.NormalControl;
            this.krpcbComputer.Location = new System.Drawing.Point(38, 11);
            this.krpcbComputer.Name = "krpcbComputer";
            this.krpcbComputer.Size = new System.Drawing.Size(78, 20);
            this.krpcbComputer.TabIndex = 1;
            this.krpcbComputer.Text = "Computer";
            this.krpcbComputer.Values.Text = "Computer";
            // 
            // MenuListWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(826, 561);
            this.Controls.Add(this.krptPlatform);
            this.Controls.Add(this.krpExpenditurePermission);
            this.Controls.Add(this.krpIncomePermission);
            this.Controls.Add(this.krpSystem);
            this.Controls.Add(this.krpMenu);
            this.Controls.Add(this.krpbChange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MenuListWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MenuListWindow";
            ((System.ComponentModel.ISupportInitialize)(this.krpMenu.Panel)).EndInit();
            this.krpMenu.Panel.ResumeLayout(false);
            this.krpMenu.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpMenu)).EndInit();
            this.krpMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem.Panel)).EndInit();
            this.krpSystem.Panel.ResumeLayout(false);
            this.krpSystem.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpSystem)).EndInit();
            this.krpSystem.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpIncomePermission.Panel)).EndInit();
            this.krpIncomePermission.Panel.ResumeLayout(false);
            this.krpIncomePermission.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpIncomePermission)).EndInit();
            this.krpIncomePermission.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krpExpenditurePermission.Panel)).EndInit();
            this.krpExpenditurePermission.Panel.ResumeLayout(false);
            this.krpExpenditurePermission.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krpExpenditurePermission)).EndInit();
            this.krpExpenditurePermission.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.krptPlatform.Panel)).EndInit();
            this.krptPlatform.Panel.ResumeLayout(false);
            this.krptPlatform.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.krptPlatform)).EndInit();
            this.krptPlatform.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonButton krpbChange;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpMenu;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbProductManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbProductTypeManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbRoomManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbMemberManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbExpenditureManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbOrderManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbInnerBill;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbChangeSet;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpSystem;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbAdminManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbDeviceManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbPrinterManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbStatistic;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbAdminLog;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbSupplierManager;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbFinanceLog;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbIncomeTradingManage;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpIncomePermission;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbRequest;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbBalance;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbTemporaryChangePrice;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbCancelOrder;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krpExpenditurePermission;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbChangeUnitPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbChangeCostPrice;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbBindMemberByNo;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbBindSupplierByNo;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbDeleteProduct;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbOuterBill;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbOpenCashDrawer;
        private ComponentFactory.Krypton.Toolkit.KryptonGroupBox krptPlatform;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbPhone;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbTablet;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbComputer;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbDecreaseProductCount;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbReplaceRoom;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbChangeLanguage;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox krpcbReturnMoney;
    }
}