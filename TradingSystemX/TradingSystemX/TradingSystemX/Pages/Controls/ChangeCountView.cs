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
	public partial class ChangeCountView : ContentView
    {
   
        public ChangeCountView()
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


            ChangeCountViewModel viewModel = this.BindingContext as ChangeCountViewModel;
            viewModel.ChangeCount();
        }





    }
}
