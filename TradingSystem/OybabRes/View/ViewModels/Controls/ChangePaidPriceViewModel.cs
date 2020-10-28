using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Oybab.Res.Tools;
using System.Text.RegularExpressions;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class ChangePaidPriceViewModel : ViewModelBase
    {
        private UIElement _element;
        private Action Recalc;
        private Panel BalanceList;
        private Style BalanceStyle;
        private bool IsMember = true;

        internal List<CommonPayModel> PayModel { get; private set; } //返回值


        internal ChangePaidPriceViewModel(UIElement element, Action Recalc, Panel balanceList)
        {
            this._element = element;
            this.Recalc = Recalc;
            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);

            this.BalanceList = balanceList;
            this.BalanceStyle = (BalanceList as FrameworkElement).FindResource("cbSelectStyle") as Style;

            this.AddMemberView = new AddMemberViewModel(element, AddNewMemberOrSupplier);
        }


        private List<BalanceItemModel> ResultBalanceList = new List<BalanceItemModel>();

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(double TotalPrice, List<CommonPayModel> PayModel, bool IsMember = true, bool IsCheckout = false)
        {
            this.Mode = 1;
            this.BalanceMode = 0;
            this.DisplayMode = 0;
            this.TotalPrice = TotalPrice.ToString();
            this.IsMember = IsMember;
            this.ChangePrice = "0";
            this.PayModel = PayModel;
            this.IsCheckout = IsCheckout;

            this.IsAddedMemberOrSupplier = false;


            if (IsMember)
                MemberName = Resources.GetRes().GetString("MemberName");
            else
                MemberName = Resources.GetRes().GetString("SupplierName");

            if (null == this.Remark)
                Remark = "";

            ResultBalanceList.Clear();

            DisplayRemark = Remark;

            ReloadBalanceList();

            Calc();

            // 把滚动框对齐到第一
            ScrollViewer sv = BalanceList.Parent as ScrollViewer;
            if (null != sv)
                sv.ScrollToLeftEnd();
        }


        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.ChangePrice.Length < 10)
            {
                if (this.ChangePrice == "0" && no != ".")
                    ChangePrice = no;
                else
                    this.ChangePrice += no;

                ChangePrices();
            }
        }

        private int _displayMode;
        /// <summary>
        /// 显示模式(文本框选择为: 1现金2刷卡3备注)
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


        private int _mode = 1;
        /// <summary>
        /// 模式(1增加2减少)
        /// </summary>
        public int Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                OnPropertyChanged("Mode");
            }
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


        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.ChangePrice.Length > 0)
            {
                if (IsAll)
                    this.ChangePrice = "0";
                else
                    this.ChangePrice = this.ChangePrice.Remove(this.ChangePrice.Length - 1);

                if (ChangePrice == "")
                    ChangePrice = "0";

                ChangePrices();
            }
           
        }

        /// <summary>
        /// 选择下一个余额
        /// </summary>
        public void SelectNextBalance()
        {
            if (ResultBalanceList.Count > 1)
            {
                BalanceItemModel currentModel = ResultBalanceList.FirstOrDefault(x => x.UseState);
                int SelectedIndex = ResultBalanceList.IndexOf(currentModel);

                if (SelectedIndex < ResultBalanceList.Count - 1)
                    SelectedIndex = SelectedIndex + 1;
                else
                    SelectedIndex = 0;

                currentModel = ResultBalanceList[SelectedIndex];


                foreach (var item in BalanceList.Children)
                {
                    CheckBox cb = item as CheckBox;

                    if (null != cb && cb.DataContext == currentModel)
                    {
                        cb.Command.Execute(currentModel);

                        ScrollViewer sv = BalanceList.Parent as ScrollViewer;
                        if (null != sv)
                        {
                            cb.BringIntoView();
                        }
                        break;
                    }
                }

               
            }
        }




        /// <summary>
        /// 刷新支付列表
        /// </summary>
        private void ReloadBalanceList()
        {
            BalanceItemModel currentModel = ResultBalanceList.FirstOrDefault(x => x.UseState);
            int SelectedIndex = ResultBalanceList.IndexOf(currentModel);

            BalanceList.Children.Clear();
            ResultBalanceList.Clear();
            if (Resources.GetRes().Balances.Count > 0)
            {
                long IncomeOrExpenditure = 0;
                if (IsMember)
                    IncomeOrExpenditure = 2;
                else
                    IncomeOrExpenditure = 3;

                List<Balance> balance = Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == IncomeOrExpenditure).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).ToList();


                foreach (var item in balance)
                {
                    string BalanceName = "";


                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + PayModel.Where(x => x.BalanceId == item.BalanceId).Sum(x => x.OriginalPrice) + "";


                    BalanceName += "  ";
                    if (Resources.GetRes().MainLangIndex == 0)
                        BalanceName += item.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        BalanceName += item.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        BalanceName += item.BalanceName2;

                    ResultBalanceList.Add(new BalanceItemModel() { Text = BalanceName, Balance = item, IsBalance = true, IsChange = true });
                }

                List<CommonPayModel> alreadyAddedModel = new List<CommonPayModel>();

                foreach (var item in PayModel)
                {
                    if (!alreadyAddedModel.Any(x => x.BalanceId == item.BalanceId && x.MemberId == item.MemberId && x.SupplierId == x.SupplierId && x.ParentId == item.ParentId && x.IsChange == item.IsChange))
                    {
                        AddMemberOrSupplier(item);
                        alreadyAddedModel.Add(item);
                    }

                }

                foreach (var item in ResultBalanceList)
                {
                    AddBalanceItem(item);
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


        /// <summary>
        /// 添加卡片阅读器
        /// </summary>
        /// <param name="item"></param>
        private void AddBalanceItem(BalanceItemModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = BalanceStyle;
                btn.DataContext = item;
                btn.Command = SelectBalance;
                btn.CommandParameter = item;
                BalanceList.Children.Add(btn);
            }));
        }




        private bool FindItemAndAddPrice(Action _action, bool _isIgnore = false)
        {
            double changePriceDouble = double.Parse(ChangePrice);
            // 去掉的钱不能超出总支付金额
            if (Mode == 2)
            {

                if (!Common.GetCommon().IsReturnMoney() && changePriceDouble > Math.Round(PayModel.Sum(x => x.OriginalPrice), 2))
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("CanNotExceed"), Math.Round(PayModel.Sum(x => x.OriginalPrice), 2)), null, PopupType.Warn));
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

                if (!item.IsChange)
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Save")), null, PopupType.Warn));
                    return false;
                }

                if (!item.IsBalance)
                {

                    if (null != item.Member)
                    {
                        double originalPrice = PayModel.Where(x => x.IsChange && x.MemberId == item.Member.MemberId).Sum(x => x.OriginalPrice);
                        // 输入的钱不能多于余额
                        if (Mode == 1)
                        {
                            if (!_isIgnore && Math.Round(originalPrice / 100.0 * item.Member.OfferRate, 2) + Math.Round(changePriceDouble / 100.0 * item.Member.OfferRate, 2) > item.Member.BalancePrice)
                            {
                                if (item.Member.IsAllowBorrow == 1 || Resources.GetRes().AdminModel.Mode == 2)
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmItemBalanceNotEnough"), Resources.GetRes().GetString("Member")), msg =>
                                    {
                                        if (msg == "NO")
                                            return;
                                        else
                                            FindItemAndAddPrice(_action, true);

                                    }, PopupType.Question));
                                }
                                else
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("MemberBalanceNotEnough"), null, PopupType.Warn));
                                    return false;
                                }
                            }
                        }

                        originalPrice = PayModel.Where(x => x.MemberId == item.Member.MemberId).Sum(x => x.OriginalPrice);
                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.GetCommon().IsReturnMoney() && Mode == 2 && originalPrice + (-changePriceDouble) < 0)
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("MemberBalanceNotEnough"), null, PopupType.Warn));
                            return false;
                        }

                    }
                    else if (null != item.Supplier)
                    {
                        double originalPrice = PayModel.Where(x => x.IsChange && x.SupplierId == item.Supplier.SupplierId).Sum(x => x.OriginalPrice);
                        // 输入的钱不能多于余额
                        if (Mode == 1)
                        {
                            if (!_isIgnore && Math.Round(originalPrice / 100.0 * item.Supplier.OfferRate, 2) + Math.Round(changePriceDouble / 100.0 * item.Supplier.OfferRate, 2) > item.Supplier.BalancePrice)
                            {
                                if (item.Supplier.IsAllowBorrow == 1 || Resources.GetRes().AdminModel.Mode == 2)
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmItemBalanceNotEnough"), Resources.GetRes().GetString("Supplier")), msg =>
                                    {
                                        if (msg == "NO")
                                            return;
                                        else
                                            FindItemAndAddPrice(_action, true);

                                    }, PopupType.Question));
                                    return false;
                                }
                                else
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SupplierBalanceNotEnough"), null, PopupType.Warn));
                                    return false;
                                }
                            }
                        }

                        originalPrice = PayModel.Where(x => x.SupplierId == item.Supplier.SupplierId).Sum(x => x.OriginalPrice);
                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.GetCommon().IsReturnMoney() && Mode == 2 && originalPrice + (-changePriceDouble) < 0)
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SupplierBalanceNotEnough"), null, PopupType.Warn));
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


        private void AddMemberOrSupplier(CommonPayModel item)
        {
            string BalanceName = "";
            if (IsMember && null != item.MemberId)
            {

                if (null != item.Member && item.IsChange)
                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + (item.Member.BalancePrice) + "";



                BalanceName += "(" + Resources.GetRes().PrintInfo.PriceSymbol + PayModel.Where(x => x.MemberId == item.MemberId && x.ParentId == item.ParentId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.GetRes().GetString("Member");

                ResultBalanceList.Add(new BalanceItemModel() { Text = BalanceName, Member = item.Member, IsBalance = false, IsChange = item.IsChange });
            }
            else if (!IsMember && null != item.SupplierId)
            {

                if (null != item.Supplier && item.IsChange)
                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + (item.Supplier.BalancePrice) + "";

                BalanceName += "(" + Resources.GetRes().PrintInfo.PriceSymbol + PayModel.Where(x => x.SupplierId == item.SupplierId && x.ParentId == item.ParentId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.GetRes().GetString("Member");

                ResultBalanceList.Add(new BalanceItemModel() { Text = BalanceName, Supplier = item.Supplier, IsBalance = false, IsChange = item.IsChange });
            }
        }

        private void FindItemAndAddToPayModel(BalanceItemModel item, double changePriceDouble)
        {
            CommonPayModel model = new CommonPayModel();

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
                    model.Member = item.Member;
                }
                else
                {
                    model.SupplierId = item.Supplier.SupplierId;
                    model.Rate = item.Supplier.OfferRate;
                    model.Supplier = item.Supplier;
                }

                model.IsChange = item.IsChange;
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

            PayModel.Add(model);
        }




        /// <summary>
        /// 修改价格
        /// </summary>
        public void ChangePrices()
        {
            if (this.ChangePrice == "")
                this.ChangePrice = "0";
            else
            {
                double price = 0;
                if (!double.TryParse(ChangePrice, out price))
                {
                    this.ChangePrice = "0";
                }

                if (!ChangePrice.EndsWith("."))
                    this.ChangePrice = Math.Round(price, 2).ToString();
            }
            

            Calc();



        }


        private void Calc()
        {
            PaidPrice = Math.Round(PayModel.Sum(x => x.OriginalPrice), 2).ToString();
            double balancePrice = 0;
            if (Mode == 1)
                BalancePrice = (balancePrice = Math.Round(Math.Round(PayModel.Sum(x => x.OriginalPrice), 2) + Math.Round(double.Parse(ChangePrice), 2) - double.Parse(TotalPrice), 2)).ToString();
            else if (Mode == 2)
                BalancePrice = (balancePrice = Math.Round(Math.Round(PayModel.Sum(x => x.OriginalPrice), 2) - Math.Round(double.Parse(ChangePrice), 2) - double.Parse(TotalPrice), 2)).ToString();


            if (balancePrice > 0)
                BalanceMode = 1;
            else if (balancePrice < 0)
                BalanceMode = 2;
            else
                BalanceMode = 0;
        }

        /// <summary>
        /// 回车
        /// </summary>
        public void Handle(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_keyboardLittle.IsDisplayKeyboard)
                {
                    _keyboardLittle.OKCommand.Execute(null);
                }
                else
                {
                    // 无法用, 因为已经失去了焦点
                    this.OKCommand.Execute(null);
                }
            }else if (e.Key == Key.Escape)
            {
                if (_keyboardLittle.IsDisplayKeyboard)
                {
                    _keyboardLittle.IsDisplayKeyboard = false;
                }
            }
        }


        /// <summary>
        /// 命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetCommand(string no)
        {
            // 确定
            if (no == "OK")
            {
                this.KeyboardLittle.IsDisplayKeyboard = false;
                if (this.IsDisplay)
                    this.DisplayMode = 0;
                ClearFocus();
            }
            // 取消
            else if (no == "Cancel")
            {
                RemoveText(true);
            }
            // 删除
            else if (no == "Del")
            {
                RemoveText(false);
            }
        }


        private KeyboardLittleViewModel _keyboardLittle;
        /// <summary>
        /// 小键盘
        /// </summary>
        public KeyboardLittleViewModel KeyboardLittle
        {
            get { return _keyboardLittle; }
            set
            {
                _keyboardLittle = value;
                OnPropertyChanged("KeyboardLittle");
            }
        }


        /// <summary>
        /// 去掉焦点
        /// </summary>
        private void ClearFocus()
        {
            var scope = FocusManager.GetFocusScope(_element); // elem is the UIElement to unfocus
            FocusManager.SetFocusedElement(scope, null); // remove logical focus
            Keyboard.ClearFocus(); // remove keyboard focus
        }


        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

            IsDisplay = true;
            IsShow = true;
        }



        /// <summary>
        /// 隐藏
        /// </summary>
        internal void Hide()
        {
            IsShow = false;

            new Action(() =>
            {
                System.Threading.Thread.Sleep(Resources.GetRes().AnimateTime);

                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsDisplay = false;


                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));
                }));

            }).BeginInvoke(null, null);

        }





        private bool _isShow = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                OnPropertyChanged("IsShow");
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




        private bool _isMemberAdd = false;
        /// <summary>
        /// 增加会员显示
        /// </summary>
        public bool IsMemberAdd
        {
            get { return _isMemberAdd; }
            set
            {
                _isMemberAdd = value;
                OnPropertyChanged("IsMemberAdd");
            }
        }

        private bool _isMemberRemove = false;
        /// <summary>
        /// 去掉会员显示
        /// </summary>
        public bool IsMemberRemove
        {
            get { return _isMemberRemove; }
            set
            {
                _isMemberRemove = value;
                OnPropertyChanged("IsMemberRemove");
            }
        }

        private bool _isMemberAddShow = false;
        /// <summary>
        /// 会员名称容器显示
        /// </summary>
        public bool IsMemberAddShow
        {
            get { return _isMemberAddShow; }
            set
            {
                _isMemberAddShow = value;
                OnPropertyChanged("IsMemberAddShow");
            }
        }


        private bool _isMemberNameValueShow = false;
        /// <summary>
        /// 会员名称内容显示
        /// </summary>
        public bool IsMemberNameValueShow
        {
            get { return _isMemberNameValueShow; }
            set
            {
                _isMemberNameValueShow = value;
                OnPropertyChanged("IsMemberNameValueShow");
            }
        }


        private bool _isSave = false;
        /// <summary>
        /// 会员名称内容显示
        /// </summary>
        public bool IsSave
        {
            get { return _isSave; }
            set
            {
                _isSave = value;
                OnPropertyChanged("IsSave");
            }
        }



        private bool _isCheckout = false;
        /// <summary>
        /// 是否是结账
        /// </summary>
        public bool IsCheckout
        {
            get { return _isCheckout; }
            set
            {
                _isCheckout = value;
                OnPropertyChanged("IsCheckout");
            }
        }




        private string _totalPrice = "0";
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



        private string _paidPrice = "0";
        /// <summary>
        /// 支付金额
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



        private string _memberPaidPrice = "0";
        /// <summary>
        /// 会员支付金额
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



        private string _memberName = "";
        /// <summary>
        /// 会员名称
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



        private string _memberNameValue = "";
        /// <summary>
        /// 会员名称内容
        /// </summary>
        public string MemberNameValue
        {
            get { return _memberNameValue; }
            set
            {
                _memberNameValue = value;
                OnPropertyChanged("MemberNameValue");
            }
        }




        private string _changePrice = "0";
        /// <summary>
        /// 修改的现金
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


     
        
        private string _balancePrice = "0";
        /// <summary>
        /// 余额
        /// </summary>
        public string BalancePrice
        {
            get { return _balancePrice; }
            set
            {
                _balancePrice = value;
                OnPropertyChanged("BalancePrice");
            }
        }


        private string _remark = "";
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                OnPropertyChanged("Remark");
            }
        }


        private string _displayRemark = "";
        /// <summary>
        /// 显示的备注
        /// </summary>
        public string DisplayRemark
        {
            get { return _displayRemark; }
            set
            {
                _displayRemark = value;
                OnPropertyChanged("DisplayRemark");
            }
        }



        /// <summary>
        /// 预设
        /// </summary>
        /// <param name="price"></param>
        internal void SetChangePrice(double price, string mark)
        {
            if (mark == "+")
                Mode = 1;
            else if (mark == "-")
                Mode = 2;


            ChangePrice = price.ToString();

            Calc();
        }




        private bool IsAddedMemberOrSupplier = false;
        private BalanceItemModel CurrentBalance = null;
        /// <summary>
        /// 选择卡片阅读器
        /// </summary>
        private RelayCommand _selectBalance;
        public ICommand SelectBalance
        {
            get
            {
                if (_selectBalance == null)
                {
                    _selectBalance = new RelayCommand(param =>
                    {
                        BalanceItemModel model = param as BalanceItemModel;
                        if (null == model)
                            return;

                        CurrentBalance = model;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in ResultBalanceList)
                        {
                            if (model == item)
                                item.UseState = true;
                            else
                                item.UseState = false;
                        }

                        CurrentSelectedItemChange();

                    });
                }
                return _selectBalance;
            }
        }


        private void CurrentSelectedItemChange()
        {
            IsSave = true;
            BalanceItemModel item = CurrentBalance;
            if (!item.IsBalance)
            {
                IsMemberAddShow = true;
                if (item.IsChange)
                {
                    IsMemberAdd = false;
                    IsMemberRemove = true;
                    IsMemberNameValueShow = true;
                    if (IsMember)
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            MemberNameValue = item.Member.MemberName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            MemberNameValue = item.Member.MemberName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            MemberNameValue = item.Member.MemberName2;
                    }
                    else
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            MemberNameValue = item.Supplier.SupplierName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            MemberNameValue = item.Supplier.SupplierName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            MemberNameValue = item.Supplier.SupplierName2;
                    }
                }
                else
                {
                    IsSave = false;
                    IsMemberAddShow = false;
                }
            }
            else
            {
                if (IsAddedMemberOrSupplier)
                {
                    IsMemberAddShow = false;
                }
                else
                {
                    IsMemberAddShow = true;
                    IsMemberAdd = true;
                    IsMemberRemove = false;
                    IsMemberNameValueShow = false;
                }
            }

            if (IsMember && !Common.GetCommon().IsBindMemberByNo())
            {
                IsMemberAddShow = false;
            }
            else if (!IsMember && !Common.GetCommon().IsBindSupplierByNo())
            {
                IsMemberAddShow = false;
            }
        }




        private AddMemberViewModel _addMemberView;
        /// <summary>
        /// 添加会员窗口
        /// </summary>
        public AddMemberViewModel AddMemberView
        {
            get { return _addMemberView; }
            set
            {
                _addMemberView = value;
                OnPropertyChanged("AddMemberView");
            }
        }



        /// <summary>
        /// 添加会员
        /// </summary>
        private RelayCommand _addMemberCommand;
        public ICommand AddMemberCommand
        {
            get
            {
                if (_addMemberCommand == null)
                {
                    _addMemberCommand = new RelayCommand(param =>
                    {

                        OpenAddMember();
                    });
                }
                return _addMemberCommand;
            }
        }



        private void OpenAddMember(bool IsScan = false, string CodeNo = null)
        {
            List<long> Ids = new List<long>();

            if (IsMember)
                Ids = PayModel.Where(x => null != x.MemberId && x.AddTime == 0).Select(x => x.MemberId.Value).ToList();
            else
                Ids = PayModel.Where(x => null != x.SupplierId && x.AddTime == 0).Select(x => x.SupplierId.Value).ToList();

            AddMemberView.Init(IsMember, Ids, IsScan);

            if (IsScan)
                AddMemberView.SearchByScanner(CodeNo);

            AddMemberView.Show();
        }

        private void AddNewMemberOrSupplier(object ReturnValue)
        {
            Member member = ReturnValue as Member;
            Supplier supplier = ReturnValue as Supplier;

            AddMember(member, supplier);

            ScrollViewer sv = BalanceList.Parent as ScrollViewer;
            if (null != sv)
                sv.ScrollToRightEnd();
        }



        /// <summary>
        /// 添加会员
        /// </summary>
        /// <param name="member"></param>
        private void AddMember(Member member, Supplier supplier)
        {


            CommonPayModel model = new CommonPayModel();

            if (null != member)
            {
                model.MemberId = member.MemberId;
                model.Member = member;
                model.IsChange = true;
            }

            if (null != supplier)
            {
                model.SupplierId = supplier.SupplierId;
                model.Supplier = supplier;
                model.IsChange = true;
            }


            AddMemberOrSupplier(model);

            AddBalanceItem(ResultBalanceList.LastOrDefault());
            

            SelectBalance.Execute(ResultBalanceList.LastOrDefault());


            IsAddedMemberOrSupplier = true;



            double balancePrice = double.Parse(BalancePrice);

            // 如果会员的钱够, 则放进需要放的钱中.
            if (Mode == 1 && balancePrice < 0 && (null != member && member.BalancePrice >= Math.Abs(balancePrice)) || (null != supplier && supplier.BalancePrice >= Math.Abs(balancePrice)))
            {


                ChangePrice = Math.Abs(balancePrice).ToString();

                Calc();

            }
        }




        /// <summary>
        /// 去掉会员
        /// </summary>
        private RelayCommand _removeMemberCommand;
        public ICommand RemoveMemberCommand
        {
            get
            {
                if (_removeMemberCommand == null)
                {
                    _removeMemberCommand = new RelayCommand(param =>
                    {
                        BalanceItemModel model = CurrentBalance;

                        long id = 0;

                        if (null != model.Member)
                            id = model.Member.MemberId;
                        else if (null != model.Supplier)
                            id = model.Supplier.SupplierId;


                        List<CommonPayModel> removeModel = new List<CommonPayModel>();
                        foreach (var item in PayModel.Where(x => x.IsChange))
                        {
                            if (IsMember && item.MemberId == id)
                                removeModel.Add(item);
                            else if (!IsMember && item.SupplierId == id)
                                removeModel.Add(item);
                        }


                        foreach (var item in removeModel)
                        {
                            this.PayModel.Remove(item);
                        }

                        ResultBalanceList.Remove(model);

                        IsAddedMemberOrSupplier = false;
                        ReloadBalanceList();
                        Calc();

                        ScrollViewer sv = BalanceList.Parent as ScrollViewer;
                        if (null != sv)
                            sv.ScrollToLeftEnd();

                        if (null != Recalc)
                            Recalc();

                    });
                }
                return _removeMemberCommand;
            }
        }


        /// <summary>
        /// 根据扫码绑定会员
        /// </summary>
        /// <param name="code"></param>
        internal void OpenMemberByScanner(string code)
        {
            if (!AddMemberView.IsShow && !this.IsAddedMemberOrSupplier && null != CurrentBalance && CurrentBalance.IsBalance)
            {
                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    OpenAddMember(true, code);
                }));

            }

        }




        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");
        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(param =>
                    {

                        FindItemAndAddPrice(() =>
                        {
                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (string.IsNullOrWhiteSpace(this.DisplayRemark))
                                    this.DisplayRemark = null;

                                this.Remark = this.DisplayRemark;

                                if (null != Recalc)
                                    Recalc();


                                if (!this.IsCheckout)
                                    this.Hide();
                            }));
                        });
                    

                       


                    });
                }
                return _okCommand;
            }
        }





        /// <summary>
        /// 增加按钮
        /// </summary>
        private RelayCommand _addCommand;
        public ICommand AddCommand
        {
            get
            {
                if (_addCommand == null)
                {
                    _addCommand = new RelayCommand(param =>
                    {
                        FindItemAndAddPrice(null);

                    });
                }
                return _addCommand;
            }
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(param =>
                    {
                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }







        /// <summary>
        /// 增加支付金额
        /// </summary>
        public void AddMemberPaidPrice()
        {
            if (ResultBalanceList.Count > 0 && null != CurrentBalance && CurrentBalance.IsBalance && null != CurrentBalance.Balance)
            {
                BalanceItemModel model = ResultBalanceList.LastOrDefault();


                double changePrice = double.Parse(this.ChangePrice);

                if (changePrice != 0 && null != model && null != model.Member && model.IsChange)
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
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmAddPay"), Resources.GetRes().PrintInfo.PriceSymbol, ChangePrice, balanceName, memberName), msg =>
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
                                        RemoveMemberCommand.Execute(null);
                                        AddMember(newMember, null);
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
                else if (changePrice != 0 && null != model && null != model.Supplier && model.IsChange)
                {
                    string supplierName = "";
                    string balanceName = "";


                    if (Resources.GetRes().MainLangIndex == 0)
                        supplierName = model.Supplier.SupplierName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        supplierName = model.Supplier.SupplierName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        supplierName = model.Supplier.SupplierName2;


                    if (Resources.GetRes().MainLangIndex == 0)
                        balanceName = CurrentBalance.Balance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        balanceName = CurrentBalance.Balance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        balanceName = CurrentBalance.Balance.BalanceName2;




                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmAddPay"), Resources.GetRes().PrintInfo.PriceSymbol, ChangePrice, balanceName, supplierName), msg =>
                    {
                        if (msg == "NO")
                            return;

                        SupplierPay supplierpay = new SupplierPay();


                        supplierpay.Price = changePrice;
                        supplierpay.BalanceId = CurrentBalance.Balance.BalanceId;
                        supplierpay.SupplierId = model.Supplier.SupplierId;


                        // 开始支付
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                        Task.Factory.StartNew(() =>
                    {
                        ResultModel result = new ResultModel();
                        double originalBalancePrice = model.Supplier.BalancePrice;
                        try
                        {
                            // 更新供应者信息
                            model.Supplier.BalancePrice = model.Supplier.BalancePrice + supplierpay.Price;

                            Supplier newSupplier;
                            SupplierPay newSupplierPay;
                            result = OperatesService.GetOperates().ServiceAddSupplierPay(model.Supplier, supplierpay, out newSupplier, out newSupplierPay);
                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveSuccess"), null, PopupType.Information));



                                    // 去掉当前最后一个供应者并重新添加
                                    RemoveMemberCommand.Execute(null);
                                    AddMember(null, newSupplier);
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
                                    // 失败了就复原供应者信息
                                    if (result.Result)
                                        model.Supplier.BalancePrice = originalBalancePrice;

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

        /// <summary>
        /// 快速整平余额
        /// </summary>
        public void FinishPaidPrice()
        {
            double lessPrice = Math.Round(double.Parse(TotalPrice) - Math.Round(PayModel.Sum(x => x.OriginalPrice), 2), 2);

            if (lessPrice != 0)
            {
                if (lessPrice > 0)
                {
                    SetChangePrice(lessPrice, "+");
                }
                else if (lessPrice < 0)
                {
                    SetChangePrice(Math.Abs(lessPrice), "-");
                }

            }
        }


    }
}
