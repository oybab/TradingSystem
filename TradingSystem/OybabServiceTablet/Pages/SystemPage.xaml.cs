using Oybab.Res.Server;
using Oybab.Res.Tools;
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
    public partial class SystemPage : Page
    {
        public SystemPage()
        {
            InitializeComponent();

            SystemViewModel viewModel = new SystemViewModel(this);


            this.DataContext = viewModel;

            Notification.Instance.NotificationLogin += (obj, value, args) => { this.Dispatcher.BeginInvoke(new Action(() => { LoginRefresh(); })); };
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
            SystemViewModel viewModel = this.DataContext as SystemViewModel;
            viewModel.Init(obj);
        }



        /// <summary>
        /// 登录后刷新一些数据
        /// </summary>
        private void LoginRefresh()
        {

            // 根据权限显示财务日志
            if (Common.GetCommon().IsAllowFinancceLog())
                spFinanceLog.Visibility = Visibility.Visible;
            else
                spFinanceLog.Visibility = Visibility.Collapsed;

            
            // 根据权限显示添加外部账单
            if (Common.GetCommon().IsAllowImportManager())
                spImportManager.Visibility = Visibility.Visible;
            else
                spImportManager.Visibility = Visibility.Collapsed;

            // 根据权限显示添加外部账单
            if (Common.GetCommon().IsAddOuterBill())
                spAddOuterBill.Visibility = Visibility.Visible;
            else
                spAddOuterBill.Visibility = Visibility.Collapsed;
        }


    }
}
