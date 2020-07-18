using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Component;
using Oybab.Res.View.Converters;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class RoomViewModel : ViewModelBase
    {
        private UIElement _element;

        internal long LastRoomId = 0;
        public Action TimeupAlr;

        private static RoomViewModel _roomViewModel;
        internal static RoomViewModel GetRoomViewModel { get { return _roomViewModel; } }


        public RoomViewModel(UIElement element, Panel ugRoomList)
        {
            this._element = element;


            // 设置关于窗口
            this.About = new AboutViewModel();
            this.About._element = this._element;

            // 设置操作窗口
            this.Operate = new OperateViewModel(element);
            this.Operate.RoomView = this;

            // 设置替换窗口
            this.Replace = new ReplaceViewModel(element, ugRoomList);

            // 设置系统语言窗口
            this.Language = new LanguageViewModel(element, ChangeLanguage, true);


            Language.LanguageMode = Resources.GetRes().CurrentLangIndex;



            Notification.Instance.NotificateSendFromServer += (obj, value, args) => { if (null != args && args.ToString() == "Call") RoomCallAdd(value); else RefreshSome(new List<long>() { value }); };
            Notification.Instance.NotificateSendsFromServer += (obj, value, args) => { RefreshSome(value); };
            Notification.Instance.NotificateGetsFromServer += (obj, value, args) => { RefreshSomeFromServer(value); };
            Notification.Instance.NotificateSend += (obj, value, args) => { if (value == -1) _element.Dispatcher.BeginInvoke(new Action(() => { InitFire(); })); };

            _roomViewModel = this;



            InitFire();

        }


        /// <summary>
        /// 修改语言
        /// </summary>
        /// <param name="lang"></param>
        private void ChangeLanguage(int lang)
        {


            // 一样就无需更改
            if (Resources.GetRes().MainLangIndex  != lang)
            {
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                new Action(new Action(() =>
                {
                    System.Threading.Thread.Sleep(1000);

                    _element.Dispatcher.Invoke(new Action(() =>
                    {

                        LangConverter.Instance.ChangeCulture(lang);
                        Config.GetConfig().SetLanguage(lang);

                        Language.LanguageMode = lang;

                        Notification.Instance.ActionLanguage(null, lang, null);

                    }));

                    System.Threading.Thread.Sleep(500);



                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        this.Language.Hide(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                        }));
                        
                    }));


                })).BeginInvoke(null, null);
            }
            else
            {
                this.Language.Hide(null);
            }
            
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
            long RoomId = (long)obj;
            // 0 表示重新加载所有的
            if (RoomId == 0)
            {

                // 0表示重新加载所有数据, 相当于重新登录了一次
                FirstLoad();

            }
            else
            {
                // -1 表示本地刷新上次进入的
                if (RoomId == -1)
                {
                    if (LastRoomId != 0)
                    {
                        // 更新该订单信息
                        RefreshSome(new List<long>() { LastRoomId });
                    }

                }
                else
                {
                    // 重试代表订单数据不是最新的, 重新获取
                    RefreshSomeFromServer(new List<long>() { RoomId });
                }

                Common.GetCommon().OpenPriceMonitor(null);
                // 刷新第二屏幕
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
                }
            }
        }


        /// <summary>
        /// 刷新所有
        /// </summary>
        public void RefreshAll(bool IsAlert, List<long> Rooms = null)
        {
            SetColor(IsAlert, Rooms);
            RefreshNotification();
            RefreshCount();
        }



        /// <summary>
        /// 第一次加载所有
        /// </summary>
        private void FirstLoad()
        {
            RoomList.Clear();

            foreach (var item in Resources.GetRes().RoomsModel.OrderByDescending(x => x.Order).ThenBy(x => x.RoomNo.Length).ThenBy(x => x.RoomNo))
            {
                RoomList.Add(new RoomStateModel() { RoomId = item.RoomId, RoomNo = item.RoomNo, UseState = (null != item.PayOrder), OrderSession = item.OrderSession, PayOrder = item.PayOrder, OpenRoom = OpenRoom });
            }


            RefreshAll(false);
        }


        /// <summary>
        /// 更新部分
        /// </summary>
        private void RefreshSome(List<long> RoomsId)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var RoomId in RoomsId)
                {
                    RoomModel item = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                    RoomStateModel model = RoomList.Where(x => x.RoomId == RoomId).FirstOrDefault();
                    if (null == model)
                    {
                        RoomStateModel newRoomStateModel = new RoomStateModel() { RoomId = item.RoomId, RoomNo = item.RoomNo, UseState = (null != item.PayOrder), OrderSession = item.OrderSession, PayOrder = item.PayOrder, OpenRoom = OpenRoom };
                        RoomList.Add(newRoomStateModel);
                    }
                    else
                    {
                        if (null != item && null != model && model.OrderSession != item.OrderSession)
                        {
                            RoomStateModel oldModel = RoomList.Where(x => null != x.OrderSession && x.OrderSession.Equals(model.OrderSession, StringComparison.Ordinal)).FirstOrDefault();
                            int no = RoomList.Count;
                            if (null != oldModel)
                            {
                                no = RoomList.IndexOf(oldModel);
                                RoomList.RemoveAt(no);
                            }
                            RoomStateModel newRoomStateModel = new RoomStateModel() { RoomId = item.RoomId, RoomNo = item.RoomNo, UseState = (null != item.PayOrder), OrderSession = item.OrderSession, PayOrder = item.PayOrder, OpenRoom = OpenRoom };
                            RoomList.Insert(no, newRoomStateModel);
                        }
                        else
                        {
                            if (null != model && item == null)
                            {
                                RoomStateModel oldModel = RoomList.Where(x => x.OrderSession.Equals(model.OrderSession, StringComparison.Ordinal)).FirstOrDefault();
                                if (null != oldModel)
                                {
                                    int no = RoomList.IndexOf(oldModel);
                                    if (-1 != no)
                                    {
                                        RoomList.RemoveAt(no);
                                    }
                                }
                            }
                        }
                    }
                }

                RefreshAll(true);
            }));

        }

        /// <summary>
        /// 设置颜色(快到时间检查)
        /// </summary>
        private void SetColor(bool IsAlert, List<long> Rooms = null)
        {
            bool IsFlash = false;
            // 如果是新新部分
            if (null != Rooms)
            {
                
                // 待确认改为红色
                for (int i = 0; i < RoomList.Count; i++)
                {
                    try
                    {
                        RoomStateModel model = RoomList[i] as RoomStateModel;


                        if (Rooms.Contains(model.RoomId))
                        {
                            if (model.RefreshImageState())
                                IsFlash = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
            }
            else
            {
                // 待确认改为红色
                for (int i = 0; i < RoomList.Count; i++)
                {
                    try
                    {
                        RoomStateModel model = RoomList[i] as RoomStateModel;

                        if (model.RefreshImageState())
                            IsFlash = true;
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
            }

            // 如果需要闪烁
            if (IsAlert && IsFlash)
            {
                if (null != TimeupAlr)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        TimeupAlr();
                    }));
                }
            }
        }







        /// <summary>
        /// 包厢呼叫去掉
        /// </summary>
        /// <param name="room"></param>
        internal void RoomCallAdd(long RoomId)
        {

            for (int i = 0; i < RoomList.Count; i++)
            {
                if (RoomList[i].RoomId == RoomId)
                {
                    RoomStateModel model = (RoomList[i]);
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        model.Called = true;
                        if (null != TimeupAlr)
                        {
                            TimeupAlr();

                            // 点歌系统就显示呼叫
                            if (Res.Resources.GetRes().IsRequired("Vod"))
                            {
                                // 呼叫
                                Common.GetCommon().CallDevice(model.RoomNo);
                            }
                        }
                    }));

                    break;
                }
            }
        }


        /// <summary>
        /// 包厢呼叫去掉
        /// </summary>
        /// <param name="room"></param>
        internal void RoomCallRemove(RoomStateModel room)
        {
            room.Called = false;
            
        }

        /// <summary>
        /// 刷新提醒状态
        /// </summary>
        private void RefreshNotification()
        {

        }


        /// <summary>
        /// 从服务器刷新
        /// </summary>
        /// <param name="RoomsId"></param>
        private void RefreshSomeFromServer(List<long> RoomsId)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    bool result = OperatesService.GetOperates().ServiceSession(false, RoomsId.ToArray());

                    // 如果成功, 则新增产品
                    if (result)
                    {
                        // 更新该订单信息
                        RefreshSome(RoomsId);
                    }
                    else
                    {
                        // 发出错误 
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("RefreshOrderFailed"), null, PopupType.Warn));
                        }));
                    }
                }
                catch (Exception ex)
                {
                    // 发出错误
                    ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                    {
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                        }));
                    }), false, Resources.GetRes().GetString("RefreshOrderFailed"));

                }

            });
        }






        /// <summary>
        /// 刷新使用数量
        /// </summary>
        private void RefreshCount()
        {
            RoomCount = this.RoomList.Count.ToString("00");
            RoomUsedCount = this.RoomList.Where(x => null != x.PayOrder).Count().ToString("00");
        }




        // 数据
        private ObservableCollection<RoomStateModel> _roomdList = new ObservableCollection<RoomStateModel>();
        /// <summary>
        /// 雅座列表
        /// </summary>
        public ObservableCollection<RoomStateModel> RoomList
        {
            get { return _roomdList; }
            set
            {
                _roomdList = value;
                OnPropertyChanged("RoomList");
            }
        }


        private string _roomCount;
        /// <summary>
        /// 雅座数量
        /// </summary>
        public string RoomCount
        {
            get { return _roomCount; }
            set
            {
                _roomCount = value;
                OnPropertyChanged("RoomCount");
            }
        }




        private string _roomUsedCount;
        /// <summary>
        /// 雅座使用数量
        /// </summary>
        public string RoomUsedCount
        {
            get { return _roomUsedCount; }
            set
            {
                _roomUsedCount = value;
                OnPropertyChanged("RoomUsedCount");
            }
        }






        private ReplaceViewModel _replace;
        /// <summary>
        /// 显示替换框
        /// </summary>
        public ReplaceViewModel Replace
        {
            get { return _replace; }
            set
            {
                _replace = value;
                OnPropertyChanged("Replace");
            }
        }

        private OperateViewModel _operate;
        /// <summary>
        /// 显示操作框
        /// </summary>
        public OperateViewModel Operate
        {
            get { return _operate; }
            set
            {
                _operate = value;
                OnPropertyChanged("Operate");
            }
        }

        private LanguageViewModel _language;
        /// <summary>
        /// 显示系统语言选择
        /// </summary>
        public LanguageViewModel Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged("Language");
            }
        }

        private AboutViewModel _about;
        /// <summary>
        /// 关于
        /// </summary>
        public AboutViewModel About
        {
            get { return _about; }
            set
            {
                _about = value;
                OnPropertyChanged("About");
            }
        }




        /// <summary>
        /// 系统按钮
        /// </summary>
        private RelayCommand _systemCommand;
        public ICommand SystemCommand
        {
            get
            {
                if (_systemCommand == null)
                {
                    _systemCommand = new RelayCommand(param => _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.System)));
                }
                return _systemCommand;
            }
        }



        /// <summary>
        /// 语言按钮
        /// </summary>
        private RelayCommand _languageCommand;
        public ICommand LanguageCommand
        {
            get
            {
                if (_languageCommand == null)
                {
                    _languageCommand = new RelayCommand(param => { });
                }
                return _languageCommand;
            }
        }




        /// <summary>
        /// 关于按钮
        /// </summary>
        private RelayCommand _aboutCommand;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand(param =>
                        {
                            About.Show();
                        });
                }
                return _aboutCommand;
            }
        }






        /// <summary>
        /// 打开雅座
        /// </summary>
        private RelayCommand _openRoom;
        public ICommand OpenRoom
        {
            get
            {
                if (_openRoom == null)
                {
                    _openRoom = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;

                        if (model != null)
                        {
                            // 是否忽略
                            if (model.IsIgnore)
                            {
                                model.IsIgnore = false;
                                return;
                            }
                            // 是否长按(大于1秒)
                            else if (model.IsLong)
                            {
                                model.IsLong = false;

                                this.Operate.RoomNo = model.RoomNo;
                                this.Operate.model = model;
                                this.Operate.Show();

                                return;
                            }
                            LastRoomId = model.RoomId;

                            if (model.Called)
                            {
                                // 去掉呼叫
                                if (Resources.GetRes().CallNotifications.ContainsKey(LastRoomId))
                                {
                                    Resources.GetRes().CallNotifications[LastRoomId] = false;
                                }

                                //处理
                                RoomCallRemove(model);


                                return;
                            }

                            _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Order, model.RoomId));
                        }

                    });
                }
                return _openRoom;
            }
        }





        /// <summary>
        /// 切换系统语言按钮
        /// </summary>
        private RelayCommand _changeSystemLanguageCommand;
        public ICommand ChangeSystemLanguageCommand
        {
            get
            {
                if (_changeSystemLanguageCommand == null)
                {
                    _changeSystemLanguageCommand = new RelayCommand(param =>
                    {
                        this.Language.Show();
                    });
                }
                return _changeSystemLanguageCommand;
            }
        }


        private bool _sendFireAlarmMode;
        /// <summary>
        /// 火警显示模式(True显示False不显示)
        /// </summary>
        public bool SendFireAlarmMode
        {
            get { return _sendFireAlarmMode; }
            set
            {
                _sendFireAlarmMode = value;
                OnPropertyChanged("SendFireAlarmMode");
            }
        }

        private bool _fireAlarmMode;
        /// <summary>
        /// 火警模式(True火警False没火警)
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


        /// <summary>
        /// 初始化消防
        /// </summary>
        /// <param name="value"></param>
        private void InitFire()
        {
            // 点歌系统就显示呼叫
            if (Resources.GetRes().IsRequired("Fire"))
            {
                SendFireAlarmMode = true;
                if (Resources.GetRes().IsFireAlarmEnable)
                {
                    FireAlarmMode = true;
                }
                else
                {
                    FireAlarmMode = false;
                }
            }
        }






        /// <summary>
        /// 发送火警
        /// </summary>
        private RelayCommand _sendFireAlarmCommand;
        public ICommand SendFireAlarmCommand
        {
            get
            {
                if (_sendFireAlarmCommand == null)
                {
                    _sendFireAlarmCommand = new RelayCommand(param =>
                    {
                        string ErrMsgName = Resources.GetRes().GetString("SendFireAlarm");

                        int sendType = 8; // FireOn

                        if (Resources.GetRes().IsFireAlarmEnable)
                        {
                            ErrMsgName = Resources.GetRes().GetString("CancelFireAlarm");

                            sendType = 16; // FireOff
                        }


                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmOperate"), ErrMsgName), msg =>
                        {
                            if (msg == "NO")
                                return;

                            // 所有设备ID都发送
                            Send(Resources.GetRes().Devices.Select(x => x.DeviceId).ToList(), ErrMsgName, sendType, () =>
                            {
                                InitFire();
                            });

                        }, PopupType.Question));

                    });
                }
                return _sendFireAlarmCommand;
            }
        }


        /// <summary>
        /// 发送
        /// </summary>
        private void Send(List<long> RoomsId, string ErrMsgName, int SendType, Action success = null)
        {

            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
                try
                {
                    bool result = OperatesService.GetOperates().ServiceSend(RoomsId, SendType);

                    // 如果成功则提示
                    if (result)
                    {
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), ErrMsgName), (x) =>
                            {
                                if (null != success)
                                {
                                    success();
                                }
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


    }
}
