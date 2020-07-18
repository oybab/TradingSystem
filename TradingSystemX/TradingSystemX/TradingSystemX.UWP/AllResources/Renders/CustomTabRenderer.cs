using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(Oybab.TradingSystemX.UWP.AllResources.Renders.CustomTabRenderer))]
namespace Oybab.TradingSystemX.UWP.AllResources.Renders
{
    public class CustomTabRenderer : TabbedPageRenderer
    {
        public CustomTabRenderer()
        {
            this.ElementChanged -= CustomTabRenderer_ElementChanged;
            this.ElementChanged += CustomTabRenderer_ElementChanged;

        }

        private void CustomTabRenderer_ElementChanged(object sender, VisualElementChangedEventArgs e)
        {
            Control.HeaderTemplate = GetStyledTitleTemplate();
            //Windows.UI.Xaml.DataTemplate dt = Control.HeaderTemplate as Windows.UI.Xaml.DataTemplate;
            


        }

        private Windows.UI.Xaml.DataTemplate GetStyledTitleTemplate()
        {
            string dataTemplateXaml = @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                 <TextBlock
                    Text=""{Binding Title}""
                    FontFamily=""/Assets/OybabTuz.ttf#Oybab Tuz""
                    FontWeight = ""Normal""
                    FontSize = ""15"" />
                  </DataTemplate>";

            return (Windows.UI.Xaml.DataTemplate)XamlReader.Load(dataTemplateXaml);
        }
    }
}
