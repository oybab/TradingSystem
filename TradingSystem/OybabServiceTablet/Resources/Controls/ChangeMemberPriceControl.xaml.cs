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
    public partial class ChangeMemberPriceControl : UserControl
    {
        public ChangeMemberPriceControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangeMemberPriceViewModel model = this.DataContext as ChangeMemberPriceViewModel;

            model.DisplayMode = 1;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeMemberPriceViewModel model = this.DataContext as ChangeMemberPriceViewModel;

            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;


        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Trim();

            ChangeMemberPriceViewModel model = this.DataContext as ChangeMemberPriceViewModel;
            model.ChangePrice();

            if (tb.CaretIndex != tb.Text.Length)
                tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Trim();


            ChangeMemberPriceViewModel model = this.DataContext as ChangeMemberPriceViewModel;
            model.ChangePrice();

            if (null != tb)
                tb.CaretIndex = tb.Text.Length;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ChangeMemberPriceViewModel model = this.DataContext as ChangeMemberPriceViewModel;

            CheckBox cb = sender as CheckBox;
            if (null == cb)
                return;

            if (model.IsPayByCard)
            {
                model.IsPayByCard = false;
            }
            else
            {
                model.IsPayByCard = true;
            }
        }
    }
}
