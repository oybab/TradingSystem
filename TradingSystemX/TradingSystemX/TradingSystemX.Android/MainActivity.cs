using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System.Threading.Tasks;
using Android.Content.Res;

namespace Oybab.TradingSystemX.Droid
{
    [Activity(Label = "Oybab Trading", Icon = "@mipmap/ic_launcher", Theme = "@style/MyTheme.Splash", LaunchMode = LaunchMode.SingleTask, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            //TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
            //TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AndroidEnvironment.UnhandledExceptionRaiser -= AndroidEnvironmentOnUnhandledException;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironmentOnUnhandledException;

            base.OnCreate(savedInstanceState);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            ZXing.Mobile.MobileBarcodeScanner.Initialize(Application);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

        }

        private void AndroidEnvironmentOnUnhandledException(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;

            System.Diagnostics.Debug.WriteLine("Catch in AndroidEnvironmentOnUnhandledException:   " + e.Exception.Message + e.Exception.StackTrace);


            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, Res.Instance.GetString("ErrorBig"), e.Exception.Message + e.Exception.StackTrace, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
        }

        //private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Catch in TaskSchedulerOnUnobservedTaskException:   " + e.Exception.Message + e.Exception.StackTrace);


        //    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, Res.Instance.GetString("ErrorBig"), e.Exception.Message + e.Exception.StackTrace, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
        //}

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Catch in CurrentDomainOnUnhandledException:   " + e.ExceptionObject.ToString());


            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, Res.Instance.GetString("ErrorBig"), e.ExceptionObject.ToString(), MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnBackPressed()
        {
            
            bool IsHandled = false;

            //// 如果上一个页面可以返回
            //if (!IsHandled && NavigationPath.Instance.CanGoBack)
            //{
            //        // Global
            //        IsHandled = true;
            //        base.OnBackPressed();
                
            //}
            //// 如果上一个页面无法返回.
            //else if
            if (!IsHandled && !NavigationPath.Instance.CanGoBack)
            {
                 // 结账时进入支付窗口时可以返回前一页到已选列表页中. 如果在支付页面则处理它
                if (NavigationPath.Instance.NavigationMode == 4)
                {
                    IsHandled = true;
                    NavigationPath.Instance.CloseCheckoutPanels(true);
                }
                // 关闭订单页面中的那些菜单
                else if (NavigationPath.Instance.NavigationMode == 3)
                {
                    IsHandled = true;
                    NavigationPath.Instance.ClosePanels(true);
                }
                else if (NavigationPath.Instance.NavigationMode == 2)
                {
                    // 内部账单和外部账单详情页返回到主界面.
                    IsHandled = true;

                    if (!NavigationPath.Instance.CloseModelPages())
                    {
                        if (NavigationPath.Instance.CurrentMasterDetail == NavigationPath.Instance.OrderPage)
                            NavigationPath.Instance.CloseOrder();
                        else if (NavigationPath.Instance.CurrentMasterDetail == NavigationPath.Instance.TakeoutPage)
                            NavigationPath.Instance.CloseTakeout();
                        else if (NavigationPath.Instance.CurrentMasterDetail == NavigationPath.Instance.ImportPage)
                            NavigationPath.Instance.CloseImport();
                    }
                }
                
               
            }


            if (!IsHandled)
            {
                if (NavigationPath.Instance.IsGoBackground)
                    MoveTaskToBack(true);
                else if (NavigationPath.Instance.IsExit)
                    Common.Instance.Exit();
                else
                {
                    if (!NavigationPath.Instance.GoBack())
                        base.OnBackPressed();
                }


            }
 


            // 以后可能用到: 防止同一个页面多次push到navigation
            //       if (Navigation.NavigationStack.Count == 0 ||
            //Navigation.NavigationStack.Last().GetType() != typeof(CustomerPage))
            //       {
            //           await Navigation.PushAsync(new CustomerPage(), true);
            //       }
        }



        //public override void OnConfigurationChanged(Configuration newConfig)
        //{
        //    base.OnConfigurationChanged(newConfig);

        //    if ((newConfig.UiMode & UiMode.NightNo) != 0)
        //    {
        //        // change to light theme
        //        Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Light);
        //    }
        //    else
        //    {
        //        // change to dark theme
        //        Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Dard);
        //    }
        //}






    }





}