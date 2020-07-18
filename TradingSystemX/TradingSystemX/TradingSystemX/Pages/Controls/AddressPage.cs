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
	public partial class AddressPage : ContentPage
    {
        private AddressViewModel viewModel;

        public AddressPage()
		{
            viewModel = new AddressViewModel(this);

            this.BindingContext = viewModel;
            InitializeComponent();

        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            viewModel.Initial();
        }
    }
}
