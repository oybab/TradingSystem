using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Reports;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;


namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class SystemViewModel: ViewModelBase
    {
        private UIElement _element;


        public SystemViewModel(UIElement element)
        {
            this._element = element;
            this.ChangePassword = new ChangePasswordViewModel(element);
            

        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
        }




       
        private ChangePasswordViewModel _changePassword;
        /// <summary>
        /// 修改密码
        /// </summary>
        public ChangePasswordViewModel ChangePassword
        {
            get { return _changePassword; }
            set
            {
                _changePassword = value;
                OnPropertyChanged("ChangePassword");
            }
        }


        /// <summary>
        /// 后退按钮
        /// </summary>
        private RelayCommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(param => _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Back)));
                }
                return _backCommand;
            }
        }


        /// <summary>
        /// 更改密码按钮
        /// </summary>
        private RelayCommand _changePasswordCommand;
        public ICommand ChangePasswordCommand
        {
            get
            {
                if (_changePasswordCommand == null)
                {
                    _changePasswordCommand = new RelayCommand(param =>
                    {
                        ChangePassword.Show();
                    });
                }
                return _changePasswordCommand;
            }
        }


        /// <summary>
        /// 更改密码按钮
        /// </summary>
        private RelayCommand _financeLogCommand;
        public ICommand FinanceLogCommand
        {
            get
            {
                if (_financeLogCommand == null)
                {
                    _financeLogCommand = new RelayCommand(param =>
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {

                                List<Log> logs;
                                List<Balance> Balances;

                                bool result = OperatesService.GetOperates().ServiceGetLog(1, -1, 0, 0, out Balances, out logs);

                                //获取成功
                                _element.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    if (result)
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, ReloadBalanceList(Balances), null, PopupType.Information));
                                    }
                                }));

                            }
                            catch (Exception ex)
                            {
                                _element.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                                    }));
                                }));
                            }
                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                            }));


                        });
                    });
                }
                return _financeLogCommand;
            }
        }



        /// <summary>
        /// 刷新支付列表
        /// </summary>
        private string ReloadBalanceList(List<Balance> balances)
        {
            string Log = "";
            if (null != balances && balances.Count > 0)
            {

                foreach (var item in balances)
                {
                    Balance currentBalance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.BalanceId).FirstOrDefault();
                   
                    if (Resources.GetRes().MainLangIndex == 0)
                        Log += currentBalance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        Log += currentBalance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        Log += currentBalance.BalanceName2;

                    Log += "  ";

                    Log += Resources.GetRes().PrintInfo.PriceSymbol + item.BalancePrice + "";

                    Log += Environment.NewLine;


                }

            }
            return Log;
        }



        /// <summary>
        /// 退出按钮
        /// </summary>
        private RelayCommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(param =>
                    {
                        _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Exit));
                    });
                }
                return _exitCommand;
            }
        }





        /// <summary>
        /// 外部账单按钮
        /// </summary>
        private RelayCommand _takeoutCommand;
        public ICommand TakeoutCommand
        {
            get
            {
                if (_takeoutCommand == null)
                {
                    _takeoutCommand = new RelayCommand(param =>
                    {
                        _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Takeout));
                    });
                }
                return _takeoutCommand;
            }
        }





        /// <summary>
        /// 支出管理按钮
        /// </summary>
        private RelayCommand _importCommand;
        public ICommand ImportCommand
        {
            get
            {
                if (_importCommand == null)
                {
                    _importCommand = new RelayCommand(param =>
                    {
                        _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Import));
                    });
                }
                return _importCommand;
            }
        }

    }
}
