using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Oybab.TradingSystemX.Droid.AllResources.Implements;
using Oybab.TradingSystemX.VM.DService;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(DroidKeyboardHelper))]
namespace Oybab.TradingSystemX.Droid.AllResources.Implements
{
    public class DroidKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {

            var inputMethodManager = Android.App.Application.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            if (inputMethodManager != null && Android.App.Application.Context is Activity)
            {
                var activity = Android.App.Application.Context as Activity;
                var token = activity.CurrentFocus?.WindowToken;
                inputMethodManager.HideSoftInputFromWindow(token, HideSoftInputFlags.None);

                activity.Window.DecorView.ClearFocus();
            }
        }
    }
}