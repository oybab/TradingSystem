using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Reports;
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
    public sealed class TakeoutCheckoutViewModel: ViewModelBase
    {
        private UIElement _element;

        private Takeout takeout = null;
        private List<TakeoutDetail> details = null;
        private List<TakeoutPay> payList = null;
        private List<TakeoutPay> tempPayList = new List<TakeoutPay>();


        public TakeoutCheckoutViewModel(UIElement element, WrapPanel paidPricePanel)
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
                        this.ChangePaidPrice.Init(this.takeout.TotalPrice, oldList, true, true);
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
            Takeout model = obj as Takeout;



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



            if (!string.IsNullOrWhiteSpace(takeout.Remark))
                this.ChangePaidPrice.Remark = takeout.Remark;
            else
                this.ChangePaidPrice.Remark = null;



            takeout.tb_member = model.tb_member;

            


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
            double lessPrice = Math.Round(takeout.TotalPrice - takeout.TotalPaidPrice, 2);

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
                tempPayList = ChangePaidPrice.PayModel.Select(x => x.GetTakeoutPay()).ToList();
            }

            if (RoomPaidPriceChanged)
            {


                Calc();

            }
        }




        /// <summary>
        /// 计算
        /// </summary>
        private void Calc(bool OnlyResult = false)
        {

            this.TotalPrice = takeout.TotalPrice.ToString();

            takeout.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            takeout.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);




            this.MemberPaidPrice = takeout.MemberPaidPrice.ToString();
                this.PaidPrice = takeout.PaidPrice.ToString();


                this.TotalPaidPrice = (takeout.TotalPaidPrice = Math.Round(takeout.MemberPaidPrice + takeout.PaidPrice, 2)).ToString();



            


            double balancePrice = Math.Round(takeout.TotalPaidPrice - takeout.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                this.KeepPrice = (takeout.KeepPrice = balancePrice).ToString();
                this.BorrowPrice = (takeout.BorrowPrice = 0).ToString();
            }
            else if (balancePrice < 0)
            {
                this.BorrowPrice = (takeout.BorrowPrice = balancePrice).ToString();
                this.KeepPrice = (takeout.KeepPrice = 0).ToString();


            }
            else if (balancePrice == 0)
            {
                this.BorrowPrice = (takeout.BorrowPrice = 0).ToString();
                this.KeepPrice = (takeout.KeepPrice = 0).ToString();
            }

            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(takeout.BorrowPrice.ToString());
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(takeout, details, null));
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






        private string _memberName;
        /// <summary>
        /// 会员名
        /// </summary>
        public string MemberName
        {
            get { return _memberName; }
            set
            {
                _memberName = value;
                OnPropertyChanged("MemberName");
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
                            if (takeout.BorrowPrice != 0 || takeout.KeepPrice != 0)
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
            if (takeout.PaidPrice == 0 && takeout.MemberPaidPrice == 0)
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

            takeout.State = 1;
            bool IsRechecked = false;

            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.GetRes().GetString("CheckoutOrder");

            if (takeout.Remark != ChangePaidPrice.Remark)
            {
                if (!string.IsNullOrWhiteSpace(ChangePaidPrice.Remark))
                    takeout.Remark = this.ChangePaidPrice.Remark;
                else
                    takeout.Remark = null;
            }

            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
                ResultModel result = new ResultModel();

                try
                {
                   

                    long UpdateTime;
                    List<TakeoutDetail> resultDetails = new List<TakeoutDetail>();
                    string resultTakeoutSession;

                    result = OperatesService.GetOperates().ServiceAddTakeout(takeout, details, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.TakeoutId = takeout.TakeoutId; return x; }).ToList(), null, out resultDetails, out List<TakeoutPay> temp, out resultTakeoutSession, out UpdateTime);
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


                                                double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); // 获取到需要整零的数量
                                                ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); // 父级中去掉的    自从long 改为double,就不能这样用了 Math.Round(NeedChangeFromParent / product.ProductParentCount + (NeedChangeFromParent % product.ProductParentCount == 0 ? 0 : 1), 3);
                                                ParentRemove = (int)Math.Ceiling(ParentRemove); // 0.1 to 1. 1.1 to 2
                                                
                                                ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); // 应该把所有整零的子产品数加进来  之前的错误:加入产品中的(父级整零后剩余的零的)(NeedChangeFromParent % product.ProductParentCount)


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

                        }
                    }

                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            Print.Instance.PrintTakeoutAfterCheckout(takeout, details);
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), (x) =>
                            {
                                if (x == "OK")
                                    _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.TakeoutBack, null));
                            }, PopupType.Information));
                            Resources.GetRes().DefaultOrderLang = Resources.GetRes().GetMainLangByLangIndex((int)takeout.Lang).MainLangIndex;

                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Back));
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
