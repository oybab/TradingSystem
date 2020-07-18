using Oybab.DAL;
using Oybab.Res.View.Commands;
using Oybab.Res.View.ViewModels;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oybab.Res.View.Models
{
    public sealed class BalanceItemModel : ViewModelBase
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

        internal RoomViewModel viewModel { get; set; }




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
        public ICommand SelectCommand
        {
            get;
            internal set;
        }

    }
}
