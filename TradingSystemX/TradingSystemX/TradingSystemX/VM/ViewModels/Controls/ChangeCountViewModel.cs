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
    internal sealed class ChangeCountViewModel : ViewModelBase
    {
        private Xamarin.Forms.View _element;
        private DetailsModel model;
        private SelectedViewModel SelectedViewModel;
        private Action Recalc;

        internal ChangeCountViewModel(Xamarin.Forms.View element, SelectedViewModel SelectedViewModel, Action Recalc)
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

            OldCount = model.OrderDetail.Count.ToString();

            NewCount = model.OrderDetail.Count.ToString();

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
        /// 修改数量
        /// </summary>
        public void ChangeCount()
        {
            if (this.NewCount == "")
                this.NewCount = "0";
            else
            {
                double count = 0;
                if (!double.TryParse(NewCount, out count))
                {
                    this.NewCount = "0";
                }

                if (count > 9999)
                {
                    count = 9999;
                    this.NewCount = "9999";
                }


                if (!NewCount.EndsWith("."))
                    this.NewCount = Math.Round(count, 3).ToString("0.###");
            }

        }


        













        private string _oldCount = "0";
        /// <summary>
        /// 老数量
        /// </summary>
        public string OldCount
        {
            get { return _oldCount; }
            set
            {
                _oldCount = value;
                OnPropertyChanged("OldCount");
            }
        }



        private string _newCount = "0";
        /// <summary>
        /// 新数量
        /// </summary>
        public string NewCount
        {
            get { return _newCount; }
            set
            {
                _newCount = value;
                OnPropertyChanged("NewCount");
            }
        }


        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,3})?$");
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
                    if (NewCount == "")
                    {
                        NewCount = "0";
                        return;
                    }
                    if (!match.IsMatch(NewCount))
                    {
                        NewCount = OldCount;
                        return;
                    }

                    SelectedViewModel.IsLoading = true;
                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            if (OldCount != NewCount)
                            {
                                model.Count = double.Parse(NewCount);
                            }
                            else
                            {
                                model.Count = double.Parse(NewCount);
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
