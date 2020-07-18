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
    public partial class ChangePaidPriceControl : UserControl
    {
        public ChangePaidPriceControl()
        {
            InitializeComponent();
            btnForFocusProblem.IsVisibleChanged -= MsgControl_IsVisibleChanged;
            btnForFocusProblem.IsVisibleChanged += MsgControl_IsVisibleChanged;
        }



        // 去掉失去焦点操作(登录时会导致总是显示键盘)
        internal bool IgnoreLastFocus = false;

        /// <summary>
        /// 获取焦点后记得把它设置成已选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible == true)
            {
                RemoveFocus();
            }
            
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.DisplayMode = 1;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = true;
        }

      

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;


        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Trim();

            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;
            model.ChangePrices();

            if (tb.CaretIndex != tb.Text.Length)
                tb.CaretIndex = tb.Text.Length;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.Text = tb.Text.Trim();


            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;
            model.ChangePrices();

            if (null != tb)
                tb.CaretIndex = tb.Text.Length;

            model.Handle(e);
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.Mode = 1;
            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;
            model.ChangePrices();
            RemoveFocus();
        }

        private void CheckBox1_Click(object sender, RoutedEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.Mode = 2;
            model.DisplayMode = 0;
            model.KeyboardLittle.IsDisplayKeyboard = false;
            model.ChangePrices();
            RemoveFocus();
        }

        /// <summary>
        /// 焦点发送的其他地方
        /// </summary>
        private void RemoveFocus()
        {
            btnForFocusProblem.Focus();
        }

        private void TextBox_GotFocus_3(object sender, RoutedEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.DisplayMode = 3;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = false;

        }



        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.AddMemberPaidPrice();
        }

        private void Label_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

            model.FinishPaidPrice();
        }

        private void control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                ChangePaidPriceViewModel model = this.DataContext as ChangePaidPriceViewModel;

                model.SelectNextBalance();

                e.Handled = true;
            }
        }

  
    }
}
