using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Oybab.ServiceTablet.Pages
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TakeoutCheckoutPage : Page
    {
        public TakeoutCheckoutPage()
        {
            InitializeComponent();

            TakeoutCheckoutViewModel viewModel = new TakeoutCheckoutViewModel(this, ctrlPaidPrice.wpBalanceList);

            //viewModel.Init();
            this.DataContext = viewModel;


          
        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
            TakeoutCheckoutViewModel viewModel = this.DataContext as TakeoutCheckoutViewModel;
            viewModel.Init(obj);


            
        }

        private void WrapPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TakeoutCheckoutViewModel viewModel = this.DataContext as TakeoutCheckoutViewModel;
            viewModel.FinishPaidPrice();
        }
    }
}
