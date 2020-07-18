using Newtonsoft.Json;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class AboutViewModel : ViewModelBase
    {

        public AboutViewModel()
        {
            Oybab.Res.Server.Notification.Instance.NotificateSend += (obj, value, args) => { if (value == -2) _element.Dispatcher.BeginInvoke(new Action(() => { InitDisable(); })); };

           
        }

        /// <summary>
        /// 打开禁用
        /// </summary>
        public void OpenDisable()
        {
            // 点歌系统就显示呼叫
            if (Resources.GetRes().IsRequired("Vod"))
                DisableVisibleMode = true;
        }




        /// <summary>
        /// 初始化锁住
        /// </summary>
        /// <param name="value"></param>
        private void InitDisable()
        {
            // 点歌系统就显示呼叫
            if (Resources.GetRes().ExtendInfo.SliderMode == "2")
            {

                DisableMode = true; 
            }
            else
            {
                DisableMode = false;
            }

        }


        /// <summary>
        /// 单击按钮
        /// </summary>
        private RelayCommand _disableCommand;
        public ICommand DisableCommand
        {
            get
            {
                if (_disableCommand == null)
                {
                    _disableCommand = new RelayCommand(param =>
                    {
                        string ErrMsgName = Resources.GetRes().GetString("Disable");

                        int sendType = 17; // ExtInfo

                        ExtendInfo info = new ExtendInfo();



                        if (Resources.GetRes().ExtendInfo.SliderMode == "2")
                        {
                            ErrMsgName = Resources.GetRes().GetString("Enable");

                            info.SliderMode = "1";
                        }
                        else
                        {
                            info.SliderMode = "2";
                        }


                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmOperate"), ErrMsgName), msg =>
                        {
                            if (msg == "NO")
                                return;

                            // 所有设备ID都发送
                            Send(Resources.GetRes().Devices.Select(x => x.DeviceId).ToList(), ErrMsgName, sendType, info, () =>
                            {
                                InitDisable();
                            });

                        }, PopupType.Question));
                    });
                }
                return _disableCommand;
            }
        }



        /// <summary>
        /// 发送
        /// </summary>
        private void Send(List<long> RoomsId, string ErrMsgName, int SendType, ExtendInfo info, Action success = null)
        {

            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
                try
                {
                    bool result = OperatesService.GetOperates().ServiceSend(RoomsId, SendType, JsonConvert.SerializeObject(info));

                    // 如果成功则提示
                    if (result)
                    {
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), ErrMsgName), (x) =>
                            {
                                //if (x == "OK")
                                //{
                                if (null != success)
                                {
                                    success();
                                }
                                //}
                            }, PopupType.Information));
                        }));
                    }
                }
                catch (Exception ex)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName));
                    }));
                }
                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                }));
            });
        }




        internal UIElement _element;

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


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            VersionNo = assembly.GetName().Version.ToString();//获取主版本号


            int day = Resources.GetRes().ExpiredRemainingDays;
            if (day > 99)
                day = 99;

            RemainingDays = string.Format("{0}", day);// : {0}. 


            Device = string.Format("{0}", Resources.GetRes().DeviceCount);


            Table = string.Format("{0}", Resources.GetRes().RoomCount);


            //语言
            if (Resources.GetRes().MainLangIndex == 0)
            {
                Admin = Resources.GetRes().AdminModel.AdminName0;
            }
            else if (Resources.GetRes().MainLangIndex == 1)
            {
                Admin = Resources.GetRes().AdminModel.AdminName1;
            }
            else if (Resources.GetRes().MainLangIndex == 2)
            {
                Admin = Resources.GetRes().AdminModel.AdminName2;
            }
            Role = GetModeName(Resources.GetRes().AdminModel.Mode);



            DisableVisibleMode = false;
            InitDisable();
        }



        /// <summary>
        /// 获取管理员模式名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetModeName(long typeNo)
        {
            if (typeNo == 0)
                return Resources.GetRes().GetString("Guest");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Employee");
            else if (typeNo == 2)
                return Resources.GetRes().GetString("Admin");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }



        /// <summary>
        /// 单击按钮
        /// </summary>
        private RelayCommand _command;
        public ICommand Command
        {
            get
            {
                if (_command == null)
                {
                    _command = new RelayCommand(param =>
                    {
                        this.Hide();
                    });
                }
                return _command;
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
    }
}
