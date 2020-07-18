using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.Res.View.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Models
{
    public sealed class ProductTypeStateModel : ViewModelBase
    {
        internal ProductType ProductType { get; set; }


        private string _productTypeName;
        /// <summary>
        /// 产品类型名
        /// </summary>
        public string ProductTypeName
        {
            get
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    return ProductType.ProductTypeName0;
                else if (Resources.GetRes().MainLangIndex == 1)
                    return ProductType.ProductTypeName1;
                else if (Resources.GetRes().MainLangIndex == 2)
                    return ProductType.ProductTypeName2;
                else
                    return "";
            }
            set
            {
                _productTypeName = value;
                OnPropertyChanged("ProductTypeName");
            }
        }

        /// <summary>
        /// 语言变了
        /// </summary>
        internal void LanguageChange()
        {
            OnPropertyChanged("ProductTypeName");
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
