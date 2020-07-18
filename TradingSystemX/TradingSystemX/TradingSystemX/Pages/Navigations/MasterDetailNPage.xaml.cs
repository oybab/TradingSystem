using Oybab.TradingSystemX.VM.DService;
using Oybab.TradingSystemX.VM.ViewModels.Navigations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Navigations
{
	public partial class MasterDetailNPage : NavigationPage
	{
		public MasterDetailNPage()
		{

        }
        public MasterDetailNPage(Page page):base(page)
        {
            InitializeComponent();
        }

       
    }
}
