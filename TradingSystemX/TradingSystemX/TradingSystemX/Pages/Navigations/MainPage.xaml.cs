using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Navigations
{
	public partial class MainPage : NavigationPage
    {
		public MainPage ()
		{
            InitializeComponent ();
		}

        public MainPage(Page page):base(page)
        {
            InitializeComponent();
        }
	}
}
