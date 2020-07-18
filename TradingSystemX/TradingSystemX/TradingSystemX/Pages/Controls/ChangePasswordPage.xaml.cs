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
	public partial class ChangePasswordPage : ContentPage
    {
        private ChangePasswordViewModel viewModel;

        public ChangePasswordPage()
		{
            viewModel = new ChangePasswordViewModel(this);
            this.BindingContext = viewModel;

            InitializeComponent ();
		}

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            viewModel.Init();
        }
    }
}
