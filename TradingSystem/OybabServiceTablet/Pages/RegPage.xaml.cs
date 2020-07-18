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
    public partial class RegPage : Window
    {
        public RegPage(int RegType)
        {
            InitializeComponent();

#if !DEBUG
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Topmost = true;

            // 防止超出第一屏幕在第二屏幕边缘也显示
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
#endif


            RegViewModel viewModel = new RegViewModel(this, RegType);

            viewModel.Init();
            this.DataContext = viewModel;



            //鼠标不需要显示
            if (!Res.Resources.GetRes().DisplayCursor)
                Mouse.OverrideCursor = Cursors.None;



            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);
        }



        /// <summary>
        /// 处理旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowState = System.Windows.WindowState.Maximized;
        }

        private void tbRegNo_GotFocus(object sender, RoutedEventArgs e)
        {
            RegViewModel model = this.DataContext as RegViewModel;

            model.RegNoEnable = true;
            model.IsDisPlayKeyboard = true;

            tbRegNo.CaretIndex = tbRegNo.Text.Length;
        }



        /// <summary>
        /// 关闭应用程序
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown(0);
        }
    }
}
