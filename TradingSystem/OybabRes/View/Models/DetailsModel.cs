using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.Models
{
    public sealed class DetailsModel : ViewModels.ViewModelBase
    {
        internal UIElement _element;
        internal Product Product { get; set; }
        internal OrderDetail OrderDetail { get; set; }

        internal Action<int, DetailsModel> Operate { get; set; }

        internal int Mode { get; set; } = 0; // 0为Order和Takeout, 1为Import



        private string _productName;
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName
        {
            get
            {

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



        private string _addTime = "";
        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddTime
        {
            get { return _addTime; }
            set
            {
                _addTime = value;
                OnPropertyChanged("AddTime");
            }
        }



        private bool _isNew = false;
        /// <summary>
        /// 是否新的
        /// </summary>
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                _isNew = value;
                OnPropertyChanged("IsNew");
            }
        }


        /// <summary>
        /// 是否允许删除
        /// </summary>
        public bool IsDeleteProduct
        {
            get { return Common.GetCommon().IsDeleteProduct(this.IsNew); }

        }


        private bool _isRequest = false;
        /// <summary>
        /// 是否新的
        /// </summary>
        public bool IsRequest
        {
            get { return !string.IsNullOrWhiteSpace(this.OrderDetail.Request); }
            set
            {
                _isRequest = value;
                OnPropertyChanged("IsRequest");
            }
        }



        private bool _isPackage = false;
        /// <summary>
        /// 是否包装
        /// </summary>
        public bool IsPackage
        {
            get { return Convert.ToBoolean(this.OrderDetail.IsPack); }
            set
            {
                _isPackage = value;
                this.OrderDetail.IsPack = Convert.ToInt64(value);
                OnPropertyChanged("IsPackage");
            }
        }



        private bool _isShowRequest = false;
        /// <summary>
        /// 是否包装
        /// </summary>
        public bool IsShowRequest
        {
            get { return Mode == 0; }
            set
            {
                _isShowRequest = value;
                OnPropertyChanged("IsShowRequest");
            }
        }



        private double _count = 0;
        /// <summary>
        /// 数量
        /// </summary>
        public double Count
        {
            get { return OrderDetail.Count; }
            set
            {
                _count = value;
                OrderDetail.Count = value;
                OnPropertyChanged("Count");
                CountStr = _count.ToString("0.###");
                TotalPrice = Math.Round(OrderDetail.Price * value, 2);
            }
        }



        private string _countStr = "0";
        /// <summary>
        /// 数量
        /// </summary>
        public string CountStr
        {
            get { return Count.ToString("0.###"); }
            set
            {
                _countStr = value;
                OnPropertyChanged("CountStr");
            }
        }



        private double _totalPrice = 0;
        /// <summary>
        /// 总价
        /// </summary>
        public double TotalPrice
        {
            get
            {
                if (!IsNew)
                    return OrderDetail.TotalPrice;
                else if (NewPrice.HasValue)
                    return Math.Round(NewPrice.Value * Count, 2);
                else
                    return Math.Round(OrderDetail.Price * Count,2);
            }
            set
            {
                _totalPrice = value;
                OrderDetail.TotalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
        }


        private double? _newPrice = null;
        /// <summary>
        /// 总价
        /// </summary>
        public double? NewPrice
        {
            get { return _newPrice; }
            set
            {
                _newPrice = value;
                OnPropertyChanged("NewPrice");
                OnPropertyChanged("TotalPrice");
            }
        }


        private bool _isChangePrice;
        /// <summary>
        /// 是否可修改价格
        /// </summary>
        public bool IsChangePrice
        {
            get
            {
                if (Mode == 0 && IsNew && Product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                    return true;
                else if (Mode == 1 && IsNew && Product.PriceChangeMode == 1 && Common.GetCommon().IsChangeCostPrice())
                    return true;
                else
                    return false;
            }
            set
            {
                _isChangePrice = value;
                OnPropertyChanged("IsChangePrice");
            }
        }





        private long _state = 0;
        /// <summary>
        /// 状态
        /// </summary>
        public long State
        {
            get { return OrderDetail.State; }
            set
            {
                _state = value;
                OrderDetail.State = value;
                OnPropertyChanged("State");
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
        /// 添加数量
        /// </summary>
        private RelayCommand _addCount;
        public ICommand AddCount
        {
            get
            {
                if (_addCount == null)
                {
                    _addCount = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }


                        if (model.IsNew && model.Count < 99)
                            model.Count = model.Count + 1;

                        if (model.Count == 0)
                            model.Count = 1;

                        Operate(-1, null);
                    });
                }
                return _addCount;
            }
        }









        /// <summary>
        /// 修改数量
        /// </summary>
        private RelayCommand _changeCount;
        public ICommand ChangeCount
        {
            get
            {
                if (_changeCount == null)
                {
                    _changeCount = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        // 是否长按(大于1秒)
                        else if (model.IsLong)
                        {
                            model.IsLong = false;

                            _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.ChangeCount, model));

                            return;
                        }


                    

                     
                    });
                }
                return _changeCount;
            }
        }



        /// <summary>
        /// 减少数量
        /// </summary>
        private RelayCommand _subtractCount;
        public ICommand SubtractCount
        {
            get
            {
                if (_subtractCount == null)
                {
                    _subtractCount = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsNew && model.Count <= 1 && Common.GetCommon().IsDecreaseProductCount())
                        {
                            if (model.Count == 1)
                                model.Count = -1;
                            else
                                model.Count = model.Count - 1;
                        }
                        if (model.IsNew && model.Count > 1)
                            model.Count = model.Count - 1;


                        

                        Operate(-1, null);
                    });
                }
                return _subtractCount;
            }
        }



        /// <summary>
        /// 请求
        /// </summary>
        private RelayCommand _request;
        public ICommand Request
        {
            get
            {
                if (_request == null)
                {
                    _request = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }


                        if (model.IsNew)
                        {
                            _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null , BoxType.Request, model));
                        }
                    });
                }
                return _request;
            }
        }


        /// <summary>
        /// 包装
        /// </summary>
        private RelayCommand _package;
        public ICommand Package
        {
            get
            {
                if (_package == null)
                {
                    _package = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }


                        if (model.IsNew && model.IsPackage)
                            model.IsPackage = false;
                        else if (model.IsNew)
                            model.IsPackage = true;
                    });
                }
                return _package;
            }
        }



        /// <summary>
        /// 删除
        /// </summary>
        private RelayCommand _delete;
        public ICommand Delete
        {
            get
            {
                if (_delete == null)
                {
                    _delete = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }


                        if (model.IsNew)
                            Operate(1, model);
                        else
                            Operate(2, model);
                    });
                }
                return _delete;
            }
        }





        /// <summary>
        /// 修改改价
        /// </summary>
        private RelayCommand _changePriceCommand;
        public ICommand ChangePriceCommand
        {
            get
            {
                if (_changePriceCommand == null)
                {
                    _changePriceCommand = new RelayCommand(param =>
                    {
                        DetailsModel model = param as DetailsModel;

                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.ChangePrice, model));
                        
                    });
                }
                return _changePriceCommand;
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

        private double _unitPrice = 0;
        /// <summary>
        /// 单价
        /// </summary>
        public double UnitPrice
        {
            get
            {
                if (NewPrice.HasValue)
                    return NewPrice.Value;
                else
                    return OrderDetail.Price;
            }
            set
            {
                _unitPrice = value;
                if (value == OrderDetail.Price)
                {
                    NewPrice = null;
                }
                else
                {
                    NewPrice = value;
                }
                OnPropertyChanged("UnitPrice");
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



        private string _countPos = null;
        /// <summary>
        /// 数量
        /// </summary>
        public string CountPos
        {
            get { return (null != _countPos) ? _countPos : Count.ToString(); }
            set
            {
                _countPos = value;
                OnPropertyChanged("CountPos");

                if (null != _countPos)
                    Count = double.Parse(_countPos);
            }
        }




        private string _unitPricePos = null;
        /// <summary>
        /// 单价
        /// </summary>
        public string UnitPricePos
        {
            get { return (null != _unitPricePos) ? _unitPricePos : UnitPrice.ToString(); }
            set
            {
                _unitPricePos = value;
                OnPropertyChanged("UnitPricePos");

                if (null != _unitPricePos)
                    UnitPrice = double.Parse(_unitPricePos);
            }
        }



        internal bool IsOnlyOrder = false;


    }
}
