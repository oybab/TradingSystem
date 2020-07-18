using Oybab.DAL;
using Oybab.Res.Server;
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
    public sealed class ProductsViewModel : ViewModelBase
    {
        private UIElement _element;
        private StackPanel _spProductTypeList;
        private ScrollViewer _scrollViewerProducts;

        private long ProductTypeId = -1;
        internal Func<bool, long, DetailsModel> ProductChange;
        internal Func<Product, DetailsModel> DetectProductIsSelected;
        internal Action<int, DetailsModel> Operate { get; set; }
        private long RoomId = -1;
        private bool IsImport = false;

        private Style productTypeStyle;

        // 数据
        private List<ProductTypeStateModel> resultListType = new List<ProductTypeStateModel>();



        internal ProductsViewModel(UIElement element, StackPanel spProductTypeList, ScrollViewer scrollViewerProducts, bool IsImport = false)
        {
            this._element = element;
            this._spProductTypeList = spProductTypeList;
            this._scrollViewerProducts = scrollViewerProducts;
            this.IsImport = IsImport;

            productTypeStyle = (_element as FrameworkElement).FindResource("cbProductTypeStyle") as Style;


            InitialProductType();


            Notification.Instance.NotificationLanguage += (obj, value, args) => { _element.Dispatcher.BeginInvoke(new Action(() => { SetCurrentName(); })); };

        }


        /// <summary>
        /// 重新设置当前语言
        /// </summary>
        private void SetCurrentName()
        {
            // 更改产品类型名字
            foreach (var item in resultListType)
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
            resultListType.Clear();
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                this._spProductTypeList.Children.Clear();
            }));

            IEnumerable<ProductType> models = null;

            if (!IsImport)
                models = Resources.GetRes().ProductTypes.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId);
            else
                models = Resources.GetRes().ProductTypes.Where(x => x.HideType == 0 || x.HideType == 3).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId);

            resultListType.Add(new ProductTypeStateModel() { ProductType = new ProductType() { ProductTypeId = -1, ProductTypeName0 = Resources.GetRes().GetString("All", Resources.GetRes().GetMainLangByMainLangIndex(0).Culture), ProductTypeName1 = Resources.GetRes().GetString("All", Resources.GetRes().GetMainLangByMainLangIndex(1).Culture), ProductTypeName2 = Resources.GetRes().GetString("All", Resources.GetRes().GetMainLangByMainLangIndex(2).Culture), }, IsSelected = false });

            foreach (var item in models)
            {
                resultListType.Add(new ProductTypeStateModel() { ProductType = item });
            }

            
            foreach (var item in resultListType)
            {
                AddProductTypeItem(item);
            }
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


                ProductList.Clear();



                IEnumerable<Product> models = null;

                if (!IsImport)
                    models = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2);
                else
                    models = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 3);

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


                    ProductList.Add(newProductStateModel);

                }

            }


        }





        /// <summary>
        /// 加载所有产品
        /// </summary>
        internal void RefreshProduct(List<Product> products)
        {
            // 产品类型全部取消选中
            foreach (var item in resultListType)
            {
                item.IsSelected = false;
            }


            ProductList.Clear();

            foreach (var item in products)
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


                ProductList.Add(newProductStateModel);

            }


               
        }






        /// <summary>
        /// 添加产品类型对象
        /// </summary>
        /// <param name="item"></param>
        private void AddProductTypeItem(ProductTypeStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = productTypeStyle;
                btn.DataContext = item;
                btn.Command = ProductType;
                btn.CommandParameter = item;
                _spProductTypeList.Children.Add(btn);
            }));
        }




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





        /// <summary>
        /// 打开产品列表
        /// </summary>
        private RelayCommand _ProductType;
        public ICommand ProductType
        {
            get
            {
                if (_ProductType == null)
                {
                    _ProductType = new RelayCommand(param =>
                    {
                        ProductTypeStateModel model = param as ProductTypeStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }



                        // 已选的就算了
                        if (model.IsSelected)
                            return;

                        foreach (var item in resultListType)
                        {
                            if (model.ProductType == item.ProductType)
                                item.IsSelected = true;
                            else
                                item.IsSelected = false;

                        }

                        this.ProductTypeId = model.ProductType.ProductTypeId;

                        // 重新装载产品, 而不只是刷新模型
                        RefreshProduct(false);

                        
                    });
                }
                return _ProductType;
            }
        }

        // 移动间隔
        private int MovePositionInterval = 200;

        /// <summary>
        /// 打开产品列表往左
        /// </summary>
        private RelayCommand _turnLeft;
        public ICommand TurnLeft
        {
            get
            {
                if (_turnLeft == null)
                {
                    _turnLeft = new RelayCommand(param =>
                    {
                         if (_spProductTypeList.ActualWidth > 0)
                         {
                             ScrollViewer parent = _spProductTypeList.Parent as ScrollViewer;

                             double left = parent.HorizontalOffset - MovePositionInterval;

                             if (left < 0)
                                 left = 0;

                             _element.Dispatcher.BeginInvoke(new Action(() =>
                             {
                                 parent.ScrollToHorizontalOffset(left);
                             }));
                         }
                    });
                }
                return _turnLeft;
            }
        }




        /// <summary>
        /// 打开产品列表往右
        /// </summary>
        private RelayCommand _turnRight;
        public ICommand TurnRight
        {
            get
            {
                if (_turnRight == null)
                {
                    _turnRight = new RelayCommand(param =>
                    {

                        ScrollViewer parent = _spProductTypeList.Parent as ScrollViewer;
                       




                        double left = parent.HorizontalOffset + MovePositionInterval;

                        if (left >= parent.ScrollableWidth + 100) // 120 为转左和转右按钮宽度
                            left = parent.ScrollableWidth + 100;

                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            parent.ScrollToHorizontalOffset(left);
                        }));
                    });
                }
                return _turnRight;
            }
        }




        private string _alertMsg = "";
        /// <summary>
        /// 显示内容
        /// </summary>
        public string AlertMsg
        {
            get { return _alertMsg; }
            set
            {
                _alertMsg = value;
                OnPropertyChanged("AlertMsg");
            }
        }



        private bool _alertMsgMode = false;
        /// <summary>
        /// 是否显示模式
        /// </summary>
        public bool AlertMsgMode
        {
            get { return _alertMsgMode; }
            set
            {
                _alertMsgMode = value;
                OnPropertyChanged("AlertMsgMode");
            }
        }


        private int _alertMsgImageMode = 0;
        /// <summary>
        /// 显示图类型
        /// </summary>
        public int AlertMsgImageMode
        {
            get { return _alertMsgImageMode; }
            set
            {
                _alertMsgImageMode = value;
                OnPropertyChanged("AlertMsgImageMode");
            }
        }



        private int _alertMsgButtonMode = 0;
        /// <summary>
        /// 显示按钮类型
        /// </summary>
        public int AlertMsgButtonMode
        {
            get { return _alertMsgButtonMode; }
            set
            {
                _alertMsgButtonMode = value;
                OnPropertyChanged("AlertMsgButtonMode");
            }
        }

    }
}
