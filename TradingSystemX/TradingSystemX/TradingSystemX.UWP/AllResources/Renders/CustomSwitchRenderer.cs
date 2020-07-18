using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(Switch), typeof(Oybab.TradingSystemX.UWP.AllResources.Renders.CustomSwitchRenderer))]
namespace Oybab.TradingSystemX.UWP.AllResources.Renders
{
    internal class CustomSwitchRenderer : SwitchRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Switch> e)
        {
            base.OnElementChanged(e);

            if (null != Control)
            {
                if (null != Control.OnContent)
                {
                    Control.OnContent = null;
                    Control.OffContent = null;
                }
            }
        }
    }
 
}
