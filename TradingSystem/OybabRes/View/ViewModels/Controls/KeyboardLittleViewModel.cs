using Oybab.Res.View.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class KeyboardLittleViewModel : ViewModelBase
    {
        private Action<string> Input;
        private Action<string> Command;

        internal KeyboardLittleViewModel(Action<string> Input, Action<string> Command)
        {
            this.Input = Input;
            this.Command = Command;
        }


        private bool _isDisplayKeyboard;
        /// <summary>
        /// 显示键盘
        /// </summary>
        public bool IsDisplayKeyboard
        {
            get { return _isDisplayKeyboard; }
            set
            {
                _isDisplayKeyboard = value;
                OnPropertyChanged("IsDisplayKeyboard");
            }
        }



        /// <summary>
        /// 输入数字1
        /// </summary>
        private RelayCommand _no1Command;
        public ICommand No1Command
        {
            get
            {
                if (_no1Command == null)
                {
                    _no1Command = new RelayCommand(param => Input("1"));
                }
                return _no1Command;
            }
        }


        /// <summary>
        /// 输入数字2
        /// </summary>
        private RelayCommand _no2Command;
        public ICommand No2Command
        {
            get
            {
                if (_no2Command == null)
                {
                    _no2Command = new RelayCommand(param => Input("2"));
                }
                return _no2Command;
            }
        }


        /// <summary>
        /// 输入数字3
        /// </summary>
        private RelayCommand _no3Command;
        public ICommand No3Command
        {
            get
            {
                if (_no3Command == null)
                {
                    _no3Command = new RelayCommand(param => Input("3"));
                }
                return _no3Command;
            }
        }


        /// <summary>
        /// 输入数字4
        /// </summary>
        private RelayCommand _no4Command;
        public ICommand No4Command
        {
            get
            {
                if (_no4Command == null)
                {
                    _no4Command = new RelayCommand(param => Input("4"));
                }
                return _no4Command;
            }
        }


        /// <summary>
        /// 输入数字5
        /// </summary>
        private RelayCommand _no5Command;
        public ICommand No5Command
        {
            get
            {
                if (_no5Command == null)
                {
                    _no5Command = new RelayCommand(param => Input("5"));
                }
                return _no5Command;
            }
        }


        /// <summary>
        /// 输入数字6
        /// </summary>
        private RelayCommand _no6Command;
        public ICommand No6Command
        {
            get
            {
                if (_no6Command == null)
                {
                    _no6Command = new RelayCommand(param => Input("6"));
                }
                return _no6Command;
            }
        }


        /// <summary>
        /// 输入数字7
        /// </summary>
        private RelayCommand _no7Command;
        public ICommand No7Command
        {
            get
            {
                if (_no7Command == null)
                {
                    _no7Command = new RelayCommand(param => Input("7"));
                }
                return _no7Command;
            }
        }


        /// <summary>
        /// 输入数字8
        /// </summary>
        private RelayCommand _no8Command;
        public ICommand No8Command
        {
            get
            {
                if (_no8Command == null)
                {
                    _no8Command = new RelayCommand(param => Input("8"));
                }
                return _no8Command;
            }
        }


        /// <summary>
        /// 输入数字9
        /// </summary>
        private RelayCommand _no9Command;
        public ICommand No9Command
        {
            get
            {
                if (_no9Command == null)
                {
                    _no9Command = new RelayCommand(param => Input("9"));
                }
                return _no9Command;
            }
        }


        /// <summary>
        /// 输入数字0
        /// </summary>
        private RelayCommand _no0Command;
        public ICommand No0Command
        {
            get
            {
                if (_no0Command == null)
                {
                    _no0Command = new RelayCommand(param => Input("0"));
                }
                return _no0Command;
            }
        }


        /// <summary>
        /// 输入数字.
        /// </summary>
        private RelayCommand _noPointCommand;
        public ICommand NoPointCommand
        {
            get
            {
                if (_noPointCommand == null)
                {
                    _noPointCommand = new RelayCommand(param => Input("."));
                }
                return _noPointCommand;
            }
        }



        /// <summary>
        /// 删除数字
        /// </summary>
        private RelayCommand _delCommand;
        public ICommand DelCommand
        {
            get
            {
                if (_delCommand == null)
                {
                    _delCommand = new RelayCommand(param => Command("Del"));
                }
                return _delCommand;
            }
        }



        /// <summary>
        /// 确定按钮
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
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(param => Command("Cancel"));
                }
                return _cancelCommand;
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
                    _enterCommand = new RelayCommand(param => Command("OK"));
                }
                return _enterCommand;
            }
        }
    }
}
