using Oybab.TradingSystemX.VM.ViewModels.Navigations;
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Navigations
{
	public partial class LoginMainPage : NavigationPage
    {
		public LoginMainPage ()
		{
            LoginMainViewModel viewModel = new LoginMainViewModel(this);

            this.BindingContext = viewModel;


            InitializeComponent();
		}
	}
}
