using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class LoginPage : ContentPage
    {
		public LoginPage ()
		{
            LoginViewModel viewModel = new LoginViewModel(this);
            viewModel.Init();
            this.BindingContext = viewModel;

            InitializeComponent();


        }
	}
}
