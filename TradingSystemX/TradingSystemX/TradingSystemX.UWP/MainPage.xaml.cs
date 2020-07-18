using Oybab.TradingSystemX.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Oybab.TradingSystemX.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new Oybab.TradingSystemX.App());

            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer.Init();

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, e) =>
            {
                Frame rootFrame = Window.Current.Content as Frame;

                // 如果上一个页面可以返回
                if (!e.Handled && rootFrame != null && rootFrame.CanGoBack)
                {
                    // 结账时进入支付窗口时可以返回前一页到已选列表页中. 如果在支付页面则处理它
                    if (NavigationPath.Instance.NavigationMode == 4)
                    {
                        e.Handled = true;
                        NavigationPath.Instance.CloseCheckoutPanels(true);
                    }
                    else
                    {
                        // Global
                        e.Handled = true;
                        rootFrame.GoBack();
                    }
                }
                // 关闭订单页面中的那些菜单
                else if (NavigationPath.Instance.NavigationMode == 3)
                {
                    e.Handled = true;
                    NavigationPath.Instance.ClosePanels(true);
                }
                // 如果上一个页面无法返回.
                else if (!e.Handled && rootFrame != null && !rootFrame.CanGoBack)
                {
                    if (NavigationPath.Instance.NavigationMode == 2)
                    {
                        // 内部账单和外部账单详情页返回到主界面.
                        e.Handled = true;
                        if (NavigationPath.Instance.CurrentMasterDetail == NavigationPath.Instance.OrderPage)
                            NavigationPath.Instance.CloseOrder();
                        else if (NavigationPath.Instance.CurrentMasterDetail == NavigationPath.Instance.TakeoutPage)
                            NavigationPath.Instance.CloseTakeout();
                        else if (NavigationPath.Instance.CurrentMasterDetail == NavigationPath.Instance.ImportPage)
                            NavigationPath.Instance.CloseImport();
                    }
                    
                }
            };
        }




        //private void ColorValuesChanged(UISettings sender, object args)
        //{
        //    var backgroundColor = sender.GetColorValue(UIColorType.Background);
        //    var isDarkMode = backgroundColor == Colors.Black;
        //    if (isDarkMode)
        //    {
        //        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //            // change to dark theme
        //            Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Dard);
        //        });
        //    }
        //    else
        //    {
        //        Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
        //        {
        //            // change to light theme
        //            Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Light);
        //        });
        //    }
        //}
    }
}
