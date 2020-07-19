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

namespace Oybab.TradingSystemX.VM.ViewModels.Pages.Controls
{
    internal sealed class ChangePasswordViewModel : ViewModelBase
    {
        private Page _element;
        public ChangePasswordViewModel(Page _element)
        {
            this._element = _element;

        }

       


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            OldPassword = "";

            NewPassword = "";

            ConfirmPassword = "";
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




        private string _oldPassword = "";
        /// <summary>
        /// 老密码
        /// </summary>
        public string OldPassword
        {
            get { return _oldPassword; }
            set
            {
                if (value.Length <= 16)
                {
                    _oldPassword = value;
                    OnPropertyChanged("OldPassword");
                }
            }
        }




        private string _newPassword = "";
        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword
        {
            get { return _newPassword; }
            set
            {
                if (value.Length <= 16)
                {
                    _newPassword = value;
                    OnPropertyChanged("NewPassword");
                }
            }
        }






        private string _confirmPassword = "";
        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (value.Length <= 16)
                {
                    _confirmPassword = value;
                    OnPropertyChanged("ConfirmPassword");
                }
            }
        }





        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _changeCommand;
        public Command ChangeCommand
        {
            get
            {
                return _changeCommand ?? (_changeCommand = new RelayCommand(async param =>
                {
                    //判断是否空
                    if (OldPassword.Trim().Equals(""))
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("LoginValid"), Resources.Instance.GetString("OldPassword")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                    else if (NewPassword.Trim().Equals(""))
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("LoginValid"), Resources.Instance.GetString("NewPassword")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                    else if (ConfirmPassword.Trim().Equals(""))
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("LoginValid"), Resources.Instance.GetString("ConfirmPassword")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                    else if (!NewPassword.Trim().Equals(ConfirmPassword.Trim()))
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("TwoNewPasswordNotEqual"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                    else
                    {
                        if (NewPassword.Trim().Length < 6)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("NewPasswordSizeLimit"), 6), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            return;
                        }

                        IsLoading = true;

                        try
                        {
                            ResultModel result = await OperatesService.Instance.ChangePWD(OldPassword.Trim(), NewPassword.Trim());

                            //如果验证成功
                            //修改成功

                            if (result.Result)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, Resources.Instance.GetString("ChangeSuccess"), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, (msg) =>
                                {
                                    NavigationPath.Instance.GoNavigateBack();
                                }, null);
                            }
                            else
                            {
                                //验证成功,修改失败
                                if (result.ValidResult)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("ChangeFaild"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                }
                                //验证失败
                                else
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PwdError"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                            }));
                        }

                        IsLoading = false;

                    }
                }));
            }
        }

        /// <summary>
        /// 返回
        /// </summary>
        private RelayCommand _backCommand;
        public Command BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.GoNavigateBack();
                }));
            }
        }


    }
}
