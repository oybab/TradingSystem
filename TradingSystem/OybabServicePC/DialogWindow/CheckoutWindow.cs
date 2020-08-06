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
using Oybab.Res.View.Models;
using Oybab.ServicePC.Tools;
using Oybab.ServicePC.SubWindow;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class CheckoutWindow : KryptonForm
    {
        private long RoomId = 0;
        private Order order = null;
        private List<OrderDetail> details = null;
        private List<OrderPay> payList = null;
        private List<OrderPay> tempPayList = new List<OrderPay>();
        private string RoomSession;
        private bool IsRechecked = false;

        public CheckoutWindow(RoomModel model, bool IsRechecked = false)
        {

            RoomId = model.RoomId;
            RoomSession = model.OrderSession;

            if (null != model.PayOrder.tb_orderdetail)
                details = model.PayOrder.tb_orderdetail.ToList();
            else
                details = new List<OrderDetail>();

            if (null != model.PayOrder.tb_orderpay)
                payList = model.PayOrder.tb_orderpay.ToList();
            else
                payList = new List<OrderPay>();

            this.tempPayList = payList.ToList();

            order = model.PayOrder.FastCopy();
            this.IsRechecked = IsRechecked;

            


            order.tb_member = model.PayOrder.tb_member;


            InitializeComponent();

            this.Text = Resources.GetRes().GetString("CheckoutOrder");
           
            krplTotalTime.Text = Resources.GetRes().GetString("TotalTime");
            krplRemainingTime.Text = Resources.GetRes().GetString("RemainingTime");
            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");
            

            krpbCheckout.Text = Resources.GetRes().GetString("CheckoutOrder");
            

            
            krplBorrowPrice.Text = Resources.GetRes().GetString("OwedPrice");
            krplKeepPrice.Text = Resources.GetRes().GetString("KeepPrice");
            
            krplMemberPaidPrice.Text = Resources.GetRes().GetString("MemberPaidPrice");
            krplTotalPaidPrice.Text = Resources.GetRes().GetString("TotalPaidPrice");


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.CheckoutOrder.ico"));

            krplRoomNo.Text = Resources.GetRes().GetString("RoomNo");
            krplRoomPriceValue.Text = string.Format("({1}{0})", order.RoomPrice, Resources.GetRes().PrintInfo.PriceSymbol);
            krplRoomNoValue.Text = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).Select(x => x.RoomNo).FirstOrDefault();
            

            Calc();

            payWindow = new PriceCommonChangeWindow("+", order.TotalPrice, tempPayList.Select(x => new CommonPayModel(x)).ToList(), true, true, Recalc);
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
        private void Calc()
        {
            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();


            if (this.order.StartTime != null && this.order.EndTime != null)
            {
                TimeSpan total = (DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null) - DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null));
                TimeSpan balance = (DateTime.Now - DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null));


                if (room.IsPayByTime == 1)
                    krplTotalTimeValue.Text = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                else if (room.IsPayByTime == 2)
                    krplTotalTimeValue.Text = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                // 如果剩余时间已经超出了, 默认0:0显示
                if (DateTime.Now < DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null))
                {
                    if (room.IsPayByTime == 1)
                        krplRemainingTimeValue.Text = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                    else if (room.IsPayByTime == 2)
                        krplRemainingTimeValue.Text = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                }
                else
                    krplRemainingTimeValue.Text = "0:0";
            }
            else
            {
                TimeSpan total = (DateTime.Now - DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", null));



                if (room.IsPayByTime == 2)
                    krplTotalTimeValue.Text = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);
                else
                    krplTotalTimeValue.Text = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);

                krplRemainingTimeValue.Text = "0:0";
            }


            this.krplTotalPriceValue.Text = order.TotalPrice.ToString();



            order.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            order.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);




            this.krplMemberPaidPriceValue.Text = order.MemberPaidPrice.ToString();
            this.krplPaidPriceValue.Text = (order.PaidPrice).ToString();

            this.krplTotalPaidPriceValue.Text = (order.TotalPaidPrice = Math.Round(order.MemberPaidPrice + order.PaidPrice, 2)).ToString();











            double balancePrice = Math.Round(order.TotalPaidPrice - order.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Blue;
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                this.krplKeepPriceValue.Text = (order.KeepPrice = balancePrice).ToString();
                this.krplBorrowPriceValue.Text = (order.BorrowPrice = 0).ToString();
            }
            else if (balancePrice < 0)
            {
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Red;

                this.krplBorrowPriceValue.Text = (order.BorrowPrice = balancePrice).ToString();
                this.krplKeepPriceValue.Text = (order.KeepPrice = 0).ToString();


            }
            else if (balancePrice == 0)
            {
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                this.krplBorrowPriceValue.Text = (order.BorrowPrice = 0).ToString();
                this.krplKeepPriceValue.Text = (order.KeepPrice = 0).ToString();
            }

            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(order.BorrowPrice.ToString());
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                RoomInfoModel roomInfo = new RoomInfoModel();
                roomInfo.RoomNo = krplRoomNoValue.Text;
                roomInfo.RoomPrice = order.RoomPrice;
                roomInfo.TotalTime = krplTotalTimeValue.Text;
                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(order, null, details, roomInfo, true));
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
            if (order.BorrowPrice != 0 || order.KeepPrice != 0)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmBalanceInjustice"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;
            }

            // 退的钱如果比付的钱还要多(就看是否允许退钱了)
            if (!Common.GetCommon().IsReturnMoney() && Math.Round(tempPayList.Sum(x => x.OriginalPrice), 2) < 0)
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("CanNotExceed"), Math.Round(tempPayList.Sum(x => x.OriginalPrice), 2)), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (order.PaidPrice == 0 && order.MemberPaidPrice == 0)
            {

                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmNotPay"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return; 
            }

          

            order.State = 1;

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
                    
                    result = OperatesService.GetOperates().ServiceEditOrder(order, null, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.OrderId = order.OrderId; return x; }).ToList(), RoomSession, IsRechecked, out newRoomSessionId, out UpdateTime);
                    if (result.Result)
                    {
                        if (!IsRechecked)
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                            model.OrderSession = newRoomSessionId;
                            model.PayOrder = null;

                            


                            if (Resources.GetRes().CallNotifications.ContainsKey(RoomId))
                                Resources.GetRes().CallNotifications.Remove(RoomId);





                            // 更新会员信息
                            foreach (var item in tempPayList.Where(x => x.AddTime == 0))
                            {
                                if (null != item.MemberId)
                                {
                                    Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);

                                }
                            }
                        }

                       
                    }

                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            Print.Instance.PrintOrderAfterCheckout(order, details);
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            IsClose = true;
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
                Recalc();
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
                tempPayList = payWindow.PayModel.Select(x => x.GetOrderPay()).ToList();
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
            double lessPrice = Math.Round(order.TotalPrice - order.TotalPaidPrice, 2);

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
