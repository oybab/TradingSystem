using Oybab.Res.View.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oybab.ServiceTablet.Resources.Controls
{
    /// <summary>
    /// KeyboardLittleControl.xaml 的交互逻辑
    /// </summary>
    public partial class AboutControl : UserControl
    {
        public AboutControl()
        {
            InitializeComponent();
            this.IsVisibleChanged -= AboutControl_IsVisibleChanged;       
            this.IsVisibleChanged += AboutControl_IsVisibleChanged;
        }

        private void AboutControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DisableCount = 0;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AboutViewModel viewModel = this.DataContext as AboutViewModel;
            viewModel.Command.Execute(null);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private int DisableCount = 0;

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DisableCount >= 100 || (DisableCount >= 3 && Res.Resources.GetRes().ExtendInfo.SliderMode != "2"))
            {
                AboutViewModel viewModel = this.DataContext as AboutViewModel;
                viewModel.OpenDisable();
               
            }
            ++DisableCount;

            e.Handled = true;
        }

       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/sourcecode/" + Res.Resources.GetRes().MainLang.Culture.Name);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/donate/" + Res.Resources.GetRes().MainLang.Culture.Name);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/company/" + Res.Resources.GetRes().MainLang.Culture.Name);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://oybab.net/tradingsystem/license/" + Res.Resources.GetRes().MainLang.Culture.Name);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:service@oybab.net");
        }
    }
}
