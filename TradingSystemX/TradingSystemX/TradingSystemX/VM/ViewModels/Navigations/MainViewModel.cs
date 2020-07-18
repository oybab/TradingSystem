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
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using Xamarin.Essentials;

namespace Oybab.TradingSystemX.VM.ViewModels.Navigations
{
    internal sealed class MainViewModel : ViewModelBase
    {
        private Page _element;
        //属性
        private bool IsOpenKeyWindow = false;

        public MainViewModel(Page _element)
        {
            this._element = _element;
            this.MessageBox = new MessageBoxViewModel(this._element);

            // 注册弹出事件
            QueueMessageBoxNotification.Instance.NotificationMessageBox += this.MessageBox.Instance_NotificationMessageBox;


            // 定时检测
            LoadingCheck();


            // 更新检测
            SetUpdate();

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
            if (IsOpenKeyWindow && OperatesService.Instance.IsExpired == false && OperatesService.Instance.IsAdminUsing == false && Resources.Instance.SERVER_SESSION != null)
                            this.IsOpenKeyWindow = false;



            // 重新将所有包厢重新加载一遍
            RoomListViewModel viewModel = NavigationPath.Instance.RoomListPage.BindingContext as RoomListViewModel;
            viewModel.RefreshAllWithAnimate();


        }

       



        private MessageBoxViewModel _messageBox;
        /// <summary>
        /// 弹出提醒
        /// </summary>
        public MessageBoxViewModel MessageBox
        {
            get { return _messageBox; }
            set
            {
                _messageBox = value;
                OnPropertyChanged("MessageBox");
            }
        }




        /// <summary>
        /// 加载定时检查
        /// </summary>
        private void LoadingCheck()
        {
            //KEY检查
            Session.Instance.StartSession(async (IsAuto) =>
            {
                //如果窗口本来就打开了,就别打开了(自动检查模式下).
                if (IsAuto && IsOpenKeyWindow)
                    return;

                await Common.Instance.CheckAndAlertOnce(new Action<string>((message) =>
                {
                    if (string.IsNullOrWhiteSpace(message))
                        message = Resources.Instance.GetString("Exception_ServerCantCorrConn");
                    else
                        message = string.Format(Resources.Instance.GetString("Exception_ExceptionSource"), Resources.Instance.GetString("Exception_ServerCantCorrConn"), "", message);

                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, message, MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.RetryExit, async (string operate) =>
                    {
                        // 重试
                        if (operate == "Retry")
                        {
                            if (OperatesService.Instance.IsExpired || OperatesService.Instance.IsAdminUsing)
                            {
                                ReLyout();
                                return;
                            }
                            else
                            {
                                // 重试
                                Session.Instance.Keep(false);

                            }

                        }
                        // 退出
                        else if (operate == "Exit")
                        {

                            await OperatesService.Instance.ServiceClose();
                            // 重新登录
                            ReLyout();


                        }
                    }, null);

                    IsOpenKeyWindow = true;

                }), new Action(() =>
                {

                    IsOpenKeyWindow = false;

                    NavigationPath.Instance.RoomListPage.RefreshRoomList();

                }), IsAuto);
            });
        }


        /// <summary>
        /// 重新登录
        /// </summary>
        internal void ReLyout()
        {
            IsOpenKeyWindow = true;


            OperatesService.Instance.IsExpired = false;

            Resources.Instance.SERVER_SESSION = null;

            // 如果在结账页面, 先后退一下
            NavigationPath.Instance.BackMasterDetailsNavigationPage();


            // 重新登录
            NavigationPath.Instance.SwitchNavigate(0);
            // 重新初始化
            LoginViewModel viewModel = NavigationPath.Instance.CurrentPage.BindingContext as LoginViewModel;
            viewModel.Init();
        }


        /// <summary>
        /// 设置更新
        /// </summary>
        private void SetUpdate()
        {
            Oybab.Res.Tools.Update.SearchUpdate(Resources.Instance.SOFT_SERVICE_MOBILE_NAME, model =>
            {
                if (!string.IsNullOrWhiteSpace(model.DisplayMsg))
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        //显示信息, 如果有URL提示确认后跳转. 否则就显示提示
                        if (!string.IsNullOrWhiteSpace(model.Url))
                        {

                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, model.DisplayMsg, MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                            {
                                if (msg == "NO")
                                    return;


                                Launcher.OpenAsync(new Uri(model.Url));

                            }, null);
                        }
                        else
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, model.DisplayMsg, MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);
                        }
                    }));

                }
            });
        }







    }
}
