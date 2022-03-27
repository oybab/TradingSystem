using Oybab.Res;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.Converters
{
    /// <summary>
    /// 加载自定义字体
    /// </summary>
    internal sealed class CustomFontConverter : IValueConverter
    {

        #region Instance
        /// <summary>
        /// For Custom Font
        /// </summary>
        private static CustomFontConverter _instance;
        public static CustomFontConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomFontConverter();
                return _instance;
            }
        }

        public CustomFontConverter()
        {
            _instance = this;
        }


        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Res.Instance.GetString("UseCustomFont") == "1")
            {
                return parameter;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
