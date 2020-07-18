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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class RegViewModel: ViewModelBase
    {
        private UIElement _element;


        public RegViewModel(UIElement element, int regType)
        {
            this._element = element;
            this.RegType = regType;


            _keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);



            // 获取注册码
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            MachineNo = "";
            RegNo = "";
            IsDisPlayKeyboard = false;
            IsLoading = false;
            IsMsg = false;
            Msg = "";
            RegNoEnable = false;
            
            AlertMsg = "";
            AlertMsgMode = 0;

            ClearFocus();
            InitMachineNo();
        }



        /// <summary>
        /// 初始化机器码
        /// </summary>
        internal void InitMachineNo()
        {

            if ((RegType == 0 && string.IsNullOrWhiteSpace(Resources.GetRes().RegTimeRequestCode)) || (RegType == 1 && string.IsNullOrWhiteSpace(Resources.GetRes().RegCountRequestCode)))
            {

                IsLoading = true;

                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        bool result = false;

                        if (RegType == 0)
                            result = OperatesService.GetOperates().ServiceRequestTimeCode();
                        else if (RegType == 1)
                            result = OperatesService.GetOperates().ServiceRequestTimeCode();

                        //如果验证成功

                            if (result)
                            {
                                if (RegType == 0)
                                    MachineNo = Resources.GetRes().RegTimeRequestCode;
                                else if (RegType == 1)
                                    MachineNo = Resources.GetRes().RegCountRequestCode;
                            }
                            else
                            {
                                IsMsg = true;
                                Msg = string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("MachineNo"));
                            }

                    }
                    catch (Exception ex)
                    {
                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                            {
                                IsMsg = true;
                                Msg = message;
                            }));
                    }
                    IsLoading = false;

                });
            }
            else
            {
                if (RegType == 0)
                    MachineNo = Resources.GetRes().RegTimeRequestCode;
                else if (RegType == 1)
                    MachineNo = Resources.GetRes().RegCountRequestCode;
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


        private string _machineNo = "";
        /// <summary>
        /// 机器码
        /// </summary>
        public string MachineNo
        {
            get { return _machineNo; }
            set
            {
                _machineNo = value;
                OnPropertyChanged("MachineNo");
            }
        }



        private string _regNo = "";
        /// <summary>
        /// 注册码
        /// </summary>
        public string RegNo
        {
            get { return _regNo; }
            set
            {
                _regNo = value;
                OnPropertyChanged("RegNo");
            }
        }



        private int _regType = 0;
        /// <summary>
        /// 注册类型
        /// </summary>
        public int RegType
        {
            get { return _regType; }
            set
            {
                _regType = value;
                OnPropertyChanged("RegType");
            }
        }


        /// <summary>
        /// 注册
        /// </summary>
        private RelayCommand _regCommand;
        public ICommand RegCommand
        {
            get
            {
                if (_regCommand == null)
                {
                    _regCommand = new RelayCommand(param =>
                    {
                        

                        IsMsg = false;
                        Msg = "";

                        new Action(() =>
                        {
                            //判断是否空
                            if (RegNo.Trim().Equals("") || MachineNo.Trim().Equals(""))
                            {
                                SystemSounds.Asterisk.Play();
                            }
                            else
                            {
                                IsLoading = true;
                                try
                                {
                                    
                                    bool result = false;

                                    if (RegType == 0)
                                        result = OperatesService.GetOperates().ServiceRegTime(RegNo);
                                    else if (RegType == 1)
                                        result = OperatesService.GetOperates().ServiceRegCount(RegNo);

                                    //成功
                                    if (result)
                                    {
                                        SystemSounds.Asterisk.Play();
                                        AlertMsgMode = 1;
                                        AlertMsg = Resources.GetRes().GetString("RegisterSuccess");
                                       
                                    }
                                    else
                                    {
                                        SystemSounds.Asterisk.Play();
                                        IsLoading = false;
                                        IsMsg = true;
                                        Msg = string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Register"));
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
                return _regCommand;
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
                        RegNo = "";
                        ClearFocus();
                    });
                }
                return _resetCommand;
            }
        }



        /// <summary>
        /// 注册是按钮
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
                        Exit();
                    });
                }
                return _okCommand;
            }
        }




        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (RegNoEnable && RegNo.Length < 32)
            {
                RegNo += no;
            }
        }


        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (RegNoEnable && RegNo.Length > 0)
            {
                if (IsAll)
                    RegNo = "";
                else
                    RegNo = RegNo.Remove(RegNo.Length - 1); ;
            }
        }

        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveAllText()
        {
            if (RegNoEnable && RegNo.Length > 0)
            {
                RegNo = "";
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
                RegNoEnable = false;
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




        private bool _regNoEnable;
        /// <summary>
        /// 注册码选中
        /// </summary>
        public bool RegNoEnable
        {
            get { return _regNoEnable; }
            set
            {
                _regNoEnable = value;
                if (value)
                {
                    IsDisPlayKeyboard = true;
                }
                OnPropertyChanged("RegNoEnable");
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


        /// <summary>
        /// 退出
        /// </summary>
        private void Exit()
        {
            _element.Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.Shutdown(0);
            }));
        }

    }
}
