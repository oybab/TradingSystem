using Oybab.DAL;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Oybab.Res.Tools;
using System.Threading.Tasks;
using Oybab.Res.Server;
using Oybab.Res.Exceptions;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class OperateViewModel : ViewModelBase
    {
        internal RoomViewModel RoomView { get; set; }
        private UIElement _element;
        internal RoomStateModel model { get; set; }

        internal OperateViewModel(UIElement element)
        {
            this._element = element;
        }



        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {
            OnPropertyChanged("IsRoleIn");
            OnPropertyChanged("IsCancelOrder");
            OnPropertyChanged("IsReplaceRoom");
            OnPropertyChanged("IsExists");

            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

            IsDisplay = true;
            IsShow = true;
        }



        /// <summary>
        /// 隐藏
        /// </summary>
        internal void Hide(bool Immediately)
        {
            if (Immediately)
            {
                IsShow = false;
                IsDisplay = false;
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));
            }
            else
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



        private bool _isDisplay = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set
            {
                _isDisplay = value;
                OnPropertyChanged("IsDisplay");
            }
        }



        private bool _isExists = false;
        /// <summary>
        /// 是否存在账单
        /// </summary>
        public bool IsExists
        {
            get { return (null != model && null != model.PayOrder); }
            set
            {
                _isExists = value;
                OnPropertyChanged("IsExists");
            }
        }


        private bool _isRoleIn = false;
        /// <summary>
        /// 是否允许收入交易管理
        /// </summary>
        public bool IsRoleIn
        {
            get { return Common.GetCommon().IsIncomeTradingManage(); }
            set
            {
                _isRoleIn = value;
                OnPropertyChanged("IsRoleIn");
            }
        }


        private bool _isCancelOrder = false;
        /// <summary>
        /// 是否允许取消订单
        /// </summary>
        public bool IsCancelOrder
        {
            get { return Common.GetCommon().IsCancelOrder(); }
            set
            {
                _isCancelOrder = value;
                OnPropertyChanged("IsCancelOrder");
            }
        }



        private bool _isReplaceRoom = false;
        /// <summary>
        /// 是否允许更改包厢
        /// </summary>
        public bool IsReplaceRoom
        {
            get { return Common.GetCommon().IsReplaceRoom(); }
            set
            {
                _isReplaceRoom = value;
                OnPropertyChanged("IsReplaceRoom");
            }
        }



        private string _roomNo = "";
        /// <summary>
        /// 雅座编号
        /// </summary>
        public string RoomNo
        {
            get { return _roomNo; }
            set
            {
                _roomNo = value;
                OnPropertyChanged("RoomNo");
            }
        }






        /// <summary>
        /// 新建
        /// </summary>
        private RelayCommand _newCommand;
        public ICommand NewCommand
        {
            get
            {
                if (_newCommand == null)
                {
                    _newCommand = new RelayCommand(param =>
                    {
                        this.Hide(true);
                        RoomView.LastRoomId = model.RoomId;
                        _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Order, model.RoomId));
                    });
                }
                return _newCommand;
            }
        }


        /// <summary>
        /// 查看
        /// </summary>
        private RelayCommand _showCommand;
        public ICommand ShowCommand
        {
            get
            {
                if (_showCommand == null)
                {
                    _showCommand = new RelayCommand(param =>
                    {
                        this.Hide(true);
                        RoomView.LastRoomId = model.RoomId;
                        _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Order, model.RoomId));
                    });
                }
                return _showCommand;
            }
        }




        /// <summary>
        /// 取消
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
                        //确认取消
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("ConfirmCancelOrder"), msg =>
                        {


                            if (msg == "NO")
                                return;

                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == this.model.RoomId).FirstOrDefault();


                            if (null == model.PayOrder)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                Notification.Instance.ActionGetsFromService(null, new List<long>() { model.RoomId }, null);
                                return;
                            }

                            Order cancelOrder = model.PayOrder.FastCopy();
                            cancelOrder.State = 2;

                            string newRoomSessionId;
                            string ErrMsgName, SucMsgName;
                            ErrMsgName = SucMsgName = Resources.GetRes().GetString("CancelOrder");


                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                            Task.Factory.StartNew(() =>
                            {
                                try
                                {


                                    long UpdateTime;

                                    ResultModel result = OperatesService.GetOperates().ServiceEditOrder(cancelOrder, null, null, model.OrderSession, false, out newRoomSessionId, out UpdateTime);

                                    if (result.Result)
                                    {
                                        foreach (var item in model.PayOrder.tb_orderdetail)
                                        {
                                            // 未确认的还没确定价格, 所以不需要
                                            if (item.State != 1 && item.State != 3)
                                            {
                                                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                                if (null != product && product.IsBindCount == 1)
                                                {
                                                    product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3); // product.BalanceCount += item.Count;
                                                    product.UpdateTime = UpdateTime;

                                                    Notification.Instance.ActionProduct(null, product, 2);
                                                }
                                            }
                                        }

                                        foreach (var item in model.PayOrder.tb_orderpay)
                                        {
                                            if (null != item.MemberId)
                                            {
                                                Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                                item.MemberId = item.tb_member.MemberId;
                                            }
                                        }
                                    }

                                    _element.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        if (result.Result)
                                        {
                                            this.Hide(false);
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), null, PopupType.Information));

                                            model.PayOrder = null;
                                            model.OrderSession = newRoomSessionId;
                                            if (Resources.GetRes().CallNotifications.ContainsKey(model.RoomId))
                                                Resources.GetRes().CallNotifications.Remove(model.RoomId);
                                            Notification.Instance.ActionSendsFromService(null, new List<long>() { model.RoomId }, null);
                                        }
                                        else
                                        {
                                            if (result.IsRefreshSessionModel)
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                                Notification.Instance.ActionGetsFromService(null, new List<long>() { model.RoomId }, null);
                                            }
                                            else if (result.IsSessionModelSameTimeOperate)
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenWaitRetry"), null, PopupType.Warn));
                                            }
                                            else
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName), null, PopupType.Warn));
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
                                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName));
                                    }));

                                }
                                _element.Dispatcher.BeginInvoke(new Action(() =>
                               {
                                   _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                               }));
                            });

                        }, PopupType.Question));

                    });
                   
                }
                return _cancelCommand;
            }
        }




        /// <summary>
        /// 结账
        /// </summary>
        private RelayCommand _checkoutCommand;
        public ICommand CheckoutCommand
        {
            get
            {
                if (_checkoutCommand == null)
                {
                    _checkoutCommand = new RelayCommand(param =>
                    {
                        this.Hide(true);

                        RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == this.model.RoomId).FirstOrDefault();


                        if (null == model.PayOrder)
                        {
                            
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), (x) =>
                            {
                                // 重试代表订单数据不是最新的, 重新获取
                                Notification.Instance.ActionGetsFromService(null, new List<long>() { model.RoomId }, null);
                            }, PopupType.Warn));

                        }

                        else
                        {
                            RoomView.LastRoomId = model.RoomId;
                            _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.CheckoutOrder, model));
                        }
                    });
                }
                return _checkoutCommand;
            }
        }




        /// <summary>
        /// 替换
        /// </summary>
        private RelayCommand _replaceCommand;
        public ICommand ReplaceCommand
        {
            get
            {
                if (_replaceCommand == null)
                {
                    _replaceCommand = new RelayCommand(param =>
                    {
                        RoomView.Replace.InitialRooms(model);
                        this.IsDisplay = false;
                        this.IsShow = false;
                        RoomView.Replace.Show();
                        

                    });
                }
                return _replaceCommand;
            }
        }




        /// <summary>
        /// 关闭
        /// </summary>
        private RelayCommand _closeCommand;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                {
                    _closeCommand = new RelayCommand(param =>
                    {
                        this.Hide(false);
                    });
                }
                return _closeCommand;
            }
        }


    }
}
