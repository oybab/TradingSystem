using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Oybab.TradingSystemX.VM.Converters;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.VM.ViewModels.Navigations;
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.DAL;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class BalanceManagerViewModel : ViewModelBase
    {
        private Page _element;
        private List<Balance> BalanceList = new List<Balance>();



        public BalanceManagerViewModel(Page _element)
        {
            this._element = _element;


            RefreshBalanceLists();

        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(bool RefreshAll = true)
        {

            ChangePrice = "0";
            Remark = "";


            RefreshBalanceLists();

            if (RefreshAll)
            {
                BalanceMode = false;
                Mode = 1;



                if (OldBalanceList.Count > 0)
                    SelectedOldBalance = OldBalanceList.FirstOrDefault();

                if (NewBalanceList.Count > 0)
                    SelectedNewBalance = NewBalanceList.FirstOrDefault();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <returns></returns>
        internal async Task Refresh()
        {
            try
            {
                var taskResult = await OperatesService.Instance.ServiceGetBalances(0);

                if (taskResult.result)
                    this.BalanceList = taskResult.balances;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);

                }));
            }
        }


        private void RefreshBalanceLists()
        {

            long oldBalanceId = -1;
            long newBalanceId = -1;


            if (null != SelectedOldBalance)
                oldBalanceId = (SelectedOldBalance.Value as Balance).BalanceId;

            if (null != SelectedNewBalance)
                newBalanceId = (SelectedNewBalance.Value as Balance).BalanceId;

            OldBalanceList.Clear();
            NewBalanceList.Clear();


           

            if (Res.Instance.MainLangIndex == 0)
            {
                foreach (var item in BalanceList.OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId))
                {
                    OldBalanceList.Add(new Dict() { Name = item.BalanceName0 + "  " + Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice, Value = item });
                    NewBalanceList.Add(new Dict() { Name = item.BalanceName0 + "  " + Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice, Value = item });
                }
            }
            else if (Res.Instance.MainLangIndex == 1)
            {
                foreach (var item in BalanceList.OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId))
                {
                    OldBalanceList.Add(new Dict() { Name = item.BalanceName1 + "  " + Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice, Value = item });
                    NewBalanceList.Add(new Dict() { Name = item.BalanceName1 + "  " + Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice, Value = item });
                }
            }
            else if (Res.Instance.MainLangIndex == 2)
            {
                foreach (var item in BalanceList.OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId))
                {
                    OldBalanceList.Add(new Dict() { Name = item.BalanceName2 + "  " + Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice, Value = item });
                    NewBalanceList.Add(new Dict() { Name = item.BalanceName2 + "  " + Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice, Value = item });
                }
            }

            if (oldBalanceId != -1)
                SelectedOldBalance = OldBalanceList.FirstOrDefault(x => (x.Value as Balance).BalanceId == oldBalanceId);
            if (newBalanceId != -1)
                SelectedNewBalance = NewBalanceList.FirstOrDefault(x => (x.Value as Balance).BalanceId == newBalanceId);


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


        }




        private bool _balanceMode = false;
        /// <summary>
        /// 余额模式True余额支付False转账
        /// </summary>
        public bool BalanceMode
        {
            get { return _balanceMode; }
            set
            {
                _balanceMode = value;
                OnPropertyChanged("BalanceMode");
            }
        }


        private int _mode = 1;
        /// <summary>
        /// 1+, 2-
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


        private string _remark = "";
        /// <summary>
        /// 1备注
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


        private ObservableCollection<Dict> _oldBalanceList = new ObservableCollection<Dict>();
        /// <summary>
        /// 老余额名
        /// </summary>
        public ObservableCollection<Dict> OldBalanceList
        {
            get { return _oldBalanceList; }
            set
            {
                _oldBalanceList = value;
                OnPropertyChanged("OldBalanceList");
            }
        }


        private Dict _selectedOldBalance = null;
        /// <summary>
        /// 选中的老余额
        /// </summary>
        public Dict SelectedOldBalance
        {
            get { return _selectedOldBalance; }
            set
            {
                _selectedOldBalance = value;
                OnPropertyChanged("SelectedOldBalance");
            }
        }



        private ObservableCollection<Dict> _newBalanceList = new ObservableCollection<Dict>();
        /// <summary>
        /// 新余额名
        /// </summary>
        public ObservableCollection<Dict> NewBalanceList
        {
            get { return _newBalanceList; }
            set
            {
                _newBalanceList = value;
                OnPropertyChanged("NewBalanceList");
            }
        }


        private Dict _selectedNewBalance = null;
        /// <summary>
        /// 选中的新余额
        /// </summary>
        public Dict SelectedNewBalance
        {
            get { return _selectedNewBalance; }
            set
            {
                _selectedNewBalance = value;
                OnPropertyChanged("SelectedNewBalance");
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
                        Balance _oldBalance = (SelectedOldBalance.Value as Balance);

                        if (_oldBalance.BalancePrice != 0)
                        {
                            ChangePrice = Math.Abs(_oldBalance.BalancePrice).ToString();
                            if (_oldBalance.BalancePrice < 0)
                                Mode = 1;
                            else if (_oldBalance.BalancePrice > 0)
                                Mode = 2;

                        }


                    });
                }
                return _finishPriceCommand;
            }
        }




        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _changeCommand;
        public Command ChangeCommand
        {
            get
            {
                return _changeCommand ?? (_changeCommand = new RelayCommand(param =>
                {
                    
                        double price = double.Parse(ChangePrice.ToString());
                    long selectedNewBalanceId = (SelectedNewBalance.Value as Balance).BalanceId;
                    long selectedOldBalanceId = (SelectedOldBalance.Value as Balance).BalanceId;




                    // 如果余额支付, 价格不能为0, 如果转账则价格不能为0. 转账的余额不能一样!
                    if ((BalanceMode == false && price == 0) || (BalanceMode == true && (price == 0 || selectedNewBalanceId == selectedOldBalanceId)))
                    {
                        return;
                    }

                    if (BalanceMode)
                    {
                        TransferBalance(price);
                    }
                    else
                    {
                        AddBalancePay(price);
                    }

                       

                }));
            }
        }


        /// <summary>
        /// 余额支付
        /// </summary>
        private void AddBalancePay(double price)
        {
            //确认
            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmOperate"), Resources.Instance.GetString("Change")), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, async (string msg) =>
            {

                if (msg == "NO")
                    return;
                BalancePay balancePay = new BalancePay();
                balancePay.Price = price;

                balancePay.Price = price;
                if (Mode == 2)
                    balancePay.Price = -price;

                if (!string.IsNullOrWhiteSpace(Remark))
                    balancePay.Remark = Remark;

                balancePay.BalanceId = (SelectedOldBalance.Value as Balance).BalanceId;



                IsLoading = true;


                try
                {
                    var taskResult = await OperatesService.Instance.ServiceAddBalancePay(balancePay);
                    ResultModel result = taskResult.resultModel;
                    BalancePay newBalancePay = taskResult.newBalancePay;
                    Balance balance = taskResult.newBalance;

                    if (result.Result)
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), Resources.Instance.GetString("Change")), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, msg2 =>
                        {
                            Balance oldBalance = BalanceList.FirstOrDefault(x => x.BalanceId == balance.BalanceId);
                            oldBalance.BalancePrice = balance.BalancePrice;

                            Init(false);
                        }, null);


                    }
                    else
                    {
                        if (result.IsDataHasRefrence)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("PropertyUsed"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        else if (result.UpdateModel)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("PropertyUnSame"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        else
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("SaveFailt"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                    }
                }
                catch (Exception ex)
                {

                    ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                    }));

                }


                IsLoading = false;


            }, null);
        }


        /// <summary>
        /// 转账
        /// </summary>
        private void TransferBalance(double price)
        {

            //确认
            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmOperate"), Resources.Instance.GetString("Change")), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, async (string msg) =>
            {

                if (msg == "NO")
                    return;

                BalancePay balancePay1 = new BalancePay();
                balancePay1.BalanceId = (SelectedNewBalance.Value as Balance).BalanceId;
                balancePay1.Price = price;
                if (!string.IsNullOrWhiteSpace(Remark))
                    balancePay1.Remark = Remark;

                BalancePay balancePay2 = new BalancePay();
                balancePay2.BalanceId = (SelectedOldBalance.Value as Balance).BalanceId;
                balancePay2.Price = -price;
                if (!string.IsNullOrWhiteSpace(Remark))
                    balancePay2.Remark = Remark;


                IsLoading = true;



                try
                {

                    var taskResult = await OperatesService.Instance.ServiceTransferBalancePay(balancePay1, balancePay2);
                    ResultModel result = taskResult.resultModel;
                    BalancePay newBalancePay1 = taskResult.newBalancePay1;
                    BalancePay newBalancePay2 = taskResult.newBalancePay2;
                    Balance newBalance1 = taskResult.newBalance1;
                    Balance newBalance2 = taskResult.newBalance2;

                    if (result.Result)
                    {

                        await Refresh();

                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), Resources.Instance.GetString("Change")), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, msg2 =>
                        {
                            Init(false);
                        }, null);

                    }
                    else
                    {
                        if (result.IsDataHasRefrence)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("PropertyUsed"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        else if (result.UpdateModel)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("PropertyUnSame"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        else
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("SaveFailt"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                    }
                }
                catch (Exception ex)
                {

                    ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                    }));

                }


                IsLoading = false;

            }, null);


        }





        /// <summary>
        /// 返回
        /// </summary>
        private RelayCommand _backCommand;
        public Command BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.GoNavigateBack();
                }));
            }
        }



    }
}
