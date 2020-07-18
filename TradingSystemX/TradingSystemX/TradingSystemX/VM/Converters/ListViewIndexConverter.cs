using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Collections.ObjectModel;

namespace Oybab.TradingSystemX.VM.Converters
{
    /// <summary>
    /// 获取列表索引号
    /// </summary>
    internal sealed class ListViewIndexConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            ObservableCollection<DetailsModel> list = value as ObservableCollection<DetailsModel>;

            DetailsModel model = parameter as DetailsModel;

            if (value == null || parameter == null || list == null || model == null) return "?";


            var index = list.Count - list.IndexOf(model);
            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
