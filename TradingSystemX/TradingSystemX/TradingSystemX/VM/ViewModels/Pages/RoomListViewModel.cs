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

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class RoomListViewModel : ViewModelBase
    {
        private Page _element;
        internal long LastRoomId = 0;
        public Action TimeupAlr = null;

        private Xamarin.Forms.StackLayout _spList;
        private Xamarin.Forms.ControlTemplate _ctControlTemplate;

        public RoomListViewModel(Page _element, Xamarin.Forms.StackLayout spList, Xamarin.Forms.ControlTemplate ctControlTemplate)
        {
            this._element = _element;

            this._spList = spList;
            this._ctControlTemplate = ctControlTemplate;

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
            }
        }


        /// <summary>
        /// 刷新所有
        /// </summary>
        internal void RefreshAll(bool IsAlert, List<long> Rooms = null)
        {
            SetColor(IsAlert, Rooms);
            RefreshNotification();
            RefreshCount();
        }

        private bool LoadFinsh = false;

        /// <summary>
        /// 第一次加载所有
        /// </summary>
        private void FirstLoad()
        {
            if (LoadFinsh)
                return;


            // 清空并刷新所有
            ClearAndRefreshAll();


            if (!LoadFinsh)
            {
                Notification.Instance.NotificateSendFromServer += (obj, value, args) => { RefreshSome(new List<long>() { value }); };
                Notification.Instance.NotificateSendsFromServer += (obj, value, args) => { RefreshSome(value); };
                Notification.Instance.NotificateGetsFromServer += (obj, value, args) => { RefreshSomeFromServer(value); };

                LoadFinsh = true;
            }
        }


        /// <summary>
        /// 清空并刷新所有(首次或者登录后)
        /// </summary>
        internal void RefreshAllWithAnimate()
        {

            IsLoading = true;

            Task.Run(async () =>
            {

                await ExtX.WaitForLoading();


                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {

                    ClearAndRefreshAll();

                    IsLoading = false;

                });
            });
        }



        /// <summary>
        /// 刷新模型
        /// </summary>
        private void ClearAndRefreshAll()
        {
            ClearList();

            foreach (var item in Resources.Instance.RoomsModel.OrderByDescending(x => x.Order).ThenBy(x => x.RoomNo.Length).ThenBy(x => x.RoomNo))
            {
                AddList(new RoomStateModel() { RoomId = item.RoomId, RoomNo = item.RoomNo, UseState = (null != item.PayOrder), OrderSession = item.OrderSession, PayOrder = item.PayOrder });
            }

            RefreshAll(false);
        }


        /// <summary>
        /// 更新部分
        /// </summary>
        private void RefreshSome(List<long> RoomsId)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var RoomId in RoomsId)
                {
                    RoomModel item = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                    RoomStateModel model = RoomLists.Where(x => x.RoomId == RoomId).FirstOrDefault();
                    if (null == model)
                    {
                        RoomStateModel newRoomStateModel = new RoomStateModel() { RoomId = item.RoomId, RoomNo = item.RoomNo, UseState = (null != item.PayOrder), OrderSession = item.OrderSession, PayOrder = item.PayOrder };
                        AddList(newRoomStateModel);
                    }
                    else
                    {
                        if (null != item && null != model && model.OrderSession != item.OrderSession)
                        {
                            RoomStateModel oldModel = RoomLists.Where(x => null != x.OrderSession && x.OrderSession.Equals(model.OrderSession, StringComparison.Ordinal)).FirstOrDefault();
                            int no = RoomLists.Count;
                           

                            RoomStateModel newRoomStateModel = new RoomStateModel() { RoomId = item.RoomId, RoomNo = item.RoomNo, UseState = (null != item.PayOrder), OrderSession = item.OrderSession, PayOrder = item.PayOrder };
                            if (null != oldModel)
                            {
                                no = RoomLists.IndexOf(oldModel);
                                ReplaceSelected(no, oldModel, newRoomStateModel);

                            }
                            else
                            {
                                AddList(newRoomStateModel, no);
                            }
                            
                            
                        }
                        else
                        {
                            if (null != model && item == null)
                            {
                                RoomStateModel oldModel = RoomLists.Where(x => x.OrderSession.Equals(model.OrderSession, StringComparison.Ordinal)).FirstOrDefault();
                                if (null != oldModel)
                                {
                                  
                                    RemoveSelected(oldModel);
                                    
                                }
                            }
                        }
                    }
                }

                RefreshAll(true);
            });
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
                for (int i = 0; i < RoomLists.Count; i++)
                {
                    try
                    {
                        RoomStateModel model = RoomLists[i] as RoomStateModel;


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
                for (int i = 0; i < RoomLists.Count; i++)
                {
                    try
                    {
                        RoomStateModel model = RoomLists[i] as RoomStateModel;

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
                    TimeupAlr();
                }
            }
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
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    bool result = await OperatesService.Instance.ServiceSession(false, RoomsId.ToArray());

                    
                    // 如果成功, 则新增产品
                    if (result)
                    {
                        // 更新该订单信息
                        RefreshSome(RoomsId);
                    }
                    else
                    {
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            // 发出错误 
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("RefreshOrderFailed"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        });
                    }
                }
                catch (Exception ex)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                    {
                        // 发出错误
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                        }), false, Resources.Instance.GetString("RefreshOrderFailed"));
                    });

                }

            });
        }





        /// <summary>
        /// 刷新使用数量
        /// </summary>
        private void RefreshCount()
        {
            
        }



        private ObservableCollection<RoomStateModel> _roomLists = new ObservableCollection<RoomStateModel>();
        /// <summary>
        /// 雅座列表0
        /// </summary>
        public ObservableCollection<RoomStateModel> RoomLists
        {
            get { return _roomLists; }
            set
            {
                _roomLists = value;
                OnPropertyChanged("RoomLists");
            }
        }




        /// <summary>
        /// 清空
        /// </summary>
        private void ClearList()
        {
            RoomLists.Clear();
            
            this._spList.Children.Clear();

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Index"></param>
        private void AddList(RoomStateModel model, int Index = -1)
        {
           
            model.GoCommand = this.GoCommand;

            if (Index != -1)
                RoomLists.Insert(Index, model);
            else
                RoomLists.Add(model);

            AddSelectedItem(model, Index);

        }



        /// <summary>
        /// 添加已选对象
        /// </summary>
        /// <param name="item"></param>
        private void AddSelectedItem(RoomStateModel item, int Index = -1)
        {

            Xamarin.Forms.TemplatedView view = new Xamarin.Forms.TemplatedView();
            view.ControlTemplate = _ctControlTemplate;
            view.BindingContext = item;

            if (Index != -1)
                _spList.Children.Insert(Index, view);
            else
                _spList.Children.Add(view);
        }

        /// <summary>
        /// 删除已选
        /// </summary>
        /// <param name="item"></param>
        private void RemoveSelected(RoomStateModel item)
        {
            RoomLists.Remove(item);

            Xamarin.Forms.TemplatedView _view = null;
            foreach (Xamarin.Forms.TemplatedView items in this._spList.Children)
            {
                if (items.BindingContext == item)
                {
                    _view = items;
                    break;
                }
            }

            if (null != _view)
            {
                _view.BindingContext = null;
                this._spList.Children.Remove(_view);
            }
        }

        /// <summary>
        /// 替换
        /// </summary>
        /// <param name="index"></param>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        private void ReplaceSelected(int index, RoomStateModel oldItem, RoomStateModel newItem)
        {
            RoomLists.Remove(oldItem);

            Xamarin.Forms.TemplatedView _view = null;
            foreach (Xamarin.Forms.TemplatedView items in this._spList.Children)
            {
                if (items.BindingContext == oldItem)
                {
                    _view = items;
                    break;
                }
            }


            newItem.GoCommand = this.GoCommand;
            RoomLists.Insert(index, newItem);
            if (null != _view)
            {
                _view.BindingContext = null;
                _view.BindingContext = newItem;
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
        /// 包厢呼叫去掉
        /// </summary>
        /// <param name="room"></param>
        internal void RoomCallAdd(long RoomId)
        {

            for (int i = 0; i < RoomLists.Count; i++)
            {
                if (RoomLists[i].RoomId == RoomId)
                {
                    RoomStateModel model = (RoomLists[i]);
                    Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        model.Called = true;
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
        /// 跳转
        /// </summary>
        private RelayCommand _goCommand;
        public Command GoCommand
        {
            get
            {
                return _goCommand ?? (_goCommand = new RelayCommand(param =>
                {

                    RoomStateModel model = param as RoomStateModel;

                    if (model != null)
                    {
                        // 关闭呼叫
                        if (Resources.Instance.CallNotifications.ContainsKey(model.RoomId))
                        {
                            Resources.Instance.CallNotifications[model.RoomId] = false;
                        }


                        IsLoading = true;
                        LastRoomId = model.RoomId;

                        if (model.Called)
                        {
                            // 去掉呼叫
                            if (Resources.Instance.CallNotifications.ContainsKey(LastRoomId))
                            {
                                Resources.Instance.CallNotifications[LastRoomId] = false;
                            }

                            //处理
                            RoomCallRemove(model);

                        }

                        Task.Run(async () =>
                        {

                            await ExtX.WaitForLoading();


                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                                    {

                                                        NavigationPath.Instance.OrderPage.Init(model.RoomId, (index =>
                                                        {
                                                            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                                            {
                                                                if (index == 0)
                                                                    NavigationPath.Instance.OrderPage.Detail = NavigationPath.Instance.ProductNavigationPage;
                                                                else if (index == 1)
                                                                    NavigationPath.Instance.OrderPage.Detail = NavigationPath.Instance.SelectedNavigationPage;

                                                                NavigationPath.Instance.InitialMasterDetail(NavigationPath.Instance.OrderPage, NavigationPath.Instance.ProductNavigationPage, NavigationPath.Instance.SelectedNavigationPage);

                                                                NavigationPath.Instance.SwitchMasterDetailNavigate(index);
                                                            });
                                                        }));

                                                        IsLoading = false;
                                                    });
                        });


                    }

                }));
            }
        }




    }
}
