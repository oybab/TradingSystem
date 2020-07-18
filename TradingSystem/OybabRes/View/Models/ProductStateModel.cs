using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oybab.Res.View.Models
{
    public sealed class ProductStateModel : ViewModelBase
    {
        internal Product Product { get; set; }
        internal Func<bool, long, DetailsModel> ProductChange;
        internal Func<Product, DetailsModel> DetectProductIsSelected;
        internal Action<int, DetailsModel> Operate { get; set; }

        internal bool IsImport { get; set; } = false;


        private DetailsModel _detailsModel = null;
        /// <summary>
        /// 详细
        /// </summary>
        public DetailsModel DetailsModel
        {
            get { return _detailsModel; }
            set
            {
                _detailsModel = value;
                OnPropertyChanged("DetailsModel");
            }
        }


        private string _productName;
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName
        {
            get {

                if (Resources.GetRes().MainLangIndex == 0)
                    return Product.ProductName0;
                else if (Resources.GetRes().MainLangIndex == 1)
                    return Product.ProductName1;
                else if (Resources.GetRes().MainLangIndex == 2)
                    return Product.ProductName2;
                else
                    return "";
            }
            set
            {
                _productName = value;
                OnPropertyChanged("ProductName");
            }
        }


        /// <summary>
        /// 语言变了
        /// </summary>
        internal void LanguageChange()
        {
            OnPropertyChanged("ProductName");
        }



        private bool _isSelected = false;
        /// <summary>
        /// 选中状态
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }




        private double _price = 0;
        /// <summary>
        /// 价格
        /// </summary>
        public double Price
        {
            get { if (!IsImport)
                    return Product.Price;
                else
                    return Product.CostPrice;
            }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }




        private bool DisPlayImage = true;
        

        /// <summary>
        /// 获取图片次数(用来防止频繁获取错误连接)
        /// </summary>
        private int GetImageCount { get; set; }



        private string _imagePath;
        /// <summary>
        /// 图片
        /// </summary>
        public string ImagePath
        {
            get
            {
                if (DisPlayImage)
                {
                    if (null != _imagePath)
                    {
                        return _imagePath;
                    }
                    else
                    {
                        if (GetImageCount >= 2)
                            return null;

                        else if (string.IsNullOrWhiteSpace(Product.ImageName))
                        {
                            GetImageCount = 2;
                            return null;
                        }

                        new Action(() =>
                        {
                            ++GetImageCount;
                            string img = ImageCache.GetImage(Path.Combine(Resources.GetRes().ROOT, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().PRODUCTS_FOLDER, Product.ImageName), Product.ImageName);
                            if (null != img)
                            {
                                ImagePath = img;
                            }
                        }).BeginInvoke(null, null);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                _imagePath = value;
                OnPropertyChanged("ImagePath");
            }
        }






        /// <summary>
        /// 打开产品
        /// </summary>
        private RelayCommand _goProduct;
        public ICommand GoProduct
        {
            get
            {
                if (_goProduct == null)
                {
                    _goProduct = new RelayCommand(param =>
                    {
                        ProductStateModel model = param as ProductStateModel;
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

                        if (model.IsSelected && null != DetectProductIsSelected(model.Product))
                        {
                            model.IsSelected = false;
                            ProductChange(model.IsSelected, model.Product.ProductId);
                        }
                        else
                        {
                          
                            model.IsSelected = true;
                            ProductChange(model.IsSelected, model.Product.ProductId);
                        }



                    });
                }
                return _goProduct;
            }
        }





        /// <summary>
        /// 添加产品
        /// </summary>
        private RelayCommand _addProduct;
        public ICommand AddProduct
        {
            get
            {
                if (_addProduct == null)
                {
                    _addProduct = new RelayCommand(param =>
                    {
                        ProductStateModel model = param as ProductStateModel;
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

                        if (model.IsSelected && null != DetailsModel)
                        {
                            if (DetailsModel.Count < 99)
                            {
                                ++model.DetailsModel.Count;
                                Operate(-1, null);
                            }
                        }
                        else
                        {
                            model.IsSelected = true;
                            DetailsModel = ProductChange(model.IsSelected, model.Product.ProductId);
                        }



                    });
                }
                return _addProduct;
            }
        }




        /// <summary>
        /// 减少产品
        /// </summary>
        private RelayCommand _removeProduct;
        public ICommand RemoveProduct
        {
            get
            {
                if (_removeProduct == null)
                {
                    _removeProduct = new RelayCommand(param =>
                    {
                        ProductStateModel model = param as ProductStateModel;
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

                        if (model.IsSelected && null != DetailsModel && DetailsModel.Count > 1)
                        {
                            --model.DetailsModel.Count;
                            Operate(-1, null);
                        }
                        else
                        {
                            model.IsSelected = false;
                            DetailsModel = ProductChange(model.IsSelected, model.Product.ProductId);
                        }



                    });
                }
                return _removeProduct;
            }
        }










        // POS需要用的数据

        private bool _isNavigated = false;
        /// <summary>
        /// 是不是定位到当前数据
        /// </summary>
        public bool IsNavigated
        {
            get { return _isNavigated; }
            set
            {
                _isNavigated = value;
                OnPropertyChanged("IsNavigated");
            }
        }


        private int _navigateMode = 0;
        /// <summary>
        /// 导航定位模式(0数量,1单价)
        /// </summary>
        public int NavigateMode
        {
            get { return _navigateMode; }
            set
            {
                _navigateMode = value;
                OnPropertyChanged("NavigateMode");
            }
        }



        private int _no = 0;
        /// <summary>
        /// 编号
        /// </summary>
        public int No
        {
            get { return _no; }
            set
            {
                _no = value;
                OnPropertyChanged("No");
            }
        }



    }
}
