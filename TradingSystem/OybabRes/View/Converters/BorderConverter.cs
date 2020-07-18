using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Oybab.Res.View.Converters
{
    /// <summary>
    /// 高度尺寸计算
    /// </summary>
    public sealed class BorderConverter : IMultiValueConverter
    {

        #region Instance
        /// <summary>
        /// For Controls
        /// </summary>
        private static Oybab.Res.View.Converters.BorderConverter _instance;
        public static Oybab.Res.View.Converters.BorderConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Oybab.Res.View.Converters.BorderConverter();
                return _instance;
            }
        }

        #endregion

       

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Any(e => e == DependencyProperty.UnsetValue))
            {
                return new Thickness();
            }

            int mode = int.Parse(parameter.ToString());


            if (mode == 0)
                return PosLine.MarginSelectedList;
            else
                return PosLine.MarginSearchList;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region IsSentinelObject
        public bool IsSentinelObject(object dataContext)
        {
            return disconnectedItem.Equals(dataContext);
        }

        object disconnectedItem = typeof(System.Windows.Data.BindingExpressionBase).GetField("DisconnectedItem", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
        #endregion
    }

    /// <summary>
    /// POS一些参数
    /// </summary>
    public static class PosLine {
        public static Thickness MarginSelectedList { get; set; } = new Thickness();
        public static Thickness MarginSearchList { get; set; } = new Thickness();

        public static int ListCountSelected { get; set; }
        public static int ListCountProduct { get; set; }
    }
}
