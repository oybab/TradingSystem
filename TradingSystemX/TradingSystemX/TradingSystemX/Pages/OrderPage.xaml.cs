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
	public partial class OrderPage : MasterDetailPage
    {
        OrderViewModel viewModel;

        public OrderPage()
		{
            viewModel = new OrderViewModel(this);
            
            this.BindingContext = viewModel;
            InitializeComponent();

            
        }



     

        internal void Init(long RoomId, Action<int> FistLoad)
        {
            viewModel.Init(RoomId, FistLoad);
        }



    }
}
