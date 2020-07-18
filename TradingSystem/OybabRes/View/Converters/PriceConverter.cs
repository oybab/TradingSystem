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
    public class PriceConverter : IValueConverter
    {
        #region Instance
        /// <summary>
        /// For Controls
        /// </summary>
        private static Oybab.Res.View.Converters.PriceConverter _instance;
        public static Oybab.Res.View.Converters.PriceConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Oybab.Res.View.Converters.PriceConverter();
                return _instance;
            }
        }

        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == parameter)
            {
                return Resources.GetRes().PrintInfo.PriceSymbol;
            }
            else
            {
                return (parameter as string).Replace("￥", Resources.GetRes().PrintInfo.PriceSymbol);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
