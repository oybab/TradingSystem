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
using Oybab.Res.Reports;
using Oybab.Res.Tools;
using Oybab.ServicePC.Tools;
using Oybab.ServicePC.SubWindow;
using Oybab.Res.View.Models;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class TakeoutCheckoutWindow : KryptonForm
    {
        private Takeout takeout = null;
        private List<TakeoutDetail> details = null;
        private List<TakeoutPay> payList = null;
        private List<TakeoutPay> tempPayList = new List<TakeoutPay>();

        private bool IsRechecked = false;

        public TakeoutCheckoutWindow(Takeout model, bool IsRechecked = false)
        {
            if (null != model.tb_takeoutdetail)
                details = model.tb_takeoutdetail.ToList();
            else
                details = new List<TakeoutDetail>();

            if (null != model.tb_takeoutpay)
                payList = model.tb_takeoutpay.ToList();
            else
                payList = new List<TakeoutPay>();

            this.tempPayList = payList.ToList();

            takeout = model.FastCopy();
            this.IsRechecked = IsRechecked;




            takeout.tb_member = model.tb_member;


            InitializeComponent();
           
            this.Text = Resources.GetRes().GetString("CheckoutOrder");


            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");
           

            krpbCheckout.Text = Resources.GetRes().GetString("CheckoutOrder");
           
            
            krplBorrowPrice.Text = Resources.GetRes().GetString("OwedPrice");
            krplKeepPrice.Text = Resources.GetRes().GetString("KeepPrice");
           
            krplMemberPaidPrice.Text = Resources.GetRes().GetString("MemberPaidPrice");
            krplTotalPaidPrice.Text = Resources.GetRes().GetString("TotalPaidPrice");


          
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.CheckoutOrder.ico"));



            payWindow = new PriceCommonChangeWindow("+", takeout.TotalPrice, tempPayList.Select(x => new CommonPayModel(x)).ToList(), true, true, Recalc);
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



            this.krplTotalPriceValue.Text = takeout.TotalPrice.ToString();

            takeout.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            takeout.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);




            this.krplMemberPaidPriceValue.Text = takeout.MemberPaidPrice.ToString();
            this.krplPaidPriceValue.Text = (takeout.PaidPrice).ToString();

            this.krplTotalPaidPriceValue.Text = (takeout.TotalPaidPrice = Math.Round(takeout.MemberPaidPrice + takeout.PaidPrice, 2)).ToString();










            double balancePrice = Math.Round(takeout.TotalPaidPrice - takeout.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Blue;
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                this.krplKeepPriceValue.Text = (takeout.KeepPrice = balancePrice).ToString();
                this.krplBorrowPriceValue.Text = (takeout.BorrowPrice = 0).ToString();
            }
            else if (balancePrice < 0)
            {
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Red;

                this.krplBorrowPriceValue.Text = (takeout.BorrowPrice = balancePrice).ToString();
                this.krplKeepPriceValue.Text = (takeout.KeepPrice = 0).ToString();


            }
            else if (balancePrice == 0)
            {
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                this.krplBorrowPriceValue.Text = (takeout.BorrowPrice = 0).ToString();
                this.krplKeepPriceValue.Text = (takeout.KeepPrice = 0).ToString();
            }

            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(takeout.BorrowPrice.ToString());
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(takeout, details, null, true));
            }




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
            if (takeout.BorrowPrice != 0 || takeout.KeepPrice != 0)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmBalanceInjustice"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;
            }

            if (takeout.PaidPrice == 0 && takeout.MemberPaidPrice == 0)
            {

                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmNotPay"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;
            }

          

            takeout.State = 1;
         


            string newRoomSessionId;
            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.GetRes().GetString("CheckoutOrder");

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                ResultModel result = new ResultModel();
              
                bool IsClose = false;

                try
                {
                    

                    long UpdateTime;
                    List<TakeoutDetail> resultDetailsList;
                    List<TakeoutPay> resultPaysList;

                    result = OperatesService.GetOperates().ServiceAddTakeout(takeout, details, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.TakeoutId = takeout.TakeoutId; return x; }).ToList(), null, out resultDetailsList, out resultPaysList, out newRoomSessionId, out UpdateTime);
                    if (result.Result)
                    {
                        if (!IsRechecked)
                        {
                            

                            if (null != takeout.tb_member)
                            {
                                Notification.Instance.ActionMember(this, takeout.tb_member, null);
                                takeout.MemberId = takeout.tb_member.MemberId;
                            }



                            // 如果成功, 则新增产品
                            foreach (var item in details)
                            {
                                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {


                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                            if (null != productParent && productParent.IsBindCount == 1)
                                            {
                                                double ParentRemove = 0;
                                                double ProductAdd = 0;


                                                double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3);
                                                ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                ParentRemove = (int)Math.Ceiling(ParentRemove); 
                                                
                                                ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 
                                                

                                                // 从父级中去掉
                                                productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                productParent.UpdateTime = UpdateTime;
                                               

                                                // 给产品增加零的
                                                product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);
                                            }
                                        }
                                    }



                                    product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                    product.UpdateTime = UpdateTime;

                                    Notification.Instance.ActionProduct(null, product, 2);
                                }
                            }




                            // 更新会员信息
                            foreach (var item in resultPaysList)
                            {
                                if (null != item.MemberId)
                                {
                                    Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                    item.MemberId = item.tb_member.MemberId;
                                }
                            }

                        }
                    }

                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {

                            Print.Instance.PrintTakeoutAfterCheckout(takeout, details);
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            IsClose = true;

                            Resources.GetRes().DefaultOrderLang = Resources.GetRes().GetMainLangByLangIndex((int)takeout.Lang).MainLangIndex;
                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                DialogResult = System.Windows.Forms.DialogResult.Retry;
                                IsClose = true;
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
                    }));
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

            PriceCommonChangeWindow window = new PriceCommonChangeWindow(mark, totalPrice, oldList.ToList());
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
                    tempPayList = window.PayModel.Select(x => x.GetTakeoutPay()).ToList();
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
                tempPayList = payWindow.PayModel.Select(x => x.GetTakeoutPay()).ToList();
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
            double lessPrice = Math.Round(takeout.TotalPrice - takeout.TotalPaidPrice, 2);

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
