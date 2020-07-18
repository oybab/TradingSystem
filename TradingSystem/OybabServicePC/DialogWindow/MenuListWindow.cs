using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using System.IO;
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class MenuListWindow : KryptonForm
    {
        public string ReturnValue { get; private set; } //返回值
        public MenuListWindow(string Menus, Admin admin = null)
        {
            InitializeComponent();

            this.Text = Resources.GetRes().GetString("PermissionsMenu");

            if (null != admin)
                this.Text = this.Text + " - " + admin.AdminNo;

            krptPlatform.Text = Resources.GetRes().GetString("Platform");
            krpcbComputer.Text = Resources.GetRes().GetString("Computer");
            krpcbTablet.Text = Resources.GetRes().GetString("Tablet");
            krpcbPhone.Text = Resources.GetRes().GetString("Phone");
            krpcbChangeLanguage.Text = Resources.GetRes().GetString("ChangeLanguage");
            krpbChange.Text = Resources.GetRes().GetString("Change");
            krpMenu.Text = Resources.GetRes().GetString("Menu");
            krpSystem.Text = Resources.GetRes().GetString("System");

            krpcbInnerBill.Text = Resources.GetRes().GetString("InnerBill");
            krpcbOrderManager.Text = Resources.GetRes().GetString("IncomeManager");
            krpcbExpenditureManager.Text = Resources.GetRes().GetString("ExpenditureManager");
            krpcbMemberManager.Text = Resources.GetRes().GetString("MemberManager");
            krpcbRoomManager.Text = Resources.GetRes().GetString("RoomManager");
            krpcbProductTypeManager.Text = Resources.GetRes().GetString("ProductTypeManager");
            krpcbProductManager.Text = Resources.GetRes().GetString("ProductManager");
            krpcbAdminManager.Text = Resources.GetRes().GetString("AdminManager");
            krpcbDeviceManager.Text = Resources.GetRes().GetString("DeviceManager");
            krpcbPrinterManager.Text = Resources.GetRes().GetString("PrinterManager");
            krpcbChangeSet.Text = Resources.GetRes().GetString("ChangeSet");
            krpcbStatistic.Text = Resources.GetRes().GetString("Statistic");
            krpcbFinanceLog.Text = Resources.GetRes().GetString("FinanceLog");
            krpcbSupplierManager.Text = Resources.GetRes().GetString("SupplierManager");
            krpcbAdminLog.Text = Resources.GetRes().GetString("AdminLog");
            krpcbIncomeTradingManage.Text = Resources.GetRes().GetString("IncomeTradingManage");
            krpIncomePermission.Text = Resources.GetRes().GetString("IncomePermission");
            krpExpenditurePermission.Text = Resources.GetRes().GetString("ExpenditurePermission");
            krpcbRequest.Text = Resources.GetRes().GetString("RequestManager");
            krpcbBalance.Text = Resources.GetRes().GetString("BalanceManager");
            krpcbTemporaryChangePrice.Text = Resources.GetRes().GetString("TemporaryChangePrice");
            krpcbReplaceRoom.Text = Resources.GetRes().GetString("ReplaceRoom");
            krpcbCancelOrder.Text = Resources.GetRes().GetString("CancelOrder");
            krpcbDeleteProduct.Text = Resources.GetRes().GetString("DeleteProduct");
            krpcbChangeUnitPrice.Text = Resources.GetRes().GetString("ChangeUnitPrice");
            krpcbChangeCostPrice.Text = Resources.GetRes().GetString("ChangeCostPrice");
            krpcbBindMemberByNo.Text = Resources.GetRes().GetString("BindMemberByNo");
            krpcbBindSupplierByNo.Text = Resources.GetRes().GetString("BindSupplierByNo");
            krpcbOuterBill.Text = Resources.GetRes().GetString("OuterBill");
            krpcbOpenCashDrawer.Text = Resources.GetRes().GetString("OpenCashDrawer");
            krpcbDecreaseProductCount.Text = Resources.GetRes().GetString("DecreaseProductCount");
            krpcbReturnMoney.Text = Resources.GetRes().GetString("ReturnMoney");

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.MenuList.ico"));


            // 如果KEY中桌子数为0, 则不显示桌子, 也不显示外部账单
            if (Resources.GetRes().RoomCount <= 0)
            {
                krpcbRoomManager.Visible = false;

                krpcbProductTypeManager.Location = new Point(krpcbProductTypeManager.Location.X, krpcbProductTypeManager.Location.Y - 36.RecalcMagnification2());
                krpcbProductManager.Location = new Point(krpcbProductManager.Location.X, krpcbProductManager.Location.Y - 36.RecalcMagnification2());
                krpcbChangeSet.Location = new Point(krpcbChangeSet.Location.X, krpcbChangeSet.Location.Y - 36.RecalcMagnification2());



                krpcbOuterBill.Visible = false;
                krpcbIncomeTradingManage.Location = new Point(krpcbIncomeTradingManage.Location.X, krpcbIncomeTradingManage.Location.Y - 35.RecalcMagnification2());
                krpcbTemporaryChangePrice.Location = new Point(krpcbTemporaryChangePrice.Location.X, krpcbTemporaryChangePrice.Location.Y - 35.RecalcMagnification2());

                krpcbReplaceRoom.Location = new Point(krpcbReplaceRoom.Location.X, krpcbReplaceRoom.Location.Y - 35.RecalcMagnification2());
                krpcbCancelOrder.Location = new Point(krpcbCancelOrder.Location.X, krpcbCancelOrder.Location.Y - 35.RecalcMagnification2());
                krpcbDeleteProduct.Location = new Point(krpcbDeleteProduct.Location.X, krpcbDeleteProduct.Location.Y - 35.RecalcMagnification2());
                krpcbBindMemberByNo.Location = new Point(krpcbBindMemberByNo.Location.X, krpcbBindMemberByNo.Location.Y - 35.RecalcMagnification2());


            }

            

            if (string.IsNullOrWhiteSpace(Menus))
                return;


            string[] menuList = Menus.Split('&');

            if (menuList.Contains(Resources.GetRes().GetString("Computer")))
                krpcbComputer.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("Tablet")))
                krpcbTablet.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("Phone")))
                krpcbPhone.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ChangeLanguage")))
                krpcbChangeLanguage.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("InnerBill")))
                krpcbInnerBill.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("IncomeManager")))
                krpcbOrderManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ExpenditureManager")))
                krpcbExpenditureManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("MemberManager")))
                krpcbMemberManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("RoomManager")))
                krpcbRoomManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ProductTypeManager")))
                krpcbProductTypeManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ProductManager")))
                krpcbProductManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("AdminManager")))
                krpcbAdminManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("DeviceManager")))
                krpcbDeviceManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("PrinterManager")))
                krpcbPrinterManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ChangeSet")))
                krpcbChangeSet.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("FinanceLog")))
                krpcbFinanceLog.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("Statistic")))
                krpcbStatistic.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("SupplierManager")))
                krpcbSupplierManager.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("AdminLog")))
                krpcbAdminLog.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("IncomeTradingManage")))
                krpcbIncomeTradingManage.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("RequestManager")))
                krpcbRequest.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("BalanceManager")))
                krpcbBalance.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("TemporaryChangePrice")))
                krpcbTemporaryChangePrice.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ReplaceRoom")))
                krpcbReplaceRoom.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("CancelOrder")))
                krpcbCancelOrder.Checked = true; 
            if (menuList.Contains(Resources.GetRes().GetString("DeleteProduct")))
                krpcbDeleteProduct.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("DecreaseProductCount")))
                krpcbDecreaseProductCount.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ReturnMoney")))
                krpcbReturnMoney.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ChangeUnitPrice")))
                krpcbChangeUnitPrice.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("ChangeCostPrice")))
                krpcbChangeCostPrice.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("BindMemberByNo")))
                krpcbBindMemberByNo.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("BindSupplierByNo")))
                krpcbBindSupplierByNo.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("OuterBill")))
                krpcbOuterBill.Checked = true;
            if (menuList.Contains(Resources.GetRes().GetString("OpenCashDrawer")))
                krpcbOpenCashDrawer.Checked = true; 
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChange_Click(object sender, EventArgs e)
        {
            List<string> strList = new List<string>();
            if (krpcbComputer.Checked)
            {
                strList.Add(Resources.GetRes().GetString("Computer"));
            }
            if (krpcbTablet.Checked)
            {
                strList.Add(Resources.GetRes().GetString("Tablet"));
            }
            if (krpcbPhone.Checked)
            {
                strList.Add(Resources.GetRes().GetString("Phone"));
            }
            if (krpcbChangeLanguage.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ChangeLanguage"));
            }
            if (krpcbInnerBill.Checked)
            {
                strList.Add(Resources.GetRes().GetString("InnerBill"));
            }
            if (krpcbOrderManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("IncomeManager"));
            }
            if (krpcbExpenditureManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ExpenditureManager"));
            }
            if (krpcbMemberManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("MemberManager"));
            }
            if (krpcbRoomManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("RoomManager"));
            }
            if (krpcbProductTypeManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ProductTypeManager"));
            }
            if (krpcbProductManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ProductManager"));
            }
            if (krpcbAdminManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("AdminManager"));
            }
            if (krpcbDeviceManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("DeviceManager"));
            }
            if (krpcbPrinterManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("PrinterManager"));
            }
            if (krpcbChangeSet.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ChangeSet"));
            }
            if (krpcbFinanceLog.Checked)
            {
                strList.Add(Resources.GetRes().GetString("Statistic"));
            }
            if (krpcbStatistic.Checked)
            {
                strList.Add(Resources.GetRes().GetString("FinanceLog"));
            }
            if (krpcbSupplierManager.Checked)
            {
                strList.Add(Resources.GetRes().GetString("SupplierManager"));
            }
            if (krpcbAdminLog.Checked)
            {
                strList.Add(Resources.GetRes().GetString("AdminLog"));
            }
            if (krpcbRequest.Checked)
            {
                strList.Add(Resources.GetRes().GetString("RequestManager"));
            }
            if (krpcbBalance.Checked)
            {
                strList.Add(Resources.GetRes().GetString("BalanceManager"));
            }
            if (krpcbOuterBill.Checked)
            {
                strList.Add(Resources.GetRes().GetString("OuterBill"));
            }
            if (krpcbIncomeTradingManage.Checked)
            {
                strList.Add(Resources.GetRes().GetString("IncomeTradingManage"));
            }
            if (krpcbTemporaryChangePrice.Checked)
            {
                strList.Add(Resources.GetRes().GetString("TemporaryChangePrice"));
            }
            if (krpcbReplaceRoom.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ReplaceRoom"));
            }
            if (krpcbCancelOrder.Checked)
            {
                strList.Add(Resources.GetRes().GetString("CancelOrder"));
            }
            if (krpcbDeleteProduct.Checked)
            {
                strList.Add(Resources.GetRes().GetString("DeleteProduct"));
            }
            if (krpcbDecreaseProductCount.Checked)
            {
                strList.Add(Resources.GetRes().GetString("DecreaseProductCount"));
            }
            if (krpcbReturnMoney.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ReturnMoney"));
            }
            if (krpcbChangeUnitPrice.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ChangeUnitPrice"));
            }
            if (krpcbChangeCostPrice.Checked)
            {
                strList.Add(Resources.GetRes().GetString("ChangeCostPrice"));
            }
            if (krpcbBindMemberByNo.Checked)
            {
                strList.Add(Resources.GetRes().GetString("BindMemberByNo"));
            }
            if (krpcbBindSupplierByNo.Checked)
            {
                strList.Add(Resources.GetRes().GetString("BindSupplierByNo"));
            }
            if (krpcbOpenCashDrawer.Checked)
            {
                strList.Add(Resources.GetRes().GetString("OpenCashDrawer"));
            }
            


            ReturnValue = strList.Count == 0 ? "" : string.Join("&", strList);

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

      
    }
}
