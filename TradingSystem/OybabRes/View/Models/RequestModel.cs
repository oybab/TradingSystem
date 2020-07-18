using Oybab.DAL;
using Oybab.Res.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Models
{
    public sealed class RequestModel : ViewModelBase
    {

        internal Request Request { get; set; }


        private string _requestName;
        /// <summary>
        /// 产品名
        /// </summary>
        public string RequestName
        {
            get
            {

                if (Resources.GetRes().MainLangIndex == 0)
                    return Request.RequestName0;
                else if (Resources.GetRes().MainLangIndex == 1)
                    return Request.RequestName1;
                else if (Resources.GetRes().MainLangIndex == 2)
                    return Request.RequestName2;
                else
                    return "";
            }
            set
            {
                _requestName = value;
                OnPropertyChanged("RequestName");
            }
        }


        private bool _isSelected = false;
        /// <summary>
        /// 选中状态
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        
    }
}
