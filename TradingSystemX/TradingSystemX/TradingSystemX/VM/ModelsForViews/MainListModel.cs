using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.Converters;
using Oybab.TradingSystemX.VM.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.ModelsForViews
{
    internal sealed class MainListModel : ViewModelBase
    {
        /// <summary>
        /// 语言更改
        /// </summary>
        internal void LangChange()
        {
            OnPropertyChanged("DisplayName");
        }

        private string _name = "";
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("DisplayName");
            }
        }


        private bool _visibility = true;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visibility
        {
            get { return _visibility; }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }


        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get { if (!string.IsNullOrWhiteSpace(Name))
                    return Resources.Instance.GetString(Name);
                else
                    return "";
            }
            
        }



        /// <summary>
        /// 跳转
        /// </summary>
        public Command GoCommand
        {
            get;
            internal set;
        }
    }
}
