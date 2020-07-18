using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Oybab.TradingSystemX.iOS.AllResources.Implements;
using Oybab.TradingSystemX.VM.DService;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApplication))]
namespace Oybab.TradingSystemX.iOS.AllResources.Implements
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}