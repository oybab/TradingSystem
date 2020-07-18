using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.ModelsForViews
{
    public sealed class DetailsModel : ViewModels.ViewModelBase
    {
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

                // 只是为了在UWP中不该显示的删除按钮不显示
                this.State = this.State + 1;
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



        internal bool IsTakeout { get; set; }


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
        /// 是否显示请求
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
                //if (No == -99999)
                //    return 0;
                if (!IsNew)
                    return OrderDetail.TotalPrice;
                else if (NewPrice.HasValue)
                    return Math.Round(NewPrice.Value * Count, 2);
                else
                    return Math.Round(OrderDetail.Price * Count, 2);
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
                if (Mode == 0 && IsNew && Product.PriceChangeMode == 1 && Common.Instance.IsTemporaryChangePrice())
                    return true;
                else if (Mode == 1 && IsNew && Product.PriceChangeMode == 1 && Common.Instance.IsChangeCostPrice())
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
                OnPropertyChanged("State");
                OnPropertyChanged("IsDeleted");
                OnPropertyChanged("IsDeleteProduct");
            }
        }



        /// <summary>
        /// 状态
        /// </summary>
        public bool IsDeleted
        {
            get { return OrderDetail.State == 3; }
            
        }


        /// <summary>
        /// 是否允许删除
        /// </summary>
        public bool IsDeleteProduct
        {
            get { return Common.Instance.IsDeleteProduct(this.IsNew); }

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
        /// 添加数量
        /// </summary>
        private RelayCommand _addCount;
        public Xamarin.Forms.Command AddCount
        {
            get
            {
                return _addCount ?? (_addCount = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;

                    if (model.IsNew && model.Count < 99)
                        model.Count = model.Count + 1;

                    if (model.Count == 0)
                        model.Count = 1;

                    Operate(-1, null);
                }));
            }

        }









        /// <summary>
        /// 修改数量
        /// </summary>
        private RelayCommand _changeCount;
        public Command ChangeCount
        {
            get
            {
                return _changeCount ?? (_changeCount = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;

                    if (!model.IsNew)
                        return;

                    SelectedViewModel viewModel = null;

                    if (Mode == 1)
                        viewModel = NavigationPath.Instance.ImportSelectedPage.BindingContext as SelectedViewModel;
                    else if (!IsTakeout)
                        viewModel = NavigationPath.Instance.SelectedPage.BindingContext as SelectedViewModel;
                    else
                        viewModel = NavigationPath.Instance.TakeoutSelectedPage.BindingContext as SelectedViewModel;

                    viewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            viewModel.ChangeCountView.InitialView(this);

                            viewModel.IsLoading = false;
                        });
                    });

                }));

            }
        }



        /// <summary>
        /// 减少数量
        /// </summary>
        private RelayCommand _subtractCount;
        public Command SubtractCount
        {
            get
            {
                return _subtractCount ?? (_subtractCount = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;

                    if (model.IsNew && model.Count <= 1 && Common.Instance.IsDecreaseProductCount())
                    {
                        if (model.Count == 1)
                            model.Count = -1;
                        else
                            model.Count = model.Count - 1;
                    }
                    if (model.IsNew && model.Count > 1)
                        model.Count = model.Count - 1;

                    Operate(-1, null);
                }));
            }
        }



        /// <summary>
        /// 请求
        /// </summary>
        private RelayCommand _request;
        public Command Request
        {
            get
            {
                return _request ?? (_request = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;

                    if (model.IsNew)
                    {
                        SelectedViewModel viewModel = null;

                        if (Mode == 1)
                            viewModel = NavigationPath.Instance.ImportSelectedPage.BindingContext as SelectedViewModel;
                        else if (!IsTakeout)
                            viewModel = NavigationPath.Instance.SelectedPage.BindingContext as SelectedViewModel;
                        else
                            viewModel = NavigationPath.Instance.TakeoutSelectedPage.BindingContext as SelectedViewModel;

                        viewModel.IsLoading = true;

                        Task.Run(async () =>
                        {

                            await ExtX.WaitForLoading();
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                            {

                                viewModel.RequestView.InitialRequest(this);

                                viewModel.IsLoading = false;
                            });
                        });

                    }

                }));
            }
        }


        /// <summary>
        /// 包装
        /// </summary>
        private RelayCommand _package;
        public Command Package
        {
            get
            {
                return _package ?? (_package = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;


                    if (model.IsNew && model.IsPackage)
                        model.IsPackage = false;
                    else if (model.IsNew)
                        model.IsPackage = true;
                }));
            }
        }



        /// <summary>
        /// 删除
        /// </summary>
        private RelayCommand _delete;
        public Command Delete
        {
            get
            {
                return _delete ?? (_delete = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;


                    if (model.IsNew)
                        Operate(1, model);
                    else
                        Operate(2, model);
                }));
            }
        }





        /// <summary>
        /// 修改改价
        /// </summary>
        private RelayCommand _changePriceCommand;
        public Command ChangePriceCommand
        {
            get
            {
                return _changePriceCommand ?? (_changePriceCommand = new RelayCommand(param =>
                {
                    DetailsModel model = param as DetailsModel;

                    if (null == model)
                        return;

                    if (!model.IsNew)
                        return;

                    SelectedViewModel viewModel = null;

                    if (Mode == 1)
                        viewModel = NavigationPath.Instance.ImportSelectedPage.BindingContext as SelectedViewModel;
                    else if (!IsTakeout)
                        viewModel = NavigationPath.Instance.SelectedPage.BindingContext as SelectedViewModel;
                    else
                        viewModel = NavigationPath.Instance.TakeoutSelectedPage.BindingContext as SelectedViewModel;

                    viewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            viewModel.ChangePriceView.InitialView(this);

                            viewModel.IsLoading = false;
                        });
                    });

                }));
            }

        }

        internal bool IsOnlyOrder = false;
    }
}
