using Oybab.DAL;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class NewMemberPage : ContentPage
    {
        private NewMemberViewModel viewModel;

        public NewMemberPage()
		{
            viewModel = new NewMemberViewModel(this);

            this.BindingContext = viewModel;
            InitializeComponent();

        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(Member member)
        {
            viewModel.Initial(member);
        }
    }
}
