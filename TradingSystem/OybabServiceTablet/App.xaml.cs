using Oybab.Res.Exceptions;
using Oybab.Res.Tools;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;

namespace Oybab.ServiceTablet
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static string UniqueMutexName = "TradingSystemTablet";
        private static string UniqueEventName = "TradingSystemTablet" + "TradingSystemTablet";
        private static System.Threading.Mutex mutex;

        [STAThread]
        public static void Main()
        {


            bool isOwned = false;
            mutex = new System.Threading.Mutex(true, UniqueMutexName, out isOwned);
            System.Threading.EventWaitHandle eventWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset, UniqueEventName);


            if (isOwned)
            {
                // Spawn a thread which will be waiting for our event
                var thread = new System.Threading.Thread(
                    () =>
                    {
                        while (eventWaitHandle.WaitOne())
                        {
                            Current.Dispatcher.BeginInvoke(
                                (Action)(() => BringToForeground()));
                        }
                    });

                // It is important mark it as background otherwise it will prevent app from exiting.
                thread.IsBackground = true;

                thread.Start();

                var application = new App();
                application.InitializeComponent();

                //错误处理(UI)
                Application.Current.Dispatcher.UnhandledException += Application_DispatcherUnhandledException;
                //错误处理(非UI)
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


                //加载日志
                Res.Resources.GetRes().LoadLog();

                // 这是平板
                Res.Resources.GetRes().SetDeviceType((long)2);

                //加载配置文件
                Config.GetConfig().GetConfigs();

                //加载屏幕
                //Screen.LoadScreenConfig();

                //隐藏任务栏
                Taskbar.Instance.Hide();

                //关闭声音
                NavigateSoundManager.Instance.DisableSound();

                //字体效果
                FontSmoothing.OpenFontEffect();


                try
                {
                    application.Run(new LoginWindow());
                }
                catch (Exception ex)
                {
                    throw ex;
                }


                return;
            }
            // Notify other instance so it could bring itself to foreground.
            eventWaitHandle.Set();

            // Terminate this instance.
            Environment.Exit(0);
        }


        /// <summary>Brings main window to foreground.</summary>
        private static void BringToForeground()
        {
            Window window = null;
            foreach (Window win in App.Current.Windows)
            {
                if (win.IsVisible)
                {
                    window = win;
                    break;
                }
            }

            if (null == window)
            {
                return;
            }

            if (window.WindowState == WindowState.Minimized || window.Visibility == Visibility.Hidden)
            {
                window.Show();
                window.WindowState = WindowState.Normal;
            }

            // According to some sources these steps gurantee that an app will be brought to foreground.
            window.Activate();
            window.Topmost = true;
            window.Topmost = false;
            window.Focus();
        }


        /// <summary>
        /// 应用程序关闭
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            NavigateSoundManager.Instance.EnableSound();
            Taskbar.Instance.Show();

            Common.GetCommon().Close();

            Environment.Exit(0);

        }


        /// <summary>
        /// 发生未捕获错误(UI)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionPro.ExpLog(e.Exception, new Action<string>((message) =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, Res.Resources.GetRes().GetString("ErrorBig"), MessageBoxButton.OK, MessageBoxImage.Error);
            }));
            e.Handled = true;
        }



        /// <summary>
        /// 发生所有未捕获异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ExceptionPro.ExpLog(e.ExceptionObject as Exception, new Action<string>((message) =>
            {
                MessageBox.Show(Application.Current.MainWindow, message, Res.Resources.GetRes().GetString("ErrorBig"), MessageBoxButton.OK, MessageBoxImage.Error);
            }));
        }


    }
}
