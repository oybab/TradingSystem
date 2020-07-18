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

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class TakeoutCheckoutViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;

        private Takeout takeout = null;
        private List<TakeoutDetail> details = null;
        private List<TakeoutPay> payList = null;
        private List<TakeoutPay> tempPayList = new List<TakeoutPay>();



        public TakeoutCheckoutViewModel(Xamarin.Forms.Page element, StackLayout paidPricePanel, ControlTemplate paidPriceTemplate)
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
                            ChangePaidPrice.InitialView(this.takeout.TotalPrice, oldList, true, true, 2);

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
        public Command CheckoutCommand
        {
            get
            {
                return _checkoutCommand ?? (_checkoutCommand = new RelayCommand(param =>
                {

                    if (null != takeout.tb_member && takeout.tb_member.IsAllowBorrow == 0 && takeout.tb_member.BalancePrice < double.Parse(MemberPaidPrice))
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("MemberBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        return;
                        }



                        //余额不平确认
                        if (takeout.BorrowPrice != 0 || takeout.KeepPrice != 0)
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
                    ChangePaidPrice.InitialView(this.takeout.TotalPrice, oldList, true, true, 2);

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


                    IsLoading = false;
                });
            });

            
        }



        /// <summary>
        /// 检查支付并结账
        /// </summary>
        private void CheckPayAndCheckout()
        {
            if (takeout.PaidPrice == 0 && takeout.MemberPaidPrice == 0)
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

            takeout.State = 1;
            bool IsRechecked = false;


            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.Instance.GetString("CheckoutOrder");

            if (takeout.Remark != ChangePaidPrice.Remark)
            {
                if (!string.IsNullOrWhiteSpace(ChangePaidPrice.Remark))
                    takeout.Remark = this.ChangePaidPrice.Remark;
                else
                    takeout.Remark = null;
            }

            IsLoading = true;

            Task.Factory.StartNew(async () =>
            {
   
                try
                {


                    var taskResult = await OperatesService.Instance.ServiceAddTakeout(takeout, details, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.TakeoutId = takeout.TakeoutId; return x; }).ToList(), null);

                    ResultModel result = taskResult.resultModel;
                    List<TakeoutDetail> resultDetails = taskResult.takeoutDetailsResult;
                    List <TakeoutPay> temp = taskResult.takeoutPaysResult;
                    string resultTakeoutSession = taskResult.newTakeoutStateSession;
                    long UpdateTime = taskResult.UpdateTime;

                    if (result.Result)
                    {
                        if (!IsRechecked)
                        {
                           
                            if (null != takeout.tb_member)
                            {
                                Notification.Instance.ActionMember(this, takeout.tb_member, null);
                                takeout.MemberId = takeout.tb_member.MemberId;
                            }

                            Resources.Instance.DefaultOrderLang = Res.Instance.GetMainLangByLangIndex((int)takeout.Lang).MainLangIndex;

                            // 如果成功, 则新增产品
                            foreach (var item in details)
                            {
                                Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {
                                    if (product.BalanceCount < item.Count)
                                    {

                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.Instance.Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

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

                        }
                    }

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        if (result.Result)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), SucMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, (msg)=>
                            {
                                NavigationPath.Instance.TakeoutPage.Init();

                                NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                NavigationPath.Instance.SwitchMasterDetailNavigate(0);


                            }, null);


                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (msg) =>
                                {
                                    NavigationPath.Instance.TakeoutPage.Init();
                                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                    NavigationPath.Instance.SwitchMasterDetailNavigate(0);


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
