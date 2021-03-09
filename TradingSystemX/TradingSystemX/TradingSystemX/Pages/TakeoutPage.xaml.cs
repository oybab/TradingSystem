using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.DService;
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class TakeoutPage : FlyoutPage
    {
        TakeoutViewModel viewModel;

        public TakeoutPage()
		{
            viewModel = new TakeoutViewModel(this);
            
            this.BindingContext = viewModel;
            InitializeComponent();


        }

     
        internal void Init(Action IsFirst = null)
        {
            viewModel.Init(IsFirst);
        }

    }
}
