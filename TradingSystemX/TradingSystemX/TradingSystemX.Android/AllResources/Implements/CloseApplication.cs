using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Oybab.TradingSystemX.Droid.AllResources.Implements;
using Oybab.TradingSystemX.VM.DService;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApplication))]
namespace Oybab.TradingSystemX.Droid.AllResources.Implements
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}