using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class SelectedViewModel : ViewModelBase
    {
        private UIElement _element;
        private ListBox _lbList;
        internal long RoomId = -1;
        internal long StartTimeTemp = 0;
        internal long EndTimeTemp = 0;
        internal Action Save;
        internal Action Checkout;
        internal Action RefreshTime;

        internal SelectedViewModel(UIElement element, ListBox lbList)
        {
            this._element = element;
            this._lbList = lbList;
        }



        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
           
        }


        private ObservableCollection<DetailsModel> _currentSelectedList = new ObservableCollection<DetailsModel>();
        /// <summary>
        /// 当前已选列表
        /// </summary>
        public ObservableCollection<DetailsModel> CurrentSelectedList
        {
            get { return _currentSelectedList; }
            set
            {
                _currentSelectedList = value;
                OnPropertyChanged("CurrentSelectedList");
            }
        }

        
         private bool _isImport = false;
        /// <summary>
        /// 是否支出
        /// </summary>
        public bool IsImport
        {
            get { return _isImport; }
            set
            {
                _isImport = value;
                OnPropertyChanged("IsImport");
            }
        }


        private string _roomNo = "";
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


        private double _totalPrice = 0;
        /// <summary>
        /// 总额
        /// </summary>
        public double TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
        }



        private double _paidPrice = 0;
        /// <summary>
        /// 已付金额
        /// </summary>
        public double PaidPrice
        {
            get { return _paidPrice; }
            set
            {
                _paidPrice = value;
                OnPropertyChanged("PaidPrice");
            }
        }


        private int _saveMode = 0;
        /// <summary>
        /// 保存类型(0不显示1保存2结账99更新)
        /// </summary>
        public int SaveMode
        {
            get
            {
                if (IsRefresh)
                    return 99;
                else
                    return _saveMode;
            }
            set
            {
                _saveMode = value;
                OnPropertyChanged("SaveMode");
            }
        }


        private bool _isRefresh = false;
        /// <summary>
        /// 更新
        /// </summary>
        public bool IsRefresh
        {
            get { return _isRefresh; }
            set
            {
                _isRefresh = value;
                OnPropertyChanged("IsRefresh");
                OnPropertyChanged("SaveMode");
            }
        }


        private int _hasAddress = 0;
        /// <summary>
        /// 是否有地址(0不显示1显示2选中)
        /// </summary>
        public int HasAddress
        {
            get { return _hasAddress; }
            set
            {
                _hasAddress = value;
                OnPropertyChanged("HasAddress");
            }
        }



        private bool _roomDisplay = false;
        /// <summary>
        /// 是否显示雅座
        /// </summary>
        public bool RoomDisplay
        {
            get { return _roomDisplay; }
            set
            {
                _roomDisplay = value;
                OnPropertyChanged("RoomDisplay");
            }
        }

        private bool _roomTimeChange = false;
        /// <summary>
        /// 雅座时间是否变更了
        /// </summary>
        public bool RoomTimeChange
        {
            get { return _roomTimeChange; }
            set
            {
                _roomTimeChange = value;
                OnPropertyChanged("RoomTimeChange");
            }
        }


        private bool _roomPaidPriceChange = false;
        /// <summary>
        /// 支付价格更改
        /// </summary>
        public bool RoomPaidPriceChanged
        {
            get { return _roomPaidPriceChange; }
            set
            {
                _roomPaidPriceChange = value;
                OnPropertyChanged("RoomPaidPriceChanged");
            }
        }

        private bool _remarkChange = false;
        /// <summary>
        /// 备注更改
        /// </summary>
        public bool RemarkChanged
        {
            get { return _remarkChange; }
            set
            {
                _remarkChange = value;
                OnPropertyChanged("RemarkChanged");
            }
        }



        private int _roomType = 0;
        /// <summary>
        /// 雅座类型(是否按时间收费)
        /// </summary>
        public int RoomType
        {
            get { return _roomType; }
            set
            {
                _roomType = value;
                OnPropertyChanged("RoomType");
            }
        }


        /// <summary>
        /// 是否允许支付
        /// </summary>
        public bool AllowPaid
        {
            get { return Common.GetCommon().IsIncomeTradingManage(); }
            set
            {
                OnPropertyChanged("AllowPaid");
            }
        }


        private double _roomPrice = 0;
        /// <summary>
        /// 雅座价格
        /// </summary>
        public double RoomPrice
        {
            get { return _roomPrice; }
            set
            {
                _roomPrice = value;
                OnPropertyChanged("RoomPrice");
            }
        }



        private string _roomTime = "";
        /// <summary>
        /// 雅座时间
        /// </summary>
        public string RoomTime
        {
            get { return _roomTime; }
            set
            {
                _roomTime = value;
                OnPropertyChanged("RoomTime");
            }
        }


        
        /// <summary>
        /// 更改时间
        /// </summary>
        private RelayCommand _changeTime;
        public ICommand ChangeTime
        {
            get
            {
                if (_changeTime == null)
                {
                    _changeTime = new RelayCommand(param =>
                    {
                        SelectedViewModel model = param as SelectedViewModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }
                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.ChangeTime, model));
                    });
                }
                return _changeTime;
            }
        }





        /// <summary>
        /// 修改支付金额
        /// </summary>
        private RelayCommand _changePaidPrice;
        public ICommand ChangePaidPrice
        {
            get
            {
                if (_changePaidPrice == null)
                {
                    _changePaidPrice = new RelayCommand(param =>
                    {
                        SelectedViewModel model = param as SelectedViewModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }
                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.ChangePaidPrice, model));
                    });
                }
                return _changePaidPrice;
            }
        }




        

        /// <summary>
        /// 地址
        /// </summary>
        private RelayCommand _addressCommand;
        public ICommand AddressCommand
        {
            get
            {
                if (_addressCommand == null)
                {
                    _addressCommand = new RelayCommand(param =>
                    {
                        SelectedViewModel model = param as SelectedViewModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.Address, model));
                    });
                }
                return _addressCommand;
            }
        }


        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param =>
                    {
                        Save();
                    });
                }
                return _saveCommand;
            }
        }




        /// <summary>
        /// 结账
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
                        Checkout();
                    });
                }
                return _checkoutCommand;
            }
        }



        public ICommand RefreshCommand
        {
            get;set;
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


        private bool _tempUnlimitedTime = false;
        /// <summary>
        /// 是否无限时间
        /// </summary>
        public bool TempUnlimitedTime
        {
            get { return _tempUnlimitedTime; }
            set
            {
                _tempUnlimitedTime = value;
                OnPropertyChanged("TempUnlimitedTime");
            }
        }

    }
}
