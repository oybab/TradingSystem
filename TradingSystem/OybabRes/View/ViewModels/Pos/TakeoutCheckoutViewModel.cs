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

namespace Oybab.Res.View.ViewModels.Pos.Controls
{
    public sealed class TakeoutCheckoutViewModel : ViewModelBase
    {
        private UIElement _window;
        private UIElement _element;

        private Takeout takeout = null;
        private List<TakeoutDetail> details = null;
        private List<TakeoutPay> payList = null;
        private List<TakeoutPay> tempPayList = new List<TakeoutPay>();
        private List<BalanceItemModel> ResultBalanceList = new List<BalanceItemModel>();


        private Action BackAction;
        private Action SuccessAction;


        public TakeoutCheckoutViewModel(UIElement window, UIElement element, Action _backAction, Action _successAction)
        {
            this._window = window;
            this._element = element;
            this.BackAction = _backAction;
            this.SuccessAction = _successAction;

        }


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



            ChangePrice = "0";
            MemberNo = "";
            ReturnPrice = "0";
            BorrowPrice = "0";
            TotalPaidPrice = "0";
            MemberPaidPrice = "0";

            takeout.tb_member = model.tb_member;

            DisplayMode = 1;

            ResultBalanceList.Clear();

            ReloadBalanceList();

            Calc();

            IsDisplay = true;


            if (!IsScanReady)
            {
                IsScanReady = true;
                // 刷卡
                Notification.Instance.NotificationCardReader += Instance_NotificationCardReader;
            }


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
            ScanCardcode(value, null);
        }


        /// <summary>
        /// 扫条形码
        /// </summary>
        private void ScanCardcode(string cardNo, string MemberNoValue)
        {
            if (!_element.IsVisible || !this.IsDisplay)
                return;


            //判断是否空
            if (null == cardNo && MemberNoValue.Trim().Equals(""))
            {
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("MemberNo")), null, PopupType.Warn));
            }
            else
            {
                string memberNo = null;
                if (!string.IsNullOrWhiteSpace(MemberNoValue.Trim()))
                    memberNo = MemberNoValue.Trim();

                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        List<long> Ids = tempPayList.Where(x => null != x.MemberId && x.AddTime == 0).Select(x => x.MemberId.Value).ToList();
                        List<Member> Members;
                            bool result = OperatesService.GetOperates().ServiceGetMembers(0, memberNo, cardNo, null, null, true, out Members);

                            //如果验证成功
                            //修改成功
                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (result && Members.Count > 0)
                                {
                                    // 检查下会员是否到期先
                                    if (Members.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Members.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.Now)
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("MemberExpired"), null, PopupType.Warn));
                                    }
                                    else if (Members.FirstOrDefault().IsEnable == 0)
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("Exception_MemberDisabled"), null, PopupType.Warn));
                                    }
                                    else if (Ids.Contains(Members.FirstOrDefault().MemberId))
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("Member")), null, PopupType.Warn));
                                    }
                                    else
                                    {
                                        AddMember(Members.FirstOrDefault());

                                        if (MemberNoValue != null)
                                            DisplayMode = 1;
                                    }

                                }
                                else
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("MemberNo")), null, PopupType.Warn));
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
                            }));
                        }));
                    }

                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                    }));


                });
            }
        }


        private BalanceItemModel _currentBalance;
        /// <summary>
        /// 会员支付价
        /// </summary>
        public BalanceItemModel CurrentBalance
        {
            get { return _currentBalance; }
            set
            {
                _currentBalance = value;
                OnPropertyChanged("CurrentBalance");
            }
        }
        /// <summary>
        /// 刷新支付列表
        /// </summary>
        private void ReloadBalanceList()
        {
            BalanceItemModel currentModel = ResultBalanceList.FirstOrDefault(x => x.UseState);
            int SelectedIndex = ResultBalanceList.IndexOf(currentModel);


            ResultBalanceList.Clear();
            if (Resources.GetRes().Balances.Count > 0)
            {
                long IncomeOrExpenditure = 2;
 

                List<Balance> balance = Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == IncomeOrExpenditure).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).ToList();


                foreach (var item in balance)
                {
                    string BalanceName = "";


                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + tempPayList.Where(x => x.BalanceId == item.BalanceId).Sum(x => x.OriginalPrice) + "";


                    BalanceName += "  ";
                    if (Resources.GetRes().MainLangIndex == 0)
                        BalanceName += item.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        BalanceName += item.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        BalanceName += item.BalanceName2;

                    ResultBalanceList.Add(new BalanceItemModel() { Text = BalanceName, Balance = item, IsBalance = true, IsChange = true });
                }

                List<TakeoutPay> alreadyAddedModel = new List<TakeoutPay>();

                foreach (var item in tempPayList)
                {
                    if (!alreadyAddedModel.Any(x => x.BalanceId == item.BalanceId && x.MemberId == item.MemberId))
                    {
                        AddMemberOrSupplier(item);
                        alreadyAddedModel.Add(item);
                    }

                }

             

                if (SelectedIndex == -1 || SelectedIndex >= ResultBalanceList.Count)
                {
                    currentModel = ResultBalanceList.FirstOrDefault();
                    if (null != currentModel)
                    {
                        currentModel.UseState = true;
                        CurrentBalance = currentModel;
                    }
                }
                else
                {
                    ResultBalanceList[SelectedIndex].UseState = true;
                    CurrentBalance = currentModel;
                }

                CurrentSelectedItemChange();
            }
        }





        private void CurrentSelectedItemChange()
        {
            BalanceItemModel item = CurrentBalance;
            if (!item.IsBalance)
            {
                IsMember = true;
                if (Resources.GetRes().MainLangIndex == 0)
                    MemberName = item.Member.MemberName0;
                else if (Resources.GetRes().MainLangIndex == 1)
                    MemberName = item.Member.MemberName1;
                else if (Resources.GetRes().MainLangIndex == 2)
                    MemberName = item.Member.MemberName2;

                MemberBalance = item.Member.BalancePrice.ToString();
            }
            else
            {
                IsMember = false;
            }

        }





        private bool FindItemAndAddPrice(Action _action = null, int Mode = 1, bool _isIgnore = false)
        {
            double changePriceDouble = double.Parse(ChangePrice);

            // 去掉的钱不能超出总支付金额
            if (Mode == 2)
            {
                changePriceDouble = double.Parse(KeepPrice);
                if (!Common.GetCommon().IsReturnMoney() && changePriceDouble > Math.Round(tempPayList.Sum(x => x.OriginalPrice), 2))
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("CanNotExceed"), Math.Round(tempPayList.Sum(x => x.OriginalPrice), 2)), null, PopupType.Warn));
                    return false;
                }
            }

            if (0 != changePriceDouble)
            {
                BalanceItemModel item = ResultBalanceList.FirstOrDefault(x => x.UseState);
                if (null == item)
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Save")), null, PopupType.Warn));
                    return false;
                }

               

                if (!item.IsBalance)
                {

                    if (null != item.Member)
                    {
                        double originalPrice = tempPayList.Where(x => x.MemberId == item.Member.MemberId).Sum(x => x.OriginalPrice);


                        if (!_isIgnore && Math.Round(originalPrice / 100.0 * item.Member.OfferRate, 2) + Math.Round(changePriceDouble / 100.0 * item.Member.OfferRate, 2) > item.Member.BalancePrice)
                        {

                            if (item.Member.IsAllowBorrow == 1 || Resources.GetRes().AdminModel.Mode == 2)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmItemBalanceNotEnough"), Resources.GetRes().GetString("Member")), msg =>
                                {
                                    if (msg == "NO")
                                        return;
                                    else
                                        FindItemAndAddPrice(_action, Mode, true);

                                }, PopupType.Question));
                            }
                            else
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("MemberBalanceNotEnough"), null, PopupType.Warn));
                                return false;
                            }
                        }

                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.GetCommon().IsReturnMoney() && Mode == 2 && originalPrice + (-changePriceDouble) < 0)
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("MemberBalanceNotEnough"), null, PopupType.Warn));
                            return false;
                        }

                    }
                    
                }

                if (Mode == 2)
                    changePriceDouble = -changePriceDouble;



                FindItemAndAddToPayModel(item, changePriceDouble);

                ChangePrice = "0";



                ReloadBalanceList();

                Calc();

            }

            if (null != _action)
                _action();

            return true;
        }





        

       


        private void AddMemberOrSupplier(TakeoutPay item)
        {
            string BalanceName = "";
            if (null != item.tb_member)
            {

                if (null != item.tb_member)
                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + (item.tb_member.BalancePrice) + "";



                BalanceName += "(" + Resources.GetRes().PrintInfo.PriceSymbol + tempPayList.Where(x => x.MemberId == item.MemberId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.GetRes().GetString("Member");

                ResultBalanceList.Add(new BalanceItemModel() { Text = BalanceName, Member = item.tb_member, IsBalance = false });
            }
            
        }





        private void SelectBalance(BalanceItemModel model)
        {

            if (null == model)
                return;

            CurrentBalance = model;



            foreach (var item in ResultBalanceList)
            {
                if (model == item)
                    item.UseState = true;
                else
                    item.UseState = false;
            }

          
            CurrentSelectedItemChange();
        }


        /// <summary>
        /// 添加会员
        /// </summary>
        /// <param name="member"></param>
        private void AddMember(Member member)
        {

            AddMemberOrSupplier(new TakeoutPay() { MemberId = member.MemberId, tb_member = member });


            SelectBalance(ResultBalanceList.LastOrDefault());




            double balancePrice = double.Parse(BorrowPrice);

            // 如果会员的钱够, 则放进需要放的钱中.
            if (balancePrice < 0 && (null != member && member.BalancePrice >= Math.Abs(balancePrice)))
            {


                ChangePrice = Math.Abs(balancePrice).ToString();

                Calc();

            }
        }




        /// <summary>
        /// 计算
        /// </summary>
        private void Calc()
        {

            this.TotalPrice = takeout.TotalPrice.ToString();

            takeout.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            takeout.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);




            this.TotalPaidPrice = (takeout.TotalPaidPrice = Math.Round(takeout.MemberPaidPrice + takeout.PaidPrice, 2)).ToString();





            double balancePrice = Math.Round((takeout.TotalPaidPrice + double.Parse(ChangePrice)) - takeout.TotalPrice, 2);


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

            this.ReturnPrice = this.KeepPrice;


            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(takeout.BorrowPrice.ToString());
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(takeout, details, null));
            }

        }





        private bool _isDisplay = false;
        /// <summary>
        /// 是否显示动画
        /// </summary>
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set
            {
                _isDisplay = value;
                OnPropertyChanged("IsDisplay");
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
                        if (this.DisplayMode == 3)
                        {
                            this.BindMemberCommand.Execute(null);
                        }
                        else
                        {
                            this.CheckoutCommand.Execute(null);
                        }
                    });
                }
                return _enterCommand;
            }
        }



        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (DisplayMode == 1 && ChangePrice.Length < 10)
            {
                if (ChangePrice == "0" && no != ".")
                    ChangePrice = no;
                else
                    ChangePrice += no;
                ChangePrices();
            }
          
            else if (DisplayMode == 3 && MemberNo.Length < 16)
            {
                MemberNo += no;
            }
            
        }




        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (DisplayMode == 1 && ChangePrice.Length > 0)
            {
                if (IsAll)
                    ChangePrice = "0";
                else
                {
                    ChangePrice = ChangePrice.Remove(ChangePrice.Length - 1);
                    if (ChangePrice == "")
                        ChangePrice = "0";
                }

                ChangePrices();
            }
            
            else if (DisplayMode == 3 && MemberNo.Length > 0)
            {
                if (IsAll)
                    MemberNo = "";
                else
                {
                    MemberNo = MemberNo.Remove(MemberNo.Length - 1);
                }
            }
           
            else if (DisplayMode == 9 && AddPrice.Length > 0)
            {
                if (IsAll)
                    AddPrice = "0";
                else
                {
                    AddPrice = AddPrice.Remove(AddPrice.Length - 1);
                    if (AddPrice == "")
                        AddPrice = "0";
                }

                ChangePrices();
            }
        }


        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");
        public void ChangePrices()
        {
            string Price = "";
            double OrderPrice = 0;

            // 计算现金
            if (DisplayMode == 1)
            {
                Price = ChangePrice;
                OrderPrice = _lastChangePrice;

                if (Price == "" || Price == "0")
                {
                    Price = "0";
                }
                if (!match.IsMatch(Price))
                {
                    ChangePrice = OrderPrice.ToString();
                    return;
                }
                _lastChangePrice = double.Parse(Price);
            }
           
            else if (DisplayMode == 3)
            {

            }
            
            else if (DisplayMode == 9)
            {
                Price = AddPrice;
                OrderPrice = _lastAddPrice;

                if (Price == "" || Price == "0")
                {
                    Price = "0";
                }
                if (!match.IsMatch(Price))
                {
                    AddPrice = OrderPrice.ToString();
                    return;
                }
                _lastAddPrice = double.Parse(Price);
            }


            if (DisplayMode != 3 && OrderPrice != double.Parse(Price))
                Calc();
            
        }




        private void FindItemAndAddToPayModel(BalanceItemModel item, double changePriceDouble)
        {
            TakeoutPay model = new TakeoutPay();

            if (item.IsBalance)
            {
                model.BalanceId = item.Balance.BalanceId;
                model.OriginalPrice = changePriceDouble;
                model.Rate = item.Balance.RemoveRate;


                if (model.Rate != 0)
                {
                    model.Price = Math.Round(model.OriginalPrice * model.Rate, 2);
                    model.RemovePrice = model.OriginalPrice - model.Price;
                }
                else
                {
                    model.Price = model.OriginalPrice;
                }


            }
            else if (!item.IsBalance)
            {
                if (this.IsMember)
                {
                    model.MemberId = item.Member.MemberId;
                    model.Rate = item.Member.OfferRate;
                    model.tb_member = item.Member;
                }
                


                model.OriginalPrice = changePriceDouble;

                if (model.Rate != 0)
                {
                    model.Price = Math.Round(model.OriginalPrice / 100.0 * model.Rate, 2);
                    model.RemovePrice = model.OriginalPrice - model.Price;
                }
                else
                {
                    model.Price = model.OriginalPrice;
                }




            }

            tempPayList.Add(model);
        }



        
        /// <summary>
        /// 用现金支付少的钱
        /// </summary>
        private void FinishPayPrice()
        {
            double lessPrice = Math.Round(takeout.TotalPrice - takeout.TotalPaidPrice, 2);

            if (lessPrice != 0)
            {
                if (lessPrice > 0)
                {
                    ChangePrice = lessPrice.ToString();

                    Calc();
                }
                

            }
        }





        private int _displayMode;
        /// <summary>
        /// 显示模式 1输入钱 2余额类型选择 3会员号输入
        /// </summary>
        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                OnPropertyChanged("DisplayMode");
            }
        }


        


        private bool _isMember;
        /// <summary>
        /// 是否是会员
        /// </summary>
        public bool IsMember
        {
            get { return _isMember; }
            set
            {
                _isMember = value;
                OnPropertyChanged("IsMember");
            }
        }



       


        private string _memberNo;
        /// <summary>
        /// 会员编号
        /// </summary>
        public string MemberNo
        {
            get { return _memberNo; }
            set
            {
                _memberNo = value;
                OnPropertyChanged("MemberNo");
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



        private string _memberBalance;
        /// <summary>
        /// 会员余额
        /// </summary>
        public string MemberBalance
        {
            get { return _memberBalance; }
            set
            {
                _memberBalance = value;
                OnPropertyChanged("MemberBalance");
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






        private string _changePrice;
        /// <summary>
        /// 修改价格
        /// </summary>
        public string ChangePrice
        {
            get { return _changePrice; }
            set
            {
                _changePrice = value;
                OnPropertyChanged("ChangePrice");
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



        private string _returnPrice;
        /// <summary>
        /// 退回价格
        /// </summary>
        public string ReturnPrice
        {
            get { return _returnPrice; }
            set
            {
                _returnPrice = value;
                OnPropertyChanged("ReturnPrice");
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
                    _backCommand = new RelayCommand(param =>
                    {
                        this.IsDisplay = false;
                        BackAction();
                     });
                }
                return _backCommand;
            }
        }




        /// <summary>
        /// 打开现金命令
        /// </summary>
        private RelayCommand _cashCommand;
        public ICommand CashCommand
        {
            get
            {
                if (_cashCommand == null)
                {
                    _cashCommand = new RelayCommand(param => SetDisplay(1));
                }
                return _cashCommand;
            }
        }


        /// <summary>
        /// 打开余额选择命令
        /// </summary>
        private RelayCommand _balanceCommand;
        public ICommand BalanceCommand
        {
            get
            {
                if (_balanceCommand == null)
                {
                    _balanceCommand = new RelayCommand(param => SetDisplay(2));
                }
                return _balanceCommand;
            }
        }



        /// <summary>
        /// 打开会员卡输入命令
        /// </summary>
        private RelayCommand _memberInputCommand;
        public ICommand MemberInputCommand
        {
            get
            {
                if (_memberInputCommand == null)
                {
                    _memberInputCommand = new RelayCommand(param => SetDisplay(3));
                }
                return _memberInputCommand;
            }
        }


        /// <summary>
        /// 往上
        /// </summary>
        private void GoUp() {
            if (DisplayMode == 1)
                DisplayMode = 2;
            
        }

        /// <summary>
        /// 往下
        /// </summary>
        private void GoDown()
        {
            if (DisplayMode == 2)
                    DisplayMode = 1;
            
        }

        /// <summary>
        /// 往左
        /// </summary>
        private void GoLeft() {
            if (DisplayMode == 2)
            {
                int indexNo = ResultBalanceList.IndexOf(CurrentBalance);
                if (indexNo > 0)
                {
                    SelectBalance(ResultBalanceList[indexNo -1 ]);
                }
            }
        }

        
        /// <summary>
        /// 往右
        /// </summary>
        private void GoRight() {
            if (DisplayMode == 2)
            {
                int indexNo = ResultBalanceList.IndexOf(CurrentBalance);
                if (indexNo < ResultBalanceList.Count - 1)
                {
                    SelectBalance(ResultBalanceList[indexNo + 1]);
                }
            }
        }

        private int _lastMode = 0;
        /// <summary>
        /// 处理KEY
        /// </summary>
        /// <param name="args"></param>
        internal void HandleKey(KeyEventArgs args)
        {
            // 如果是功能(如上下左右,换页)
            if (args.Key >= Key.PageUp && args.Key <= Key.Down && DisplayMode != 3)
            {
                switch (args.Key)
                {
                    case Key.Up:
                        GoUp();
                        break;
                    case Key.Down:
                        GoDown();
                        break;
                    case Key.Left:
                        GoLeft();
                        break;
                    case Key.Right:
                        GoRight();
                        break;
                }
            }
            // 如果要增加数量或减少数量
            else if (args.Key == Key.Enter || args.Key == Key.Escape || args.Key == Key.F10 || args.Key == Key.System || args.Key == Key.Space || args.Key == Key.Add)
            {
                if (args.Key == Key.Enter)
                {
                    if (this.DisplayMode == 3)
                        this.BindMemberCommand.Execute(null);
                    else
                        this.CheckoutCommand.Execute(null);
                }
                else if (args.Key == Key.Add && DisplayMode == 1)
                {
                    FindItemAndAddPrice();
                }
                else if (args.Key == Key.Escape)
                {
                    if (this.DisplayMode == 3 || this.DisplayMode == 9)
                        this.DisplayMode = this._lastMode;
                    else
                    this.BackCommand.Execute(null);
                }
                else if (args.Key == Key.F10 || args.Key == Key.System)
                {
                    if (this.DisplayMode != 9 && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                    {
                        AddMemberPaidPrice();
                    }
                    else
                    {
                        if (this.IsMember)
                            this.UnbindMemberCommand.Execute(null);
                        else
                        {
                            if (DisplayMode != 3)
                            {
                                if (Common.GetCommon().IsBindMemberByNo())
                                {
                                    this._lastMode = this.DisplayMode;
                                    MemberNo = "";
                                    this.DisplayMode = 3;
                                }
                            }
                        }
                    }
                }
                else if (args.Key == Key.Space)
                {
                    FinishPayPrice();
                }
            }
            // 如果是改动数字
            else if ((args.Key >= Key.D0 && args.Key <= Key.D9) || (args.Key >= Key.NumPad0 && args.Key <= Key.NumPad9) || args.Key == Key.OemPeriod || args.Key == Key.Decimal || args.Key == Key.Back)
            {

                if (args.Key == Key.Back)
                {

                    RemoveText(false);

                }
                else
                {
                    string keyChar = Common.GetCommon().GetStrFromKey(args.Key);

                    SetText(keyChar);
                }
            }

        }


        

        /// <summary>
        /// 绑定会员按钮
        /// </summary>
        private RelayCommand _bindMemberCommand;
        public ICommand BindMemberCommand
        {
            get
            {
                if (_bindMemberCommand == null)
                {
                    _bindMemberCommand = new RelayCommand(param =>
                    {
                        if (string.IsNullOrWhiteSpace(MemberNo))
                            return;

                        ScanCardcode(null, MemberNo);
                    });
                }
                return _bindMemberCommand;
            }
        }



        /// <summary>
        /// 解绑会员按钮
        /// </summary>
        private RelayCommand _unbindMemberCommand;
        public ICommand UnbindMemberCommand
        {
            get
            {
                if (_unbindMemberCommand == null)
                {
                    _unbindMemberCommand = new RelayCommand(param =>
                    {
                        BalanceItemModel model = CurrentBalance;

                        long id = 0;

                        if (null != model.Member)
                            id = model.Member.MemberId;


                        List<TakeoutPay> removeModel = new List<TakeoutPay>();
                        foreach (var item in tempPayList)
                        {
                            if (item.MemberId == id)
                                removeModel.Add(item);
                        }


                        foreach (var item in removeModel)
                        {
                            this.tempPayList.Remove(item);
                        }

                        ResultBalanceList.Remove(model);

                        ReloadBalanceList();
                        Calc();
                    });
                }
                return _unbindMemberCommand;
            }
        }


        /// <summary>
        /// 设置模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetDisplay(int mode)
        {
            DisplayMode = mode;
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

                            Action action = new Action(() =>
                            {
                                _element.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //余额不平确认
                                    if (takeout.BorrowPrice != 0)
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
                                }));
                            });

                            if (!FindItemAndAddPrice(action))
                                return;

                            if (takeout.KeepPrice != 0)
                            {
                                if (!FindItemAndAddPrice(action, 2))
                                    return;
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

                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            Print.Instance.PrintTakeoutAfterCheckout(takeout, details);
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), (x) =>
                            {
                                if (x == "OK")
                                {
                                    this.IsDisplay = false;
                                    SuccessAction();
                                }
                            }, PopupType.Information));

                            Resources.GetRes().DefaultOrderLang = Resources.GetRes().GetMainLangByLangIndex((int)takeout.Lang).MainLangIndex;
                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                this.BackCommand.Execute(null);
                                
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









        private double _lastAddPrice = 0;
        private double _lastChangePrice = 0;


        private string _addPrice;
        /// <summary>
        /// 增加会员支付余额金额
        /// </summary>
        public string AddPrice
        {
            get { return _addPrice; }
            set
            {
                _addPrice = value;
                OnPropertyChanged("AddPrice");
            }
        }


        private int _payType;
        /// <summary>
        /// 会员支付类型(1现金2刷卡)
        /// </summary>
        public int PayType
        {
            get { return _payType; }
            set
            {
                _payType = value;
                OnPropertyChanged("PayType");
            }
        }




        /// <summary>
        /// 增加会员支付
        /// </summary>
        private void AddMemberPay()
        {

            if (AddPrice == "")
            {
                AddPrice = "0";
                return;
            }

            MemberPay memberpay = new MemberPay();

            memberpay.MemberPayId = -1;
            memberpay.Price = Math.Round(double.Parse(AddPrice), 2);

            memberpay.MemberId = this.takeout.tb_member.MemberId;

            if (memberpay.Price == 0)
                return;


            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
                ResultModel result = new ResultModel();
                double originalBalancePrice = this.takeout.tb_member.BalancePrice;
                try
                {
                    this.takeout.tb_member.BalancePrice = this.takeout.tb_member.BalancePrice + memberpay.Price;

                    Member newMember;
                    MemberPay newMemberPay;
                    result = OperatesService.GetOperates().ServiceAddMemberPay(this.takeout.tb_member, memberpay, out newMember, out newMemberPay);
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveSuccess"), null, PopupType.Information));

                            this.takeout.tb_member = newMember;


                            AddMember(this.takeout.tb_member);

                            this.DisplayMode = _lastMode;


                        }
                        else
                        {
                            if (result.IsDataHasRefrence)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PropertyUsed"), null, PopupType.Warn));
                            }
                            else if (result.UpdateModel)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PropertyUsed"), null, PopupType.Warn));
                            }
                            else
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveFailt"), null, PopupType.Warn));

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
                            if (result.Result)
                                this.takeout.tb_member.BalancePrice = originalBalancePrice;

                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));

                        }), false, Resources.GetRes().GetString("SaveFailt"));
                    }));
                }
                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                }));

            });
        }



















        /// <summary>
        /// 增加会员支付金额
        /// </summary>
        public void AddMemberPaidPrice()
        {
            if (ResultBalanceList.Count > 0 && null != CurrentBalance && CurrentBalance.IsBalance && null != CurrentBalance.Balance)
            {
                BalanceItemModel model = ResultBalanceList.LastOrDefault();


                double changePrice = double.Parse(this.ChangePrice);

                if (changePrice != 0 && null != model && null != model.Member)
                {
                    string memberName = "";
                    string balanceName = "";


                    if (Resources.GetRes().MainLangIndex == 0)
                        memberName = model.Member.MemberName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        memberName = model.Member.MemberName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        memberName = model.Member.MemberName2;


                    if (Resources.GetRes().MainLangIndex == 0)
                        balanceName = CurrentBalance.Balance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        balanceName = CurrentBalance.Balance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        balanceName = CurrentBalance.Balance.BalanceName2;




                    //确认取消
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmAddMemberPay"), ChangePrice, balanceName, memberName), msg =>
                    {
                        if (msg == "NO")
                            return;



                        MemberPay memberpay = new MemberPay();


                        memberpay.Price = changePrice;
                        memberpay.BalanceId = CurrentBalance.Balance.BalanceId;
                        memberpay.MemberId = model.Member.MemberId;


                        // 开始支付
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                        Task.Factory.StartNew(() =>
                        {
                            ResultModel result = new ResultModel();
                            double originalBalancePrice = model.Member.BalancePrice;
                            try
                            {
                                // 更新会员信息
                                model.Member.BalancePrice = model.Member.BalancePrice + memberpay.Price;

                                Member newMember;
                                MemberPay newMemberPay;
                                result = OperatesService.GetOperates().ServiceAddMemberPay(model.Member, memberpay, out newMember, out newMemberPay);
                                _element.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    if (result.Result)
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveSuccess"), null, PopupType.Information));


                                        // 去掉当前最后一个会员并重新添加
                                        this.UnbindMemberCommand.Execute(null);
                                        AddMember(newMember);
                                    }
                                    else
                                    {
                                        if (result.IsDataHasRefrence)
                                        {
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PropertyUsed"), null, PopupType.Warn));

                                        }
                                        else if (result.UpdateModel)
                                        {
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PropertyUnSame"), null, PopupType.Warn));

                                        }
                                        else
                                        {
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveFailt"), null, PopupType.Warn));

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
                                        // 失败了就复原会员信息
                                        if (result.Result)
                                            model.Member.BalancePrice = originalBalancePrice;

                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));

                                    }), false, Resources.GetRes().GetString("SaveFailt"));
                                }));
                            }

                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                            }));

                        });






                    }, PopupType.Question));



                }
            }

        }

    }
}
