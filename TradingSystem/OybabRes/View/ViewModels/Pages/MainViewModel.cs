using Microsoft.Win32;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Component;
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
    public sealed class MainViewModel: ViewModelBase
    {
        private UIElement _element;
        private Action _reLogin;
        private Action _refreshRoomList;


        //属性
        private bool IsOpenKeyWindow = false;


        public MainViewModel(UIElement element, Action relogin, Action refreshRoomList)
        {
            this._element = element;
            this._reLogin = relogin;
            this._refreshRoomList = refreshRoomList;

            Notification.Instance.NotificationLanguage += (obj, value, args) => { _element.Dispatcher.BeginInvoke(new Action(() => { SetCurrentName(); })); };

            // 添加处理事件
            this._element.AddHandler(PublicEvents.PopupEvent, new RoutedEventHandler(HandlePopop), true);
            
            _msg = new MsgViewModel(SetMsgCommand);
            _key = new KeyViewModel(SetKeyCommand);
            _animation = new AnimationViewModel();

            // 设置语言
            SetCurrentName();

            // 定时检测
            LoadingCheck();


            // 更新检测
            SetUpdate();

           

            // 更新检测
            SetUpdate();


            // 注册睡眠唤醒(免得session失效)
            SystemEvents.PowerModeChanged -= this.SystemEvents_PowerModeChanged;
            SystemEvents.PowerModeChanged += this.SystemEvents_PowerModeChanged;



            (_element as Window).Loaded += (z,y)=>
            {
                if (!_isLoaded)
                {
                    _isLoaded = true;
                    // 扫码,刷卡

                    // 扫条码处理
                    hookBarcode = new KeyboardHook();

                var availbleScanners = hookBarcode.GetKeyboardDevices();
                string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == Resources.GetRes().BarcodeReader).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(first))
                {
                    hookBarcode.SetDeviceFilter(first);

                    hookBarcode.KeyPressed += OnBarcodeKey;

                    hookBarcode.AddHook(_element as Window);
                }


                hookCard = new KeyboardHook();
                first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == Resources.GetRes().CardReader).FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(first))
                {
                    hookCard.SetDeviceFilter(first);

                    hookCard.KeyPressed += OnCardKey;

                    hookCard.AddHook(_element as Window);
                }




            }
            };


    }



    private KeyboardHook hookCard;
    private KeyboardHook hookBarcode;



    private string keyInput = "";
    private void OnBarcodeKey(object sender, KeyPressedEventArgs e)
    {

        if (_element.Visibility == Visibility.Visible)
        {
            // 如果是确认, 则搜索卡号增加到队列
            if (e.Text == "\r")
            {
                if (keyInput.Trim() != "")
                    Res.Server.Notification.Instance.ActionBarcodeReader(null, keyInput, null);

                keyInput = "";
            }
            else
            {
                keyInput += e.Text;
            }
        }
    }



    private string keyInput2 = "";
        private void OnCardKey(object sender, KeyPressedEventArgs e)
        {

            if (_element.Visibility == Visibility.Visible)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput2.Trim() != "" && keyInput2.Trim().Length == 10)
                        Res.Server.Notification.Instance.ActionCardReader(null, keyInput2, null);

                    keyInput2 = "";

                }
                else
                {
                    keyInput2 += e.Text;
                }
            }
        }

        private bool _isLoaded = false;

    /// <summary>
    /// 处理睡眠或者环形
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
        private void SystemEvents_PowerModeChanged(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:

                    if (Resources.GetRes().IsSessionExists())
                    {
                        OperatesService.GetOperates().AbortService();

                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));
                        }));
                        Task.Factory.StartNew(() =>
                        {
                            // 刚启动时给点时间网络连接好
                            System.Threading.Thread.Sleep(3000);
                            try
                            {
                                OperatesService.GetOperates().ServiceSession(false, null, true);
                            }
                            catch (Exception ex)
                            {
                                ExceptionPro.ExpLog(ex);
                            }
                            finally
                            {
                                if (OperatesService.GetOperates().IsExpired || OperatesService.GetOperates().IsExpired)
                                {
                                    _element.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));

                                        this.Key.KeyMsgMode = 0;
                                        this.IsOpenKeyWindow = true;

                                        this.MsgList.Clear();
                                        this.Msg.AlertMsgMode = false;


                                        OperatesService.GetOperates().IsExpired = false;
                                        Resources.GetRes().SERVER_SESSION = null;
                                        _reLogin();
                                        return;
                                    }));
                                }
                                else
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    if (_refreshRoomList != null)
                                        _refreshRoomList();
                                    _element.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                                    }));
                                }
                                Session.Instance.Start();
                            }
                        });
                    }
                    break;
                case PowerModes.Suspend:
                    Session.Instance.Stop();
                    if (Resources.GetRes().IsSessionExists())
                    {
                        OperatesService.GetOperates().AbortService();
                    }
                    break;
            }
        }






        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {

        }

        /// <summary>
        /// 重新改尺寸
        /// </summary>
        public void Resize()
        {
            if (IsOpenKeyWindow && OperatesService.GetOperates().IsExpired == false && OperatesService.GetOperates().IsAdminUsing == false && Resources.GetRes().SERVER_SESSION != null)
                            this.IsOpenKeyWindow = false;
        }





        /// <summary>
        /// 加载定时检查
        /// </summary>
        private void LoadingCheck()
        {
            //KEY检查
            Session.Instance.StartSession((IsAuto) =>
            {
                //如果窗口本来就打开了,就别打开了(自动检查模式下).
                if (IsAuto && IsOpenKeyWindow)
                    return;

                Common.GetCommon().CheckAndAlertOnce(new Action<string>((message) =>
                {
                    
                    if (string.IsNullOrWhiteSpace(message))
                        message = Resources.GetRes().GetString("Exception_ServerCantCorrConn");
                    else
                        message = string.Format(Resources.GetRes().GetString("Exception_ExceptionSource"), Resources.GetRes().GetString("Exception_ServerCantCorrConn"), "", message);


                    this._key.KeyMsg = message;
                    this._key.KeyMsgImageMode = 2;
                    this._key.KeyMsgMode = 1;
                    IsOpenKeyWindow = true;

                }), new Action(() =>
                {
                    this._key.KeyMsgMode = 0;
                    IsOpenKeyWindow = false;

                    if (_refreshRoomList != null)
                        _refreshRoomList();

                }), IsAuto);
            });
        }



        /// <summary>
        /// 设置更新
        /// </summary>
        private void SetUpdate()
        {
            Oybab.Res.Tools.Update.SearchUpdate(Resources.GetRes().SOFT_SERVICE_TABLET_NAME, model =>
            {

                if (!string.IsNullOrWhiteSpace(model.DisplayMsg))
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (!string.IsNullOrWhiteSpace(model.Url))
                        {
                            //显示信息, 如果有URL提示确认后跳转
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, model.DisplayMsg, msg =>
                            {
                                if (msg == "NO")
                                    return;
                                
                                    System.Diagnostics.Process.Start(model.Url);
                                


                            }, PopupType.Question));
                        }else
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, model.DisplayMsg, null, PopupType.Information));
                        }
                           
                    }));

                }

                
            });
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        private void SetCurrentName()
        {
            OwnerName = "";
            if (Resources.GetRes().MainLangIndex == 0)
                OwnerName = Resources.GetRes().KEY_NAME_0;
            else if (Resources.GetRes().MainLangIndex == 1)
                OwnerName = Resources.GetRes().KEY_NAME_1;
            else if (Resources.GetRes().MainLangIndex == 2)
                OwnerName = Resources.GetRes().KEY_NAME_2;
        }


        

        private bool _lockTopMode;
        /// <summary>
        /// 锁住顶部
        /// </summary>
        public bool LockTopMode
        {
            get { return _lockTopMode; }
            set
            {
                _lockTopMode = value;
                OnPropertyChanged("LockTopMode");
            }
        }



        private string _ownerName;
        /// <summary>
        /// 拥有者名字
        /// </summary>
        public string OwnerName
        {
            get { return _ownerName; }
            set
            {
                _ownerName = value;
                OnPropertyChanged("OwnerName");
            }
        }




        private AnimationViewModel _animation;
        /// <summary>
        /// 动画
        /// </summary>
        public AnimationViewModel Animation
        {
            get { return _animation; }
            set
            {
                _animation = value;
                OnPropertyChanged("AnimationViewModel");
            }
        }



        private MsgViewModel _msg;
        /// <summary>
        /// 消息框
        /// </summary>
        public MsgViewModel Msg
        {
            get { return _msg; }
            set
            {
                _msg = value;
                OnPropertyChanged("Msg");
            }
        }



        private KeyViewModel _key;
        /// <summary>
        /// Key
        /// </summary>
        public KeyViewModel Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged("Key");
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




            if (OperatesService.GetOperates().IsExpired || OperatesService.GetOperates().IsAdminUsing)
            {
                this.Key.KeyMsgMode = 0;
                this.IsOpenKeyWindow = true;

                this.MsgList.Clear();
                this.Msg.AlertMsgMode = false;


                OperatesService.GetOperates().IsExpired = false;
                Resources.GetRes().SERVER_SESSION = null;
                _reLogin();
                return;
            }



            Msg.AlertMsgMode = false;
            InitialMsg();
            
        }




        /// <summary>
        /// Key命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetKeyCommand(string no)
        {
            // 重试
            if (no == "Retry")
            {
                if (OperatesService.GetOperates().IsExpired || OperatesService.GetOperates().IsAdminUsing)
                {
                    this.Key.KeyMsgMode = 0;

                    this.MsgList.Clear();
                    this.Msg.AlertMsgMode = false;


                    OperatesService.GetOperates().IsExpired = false;
                    Resources.GetRes().SERVER_SESSION = null;
                    _reLogin();
                    return;
                }
                else
                {
                    Session.Instance.Keep(false);
                }
                
            }
            // 退出
            else if (no == "Exit")
            {
                Exit();
            }
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
                    case PopupType.LockOn:
                        LockTopMode = true;
                        break;
                    case PopupType.LockOff:
                        LockTopMode = false;
                        break;
                    case PopupType.AnimationOn:
                        Animation.IsDisplay = true;
                        break;
                    case PopupType.AnimationOff:
                        Animation.IsDisplay = false;
                        break;
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
            if (!Msg.AlertMsgMode && MsgList.Count > 0)
            {
                PopupRoutedEventArgs popupArgs = MsgList.Peek();

                // 更改显示模式
                this.Msg.ChangeMode(popupArgs);

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
