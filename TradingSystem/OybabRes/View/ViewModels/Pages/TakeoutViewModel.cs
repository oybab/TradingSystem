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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class TakeoutViewModel: ViewModelBase
    {
        private UIElement _element;
        private UIElement _selectedUI;

        private ListBox _lbSelectedList;



        public TakeoutViewModel(UIElement element, UIElement productsUI, UIElement selectedUI, StackPanel spProductTypeList, ScrollViewer scrollViewerProducts, ListBox lbSelectedList, UniformGrid ugRequest)
        {
            this._element = element;
            this._selectedUI = selectedUI;

            this.Products = new ProductsViewModel(productsUI, spProductTypeList, scrollViewerProducts);
            this.Products.ProductChange = ProductChange;
            this.Products.DetectProductIsSelected = DetectProductIsSelected;
            this.Products.Operate = OperateDetails;


            this.Selected = new SelectedViewModel(selectedUI, lbSelectedList);
            this.Selected.Save = Save;
            this.Selected.Checkout = Checkout;
            this.Selected.HasAddress = 1;

            this.Language = new LanguageViewModel(element, ChangeLanguage);

            this.Request = new RequestViewModel(element, ugRequest);
            this._lbSelectedList = lbSelectedList;

            this.ChangePrice = new ChangePriceViewModel(element, ReCalc);
            this.ChangeCount = new ChangeCountViewModel(element, ReCalc);

            this.Address = new AddressViewModel(element);

            this.Search = new SearchViewModel(element, AddProduct);


            // 添加处理事件
            this._element.AddHandler(PublicEvents.BoxEvent, new RoutedEventHandler(HandleBox), true);



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

                Language.LanguageMode = Resources.GetRes().MainLang.LangIndex;

            }
            else
            {
                Language.LanguageMode = Resources.GetRes().DefaultOrderLang;
            }

            // 刷新第二屏语言
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(Language.LanguageMode).LangIndex, -1);
            }


            DisplayMode = 2;
                

            Selected.TotalPrice = 0;
            Selected.RemarkChanged = false;
            //RemarkView.Remark = null;


            Selected.CurrentSelectedList.Clear();


            // 刷新产品
            Products.Init(-1);

            Selected.HasAddress = 1;
            Address.Clear();

            RefreshState();

            if (!IsScanReady)
            {
                IsScanReady = true;
                // 扫条码
                Notification.Instance.NotificationBarcodeReader += Instance_NotificationBarcodeReader;
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
                    case BoxType.Request:
                        this.Request.InitialRequest(boxArgs.Param as DetailsModel);
                        this.Request.Show();
                        break;
                    case BoxType.ChangePrice:
                        this.ChangePrice.Init(boxArgs.Param as DetailsModel);
                        this.ChangePrice.Show();
                        break;
                    case BoxType.ChangeCount:
                        this.ChangeCount.Init(boxArgs.Param as DetailsModel);
                        this.ChangeCount.Show();
                        break;
                    case BoxType.Search:
                        this.Search.Show();
                        break;
                    case BoxType.Address:
                        this.Address.Init(boxArgs.Param as SelectedViewModel);
                        this.Address.Show();
                        break;
                    default:
                        break;
                }
            }
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
                model = new DetailsModel() { _element = _lbSelectedList, IsNew = true, AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Product = product, OrderDetail = new OrderDetail() { OrderDetailId = -1, ProductId = product.ProductId, Count = count, AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), Price = product.Price, State = 0, TotalPrice = product.Price }, Operate = OperateDetails };
                Selected.CurrentSelectedList.Insert(0, model);
            }

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
        /// 修改语言
        /// </summary>
        /// <param name="lang"></param>
        private void ChangeLanguage(int lang)
        {
           Language.LanguageMode = lang;

            // 刷新第二屏语言
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(Language.LanguageMode).LangIndex, -1);
            }

            this.Language.Hide(null);
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
            Takeout tempTakeout;
            List<TakeoutDetail> details;
            Calc(out details, out tempTakeout, true);

            tempTakeout.tb_takeoutdetail = details;
            _lastBorrowPrice = tempTakeout.BorrowPrice;



            tempTakeout.Phone = GetValueOrNull(Address.Phone);
            tempTakeout.Name0 = tempTakeout.Name1 = tempTakeout.Name2 = GetValueOrNull(Address.Name);
            tempTakeout.Address0 = tempTakeout.Address1 = tempTakeout.Address2 = GetValueOrNull(Address.Address);

           
            tempTakeout.Remark = GetValueOrNull(Address.Remark);



            _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.CheckoutTakeout, tempTakeout));
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

           



            
            takeout.Lang = Language.LanguageMode;
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
        /// 显示模式 1产品2已选
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



        private LanguageViewModel _language;
        /// <summary>
        /// 显示已选控件
        /// </summary>
        public LanguageViewModel Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged("Language");
            }
        }



        private RequestViewModel _request;
        /// <summary>
        /// 请求控件
        /// </summary>
        public RequestViewModel Request
        {
            get { return _request; }
            set
            {
                _request = value;
                OnPropertyChanged("Request");
            }
        }


        

        private SearchViewModel _search;
        /// <summary>
        /// 搜索控件
        /// </summary>
        public SearchViewModel Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnPropertyChanged("Search");
            }
        }





        private ChangeCountViewModel _changeCount;
        /// <summary>
        /// 修改数量控件
        /// </summary>
        public ChangeCountViewModel ChangeCount
        {
            get { return _changeCount; }
            set
            {
                _changeCount = value;
                OnPropertyChanged("ChangeCount");
            }
        }




        private ChangePriceViewModel _changePrice;
        /// <summary>
        /// 修改价格控件
        /// </summary>
        public ChangePriceViewModel ChangePrice
        {
            get { return _changePrice; }
            set
            {
                _changePrice = value;
                OnPropertyChanged("ChangePrice");
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


        private AddressViewModel _address;
        /// <summary>
        /// 显示地址控件
        /// </summary>
        public AddressViewModel Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged("Address");
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
                            this.Language.LanguageMode = Resources.GetRes().GetMainLangByMainLangIndex(Resources.GetRes().GetMainLangByLangIndex(this.Language.LanguageMode).MainLangIndex + 1).LangIndex;

                            // 刷新第二屏语言
                            if (FullScreenMonitor.Instance._isInitialized)
                            {
                                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(Language.LanguageMode).LangIndex, -1);
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



    }

}
