using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Oybab.TradingSystemX.iOS.AllResources.Implements;
using Oybab.TradingSystemX.VM.DService;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(iOSKeyboardHelper))]
namespace Oybab.TradingSystemX.iOS.AllResources.Implements
{
    public class iOSKeyboardHelper : IKeyboardHelper
    {
        public void HideKeyboard()
        {
            UIApplication.SharedApplication.KeyWindow.EndEditing(true);
        }
    }
}