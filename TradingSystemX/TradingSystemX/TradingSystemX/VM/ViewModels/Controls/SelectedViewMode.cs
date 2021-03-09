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
using System.Linq;
using Oybab.TradingSystemX.VM.Converters;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.Pages;
using Oybab.DAL;
using Oybab.Res.View.Models;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages.Controls
{
    internal sealed class SelectedViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;
        internal long RoomId = -1;
        internal long StartTimeTemp = 0;
        internal long EndTimeTemp = 0;
        internal Action Save;
        internal Action Checkout;
        internal Action RefreshTime;






        private Xamarin.Forms.StackLayout _spSelectedList;
        private Xamarin.Forms.ControlTemplate _ctSelectedControlTemplate;

        public SelectedViewModel(Xamarin.Forms.Page _element, Action Recalc, Action RecalcPaidPrice, Xamarin.Forms.StackLayout spSelectedList, Xamarin.Forms.ControlTemplate ctSelectedControlTemplate, Xamarin.Forms.StackLayout spRequestList, Xamarin.Forms.ControlTemplate ctRequestControlTemplate, Xamarin.Forms.StackLayout spBalanceList, Xamarin.Forms.ControlTemplate ctBalanceControlTemplate)
        {
            this._element = _element;


            this._spSelectedList = spSelectedList;
            this._ctSelectedControlTemplate = ctSelectedControlTemplate;

            // 列表变化时重置一下编号
            CurrentSelectedList.CollectionChanged += (sender, args) =>
            {
                ResetNo();
            };
            RequestView = new RequestViewModel(null, this, spRequestList, ctRequestControlTemplate);
            ChangeCountView = new ChangeCountViewModel(null, this, Recalc);
            ChangePriceView = new ChangePriceViewModel(null, this, Recalc);


            ChangePaidPriceView = new ChangePaidPriceViewModel(null, RecalcPaidPrice, spBalanceList, ctBalanceControlTemplate);
            ChangeTimeView = new ChangeTimeViewModel(null, this, Recalc);



            foreach (var item in Res.Instance.AllLangList.OrderBy(x => x.Value.LangOrder))
            {
                Dict dict = new Dict() { Name = Res.Instance.GetString("LangName", item.Value.Culture), Value = item.Value.LangIndex };
                AllLang.Add(dict);
            }

        }

        /// <summary>
        /// 重新加载页
        /// </summary>
        /// <param name="_element"></param>
        internal void ReloadSelectedViewModel(Xamarin.Forms.Page _element)
        {
            this._element = _element;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            //SelectedLang = AllLang.Where(x => int.Parse(x.Value.ToString()) == Res.Instance.CurrentLangIndex).FirstOrDefault();
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


        internal void ClearList()
        {
            lock (tempTemplateViewList)
            {
                foreach (Xamarin.Forms.TemplatedView item in this._spSelectedList.Children)
                {
                    item.BindingContext = null;
                    item.IsVisible = false;

                    if (!tempTemplateViewList.Contains(item))
                        tempTemplateViewList.Add(item);
                }

                CurrentSelectedList.Clear();
            }
        }

        internal void AddList(DetailsModel model)
        {
            lock (tempTemplateViewList)
            {

                AddSelectedItem(model, false);
                CurrentSelectedList.Add(model);
            }

        }

        internal void AddListToFirst(DetailsModel model)
        {

            lock (tempTemplateViewList)
            {
                AddSelectedItem(model, true);
                CurrentSelectedList.Insert(0, model);
            }

        }

        private List<Xamarin.Forms.TemplatedView> tempTemplateViewList = new List<Xamarin.Forms.TemplatedView>();

        /// <summary>
        /// 添加已选对象
        /// </summary>
        /// <param name="item"></param>
        private void AddSelectedItem(DetailsModel item, bool IsFirst)
        {
            Xamarin.Forms.TemplatedView view = null;

            
            if (tempTemplateViewList.Count > 0)
            {
                if (IsFirst)
                    view = _spSelectedList.Children.Where(x => !x.IsVisible).Skip(tempTemplateViewList.Count - 1).LastOrDefault() as Xamarin.Forms.TemplatedView;
                else
                    view = _spSelectedList.Children.LastOrDefault(x => !x.IsVisible) as Xamarin.Forms.TemplatedView;

                // 确保不报错后可以去掉
                if (view == null)
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, Res.Instance.GetString("ErrorBig"), "view null exception!(manual)", MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }
                else
                {
                    tempTemplateViewList.Remove(view);
                    view.IsVisible = true;
                    view.BindingContext = item;
                }

            }
            else
            {
                view = new Xamarin.Forms.TemplatedView();
                view.ControlTemplate = _ctSelectedControlTemplate;

                view.BindingContext = item;
                //if (IsFirst)
                    _spSelectedList.Children.Insert(0, view);
                //else
                //    _spSelectedList.Children.Add(view);

            }
            


        }

        internal void RemoveSelected(DetailsModel item)
        {
            lock (tempTemplateViewList)
            {
                if (item == null)
                    return;

                Xamarin.Forms.TemplatedView _view = null;
                foreach (Xamarin.Forms.TemplatedView items in this._spSelectedList.Children)
                {
                    if (items.BindingContext == item)
                    {
                        _view = items;
                        break;
                    }
                }

                if (null != _view)
                {
                    _view.BindingContext = null;
                    if (!tempTemplateViewList.Contains(_view))
                        tempTemplateViewList.Add(_view);
                    _view.IsVisible = false;

                }


                CurrentSelectedList.Remove(item);
            }
        }


        internal void ReloadList()
        {
            List<DetailsModel> items = CurrentSelectedList.ToList();
            CurrentSelectedList.Clear();
            CurrentSelectedList = new ObservableCollection<DetailsModel>();


            foreach (var item in items)
            {
                DetailsModel model = new DetailsModel();
                model.OrderDetail = item.OrderDetail;
                model.Product = item.Product;
                model.AddTime = item.AddTime;
                
              
                model.IsNew = item.IsNew;

                model.IsTakeout = item.IsTakeout;
                model.NewPrice = item.NewPrice;
                model.No = item.No;
                model.Operate = item.Operate;
                
                CurrentSelectedList.Add(model);
            }


            items = null;
        }


        /// <summary>
        /// 重置编号
        /// </summary>
        private void ResetNo()
        {
            
            List<Xamarin.Forms.View> list = _spSelectedList.Children.Where(x => x.IsVisible).ToList();

            int no = 1;
            for (int i = 0; i < list.Count; i++)
            {
                (list[i].BindingContext as DetailsModel).No = no;
                ++no;
            }
        }



        private RequestViewModel _requestView;
        /// <summary>
        /// 请求
        /// </summary>
        public RequestViewModel RequestView
        {
            get { return _requestView; }
            set
            {
                _requestView = value;
                OnPropertyChanged("RequestView");
            }
        }


        private ChangeCountViewModel _changeCountView;
        /// <summary>
        /// 修改数量
        /// </summary>
        public ChangeCountViewModel ChangeCountView
        {
            get { return _changeCountView; }
            set
            {
                _changeCountView = value;
                OnPropertyChanged("ChangeCountView");
            }
        }


        private ChangePriceViewModel _changePriceView;
        /// <summary>
        /// 修改价格
        /// </summary>
        public ChangePriceViewModel ChangePriceView
        {
            get { return _changePriceView; }
            set
            {
                _changePriceView = value;
                OnPropertyChanged("ChangePriceView");
            }
        }


        private ChangePaidPriceViewModel _changePaidPriceView;
        /// <summary>
        /// 修改价格
        /// </summary>
        public ChangePaidPriceViewModel ChangePaidPriceView
        {
            get { return _changePaidPriceView; }
            set
            {
                _changePaidPriceView = value;
                OnPropertyChanged("ChangePaidPriceView");
            }
        }


        private ChangeTimeViewModel _changeTimeView;
        /// <summary>
        /// 修改时间
        /// </summary>
        public ChangeTimeViewModel ChangeTimeView
        {
            get { return _changeTimeView; }
            set
            {
                _changeTimeView = value;
                OnPropertyChanged("ChangeTimeView");
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


        private bool _isLanguageShow = false;
        /// <summary>
        /// 语言是否显示
        /// </summary>
        public bool IsLanguageShow
        {
            get { return _isLanguageShow; }
            set
            {
                _isLanguageShow = value;
                OnPropertyChanged("IsLanguageShow");
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
            get { return Common.Instance.IsIncomeTradingManage(); }
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
        public Xamarin.Forms.Command ChangeTime
        {
            get
            {
                return _changeTime ?? (_changeTime = new RelayCommand(param =>
                {
                    SelectedViewModel viewModel = param as SelectedViewModel;

                    if (null == viewModel)
                        return;

                    viewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            viewModel.ChangeTimeView.InitialView();

                            viewModel.IsLoading = false;
                        });
                    });
                }));

            }
        }

        internal List<CommonPayModel> oldList = null;
        internal List<OrderPay> tempPayList = new List<OrderPay>();
        /// <summary>
        /// 更改时间
        /// </summary>
        private RelayCommand _changePaidPrice;
        public Xamarin.Forms.Command ChangePaidPrice
        {
            get
            {
                return _changePaidPrice ?? (_changePaidPrice = new RelayCommand(param =>
                {
                    SelectedViewModel viewModel = param as SelectedViewModel;

                    if (null == viewModel)
                        return;

                    viewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                            viewModel.ChangePaidPriceView.InitialView(this.TotalPrice, oldList, true, false);

                            viewModel.IsLoading = false;
                        });
                    });
                }));

            }
        }

        private DateTime _lastSaveTime = DateTime.Now;
        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _saveCommand;
        public Xamarin.Forms.Command SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new RelayCommand(param =>
                {
                    if ((DateTime.Now - _lastSaveTime).TotalSeconds <= 2)
                        return;
                    _lastSaveTime = DateTime.Now;

                    Save();
                }));

            }
        }




        /// <summary>
        /// 结账
        /// </summary>
        private RelayCommand _checkoutCommand;
        public Xamarin.Forms.Command CheckoutCommand
        {
            get
            {
                return _checkoutCommand ?? (_checkoutCommand = new RelayCommand(param =>
                {
                    Checkout();
                }));
            }

        }


        public Xamarin.Forms.Command RefreshCommand
        {
            get; set;
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
        /// 跳转
        /// </summary>
        public Xamarin.Forms.Command GoCommand
        {
            get;
            internal set;
        }




        private ObservableCollection<Dict> _allLang = new ObservableCollection<Dict>();
        /// <summary>
        /// 所有语言
        /// </summary>
        public ObservableCollection<Dict> AllLang
        {
            get { return _allLang; }
            set
            {
                _allLang = value;
                OnPropertyChanged("AllLang");
            }
        }


        private Dict _selectedLang = null;
        /// <summary>
        /// 所选语言
        /// </summary>
        public Dict SelectedLang
        {
            get { return _selectedLang; }
            set
            {
                _selectedLang = value;
                OnPropertyChanged("SelectedLang");
            }
        }



        private int _languageMode = -1;
        /// <summary>
        /// 选择模式 0中文, 1维文, 2英文
        /// </summary>
        public int LanguageMode
        {
            get { return _languageMode; }
            set
            {
                _languageMode = value;
                OnPropertyChanged("LanguageMode");
                OnPropertyChanged("LanguageName");

                //SelectedLang = AllLang.Where(x => int.Parse(x.Value.ToString()) == Res.Instance.CurrentLangIndex).FirstOrDefault();

                if (_languageMode != Res.Instance.MainLangIndex)
                {
                    if (LanguageModeNo != 1)
                        LanguageModeNo = 1;
                }
                else
                {
                    if (LanguageModeNo != 0)
                        LanguageModeNo = 0;
                }
            }
        }


        private int _languageModeNo = 0;
        /// <summary>
        /// 选择模式 0普通1红色显示
        /// </summary>
        public int LanguageModeNo
        {
            get { return _languageModeNo; }
            set
            {
                _languageModeNo = value;
                OnPropertyChanged("LanguageModeNo");
            }
        }



        private bool _languageEnable = false;
        /// <summary>
        /// 是否允许更改语言
        /// </summary>
        public bool LanguageEnable
        {
            get { return _languageEnable; }
            set
            {
                _languageEnable = value;
                OnPropertyChanged("LanguageEnable");
            }
        }


        private string _languageName = "";
        /// <summary>
        /// 选择名
        /// </summary>
        public string LanguageName
        {
            get { return Res.Instance.GetMainLangByLangIndex(_languageMode).LangName; }
            set
            {
                _languageName = value;
                OnPropertyChanged("LanguageName");
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
