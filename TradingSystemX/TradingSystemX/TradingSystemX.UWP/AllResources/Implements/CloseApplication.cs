using Oybab.TradingSystemX.UWP.AllResources.Implements;
using Oybab.TradingSystemX.VM.DService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApplication))]
namespace Oybab.TradingSystemX.UWP.AllResources.Implements
{
    public class CloseApplication : ICloseApplication
    {
        public void closeApplication()
        {
            Windows.ApplicationModel.Core.CoreApplication.Exit();
        }
    }
}
