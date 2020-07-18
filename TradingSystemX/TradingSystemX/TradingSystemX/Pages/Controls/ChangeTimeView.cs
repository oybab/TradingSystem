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
	public partial class ChangeTimeView : ContentView
    {
   
        public ChangeTimeView()
		{
            InitializeComponent();
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

            ChangeTimeViewModel viewModel = this.BindingContext as ChangeTimeViewModel;

            viewModel.Mode = 1;

            
            viewModel.ChangeTime();
        }


        /// <summary>
        /// 减去
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switcherSubtract_Toggled(object sender, ToggledEventArgs e)
        {
            if (!e.Value)
                return;


            ChangeTimeViewModel viewModel = this.BindingContext as ChangeTimeViewModel;

            viewModel.Mode = 2;

            viewModel.ChangeTime();
        }


        /// <summary>
        /// 小时被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryHour_TextChanged(object sender, TextChangedEventArgs e)
        {


            Entry tb = sender as Entry;
            tb.Text = tb.Text.Trim();


            ChangeTimeViewModel viewModel = this.BindingContext as ChangeTimeViewModel;
            viewModel.DisplayMode = 1;
            viewModel.ChangeTime();
        }


        /// <summary>
        /// 分钟被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EntryMinute_TextChanged(object sender, TextChangedEventArgs e)
        {


            Entry tb = sender as Entry;
            tb.Text = tb.Text.Trim();


            ChangeTimeViewModel viewModel = this.BindingContext as ChangeTimeViewModel;
            viewModel.DisplayMode = 2;
            viewModel.ChangeTime();
        }

       
    }
}
