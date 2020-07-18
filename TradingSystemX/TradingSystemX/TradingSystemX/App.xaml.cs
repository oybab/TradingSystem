using System;
using Xamarin.Forms;
using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.TradingSystemX.Pages.Navigations;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.ViewModels.Navigations;
using Oybab.TradingSystemX.VM.ViewModels.Pages;

using Xamarin.Forms.Xaml;
using System.Threading.Tasks;
using Oybab.Res.Exceptions;
using Oybab.TradingSystemX.VM.ViewModels.Controls;

namespace Oybab.TradingSystemX
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Device.SetFlags(new string[] { "AppTheme_Experimental" });
            Application.Current.RequestedThemeChanged += (s, a) =>
            {
                ReloadTheme();
            };


            // 这是手机
            TradingSystemX.Resources.Instance.SetTime((long)3);


            NavigationPath.Instance.LoginNavigation = new LoginMainPage();



            // 获取初始值
            Common.Instance.ReadBak();


            // 初始化导航
            NavigationPath.Instance.InitialLoginNavigations(NavigationPath.Instance.LoginNavigation);


            // 导航到第一个导航并打开第一页

            NavigationPath.Instance.NewLoginPage = new LoginPage();

            NavigationPath.Instance.SettingPage = new SettingPage();


            // The root page of your application
            NavigationPath.Instance.SwitchNavigate(0);
            NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.NewLoginPage, false);

            


        }

        protected override void OnStart()
        {
            ReloadTheme();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            if (TradingSystemX.Resources.Instance.IsSessionExists())
            {
                Session.Instance.Stop();
                OperatesService.Instance.AbortService();
            }
        }

        protected override void OnResume()
        {
            RoomListViewModel viewModel = null;

            if (null != NavigationPath.Instance.RoomListPage)
            {
                viewModel = NavigationPath.Instance.RoomListPage.BindingContext as RoomListViewModel;

                if (null != viewModel)
                    viewModel.IsLoading = true;
            }


              

            ReloadTheme();

            if (TradingSystemX.Resources.Instance.IsSessionExists())
            {
                // 注册睡眠唤醒(免得session失效)
                Task.Run(async () =>
                {
                    try
                    {
                        OperatesService.Instance.AbortService();

                     
                        // Handle when your app resumes


                        await OperatesService.Instance.ServiceSession(false, null, true);
                    }
                    catch (Exception ex)
                    {
                        //QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, Res.Instance.GetString("Error"), ex.Message + ex.StackTrace, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                        ExceptionPro.ExpLog(ex);
                    }
                    finally
                    {
                        if (OperatesService.Instance.IsExpired || OperatesService.Instance.IsAdminUsing)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                viewModel.IsLoading = false;
                                MainViewModel viewModel2 = NavigationPath.Instance.CurrentNavigate.BindingContext as MainViewModel;
                                viewModel2.ReLyout();
                            });
                        }
                        else
                        {
                            if (null != viewModel)
                            {
                                await ExtX.Sleep(100);
                                NavigationPath.Instance.RoomListPage.RefreshRoomList();
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    viewModel.IsLoading = false;
                                });
                            }
                        }

                        Session.Instance.Start();
                    }


                });
            }
        }


        /// <summary>
        /// 重新载入主题
        /// </summary>
        private void ReloadTheme()
        {
            // 确定该用的主题
            if (Xamarin.Essentials.AppInfo.RequestedTheme == Xamarin.Essentials.AppTheme.Dark)
            {
                Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Dard);
            }
            else
            {
                Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Light);
            }
        }
    }
}
