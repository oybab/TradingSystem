using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class MsgViewModel : ViewModelBase
    {
        private Action<string> Command;

        internal MsgViewModel(Action<string> Command)
        {
            this.Command = Command;
        }




        private string _alertMsg = "";
        /// <summary>
        /// 显示内容
        /// </summary>
        public string AlertMsg
        {
            get { return _alertMsg; }
            set
            {
                _alertMsg = value;
                OnPropertyChanged("AlertMsg");
            }
        }



        private bool _alertMsgMode = false;
        /// <summary>
        /// 是否显示模式
        /// </summary>
        public bool AlertMsgMode
        {
            get { return _alertMsgMode; }
            set
            {
                _alertMsgMode = value;
                OnPropertyChanged("AlertMsgMode");
            }
        }


        private int _alertMsgImageMode = 0;
        /// <summary>
        /// 显示图类型
        /// </summary>
        public int AlertMsgImageMode
        {
            get { return _alertMsgImageMode; }
            set
            {
                _alertMsgImageMode = value;
                OnPropertyChanged("AlertMsgImageMode");
            }
        }
        


        private int _alertMsgButtonMode = 0;
        /// <summary>
        /// 显示按钮类型(1确定2是否
        /// </summary>
        public int AlertMsgButtonMode
        {
            get { return _alertMsgButtonMode; }
            set
            {
                _alertMsgButtonMode = value;
                OnPropertyChanged("AlertMsgButtonMode");
            }
        }



        /// <summary>
        /// 输入回车
        /// </summary>
        private RelayCommand _enterCommand;
        public ICommand EnterCommand
        {
            get
            {
                if (_enterCommand == null)
                {
                    _enterCommand = new RelayCommand(param =>
                    {
                        // 确认
                        if (AlertMsgButtonMode == 1)
                        {
                            OKCommand.Execute(null);
                        }
                        // 否
                        else if (AlertMsgButtonMode == 2)
                        {
                            NoCommand.Execute(null);
                        }
                    });
                }
                return _enterCommand;
            }
        }




        /// <summary>
        /// 输入确定
        /// </summary>
        private RelayCommand _oKCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_oKCommand == null)
                {
                    _oKCommand = new RelayCommand(param => Command("OK"));
                }
                return _oKCommand;
            }
        }


        /// <summary>
        /// 输入是
        /// </summary>
        private RelayCommand _yesCommand;
        public ICommand YesCommand
        {
            get
            {
                if (_yesCommand == null)
                {
                    _yesCommand = new RelayCommand(param => Command("YES"));
                }
                return _yesCommand;
            }
        }



        /// <summary>
        /// 输入不
        /// </summary>
        private RelayCommand _noCommand;
        public ICommand NoCommand
        {
            get
            {
                if (_noCommand == null)
                {
                    _noCommand = new RelayCommand(param => Command("NO"));
                }
                return _noCommand;
            }
        }



        internal void ChangeMode(PopupRoutedEventArgs popupArgs)
        {
            AlertMsg = popupArgs.Msg;
            if (popupArgs.PopupType == PopupType.Information)
            {
                AlertMsgMode = true;
                AlertMsgButtonMode = 1;
                AlertMsgImageMode = 1;
                SelectedMode = 1;
            }
            else if (popupArgs.PopupType == PopupType.Warn)
            {
                AlertMsgMode = true;
                AlertMsgButtonMode = 1;
                AlertMsgImageMode = 2;
                SelectedMode = 1;
            }
            else if (popupArgs.PopupType == PopupType.Error)
            {
                AlertMsgMode = true;
                AlertMsgButtonMode = 1;
                AlertMsgImageMode = 3;
                SelectedMode = 1;
            }
            else if (popupArgs.PopupType == PopupType.Question)
            {
                AlertMsgMode = true;
                AlertMsgButtonMode = 2;
                AlertMsgImageMode = 4;
                SelectedMode = 1;
            }
        }






        // POS相关逻辑

        private int _selectedMode = 0;
        /// <summary>
        /// 显示模式(0确定,否. 1是)
        /// </summary>
        public int SelectedMode
        {
            get { return _selectedMode; }
            set
            {
                _selectedMode = value;
                OnPropertyChanged("SelectedMode");
            }
        }



        /// <summary>
        /// 处理KEY
        /// </summary>
        /// <param name="args"></param>
        internal void HandleKey(KeyEventArgs args)
        {
            // 如果是功能(如上下左右,换页)
            if (args.Key == Key.Right || args.Key == Key.Left)
            {
                if (args.Key == Key.Right && AlertMsgImageMode == 4 && SelectedMode == 1)
                {
                    SelectedMode = 2;
                }
                else if (args.Key == Key.Left && AlertMsgImageMode == 4 && SelectedMode == 2)
                {
                    SelectedMode = 1;
                }
            }
            // 如果要增加数量或减少数量
            if (args.Key == Key.Escape || args.Key == Key.Enter)
            {
                if (AlertMsgButtonMode == 1)
                {
                    OKCommand.Execute(null);
                }else if (AlertMsgButtonMode == 2)
                {
                    if (args.Key == Key.Escape)
                    {
                        NoCommand.Execute(null);
                    }
                    else if (args.Key == Key.Enter)
                    {
                        if (SelectedMode == 1)
                        {
                            NoCommand.Execute(null);
                        }
                        else if (SelectedMode == 2) {
                            YesCommand.Execute(null);
                        }
                    }
                }
            }



        }
    }
}
