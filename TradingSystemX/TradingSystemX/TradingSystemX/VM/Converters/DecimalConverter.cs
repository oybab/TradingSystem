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
    internal sealed class DecimalConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "0";
            decimal thedecimal = (decimal)value;
            return thedecimal.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (string.IsNullOrEmpty(strValue))
                strValue = "0";
            decimal resultdecimal;
            if (decimal.TryParse(strValue, out resultdecimal))
            {
                return resultdecimal;
            }
            return 0;
        }

    }
}
