﻿using Oybab.Res.View.ViewModels.Pages;
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
    public partial class TakeoutPage : Page
    {
        public TakeoutPage()
        {
            InitializeComponent();

            TakeoutViewModel viewModel = new TakeoutViewModel(this, ctrProducts, ctrlSelected, ctrProducts.spProductType, ctrProducts.svProductList, ctrlSelected.lbList, crtlRequest.ugRequestList);

            this.DataContext = viewModel;


        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            TakeoutViewModel viewModel = this.DataContext as TakeoutViewModel;
            viewModel.Init();


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
            TakeoutViewModel viewModel = this.DataContext as TakeoutViewModel;
            viewModel.RefreshPM();
        }
    }
}
