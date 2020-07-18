using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class MainListPage : ContentPage
    {
        private MainListViewModel viewModel;

        public MainListPage()
		{
            InitializeComponent();


            viewModel = new MainListViewModel(this, GetListContent(), GetListTemplate());
            
            this.BindingContext = viewModel;
            

        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            viewModel.Init();
        }



        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        private ControlTemplate GetListTemplate()
        {
            return this.Resources["ListTemplate"] as ControlTemplate;
        }

        /// <summary>
        /// 返回产品容器
        /// </summary>
        /// <returns></returns>
        private StackLayout GetListContent()
        {
            return lvList;
        }
    }
}
