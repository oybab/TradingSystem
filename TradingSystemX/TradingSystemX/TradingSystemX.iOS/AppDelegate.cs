using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using UIKit;

namespace Oybab.TradingSystemX.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainOnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            //TaskScheduler.UnobservedTaskException -= TaskSchedulerOnUnobservedTaskException;
            //TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            ZXing.Net.Mobile.Forms.iOS.Platform.Init();
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());


            return base.FinishedLaunching(app, options);
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
    }
}
