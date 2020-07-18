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
	public partial class ChangePaidPriceView : ContentView
    {
   
        public ChangePaidPriceView()
		{
            InitializeComponent();



            lbMemberName.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {

                    ChangePaidPriceViewModel viewModel = this.BindingContext as ChangePaidPriceViewModel;
                    viewModel.AddMemberPaidPrice();
                }

                )
            });
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


            ChangePaidPriceViewModel viewModel = this.BindingContext as ChangePaidPriceViewModel;
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

            ChangePaidPriceViewModel viewModel = this.BindingContext as ChangePaidPriceViewModel;

            viewModel.Mode = 1;
            viewModel.ChangePrices();
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


            ChangePaidPriceViewModel viewModel = this.BindingContext as ChangePaidPriceViewModel;

            viewModel.Mode = 2;
           
            viewModel.ChangePrices();
        }




        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        internal ControlTemplate GetListTemplate()
        {
            return this.Resources["BalanceListTemplate"] as ControlTemplate;
        }

        /// <summary>
        /// 返回产品容器
        /// </summary>
        /// <returns></returns>
        internal StackLayout GetListContent()
        {
            return spBalanceList;
        }

    }
}
