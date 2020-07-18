using Oybab.DAL;
using System;
using System.Collections.Generic;

using Oybab.TradingSystemX.VM.Commands;

namespace Oybab.TradingSystemX.VM.ModelsForViews
{
    public sealed class BalanceItemModel : ViewModels.ViewModelBase
    {

        private string _test;
        /// <summary>
        /// 雅座编号
        /// </summary>
        public string Text
        {
            get { return _test; }
            set
            {
                _test = value;
                OnPropertyChanged("Text");
            }
        }

        public Balance Balance { get; set; }

        public Member Member { get; set; }

        public Supplier Supplier { get; set; }

        public bool IsBalance { get; set; }

        public bool IsChange { get; set; }

        public override string ToString()
        {
            return Text;
        }



        

        internal Order PayOrder { get; set; }






        private bool _useState;
        /// <summary>
        /// 使用状态(选中或没有)
        /// </summary>
        public bool UseState
        {
            get { return _useState; }
            set
            {
                _useState = value;
                OnPropertyChanged("UseState");
            }
        }


        /// <summary>
        /// 选中
        /// </summary>
        public Xamarin.Forms.Command SelectCommand
        {
            get;
            internal set;
        }

    }
}
