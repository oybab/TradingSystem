using Oybab.TradingSystemX.VM.ViewModels.Pages;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class StatisticWebPage : ContentPage
    {
        private StatisticWebViewModel viewModel;

        public StatisticWebPage()
		{
            viewModel = new StatisticWebViewModel(this);
            
            this.BindingContext = viewModel;

            InitializeComponent ();

            viewModel.InitialComponent(browser);

        }

        

       
    }
}
