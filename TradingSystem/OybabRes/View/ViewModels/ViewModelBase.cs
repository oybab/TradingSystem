using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public bool IsLong { get; set; }
        public bool IsIgnore { get; set; }

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
