using Oybab.Res;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Data;

namespace Oybab.Res.View.Converters
{
    /// <summary>
    /// 加载语言
    /// </summary>
    public sealed class LangConverter : IValueConverter, INotifyPropertyChanged
    {

        #region Instance
        /// <summary>
        /// For Controls
        /// </summary>
        private static LangConverter _instance;
        public static LangConverter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LangConverter();
                return _instance;
            }
        }

        public LangConverter()
        {
            _instance = this;
        }


        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //return "xxx";
            return Resources.GetRes().GetString(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public string TStr { get { return "TStr"; } }
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Change the culture for the application.
        /// </summary>
        /// <param name="culture">Full culture name</param>
        internal void ChangeCulture(int index)
        {
            Resources.GetRes().ReloadResources(index);

            // notify that the culture has changed
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs("TStr"));
        }

    }
}
