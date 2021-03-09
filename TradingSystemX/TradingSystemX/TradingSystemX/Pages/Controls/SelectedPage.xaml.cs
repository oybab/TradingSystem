using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.VM.DService;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class SelectedPage : ContentPage
	{
		public SelectedPage()
		{
            InitializeComponent ();
		}


        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        internal ControlTemplate GetSelectedTemplate()
        {
            return this.Resources["SelectedListTemplate"] as ControlTemplate;
        }

        /// <summary>
        /// 返回产品容器
        /// </summary>
        /// <returns></returns>
        internal StackLayout GetSelectedContent()
        {
            return lvList;
        }

      

        internal StackLayout GetRequestContent() {
            return requestView.GetListContent();
        }

        internal ControlTemplate GetRequestTemplate() {
            return requestView.GetListTemplate();
        }


        internal StackLayout GetBalanceContent()
        {
            return balanceView.GetListContent();
        }

        internal ControlTemplate GetBalanceTemplate()
        {
            return balanceView.GetListTemplate();
        }


        /// <summary>
        /// For fix the damn iOS BUG....
        /// </summary>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var keyBoardHelper = DependencyService.Get<IKeyboardHelper>();
            keyBoardHelper?.HideKeyboard();

        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedViewModel view = this.BindingContext as SelectedViewModel;
            if (null != view)
            {
                if (null != view.SelectedLang && null != view.GoCommand)
                    view.GoCommand.Execute("Lang");
            }
        }
    }
}
