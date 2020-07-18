using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Oybab.Res.Tools;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class ReplaceViewModel : ViewModelBase
    {
        private UIElement _element;
        private Panel RoomList;
        private Style roomSelectStyle;
        private RoomModel oldModel;
        private string oldModelSession;


        internal ReplaceViewModel(UIElement element, Panel roomList)
        {
            this._element = element;
            this.RoomList = roomList;

            roomSelectStyle = (RoomList as FrameworkElement).FindResource("cbRoomSelectStyle") as Style;
        }


        private List<RoomStateModel> resultRoomList = new List<RoomStateModel>();
        /// <summary>
        /// 初始化雅座
        /// </summary>
        internal void InitialRooms(RoomStateModel stateModel)
        {
            this.OldRoomNo = stateModel.RoomNo;
            oldModel = Resources.GetRes().RoomsModel.Where(x => x.RoomId == stateModel.RoomId).FirstOrDefault();
            oldModelSession = oldModel.OrderSession;

            Room model = Resources.GetRes().Rooms.Where(x => x.RoomId == stateModel.RoomId).FirstOrDefault();
            resultRoomList.Clear();
            this.RoomList.Children.Clear();

            IEnumerable<Room> models = Resources.GetRes().Rooms.Where(x => x.RoomId != model.RoomId && (x.HideType == 0 || x.HideType == 2) && x.IsPayByTime == model.IsPayByTime).OrderByDescending(x => x.Order).ThenBy(x => x.RoomNo.Length).ThenBy(x => x.RoomNo);

            foreach (var item in models)
            {
                resultRoomList.Add(new RoomStateModel() { RoomNo = item.RoomNo, RoomId = item.RoomId });
            }

            
            foreach (var item in resultRoomList)
            {
                AddRoomsItem(item);
            }
        }





        /// <summary>
        /// 添加雅座
        /// </summary>
        /// <param name="item"></param>
        private void AddRoomsItem(RoomStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = roomSelectStyle;
                btn.DataContext = item;
                btn.Command = SelectRoom;
                btn.CommandParameter = item;
                RoomList.Children.Add(btn);
            }));
        }



        /// <summary>
        /// 打开产品列表
        /// </summary>
        private RelayCommand _selectRoom;
        public ICommand SelectRoom
        {
            get
            {
                if (_selectRoom == null)
                {
                    _selectRoom = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in resultRoomList)
                        {
                            if (model.RoomId == item.RoomId)
                                item.UseState = true;
                            else
                                item.UseState = false;

                        }


                    });
                }
                return _selectRoom;
            }
        }



        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {

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
            

        }






        



        private string _oldRoomNo = "";
        /// <summary>
        /// 新雅座编号
        /// </summary>
        public string OldRoomNo
        {
            get { return _oldRoomNo; }
            set
            {
                _oldRoomNo = value;
                OnPropertyChanged("OldRoomNo");
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
                        if (resultRoomList.Where(x => x.UseState).Count() == 0)
                            return;

                        long newRoomId = resultRoomList.Where(x => x.UseState).Select(x=>x.RoomId).FirstOrDefault();
                        string ErrMsgName, SucMsgName;
                        ErrMsgName = SucMsgName = Resources.GetRes().GetString("ReplaceRoom");
                        string oldRoomSessionResult, newRoomSessionResult;

                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                        Task.Factory.StartNew(() =>
                        {
                            try
                            {

                                
                                RoomModel newModel = Resources.GetRes().RoomsModel.Where(x => x.RoomId == newRoomId).FirstOrDefault();


                                ICollection<OrderDetail> oldOrderDetails = null;
                                ICollection<OrderDetail> newOrderDetails = null;
                                ICollection<OrderPay> oldOrderPays = null;
                                ICollection<OrderPay> newOrderPays = null;

                                if (null != oldModel.PayOrder && null != oldModel.PayOrder.tb_orderdetail)
                                    oldOrderDetails = oldModel.PayOrder.tb_orderdetail;
                                if (null != newModel.PayOrder && null != newModel.PayOrder.tb_orderdetail)
                                    newOrderDetails = newModel.PayOrder.tb_orderdetail;

                                if (null != oldModel.PayOrder && null != oldModel.PayOrder.tb_orderpay)
                                    oldOrderPays = oldModel.PayOrder.tb_orderpay;
                                if (null != newModel.PayOrder && null != newModel.PayOrder.tb_orderpay)
                                    newOrderPays = newModel.PayOrder.tb_orderpay;



                                Room oldRoom = Resources.GetRes().Rooms.Where(x => x.RoomId == oldModel.RoomId).FirstOrDefault();
                                Room newRoom = Resources.GetRes().Rooms.Where(x => x.RoomId == newRoomId).FirstOrDefault();


                                Order oldOrder = oldModel.PayOrder;
                                Order newOrder = newModel.PayOrder;

                                // 重新根据彼此的信息完全复制订单信息

                                Order tempOldOrder = new Order();
                                Order tempNewOrder = new Order();

                                // 更新老订单包厢价格信息
                                if (null != oldOrder)
                                {
                                    tempNewOrder = ReCalcOrder(oldOrder, newOrder, oldRoom, newRoom);
                                }
                                // 更新新订单包厢价格信息
                                if (null != newOrder)
                                {
                                    tempOldOrder = ReCalcOrder(newOrder, oldOrder, newRoom, oldRoom);
                                }

                                ResultModel result = OperatesService.GetOperates().ServiceReplaceOrder(oldModel.RoomId, newRoomId, tempOldOrder, tempNewOrder, oldModelSession, newModel.OrderSession, out oldRoomSessionResult, out newRoomSessionResult);
                                if (result.Result)
                                {


                                    long tempOld = 0;
                                    long tempNew = 0;

                                    if (null != oldOrder)
                                        tempOld = oldOrder.RoomId;
                                    else
                                        tempOld = oldModel.RoomId;

                                    if (null != newOrder)
                                        tempNew = newOrder.RoomId;
                                    else
                                        tempNew = newModel.RoomId;

                                    if (null != oldOrder)
                                        oldOrder.RoomId = tempNew;

                                    if (null != newOrder)
                                        newOrder.RoomId = tempOld;

                                    //删除老的房间中的订单并替换新的
                                    if (null != tempNewOrder && tempNewOrder.OrderId > 0)
                                        newModel.PayOrder = tempNewOrder;
                                    else
                                        newModel.PayOrder = null;

                                    //删除新的订单中的订单并替换老的
                                    if (null != tempOldOrder && tempOldOrder.OrderId > 0)
                                        oldModel.PayOrder = tempOldOrder;
                                    else
                                        oldModel.PayOrder = null;

                                    oldModel.OrderSession = oldRoomSessionResult;
                                    newModel.OrderSession = newRoomSessionResult;


                                    // 恢复对应的订单详情
                                    if (null != oldOrderDetails)
                                        newModel.PayOrder.tb_orderdetail = oldOrderDetails;

                                    if (null != newOrderDetails)
                                        oldModel.PayOrder.tb_orderdetail = newOrderDetails;


                                    if (null != oldOrderPays)
                                        newModel.PayOrder.tb_orderpay = oldOrderPays;

                                    if (null != newOrderPays)
                                        oldModel.PayOrder.tb_orderpay = newOrderPays;
                                }

                                _element.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    if (result.Result)
                                    {
                                        this.Hide();
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), null, PopupType.Information));

                                        Notification.Instance.ActionSendsFromService(null, new List<long>() { oldModel.RoomId, newRoomId }, null);
                                        
                                    }
                                    else
                                    {
                                        if (result.IsRefreshSessionModel)
                                        {
                                            this.Hide();
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                            if (oldRoomSessionResult == "-1")
                                                Notification.Instance.ActionGetsFromService(null, new List<long>() { oldModel.RoomId }, null);
                                            else if (newRoomSessionResult == "-1")
                                                Notification.Instance.ActionGetsFromService(null, new List<long>() { newRoomId }, null);
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





        private Order ReCalcOrder(Order oldOrder, Order newOrder, Room oldRoom, Room newRoom)
        {
            if (null == oldOrder)
                return null;

            Order tempOrder = oldOrder.FastCopy();

            int totalMinutesOld = 0;
            int totalMinutesNew = 0;

            tempOrder.TotalPrice = tempOrder.TotalPrice - tempOrder.RoomPrice;
            // 新增
            tempOrder.OriginalTotalPrice = tempOrder.OriginalTotalPrice - tempOrder.RoomPrice;

            if (oldRoom.IsPayByTime == 1 || oldRoom.IsPayByTime == 2)
            {
                // 担心这里出现一个问题, 就是因为时间获取的时候两个时间多算1分钟. 这就导致价格就会变更了.

                DateTime now = DateTime.Now;

                totalMinutesOld = 0;
                bool IsTimeUp = false;

                if (oldOrder.StartTime != oldOrder.EndTime)
                {

                    if (now > DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null))
                    {
                        now = DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null);
                        IsTimeUp = true;
                    }


                    int totalMinute = (int)now.Subtract(DateTime.ParseExact(oldOrder.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;

                    if (oldRoom.IsPayByTime == 1)
                    {
                        totalMinutesOld = ParseMinute(totalMinute, false);
                    }
                    else if (oldRoom.IsPayByTime == 2)
                    {
                        totalMinutesOld = ParseHour(totalMinute, false);
                    }



                    if (oldRoom.IsPayByTime == 1)
                    {
                        int totalMinuteNew = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(now).TotalMinutes;
                        totalMinutesNew = ParseMinute(totalMinuteNew, !IsTimeUp);
                    }
                    else if (oldRoom.IsPayByTime == 2)
                    {
                        int totalMinuteNew = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(now).TotalMinutes;
                        totalMinutesNew = ParseHour(totalMinuteNew, true);
                    }

                }








                tempOrder.RoomPrice = Math.Round(CommonOperates.GetCommonOperates().GetRoomPrice(oldRoom.Price, oldRoom.PriceHour, oldRoom.IsPayByTime, totalMinutesOld) + CommonOperates.GetCommonOperates().GetRoomPrice(newRoom.Price, newRoom.PriceHour, newRoom.IsPayByTime, totalMinutesNew), 2);

            }

            else
            {
                tempOrder.RoomPrice = newRoom.Price;
            }

            double lastTotal = tempOrder.TotalPrice;
            double lastOriginalTotalPrice = tempOrder.OriginalTotalPrice;


            tempOrder.TotalPrice = Math.Round(tempOrder.TotalPrice + tempOrder.RoomPrice, 2);


            // 新增
            tempOrder.OriginalTotalPrice = Math.Round(tempOrder.OriginalTotalPrice + tempOrder.RoomPrice, 2);


            // 注入雅座消费类型
            tempOrder.IsPayByTime = newRoom.IsPayByTime;

            bool _tempUnlimitedTime = (tempOrder.IsFreeRoomPrice == 2 ? true : false);

            // 如果超出最低消费,则清空雅座费
            if ((_tempUnlimitedTime) || (newRoom.FreeRoomPriceLimit > 0 && lastTotal >= newRoom.FreeRoomPriceLimit))
            {


                tempOrder.RoomPrice = 0;

                tempOrder.IsFreeRoomPrice = 1;

                if (_tempUnlimitedTime)
                {
                    tempOrder.IsFreeRoomPrice = 2;
                }

                tempOrder.TotalPrice = Math.Round(lastTotal, 2);
                tempOrder.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice, 2);

            }
            else
            {
                // 如果之前是免去了包厢费,现在需要去掉, 则重新计算房费
                if (tempOrder.IsFreeRoomPrice == 1)
                {
                    double totalMinutes = 0;
                    if (newRoom.IsPayByTime == 1 || newRoom.IsPayByTime == 2)
                    {
                        totalMinutes = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(tempOrder.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                    }
                    tempOrder.RoomPrice = Math.Round(CommonOperates.GetCommonOperates().GetRoomPrice(oldRoom.Price, oldRoom.PriceHour, oldRoom.IsPayByTime, totalMinutesOld) + CommonOperates.GetCommonOperates().GetRoomPrice(newRoom.Price, newRoom.PriceHour, newRoom.IsPayByTime, totalMinutesNew), 2);


                    tempOrder.TotalPrice = Math.Round(lastTotal + tempOrder.RoomPrice, 2);
                    tempOrder.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice + tempOrder.RoomPrice, 2);
                }

                tempOrder.IsFreeRoomPrice = 0;
            }





            double balancePrice = Math.Round(tempOrder.TotalPaidPrice - tempOrder.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                tempOrder.KeepPrice = balancePrice;
                tempOrder.BorrowPrice = 0;
            }
            else if (balancePrice < 0)
            {
                tempOrder.BorrowPrice = balancePrice;
                tempOrder.KeepPrice = 0;


            }
            else if (balancePrice == 0)
            {
                tempOrder.BorrowPrice = 0;
                tempOrder.KeepPrice = 0;
            }


            // 给这个订单新的房间号
            tempOrder.RoomId = newRoom.RoomId;


            return tempOrder;


        }





        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <returns></returns>
        private int ParseMinute(int totalMinute, bool IsNew)
        {

            if (IsNew)
                ++totalMinute;
            return totalMinute;
        }

        /// <summary>
        /// 格式化时间(用来短补, 长剪)
        /// </summary>
        /// <returns></returns>
        private int ParseHour(int totalMinute, bool IsNew)
        {
            int hour = totalMinute / 60;

            int SubCount = 0;

            if (IsNew)
                SubCount = 1;


            int MinutesIntervalTime = (int)TimeSpan.FromHours(Resources.GetRes().HoursIntervalTime).TotalMinutes;

            if (totalMinute < MinutesIntervalTime)
            {
                totalMinute = MinutesIntervalTime;
            }
            int temp = ((totalMinute / MinutesIntervalTime) - SubCount) * MinutesIntervalTime;
            if (totalMinute % MinutesIntervalTime > 0)
                temp += MinutesIntervalTime;

            return temp;
        }


    }
}
