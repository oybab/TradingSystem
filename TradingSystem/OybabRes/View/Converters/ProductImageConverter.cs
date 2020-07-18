using Oybab.DAL;
using Oybab.Res.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Oybab.Res.View.Converters
{
    /// <summary>
    /// 产品图片
    /// </summary>
    public class ProductImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Product product = value as Product;
            if (null == product)
                return null;

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(ImageCache.GetImage(Oybab.Res.Tools.Path.Combine(Resources.GetRes().ROOT, Resources.GetRes().ROOT_FOLDER, Resources.GetRes().PRODUCTS_FOLDER, product.ImageName), product.ImageName), UriKind.Absolute);
            bi.EndInit();
            return bi;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
