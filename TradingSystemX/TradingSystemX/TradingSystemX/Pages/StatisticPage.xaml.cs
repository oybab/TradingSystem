using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class StatisticPage : ContentPage
    {
        private StatisticViewModel viewModel;

        public StatisticPage()
		{
           
            viewModel = new StatisticViewModel(this);
            
            this.BindingContext = viewModel;


            InitializeComponent();


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
