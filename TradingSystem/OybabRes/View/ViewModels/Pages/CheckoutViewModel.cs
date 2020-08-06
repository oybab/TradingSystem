using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Component;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class CheckoutViewModel: ViewModelBase
    {
        private UIElement _element;
        private long RoomId = 0;
        private Order order = null;
        private List<OrderDetail> details = null;
        private List<OrderPay> payList = null;
        private List<OrderPay> tempPayList = new List<OrderPay>();
        private string RoomSession;


        public CheckoutViewModel(UIElement element, WrapPanel paidPricePanel)
        {
            this._element = element;
            
            this.ChangePaidPrice = new ChangePaidPriceViewModel(element, RecalcPaidPrice, paidPricePanel);

            // 添加处理事件
            this._element.AddHandler(PublicEvents.BoxEvent, new RoutedEventHandler(HandleBox), true);



        }


        /// <summary>
        /// 处理窗口弹出按钮路由
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleBox(object sender, RoutedEventArgs args)
        {
            BoxRoutedEventArgs boxArgs = args as BoxRoutedEventArgs;
            if (null != boxArgs)
            {
                switch (boxArgs.BoxType)
                {
                    case BoxType.ChangePaidPrice:
                        oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                        this.ChangePaidPrice.Init(this.order.TotalPrice, oldList, true, true);
                        this.ChangePaidPrice.Show();
                        break;
                    default:
                        break;
                }
            }
        }
        private List<CommonPayModel> oldList = null;


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
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

            if (!string.IsNullOrWhiteSpace(order.Remark))
                this.ChangePaidPrice.Remark = order.Remark;
            else
                this.ChangePaidPrice.Remark = null;


            order.tb_member = model.PayOrder.tb_member;



            RoomPrice =  order.RoomPrice.ToString();

            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();
            RoomNo = room.RoomNo;
            IsPayByTime = (int)room.IsPayByTime;

            
            Calc();



            if (!IsScanReady)
            {
                IsScanReady = true;
                // 刷卡
                Notification.Instance.NotificationCardReader += Instance_NotificationCardReader;
            }
            _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.ChangePaidPrice, model));

        }
        private bool IsScanReady = false;


        /// <summary>
        /// 刷卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationCardReader(object sender, string value, object args)
        {
            if (ChangePaidPrice.IsShow)
            {
                ChangePaidPrice.OpenMemberByScanner(value);
                return;
            }
        }

        /// <summary>
        /// 直接输入缺少的金额
        /// </summary>
        public void FinishPaidPrice()
        {
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
                    TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                else if (room.IsPayByTime == 2)
                    TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                // 如果剩余时间已经超出了, 默认0:0显示
                if (DateTime.Now < DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null))
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
                TimeSpan total = (DateTime.Now - DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", null));



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


            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(order.BorrowPrice.ToString());

            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                RoomInfoModel roomInfo = new RoomInfoModel();
                roomInfo.RoomNo = RoomNo;
                roomInfo.RoomPrice = order.RoomPrice;
                roomInfo.TotalTime = TotalTime;


                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(order, null, details, roomInfo, true));
            }

        }


        
        

        /// <summary>
        /// 输入回车
        /// </summary>
        private RelayCommand _enterCommand;
        public ICommand EnterCommand
        {
            get
            {
                if (_enterCommand == null)
                {
                    _enterCommand = new RelayCommand(param =>
                    {
                        
                        //this.CheckoutCommand.Execute(null);
                        
                    });
                }
                return _enterCommand;
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
        /// 会员支付价格
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
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(param => _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Back)));
                }
                return _backCommand;
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



        


        /// <summary>
        /// 结账命令
        /// </summary>
        private RelayCommand _checkoutCommand;
        public ICommand CheckoutCommand
        {
            get
            {
                if (_checkoutCommand == null)
                {
                    _checkoutCommand = new RelayCommand(param =>
                        {


                            //余额不平确认
                            if (order.BorrowPrice !=0 || order.KeepPrice !=0)
                            {

                                //确认取消
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("ConfirmBalanceInjustice"), msg =>
                                {
                                    if (msg == "NO")
                                        return;

                                    CheckPayAndCheckout();

                                }, PopupType.Question));
                            }
                            else
                            {
                                CheckPayAndCheckout();
                            }

                        });
                }
                return _checkoutCommand;
            }
        }


        /// <summary>
        /// 检查支付并结账
        /// </summary>
        private void CheckPayAndCheckout()
        {
            if (order.PaidPrice == 0 && order.MemberPaidPrice == 0)
            {
                //确认取消
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("ConfirmNotPay"), msg =>
                {
                    if (msg == "NO")
                        return;

                    Checkout();

                }, PopupType.Question));
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
            ErrMsgName = SucMsgName = Resources.GetRes().GetString("CheckoutOrder");

            if (order.Remark != ChangePaidPrice.Remark)
            {
                if (!string.IsNullOrWhiteSpace(ChangePaidPrice.Remark))
                    order.Remark = this.ChangePaidPrice.Remark;
                else
                    order.Remark = null;
            }

            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
                ResultModel result = new ResultModel();

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

                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            Print.Instance.PrintOrderAfterCheckout(order, details);
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), (x) =>
                            {
                                if (x == "OK")
                                    _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Room, (long)-1));
                            }, PopupType.Information));


                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Room, this.RoomId));
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenWaitRetry"), null, PopupType.Warn));
                            }
                            else
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName), null, PopupType.Warn));
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName));
                    }));
                }

              


                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                }));
            });
        }

    }
}
