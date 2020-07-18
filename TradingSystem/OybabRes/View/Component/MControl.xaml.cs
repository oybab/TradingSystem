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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oybab.Res.View.Component
{
    /// <summary>
    /// MControl.xaml 的交互逻辑
    /// </summary>
    public sealed partial class MControl : UserControl
    {
        DoubleAnimationUsingKeyFrames verticalAnimation = new DoubleAnimationUsingKeyFrames();
        Storyboard storyBoard = new Storyboard();
        
        public MControl()
        {
            InitializeComponent();

            storyBoard.Children.Add(verticalAnimation);

            
        }

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset",
           typeof(double), typeof(MControl), new PropertyMetadata(VerticalOffsetPropertyChanged));
        public double VerticalOffset
        {
            get { return (double)this.GetValue(VerticalOffsetProperty); }
            set {

                double NewValue = value;
                if (value <= 0)
                {
                    NewValue = 0;
                }
                else if (value >= this.scroller.ScrollableHeight)
                {
                    NewValue = this.scroller.ScrollableHeight;
                }
                else
                {
                    NewValue = value;
                }
                this.SetValue(VerticalOffsetProperty, NewValue);
            
            }
        }
        private static void VerticalOffsetPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            MControl cThis = sender as MControl;
            double value = (double)args.NewValue;

            System.Diagnostics.Debug.WriteLine("Animation: " + value);
            cThis.scroller.ScrollToVerticalOffset(value);

        }
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(object), typeof(MControl), null);
        public object Items
        {
            get { return (object)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }


        


        internal void StartScroll(MControl scrollViewer, double ToValue, bool IsMove, int? time = null)
        {

            if (IsMove)
            {

                verticalAnimation.KeyFrames.Clear();
                verticalAnimation.Duration = new TimeSpan(0, 0, 0, 0, 20);
                    verticalAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(ToValue, new TimeSpan(0, 0, 0, 0, 20)));

                    System.Diagnostics.Debug.WriteLine("Time: " + ToValue);
                    storyBoard.Begin();

            }
            else
            {
                storyBoard.BeginAnimation(MControl.VerticalOffsetProperty, null);
                scrollViewer.VerticalOffset = ToValue;

                System.Diagnostics.Debug.WriteLine("Hand: " + ToValue);
            }

        }


        internal void StopScroll(MControl scrollViewer)
        {
            double value = scrollViewer.VerticalOffset;
            storyBoard.BeginAnimation(MControl.VerticalOffsetProperty, null);
            scrollViewer.VerticalOffset = value;
        }

    }
}
