using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.Converters
{
    /// <summary>
    /// 转换数字
    /// </summary>
    internal sealed class PriceConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (null == parameter)
            {
                return Resources.Instance.PrintInfo.PriceSymbol;
            }
            else
            {
                return (parameter as string).Replace("￥", Resources.Instance.PrintInfo.PriceSymbol);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
