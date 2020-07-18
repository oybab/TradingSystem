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
    public partial class ChangePasswordControl : UserControl
    {
        public ChangePasswordControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangePasswordViewModel model = this.DataContext as ChangePasswordViewModel;
            model.SetDisplay(1);

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_GotFocus_1(object sender, RoutedEventArgs e)
        {
            ChangePasswordViewModel model = this.DataContext as ChangePasswordViewModel;
            model.SetDisplay(2);

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_GotFocus_2(object sender, RoutedEventArgs e)
        {
            ChangePasswordViewModel model = this.DataContext as ChangePasswordViewModel;
            model.SetDisplay(3);

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;
        }
    }
}
