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

namespace Oybab.TradingSystemX.VM.ViewModels.Pages.Controls
{
    internal sealed class ProductsViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;
        private long ProductTypeId = -1;
        internal Func<bool, long, DetailsModel> ProductChange;
        internal Action<string> SearchProduct;
        internal Func<Product, DetailsModel> DetectProductIsSelected;
        internal Action<int, DetailsModel> Operate { get; set; }
        private long RoomId = -1;
        private bool IsImport = false;


        public ProductsViewModel(Xamarin.Forms.Page _element, Xamarin.Forms.StackLayout spProductTypeList, Xamarin.Forms.StackLayout spProductList, Xamarin.Forms.ControlTemplate ctProductControlTemplate, bool IsImport = false)
        {
            this._element = _element;
            this._spProductTypeList = spProductTypeList;
            Notification.Instance.NotificationLanguage += (obj, value, args) => { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { SetCurrentName(); }); };
            this.IsImport = IsImport;

            this._spProductList = spProductList;
            this._ctProductControlTemplate = ctProductControlTemplate;

            // 设置语言
            SetCurrentName();

            InitialProductType();


        }

        /// <summary>
        /// 重新加载页
        /// </summary>
        /// <param name="_element"></param>
        /// <param name="spProductTypeList"></param>
        internal void ReLoadProductsViewModel(Xamarin.Forms.Page _element, Xamarin.Forms.StackLayout spProductTypeList)
        {
            this._element = _element;
            this._spProductTypeList = spProductTypeList;

            // 重新打开产品类型
            foreach (var item in ProductTypeList)
            {
                AddProductTypeItem(item);
            }

        }


        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(long RoomId)
        {
            this.RoomId = RoomId;

            RefreshProduct(true);
        }




        /// <summary>
        /// 初始化产品类型
        /// </summary>
        internal void InitialProductType()
        {
            ProductTypeList.Clear();
            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
            {
                this._spProductTypeList.Children.Clear();
            }));

            IEnumerable<ProductType> models = null;

            if (!IsImport)
                models = Resources.Instance.ProductTypes.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId);
            else
                models = Resources.Instance.ProductTypes.Where(x => x.HideType == 0 || x.HideType == 3).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId);

            ProductTypeList.Add(new ProductTypeStateModel() { ProductType = new ProductType() { ProductTypeId = -1, ProductTypeName0 = Res.Instance.GetString("All", Res.Instance.GetMainLangByMainLangIndex(0).Culture), ProductTypeName1 = Res.Instance.GetString("All", Res.Instance.GetMainLangByMainLangIndex(1).Culture), ProductTypeName2 = Res.Instance.GetString("All", Res.Instance.GetMainLangByMainLangIndex(2).Culture), }, IsSelected = false });

            foreach (var item in models)
            {
                ProductTypeList.Add(new ProductTypeStateModel() { ProductType = item });
            }

            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
            {
                foreach (var item in ProductTypeList)
                {
                    AddProductTypeItem(item);
                }
            }));
        }


        /// <summary>
        /// 加载所有产品
        /// </summary>
        internal void RefreshProduct(bool OnlyRefreshState)
        {

            // 指刷新, 不重新添加(为了平板上不影响切换快速响应)
            if (OnlyRefreshState)
            {
                foreach (var item in ProductList)
                {
                    DetailsModel model = DetectProductIsSelected(item.Product);
                    if (null != model)
                    {
                        item.IsSelected = true;
                        item.DetailsModel = model;
                    }
                    else
                    {
                        item.IsSelected = false;
                        item.DetailsModel = null;
                    }
                }
            }
            else
            {
                ClearList();


                IEnumerable<Product> models = null;

                if (!IsImport)
                    models = Resources.Instance.Products.Where(x => x.HideType == 0 || x.HideType == 2);
                else
                    models = Resources.Instance.Products.Where(x => x.HideType == 0 || x.HideType == 3);


                if (this.ProductTypeId != -1)
                    models = models.Where(x => x.ProductTypeId == this.ProductTypeId);

                models = models.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId);
                foreach (var item in models)
                {
                    ProductStateModel newProductStateModel = new ProductStateModel() { IsImport = IsImport, Product = item, DetectProductIsSelected = DetectProductIsSelected, ProductChange = ProductChange, Operate = Operate };

                    DetailsModel model = DetectProductIsSelected(newProductStateModel.Product);
                    if (null != model)
                    {
                        newProductStateModel.IsSelected = true;
                        newProductStateModel.DetailsModel = model;
                    }
                    else
                    {
                        newProductStateModel.IsSelected = false;
                        newProductStateModel.DetailsModel = null;
                    }


                    AddList(newProductStateModel);

                }
            }


        }


        internal void ClearList() {

            foreach (Xamarin.Forms.TemplatedView item in this._spProductList.Children.Reverse())
            {
                item.BindingContext = null;
                item.IsVisible = false;

                if (!tempTemplateViewList.Contains(item))
                    tempTemplateViewList.Push(item);
            }

            ProductList.Clear();

        }

        internal void AddList(ProductStateModel model) {
      
            AddProducItem(model);
            ProductList.Add(model);

        }


        private Stack<Xamarin.Forms.TemplatedView> tempTemplateViewList = new Stack<Xamarin.Forms.TemplatedView>();
        /// <summary>
        /// 添加产品对象
        /// </summary>
        /// <param name="item"></param>
        private void AddProducItem(ProductStateModel item)
        {
            Xamarin.Forms.TemplatedView view = null;
            if (tempTemplateViewList.Count > 0)
            {
                view = tempTemplateViewList.Pop();
                view.IsVisible = true;
                view.BindingContext = item;
            }
            else
            {
                view = new Xamarin.Forms.TemplatedView();
                view.ControlTemplate = _ctProductControlTemplate;

                view.BindingContext = item;
                _spProductList.Children.Add(view);
            }

        }

        private void RemoveProduct(ProductStateModel item) {
           

            Xamarin.Forms.TemplatedView _view = null;
            foreach (Xamarin.Forms.TemplatedView items in this._spProductList.Children)
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
                    tempTemplateViewList.Push(_view);
                _view.IsVisible = false;
            }

             ProductList.Remove(item);
        }


        internal void ReloadList()
        {
           
            List<ProductStateModel> items = ProductList.ToList();

            ProductList.Clear();

            ProductList = new ObservableCollection<ProductStateModel>();

           
            foreach (var item in items)
            {
                ProductStateModel model = new ProductStateModel();


                model.DetailsModel = DetectProductIsSelected(item.Product);

                model.Product = item.Product;
               
                model.IsSelected = item.IsSelected;
                model.Operate = item.Operate;

                model.Price = item.Price;

                
                model.DetectProductIsSelected = item.DetectProductIsSelected;
                model.ProductChange = item.ProductChange;




                ProductList.Add(model);

            }





            items = null;
        }



        /// <summary>
        /// 加载所有产品
        /// </summary>
        internal void RefreshProduct(List<Product> products)
        {
            // 产品类型全部取消选中
            foreach (var item in ProductTypeList)
            {
                item.IsSelected = false;
            }


            ClearList();


            foreach (var item in products)
            {
                ProductStateModel newProductStateModel = new ProductStateModel() { IsImport = IsImport, Product = item, DetectProductIsSelected = DetectProductIsSelected, ProductChange = ProductChange };

                DetailsModel model = DetectProductIsSelected(newProductStateModel.Product);
                if (null != model)
                {
                    newProductStateModel.IsSelected = true;
                    newProductStateModel.DetailsModel = model;
                }
                else
                {
                    newProductStateModel.IsSelected = false;
                    newProductStateModel.DetailsModel = null;
                }

                AddList(newProductStateModel);
            }

        }




        /// <summary>
        /// 添加产品类型对象
        /// </summary>
        /// <param name="item"></param>
        private void AddProductTypeItem(ProductTypeStateModel item)
        {

            Xamarin.Forms.Button btn = new Xamarin.Forms.Button();

            btn.SetBinding(Xamarin.Forms.Button.TextProperty, new Xamarin.Forms.Binding("ProductTypeName"));
            btn.BackgroundColor = Xamarin.Forms.Color.Transparent;
            btn.Margin = new Xamarin.Forms.Thickness(10, 0);
            btn.BindingContext = item;
            btn.Command = GoProductType;
            btn.CommandParameter = item;
            _spProductTypeList.Children.Add(btn);

        }




        private List<ProductTypeStateModel> ProductTypeList = new List<ProductTypeStateModel>();
        private Xamarin.Forms.StackLayout _spProductTypeList;
        private Xamarin.Forms.StackLayout _spProductList;
        private Xamarin.Forms.ControlTemplate _ctProductControlTemplate;



        private ObservableCollection<ProductStateModel> _productList = new ObservableCollection<ProductStateModel>();
        /// <summary>
        /// 产品列表
        /// </summary>
        public ObservableCollection<ProductStateModel> ProductList
        {
            get { return _productList; }
            set
            {
                _productList = value;
                OnPropertyChanged("ProductList");
            }
        }

        private string _searchKey;
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string SearchKey
        {
            get { return _searchKey; }
            set
            {
                _searchKey = value;
                OnPropertyChanged("SearchKey");
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
        /// 设置语言
        /// </summary>
        private void SetCurrentName()
        {
            // 更改产品类型名字
            foreach (var item in ProductTypeList)
            {
                item.LanguageChange();
            }

            // 更改产品名字
            foreach (var item in ProductList)
            {
                item.LanguageChange();
            }
        }









        /// <summary>
        /// 选择产品类型
        /// </summary>
        private RelayCommand _goProductType;
        public Xamarin.Forms.Command GoProductType
        {
            get
            {
                return _goProductType ?? (_goProductType = new RelayCommand(param =>
                {

                    ProductTypeStateModel model = param as ProductTypeStateModel;
                    if (null == model || model.IsSelected || IsLoading)
                        return;

                   
                    IsLoading = true;
                    SearchKey = "";

                    Task.Run( async () =>
                    {

                        await ExtX.WaitForLoading();

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            
                            foreach (var item in ProductTypeList)
                            {
                                if (model.ProductType == item.ProductType)
                                    item.IsSelected = true;
                                else
                                    item.IsSelected = false;

                            }

                            this.ProductTypeId = model.ProductType.ProductTypeId;

                            // 重新装载产品, 而不只是刷新模型
                            RefreshProduct(false);

                            IsLoading = false;
                        });
                    });

                }));
            }
        }

        /// <summary>
        /// 选择产品类型
        /// </summary>
        private RelayCommand _searchCommand;
        public Xamarin.Forms.Command SearchCommand
        {
            get
            {
                return _searchCommand ?? (_searchCommand = new RelayCommand(() =>
                {
                    this.IsLoading = true;
                    Task.Run(async () =>
                    {
                        await ExtX.WaitForLoading();

                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            if (string.IsNullOrWhiteSpace(SearchKey))
                            {
                                if (this.ProductList.Count > 0)
                                    ClearList();
                            }
                            else
                            {
                                SearchProduct(SearchKey);
                            }
                            IsLoading = false;
                        });
                    });
                }));
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


        /// <summary>
        /// 选择产品类型
        /// </summary>
        private RelayCommand _goX;
        public Xamarin.Forms.Command GoX
        {
            get
            {
                return _goX ?? (_goX = new RelayCommand(param =>
                {
                    ProductStateModel model = param as ProductStateModel;


                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SaveBeforeDelete"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                }));
            }
        }
    }
}
