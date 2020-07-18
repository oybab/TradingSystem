using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Oybab.TradingSystemX.VM.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region PropertyChange
        /// <summary>
        /// PropertyChange
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion PropertyChange
    }
}
