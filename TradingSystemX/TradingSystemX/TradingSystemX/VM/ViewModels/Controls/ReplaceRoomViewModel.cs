using Oybab.DAL;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oybab.Res.Server.Model;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.Res.Server;
using Oybab.Res.Exceptions;
using Oybab.Res.Tools;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class ReplaceRoomViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;
        private RoomModel oldModel;
        private string oldModelSession;

        private Xamarin.Forms.StackLayout _spList;
        private Xamarin.Forms.ControlTemplate _ctControlTemplate;

        internal ReplaceRoomViewModel(Xamarin.Forms.Page element, Xamarin.Forms.StackLayout spList, Xamarin.Forms.ControlTemplate ctControlTemplate)
        {
            this._element = element;

            this._spList = spList;
            this._ctControlTemplate = ctControlTemplate;

        }


        private ObservableCollection<RoomStateModel> _roomList = new ObservableCollection<RoomStateModel>();
        /// <summary>
        /// 雅座列表0
        /// </summary>
        public ObservableCollection<RoomStateModel> RoomList
        {
            get { return _roomList; }
            set
            {
                _roomList = value;
                OnPropertyChanged("RoomList");
            }
        }



        /// <summary>
        /// 初始化雅座
        /// </summary>
        internal void InitialRooms(RoomStateModel stateModel)
        {
            this.OldRoomNo = stateModel.RoomNo;
            oldModel = Resources.Instance.RoomsModel.Where(x => x.RoomId == stateModel.RoomId).FirstOrDefault();
            oldModelSession = oldModel.OrderSession;

            Room model = Resources.Instance.Rooms.Where(x => x.RoomId == stateModel.RoomId).FirstOrDefault();

            ClearList();

            List<Room> models = Resources.Instance.Rooms.Where(x => x.RoomId != model.RoomId && (x.HideType == 0 || x.HideType == 2) && x.IsPayByTime == model.IsPayByTime).OrderByDescending(x => x.Order).ThenBy(x => x.RoomNo.Length).ThenBy(x => x.RoomNo).ToList();

            foreach (var item in models)
            {
                AddList(new RoomStateModel() { RoomNo = item.RoomNo, RoomId = item.RoomId });
            }

        }


        /// <summary>
        /// 清空
        /// </summary>
        private void ClearList()
        {

            foreach (Xamarin.Forms.TemplatedView item in this._spList.Children.Reverse())
            {
                item.BindingContext = null;
                item.IsVisible = false;
                if (!tempTemplateViewList.Contains(item))
                    tempTemplateViewList.Push(item);
            }


            RoomList.Clear();

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Index"></param>
        private void AddList(RoomStateModel model, int Index = -1)
        {
            model.GoCommand = SelectRoom;


            AddSelectedItem(model, Index);


            if (Index != -1)
                RoomList.Insert(Index, model);
            else
                RoomList.Add(model);
        }


        private Stack<Xamarin.Forms.TemplatedView> tempTemplateViewList = new Stack<Xamarin.Forms.TemplatedView>();
        /// <summary>
        /// 添加已选对象
        /// </summary>
        /// <param name="item"></param>
        private void AddSelectedItem(RoomStateModel item, int Index = -1)
        {


            Xamarin.Forms.TemplatedView view = null;
            if (tempTemplateViewList.Count > 0)
            {
                view = tempTemplateViewList.Pop();
                view.IsVisible = true;
                view.BindingContext = item;
            }
            else
            {
                view = new Xamarin.Forms.TemplatedView();
                view.ControlTemplate = _ctControlTemplate;

                view.BindingContext = item;
                _spList.Children.Add(view);
            }


        }

        /// <summary>
        /// 删除已选
        /// </summary>
        /// <param name="item"></param>
        private void RemoveSelected(RoomStateModel item)
        {
        

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
                if (!tempTemplateViewList.Contains(_view))
                    tempTemplateViewList.Push(_view);
                _view.IsVisible = false;
            }

                RoomList.Remove(item);
        }






        /// <summary>
        /// 打开产品列表
        /// </summary>
        private RelayCommand _selectRoom;
        public Command SelectRoom
        {
            get
            {
                return _selectRoom ?? (_selectRoom = new RelayCommand(param =>
                {
                    RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;


                        foreach (var item in RoomList)
                        {
                            if (model.RoomId == item.RoomId)
                                item.UseState = true;
                            else
                                item.UseState = false;

                        }
                    }));
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
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public Xamarin.Forms.Command OKCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand(param =>
                {
                    if (RoomList.Where(x => x.UseState).Count() == 0)
                        return;

                    long newRoomId = RoomList.Where(x => x.UseState).Select(x => x.RoomId).FirstOrDefault();
                    string ErrMsgName, SucMsgName;
                    ErrMsgName = SucMsgName = Resources.Instance.GetString("ReplaceRoom");
                    string oldRoomSessionResult, newRoomSessionResult;

                    IsLoading = true;

                    Task.Factory.StartNew(async () =>
                    {
                        try
                        {

                            await ExtX.WaitForLoading();
                            RoomModel newModel = Resources.Instance.RoomsModel.Where(x => x.RoomId == newRoomId).FirstOrDefault();


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



                            Room oldRoom = Resources.Instance.Rooms.Where(x => x.RoomId == oldModel.RoomId).FirstOrDefault();
                            Room newRoom = Resources.Instance.Rooms.Where(x => x.RoomId == newRoomId).FirstOrDefault();


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

                            var taskResult = await OperatesService.Instance.ServiceReplaceOrder(oldModel.RoomId, newRoomId, tempOldOrder, tempNewOrder, oldModelSession, newModel.OrderSession);

                            ResultModel result = taskResult.resultModel;
                            oldRoomSessionResult = taskResult.oldRoomSessionResult;
                            newRoomSessionResult = taskResult.newRoomSessionResult;

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

                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), SucMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);


                                    // 为了如果要自动检测账单刷新时, 一定修改这个逻辑, 可确定取本地房间ID是old或new以后, 直接用OrderViewModel里一样直接操作上一个房间的刷新操作, 然后新房间直接用下面的操作, 这样当前不会提示房间刷新
                                    // [推荐]或者为了方便, 直接先跳转到上一个页面后再进行推送操作(这个方便, 只要没延迟就好), 也就是把下面两行顺序修改一下
                                    Notification.Instance.ActionSendsFromService(null, new List<long>() { oldModel.RoomId, newRoomId }, null);

                                    NavigationPath.Instance.SwitchNavigate(1);

                                    ClearList();

                                }
                                else
                                {
                                    if (result.IsRefreshSessionModel)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                        if (oldRoomSessionResult == "-1")
                                            Notification.Instance.ActionGetsFromService(null, new List<long>() { oldModel.RoomId }, null);
                                        else if (newRoomSessionResult == "-1")
                                            Notification.Instance.ActionGetsFromService(null, new List<long>() { newRoomId }, null);

                                        NavigationPath.Instance.SwitchNavigate(1);

                                        ClearList();

                                    }
                                    else if (result.IsSessionModelSameTimeOperate)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenWaitRetry"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                    }
                                    else
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                    }
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                            {
                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                                }), false, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName));
                            }));
                        }
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                        {
                            IsLoading = false;
                        }));
                    });

                }));
            }
        }


        /// <summary>
        /// 删除按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public Xamarin.Forms.Command CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param =>
                {

                }));
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
                        totalMinutesNew = ParseMinute(totalMinuteNew, !IsTimeUp); //  ((totalMinuteNew + totalMinutesOld > (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(tempOrder.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes) || IsTimeUp)
                    }
                    else if (oldRoom.IsPayByTime == 2)
                    {
                        int totalMinuteNew = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(now).TotalMinutes;
                        totalMinutesNew = ParseHour(totalMinuteNew, true); // ((totalMinuteNew + totalMinutesOld > (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(tempOrder.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes) || IsTimeUp)
                    }

                }


               

                    tempOrder.RoomPrice = Math.Round(CommonOperates.Instance.GetRoomPrice(oldRoom.Price, oldRoom.PriceHour, oldRoom.IsPayByTime, totalMinutesOld) + CommonOperates.Instance.GetRoomPrice(newRoom.Price, newRoom.PriceHour, newRoom.IsPayByTime, totalMinutesNew), 2);

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
                    tempOrder.RoomPrice = Math.Round(CommonOperates.Instance.GetRoomPrice(oldRoom.Price, oldRoom.PriceHour, oldRoom.IsPayByTime, totalMinutesOld) + CommonOperates.Instance.GetRoomPrice(newRoom.Price, newRoom.PriceHour, newRoom.IsPayByTime, totalMinutesNew), 2);


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
        /// 格式化时间(用来短补, 长剪) 比如假设最低分钟5分钟. 1分钟5分钟均=5分钟. 6分钟,9分钟都=10分钟
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


            int MinutesIntervalTime = (int)TimeSpan.FromHours(Resources.Instance.HoursIntervalTime).TotalMinutes;

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
