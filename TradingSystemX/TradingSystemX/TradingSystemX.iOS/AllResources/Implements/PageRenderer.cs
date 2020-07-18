using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using Oybab.TradingSystemX.iOS.AllResources.Implements;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(ContentPage), typeof(Oybab.TradingSystemX.iOS.AllResources.Implements.PageRenderer))]
namespace Oybab.TradingSystemX.iOS.AllResources.Implements
{
    public class PageRenderer : Xamarin.Forms.Platform.iOS.PageRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }

            try
            {
                SetAppTheme();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"\t\t\tERROR: {ex.Message}");
            }
        }

        //public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        //{
        //    base.TraitCollectionDidChange(previousTraitCollection);

        //    if (this.TraitCollection.UserInterfaceStyle != previousTraitCollection.UserInterfaceStyle)
        //    {
        //        SetAppTheme();
        //    }
        //}

        void SetAppTheme()
        {
            if (this.TraitCollection.UserInterfaceStyle == UIUserInterfaceStyle.Dark)
            {
                // change to dark theme
                Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Dard);
            }
            else
            {
                // change to light theme
                Oybab.TradingSystemX.Pages.Themes.Theme.Instance.ChangeTheme(Pages.Themes.ThemeMode.Light);
            }
        }
    }
}