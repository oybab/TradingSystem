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
	public partial class ImportCheckoutPage : ContentPage
    {
        private ImportCheckoutViewModel viewModel;

        public ImportCheckoutPage()
		{
            InitializeComponent();

            viewModel = new ImportCheckoutViewModel(null, GetBalanceContent(), GetBalanceTemplate());
            
            this.BindingContext = viewModel;


            lbPaidPrice.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {

                    ImportCheckoutViewModel viewModel = this.BindingContext as ImportCheckoutViewModel;
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
