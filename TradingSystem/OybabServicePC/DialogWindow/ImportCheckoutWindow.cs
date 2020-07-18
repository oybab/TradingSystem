using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.ServicePC.Tools;
using Oybab.ServicePC.SubWindow;
using Oybab.Res.View.Models;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class ImportCheckoutWindow : KryptonForm
    {
        public Import ReturnValue { get; private set; } //返回值
        private List<ImportPay> payList = null;
        private List<ImportPay> tempPayList = new List<ImportPay>();


        private Import import = null;
        private bool IsRechecked = false;

        public ImportCheckoutWindow(Import model, bool IsRechecked = false)
        {



            if (this.IsRechecked)
            {
                this.import.tb_importdetail = null;
                this.import.tb_importpay = null;
            }


            if (null != model.tb_importpay)
                payList = model.tb_importpay.ToList();
            else
                payList = new List<ImportPay>();

            this.tempPayList = payList.ToList();



            this.import = model;
            this.IsRechecked = IsRechecked;

            InitializeComponent();

            this.Text = Resources.GetRes().GetString("CheckoutImport");

            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");


            krpbCheckout.Text = Resources.GetRes().GetString("CheckoutImport");

            krplBorrowPrice.Text = Resources.GetRes().GetString("BorrowPrice");
            krplKeepPrice.Text = Resources.GetRes().GetString("KeepPrice");

            krplSupplierPaidPrice.Text = Resources.GetRes().GetString("SupplierPaidPrice");
            krplTotalPaidPrice.Text = Resources.GetRes().GetString("TotalPaidPrice");


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ExpendCheckout.ico"));





            payWindow = new PriceCommonChangeWindow("+", import.TotalPrice, tempPayList.Select(x => new CommonPayModel(x)).ToList(), false, true, Recalc);
            payWindow.StartLoad += (x, y) =>
            {
                this.StartLoad(x, y);
            };
            payWindow.StopLoad += (x, y) =>
            {
                this.StopLoad(x, y);
            };
            payWindow.TopLevel = false;
            pnPrice.Controls.Add(payWindow);
            payWindow.Show();


            Calc();




            // 刷卡
            Notification.Instance.NotificationCardReader += Instance_NotificationCardReader;


        }

        /// <summary>
        /// 刷卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationCardReader(object sender, string value, object args)
        {
            ScanCard(value);
        }

        /// <summary>
        /// 扫条形码
        /// </summary>
        private void ScanCard(string code)
        {
            // 判断能否打开
            if (null != payWindow)
            {
                payWindow.OpenMemberByScanner(code);
                return;
            }
        }


        /// <summary>
        /// 计算
        /// </summary>
        private void Calc(bool OnlyResult = false)
        {


            this.krplTotalPriceValue.Text = import.TotalPrice.ToString();



            import.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            import.SupplierPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.SupplierId).Sum(x => x.OriginalPrice), 2);





            this.krplSupplierPaidPriceValue.Text = import.SupplierPaidPrice.ToString();
            this.krplPaidPriceValue.Text = (import.PaidPrice).ToString();

            this.krplTotalPaidPriceValue.Text = (import.TotalPaidPrice = Math.Round(import.SupplierPaidPrice + import.PaidPrice, 2)).ToString();






            double balancePrice = Math.Round(import.TotalPaidPrice - import.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Blue;
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                this.krplKeepPriceValue.Text = (import.KeepPrice = balancePrice).ToString();
                this.krplBorrowPriceValue.Text = (import.BorrowPrice = 0).ToString();
            }
            else if (balancePrice < 0)
            {
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Red;

                this.krplBorrowPriceValue.Text = (import.BorrowPrice = balancePrice).ToString();
                this.krplKeepPriceValue.Text = (import.KeepPrice = 0).ToString();


            }
            else if (balancePrice == 0)
            {
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                this.krplBorrowPriceValue.Text = (import.BorrowPrice = 0).ToString();
                this.krplKeepPriceValue.Text = (import.KeepPrice = 0).ToString();
            }

            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(import.BorrowPrice.ToString());

        }

       

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbCheckout_Click(object sender, EventArgs e)
        {
            //余额不平确认
            if (import.BorrowPrice != 0 || import.KeepPrice != 0)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmBalanceInjustice"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;
            }

            if (import.PaidPrice == 0 && import.SupplierPaidPrice == 0)
            {

                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmNotPay"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;
            }

         

            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.GetRes().GetString("CheckoutImport");

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {


                ResultModel result = new ResultModel();
              

                bool IsClose = false;

                try
                {
                    

                    List<ImportDetail> resultDetails;
                    List<ImportPay> resultPaysList;
                    long UpdateTime;
                    result = OperatesService.GetOperates().ServiceAddImportWithDetail(import, import.tb_importdetail.ToList(), tempPayList.Where(x => x.AddTime == 0).Select(x => { x.ImportId = import.ImportId; return x; }).ToList(), out resultDetails, out resultPaysList, out UpdateTime);


                    if (result.Result)
                    {

                        if (null != import.tb_supplier)
                        {
                            Notification.Instance.ActionSupplier(this, import.tb_supplier, null);
                            import.SupplierId = import.tb_supplier.SupplierId;
                        }


                        // 如果成功, 则新增产品
                        foreach (var item in resultDetails)
                        {
                            Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                            if (product.IsBindCount == 1 || item.TotalPrice != item.OriginalTotalPrice || item.SalePrice != item.OriginalSalePrice)
                            {
                                if (product.IsBindCount == 1)
                                    product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);

                                product.UpdateTime = UpdateTime;

                                // 如果总支出价格和应该算出来的价格不一样, 则更改产品支出价格
                                if (item.TotalPrice != item.OriginalTotalPrice)
                                {
                                    product.CostPrice = item.Price;
                                }

                                // 如果产品价格和原始价格不同, 则更改产品价格
                                if (item.SalePrice != item.OriginalSalePrice)
                                {
                                    product.Price = item.SalePrice;
                                }

                                Notification.Instance.ActionProduct(null, product, 2);
                            }
                        }



                        // 更新会员信息
                        foreach (var item in resultPaysList)
                        {
                            if (null != item.SupplierId)
                            {
                                Notification.Instance.ActionSupplier(this, new Supplier() { SupplierId = item.SupplierId.Value }, null);
                                item.SupplierId = item.tb_supplier.SupplierId;
                            }
                        }


                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            import.tb_importdetail = resultDetails;
                            import.tb_importpay = resultPaysList;
                            this.ReturnValue = import;
                            IsClose = true;
                        }));
                    }
                    else
                    {

                        if (result.IsRefreshSessionModel)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else if (result.IsSessionModelSameTimeOperate)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenWaitRetry"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName));
                    }));
                }


               
                if (IsClose)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        this.Close();
                    }));
                }

                StopLoad(this, null);
            });


        }

        /// <summary>
        /// 增加价钱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaidPriceAdd_Click(object sender, EventArgs e)
        {
            ChangePaidPrice("+");
        }

        /// <summary>
        /// 减少价钱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaidPriceSub_Click(object sender, EventArgs e)
        {
            ChangePaidPrice("-");
        }


        private PriceCommonChangeWindow payWindow = null;
        /// <summary>
        /// 修改付款价格
        /// </summary>
        /// <param name="mark"></param>
        private void ChangePaidPrice(string mark, double price = 0)
        {
            double totalPrice = double.Parse(krplTotalPriceValue.Text);

            List<CommonPayModel> oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();

            PriceCommonChangeWindow window = new PriceCommonChangeWindow(mark, totalPrice, oldList.ToList(), false);
            payWindow = window;
            window.StartLoad += (x, y) =>
            {
                this.StartLoad(x, y);
            };
            window.StopLoad += (x, y) =>
            {
                this.StopLoad(x, y);
            };


            if (window.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // 如果跟订单上次保存的不一样,就提示未保存提示
                if (oldList.All(window.PayModel.Contains) && oldList.Count == window.PayModel.Count)
                {

                }
                else
                {
                    tempPayList = window.PayModel.Select(x => x.GetImportPay()).ToList();
                    Calc();
                }
            }

            payWindow = null;
        }


        private void Recalc()
        {
            List<CommonPayModel> oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
            // 如果跟订单上次保存的不一样,就提示未保存提示
            if (oldList.All(payWindow.PayModel.Contains) && oldList.Count == payWindow.PayModel.Count)
            {

            }
            else
            {
                tempPayList = payWindow.PayModel.Select(x => x.GetImportPay()).ToList();
                Calc();
            }
        }



        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        /// <summary>
        /// 快速整平付款价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbPaidPriceFinish_Click(object sender, EventArgs e)
        {
            krplPaidPrice_MouseClick(null, null);


        }

       
        /// <summary>
        /// 加号直接打开支付里
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (Form.ModifierKeys == Keys.None && keyData == Keys.Space)
            {
                krpbPaidPriceFinish_Click(null, null);
                return true;
            }
            else if (Form.ModifierKeys == Keys.None && keyData == Keys.Enter)
            {
                krpbCheckout_Click(null, null);
                return true;

            }


            return base.ProcessDialogKey(keyData);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Notification.Instance.NotificationCardReader -= Instance_NotificationCardReader;
        }

        private void krplPaidPrice_MouseClick(object sender, MouseEventArgs e)
        {
            double lessPrice = Math.Round(import.TotalPrice - import.TotalPaidPrice, 2);

            if (lessPrice != 0)
            {
                if (lessPrice > 0)
                {
                    payWindow.SetChangePrice(lessPrice, "+");
                }
                else if (lessPrice < 0)
                {
                    payWindow.SetChangePrice(Math.Abs(lessPrice), "-");
                }

            }
        }
    }
}
