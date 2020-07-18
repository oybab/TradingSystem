using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class ChangePriceView : ContentView
    {
   
        public ChangePriceView()
		{
            InitializeComponent();
        }


        /// <summary>
        /// 被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {


            Entry tb = sender as Entry;
            tb.Text = tb.Text.Trim();


            ChangePriceViewModel viewModel = this.BindingContext as ChangePriceViewModel;
            viewModel.ChangePrice();
        }


    }
}
