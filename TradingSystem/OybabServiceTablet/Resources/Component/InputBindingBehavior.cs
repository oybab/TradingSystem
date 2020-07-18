using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Oybab.ServiceTablet.Resources.Component
{
    public class InputBindingBehavior
    {
        public static bool GetPropagateInputBindingsToWindow(FrameworkElement obj)
        {
            return (bool)obj.GetValue(PropagateInputBindingsToWindowProperty);
        }

        public static void SetPropagateInputBindingsToWindow(FrameworkElement obj, bool value)
        {
            obj.SetValue(PropagateInputBindingsToWindowProperty, value);
        }

        public static readonly DependencyProperty PropagateInputBindingsToWindowProperty =
            DependencyProperty.RegisterAttached("PropagateInputBindingsToWindow", typeof(bool), typeof(InputBindingBehavior),
            new PropertyMetadata(false, OnPropagateInputBindingsToWindowChanged));

        private static void OnPropagateInputBindingsToWindowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((FrameworkElement)d).Loaded += frameworkElement_Loaded;
        }

        private static void frameworkElement_Loaded(object sender, RoutedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.Loaded -= frameworkElement_Loaded;

            var window = Window.GetWindow(frameworkElement);
            if (window == null)
            {
                return;
            }

            // Move input bindings from the FrameworkElement to the window.
            for (int i = frameworkElement.InputBindings.Count - 1; i >= 0; i--)
            {
                var inputBinding = (InputBinding)frameworkElement.InputBindings[i];
                window.InputBindings.Add(inputBinding);
                frameworkElement.InputBindings.Remove(inputBinding);
            }
        }
    }
}
