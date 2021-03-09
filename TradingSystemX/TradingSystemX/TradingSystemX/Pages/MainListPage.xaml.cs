using Oybab.TradingSystemX.VM.ModelsForViews;
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

            viewModel.ClickPickerEvent -= ViewModel_ClickPickerEvent;
            viewModel.ClickPickerEvent += ViewModel_ClickPickerEvent;
        }

        private void ViewModel_ClickPickerEvent(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (pkLanguage.IsFocused)
                    pkLanguage.Unfocus();

                pkLanguage.Focus();
            });
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

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            MainListViewModel view = this.BindingContext as MainListViewModel;
            if (null != view)
            {
                if (null != view.SelectedLang && null != view.GoCommand && view._isInit)
                    view.GoCommand.Execute(new MainListModel { Name = "ChangeLanguageFinish" });
            }
        }
    }
}
