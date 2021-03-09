using Oybab.DAL;
using Oybab.Res.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oybab.Res.Tools;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Oybab.TradingSystemX.VM.Commands;
using System.Threading.Tasks;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.Server;
using Oybab.Res.Exceptions;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.Res.Server;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.Res.View.Models;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class CheckoutViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;
        private long RoomId = 0;
        private Order order = null;
        private List<OrderDetail> details = null;
        private List<OrderPay> payList = null;
        private List<OrderPay> tempPayList = new List<OrderPay>();
        private string RoomSession;




        public CheckoutViewModel(Xamarin.Forms.Page element, StackLayout paidPricePanel, ControlTemplate paidPriceTemplate)
        {
            this._element = element;
            this.ChangePaidPrice = new ChangePaidPriceViewModel(null, RecalcPaidPrice, paidPricePanel, paidPriceTemplate);

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
            this.PaidPrice = "0";

            this.MemberPaidPrice = "0";
            this.BalanceMode = 0;


            RoomModel model = obj as RoomModel;
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

            if (this.ChangePaidPrice.IsShow)
                this.ChangePaidPrice.IsShow = false;


            if (!string.IsNullOrWhiteSpace(order.Remark))
                this.ChangePaidPrice.Remark = order.Remark;
            else
                this.ChangePaidPrice.Remark = null;

            order.tb_member = model.PayOrder.tb_member;
            

            RoomPrice = order.RoomPrice.ToString();

            Room room = Resources.Instance.Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();
            RoomNo = room.RoomNo;
            IsPayByTime = (int)room.IsPayByTime;


            Calc();
        }



        private List<CommonPayModel> oldList = null;
        /// <summary>
        /// 打开修改金额
        /// </summary>
        private RelayCommand _changePriceCommand;
        public Xamarin.Forms.Command ChangePriceCommand
        {
            get
            {
                return _changePriceCommand ?? (_changePriceCommand = new RelayCommand(param =>
                {
                   

                    IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                            ChangePaidPrice.InitialView(this.order.TotalPrice, oldList, true, true, 1);

                            IsLoading = false;
                        });
                    });
                }));

            }
        }




        /// <summary>
        /// 重新计算支付金额
        /// </summary>
        private void RecalcPaidPrice()
        {

            bool RoomPaidPriceChanged = false;


            // 如果跟订单上次保存的不一样,就提示未保存提示
            if (oldList.All(ChangePaidPrice.PayModel.Contains) && tempPayList.Count == ChangePaidPrice.PayModel.Count)
            {
            }
            else
            {
                RoomPaidPriceChanged = true;
                tempPayList = ChangePaidPrice.PayModel.Select(x => x.GetOrderPay()).ToList();
            }

            if (RoomPaidPriceChanged)
            {


                Calc();

            }
        }

       


         private int _isPayByTime;
        /// <summary>
        /// 是否按时间收费
        /// </summary>
        public int IsPayByTime
        {
            get { return _isPayByTime; }
            set
            {
                _isPayByTime = value;
                OnPropertyChanged("IsPayByTime");
            }
        }




        /// <summary>
        /// 计算
        /// </summary>
        private void Calc(bool OnlyResult = false)
        {

            

                Room room = Resources.Instance.Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();


                if (this.order.StartTime != null && this.order.EndTime != null)
                {
                    TimeSpan total = (DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture) - DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                    TimeSpan balance = (DateTime.Now - DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));


                    if (room.IsPayByTime == 1)
                        TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                    else if (room.IsPayByTime == 2)
                        TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                    // 如果剩余时间已经超出了, 默认0:0显示
                    if (DateTime.Now < DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture))
                    {
                        if (room.IsPayByTime == 1)
                            RemainingTime = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                        else if (room.IsPayByTime == 2)
                            RemainingTime = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                    }
                    else
                        RemainingTime = "0:0";
                }
                else
                {
                    TimeSpan total = (DateTime.Now - DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));



                    if (room.IsPayByTime == 2)
                        TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);
                    else
                        TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);

                    RemainingTime = "0:0";
                }


                this.TotalPrice = order.TotalPrice.ToString();

            order.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            order.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);




            this.MemberPaidPrice = order.MemberPaidPrice.ToString();
            this.PaidPrice = order.PaidPrice.ToString();

            this.TotalPaidPrice = (order.TotalPaidPrice = Math.Round(order.MemberPaidPrice + order.PaidPrice, 2)).ToString();












            double balancePrice = Math.Round(order.TotalPaidPrice - order.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                this.KeepPrice = (order.KeepPrice = balancePrice).ToString();
                this.BorrowPrice = (order.BorrowPrice = 0).ToString();
            }
            else if (balancePrice < 0)
            {
                this.BorrowPrice = (order.BorrowPrice = balancePrice).ToString();
                this.KeepPrice = (order.KeepPrice = 0).ToString();


            }
            else if (balancePrice == 0)
            {
                this.BorrowPrice = (order.BorrowPrice = 0).ToString();
                this.KeepPrice = (order.KeepPrice = 0).ToString();
            }

            if (balancePrice > 0)
                BalanceMode = 1;
            else if (balancePrice < 0)
                BalanceMode = 2;
            else
                BalanceMode = 0;

        }



        private int _balanceMode = 0;
        /// <summary>
        /// 余额模式(0默认,1蓝色2红色)
        /// </summary>
        public int BalanceMode
        {
            get { return _balanceMode; }
            set
            {
                _balanceMode = value;
                OnPropertyChanged("BalanceMode");
            }
        }





        private ChangePaidPriceViewModel _changePaidPrice;
        /// <summary>
        /// 修改支付价格控件
        /// </summary>
        public ChangePaidPriceViewModel ChangePaidPrice
        {
            get { return _changePaidPrice; }
            set
            {
                _changePaidPrice = value;
                OnPropertyChanged("ChangePaidPrice");
            }
        }



        


        private string _roomNo;
        /// <summary>
        /// 雅座编号
        /// </summary>
        public string RoomNo
        {
            get { return _roomNo; }
            set
            {
                _roomNo = value;
                OnPropertyChanged("RoomNo");
            }
        }




        private string _roomPrice;
        /// <summary>
        /// 雅座价格
        /// </summary>
        public string RoomPrice
        {
            get { return _roomPrice; }
            set
            {
                _roomPrice = value;
                OnPropertyChanged("RoomPrice");
            }
        }




      




        private string _totalTime;
        /// <summary>
        /// 总时间
        /// </summary>
        public string TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                OnPropertyChanged("TotalTime");
            }
        }



        private string _remainingTime;
        /// <summary>
        /// 剩余时间
        /// </summary>
        public string RemainingTime
        {
            get { return _remainingTime; }
            set
            {
                _remainingTime = value;
                OnPropertyChanged("RemainingTime");
            }
        }




        private string _totalPrice;
        /// <summary>
        /// 总价
        /// </summary>
        public string TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
        }








        private string _memberPaidPrice;
        /// <summary>
        /// 会员支付价
        /// </summary>
        public string MemberPaidPrice
        {
            get { return _memberPaidPrice; }
            set
            {
                _memberPaidPrice = value;
                OnPropertyChanged("MemberPaidPrice");
            }
        }



        private string _paidPrice;
        /// <summary>
        /// 现金支付价格
        /// </summary>
        public string PaidPrice
        {
            get { return _paidPrice; }
            set
            {
                _paidPrice = value;
                OnPropertyChanged("PaidPrice");

            }
        }






        private string _totalPaidPrice;
        /// <summary>
        /// 总支付价格
        /// </summary>
        public string TotalPaidPrice
        {
            get { return _totalPaidPrice; }
            set
            {
                _totalPaidPrice = value;
                OnPropertyChanged("TotalPaidPrice");
            }
        }






        private string _borrowPrice;
        /// <summary>
        /// 借款价格
        /// </summary>
        public string BorrowPrice
        {
            get { return _borrowPrice; }
            set
            {
                _borrowPrice = value;
                OnPropertyChanged("BorrowPrice");
            }
        }



        private string _keepPrice;
        /// <summary>
        /// 保留价格
        /// </summary>
        public string KeepPrice
        {
            get { return _keepPrice; }
            set
            {
                _keepPrice = value;
                OnPropertyChanged("KeepPrice");
            }
        }


        



        /// <summary>
        /// 后退按钮
        /// </summary>
        private RelayCommand _backCommand;
        public Command BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                }));
            }
        }






        private bool _isLoading;
        /// <summary>
        /// 显示正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }








        /// <summary>
        /// 结账命令
        /// </summary>
        private RelayCommand _checkoutCommand;
        public Command CheckoutCommand
        {
            get
            {
                return _checkoutCommand ?? (_checkoutCommand = new RelayCommand(param =>
                {

                    if (null != order.tb_member && order.tb_member.IsAllowBorrow == 0 && order.tb_member.BalancePrice < double.Parse(MemberPaidPrice))
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("MemberBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        return;
                        }



                        //余额不平确认
                        if (order.BorrowPrice != 0 || order.KeepPrice != 0)
                        {

                            //确认取消
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("ConfirmBalanceInjustice"), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                            {
                                if (msg == "NO")
                                    return;

                                CheckPayAndCheckout();

                            }, null);
                        }
                        else
                        {
                            CheckPayAndCheckout();
                        }

                    }));
            }
        }



        /// <summary>
        /// 直接输入缺少的金额
        /// </summary>
        public void FinishPaidPrice()
        {

            IsLoading = true;

            Task.Run(async () =>
            {

                await ExtX.WaitForLoading();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                    ChangePaidPrice.InitialView(this.order.TotalPrice, oldList, true, true, 1);

                    double lessPrice = Math.Round(order.TotalPrice - order.TotalPaidPrice, 2);

                    if (lessPrice != 0)
                    {
                        if (lessPrice > 0)
                        {
                            ChangePaidPrice.SetChangePrice(lessPrice, "+");
                        }
                        else if (lessPrice < 0)
                        {
                            ChangePaidPrice.SetChangePrice(Math.Abs(lessPrice), "-");
                        }

                    }

                    IsLoading = false;
                });
            });

            
        }


        /// <summary>
        /// 检查支付并结账
        /// </summary>
        private void CheckPayAndCheckout()
        {
            if (order.PaidPrice == 0 && order.MemberPaidPrice == 0)
            {
                //确认取消
                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("ConfirmNotPay"), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                {
                    if (msg == "NO")
                        return;

                    Checkout();

                }, null);
            }
            else
            {
                Checkout();
            }
        }


        /// <summary>
        /// 结账
        /// </summary>
        private void Checkout()
        {

            order.State = 1;
            bool IsRechecked = false;

            string newRoomSessionId;
            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.Instance.GetString("CheckoutOrder");

            if (order.Remark != ChangePaidPrice.Remark)
            {
                if (!string.IsNullOrWhiteSpace(ChangePaidPrice.Remark))
                    order.Remark = this.ChangePaidPrice.Remark;
                else
                    order.Remark = null;
            }

            IsLoading = true;

            Task.Factory.StartNew(async () =>
            {

               
                try
                {
                    
                    long UpdateTime;

                    var taskResult = await OperatesService.Instance.ServiceEditOrder(order, null, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.OrderId = order.OrderId; return x; }).ToList(), RoomSession, IsRechecked);
                    ResultModel result = taskResult.resultModel;
                    newRoomSessionId = taskResult.newRoomStateSession ;
                    UpdateTime = taskResult.UpdateTime;


                    if (result.Result)
                    {
                        if (!IsRechecked)
                        {
                            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                            model.OrderSession = newRoomSessionId;
                            model.PayOrder = null;

                          

                            if (Resources.Instance.CallNotifications.ContainsKey(RoomId))
                                Resources.Instance.CallNotifications.Remove(RoomId);

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

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        if (result.Result)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), SucMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, (msg)=>
                            {
                                NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                NavigationPath.Instance.SwitchNavigate(1);
                                Notification.Instance.ActionSendsFromService(null, new List<long>() { this.RoomId }, null);
                            }, null);


                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (msg) =>
                                {
                                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                    NavigationPath.Instance.SwitchNavigate(1);
                                    RoomListViewModel viewModel = NavigationPath.Instance.CurrentPage.BindingContext as RoomListViewModel;
                                    viewModel.Init(this.RoomId);


                                }, null);
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenWaitRetry"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }
                            else
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, (msg) =>
                            {
                                if (OperatesService.Instance.IsExpired || OperatesService.Instance.IsAdminUsing)
                                {
                                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                }
                            }, null);
                        }), false, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName));
                    }));
                }

                
                Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                {
                    IsLoading = false;
                }));
            });
        }

    }
}
