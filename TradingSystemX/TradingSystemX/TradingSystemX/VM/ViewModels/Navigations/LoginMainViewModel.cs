using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.ViewModels.Navigations
{
    internal sealed class LoginMainViewModel : ViewModelBase
    {
        private Page _element;
        public LoginMainViewModel(Page _element)
        {
            this._element = _element;
            this.MessageBox = new MessageBoxViewModel(this._element);

            // 注册弹出事件
            QueueMessageBoxNotification.Instance.NotificationMessageBox += this.MessageBox.Instance_NotificationMessageBox;
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
