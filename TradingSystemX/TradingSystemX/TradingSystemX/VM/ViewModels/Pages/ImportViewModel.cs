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
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.DAL;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using Oybab.Res.Tools;
using ZXing.Mobile;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class ImportViewModel : ViewModelBase
    {
        private Page _element;
       

        public ImportViewModel(Page _element)
        {
            this._element = _element;

            // 设置物理后退弹出面板
            if (null == NavigationPath.Instance.ImportPanelClose)
                NavigationPath.Instance.ImportPanelClose = ReInitPanels;
            // 设置物理后退关闭该页面
            if (null == NavigationPath.Instance.ImportClose)
                NavigationPath.Instance.ImportClose = Close;

            // 设置重新加载页面(因为ListView的BUG导致的临时修复)
            if (null == NavigationPath.Instance.ReloadImportProductPage)
                NavigationPath.Instance.ReloadImportProductPage = ReloadImportProductPage;
            if (null == NavigationPath.Instance.ReloadImportSelectPage)
                NavigationPath.Instance.ReloadImportSelectPage = ReloadImportSelectPage;

            Notification.Instance.NotificationLanguage += (obj, value, args) => { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { SetCurrentName(); }); };

            // 设置语言
            SetCurrentName();



            // 菜单列表填充一下
            MainLists.Add(new MainListModel() { Name = "Products", GoCommand = this.GoCommand });
            MainLists.Add(new MainListModel() { Name = "Selected", GoCommand = this.GoCommand });
            MainLists.Add(new MainListModel());
            MainLists.Add(new MainListModel() { Name = "Back", GoCommand = this.GoCommand });


            this.Products = new ProductsViewModel(NavigationPath.Instance.ImportProductPage, NavigationPath.Instance.ImportProductPage.GetProductTypeListContent(), NavigationPath.Instance.ImportProductPage.GetProductContent(), NavigationPath.Instance.ImportProductPage.GetProductTemplate(), true);
            this.Products.ProductChange = ProductChange;
            this.Products.SearchProduct = AddProduct;
            this.Products.DetectProductIsSelected = DetectProductIsSelected;
            this.Products.GoCommand = GoCommand;
            this.Products.Operate = OperateDetails;


            this.Selected = new SelectedViewModel(NavigationPath.Instance.ImportSelectedPage, ReCalc, RecalcPaidPrice, NavigationPath.Instance.ImportSelectedPage.GetSelectedContent(), NavigationPath.Instance.ImportSelectedPage.GetSelectedTemplate(), NavigationPath.Instance.ImportSelectedPage.GetRequestContent(), NavigationPath.Instance.ImportSelectedPage.GetRequestTemplate(), NavigationPath.Instance.ImportSelectedPage.GetBalanceContent(), NavigationPath.Instance.ImportSelectedPage.GetBalanceTemplate());
            this.Selected.Save = Save;
            this.Selected.Checkout = Checkout;
            this.Selected.GoCommand = GoCommand;
            this.Selected.IsImport = true;


            NavigationPath.Instance.ImportSelectedPage.BindingContext = this.Selected;
            NavigationPath.Instance.ImportProductPage.BindingContext = this.Products;
        }


        /// <summary>
        /// 重新加载产品页
        /// </summary>
        private void ReloadImportProductPage()
        {
            NavigationPath.Instance.ImportProductPage = new ProductsPage();
            Products.ReLoadProductsViewModel(NavigationPath.Instance.ImportProductPage, NavigationPath.Instance.ImportProductPage.GetProductTypeListContent());
            NavigationPath.Instance.ImportProductPage.BindingContext = this.Products;

            NavigationPath.Instance.ImportProductNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.ImportProductPage);

            Products.ReloadList();
        }

        /// <summary>
        /// 重新加载已选页
        /// </summary>
        private void ReloadImportSelectPage()
        {
            NavigationPath.Instance.ImportSelectedPage = new SelectedPage();
            Selected.ReloadSelectedViewModel(NavigationPath.Instance.ImportSelectedPage);
            NavigationPath.Instance.ImportSelectedPage.BindingContext = this.Selected;

            NavigationPath.Instance.ImportSelectedNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.ImportSelectedPage);


            Selected.ReloadList();
        }




        private ObservableCollection<MainListModel> _mainLists = new ObservableCollection<MainListModel>();
        /// <summary>
        /// 所有菜单项
        /// </summary>
        public ObservableCollection<MainListModel> MainLists
        {
            get { return _mainLists; }
            set
            {
                _mainLists = value;
                OnPropertyChanged("MainLists");
            }
        }


        private object _selectedMainItem;
        /// <summary>
        /// 选择的菜单项
        /// </summary>
        public object SelectedMainItem
        {
            get { return _selectedMainItem; }
            set
            {
                _selectedMainItem = value;
                OnPropertyChanged("SelectedMainItem");
            }
        }



        private bool _isPresented;
        /// <summary>
        /// 是否展开字母菜单
        /// </summary>
        public bool IsPresented
        {
            get { return _isPresented; }
            set
            {
                _isPresented = value;
                OnPropertyChanged("IsPresented");
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


        /// <summary>
        /// 设置语言
        /// </summary>
        private void SetCurrentName()
        {

            foreach (var item in MainLists)
            {
                item.LangChange();
            }
        }



        /// <summary>
        /// 重新计算支付金额
        /// </summary>
        private void RecalcPaidPrice()
        {
            AddressViewModel viewModel = NavigationPath.Instance.AddressPage.BindingContext as AddressViewModel;
            if (null != viewModel && this.Selected.ChangePaidPriceView.Remark != viewModel.Remark)
            {
                this.Selected.RemarkChanged = true;
            }

            // 如果跟订单上次保存的不一样,就提示未保存提示
            if (this.Selected.oldList.All(this.Selected.ChangePaidPriceView.PayModel.Contains) && this.Selected.tempPayList.Count == this.Selected.ChangePaidPriceView.PayModel.Count)
            {
            }
            else
            {
                this.Selected.RoomPaidPriceChanged = true;
                this.Selected.tempPayList = this.Selected.ChangePaidPriceView.PayModel.Select(x => x.GetOrderPay()).ToList();
            }

            if (this.Selected.RoomPaidPriceChanged || this.Selected.RemarkChanged)
            {


                ReCalc();

            }
        }


        /// <summary>
        /// 跳转
        /// </summary>
        private RelayCommand _goCommand;
        public Command GoCommand
        {
            get
            {
                return _goCommand ?? (_goCommand = new RelayCommand(param =>
                {

                    string model = param as string;//param as MainListModel;
                    if (null != model)
                    {
                        switch (model)
                        {
                            case "Products":
                            case "Products1":
                                IsLoading = true;
                                if (model == "Products1")
                                    this.Selected.IsLoading = true;
                                Task.Run(async () =>
                                {

                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.SwitchMasterDetailNavigate(0);
                                        IsPresented = false;
                                        IsLoading = false;
                                        if (model == "Products1")
                                            this.Selected.IsLoading = false;
                                    });
                                });
                                break;
                            case "Selected":
                            case "Selected1":
                                IsLoading = true;

                                if (model == "Selected1")
                                    this.Products.IsLoading = true;
                                Task.Run(async () =>
                                {

                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        ReInitPanels();


                                        NavigationPath.Instance.SwitchMasterDetailNavigate(1);
                                        IsPresented = false;
                                        IsLoading = false;
                                        if (model == "Selected1")
                                            this.Products.IsLoading = false;
                                    });
                                });
                                break;
                            
                            case "Back":
                                Close();
                                break;
                            case "Lang":
                                ChangeOrderLanguageCommand.Execute(null);
                                break;
                            case "Barcode":

                                this.Products.IsLoading = true;

                                Task.Run(async () =>
                                {

                                    await ExtX.WaitForLoading();

                                   

                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        bool RequestCamera = await Common.Instance.CheckCamera();

                                        if (!RequestCamera)
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, Resources.Instance.GetString("PermissionDenied"), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);
                                            this.Products.IsLoading = false;
                                            return;
                                        }

                                     
                                        var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
                                        options.PossibleFormats = new List<ZXing.BarcodeFormat>() {ZXing.BarcodeFormat.EAN_8, ZXing.BarcodeFormat.EAN_13};
                                        
                                        
                                        var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                                        scanner.CancelButtonText = Resources.Instance.GetString("Back");

                                        //var result = await scanner.Scan(options);

                                        //if (result != null)
                                        //{
                                        //    AddProduct(result.Text, true);
                                        //}
                                        options.DelayBetweenContinuousScans = 3000;
                                       
                                        bool isIgnore = false;
                                        scanner.ScanContinuously(options, x =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(x.Text) && !isIgnore)
                                            {
                                                isIgnore = true;
                                                Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                                {
                                                    AddProduct(x.Text, true);
                                                    isIgnore = false;
                                                }));
                                            }
                                        });

                                        this.Products.IsLoading = false;
                                    });
                                });
                                break;
                            default:
                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        SelectedMainItem = null;
                                        IsLoading = false;
                                    });
                                });
                                break;
                        }


                    }

                }));
            }
        }

        /// <summary>
        /// 重置一下弹出面板
        /// </summary>
        private void ReInitPanels()
        {
            if (Selected.RequestView.IsShow)
                Selected.RequestView.IsShow = false;
            if (Selected.ChangeCountView.IsShow)
                Selected.ChangeCountView.IsShow = false;
            if (Selected.ChangePriceView.IsShow)
                Selected.ChangePriceView.IsShow = false;
            if (Selected.ChangeTimeView.IsShow)
                Selected.ChangeTimeView.IsShow = false;
            if (Selected.ChangePaidPriceView.IsShow)
                Selected.ChangePaidPriceView.IsShow = false;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void Close()
        {
            IsLoading = true;
            IsPresented = false;

            Task.Run(async () =>
            {
                
                await ExtX.WaitForLoading();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    
                    NavigationPath.Instance.SwitchNavigate(1);
                    IsLoading = false;
                });
            });
        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(Action IsFirst = null)
        {
            Selected.RoomDisplay = false;

            if (!string.IsNullOrWhiteSpace(Products.SearchKey))
                Products.SearchKey = "";

            Selected.GoCommand = null;
            // 语言默认, 或者上次选择
            if (Resources.Instance.DefaultOrderLang == -1)
            {
                Selected.LanguageMode = Res.Instance.MainLang.LangIndex;
            }
            else
            {
                Selected.LanguageMode = Resources.Instance.DefaultOrderLang;
            }
            Selected.GoCommand = GoCommand;


            Selected.ClearList();



            DisplayMode = 2;


            Selected.TotalPrice = 0;


            // 刷新产品
            Products.Init(-1);

            // 重置面板
            ReInitPanels();


            RefreshState();


            Selected.ChangePaidPriceView.Remark = "";

            Selected.RoomTimeChange = false;
            Selected.RoomPaidPriceChanged = false;
            Selected.RemarkChanged = false;

            if (null != IsFirst)
            {
                IsFirst();

                IsPresented = false;
            }

        }






        /// <summary>
        /// 操作订单详情(0新增1删除,2刷新)
        /// </summary>
        /// <param name="IsAdd"></param>
        /// <param name="details"></param>
        private void OperateDetails(int mode, DetailsModel details)
        {
            if (mode == 0)
                Selected.AddListToFirst(details);
            else if (mode == 1)
            {
                Selected.RemoveSelected(details);
                ProductStateModel productStateModel = Products.ProductList.Where(x => x.Product == details.Product).FirstOrDefault();
                if (null != productStateModel && productStateModel.IsSelected)
                {
                    productStateModel.IsSelected = false;
                    productStateModel.DetailsModel = null;
                }
            }
            
            RefreshState();
        }


        private void AddProduct(string key)
        {
            AddProduct(key, false);
        }

        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="key"></param>
        private void AddProduct(string key, bool IsScan)
        {
            List<Product> productList = new List<Product>();

            long n;
            bool isNumeric = long.TryParse(key, out n);

            if (isNumeric)
            {
                // 获取条码产品
                List<Product> selectedBarcode = Resources.Instance.Products.Where(x => (x.Barcode == key || (x.IsScales == 1 && key.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 3)).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
                if (null != selectedBarcode && selectedBarcode.Count > 0)
                    productList.AddRange(selectedBarcode);

                // 扫描条码, 并只有一个则选中
                if (IsScan && productList.Count == 1)
                {


                    Product CurrentProduct = productList.FirstOrDefault();

                    // 如果找到了
                    if (null != CurrentProduct)
                    {
                        DetailsModel details = Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == CurrentProduct && x.OrderDetail.OrderDetailId <= 0).FirstOrDefault();
                        // 已存在就增加1个(暂时改为提醒已存在)
                        if (null != details)
                        {
                            
                           QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("CurrentProductAlreadyExists"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        // 不存在新加一个
                        else
                        {
                            if (CurrentProduct.IsScales == 1)
                                ProductChange(true, CurrentProduct.ProductId, double.Parse(key.Substring(7, 2) + "." + key.Substring(9, 3)));
                            else
                                ProductChange(true, CurrentProduct.ProductId, 1);

                            string name = "";
                                if (Res.Instance.MainLangIndex == 0)
                                    name = CurrentProduct.ProductName0;
                                else if (Res.Instance.MainLangIndex == 1)
                                    name = CurrentProduct.ProductName1;
                                else if (Res.Instance.MainLangIndex == 2)
                                    name = CurrentProduct.ProductName2;

                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, name, MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);

                        }

                    }

                }
            }
            else
            {

                key = key.ToUpper();

                var proList = Resources.Instance.Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId);

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
            Product product = Resources.Instance.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (!IsSelected)
            {
                Selected.RemoveSelected(Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == product).FirstOrDefault());
            }
            else
            {
                model = new DetailsModel() { Mode = 1, IsNew = true, IsTakeout = true, AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Product = product, OrderDetail = new OrderDetail() { OrderDetailId = -1, ProductId = product.ProductId, Count = count, AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), Price = product.CostPrice, State = 0, TotalPrice = product.CostPrice }, Operate = OperateDetails };
                Selected.AddListToFirst( model);
            }

            RefreshState();


            return model;
        }



        /// <summary>
        /// 刷新一些控件(每次产品或数量变动)
        /// </summary>
        private void RefreshState()
        {

            Import tempImport;
            List<ImportDetail> details;
            Calc(out details, out tempImport, true);

            // 保存按钮
            if (Selected.CurrentSelectedList.Count > 0)
            {
                if (Common.Instance.IsIncomeTradingManage())
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
        /// 返回值或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValueOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return value;
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
            
        }

        /// <summary>
        /// 结账
        /// </summary>
        private void Checkout()
        {

            Import tempImport;
            List<ImportDetail> details;
            Calc(out details, out tempImport, true);

            AddressViewModel viewModel = NavigationPath.Instance.AddressPage.BindingContext as AddressViewModel;

          

            tempImport.Remark = GetValueOrNull(viewModel.Remark);

            tempImport.tb_importdetail = details;



            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                NavigationPath.Instance.ImportCheckoutPage.Init(tempImport);
                // 跳转到结账页面
                NavigationPath.Instance.GoMasterDetailsNavigateNext(NavigationPath.Instance.ImportCheckoutPage, true, true);
            });
            
        }





        /// <summary>
        /// 重新计算
        /// </summary>
        private void ReCalc()
        {
            Import tempImport;
            List<ImportDetail> details;
            Calc(out details, out tempImport, true);
        }

        private double lastTotal = 0;
        private double lastOriginalTotalPrice = 0;


        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="details"></param>
        /// <param name="import"></param>
        /// <param name="IgnoreError"></param>
        /// <param name="OnlyTotal"></param>
        private void Calc(out List<ImportDetail> details, out Import import, bool IgnoreError = true, bool OnlyTotal = false, bool IgnoreNotConfirm = true, bool IgnoreCanceld = true, long IgnoreCancelId = -999)
        {
            details = new List<ImportDetail>();
            List<ImportDetail> detailsAll = new List<ImportDetail>();
            import = new Import();

            if (!OnlyTotal)
            {
                foreach (var item in Selected.CurrentSelectedList)
                {
                    ImportDetail orderDetails = new ImportDetail();
                    orderDetails.ProductId = item.Product.ProductId;
                    orderDetails.Count = item.OrderDetail.Count;
                    if (item.NewPrice.HasValue)
                        orderDetails.Price = item.NewPrice.Value;
                    else
                        orderDetails.Price = item.OrderDetail.Price;
                    orderDetails.TotalPrice = item.TotalPrice;
                    orderDetails.OriginalTotalPrice = Math.Round(item.OrderDetail.Price * item.OrderDetail.Count);
                   

                    orderDetails.ImportDetailId = item.OrderDetail.OrderDetailId;
                    orderDetails.State = item.OrderDetail.State;



                    if (item.IsNew)
                        details.Add(orderDetails);
                    detailsAll.Add(orderDetails);




                }

            }


            //if (!OnlyTotal)
            //{
            IEnumerable<ImportDetail> totalDetails = detailsAll;

            if (IgnoreNotConfirm)
                totalDetails = totalDetails.Where(x => x.State != 1);
            if (IgnoreCanceld)
                totalDetails = totalDetails.Where(x => x.State != 3);

            lastTotal = Math.Round(totalDetails.Sum(x => x.TotalPrice), 2);
            lastOriginalTotalPrice = Math.Round(totalDetails.Sum(x => x.OriginalTotalPrice), 2);

            if (IgnoreCancelId != -999 && totalDetails.Where(x => x.ImportDetailId == IgnoreCancelId).Count() > 0)
            {
                lastTotal = Math.Round(lastTotal - totalDetails.Where(x => x.ImportDetailId == IgnoreCancelId).FirstOrDefault().TotalPrice, 2);
                lastOriginalTotalPrice = Math.Round(lastOriginalTotalPrice - totalDetails.Where(x => x.ImportDetailId == IgnoreCancelId).FirstOrDefault().OriginalTotalPrice, 2);
            }

            //}






            import.TotalPaidPrice = Math.Round(import.SupplierPaidPrice + import.PaidPrice, 2);

            import.ImportTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));


            Selected.TotalPrice = import.TotalPrice = lastTotal;
            import.OriginalTotalPrice = lastOriginalTotalPrice;


            double balancePrice = 0;


            balancePrice = Math.Round(import.TotalPaidPrice - import.TotalPrice, 2);


            if (balancePrice > 0)
            {
                import.KeepPrice = balancePrice;
                import.BorrowPrice = 0;
            }

            else if (balancePrice < 0)
            {
                import.BorrowPrice = balancePrice;
                import.KeepPrice = 0;
            }

            else if (balancePrice == 0)
            {
                import.BorrowPrice = 0;
                import.KeepPrice = 0;
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





        /// <summary>
        /// 切换语言按钮
        /// </summary>
        private RelayCommand _changeOrderLanguageCommand;
        public Command ChangeOrderLanguageCommand
        {
            get
            {
                return _changeOrderLanguageCommand ?? (_changeOrderLanguageCommand = new RelayCommand(param =>
                {
                    //Selected.LanguageMode = Res.Instance.GetMainLangByMainLangIndex(Res.Instance.GetMainLangByLangIndex(this.Selected.LanguageMode).MainLangIndex + 1).LangIndex;
                    Selected.LanguageMode = (int)Selected.SelectedLang.Value;

                }));
            }
        }





        


    }


}
