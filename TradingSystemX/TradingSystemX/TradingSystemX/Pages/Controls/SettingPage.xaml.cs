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
	public partial class SettingPage : ContentPage
    {
        private SettingViewModel viewModel;

        public SettingPage ()
		{
            viewModel = new SettingViewModel(this);
            
            this.BindingContext = viewModel;

            InitializeComponent();


        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            viewModel.Init();
        }


    }
}
