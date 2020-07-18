using Oybab.DAL;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Oybab.TradingSystemX.Tools;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class ChangePriceViewModel : ViewModelBase
    {
        private Xamarin.Forms.View _element;
        private DetailsModel model;
        private SelectedViewModel SelectedViewModel;
        private Action Recalc;

        internal ChangePriceViewModel(Xamarin.Forms.View element, SelectedViewModel SelectedViewModel, Action Recalc)
        {
            this._element = element;
            this.SelectedViewModel = SelectedViewModel;
            this.Recalc = Recalc;
        }



        /// <summary>
        /// 初始化需求
        /// </summary>
        internal void InitialView(DetailsModel model)
        {
            this.model = model;

            OldPrice = model.OrderDetail.Price.ToString();

            if (model.NewPrice.HasValue)
                NewPrice = model.NewPrice.Value.ToString();
            else
                NewPrice = model.OrderDetail.Price.ToString();

            IsShow = true;
        }





        private bool _isShow = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                if (_isShow == true)
                    NavigationPath.Instance.OpenPanel();
                OnPropertyChanged("IsShow");
            }
        }


        /// <summary>
        /// 修改价格
        /// </summary>
        public void ChangePrice()
        {
            if (this.NewPrice == "")
                this.NewPrice = "0";
            else
            {
                double price = 0;
                if (!double.TryParse(NewPrice, out price))
                {
                    this.NewPrice = "0";
                }

                if (!NewPrice.EndsWith("."))
                    this.NewPrice = Math.Round(price, 2).ToString();
            }

        }
















        private string _oldPrice = "0";
        /// <summary>
        /// 老价格
        /// </summary>
        public string OldPrice
        {
            get { return _oldPrice; }
            set
            {
                _oldPrice = value;
                OnPropertyChanged("OldPrice");
            }
        }



        private string _newPrice = "0";
        /// <summary>
        /// 新价格
        /// </summary>
        public string NewPrice
        {
            get { return _newPrice; }
            set
            {
                _newPrice = value;
                OnPropertyChanged("NewPrice");
            }
        }


        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");
        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public Command OKCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand(param =>
                {
                    if (NewPrice == "")
                    {
                        NewPrice = "0";
                        return;
                    }
                    if (!match.IsMatch(NewPrice))
                    {
                        NewPrice = OldPrice;
                        return;
                    }


                    SelectedViewModel.IsLoading = true;
                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            if (OldPrice != NewPrice)
                            {
                                model.NewPrice = double.Parse(NewPrice);
                            }
                            else
                            {
                                model.NewPrice = null;
                            }

                            if (null != Recalc)
                                Recalc();

                            // 关闭面板
                            NavigationPath.Instance.ClosePanels(false);

                            IsShow = false;
                            SelectedViewModel.IsLoading = false;
                        });
                    });
                }));
            }
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public Command CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param =>
                {
                    SelectedViewModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            // 关闭面板
                            NavigationPath.Instance.ClosePanels(false);

                            this.IsShow = false;
                           
                            SelectedViewModel.IsLoading = false;

                        });
                    });
                }));
            }
        }






    }
}
