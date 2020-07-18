using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class ChangePasswordViewModel : ViewModelBase
    {
        private UIElement _element;

        internal ChangePasswordViewModel(UIElement element)
        {
            this._element = element;
            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
        }



        /// <summary>
        /// 设置显示模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetDisplay(int mode)
        {
            this.DisplayMode = mode;
            KeyboardLittle.IsDisplayKeyboard = true;
        }




        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

            IsDisplay = true;
            IsShow = true;
        }



        /// <summary>
        /// 隐藏
        /// </summary>
        internal void Hide()
        {
            IsShow = false;

            new Action(() =>
            {
                System.Threading.Thread.Sleep(Resources.GetRes().AnimateTime);

                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsDisplay = false;


                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));
                }));

            }).BeginInvoke(null, null);

        }





        private bool _isShow = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                OnPropertyChanged("IsShow");
            }
        }


        private int _displayMode;
        /// <summary>
        /// 显示模式(1老密码, 1新密码, 2旧密码)
        /// </summary>
        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                OnPropertyChanged("DisplayMode");
            }
        }

        

        private bool _isDisplay = false;
        /// <summary>
        /// 是否显示动画
        /// </summary>
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set
            {
                _isDisplay = value;
                if (_isDisplay == true)
                    Init();
                OnPropertyChanged("IsDisplay");
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            OldPassword = "";
            OldPasswordTemp = "";

            NewPassword = "";
            NewPasswordTemp = "";

            ConfirmPassword = "";
            ConfirmPasswordTemp = "";
            this.DisplayMode = 0;
        }






        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.OldPassword.Length < 16)
            {
                this.OldPassword += no;
            }
            else if (this.IsDisplay && this.DisplayMode == 2 && this.NewPassword.Length < 16)
            {
                this.NewPassword += no;
            }
            else if (this.IsDisplay && this.DisplayMode == 3 && this.ConfirmPassword.Length < 16)
            {
                this.ConfirmPassword += no;
            }
        }




        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.OldPassword.Length > 0)
            {
                if (IsAll)
                    this.OldPassword = "";
                else
                    this.OldPassword = this.OldPassword.Remove(this.OldPassword.Length - 1);
            }
            else if (this.IsDisplay && this.DisplayMode == 2 && this.NewPassword.Length > 0)
            {
                if (IsAll)
                    this.NewPassword = "";
                else
                    this.NewPassword = this.NewPassword.Remove(this.NewPassword.Length - 1);
            }
            else if (this.IsDisplay && this.DisplayMode == 3 && this.ConfirmPassword.Length > 0)
            {
                if (IsAll)
                    this.ConfirmPassword = "";
                else
                    this.ConfirmPassword = this.ConfirmPassword.Remove(this.ConfirmPassword.Length - 1);
            }
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
                this.KeyboardLittle.IsDisplayKeyboard = false;
                if (this.IsDisplay)
                    this.DisplayMode = 0;
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


        /// <summary>
        /// 去掉焦点
        /// </summary>
        private void ClearFocus()
        {
            var scope = FocusManager.GetFocusScope(_element); // elem is the UIElement to unfocus
            FocusManager.SetFocusedElement(scope, null); // remove logical focus
            Keyboard.ClearFocus(); // remove keyboard focus
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
                _oldPassword = value;
                OnPropertyChanged("OldPassword");

                string temp = "";
                //设置虚拟密码
                for (int i = 0; i < _oldPassword.Length; i++)
                {
                    temp += "●";
                }
                OldPasswordTemp = temp;
                
            }
        }


        private string _oldPasswordTemp = "";
        /// <summary>
        /// 临时密码(用于显示)
        /// </summary>
        public string OldPasswordTemp
        {
            get { return _oldPasswordTemp; }
            set
            {
                _oldPasswordTemp = value;
                OnPropertyChanged("OldPasswordTemp");
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
                _newPassword = value;
                OnPropertyChanged("NewPassword");

                string temp = "";
                //设置虚拟密码
                for (int i = 0; i < _newPassword.Length; i++)
                {
                    temp += "●";
                }
                NewPasswordTemp = temp;
            }
        }


        private string _newPasswordTemp = "";
        /// <summary>
        /// 临时密码(用于显示)
        /// </summary>
        public string NewPasswordTemp
        {
            get { return _newPasswordTemp; }
            set
            {
                _newPasswordTemp = value;
                OnPropertyChanged("NewPasswordTemp");
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
                _confirmPassword = value;
                OnPropertyChanged("ConfirmPassword");

                string temp = "";
                //设置虚拟密码
                for (int i = 0; i < _confirmPassword.Length; i++)
                {
                    temp += "●";
                }
                ConfirmPasswordTemp = temp;
            }
        }


        private string _confirmPasswordTemp = "";
        /// <summary>
        /// 临时密码(用于显示)
        /// </summary>
        public string ConfirmPasswordTemp
        {
            get { return _confirmPasswordTemp; }
            set
            {
                _confirmPasswordTemp = value;
                OnPropertyChanged("ConfirmPasswordTemp");
            }
        }
      



        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(param =>
                    {

                        //判断是否空
                        if (OldPassword.Trim().Equals(""))
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("OldPassword")), null, PopupType.Warn));
                        }
                        else if (NewPassword.Trim().Equals(""))
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("NewPassword")), null, PopupType.Warn));
                        }
                        else if (ConfirmPassword.Trim().Equals(""))
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("ConfirmPassword")), null, PopupType.Warn));
                        }
                        else if (!NewPassword.Trim().Equals(ConfirmPassword.Trim()))
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("TwoNewPasswordNotEqual"), null, PopupType.Warn));
                        }
                        else
                        {
                            if (NewPassword.Trim().Length < 6)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("NewPasswordSizeLimit"), 6), null, PopupType.Warn));
                                return;
                            }

                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));


                            Task.Factory.StartNew(() =>
                            {
                                try
                                {


                                    ResultModel result = OperatesService.GetOperates().ChangePWD(OldPassword.Trim(), NewPassword.Trim());

                                    //如果验证成功
                                    //修改成功
                                    _element.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        if (result.Result)
                                        {
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("ChangeSuccess"), null, PopupType.Information));

                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));
                                            KeyboardLittle.IsDisplayKeyboard = false;
                                            this.DisplayMode = 0;
                                            this.Hide();
                                        }
                                        else
                                        {
                                            //验证成功,修改失败
                                            if (result.ValidResult)
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("ChangeFaild"), null, PopupType.Warn));
                                            }
                                            //验证失败
                                            else
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PwdError"), null, PopupType.Warn));
                                            }
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
                        }


                        
                    });
                }
                return _okCommand;
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
                    _cancelCommand = new RelayCommand(param =>
                    {
                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }



    }
}
