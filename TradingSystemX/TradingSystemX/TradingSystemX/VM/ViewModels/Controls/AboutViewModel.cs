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
using Newtonsoft.Json;
using Oybab.ServerManager.Model.Models;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages.Controls
{
    internal sealed class AboutViewModel : ViewModelBase
    {
        private Page _element;
        public AboutViewModel(Page _element)
        {
            this._element = _element;




            Oybab.Res.Server.Notification.Instance.NotificateSend += (obj, value, args) => { if (value == -2) Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() => { InitDisable(); })); };
            Notification.Instance.NotificateSend += (obj, value, args) => { if (value == -1) Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() => { InitFire(); })); };

        }


        /// <summary>
        /// 初始化锁住
        /// </summary>
        /// <param name="value"></param>
        private void InitDisable()
        {
            // 点歌系统就显示呼叫
            if (Resources.Instance.ExtendInfo.SliderMode == "2")
            {

                DisableMode = true;
            }
            else
            {
                DisableMode = false;
            }

        }


        /// <summary>
        /// 初始化消防
        /// </summary>
        /// <param name="value"></param>
        private void InitFire()
        {

            if (Resources.Instance.IsFireAlarmEnable)
            {
                FireAlarmMode = true;
            }
            else
            {
                FireAlarmMode = false;
            }

        }


        /// <summary>
        /// 单击按钮
        /// </summary>
        private RelayCommand _disableCommand;
        public Command DisableCommand
        {
            get
            {
                if (_disableCommand == null)
                {
                    _disableCommand = new RelayCommand(param =>
                    {
                        string ErrMsgName = Resources.Instance.GetString("Disable");

                        int sendType = 17; // ExtInfo

                        ExtendInfo info = new ExtendInfo();



                        if (Resources.Instance.ExtendInfo.SliderMode == "2")
                        {
                            ErrMsgName = Resources.Instance.GetString("Enable");

                            info.SliderMode = "1";
                        }
                        else
                        {
                            info.SliderMode = "2";
                        }

                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmOperate"), ErrMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, async (string msg) =>
                        {
                        
                            if (msg == "NO")
                                return;

                            // 所有设备ID都发送
                            await Send(Resources.Instance.Devices.Select(x => x.DeviceId).ToList(), ErrMsgName, sendType, info, () =>
                            {
                                InitDisable();
                            });

                        }, null);
                    });
                }
                return _disableCommand;
            }
        }





        /// <summary>
        /// 单击火警按钮
        /// </summary>
        private RelayCommand _disableFireCommand;
        public Command DisableFireCommand
        {
            get
            {
                if (_disableFireCommand == null)
                {
                    _disableFireCommand = new RelayCommand(param =>
                    {
                        string ErrMsgName = Resources.Instance.GetString("SendFireAlarm");

                        int sendType = 8; // FireOn


                        if (Resources.Instance.IsFireAlarmEnable)
                        {
                            ErrMsgName = Resources.Instance.GetString("CancelFireAlarm");

                            sendType = 16; // FireOff
                        }
                       

                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmOperate"), ErrMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, async (string msg) =>
                        {

                            if (msg == "NO")
                                return;

                            // 所有设备ID都发送
                            await Send(Resources.Instance.Devices.Select(x => x.DeviceId).ToList(), ErrMsgName, sendType,  null, () =>
                            {
                                InitFire();
                            });

                        }, null);
                    });
                }
                return _disableFireCommand;
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


        /// <summary>
        /// 发送
        /// </summary>
        private async Task Send(List<long> RoomsId, string ErrMsgName, int SendType, ExtendInfo info, Action success = null)
        {
            IsLoading = true;

            try
            {
                bool result = await OperatesService.Instance.ServiceSend(RoomsId, SendType, JsonConvert.SerializeObject(info));


                // 如果成功则提示
                if (result)
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), ErrMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);



                    if (null != success)
                    {
                        success();
                    }
                }
            }
            catch (Exception ex)
            {

                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }), false, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName));

            }

            IsLoading = false;
        }


        private bool _disableVisibleMode = false;
        /// <summary>
        /// 是否显示关闭
        /// </summary>
        public bool DisableVisibleMode
        {
            get { return _disableVisibleMode; }
            set
            {
                _disableVisibleMode = value;
                OnPropertyChanged("DisableVisibleMode");
            }
        }


        private bool _disableFireMode = false;
        /// <summary>
        /// 是否火警显示关闭
        /// </summary>
        public bool DisableFireMode
        {
            get { return _disableFireMode; }
            set
            {
                _disableFireMode = value;
                OnPropertyChanged("DisableFireMode");
            }
        }



        private bool _fireAlarmMode = false;
        /// <summary>
        /// 是否火警
        /// </summary>
        public bool FireAlarmMode
        {
            get { return _fireAlarmMode; }
            set
            {
                _fireAlarmMode = value;
                OnPropertyChanged("FireAlarmMode");
            }
        }



        private bool _disableMode = false;
        /// <summary>
        /// 关闭模式
        /// </summary>
        public bool DisableMode
        {
            get { return _disableMode; }
            set
            {
                _disableMode = value;
                OnPropertyChanged("DisableMode");
            }
        }




        /// <summary>
        /// 打开禁用
        /// </summary>
        internal void OpenDisable()
        {
            DisableVisibleMode = true;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            VersionNo = OperatesService.Instance.AllVersion;


            int day = Resources.Instance.ExpiredRemainingDays;
            if (day > 99)
                day = 99;

            RemainingDays = string.Format("{0}", day);// : {0}. 


            Device = string.Format("{0}", Resources.Instance.DeviceCount);


            Table = string.Format("{0}", Resources.Instance.RoomCount);


            //语言
            if (Res.Instance.MainLangIndex == 0)
            {
                Admin = Resources.Instance.AdminModel.AdminName0;
            }
            else if (Res.Instance.MainLangIndex == 1)
            {
                Admin = Resources.Instance.AdminModel.AdminName1;
            }
            else if (Res.Instance.MainLangIndex == 2)
            {
                Admin = Resources.Instance.AdminModel.AdminName2;
            }
            Role = GetModeName(Resources.Instance.AdminModel.Mode);


            DisableVisibleMode = false;
            DisableFireMode = false;

            InitDisable();
            InitFire();
        }



        /// <summary>
        /// 获取管理员模式名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetModeName(long typeNo)
        {
            if (typeNo == 0)
                return Resources.Instance.GetString("Guest");
            else if (typeNo == 1)
                return Resources.Instance.GetString("Employee");
            else if (typeNo == 2)
                return Resources.Instance.GetString("Admin");
            else
            {
                throw new Exception(Resources.Instance.GetString("Exception_InvalidType"));
            }
        }








        private string _versionNo = "";
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNo
        {
            get { return _versionNo; }
            set
            {
                _versionNo = value;
                OnPropertyChanged("VersionNo");
            }
        }



        private string _remainingDays = "";
        /// <summary>
        /// 剩余天数
        /// </summary>
        public string RemainingDays
        {
            get { return _remainingDays; }
            set
            {
                _remainingDays = value;
                OnPropertyChanged("RemainingDays");
            }
        }


        private string _device = "";
        /// <summary>
        /// 设备数
        /// </summary>
        public string Device
        {
            get { return _device; }
            set
            {
                _device = value;
                OnPropertyChanged("Device");
            }
        }



        private string _table = "";
        /// <summary>
        /// 雅座数
        /// </summary>
        public string Table
        {
            get { return _table; }
            set
            {
                _table = value;
                OnPropertyChanged("Table");
            }
        }


        private string _admin = "";
        /// <summary>
        /// 管理员
        /// </summary>
        public string Admin
        {
            get { return _admin; }
            set
            {
                _admin = value;
                OnPropertyChanged("Admin");
            }
        }


        private string _role = "";
        /// <summary>
        /// 权限
        /// </summary>
        public string Role
        {
            get { return _role; }
            set
            {
                _role = value;
                OnPropertyChanged("Role");
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

    }
}
