using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class LoginViewModel: ViewModelBase
    {
        private UIElement _element;
        private Action _loginSuccess;
        private Action _regPage;


        public LoginViewModel(UIElement element, Panel drawPanel, Panel pricePanel, Panel barcodePanel, Panel cardPanel, Panel langPanel, Action loginSuccess, Action regPage)
        {
            this._element = element;
            this._loginSuccess = loginSuccess;
            this._regPage = regPage;



            _keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
            _setting = new SettingViewModel(element, drawPanel, pricePanel, barcodePanel, cardPanel, langPanel);

            // 添加处理事件
            this._element.AddHandler(PublicEvents.PopupEvent, new RoutedEventHandler(HandlePopop), true);

            _msgs = new MsgViewModel(SetMsgCommand);

        }

        int InitCount = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            AdminNo = "";
            Password = "";
            TempPassword = "";
            IsDisPlayKeyboard = false;
            IsLoading = false;
            IsMsg = false;
            Msg = "";
            AdminNoEnable = false;
            PasswordEnable = false;
            
            AlertMsg = "";
            AlertMsgMode = 0;

            ClearFocus();

            Common.GetCommon().ReadBak();

            if (!string.IsNullOrWhiteSpace(Resources.GetRes().LastLoginAdminNo))
                AdminNo = Resources.GetRes().LastLoginAdminNo;

            if (InitCount > 0)
            {
                HideSetting = true;
                if (OperatesService.GetOperates().IsAdminUsing)
                {
                    OperatesService.GetOperates().IsAdminUsing = false;
                    SystemSounds.Asterisk.Play();

                    IsMsg = true;
                    Msg = string.Format(Resources.GetRes().GetString("Exception_AdminExists"), Common.GetCommon().GetFormat());
                }
            }
                

            ++InitCount;

        }



        private KeyboardLittleViewModel _keyboardLittle;
        /// <summary>
        /// 小键盘
        /// </summary>
        public KeyboardLittleViewModel KeyboardLittle
        {
            get { return _keyboardLittle; }
            set
            {
                _keyboardLittle = value;
                OnPropertyChanged("KeyboardLittle");
            }
        }



        private SettingViewModel _setting;
        /// <summary>
        /// 设置
        /// </summary>
        public SettingViewModel Setting
        {
            get { return _setting; }
            set
            {
                _setting = value;
                OnPropertyChanged("Setting");
            }
        }


        private MsgViewModel _msgs;
        /// <summary>
        /// 消息框
        /// </summary>
        public MsgViewModel Msgs
        {
            get { return _msgs; }
            set
            {
                _msgs = value;
                OnPropertyChanged("Msgs");
            }
        }



        /// <summary>
        /// 消息命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetMsgCommand(string no)
        {
            // 确定
            if (no == "OK")
            {

            }
            // 是
            else if (no == "Yes")
            {

            }
            // 否
            else if (no == "No")
            {

            }

            PopupRoutedEventArgs popupArgs = MsgList.Dequeue();
            if (null != popupArgs.Operate)
                popupArgs.Operate(no);




           


            Msgs.AlertMsgMode = false;
            InitialMsg();

        }
        /// <summary>
        /// 信息列表
        /// </summary>
        private Queue<PopupRoutedEventArgs> MsgList = new Queue<PopupRoutedEventArgs>();




        /// <summary>
        /// 处理弹出按钮路由
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandlePopop(object sender, RoutedEventArgs args)
        {
            PopupRoutedEventArgs popupArgs = args as PopupRoutedEventArgs;
            if (null != popupArgs)
            {
                switch (popupArgs.PopupType)
                {
                    case PopupType.Information:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    case PopupType.Warn:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    case PopupType.Error:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    case PopupType.Question:
                        MsgList.Enqueue(popupArgs);
                        InitialMsg();
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// 初始化信息
        /// </summary>
        private void InitialMsg()
        {
            // 没有显示窗口, 并消息堆里大于0时执行
            if (!Msgs.AlertMsgMode && MsgList.Count > 0)
            {
                PopupRoutedEventArgs popupArgs = MsgList.Peek();

                // 更改显示模式
                this.Msgs.ChangeMode(popupArgs);

            }
        }


        private string _adminNo = "";
        /// <summary>
        /// 管理员编号
        /// </summary>
        public string AdminNo
        {
            get { return _adminNo; }
            set
            {
                _adminNo = value;
                OnPropertyChanged("AdminNo");
            }
        }

        private string _password = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                
                _password = value;
                OnPropertyChanged("Password");

                string temp = "";
                //设置虚拟密码
                for (int i = 0; i < _password.Length; i++)
                {
                    temp += "●";
                }
                TempPassword = temp;
            }
        }


        private string _tempPassword = "";
        /// <summary>
        /// 临时密码(用于显示)
        /// </summary>
        public string TempPassword
        {
            get { return _tempPassword; }
            set
            {
                _tempPassword = value;
                OnPropertyChanged("TempPassword");
            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        private RelayCommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                {
                    _loginCommand = new RelayCommand(param =>
                    {
                        

                        IsMsg = false;
                        Msg = "";

                        new Action(() =>
                        {
                            //判断是否空
                            if (AdminNo.Trim().Equals("") || Password.Trim().Equals(""))
                            {
                                SystemSounds.Asterisk.Play();
                            }
                            else
                            {
                                if (IsLoading == true)
                                    return;

                                IsLoading = true;

                                //查询当前输入的密码
                                try
                                {
                                    //检查路径和数据库
                                    //获取密码

                                    ResultModel result = Common.GetCommon().Load(AdminNo, Password);

                                    //如果验证成功
                                    //修改成功
                                    if (result.Result)
                                    {
                                        if (!result.ValidResult)
                                        {
                                            Exit();
                                            return;
                                        }

                                        Resources.GetRes().LastLoginAdminNo = AdminNo;
                                        Common.GetCommon().SetBak();


                                        // 没有权限就不要进了
                                        if (!Common.GetCommon().IsAllowPlatform(2))
                                        {
                                            SystemSounds.Asterisk.Play();
                                            //IsLoading = false;
                                            AlertMsgMode = 4;
                                            AlertMsg = Resources.GetRes().GetString("PermissionDenied");
                                        }
                                        else if (result.IsAdminUsing)
                                        {
                                            SystemSounds.Asterisk.Play();
                                            AlertMsgMode = 2;
                                            AlertMsg = Resources.GetRes().GetString("Exception_AdminUsing");
                                        }
                                        // 如果快过期了则提示
                                        else if (Resources.GetRes().ExpiredRemainingDays != -1 && Resources.GetRes().ExpiredRemainingDays <= 30)
                                        {

                                            SystemSounds.Asterisk.Play();
                                            AlertMsgMode = 1;
                                            AlertMsg = string.Format(Resources.GetRes().GetString("SoftwareSoonExpired"), Resources.GetRes().ExpiredRemainingDays, "");
                                        }
                                        else
                                        {
                                            AlertMsgMode = 0;
                                            AlertMsg = "";
                                            _loginSuccess();
                                        }
                                    }
                                    else
                                    {
                                        // 过期
                                        if (result.IsExpired)
                                        {
                                            SystemSounds.Asterisk.Play();
                                            AlertMsgMode = 3;
                                            AlertMsg = Resources.GetRes().GetString("SoftwareExpired");
                                        }
                                        //验证成功,只是登录失败
                                        if (result.ValidResult)
                                        {
                                            SystemSounds.Asterisk.Play();
                                            IsLoading = false;
                                            IsMsg = true;
                                            Msg = string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Login"));
                                        }
                                        //验证失败
                                        else
                                        {
                                            SystemSounds.Asterisk.Play();
                                            IsLoading = false;
                                            IsMsg = true;
                                            Msg = Resources.GetRes().GetString("PwdError");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    
                                    ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                    {
                                        SystemSounds.Asterisk.Play();
                                        IsLoading = false;
                                        IsMsg = true;
                                        Msg = message;
                                    }));
                                }

                            }
                        }).BeginInvoke(null, null);
                    });
                }
                return _loginCommand;
            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        private RelayCommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(param => Exit());
                }
                return _exitCommand;
            }
        }



        /// <summary>
        /// 重置
        /// </summary>
        private RelayCommand _resetCommand;
        public ICommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(param =>
                    {
                        AdminNo = "";
                        Password = "";
                        ClearFocus();
                    });
                }
                return _resetCommand;
            }
        }



        /// <summary>
        /// 注册是按钮
        /// </summary>
        private RelayCommand _yesCommand;
        public ICommand YesCommand
        {
            get
            {
                if (_yesCommand == null)
                {
                    _yesCommand = new RelayCommand(param =>
                    {
                        AlertMsgMode = 0;
                        AlertMsg = "";
                        _regPage();
                    });
                }
                return _yesCommand;
            }
        }

        /// <summary>
        /// 注册不按钮
        /// </summary>
        private RelayCommand _noCommand;
        public ICommand NoCommand
        {
            get
            {
                if (_noCommand == null)
                {
                    _noCommand = new RelayCommand(param =>
                    {
                        if (AlertMsgMode == 1 && Resources.GetRes().ExpiredRemainingDays != -1 && Resources.GetRes().ExpiredRemainingDays <= 7)
                        {
                            SystemSounds.Asterisk.Play();

                            AlertMsgMode = 2;
                            AlertMsg = string.Format(Resources.GetRes().GetString("SoftwareExpiredWarn"));
                        }
                        else
                        {
                            AlertMsgMode = 0;
                            AlertMsg = "";
                            _loginSuccess();
                        }
                    });
                }
                return _noCommand;
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
                        if (!_setting.IsDisplay)
                        {
                            if (_keyboardLittle.IsDisplayKeyboard)
                            {
                                _keyboardLittle.OKCommand.Execute(null);
                            }else
                            {
                                this.LoginCommand.Execute(null);
                            }
                        }
                    });
                }
                return _enterCommand;
            }
        }





        /// <summary>
        /// 打开设置
        /// </summary>
        private RelayCommand _settingCommand;
        public ICommand SettingCommand
        {
            get
            {
                if (_settingCommand == null)
                {
                    _settingCommand = new RelayCommand(param =>
                    {
                        Setting.Initial();
                        Setting.Show();
                    });
                }
                return _settingCommand;
            }
        }



        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (AdminNoEnable && AdminNo.Length < 16)
            {
                AdminNo += no;
            }
            else if (PasswordEnable && Password.Length < 16)
            {
                Password += no;
            }
        }


        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (AdminNoEnable && AdminNo.Length > 0)
            {
                if (IsAll)
                    AdminNo = "";
                else
                    AdminNo = AdminNo.Remove(AdminNo.Length - 1);
            }
            else if (PasswordEnable && Password.Length > 0)
            {
                if (IsAll)
                    Password = "";
                else
                    Password = Password.Remove(Password.Length - 1);
            }
        }

        /// <summary>
        /// 去掉焦点
        /// </summary>
        private void ClearFocus()
        {
            var scope = FocusManager.GetFocusScope(_element); // elem is the UIElement to unfocus
            FocusManager.SetFocusedElement(scope, null); // remove logical focus
            Keyboard.ClearFocus(); // remove keyboard focus
            _element.Focus();
        }



        /// <summary>
        /// 命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetCommand(string no)
        {
            // 确定
            if (no == "OK")
            {
                IsDisPlayKeyboard = false;
                AdminNoEnable = false;
                PasswordEnable = false;
                ClearFocus();
            }
            // 取消
            else if (no == "Cancel")
            {
                RemoveText(true);
            }
            // 删除
            else if (no == "Del")
            {
                RemoveText(false);
            }
        }




        private bool _adminNoEnable;
        /// <summary>
        /// 管理员编号是否可用
        /// </summary>
        public bool AdminNoEnable
        {
            get { return _adminNoEnable; }
            set
            {
                _adminNoEnable = value;
                if (value)
                {
                    PasswordEnable = false;
                    IsDisPlayKeyboard = true;
                }
                OnPropertyChanged("AdminNoEnable");
            }
        }


        private bool _passwordEnable;
        /// <summary>
        /// 密码是否可用
        /// </summary>
        public bool PasswordEnable
        {
            get { return _passwordEnable; }
            set
            {
                _passwordEnable = value;
                if (value)
                {
                    AdminNoEnable = false;
                    IsDisPlayKeyboard = true;
                }
                OnPropertyChanged("PasswordEnable");
            }
        }





        private string _msg = " ";
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg
        {
            get { return _msg; }
            set
            {
                _msg = value;
                OnPropertyChanged("Msg");
            }
        }


        private string _alertMsg = " ";
        /// <summary>
        /// 弹出提示信息
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



        private int _alertMsgMode = 0;
        /// <summary>
        /// 弹出提示信息类型
        /// </summary>
        public int AlertMsgMode
        {
            get { return _alertMsgMode; }
            set
            {
                _alertMsgMode = value;
                OnPropertyChanged("AlertMsgMode");
            }
        }




        private bool _isMsg;
        /// <summary>
        /// 显示信息
        /// </summary>
        public bool IsMsg
        {
            get { return _isMsg; }
            set
            {
                _isMsg = value;
                OnPropertyChanged("IsMsg");
            }
        }



        private bool _isLoading;
        /// <summary>
        /// 显示正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }



        private bool _isDisPlayKeyboard;
        /// <summary>
        /// 显示键盘
        /// </summary>
        public bool IsDisPlayKeyboard
        {
            get { return _isDisPlayKeyboard; }
            set
            {
                _isDisPlayKeyboard = value;
                KeyboardLittle.IsDisplayKeyboard = value;
                OnPropertyChanged("IsDisPlayKeyboard");
            }
        }



        private bool _hideSetting = false;
        /// <summary>
        /// 隐藏设置
        /// </summary>
        public bool HideSetting
        {
            get { return _hideSetting; }
            set
            {
                _hideSetting = value;
                OnPropertyChanged("HideSetting");
            }
        }


        /// <summary>
        /// 退出
        /// </summary>
        private void Exit()
        {
            Common.GetCommon().Close();
            _element.Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.Shutdown(0);
            }));
        }

    }
}
