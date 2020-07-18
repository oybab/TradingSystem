using Oybab.Res.View.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class KeyViewModel : ViewModelBase
    {
        private Action<string> Command;

        internal KeyViewModel(Action<string> Command)
        {
            this.Command = Command;
        }




        private string _keyMsg = "";
        /// <summary>
        /// 显示内容
        /// </summary>
        public string KeyMsg
        {
            get { return _keyMsg; }
            set
            {
                _keyMsg = value;
                OnPropertyChanged("KeyMsg");
            }
        }



        private int _keyMsgMode = 0;
        /// <summary>
        /// 是否显示模式
        /// </summary>
        public int KeyMsgMode
        {
            get { return _keyMsgMode; }
            set
            {
                _keyMsgMode = value;
                OnPropertyChanged("KeyMsgMode");
            }
        }


        private int _keyMsgImageMode = 0;
        /// <summary>
        /// 显示图类型
        /// </summary>
        public int KeyMsgImageMode
        {
            get { return _keyMsgImageMode; }
            set
            {
                _keyMsgImageMode = value;
                OnPropertyChanged("KeyMsgImageMode");
            }
        }


        /// <summary>
        /// 输入重试
        /// </summary>
        private RelayCommand _retryCommand;
        public ICommand RetryCommand
        {
            get
            {
                if (_retryCommand == null)
                {
                    _retryCommand = new RelayCommand(param => Command("Retry"));
                }
                return _retryCommand;
            }
        }


        /// <summary>
        /// 输入退出
        /// </summary>
        private RelayCommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(param => Command("Exit"));
                }
                return _exitCommand;
            }
        }




    }
}
