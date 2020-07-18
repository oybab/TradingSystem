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
    public partial class SearchControl : UserControl
    {
        public SearchControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchViewModel model = this.DataContext as SearchViewModel;

            model.DisplayMode = 1;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchViewModel model = this.DataContext as SearchViewModel;

            model.DisplayMode = 0;

            model.KeyboardLittle.IsDisplayKeyboard = false;

        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            SearchViewModel model = this.DataContext as SearchViewModel;

            model.Mode = 1;
            //model.DisplayMode = 0;
            //model.KeyboardLittle.IsDisplayKeyboard = false;
        }

        private void CheckBox1_Click(object sender, RoutedEventArgs e)
        {
            SearchViewModel model = this.DataContext as SearchViewModel;

            model.Mode = 2;
            //model.DisplayMode = 0;
            //model.KeyboardLittle.IsDisplayKeyboard = false;
        }

        
    }
}
