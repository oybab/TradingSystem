using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.TradingSystemX.VM.ModelsForViews
{
    public sealed class ProductStateModel : ViewModelBase
    {
        internal Product Product { get; set; }
        internal Func<bool, long, DetailsModel> ProductChange;
        internal Func<Product, DetailsModel> DetectProductIsSelected;
        internal Action<int, DetailsModel> Operate { get; set; }

        private DetailsModel _detailsModel = null;

        internal bool IsImport { get; set; } = false;
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

                if (Res.Instance.MainLangIndex == 0)
                    return Product.ProductName0;
                else if (Res.Instance.MainLangIndex == 1)
                    return Product.ProductName1;
                else if (Res.Instance.MainLangIndex == 2)
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
            get
            {
                if (!IsImport)
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



        private string _countStr = "0";
        /// <summary>
        /// 数量(测试用)
        /// </summary>
        public string CountStr
        {
            get { return null == DetailsModel ? "0" : DetailsModel.CountStr; }
            set
            {
                _countStr = value;
                OnPropertyChanged("CountStr");
            }
        }


        private bool DisPlayImage = false;
      




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
        /// 选择产品
        /// </summary>
        private RelayCommand _goProduct;
        public Xamarin.Forms.Command GoProduct
        {
            get
            {
                return _goProduct ?? (_goProduct = new RelayCommand(param =>
                {

                    ProductStateModel model = param as ProductStateModel;
                    if (null == model)
                        return;

                    

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

                }));
            }
        }




        /// <summary>
        /// 增加产品
        /// </summary>
        private RelayCommand _addProduct;
        public Xamarin.Forms.Command AddProduct
        {
            get
            {
                return _addProduct ?? (_addProduct = new RelayCommand(param =>
                {

                    ProductStateModel model = param as ProductStateModel;
                    if (null == model)
                        return;



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

                }));
            }
        }




        /// <summary>
        /// 减少产品
        /// </summary>
        private RelayCommand _removeProduct;
        public Xamarin.Forms.Command RemoveProduct
        {
            get
            {
                return _removeProduct ?? (_removeProduct = new RelayCommand(param =>
                {

                    ProductStateModel model = param as ProductStateModel;
                    if (null == model)
                        return;



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

                }));
            }
        }







    }
}
