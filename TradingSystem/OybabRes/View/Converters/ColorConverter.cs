using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Oybab.Res.View.Converters
{
    /// <summary>
    /// 颜色转换
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        #region Instance
        /// <summary>
        /// For Controls
        /// </summary>
        private static Oybab.Res.View.Converters.ColorConverter _instance;
        public static Oybab.Res.View.Converters.ColorConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Oybab.Res.View.Converters.ColorConverter();
                return _instance;
            }
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush b =  new SolidColorBrush((Color)System.Windows.Media.ColorConverter.ConvertFromString(parameter.ToString()));
            return b;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
