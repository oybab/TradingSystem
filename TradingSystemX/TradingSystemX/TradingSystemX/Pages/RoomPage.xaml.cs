using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class RoomPage : ContentPage
    {
		public RoomPage()
		{
            InitializeComponent();

        }



        private void Goback(object obj, EventArgs even)
        {

            NavigationPath.Instance.GoNavigateBack(true, true);
        }
	}
}
