using Oybab.Res.Server;
using Oybab.Res.Tools;
using Oybab.Res.View.ViewModels.Pages;
using Oybab.ServiceTablet.Pages;
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

namespace Oybab.ServiceTablet
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        // 主页
        MainWindow main = null;
        private bool _isLoaded = false;

        public LoginWindow()
        {
            InitializeComponent();
            this.ctrMsg.IgnoreLastFocus = true;

#if !DEBUG
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Topmost = true;

            // 防止超出第一屏幕在第二屏幕边缘也显示
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
#endif

            this.Loaded += (x, y) =>
            {
                if (!_isLoaded)
                {
                    _isLoaded = true;
                    Size size = new Size();
                    if (this.RenderSize.Width > this.RenderSize.Height)
                    {
                        size.Width = this.RenderSize.Width;
                        size.Height = this.RenderSize.Height;
                    }else
                    {
                        size.Width = this.RenderSize.Height;
                        size.Height = this.RenderSize.Width;
                    }
                    Res.Resources.GetRes().setSize(size);

                }
            };

            LoginViewModel viewModel = new LoginViewModel(this, crlSetting.ugCashDrawerList, crlSetting.ugPriceMonitorList, crlSetting.ugBarcodeReaderList, crlSetting.ugCardReaderList, crlSetting.ugLanguageDrawerList, new Action(() =>
            {

                new Action(() =>
                {

                    if (null == main)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            main = new MainWindow(this);
                            // 登录通知
                            Notification.Instance.ActionLogin(null, null, null);
                            main.Init(true);
                        }));
                    }
                    else
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            // 登录通知
                            Notification.Instance.ActionLogin(null, null, null);
                            main.Init(false);
                        }));
                    }

                    while (true)
                    {
                        if (null == main)
                            System.Threading.Thread.Sleep(1000);
                        else
                            break;
                    }

                    System.Threading.Thread.Sleep(1000);


                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        main.Show();
                        this.Hide();
                        return;
                    }));
                }).BeginInvoke(null, null);


            }), new Action(() =>
            {
                this.Dispatcher.Invoke(new Action(()=>{
                    RegPage reg = new RegPage(0);
                    reg.Show();
                    this.Hide();
                }));
            }));

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



        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            LoginViewModel viewModel = this.DataContext as LoginViewModel;
            viewModel.Init();
        }

        private void tbAdminNo_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginViewModel model = this.DataContext as LoginViewModel;

            model.AdminNoEnable = true;
            model.PasswordEnable = false;
            model.IsDisPlayKeyboard = true;
            tbAdminNo.CaretIndex = tbAdminNo.Text.Length;
        }

        private void pbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginViewModel model = this.DataContext as LoginViewModel;

            model.AdminNoEnable = false;
            model.PasswordEnable = true;
            model.IsDisPlayKeyboard = true;
            pbPassword.CaretIndex = pbPassword.Text.Length;
        }
    }
}
