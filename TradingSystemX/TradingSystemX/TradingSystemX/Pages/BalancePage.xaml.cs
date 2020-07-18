using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class BalancePage : ContentPage
    {
        private BalanceManagerViewModel viewModel;

        public BalancePage()
		{
           
            viewModel = new BalanceManagerViewModel(this);
            
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





        /// <summary>
        /// 被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {


            Entry tb = sender as Entry;
            tb.Text = tb.Text.Trim();


            BalanceManagerViewModel viewModel = this.BindingContext as BalanceManagerViewModel;
            viewModel.ChangePrices();
        }



        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switcherAdd_Toggled(object sender, ToggledEventArgs e)
        {
            Switch sw = sender as Switch;
            if (!e.Value)
                return;

            BalanceManagerViewModel viewModel = this.BindingContext as BalanceManagerViewModel;

            viewModel.Mode = 1;

        }


        /// <summary>
        /// 减去
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switcherSubtract_Toggled(object sender, ToggledEventArgs e)
        {
            Switch sw = sender as Switch;
            if (!e.Value)
                return;


            BalanceManagerViewModel viewModel = this.BindingContext as BalanceManagerViewModel;

            viewModel.Mode = 2;


        }





        /// <summary>
        /// 余额支付
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switcherAdd_Toggled2(object sender, ToggledEventArgs e)
        {
            Switch sw = sender as Switch;
            if (!e.Value)
                return;

            BalanceManagerViewModel viewModel = this.BindingContext as BalanceManagerViewModel;

            viewModel.BalanceMode = false;
        }


        /// <summary>
        /// 余额管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switcherSubtract_Toggled2(object sender, ToggledEventArgs e)
        {
            Switch sw = sender as Switch;
            if (!e.Value)
                return;


            BalanceManagerViewModel viewModel = this.BindingContext as BalanceManagerViewModel;

            viewModel.BalanceMode = true;

        }




    }
}
