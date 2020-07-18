using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
    public partial class ProductsControl : UserControl
    {
        public ProductsControl()
        {
            InitializeComponent();


            //_element.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    _scrollViewerProducts.ScrollToVerticalOffset(0);
            //}));

          

        }

        internal void InitialProduct()
        {
            // 已选有改动时刷新索引
            ((INotifyCollectionChanged)lbList.ItemsSource).CollectionChanged -= ProductsControl_CollectionChanged;
            ((INotifyCollectionChanged)lbList.ItemsSource).CollectionChanged += ProductsControl_CollectionChanged;
        }

        private void ProductsControl_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            if (e.NewStartingIndex == 0 && svProductList.VerticalOffset != 0)
            {
                svProductList.Stop();
                svProductList.ScrollToVerticalOffset(0);
            }
        }
    }
}
