using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.TradingSystemX.Pages.Navigations;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.TradingSystemX.VM.ViewModels.Navigations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class LoginViewModel : ViewModelBase
    {
        private Page _element;
        public LoginViewModel(Page _element)
        {
            this._element = _element;
        }

       

        int InitCount = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            AdminNo = "";
            Password = "";
            IsSavePassword = false;



            if (!string.IsNullOrWhiteSpace(Resources.Instance.LastLoginAdminNo))
                AdminNo = Resources.Instance.LastLoginAdminNo;
            if (Resources.Instance.IsSavePassword && !string.IsNullOrWhiteSpace(Resources.Instance.LastLoginPassword))
                Password = Resources.Instance.LastLoginPassword;
            if (Resources.Instance.IsSavePassword)
                IsSavePassword = Resources.Instance.IsSavePassword;

            if (InitCount > 0)
            {
                HideSetting = true;
                if (OperatesService.Instance.IsAdminUsing)
                {
                    OperatesService.Instance.IsAdminUsing = false;

                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("Exception_AdminExists"), Common.Instance.GetFormat()), MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }
            }
                

            ++InitCount;

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
                if (_password.Length < 16)
                {
                _password = value;
                OnPropertyChanged("Password");

               

                }
            }
        }



        private bool _isSavePassword;
        /// <summary>
        /// 是否保存密码
        /// </summary>
        public bool IsSavePassword
        {
            get { return _isSavePassword; }
            set
            {
                _isSavePassword = value;
                OnPropertyChanged("IsSavePassword");
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

        private bool _isAgreeRequirements = true;
        /// <summary>
        /// 是否同意要求
        /// </summary>
        public bool IsAgreeRequirements
        {
            get { return _isAgreeRequirements; }
            set
            {
                _isAgreeRequirements = value;
                OnPropertyChanged("IsAgreeRequirements");
            }
        }
        
        /// <summary>
        /// 打开隐私政策
        /// </summary>
        private RelayCommand _privacyPolicyCommand;
        public Command PrivacyPolicyCommand
        {
            get
            {
                return _privacyPolicyCommand ?? (_privacyPolicyCommand = new RelayCommand(param =>
                {
                    if (Res.Instance.CurrentLangIndex == 0 || Res.Instance.CurrentLangIndex == 1)
                        Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/privacy.html#chinese")); 
                    else
                        Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/privacy.html#english"));
                }));
            }
        }



        /// <summary>
        /// 打开用户协议
        /// </summary>
        private RelayCommand _userAgreementCommand;
        public Command UserAgreementCommand
        {
            get
            {
                return _userAgreementCommand ?? (_userAgreementCommand = new RelayCommand(param =>
                {
                    if (Res.Instance.CurrentLangIndex == 0 || Res.Instance.CurrentLangIndex == 1)
                        Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/argument.html#chinese"));
                    else
                        Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/argument.html#english"));
                }));
            }
        }



        /// <summary>
        /// 退出
        /// </summary>
        private RelayCommand _loginCommand;
        public Command LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new RelayCommand(async param =>
                {
                    //判断是否空
                    if (AdminNo.Trim().Equals("") || Password.Trim().Equals("") || !IsAgreeRequirements)
                    {

                    }
                    else
                    {
                        IsLoading = true;

                        await ExtX.WaitForLoading();

                        //查询当前输入的密码
                        try
                        {
                            //检查路径和数据库
                            //获取密码

                            ResultModel result = await Common.Instance.Load(AdminNo, Password);

                            string message = null;
                            //如果验证成功
                            //修改成功
                            if (result.Result)
                            {
                                if (!result.ValidResult)
                                {
                                    // 最好永久停留在这里
                                }

                                else
                                {

                                    // 没有权限就不要进了
                                    if (!Common.Instance.IsAllowPlatform(3))
                                    {
                                        // 退出
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PermissionDenied"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (string operate) =>
                                        {
                                            CloseCommand.Execute(null);
                                        }, null);
                                    }
                                    else if (result.IsAdminUsing)
                                    {
                                        // 提醒管理员使用中
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("Exception_AdminUsing"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (string operate) =>
                                        {
                                                // 登录
                                                Login();
                                        }, null);
                                    }
                                    // 如果快过期了则提示
                                    else if (Resources.Instance.ExpiredRemainingDays != -1 && Resources.Instance.ExpiredRemainingDays <= 30)
                                    {
                                        message = string.Format(Resources.Instance.GetString("SoftwareSoonExpired"), Resources.Instance.ExpiredRemainingDays, "");


                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, message, MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string operate) =>
                                        {
                                            if (operate == "")
                                            {

                                            }
                                            else if (operate == "")
                                            {


                                            }


                                            if (Resources.Instance.ExpiredRemainingDays != -1 && Resources.Instance.ExpiredRemainingDays <= 7)
                                            {
                                                message = string.Format(Resources.Instance.GetString("SoftwareExpiredWarn"));

                                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, message, MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (string operate2) =>
                                                    {

                                                            // 登录
                                                            Login();


                                                    }, null);
                                            }
                                            else
                                            {
                                                    // 登录
                                                    Login();
                                            }


                                            IsLoading = false;

                                        }, null);

                                    }
                                    else
                                    {
                                        // 登录
                                        Login();
                                        IsLoading = false;
                                    }

                                }
                            }
                            else
                            {
                                // 过期
                                if (result.IsExpired)
                                {
                                    IsLoading = false;
                                    message = Resources.Instance.GetString("SoftwareExpired");

                                }
                                //验证成功,只是登录失败
                                else if (result.ValidResult)
                                {

                                    IsLoading = false;
                                    message = string.Format(Resources.Instance.GetString("OperateFaild"), Resources.Instance.GetString("Login"));


                                }
                                //验证失败
                                else
                                {
                                    IsLoading = false;
                                    message = Resources.Instance.GetString("PwdError");


                                }


                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, message, MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }
                        }
                        catch (Exception ex)
                        {

                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                            {
                                IsLoading = false;
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                            }));
                        }

                    }

                }));
            }
        }


        /// <summary>
        /// 保存设置
        /// </summary>
        private void Setting()
        {
            Task.Run(async () =>
            {
                await ExtX.WaitForLoading();

                Resources.Instance.LastLoginAdminNo = AdminNo;
                Resources.Instance.IsSavePassword = IsSavePassword;
                if (IsSavePassword)
                {
                    Resources.Instance.LastLoginPassword = Password;
                }
                else
                {
                    Resources.Instance.LastLoginPassword = null;

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.Password = "";
                    });
                }

                Common.Instance.SetBak();
            });
        }

        /// <summary>
        /// 登录
        /// </summary>
        private void Login()
        {
            // 登录
            Device.BeginInvokeOnMainThread(async () =>
            {
                


                // 登录
                if (InitCount <= 1)
                {
                    IsLoading = true;


                    await Task.Run(async () =>
                    {
                        await ExtX.WaitForLoading();

                        Device.BeginInvokeOnMainThread(() =>
                        {

                            NavigationPath.Instance.RoomListPage = new RoomListPage();
                            NavigationPath.Instance.ChangePasswordPage = new ChangePasswordPage();
                            NavigationPath.Instance.AboutPage = new AboutPage();
                            NavigationPath.Instance.BalancePage = new BalancePage();






                            NavigationPath.Instance.MainListPage = new MainListPage();
                            NavigationPath.Instance.MainNavigation = new MainPage(NavigationPath.Instance.MainListPage);
                            NavigationPath.Instance.InitialMainNavigations(NavigationPath.Instance.MainNavigation);


                        });

                        await ExtX.Sleep(2000);
                    });

                    // 登录通知
                    Notification.Instance.ActionLogin(null, null, null);

                    NavigationPath.Instance.SwitchNavigate(1);
                    NavigationPath.Instance.CurrentNavigate.BindingContext = new MainViewModel(NavigationPath.Instance.CurrentNavigate);



                   
                    NavigationPath.Instance.MainListPage.Init();
                    NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.MainListPage);

                    IsLoading = false;

                    Setting();

                }
                // 以后就是直接打开了
                else
                {
                    // 登录通知
                    Notification.Instance.ActionLogin(null, null, null);

                    NavigationPath.Instance.SwitchNavigate(1);
                    MainViewModel viewModel = NavigationPath.Instance.CurrentNavigate.BindingContext as MainViewModel;
                    viewModel.Resize();

                    Setting();
                }

            });
        }


        /// <summary>
        /// 重置
        /// </summary>
        private RelayCommand _resetCommand;
        public Command ResetCommand
        {
            get
            {
                return _resetCommand ?? (_resetCommand = new RelayCommand(param =>
                {
                    if (!HideSetting)
                        AdminNo = "";
                    Password = "";
                    IsSavePassword = false;
                }));
            }
        }



        
        /// <summary>
        /// 进入设置
        /// </summary>
        private RelayCommand _settingCommand;
        public Command SettingCommand
        {
            get
            {
                return _settingCommand ?? (_settingCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.SettingPage.Init();
                    NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.SettingPage);
                }));
            }
        }



        /// <summary>
        /// 进入设置
        /// </summary>
        private RelayCommand _closeCommand;
        public Command CloseCommand
        {
            get
            {
                return _closeCommand ?? (_closeCommand = new RelayCommand(param =>
                {
                    Common.Instance.Exit();
                    
                }));
            }
        }



        /// <summary>
        /// 进入设置
        /// </summary>
        private RelayCommand _buttonCommand;
        public Command ButtonCommand
        {
            get
            {
                return _buttonCommand ?? (_buttonCommand = new RelayCommand(param =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, "ButtonCommand", MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (string operate) =>
                    {
                    }, null);

                }));
            }
        }


        /// <summary>
        /// 进入设置
        /// </summary>
        private RelayCommand _buttonLongCommand;
        public Command ButtonLongCommand
        {
            get
            {
                return _buttonLongCommand ?? (_buttonLongCommand = new RelayCommand(param =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, "ButtonLongCommand", MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (string operate) =>
                    {
                    }, null);

                }));
            }
        }


        

    }
}
