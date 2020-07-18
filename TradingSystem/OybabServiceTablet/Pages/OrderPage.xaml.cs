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

namespace Oybab.ServiceTablet.Pages
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();

            OrderViewModel viewModel = new OrderViewModel(this, ctrProducts, ctrlSelected, ctrProducts.spProductType, ctrProducts.svProductList, ctrlSelected.lbList, crtlRequest.ugRequestList, ctrlPaidPrice.wpBalanceList);

            this.DataContext = viewModel;


            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object roomId)
        {
            OrderViewModel viewModel = this.DataContext as OrderViewModel;
            viewModel.Init(roomId);


            if (!IsInitialProduct)
            {
                ctrProducts.InitialProduct();
                IsInitialProduct = true;
            }
        }

        bool IsInitialProduct = false;

        /// <summary>
        /// 刷新客显
        /// </summary>
        public void RefreshPM()
        {
            OrderViewModel viewModel = this.DataContext as OrderViewModel;
            viewModel.RefreshPM();
        }

        private void page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                OrderViewModel viewModel = this.DataContext as OrderViewModel;
                viewModel.PrePrintOrder();
                e.Handled = true;
            }
        }
    }
}
