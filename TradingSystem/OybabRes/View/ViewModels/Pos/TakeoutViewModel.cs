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
using Oybab.Res.View.ViewModels.Pos.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Oybab.Res.View.ViewModels.Pos
{
    public sealed class TakeoutViewModel: ViewModelBase
    {
        private UIElement _element;
        private UIElement _selectedUI;



        public TakeoutViewModel(UIElement element, UIElement productsUI, UIElement selectedUI, UIElement checkoutUI)
        {
            this._element = element;
            this._selectedUI = selectedUI;

            this.Products = new ProductsViewModel(productsUI);
            this.Products.ProductChange = ProductChange;
            this.Products.DetectProductIsSelected = DetectProductIsSelected;
            this.Products.Operate = OperateDetails;


            this.Selected = new SelectedViewModel(selectedUI);
            this.Selected.Save = Save;
            this.Selected.Checkout = Checkout;
            this.Selected.HasAddress = 1;

            this.TakeoutCheckout = new TakeoutCheckoutViewModel(element, checkoutUI, RefreshPM, Init);


            // 添加处理事件
            this._element.AddHandler(PublicEvents.PopupEvent, new RoutedEventHandler(HandlePopop), true);

            _msg = new ViewModels.Controls.MsgViewModel(SetMsgCommand);
            _animation = new ViewModels.Controls.AnimationViewModel();

            // 语言修改相关参数
            Notification.Instance.NotificationLanguage += (obj, value, args) => { _element.Dispatcher.BeginInvoke(new Action(() => { SetCurrentName(); })); };
            SetCurrentName();
        }



        /// <summary>
        /// 扫条形码
        /// </summary>
        private void ScanBarcode(string code)
        {
            // 获取条码产品
            List<Product> ResultProducts = Resources.GetRes().Products.Where(x => (x.Barcode == code || (x.IsScales == 1 && code.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).ToList();

            if (ResultProducts.Count > 1)
            {
                ResultProducts = ResultProducts.OrderByDescending(x => x.Order).ThenBy(x => x.ProductParentCount).ToList();
                // 刷新产品
                Products.RefreshProduct(ResultProducts);
                this.DisplayMode = 1;

                return;
            }
            else if (ResultProducts.Count == 0)
            {
                return;
            }

            Product CurrentProduct = ResultProducts.FirstOrDefault();
            
            // 如果找到了
            if (null != CurrentProduct)
            {
                DetailsModel details = Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == CurrentProduct && x.OrderDetail.OrderDetailId <= 0).FirstOrDefault();
                // 已存在就增加1个(暂时改为提醒已存在)
                if (null != details)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("CurrentProductAlreadyExists"), null, PopupType.Warn));
                    }));
                }
                // 不存在新加一个
                else
                {
                    if (CurrentProduct.IsScales == 1)
                        ProductChange(true, CurrentProduct.ProductId, double.Parse(code.Substring(7, 2) + "." + code.Substring(9, 3)));
                    else
                        ProductChange(true, CurrentProduct.ProductId, 1);
                }
            }
        }



        /// <summary>
        /// 设置语言
        /// </summary>
        private void SetCurrentName()
        {
            OwnerName = "";
            UserName = "";
            if (Resources.GetRes().MainLangIndex == 0)
            {
                OwnerName = Resources.GetRes().KEY_NAME_0;
                UserName = Resources.GetRes().AdminModel.AdminName0;
            }
            else if (Resources.GetRes().MainLangIndex == 1)
            {
                OwnerName = Resources.GetRes().KEY_NAME_1;
                UserName = Resources.GetRes().AdminModel.AdminName1;
            }
            else if (Resources.GetRes().MainLangIndex == 2)
            {
                OwnerName = Resources.GetRes().KEY_NAME_2;
                UserName = Resources.GetRes().AdminModel.AdminName2;
            }
        }

        private string _ownerName;
        /// <summary>
        /// 拥有者名字
        /// </summary>
        public string OwnerName
        {
            get { return _ownerName; }
            set
            {
                _ownerName = value;
                OnPropertyChanged("OwnerName");
            }
        }


        private string _userName;
        /// <summary>
        /// 用户名字
        /// </summary>
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
            }
        }






        /// <summary>
        /// 刷新客显
        /// </summary>
        public void RefreshPM()
        {
            RefreshState();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            Selected.RoomDisplay = false;

            Common.GetCommon().OpenPriceMonitor("0");

            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
            }

            // 语言默认, 或者上次选择
            if (Resources.GetRes().DefaultOrderLang == -1)
            {
                LanguageMode = Resources.GetRes().MainLang.LangIndex;
            }
            else
            {
                LanguageMode = Resources.GetRes().DefaultOrderLang;
            }

            // 刷新第二屏语言
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(LanguageMode).LangIndex, -1);
            }


            DisplayMode = 2;


            Selected.TotalPrice = 0;


            Selected.CurrentSelectedList.Clear();
            Selected.CurrentSelectedListNew.Clear();

            Products.ProductList.Clear();
            Products.ProductListNew.Clear();

            Selected.ResetPage();

            // 刷新产品
            Products.Init(-1);

            Selected.HasAddress = 1;


            RefreshState();

            if (!IsScanReady)
            {
                IsScanReady = true;
                // 扫条码
                Notification.Instance.NotificationBarcodeReader += Instance_NotificationBarcodeReader;

                // 扫码,刷卡

                // 扫条码处理
                hookBarcode = new KeyboardHook();

                var availbleScanners = hookBarcode.GetKeyboardDevices();
                string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == Resources.GetRes().BarcodeReader).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(first))
                {
                    hookBarcode.SetDeviceFilter(first);

                    hookBarcode.KeyPressed += OnBarcodeKey;

                    hookBarcode.AddHook(_element as Window);
                }


                hookCard = new KeyboardHook();
                first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == Resources.GetRes().CardReader).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(first))
                {
                    hookCard.SetDeviceFilter(first);

                    hookCard.KeyPressed += OnCardKey;

                    hookCard.AddHook(_element as Window);
                }




            }



        }



        private KeyboardHook hookCard;
        private KeyboardHook hookBarcode;



        private string keyInput = "";
        private void OnBarcodeKey(object sender, KeyPressedEventArgs e)
        {

            if (_element.Visibility == Visibility.Visible)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput.Trim() != "")
                        Res.Server.Notification.Instance.ActionBarcodeReader(null, keyInput, null);

                    keyInput = "";
                }
                else
                {
                    keyInput += e.Text;
                }
            }
        }



        private string keyInput2 = "";
        private void OnCardKey(object sender, KeyPressedEventArgs e)
        {

            if (_element.Visibility == Visibility.Visible)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput2.Trim() != "" && keyInput2.Trim().Length == 10)
                        Res.Server.Notification.Instance.ActionCardReader(null, keyInput2, null);

                    keyInput2 = "";

                }
                else
                {
                    keyInput2 += e.Text;
                }
            }
        }

        private bool IsScanReady = false;


        /// <summary>
        /// 扫条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationBarcodeReader(object sender, string value, object args)
        {
            if (_selectedUI.IsVisible)
                ScanBarcode(value);
        }


     








        /// <summary>
        /// 重新计算
        /// </summary>
        private void ReCalc()
        {
            Takeout tempTakeout;
            List<TakeoutDetail> details;
            Calc(out details, out tempTakeout, true);
        }


        /// <summary>
        /// 操作订单详情(0新增1删除,)
        /// </summary>
        /// <param name="IsAdd"></param>
        /// <param name="details"></param>
        private void OperateDetails(int mode, DetailsModel details)
        {
            if (mode == 0)
                Selected.CurrentSelectedList.Insert(0, details);
            else if (mode == 1)
            {
                Selected.CurrentSelectedList.Remove(details);
                ProductStateModel productStateModel = Products.ProductList.Where(x => x.Product == details.Product).FirstOrDefault();
                if (null != productStateModel && productStateModel.IsSelected)
                {
                    productStateModel.IsSelected = false;
                    productStateModel.DetailsModel = null;
                }
            }

            Selected.ResetPage();

            RefreshState();
        }


        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="key"></param>
        private void AddProduct(int mode, string key)
        {
            List<Product> productList = new List<Product>();

            if (mode == 1)
            {
                // 获取条码产品
                productList = Resources.GetRes().Products.Where(x => (x.Barcode == key || (x.IsScales == 1 && key.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
            }
            else if (mode == 2)
            {
                var proList = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId);

                productList = proList.Where(x => x.ProductName0.Contains(key) || x.ProductName1.Contains(key) || x.ProductName2.Contains(key)).ToList();
            }


            // 刷新产品
            Products.RefreshProduct(productList);
        }



        /// <summary>
        /// 产品修改
        /// </summary>
        /// <param name="IsSelected"></param>
        /// <param name="productId"></param>
        private DetailsModel ProductChange(bool IsSelected, long productId)
        {
            return ProductChange(IsSelected, productId, 1);
        }


        /// <summary>
        /// 产品修改
        /// </summary>
        /// <param name="IsSelected"></param>
        /// <param name="productId"></param>
        private DetailsModel ProductChange(bool IsSelected, long productId, double count)
        {
            DetailsModel model = null;
            Product product = Resources.GetRes().Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (!IsSelected)
            {
                Selected.CurrentSelectedList.Remove(Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == product).FirstOrDefault());
            }
            else
            {
                model = new DetailsModel() { IsNew = true, AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Product = product, OrderDetail = new OrderDetail() { OrderDetailId = -1, ProductId = product.ProductId, Count = count, AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), Price = product.Price, State = 0, TotalPrice = product.Price }, Operate = OperateDetails };
                Selected.CurrentSelectedList.Insert(0, model);
            }

            Selected.ResetPage();

            RefreshState();

            return model;
        }

        /// <summary>
        /// 刷新一些控件(每次产品或数量变动)
        /// </summary>
        private void RefreshState()
        {

            Takeout tempTakeout;
            List<TakeoutDetail> details;
            Calc(out details, out tempTakeout, true);

            // 保存按钮
            if (Selected.CurrentSelectedList.Count > 0)
            {
                if (Common.GetCommon().IsIncomeTradingManage())
                    Selected.SaveMode = 2;
                else
                    Selected.SaveMode = 0;
            }
            else
            {
                Selected.SaveMode = 0;

            }
           
                
        }

        /// <summary>
        /// 检测新产品是否已选中
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private DetailsModel DetectProductIsSelected(Product product)
        {
            return Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == product && x.OrderDetail.OrderDetailId <= 0).FirstOrDefault();
        }


        


        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
             //Operate(1);
        }



        private double _lastBorrowPrice = 0;
        /// <summary>
        /// 结账
        /// </summary>
        private void Checkout()
        {
            
            // 准确输入数量信息
            if (Selected.CurrentSelectedList.Any(x=>x.Count == 0))
            {
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("Count")), null, PopupType.Warn));
                return;
            }

            Takeout tempTakeout;
            List<TakeoutDetail> details;
            Calc(out details, out tempTakeout, true);

            tempTakeout.tb_takeoutdetail = details;
            _lastBorrowPrice = tempTakeout.BorrowPrice;
            

            this.TakeoutCheckout.Init(tempTakeout);
        }

        /// <summary>
        /// 返回值或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValueOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return value.Trim();
        }





        private double lastTotal = 0;
        private double lastOriginalTotalPrice = 0;

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="details"></param>
        /// <param name="takeout"></param>
        /// <param name="IgnoreError"></param>
        /// <param name="OnlyTotal"></param>
        /// <param name="IgnoreNotConfirm"></param>
        /// <param name="IgnoreCanceld"></param>
        /// <param name="IgnoreCancelId"></param>
        private void Calc(out List<TakeoutDetail> details, out Takeout takeout, bool IgnoreError = true, bool OnlyTotal = false, bool IgnoreNotConfirm = true, bool IgnoreCanceld = true, long IgnoreCancelId = -999)
        {
            details = new List<TakeoutDetail>();
            List<TakeoutDetail> detailsAll = new List<TakeoutDetail>();
            takeout = new Takeout();

            if (!OnlyTotal)
            {
                foreach (var item in Selected.CurrentSelectedList)
                {
                    TakeoutDetail takeoutDetails = new TakeoutDetail();
                    takeoutDetails.ProductId = item.Product.ProductId;
                    takeoutDetails.IsPack = item.OrderDetail.IsPack;
                    takeoutDetails.Count = item.OrderDetail.Count;
                    if (item.NewPrice.HasValue)
                        takeoutDetails.Price = item.NewPrice.Value;
                    else
                        takeoutDetails.Price = item.OrderDetail.Price;
                    takeoutDetails.TotalPrice = item.TotalPrice;

                    takeoutDetails.OriginalTotalPrice = Math.Round(item.OrderDetail.Price * item.OrderDetail.Count);
                    takeoutDetails.TotalCostPrice = Math.Round(item.Product.CostPrice * item.OrderDetail.Count);
                    if (item.Product.CostPrice == 0 && null != item.Product.ProductParentId)
                    {
                        Product parentProduct = Resources.GetRes().Products.FirstOrDefault(x => x.ProductId == item.Product.ProductParentId);

                        if (null != parentProduct)
                        {
                            double price = Math.Round(parentProduct.CostPrice / item.Product.ProductParentCount, 2);
                            takeoutDetails.TotalCostPrice = Math.Round(price * takeoutDetails.Count, 2);
                        }
                    }

                    takeoutDetails.TakeoutDetailId = item.OrderDetail.OrderDetailId;
                    takeoutDetails.State = item.OrderDetail.State;
                    takeoutDetails.Request = item.OrderDetail.Request;


                    if (item.IsNew)
                        details.Add(takeoutDetails);
                    detailsAll.Add(takeoutDetails);




                }

            }


           
            IEnumerable<TakeoutDetail> totalDetails = detailsAll;

            if (IgnoreNotConfirm)
                totalDetails = totalDetails.Where(x => x.State != 1);
            if (IgnoreCanceld)
                totalDetails = totalDetails.Where(x => x.State != 3);

            lastTotal = Math.Round(totalDetails.Sum(x => x.TotalPrice), 2);
            lastOriginalTotalPrice = Math.Round(totalDetails.Sum(x => x.OriginalTotalPrice), 2);

            if (IgnoreCancelId != -999 && totalDetails.Where(x => x.TakeoutDetailId == IgnoreCancelId).Count() > 0)
            {
                lastTotal = Math.Round(lastTotal - totalDetails.Where(x => x.TakeoutDetailId == IgnoreCancelId).FirstOrDefault().TotalPrice, 2);
                lastOriginalTotalPrice =Math.Round( lastOriginalTotalPrice - totalDetails.Where(x => x.TakeoutDetailId == IgnoreCancelId).FirstOrDefault().OriginalTotalPrice, 2);
            }



            
            takeout.Lang = LanguageMode;
            takeout.IsPack = 0;




            
            takeout.TotalPaidPrice = Math.Round(takeout.MemberPaidPrice + takeout.PaidPrice, 2);


           



            
            Selected.TotalPrice = takeout.TotalPrice = lastTotal ;
            takeout.OriginalTotalPrice = lastOriginalTotalPrice;



            double balancePrice = 0;

            balancePrice = Math.Round(takeout.TotalPaidPrice - takeout.TotalPrice, 2);


            if (balancePrice > 0)
            {
                takeout.KeepPrice = balancePrice;
                takeout.BorrowPrice = 0;
            }

            else if (balancePrice < 0)
            {
                takeout.BorrowPrice = balancePrice;
                takeout.KeepPrice = 0;
            }

            else if (balancePrice == 0)
            {
                takeout.BorrowPrice = 0;
                takeout.KeepPrice = 0;
            }

            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(takeout.BorrowPrice.ToString());
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(takeout, details, null));
            }

            if (OnlyTotal)
                return;



        }






        private int _displayMode;
        /// <summary>
        /// 显示模式 1产品2已选3产品输入
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



        


        private ProductsViewModel _products;
        /// <summary>
        /// 显示产品控件
        /// </summary>
        public ProductsViewModel Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged("ProductsViewModel");
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
            }
        }


        private string _languageName = "";
        /// <summary>
        /// 选择名
        /// </summary>
        public string LanguageName
        {
            get { return Resources.GetRes().GetMainLangByLangIndex(_languageMode).LangName; }
            set
            {
                _languageName = value;
                OnPropertyChanged("LanguageName");
            }
        }
        



        private bool _isSave = false;
        /// <summary>
        /// 是否临时保存
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


        private ViewModels.Controls.AnimationViewModel _animation;
        /// <summary>
        /// 动画
        /// </summary>
        public ViewModels.Controls.AnimationViewModel Animation
        {
            get { return _animation; }
            set
            {
                _animation = value;
                OnPropertyChanged("AnimationViewModel");
            }
        }


        private ViewModels.Controls.MsgViewModel _msg;
        /// <summary>
        /// 消息框
        /// </summary>
        public ViewModels.Controls.MsgViewModel Msg
        {
            get { return _msg; }
            set
            {
                _msg = value;
                OnPropertyChanged("Msg");
            }
        }

        /// <summary>
        /// 消息命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetMsgCommand(string no)
        {
            // 确定
            if (no == "OK")
            {

            }
            // 是
            else if (no == "Yes")
            {

            }
            // 否
            else if (no == "No")
            {

            }

            PopupRoutedEventArgs popupArgs = MsgList.Dequeue();
            if (null != popupArgs.Operate)
                popupArgs.Operate(no);



            Msg.AlertMsgMode = false;
            InitialMsg();

        }
        /// <summary>
        /// 信息列表
        /// </summary>
        private Queue<PopupRoutedEventArgs> MsgList = new Queue<PopupRoutedEventArgs>();
        /// <summary>
        /// 处理弹出按钮路由
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandlePopop(object sender, RoutedEventArgs args)
        {
            PopupRoutedEventArgs popupArgs = args as PopupRoutedEventArgs;
            if (null != popupArgs)
            {
                switch (popupArgs.PopupType)
                {
                    case PopupType.AnimationOn:
                        Animation.IsDisplay = true;
                        break;
                    case PopupType.AnimationOff:
                        Animation.IsDisplay = false;
                        break;
                    case PopupType.Information:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    case PopupType.Warn:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    case PopupType.Error:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    case PopupType.Question:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// 初始化信息
        /// </summary>
        private void InitialMsg()
        {
            // 没有显示窗口, 并消息堆里大于0时执行
            if (!Msg.AlertMsgMode && MsgList.Count > 0)
            {
                PopupRoutedEventArgs popupArgs = MsgList.Peek();

                // 更改显示模式
                this.Msg.ChangeMode(popupArgs);

            }
        }



        private SelectedViewModel _selected;
        /// <summary>
        /// 显示已选控件
        /// </summary>
        public SelectedViewModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }


        private TakeoutCheckoutViewModel _takeoutCheckoutView;
        /// <summary>
        /// 显示结账控件
        /// </summary>
        public TakeoutCheckoutViewModel TakeoutCheckout
        {
            get { return _takeoutCheckoutView; }
            set
            {
                _takeoutCheckoutView = value;
                OnPropertyChanged("CheckoutView");
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




        /// <summary>
        /// 打开产品列表按钮
        /// </summary>
        private RelayCommand _selectedCommand;
        public ICommand SelectedCommand
        {
            get
            {
                if (_selectedCommand == null)
                {
                    _selectedCommand = new RelayCommand(param => DisplayMode = 1);
                }
                return _selectedCommand;
            }
        }


        /// <summary>
        /// 打开产品列表按钮
        /// </summary>
        private RelayCommand _productsCommand;
        public ICommand ProductsCommand
        {
            get
            {
                if (_productsCommand == null)
                {
                    _productsCommand = new RelayCommand(param => DisplayMode = 2);
                }
                return _productsCommand;
            }
        }




        /// <summary>
        /// 切换语言按钮
        /// </summary>
        private RelayCommand _changeOrderLanguageCommand;
        public ICommand ChangeOrderLanguageCommand
        {
            get
            {
                if (_changeOrderLanguageCommand == null)
                {
                    _changeOrderLanguageCommand = new RelayCommand(param =>
                        {
                            this.LanguageMode = Resources.GetRes().GetMainLangByMainLangIndex(Resources.GetRes().GetMainLangByLangIndex(this.LanguageMode).MainLangIndex + 1).LangIndex;

                            // 刷新第二屏语言
                            if (FullScreenMonitor.Instance._isInitialized)
                            {
                                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(LanguageMode).LangIndex, -1);
                            }
                        });
                }
                return _changeOrderLanguageCommand;
            }
        }



        /// <summary>
        /// 搜索按钮
        /// </summary>
        private RelayCommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(param =>
                    {
                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.Search, null));

                    });
                }
                return _searchCommand;
            }
        }






        // 临时保存的集合
        private List<DetailsModel> TempSave = new List<DetailsModel>();

       

        /// <summary>
        /// 处理键盘输入
        /// </summary>
        /// <param name="args"></param>
        public void HandleKey(KeyEventArgs args)
        {
            // 动画效果时忽略按键
            if (Animation.IsDisplay)
            {
                if (args.Key == Key.F10 || args.Key == Key.Tab)
                    args.Handled = true;
                return;
            }

            // 弹出框处理
            else if (Msg.AlertMsgMode)
            {
                Msg.HandleKey(args);

                if (args.Key == Key.F10 || args.Key == Key.Tab)
                    args.Handled = true;
                
            }
            // 检查是不是功能键
            else if ((args.Key >= Key.F1 && args.Key <= Key.F12) || args.Key == Key.Tab || args.Key == Key.System)
            {
                // F3查找产品
                if (args.Key == Key.F3)
                {
                    if (this.DisplayMode != 3 && !this.TakeoutCheckout.IsDisplay)
                    {
                        this.Products.SearchText = "";
                        this.Products.ProductList.Clear();
                        this.Products.ResetPage();
                        this.DisplayMode = 3;
                    }
                }
                // F7 暂存
                else if (args.Key == Key.F7 && this.DisplayMode == 2 && !this.TakeoutCheckout.IsDisplay && Selected.CurrentSelectedList.Count > 0)
                {
                    // 保存到临时集合
                    TempSave.AddRange(this.Selected.CurrentSelectedList);

                    // 清空原有的集合
                    foreach (var item in TempSave)
                    {
                        OperateDetails(1, item);
                    }

                    IsSave = true;

                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("Save")), null, PopupType.Information));

                }
                else if (args.Key == Key.F8 && this.DisplayMode == 2 && !this.TakeoutCheckout.IsDisplay && Selected.CurrentSelectedList.Count == 0 && TempSave.Count > 0) 
                {
                    // 重新选中暂存中的
                    foreach (var item in TempSave)
                    {
                        ProductChange(true, item.Product.ProductId, item.Count);
                    }

                    TempSave.Clear();

                    IsSave = false;

                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("Restore")), null, PopupType.Information));
                }
                // F12关闭
                else if (args.Key == Key.F12 && this.DisplayMode == 2 && !this.TakeoutCheckout.IsDisplay)
                {
                    (_element as Window).Close();
                }
                else if (args.Key == Key.F10 || args.Key == Key.System)
                {
                    if (this.TakeoutCheckout.IsDisplay)
                        this.TakeoutCheckout.HandleKey(args);

                    args.Handled = true;
                }
                // 切换语言
                else if (args.Key == Key.Tab)
                {
                    if (this.DisplayMode == 2)
                        this.ChangeOrderLanguageCommand.Execute(null);

                    args.Handled = true;

                }else {
                    //if (args.Key == Key.F5) {
                        
                    //        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("RefreshOrderFailed"), null, PopupType.Warn));
                     
                    //}else if (args.Key == Key.F6)
                    //{

                    //    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("RefreshOrderFailed"), null, PopupType.Information));

                    //}else if (args.Key == Key.F7)
                    //{

                    //    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("RefreshOrderFailed"), null, PopupType.Question));

                    //}else if (args.Key == Key.F8)
                    //{

                    //    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("RefreshOrderFailed"), null, PopupType.Error));

                    //}
                }
            }
            // 结账处理
            else if (this.TakeoutCheckout.IsDisplay)
            {
                    this.TakeoutCheckout.HandleKey(args);
            }
            // 已选模式下
            else if (this.DisplayMode == 2)
            {

                Selected.HandleKey(args);
                
            }
            // 搜索模式下
            else if (this.DisplayMode == 3)
            {
                // 搜索指定内容
                if (args.Key == Key.Enter && this.Products.SearchText.Length > 0)
                {
                    long n = 0;
                    bool isNumeric = long.TryParse(this.Products.SearchText, out n);

                    this.DisplayMode = 1;
                    AddProduct(isNumeric ? 1 : 2, this.Products.SearchText);
                }
                // 退回
                else if (args.Key == Key.Escape)
                {
                    this.DisplayMode = 2;
                    this.Selected.CalcAndResetPage();
                }
                else
                {
                    this.Products.HandleSearchKey(args);
                }
            }
            // 产品模式
            else if (this.DisplayMode == 1)
            {
                if (args.Key == Key.Escape)
                {
                    this.DisplayMode = 3;
                }
                else
                {
                    this.Products.HandleKey(args);
                }
            }
        }


    }


}
