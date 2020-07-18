using Oybab.Res.View.ViewModels.Pages;
using Oybab.ServiceTablet.Resources.Component;
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
    public partial class RoomPage : Page
    {
        public RoomPage(Window window)
        {
            InitializeComponent();

            RoomViewModel viewModel = new RoomViewModel(this, ctrReplace.ugRoomList);


            this.DataContext = viewModel;


            viewModel.TimeupAlr = () =>
            {
                // Flash window 3 times
                FlashWindow.Instance.StartFlashWindow(window, 3);
            };

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object RoomId)
        {
            RoomViewModel viewModel = this.DataContext as RoomViewModel;
            viewModel.Init(RoomId);
        }


        /// <summary>
        /// 刷新客桌信息
        /// </summary>
        internal void RefreshRoomList()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                RoomViewModel viewModel = this.DataContext as RoomViewModel;
                viewModel.RefreshAll(true);
            }));
        }

    }
}
