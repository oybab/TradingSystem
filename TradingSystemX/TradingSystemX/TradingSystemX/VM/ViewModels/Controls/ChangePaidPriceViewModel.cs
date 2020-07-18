using Oybab.DAL;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Oybab.TradingSystemX.Tools;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using Oybab.TradingSystemX.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Exceptions;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class ChangePaidPriceViewModel : ViewModelBase
    {
        private Xamarin.Forms.View _element;
        private Action Recalc;
        private Xamarin.Forms.StackLayout spBalanceList;
        private Xamarin.Forms.ControlTemplate BalanceControlTemplate;
        private bool IsMember = true;


        internal List<CommonPayModel> PayModel { get; private set; } //返回值

        internal ChangePaidPriceViewModel(Xamarin.Forms.View element, Action Recalc, StackLayout balanceList, ControlTemplate ctBalanceList)
        {
            this._element = element;
            this.Recalc = Recalc;
            this.spBalanceList = balanceList;
            this.BalanceControlTemplate = ctBalanceList;

            this.AddMemberView = new AddMemberViewModel(element, AddNewMemberOrSupplier);
        }

        private ObservableCollection<BalanceItemModel> _currentBalanceList = new ObservableCollection<BalanceItemModel>();
        /// <summary>
        /// 当前余额列表
        /// </summary>
        public ObservableCollection<BalanceItemModel> CurrentBalanceList
        {
            get { return _currentBalanceList; }
            set
            {
                _currentBalanceList = value;
                OnPropertyChanged("CurrentBalanceList");
            }
        }
        private List<TemplatedView> tempBalanceViewList = new List<TemplatedView>();


        /// <summary>
        /// 初始化需求
        /// </summary>
        internal void InitialView(double TotalPrice, List<CommonPayModel> PayModel, bool IsMember = true, bool IsCheckout = false, int Source = 0)
        {
            this.Mode = 1;
            this.BalanceMode = 0;
            this.TotalPrice = TotalPrice.ToString();
            this.IsMember = IsMember;
            this.ChangePrice = "0";
            this.PayModel = PayModel;
            this.IsCheckout = IsCheckout;

            IsAddedMemberOrSupplier = false;


            if (IsCheckout)
            {
                if (Source == 1 && null == NavigationPath.Instance.OrderCheckoutPaidPanelClose)
                    NavigationPath.Instance.OrderCheckoutPaidPanelClose = ReInitPanels;
                else if (Source == 2 && null == NavigationPath.Instance.TakeoutCheckoutPaidPanelClose)
                    NavigationPath.Instance.TakeoutCheckoutPaidPanelClose = ReInitPanels;
                else if (Source == 3 && null == NavigationPath.Instance.ImportCheckoutPaidPanelClose)
                    NavigationPath.Instance.ImportCheckoutPaidPanelClose = ReInitPanels;
            }
            

            if (this.AddMemberView.IsShow)
                this.AddMemberView.IsShow = false;

            if (IsMember)
                MemberName = Resources.Instance.GetString("MemberName");
            else
                MemberName = Resources.Instance.GetString("SupplierName");

            if (null == this.Remark)
                Remark = "";

            ClearList();

            DisplayRemark = Remark;

            ReloadBalanceList();

            Calc();


            // 把滚动框对齐到第一
            ScrollView sv = spBalanceList.Parent as ScrollView;
            if (null != sv)
                sv.ScrollToAsync(0, 0, false);

            IsShow = true;
        }


        /// <summary>
        /// 重置一下弹出面板
        /// </summary>
        private void ReInitPanels()
        {
            if (IsShow)
                IsShow = false;
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
                if (!IsCheckout && _isShow == true)
                        NavigationPath.Instance.OpenPanel();
                else if (IsCheckout &&  _isShow == true)
                    NavigationPath.Instance.OpenCheckoutPaidPanel();
                
                OnPropertyChanged("IsShow");
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
        /// 刷新支付列表
        /// </summary>
        private void ReloadBalanceList()
        {
            BalanceItemModel currentModel = CurrentBalanceList.FirstOrDefault(x => x.UseState);
            int SelectedIndex = CurrentBalanceList.IndexOf(currentModel);

            ClearList();

            if (Resources.Instance.Balances.Count > 0)
            {
                long IncomeOrExpenditure = 0;
                if (IsMember)
                    IncomeOrExpenditure = 2;
                else
                    IncomeOrExpenditure = 3;

                List<Balance> balance = Resources.Instance.Balances.Where(x => x.HideType == 0 || x.HideType == IncomeOrExpenditure).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).ToList();


                foreach (var item in balance)
                {
                    string BalanceName = "";


                    BalanceName += Resources.Instance.PrintInfo.PriceSymbol + PayModel.Where(x => x.BalanceId == item.BalanceId).Sum(x => x.OriginalPrice) + "";


                    BalanceName += "  ";
                    if (Res.Instance.MainLangIndex == 0)
                        BalanceName += item.BalanceName0;
                    else if (Res.Instance.MainLangIndex == 1)
                        BalanceName += item.BalanceName1;
                    else if (Res.Instance.MainLangIndex == 2)
                        BalanceName += item.BalanceName2;

                    AddList(new BalanceItemModel() { Text = BalanceName, Balance = item, IsBalance = true, IsChange = true, SelectCommand = SelectBalance });
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



                if (SelectedIndex == -1 || SelectedIndex >= CurrentBalanceList.Count)
                {
                    currentModel = CurrentBalanceList.FirstOrDefault();
                    if (null != currentModel)
                    {
                        currentModel.UseState = true;
                        CurrentBalance = currentModel;
                    }
                }
                else
                {
                    CurrentBalanceList[SelectedIndex].UseState = true;
                    CurrentBalance = currentModel;
                }

                CurrentSelectedItemChange();
            }
        }




        internal void ClearList()
        {
            lock (tempBalanceViewList)
            {
                foreach (Xamarin.Forms.TemplatedView item in this.spBalanceList.Children.Reverse())
                {
                    item.BindingContext = null;
                    item.IsVisible = false;

                    if (!tempBalanceViewList.Contains(item))
                        tempBalanceViewList.Add(item);
                }

                CurrentBalanceList.Clear();
            }
        }

        internal void AddList(BalanceItemModel model)
        {
            lock (tempBalanceViewList)
            {

                AddBalanceItem(model);
                CurrentBalanceList.Add(model);
            }
        }

      

        /// <summary>
        /// 添加卡片阅读器
        /// </summary>
        /// <param name="item"></param>
        private void AddBalanceItem(BalanceItemModel item)
        {
            Xamarin.Forms.TemplatedView view = null;

            if (tempBalanceViewList.Count > 0)
            {
                view = spBalanceList.Children.FirstOrDefault(x => !x.IsVisible) as Xamarin.Forms.TemplatedView;

                // 确保不报错后可以去掉
                if (view == null)
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, Res.Instance.GetString("ErrorBig"), "view null exception!(manual)", MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }
                else
                {
                    tempBalanceViewList.Remove(view);
                    view.IsVisible = true;
                    view.BindingContext = item;
                }

            }
            else
            {
                view = new Xamarin.Forms.TemplatedView();
                view.ControlTemplate = BalanceControlTemplate;

                view.BindingContext = item;
                spBalanceList.Children.Add(view);
            }
        }




        private bool FindItemAndAddPrice(Action _action, bool _isIgnore = false)
        {
            double changePriceDouble = double.Parse(ChangePrice);
            // 去掉的钱不能超出总支付金额
            if (Mode == 2)
            {

                if (!Common.Instance.IsReturnMoney() && changePriceDouble > Math.Round(PayModel.Sum(x => x.OriginalPrice), 2))
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("CanNotExceed"), Math.Round(PayModel.Sum(x => x.OriginalPrice), 2)), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    return false;
                }
            }

            if (0 != changePriceDouble)
            {
                BalanceItemModel item = CurrentBalanceList.FirstOrDefault(x => x.UseState);
                if (null == item)
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), Resources.Instance.GetString("Save")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    return false;
                }

                if (!item.IsChange)
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), Resources.Instance.GetString("Save")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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
                                if (item.Member.IsAllowBorrow == 1 || Resources.Instance.AdminModel.Mode == 2)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmItemBalanceNotEnough"), Resources.Instance.GetString("Member")), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                                    {
                                        if (msg == "NO")
                                            return;
                                        else
                                            FindItemAndAddPrice(_action, true);

                                    }, null);
                                }
                                else
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("MemberBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                    return false;
                                }
                               
                            }


                        }

                        originalPrice = PayModel.Where(x => x.MemberId == item.Member.MemberId).Sum(x => x.OriginalPrice);
                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.Instance.IsReturnMoney() && Mode == 2 && originalPrice + (-changePriceDouble) < 0)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("MemberBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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
                                if (item.Supplier.IsAllowBorrow == 1 || Resources.Instance.AdminModel.Mode == 2)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmItemBalanceNotEnough"), Resources.Instance.GetString("Supplier")), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                                    {
                                        if (msg == "NO")
                                            return;
                                        else
                                            FindItemAndAddPrice(_action, true);

                                    }, null);
                                }
                                else
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SupplierBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                    return false;
                                }
                            }
                        }

                        originalPrice = PayModel.Where(x => x.SupplierId == item.Supplier.SupplierId).Sum(x => x.OriginalPrice);
                        // 去掉的钱不能多于该会员为这个订单总共消耗的金额
                        if (!Common.Instance.IsReturnMoney() && Mode == 2 && originalPrice + (-changePriceDouble) < 0)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SupplierBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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
                    BalanceName += Resources.Instance.PrintInfo.PriceSymbol + (item.Member.BalancePrice) + "";



                BalanceName += "(" + Resources.Instance.PrintInfo.PriceSymbol + PayModel.Where(x => x.MemberId == item.MemberId && x.ParentId == item.ParentId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.Instance.GetString("Member");

                AddList(new BalanceItemModel() { Text = BalanceName, Member = item.Member, IsBalance = false, IsChange = item.IsChange, SelectCommand = SelectBalance });
            }
            else if (!IsMember && null != item.SupplierId)
            {
                if (null != item.Supplier && item.IsChange)
                    BalanceName += Resources.Instance.PrintInfo.PriceSymbol + (item.Supplier.BalancePrice) + "";

                BalanceName += "(" + Resources.Instance.PrintInfo.PriceSymbol + PayModel.Where(x => x.SupplierId == item.SupplierId && x.ParentId == item.ParentId).Sum(x => x.OriginalPrice) + ")";

                BalanceName += "  " + Resources.Instance.GetString("Member");

                AddList(new BalanceItemModel() { Text = BalanceName, Supplier = item.Supplier, IsBalance = false, IsChange = item.IsChange, SelectCommand = SelectBalance });
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

            // 防止初始化的时候触发IsToggle导致重新计算时出现Null Reference异常
            if (null != PayModel)
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
        /// 现金
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
        public Command SelectBalance
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


                        foreach (var item in CurrentBalanceList)
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
                        if (Res.Instance.MainLangIndex == 0)
                            MemberNameValue = item.Member.MemberName0;
                        else if (Res.Instance.MainLangIndex == 1)
                            MemberNameValue = item.Member.MemberName1;
                        else if (Res.Instance.MainLangIndex == 2)
                            MemberNameValue = item.Member.MemberName2;
                    }
                    else
                    {
                        if (Res.Instance.MainLangIndex == 0)
                            MemberNameValue = item.Supplier.SupplierName0;
                        else if (Res.Instance.MainLangIndex == 1)
                            MemberNameValue = item.Supplier.SupplierName1;
                        else if (Res.Instance.MainLangIndex == 2)
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

            if (IsMember && !Common.Instance.IsBindMemberByNo())
            {
                IsMemberAddShow = false;
            }
            else if (!IsMember && !Common.Instance.IsBindSupplierByNo())
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
        public Command AddMemberCommand
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

            ScrollView sv = spBalanceList.Parent as ScrollView;
            if (null != sv)
                sv.ScrollToAsync(9999, 0, false);
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

            


            SelectBalance.Execute(CurrentBalanceList.LastOrDefault());


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
        public Command RemoveMemberCommand
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

                        CurrentBalanceList.Remove(model);

                        IsAddedMemberOrSupplier = false;
                        ReloadBalanceList();
                        Calc();

                        if (null != Recalc)
                            Recalc();


                        ScrollView sv = spBalanceList.Parent as ScrollView;
                        if (null != sv)
                            sv.ScrollToAsync(0, 0, false);

                    });
                }
                return _removeMemberCommand;
            }
        }

        

        
        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");
        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public Command OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(param =>
                    {

                        FindItemAndAddPrice(() =>
                        {
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {
                                this.IsLoading = true;
                                Task.Run(async () =>
                                {

                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {

                                        if (string.IsNullOrWhiteSpace(this.DisplayRemark))
                                            this.DisplayRemark = null;

                                        this.Remark = this.DisplayRemark;

                                        if (null != Recalc)
                                            Recalc();


                                        this.Hide();

                                        this.IsLoading = false;
                                    });
                                });

                            });

                        });


                    });
                }
                return _okCommand;
            }
        }



        private bool _isLoading = false;
        /// <summary>
        /// 是否显示
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
        /// 增加按钮
        /// </summary>
        private RelayCommand _addCommand;
        public Command AddCommand
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

        private void Hide()
        {
            IsLoading = true;

            Task.Run(async () =>
            {

                await ExtX.WaitForLoading();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    this.IsShow = false;

                    if (IsCheckout)
                        NavigationPath.Instance.ClosePanels(false);
                    else
                        NavigationPath.Instance.CloseCheckoutPanels(false);

                    IsLoading = false;

                });
            });
        }




        /// <summary>
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public Command CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param =>
                {
                    this.Hide();
                }));
            }
        }












        /// <summary>
        /// 增加支付金额
        /// </summary>
        internal void AddMemberPaidPrice()
        {
            if (IsMember && CurrentBalanceList.Count > 0 && null != CurrentBalance && CurrentBalance.IsBalance && null != CurrentBalance.Balance)
            {
                BalanceItemModel model = CurrentBalanceList.LastOrDefault();


                double changePrice = double.Parse(this.ChangePrice);

                if (changePrice != 0 && null != model && null != model.Member && model.IsChange)
                {
                    string memberName = "";
                    string balanceName = "";


                    if (Res.Instance.MainLangIndex == 0)
                        memberName = model.Member.MemberName0;
                    else if (Res.Instance.MainLangIndex == 1)
                        memberName = model.Member.MemberName1;
                    else if (Res.Instance.MainLangIndex == 2)
                        memberName = model.Member.MemberName2;


                    if (Res.Instance.MainLangIndex == 0)
                        balanceName = CurrentBalance.Balance.BalanceName0;
                    else if (Res.Instance.MainLangIndex == 1)
                        balanceName = CurrentBalance.Balance.BalanceName1;
                    else if (Res.Instance.MainLangIndex == 2)
                        balanceName = CurrentBalance.Balance.BalanceName2;


                    //确认取消
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmAddPay"), Resources.Instance.PrintInfo.PriceSymbol, ChangePrice, balanceName, memberName), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                    {
                   
                        if (msg == "NO")
                            return;



                        MemberPay memberpay = new MemberPay();


                        memberpay.Price = changePrice;
                        memberpay.BalanceId = CurrentBalance.Balance.BalanceId;
                        memberpay.MemberId = model.Member.MemberId;


                        // 开始支付
                        this.IsLoading = true;

                        Task.Factory.StartNew(async () =>
                        {
                            ResultModel result = new ResultModel();
                            double originalBalancePrice = model.Member.BalancePrice;
                            try
                            {
                                // 更新会员信息
                                model.Member.BalancePrice = model.Member.BalancePrice + memberpay.Price;

 
                                var taskResult = await OperatesService.Instance.ServiceAddMemberPay(model.Member, memberpay);
                                result = taskResult.resultModel;
                                Member newMember = taskResult.newMember;
                                MemberPay newMemberPay = taskResult.newMemberPay;

                                Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                {
                                    if (result.Result)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, Resources.Instance.GetString("SaveSuccess"), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, (msg2) =>
                                        {
                                            // 去掉当前最后一个会员并重新添加
                                            RemoveMemberCommand.Execute(null);
                                            AddMember(newMember, null);

                                        }, null);
                                    }
                                    else
                                    {
                                        if (result.IsDataHasRefrence)
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PropertyUsed"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

                                        }
                                        else if (result.UpdateModel)
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PropertyUnSame"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

                                        }
                                        else
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SaveFailt"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

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
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, (msg2) =>
                                        {
                                            // 失败了就复原会员信息
                                            if (result.Result)
                                                model.Member.BalancePrice = originalBalancePrice;


                                        }, null);
                                    }), false, Resources.Instance.GetString("SaveFailt"));
                                }));
                            }

                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                            {
                                IsLoading = false;
                            }));

                        });


                    }, null);



                }
                else if (changePrice != 0 && null != model && null != model.Supplier && model.IsChange)
                {
                    string supplierName = "";
                    string balanceName = "";


                    if (Res.Instance.MainLangIndex == 0)
                        supplierName = model.Supplier.SupplierName0;
                    else if (Res.Instance.MainLangIndex == 1)
                        supplierName = model.Supplier.SupplierName1;
                    else if (Res.Instance.MainLangIndex == 2)
                        supplierName = model.Supplier.SupplierName2;


                    if (Res.Instance.MainLangIndex == 0)
                        balanceName = CurrentBalance.Balance.BalanceName0;
                    else if (Res.Instance.MainLangIndex == 1)
                        balanceName = CurrentBalance.Balance.BalanceName1;
                    else if (Res.Instance.MainLangIndex == 2)
                        balanceName = CurrentBalance.Balance.BalanceName2;




                    //确认取消
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmAddPay"), Resources.Instance.PrintInfo.PriceSymbol, ChangePrice, balanceName, supplierName), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                    {

                        if (msg == "NO")
                            return;


                        SupplierPay supplierpay = new SupplierPay();


                        supplierpay.Price = changePrice;
                        supplierpay.BalanceId = CurrentBalance.Balance.BalanceId;
                        supplierpay.SupplierId = model.Supplier.SupplierId;


                        // 开始支付
                        this.IsLoading = true;

                        Task.Factory.StartNew(async () =>
                        {
                            ResultModel result = new ResultModel();
                            double originalBalancePrice = model.Supplier.BalancePrice;
                            try
                            {
                                // 更新供应者信息
                                model.Supplier.BalancePrice = model.Supplier.BalancePrice + supplierpay.Price;


                                var taskResult = await OperatesService.Instance.ServiceAddSupplierPay(model.Supplier, supplierpay);
                                result = taskResult.resultModel;
                                Supplier newSupplier = taskResult.newSupplier;
                                SupplierPay newSupplierPay = taskResult.newSupplierPay;

                                Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                {
                                    if (result.Result)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, Resources.Instance.GetString("SaveSuccess"), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, (msg2) =>
                                        {
                                            // 去掉当前最后一个会员并重新添加
                                            RemoveMemberCommand.Execute(null);
                                            AddMember(null, newSupplier);

                                        }, null);
                                    }
                                    else
                                    {
                                        if (result.IsDataHasRefrence)
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PropertyUsed"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

                                        }
                                        else if (result.UpdateModel)
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PropertyUnSame"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

                                        }
                                        else
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SaveFailt"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

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
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, (msg2) =>
                                        {
                                            // 失败了就复原会员信息
                                            if (result.Result)
                                                model.Member.BalancePrice = originalBalancePrice;


                                        }, null);
                                    }), false, Resources.Instance.GetString("SaveFailt"));
                                }));
                            }
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                            {
                                IsLoading = false;
                            }));

                        });

                    }, null);
                }

            }
        }



      



        /// <summary>
        /// 快速整平余额
        /// </summary>
        private RelayCommand _finishPriceCommand;
        public Command FinishPriceCommand
        {
            get
            {
                if (_finishPriceCommand == null)
                {
                    _finishPriceCommand = new RelayCommand(param =>
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
                    });
                }
                return _finishPriceCommand;
            }
        }




    }
}
