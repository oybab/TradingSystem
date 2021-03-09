using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class ReplaceRoomWindow : KryptonForm
    {
        public long ReturnValue { get; private set; } //返回值
        private RoomModel oldModel;
        private string oldModelSession;
        private long IsPayByTime;

        public ReplaceRoomWindow(long RoomId)
        {
            oldModel = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
            oldModelSession = oldModel.OrderSession;

            this.IsPayByTime = Resources.GetRes().Rooms.Where(x => x.RoomId == oldModel.RoomId).FirstOrDefault().IsPayByTime;
            InitializeComponent();


            this.Text = Resources.GetRes().GetString("ReplaceRoom");

            krplNewRoom.Text = Resources.GetRes().GetString("NewRoom");
            krplRoomNo.Text = Resources.GetRes().GetString("RoomNo");
            

            krpbSave.Text = Resources.GetRes().GetString("Save");
            
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ReplaceRoom.ico"));

            krplRoomNoValue.Text = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).Select(x => x.RoomNo).FirstOrDefault();

            foreach (var item in Resources.GetRes().Rooms.Where(x => x.RoomId != RoomId && (x.HideType == 0 || x.HideType == 2) && x.IsPayByTime == this.IsPayByTime).OrderByDescending(x=>x.Order).ThenBy(x=>x.RoomNo.Length).ThenBy(x=>x.RoomNo).Select(x => x.RoomNo))
            {
                krpcbNewRoom.Items.Add(item);
            }

            if (krpcbNewRoom.Items.Count > 0)
                krpcbNewRoom.SelectedIndex = 0;
            else
                krpbSave.Enabled = false;

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {

            long newRoomId = Resources.GetRes().RoomsModel.Where(x => x.RoomNo == krpcbNewRoom.SelectedItem.ToString()).Select(x => x.RoomId).FirstOrDefault();
            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.GetRes().GetString("ReplaceRoom");
            string oldRoomSessionResult, newRoomSessionResult;

            StartLoad(this, null);

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

                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ReturnValue = newRoomId;
                            DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                if (oldRoomSessionResult == "-1")
                                    ReturnValue = oldModel.RoomId;
                                else if (newRoomSessionResult == "-1")
                                    ReturnValue = newRoomId;
                                
                                DialogResult = System.Windows.Forms.DialogResult.Retry;
                                this.Close();
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenWaitRetry"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), ErrMsgName));
                    }));
                }
                StopLoad(this, null);
            });
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
  
                    if (now > DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture))
                    {
                        now = DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                        IsTimeUp = true;
                    }


                    int totalMinute = (int)now.Subtract(DateTime.ParseExact(oldOrder.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;

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
                        int totalMinuteNew = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(now).TotalMinutes;
                        totalMinutesNew = ParseMinute(totalMinuteNew, !IsTimeUp); 
                    }
                    else if (oldRoom.IsPayByTime == 2)
                    {
                        int totalMinuteNew = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(now).TotalMinutes;
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


            tempOrder.TotalPrice = Math.Round(tempOrder.TotalPrice + tempOrder.RoomPrice,2);


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
                        totalMinutes = (int)DateTime.ParseExact(tempOrder.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(tempOrder.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
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












        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        /// <summary>
        /// 回车修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbNewRoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && krpbSave.Enabled)
            {
                krpbSave_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
                

        }

    }
}
