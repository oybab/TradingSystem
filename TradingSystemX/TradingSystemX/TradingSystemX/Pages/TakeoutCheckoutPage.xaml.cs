using Oybab.TradingSystemX.VM.ViewModels.Pages;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class TakeoutCheckoutPage : ContentPage
    {
        private TakeoutCheckoutViewModel viewModel;

        public TakeoutCheckoutPage()
		{
            InitializeComponent();

            viewModel = new TakeoutCheckoutViewModel(null, GetBalanceContent(), GetBalanceTemplate());
            
            this.BindingContext = viewModel;


            lbPaidPrice.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {

                    TakeoutCheckoutViewModel viewModel = this.BindingContext as TakeoutCheckoutViewModel;
                    viewModel.FinishPaidPrice();
                }

               )
            });


        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="obj"></param>
        internal void Init(object obj)
        {
            viewModel.Init(obj);
        }



        private StackLayout GetBalanceContent()
        {
            return balanceView.GetListContent();
        }

        private ControlTemplate GetBalanceTemplate()
        {
            return balanceView.GetListTemplate();
        }


    }
}
