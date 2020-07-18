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
    public partial class KeyboardLittleControl : UserControl
    {
        public KeyboardLittleControl()
        {
            InitializeComponent();

            //this.IsVisibleChanged -= MsgControl_IsVisibleChanged;
            //this.IsVisibleChanged += MsgControl_IsVisibleChanged;
        }




        /// <summary>
        /// 获取焦点后记得把它设置成已选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UIElement visibilityElement = sender as UIElement;

            if (visibilityElement.IsVisible == true)
            {
                visibilityElement.Focus();
            }
            else
            {
                // Gets the element with keyboard focus.
                UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

                // Change keyboard focus.
                if (elementWithFocus != null)
                {
                    elementWithFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));

                }
            }

        }
    }
}
