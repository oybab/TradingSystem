using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Oybab.TradingSystemX.VM.Converters;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.VM.ViewModels.Pages;

namespace Oybab.TradingSystemX.VM.ViewModels.Navigations
{
    internal sealed class MasterDetailViewModel : ViewModelBase
    {
        private Page _element;

        public MasterDetailViewModel(Page _element)
        {
            this._element = _element;
            this.MessageBox = new MessageBoxViewModel(this._element);

            // 注册弹出事件
            QueueMessageBoxNotification.Instance.NotificationMessageBox += this.MessageBox.Instance_NotificationMessageBox;

        }

       


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
        }





        private MessageBoxViewModel _messageBox;
        /// <summary>
        /// 弹出提醒
        /// </summary>
        public MessageBoxViewModel MessageBox
        {
            get { return _messageBox; }
            set
            {
                _messageBox = value;
                OnPropertyChanged("MessageBox");
            }
        }

    }
}
