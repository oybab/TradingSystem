using Oybab.DAL;
using Oybab.Res.Server;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Converters;
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

namespace Oybab.Res.View.ViewModels.Pos.Controls
{
    public sealed class ProductsViewModel : ViewModelBase
    {
        private UIElement _element;


        internal Func<bool, long, DetailsModel> ProductChange;
        internal Func<Product, DetailsModel> DetectProductIsSelected;
        internal Action<int, DetailsModel> Operate { get; set; }
        private long RoomId = -1;




        internal ProductsViewModel(UIElement element)
        {
            this._element = element;

            Notification.Instance.NotificationLanguage += (obj, value, args) => { _element.Dispatcher.BeginInvoke(new Action(() => { SetCurrentName(); })); };

        }


        /// <summary>
        /// 重新设置当前语言
        /// </summary>
        private void SetCurrentName()
        {
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



                IEnumerable<Product> models = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2);


                models = models.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId);
                foreach (var item in models)
                {
                    ProductStateModel newProductStateModel = new ProductStateModel() { Product = item, DetectProductIsSelected = DetectProductIsSelected, ProductChange = ProductChange, Operate = Operate };

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

            ProductList.Clear();

            foreach (var item in products)
            {
                ProductStateModel newProductStateModel = new ProductStateModel() { Product = item, DetectProductIsSelected = DetectProductIsSelected, ProductChange = ProductChange, Operate = Operate };

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

            ResetPage();


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




        private ObservableCollection<ProductStateModel> _productListNew = new ObservableCollection<ProductStateModel>();
        /// <summary>
        /// 产品列表
        /// </summary>
        public ObservableCollection<ProductStateModel> ProductListNew
        {
            get { return _productListNew; }
            set
            {
                _productListNew = value;
                OnPropertyChanged("ProductListNew");
            }
        }

        private int _currentPage = 0;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }


        private int _totalPage = 0;
        /// <summary>
        /// 总页
        /// </summary>
        public int TotalPage
        {
            get { return _totalPage; }
            set
            {
                _totalPage = value;
                OnPropertyChanged("TotalPage");
            }
        }


        private int _currentIndex = -1;
        /// <summary>
        /// 当前模型索引
        /// </summary>
        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                _currentIndex = value;
                OnPropertyChanged("CurrentIndex");
            }
        }

        /// <summary>
        /// 重置编号
        /// </summary>
        private void ResetNo()
        {
            int no = 1;

            int page = 0;

            if (CurrentPage < TotalPage)
            {
                page = (TotalPage - 1 - CurrentPage) * PosLine.ListCountProduct + (ProductList.Count % PosLine.ListCountProduct);
            }

            for (int i = ProductListNew.Count; i > 0; i--)
            {
                ProductListNew[i - 1].No = no + page;
                ++no;
            }
        }

        /// <summary>
        /// 下一页
        /// </summary>
        private void GoNextPage()
        {
            if (CurrentPage < TotalPage)
            {
                ++CurrentPage;
                ReloadNewList();
            }

        }

        /// <summary>
        /// 上一页
        /// </summary>
        private void GoLastPage()
        {
            if (1 < CurrentPage)
            {
                --CurrentPage;
                ReloadNewList();
            }
        }

        /// <summary>
        /// 重置页
        /// </summary>
        internal void ResetPage()
        {
            CurrentPage = 1;
            CalcTotalPage();
            ReloadNewList();
        }

        /// <summary>
        /// 计算并重置页
        /// </summary>
        internal void CalcAndResetPage()
        {
            CalcTotalPage();
            if (CurrentPage > TotalPage)
            {
                CurrentPage = TotalPage;
                ReloadNewList();
            }
            ResetNo();
        }

        /// <summary>
        /// 计算页
        /// </summary>
        private void CalcTotalPage()
        {
            TotalPage = (int)((ProductList.Count - 1 + PosLine.ListCountProduct) / PosLine.ListCountProduct);
        }

        /// <summary>
        /// 选择下一个
        /// </summary>
        private void GoDown()
        {
            if (ProductListNew.Count - 1 > CurrentIndex)
            {
                ProductListNew[CurrentIndex].IsNavigated = false;

                ++CurrentIndex;
                ProductListNew[CurrentIndex].IsNavigated = true;
                ProductListNew[CurrentIndex].NavigateMode = 0;
            }
        }

        /// <summary>
        /// 选择上一个
        /// </summary>
        private void GoUp()
        {
            if (CurrentIndex > 0 && ProductListNew.Count > 0)
            {
                ProductListNew[CurrentIndex].IsNavigated = false;

                --CurrentIndex;
                ProductListNew[CurrentIndex].IsNavigated = true;
                ProductListNew[CurrentIndex].NavigateMode = 0;
            }
        }

        

        /// <summary>
        /// 重新加载列表
        /// </summary>
        private void ReloadNewList()
        {
            ProductListNew.Clear();
            foreach (var item in ProductList.Skip(PosLine.ListCountProduct * (CurrentPage - 1)).Take(PosLine.ListCountProduct))
            {
                item.IsNavigated = false;
                item.NavigateMode = 0;
                ProductListNew.Add(item);
            }

            if (ProductListNew.Count > 0)
            {
                CurrentIndex = 0;
                ProductListNew[CurrentIndex].IsNavigated = true;
                ProductListNew[CurrentIndex].NavigateMode = 0;
            }
            CalcAndResetPage();
        }

        /// <summary>
        /// 处理KEY
        /// </summary>
        /// <param name="args"></param>
        internal void HandleKey(KeyEventArgs args)
        {
            // 如果是功能(如上下左右,换页)
            if (args.Key >= Key.PageUp && args.Key <= Key.Down)
            {
                switch (args.Key)
                {
                    case Key.Up:
                        GoUp();
                        break;
                    case Key.Down:
                        GoDown();
                        break;
                    case Key.PageUp:
                        GoLastPage();
                        break;
                    case Key.PageDown:
                        GoNextPage();
                        break;
                }
            }
            // 如果要增加数量或减少数量
            else if (args.Key == Key.Add || args.Key == Key.Subtract || args.Key == Key.Delete || args.Key == Key.Enter)
            {
                if (ProductListNew.Count > 0 && ProductListNew[CurrentIndex].NavigateMode == 0)
                {
                    if (ProductListNew[CurrentIndex].IsSelected)
                    {
                        if (args.Key == Key.Delete)
                        {
                            ProductListNew[CurrentIndex].DetailsModel.Count = 1;
                            ProductListNew[CurrentIndex].RemoveProduct.Execute(ProductListNew[CurrentIndex]);
                        }
                        else
                        {
                            double count = double.Parse(ProductListNew[CurrentIndex].DetailsModel.CountPos);

                            if (args.Key == Key.Add && count < 99999999)
                                ProductListNew[CurrentIndex].DetailsModel.CountPos = (++count).ToString();
                            else if (args.Key == Key.Subtract)
                            {
                                if (count > 1)
                                {
                                    ProductListNew[CurrentIndex].DetailsModel.CountPos = (--count).ToString();
                                }
                                else
                                {
                                    ProductListNew[CurrentIndex].DetailsModel.Count = 1;
                                    ProductListNew[CurrentIndex].RemoveProduct.Execute(ProductListNew[CurrentIndex]);
                                }
                            }
                                
                        }
                    }else if (!ProductListNew[CurrentIndex].IsSelected)
                    {
                        if (args.Key == Key.Enter || args.Key == Key.Add)
                        {
                            ProductListNew[CurrentIndex].AddProduct.Execute(ProductListNew[CurrentIndex]);
                        }
                    }
                }
                    
            }
            


        }





        private string _searchText;
        /// <summary>
        /// 搜索内容
        /// </summary>
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
            }
        }

        internal void HandleSearchKey(KeyEventArgs args)
        {
            // 如果是改动数字
            if ((args.Key >= Key.D0 && args.Key <= Key.D9) || (args.Key >= Key.NumPad0 && args.Key <= Key.NumPad9) || (args.Key >= Key.A && args.Key <= Key.Z) || args.Key == Key.Back)
            {


                if (args.Key == Key.Back)
                {
                    if (SearchText.Length > 0)
                        SearchText = SearchText.Remove(SearchText.Length - 1);
                    args.Handled = true;
                }
                else
                {


                }

            }
        }



        public void HandleSearchKey(TextCompositionEventArgs args)
        {
           
            if (SearchText.Length < 32)
                SearchText += args.Text;
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
