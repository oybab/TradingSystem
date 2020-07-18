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
    public partial class SettingControl : UserControl
    {
        public SettingControl()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SettingViewModel model = this.DataContext as SettingViewModel;

            model.DisplayMode = 1;

            TextBox tb = sender as TextBox;
            if (null != tb)
                tb.CaretIndex = tb.Text.Length;


            model.KeyboardLittle.IsDisplayKeyboard = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SettingViewModel model = this.DataContext as SettingViewModel;

            model.DisplayMode = 0;

            model.KeyboardLittle.IsDisplayKeyboard = false;
            
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            
            SettingViewModel model = this.DataContext as SettingViewModel;

            CheckBox cb = sender as CheckBox;
            if (null == cb)
                return;

            if (model.IsLocalPrint)
            {
                model.IsLocalPrint = false;
            }
            else
            {
                model.IsLocalPrint = true;
            }
        }

        
    }
}
