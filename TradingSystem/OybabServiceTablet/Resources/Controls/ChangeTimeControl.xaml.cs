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
    public partial class ChangeTimeControl : UserControl
    {
        public ChangeTimeControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            model.DisplayMode = 1;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            model.DisplayMode = 0;



        }

        private void TextBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            model.DisplayMode = 2;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = true;
        }

        private void TextBox1_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            model.Mode = 1;
            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;
            model.ChangeTime();
            RemoveFocus();
        }

        private void CheckBox1_Click(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            model.Mode = 2;
            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;
            model.ChangeTime();
            RemoveFocus();
        }

        /// <summary>
        /// 焦点发送的其他地方
        /// </summary>
        private void RemoveFocus()
        {
            btnForFocusProblem.Focus();
        }



        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            tb.Text = tb.Text.Trim();


            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;
            model.ChangeTime();


            if (tb.CaretIndex != tb.Text.Length)
                tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Trim();


            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;
            model.ChangeTime();

            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.HandleKeyboard(e);
        }

        private void TextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;

            tb.Text = tb.Text.Trim();


            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;
            model.ChangeTime();


            if (tb.CaretIndex != tb.Text.Length)
                tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Trim();


            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;
            model.ChangeTime();

            if (null != tb)
                tb.CaretIndex = tb.Text.Length;

            model.HandleKeyboard(e);
        }


        /// <summary>
        /// 单击UnlimitedTime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox2_Click(object sender, RoutedEventArgs e)
        {
            ChangeTimeViewModel model = this.DataContext as ChangeTimeViewModel;

            if (model.UnlimitedTime)
                model.UnlimitedTime = false;
            else
                model.UnlimitedTime = true;

            model.KeyboardLittle.IsDisplayKeyboard = false;

            RemoveFocus();
        }

    }
}
