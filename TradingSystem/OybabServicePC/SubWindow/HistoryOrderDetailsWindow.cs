using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.ServicePC.DialogWindow;
using Oybab.ServicePC.Tools;
using Newtonsoft.Json;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class HistoryOrderDetailsWindow : KryptonForm
    {
        //页数变量
        private int ListCountOld = 100;
        private int ListCount = 1;
        private int CurrentPageOld = 1;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private Order CurrentOrder;
        private List<OrderDetail> CurrentOrderDetails = null;
        private List<OrderPay> CurrentOrderPays = null;

        List<Log> logList = new List<Log>();

        public HistoryOrderDetailsWindow(List<Log> logList)
        {

            this.logList = logList;


            InitializeComponent();

            ResetPage();
            new CustomTooltip(this.krpdgList);
            this.Text = Resources.GetRes().GetString("OrderDetails");

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");


            krplOrderNo.Text = Resources.GetRes().GetString("OrderNo");
            krplRoomNo.Text = Resources.GetRes().GetString("RoomNo");
            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            krplState.Text = Resources.GetRes().GetString("State");
            krplStartTime.Text = Resources.GetRes().GetString("StartTime");
            krplEndTime.Text = Resources.GetRes().GetString("EndTime");
            krplAddTime.Text = Resources.GetRes().GetString("AddTime");
            krplChangeTime.Text = Resources.GetRes().GetString("ChangeTime");



            krplBorrowPrice.Text = Resources.GetRes().GetString("OwedPrice");
            krplKeepPrice.Text = Resources.GetRes().GetString("KeepPrice");
            krplMemberName.Text = Resources.GetRes().GetString("MemberName");

            krplMemberPaidPrice.Text = Resources.GetRes().GetString("MemberPaidPrice");
            krplTotalPaidPrice.Text = Resources.GetRes().GetString("TotalPaidPrice");
            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");
            krplRoomPrice.Text = Resources.GetRes().GetString("RoomPrice");
            krplAdminName.Text = Resources.GetRes().GetString("AdminName");
            krplRemark.Text = Resources.GetRes().GetString("Remark");
            krplLanguage.Text = Resources.GetRes().GetString("Language");


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }



           
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.OrderDetails.ico"));

            //初始化
            Init();


            ResetPage();

            AllPage = (int)((logList.Count() - 1 + ListCount) / ListCount);
            krplPageCount.Text = AllPage.ToString();

            CurrentPage = 1;
            krptCurrentPage.Text = CurrentPage.ToString();
            if (logList.Count() > 0)
            {
                OpenPageTo(logList.Count, false);
            }

        }



        private void OpenOrderIndex(Log log, Order order, List<OrderDetail> orderdetails, List<OrderPay> orderpays)
        {


            krpdgList.RecalcMagnification();
            krpdgList.RecalcMagnification();



            if (log.AdminId > 0)
            {
                Admin FinishAdmin = Resources.GetRes().Admins.Where(x => x.AdminId == log.AdminId).FirstOrDefault();
                if (null != FinishAdmin)
                {
                    if (Resources.GetRes().MainLangIndex == 0)
                        krplAdminNameValue.Text = string.Format(": {0}", FinishAdmin.AdminName0);
                    else if (Resources.GetRes().MainLangIndex == 1)
                        krplAdminNameValue.Text = string.Format(": {0}", FinishAdmin.AdminName1);
                    else if (Resources.GetRes().MainLangIndex == 2)
                        krplAdminNameValue.Text = string.Format(": {0}", FinishAdmin.AdminName2);
                }
            }


            krplRemarkValue.Text = string.Format(": {0}", order.Remark ?? "");


            krplOrderNoValue.Text = string.Format(": {0}", order.OrderId.ToString());
            krplRoomNoValue.Text = string.Format(": {0}", Resources.GetRes().Rooms.Where(x => x.RoomId == order.RoomId).Select(x => x.RoomNo).FirstOrDefault());
            krplTotalPriceValue.Text = string.Format(": {0}", order.TotalPrice.ToString());
            krplStateValue.Text = string.Format(": {0}", GetOrderState(order.State));
            krplRoomPriceValue.Text = string.Format(": {0}", order.RoomPrice.ToString());

            if (order.IsFreeRoomPrice == 2)
                krplRoomPriceValue.Text = string.Format(": {0}", Resources.GetRes().GetString("UnlimitedTime"));

            krplBorrowPriceValue.Text = string.Format(": {0}", order.BorrowPrice.ToString());
            krplKeepPriceValue.Text = string.Format(": {0}", order.KeepPrice.ToString());


            if (null == order.tb_member)
                krplMemberNameValue.Text = ":";
            else
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krplMemberNameValue.Text = string.Format(": {0}", order.tb_member.MemberName0.ToString());
                else if (Resources.GetRes().MainLangIndex == 1)
                    krplMemberNameValue.Text = string.Format(": {0}", order.tb_member.MemberName1.ToString());
                else if (Resources.GetRes().MainLangIndex == 2)
                    krplMemberNameValue.Text = string.Format(": {0}", order.tb_member.MemberName2.ToString());
            }

            krplMemberPaidPriceValue.Text = string.Format(": {0}", order.MemberPaidPrice.ToString());
            krplTotalPaidPriceValue.Text = string.Format(": {0}", order.TotalPaidPrice.ToString());
            krplPaidPriceValue.Text = string.Format(": {0}", order.PaidPrice.ToString());
            krplChangeTimeValue.Text = string.Format(": {0}", DateTime.ParseExact(log.AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"));

            
            krplLanguageValue.Text = string.Format(": {0}", Resources.GetRes().GetLangByLangIndex((int)order.Lang).LangName);
            

            try
            {
                if (null == order.StartTime)
                    krplStartTimeValue.Text = ":";
                else
                    krplStartTimeValue.Text = string.Format(": {0}", DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"));

                if (null == order.EndTime)
                    krplEndTimeValue.Text = ":";
                else
                    krplEndTimeValue.Text = string.Format(": {0}", DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"));


                if (0 == order.AddTime)
                    krplAddTimeValue.Text = ":";
                else
                    krplAddTimeValue.Text = string.Format(": {0}", DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"));

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            double balancePrice = Math.Round(order.TotalPaidPrice - order.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
                this.krplKeepPriceValue.StateCommon.ShortText.Color1 = Color.Blue;
            else if (balancePrice < 0)
                this.krplBorrowPriceValue.StateCommon.ShortText.Color1 = Color.Red;







            //获取数据
            var currentResult = orderdetails.OrderByDescending(x => x.OrderDetailId).Skip((CurrentPageOld - 1) * ListCountOld).Take(ListCountOld);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in orderdetails)
            {
                AddToGrid("", item.OrderDetailId.ToString(), item.ProductId, item.Price, item.Count, item.TotalPrice, item.IsPack, GetOrderDetailsState(item.State), item.Request, item.AddTime, log.AdminId, log.AddTime);
            }

            // 待确认改为红色
            for (int i = 0; i < krpdgList.Rows.Count; i++)
            {
                try
                {
                    if (1 == GetOrderDetailsStateNo(krpdgList.Rows[i].Cells["krpcmState"].Value.ToString()))
                    {
                        krpdgList.Rows[i].Cells["krpcmState"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmState"].Style.SelectionForeColor = Color.Red;
                    }
                    if (3 == GetOrderDetailsStateNo(krpdgList.Rows[i].Cells["krpcmState"].Value.ToString()))
                    {

                        for (int j = 0; j < krpdgList.Rows[i].Cells.Count; j++)
                        {
                            krpdgList.Rows[i].Cells[j].Style.ForeColor = krpdgList.Rows[i].Cells[j].Style.SelectionForeColor = Color.Gray;
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            }

            krpdgPayList.Rows.Clear();
            if (null != orderpays)
            {
                //获取数据
                var currentResult2 = orderpays.OrderByDescending(x => x.OrderPayId);
                //添加到数据集中

                foreach (var item in currentResult2)
                {
                    AddToGrid2("", item.OrderPayId.ToString(), GetOrderDetailsState(item.State), item.BalanceId, item.tb_member, item.OriginalPrice, item.RemovePrice, item.BalancePrice, item.AddTime, item.AdminId, log.AddTime);
                }

                // 待确认改为红色
                for (int i = 0; i < krpdgPayList.Rows.Count; i++)
                {
                    try
                    {
                        if (3 == GetOrderDetailsStateNo(krpdgPayList.Rows[i].Cells["krpcmState2"].Value.ToString()))
                        {

                            for (int j = 0; j < krpdgPayList.Rows[i].Cells.Count; j++)
                            {
                                krpdgPayList.Rows[i].Cells[j].Style.ForeColor = krpdgPayList.Rows[i].Cells[j].Style.SelectionForeColor = Color.Gray;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
            }
        }






        /// <summary>
        /// 加入
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="State"></param>
        /// <param name="balanceId"></param>
        /// <param name="member"></param>
        /// <param name="IsPaidByCard"></param>
        /// <param name="OriginalPrice"></param>
        /// <param name="RemovePrice"></param>
        /// <param name="BalancePrice"></param>
        /// <param name="AddTime"></param>
        /// <param name="AdminId"></param>
        private void AddToGrid2(string editMark, string Id, string State, long? balanceId, Member member, double OriginalPrice, double RemovePrice, double BalancePrice, long AddTime, long AdminId, long LogTime)
        {
            string balanceName = "";
            string AddTimeStr = "";
            string AdminNameStr = "";
            string memberName = "";

            try
            {

                if (null != member)
                {
                    if (Resources.GetRes().MainLangIndex == 0)
                        memberName = member.MemberName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        memberName = member.MemberName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        memberName = member.MemberName2;

                }

                if (null != balanceId)
                {
                    Balance balance = Resources.GetRes().Balances.Where(x => x.BalanceId == balanceId).FirstOrDefault();

                    if (Resources.GetRes().MainLangIndex == 0)
                        balanceName = balance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        balanceName = balance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        balanceName = balance.BalanceName2;


                }


                if (0 == AddTime)
                    AddTimeStr = DateTime.ParseExact(LogTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");
                else
                    AddTimeStr = DateTime.ParseExact(AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");


                if (AdminId > 0)
                {
                    Admin SaleAdmin = Resources.GetRes().Admins.Where(x => x.AdminId == AdminId).FirstOrDefault();
                    if (null != SaleAdmin)
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            AdminNameStr = SaleAdmin.AdminName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            AdminNameStr = SaleAdmin.AdminName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            AdminNameStr = SaleAdmin.AdminName2;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
                krpdgPayList.Rows.Insert(0, editMark, Id, State, balanceName, memberName, OriginalPrice.ToString(), RemovePrice.ToString(), BalancePrice.ToString(), AddTimeStr, AdminNameStr);
            else
                krpdgPayList.Rows.Add(editMark, Id, State, balanceName, memberName, OriginalPrice.ToString(), RemovePrice.ToString(), BalancePrice.ToString(), AddTimeStr, AdminNameStr);
        }

        /// <summary>
        /// 初始化右键
        /// </summary>
        /// <param name="index"></param>
        /// <param name="message"></param>
        /// <param name="image"></param>
        /// <param name="handler"></param>
        private void LoadContextMenu(KryptonContextMenuItem item, string message, string ExtraMessage, Image image, EventHandler handler)
        {
            item.Text = message;
            item.Image = image;
            item.ExtraText = ExtraMessage;
            item.Click += handler;
        }

        /// <summary>
        /// 重置分页
        /// </summary>
        private void ResetPage()
        {
            krplPageCount.Text = "1";
            krptCurrentPage.Text = "1";
            krptCurrentPage.Enabled = krpbBeginPage.Enabled = krpbPrewPage.Enabled = krpbNextPage.Enabled = krpbEngPage.Enabled = krpbClickToPage.Enabled = false;
        }


        /// <summary>
        /// 设置列
        /// </summary>
        private void Init()
        {
            krpcmOrderDetailId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductName.HeaderText = Resources.GetRes().GetString("ProductName");
            krpcmPrice.HeaderText = Resources.GetRes().GetString("UnitPrice");
            krpcmCount.HeaderText = Resources.GetRes().GetString("Count");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");
            krpcmIsPack.HeaderText = Resources.GetRes().GetString("Package");
            krpcmState.HeaderText = Resources.GetRes().GetString("State");
            krpcmRequest.HeaderText = Resources.GetRes().GetString("Request2");
            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcmAdmin.HeaderText = Resources.GetRes().GetString("Admin");




            krpcmPayId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmBalanceName.HeaderText = Resources.GetRes().GetString("BalanceName");
            krpcmMemberName.HeaderText = Resources.GetRes().GetString("MemberName");
            krpcmOriginalPrice.HeaderText = Resources.GetRes().GetString("Price");
            krpcmRemovePrice.HeaderText = Resources.GetRes().GetString("RemovePrice");
            krpcmBalancePrice.HeaderText = Resources.GetRes().GetString("BalancePrice");
            krpcmAddTime2.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcmAdmin2.HeaderText = Resources.GetRes().GetString("Admin");
        }


        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSearch_Click(object sender, EventArgs e)
        {
            //为未保存数据而忽略当前操作
            if (!IgnoreOperateForSave())
                return;

        }

        /// <summary>
        /// 打开某页
        /// </summary>
        /// <param name="pageNo"></param>
        private void OpenPageTo(int pageNo, bool Manual = true)
        {
            //先判断是否能去这个页
            if (pageNo < 1 || pageNo > AllPage)
            {
                return;
            }
            if (CurrentPage == pageNo && Manual)
                return;

            //为未保存数据而忽略当前操作
            if (Manual && !IgnoreOperateForSave())
                return;


            //设定按钮
            krptCurrentPage.Enabled = AllPage > 1;
            krpbBeginPage.Enabled = pageNo > 1;
            krpbEngPage.Enabled = pageNo < AllPage;
            krpbNextPage.Enabled = pageNo < AllPage;
            krpbPrewPage.Enabled = pageNo > 1;
            krpbClickToPage.Enabled = AllPage > 1;


            CurrentPage = pageNo;
            krptCurrentPage.Text = CurrentPage.ToString();


            Log log = logList[pageNo - 1];

            CurrentOrder = null;
            CurrentOrderDetails = null;

            if (log.OperateName.StartsWith("Order#Add"))
            {
                ToServerServiceNewOrder newOrder = log.Model.DeserializeObject<ToServerServiceNewOrder>();

                CurrentOrder = newOrder.Order.DeserializeObject<Order>();
                CurrentOrderDetails = newOrder.OrderDetails.DeserializeObject<List<OrderDetail>>();
                CurrentOrderPays = newOrder.OrderPays.DeserializeObject<List<OrderPay>>();
            }
            else if (log.OperateName.StartsWith("Order#Edit"))
            {
                ToServerServiceEditOrder editOrder = log.Model.DeserializeObject<ToServerServiceEditOrder>();

                CurrentOrder = editOrder.Order.DeserializeObject<Order>();
                CurrentOrderPays = editOrder.OrderPays.DeserializeObject<List<OrderPay>>();
                if (null != CurrentOrder.tb_orderdetail)
                    CurrentOrderDetails = CurrentOrder.tb_orderdetail.ToList();
                else
                    CurrentOrderDetails = new List<OrderDetail>();
            }
            else if (log.OperateName.StartsWith("Order#Replace"))
            {
                ToServerServiceReplaceOrder replaceOrder = log.Model.DeserializeObject<ToServerServiceReplaceOrder>();



                // 因为记录时只记录老的订单编号, 也就是newOrder.OrderId, 考虑到OperateId是老的订单,(毕竟有两个有客户的客座修改可能性也比较小------以前这么认为的, 但是两个客座都在用时替换就出问题, 所以增加了)
                Order oldOrder = replaceOrder.OldOrder.DeserializeObject<Order>();
                Order newOrder = replaceOrder.NewOrder.DeserializeObject<Order>();

                if (log.OperateId == oldOrder.OrderId)
                    CurrentOrder = oldOrder;
                else if (log.OperateId == newOrder.OrderId)
                    CurrentOrder = newOrder;
                else
                    CurrentOrder = new Order();

                if (null != CurrentOrder.tb_orderdetail)
                    CurrentOrderDetails = CurrentOrder.tb_orderdetail.ToList();
                else
                    CurrentOrderDetails = new List<OrderDetail>();

                if (null != CurrentOrder.tb_orderpay)
                    CurrentOrderPays = CurrentOrder.tb_orderpay.ToList();
                else
                    CurrentOrderPays = new List<OrderPay>();
            }
            else if (log.OperateName.StartsWith("OrderDetail#Save"))
            {
                ToServerServiceSaveOrderDetail saveOrderDetail = log.Model.DeserializeObject<ToServerServiceSaveOrderDetail>();

                CurrentOrder = saveOrderDetail.Order.DeserializeObject<Order>();
                CurrentOrderDetails = saveOrderDetail.OrderDetailsAdd.DeserializeObject<List<OrderDetail>>();
                CurrentOrderPays = saveOrderDetail.OrderPaysAdd.DeserializeObject<List<OrderPay>>();
            }
            else if (log.OperateName.StartsWith("OrderDetail#Delete"))
            {
                ToServerServiceDelOrderDetail delOrderDetail = log.Model.DeserializeObject<ToServerServiceDelOrderDetail>();

                CurrentOrder = delOrderDetail.Order.DeserializeObject<Order>();
                CurrentOrderDetails = delOrderDetail.OrderDetails.DeserializeObject<List<OrderDetail>>();
                CurrentOrderPays = new List<OrderPay>();
            }
            else
            {
                CurrentOrder = new Order();
                CurrentOrderDetails = new List<OrderDetail>();
                CurrentOrderPays = new List<OrderPay>();
            }


            if (CurrentOrder != null)
            {
                OpenOrderIndex(log, CurrentOrder, CurrentOrderDetails, CurrentOrderPays);
            }


        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="productId"></param>
        /// <param name="Price"></param>
        /// <param name="Count"></param>
        /// <param name="TotalPrice"></param>
        /// <param name="State"></param>
        /// <param name="AddTime"></param>
        private void AddToGrid(string editMark, string Id, long productId, double Price, double Count, double TotalPrice, long IsPack, string State, string Request, long AddTime, long? AdminId, long LogTime)
        {
            string productName = "";
            string AddTimeStr = "";
            string RequestNames = "";
            string AdminNameStr = "";

            try
            {

                if (!string.IsNullOrWhiteSpace(Request))
                {
                    List<long> requestsIdList = Request.Split(',').Select(x => long.Parse(x)).ToList();
                    List<Request> requestList = Resources.GetRes().Requests.Where(x => requestsIdList.Contains(x.RequestId)).Distinct().ToList();

                    if (Resources.GetRes().MainLangIndex == 0)
                        RequestNames = string.Join("&", requestList.Select(x => x.RequestName0));
                    else if (Resources.GetRes().MainLangIndex == 1)
                        RequestNames = string.Join("&", requestList.Select(x => x.RequestName1));
                    else if (Resources.GetRes().MainLangIndex == 2)
                        RequestNames = string.Join("&", requestList.Select(x => x.RequestName2));


                }

                if (0 == AddTime)
                    AddTimeStr = DateTime.ParseExact(LogTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");
                else
                    AddTimeStr = DateTime.ParseExact(AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                if (Resources.GetRes().MainLangIndex == 0)
                    productName = Resources.GetRes().Products.Where(x => x.ProductId == productId).Select(x => x.ProductName0).FirstOrDefault();
                else if (Resources.GetRes().MainLangIndex == 1)
                    productName = Resources.GetRes().Products.Where(x => x.ProductId == productId).Select(x => x.ProductName1).FirstOrDefault();
                else if (Resources.GetRes().MainLangIndex == 2)
                    productName = Resources.GetRes().Products.Where(x => x.ProductId == productId).Select(x => x.ProductName2).FirstOrDefault();


                if (null != AdminId && AdminId > 0)
                {
                    Admin SaleAdmin = Resources.GetRes().Admins.Where(x => x.AdminId == AdminId).FirstOrDefault();
                    if (null != SaleAdmin)
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            AdminNameStr = SaleAdmin.AdminName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            AdminNameStr = SaleAdmin.AdminName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            AdminNameStr = SaleAdmin.AdminName2;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), IsPack.ToString(), State, RequestNames, AddTimeStr, AdminNameStr);
            else
                krpdgList.Rows.Add(editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), IsPack.ToString(), State, RequestNames, AddTimeStr, AdminNameStr);
        }

        /// <summary>
        /// 转到首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbBeginPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(1);
        }

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbPrewPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(CurrentPage - 1);
        }

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbNextPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(CurrentPage + 1);
        }

        /// <summary>
        /// 末页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbEngPage_Click(object sender, EventArgs e)
        {
            OpenPageTo(AllPage);
        }

        /// <summary>
        /// 转到指定页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbClickToPage_Click(object sender, EventArgs e)
        {
            int page = 0;
            int.TryParse(krptCurrentPage.Text, out page);
            OpenPageTo(page);
        }
        /// <summary>
        /// 同上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptCurrentPage_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                krpbClickToPage_Click(null, null);
        }

        private string temp = "";

        /// <summary>
        /// 刚开始编辑的时候存下值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (null != krpdgList.CurrentCell.Value)
                temp = krpdgList.CurrentCell.Value.ToString();
            else
                temp = "";
        }

        private DataGridViewCell _celWasEndEdit;

        /// <summary>
        /// 编辑完了以后,需要添加型号表示已修改.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _celWasEndEdit = krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex];
            //导致整行选中出现没改动也误以为改动情况
            if (null == krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value)
            {
                if (null == temp)
                    return;
                else if (temp == "")
                    krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                else
                {
                    krpdgList.Rows[e.RowIndex].Cells["krpcmEdit"].Value = "*";
                    krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                }
            }
            else if (!krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Equals(temp))
                krpdgList.Rows[e.RowIndex].Cells["krpcmEdit"].Value = "*";
        }



        private void krpdgList_SelectionChanged(object sender, EventArgs e)
        {
            {

                if (MouseButtons != 0)
                {
                    SetCurrentCell(krpdgList.CurrentCell.ColumnIndex - 1, krpdgList.CurrentCell.RowIndex);
                }

                else if (_celWasEndEdit != null && krpdgList.CurrentCell != null)
                {
                    // if we are currently in the next line of last edit cell

                    int iColumn = _celWasEndEdit.ColumnIndex;
                    int iRow = _celWasEndEdit.RowIndex;

                    SetCurrentCell(iColumn, iRow);
                }
                _celWasEndEdit = null;
            }
        }

        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                krpdgList.RowHeadersWidth,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1 + ((CurrentPageOld - 1) * ListCountOld)).ToString(),
                krpdgList.RowHeadersDefaultCellStyle.Font,
                rectangle,
                krpdgList.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }


        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgPayList_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                krpdgPayList.RowHeadersWidth,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1 + ((CurrentPage - 1) * ListCount)).ToString(),
                krpdgPayList.RowHeadersDefaultCellStyle.Font,
                rectangle,
                krpdgPayList.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }


        /// <summary>
        /// 下拉框立即显示(替换为EditingControlShowing)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1 && null != (krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex] as KryptonDataGridViewComboBoxCell))
            {
                krpdgList.BeginEdit(true);
                KryptonDataGridViewComboBoxEditingControl control = krpdgList.EditingControl as KryptonDataGridViewComboBoxEditingControl;
                if (null != control)
                    control.DroppedDown = true;
            }
        }


        /// <summary>
        /// 退出前判断是否还有数据没保存
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            //有尚未保存的数据
            if (!IgnoreOperateForSave())
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 是否为未保存数据而忽略当前的操作
        /// </summary>
        /// <returns></returns>
        private bool IgnoreOperateForSave()
        {
            bool notHandle = false;
            foreach (DataGridViewRow row in krpdgList.Rows)
            {
                if (row.Cells["krpcmEdit"].Value.Equals("*"))
                {
                    notHandle = true;
                    break;
                }
            }
            if (notHandle)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("OrderDetails"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }



        /// <summary>
        /// 显示右键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            //右键
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                
            }
        }

        /// <summary>
        /// 退出编辑模式
        /// </summary>
        private void ExitEditMode()
        {
            if (krpdgList.IsCurrentCellInEditMode)
            {
                krpdgList.EndEdit();
                krpdgList.ClearSelection();
            }
        }

        /// <summary>
        /// 单击空白处事件,只为显示增加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_MouseClick(object sender, MouseEventArgs e)
        {
            //右键
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCellMouseEventArgs args = new DataGridViewCellMouseEventArgs(-1, -1, 0, 0, e);
                krpdgList_CellMouseClick(sender, args);
            }
        }


        private Keys lastKeyPressed = Keys.EraseEof;
        /// <summary>
        /// 设置快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_KeyUp(object sender, KeyEventArgs e)
        {
            if (!e.Control)
                lastKeyPressed = Keys.EraseEof;

        }


        private void krpdgList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && lastKeyPressed != Keys.EraseEof)
            {
                // 打印
                if (e.KeyCode == Keys.P)
                {
                    Print();
                }
            }
            else if (e.Control)
                lastKeyPressed = e.KeyCode;
            else if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (null != krpdgList.CurrentCell)
                {
                    int iColumn = krpdgList.CurrentCell.ColumnIndex;
                    int iRow = krpdgList.CurrentCell.RowIndex;

                    SetCurrentCell(iColumn, iRow);
                }

            }
        }

        /// <summary>
        /// 设置当前行
        /// </summary>
        /// <param name="iColumn"></param>
        /// <param name="iRow"></param>
        private void SetCurrentCell(int iColumn, int iRow)
        {
            if (iRow == -1) return;
            // 如果是最后一列
            if (iColumn == krpdgList.Columns.Count - 1)
            {
                // 如果不是最后一行则换行
                if (iRow != krpdgList.Rows.Count - 1)
                {
                    if (krpdgList[0, iRow + 1].Visible == true)
                        krpdgList.CurrentCell = krpdgList[0, iRow + 1];
                    else
                        SetCurrentCell(0, iRow + 1);
                };
            }
            else
            {
                // 继续换到下一列
                if (krpdgList[iColumn + 1, iRow].Visible == true)
                    krpdgList.CurrentCell = krpdgList[iColumn + 1, iRow];
                else
                    SetCurrentCell(iColumn + 1, iRow);
            }
        }

        /// <summary>
        /// 为了选中行的时候能用快捷键,并退出其他编辑模式,把位置定位到只读的单元
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.StateChanged == DataGridViewElementStates.Selected)
            {
                krpdgList.CurrentCell = e.Row.Cells[0];
            }
        }



        /// <summary>
        /// 获取订单详情状态编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetOrderDetailsStateNo(string orderState)
        {
            if (orderState == "-")
                return 0;
            else if (orderState == Resources.GetRes().GetString("Requesting"))
                return 1;
            else if (orderState == Resources.GetRes().GetString("Confirmed"))
                return 2;
            else if (orderState == Resources.GetRes().GetString("Canceld"))
                return 3;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取订单详情状态
        /// </summary>
        /// <param name="orderStateNo"></param>
        /// <returns></returns>
        private string GetOrderDetailsState(long orderStateNo)
        {
            if (orderStateNo == 0)
                return "-";
            else if (orderStateNo == 1)
                return Resources.GetRes().GetString("Requesting");
            else if (orderStateNo == 2)
                return Resources.GetRes().GetString("Confirmed");
            else if (orderStateNo == 3)
                return Resources.GetRes().GetString("Canceld");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }


        /// <summary>
        /// 获取订单状态编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetOrderStateNo(string orderState)
        {
            if (orderState == Resources.GetRes().GetString("Consumption"))
                return 0;
            else if (orderState == Resources.GetRes().GetString("Checkout"))
                return 1;
            else if (orderState == Resources.GetRes().GetString("Invalid"))
                return 2;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取订单状态
        /// </summary>
        /// <param name="orderStateNo"></param>
        /// <returns></returns>
        private string GetOrderState(long orderStateNo)
        {
            if (orderStateNo == 0)
                return Resources.GetRes().GetString("Consumption");
            else if (orderStateNo == 1)
                return Resources.GetRes().GetString("Checkout");
            else if (orderStateNo == 2)
                return Resources.GetRes().GetString("Invalid");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }


        /// <summary>
        /// 打印
        /// </summary>
        private void Print()
        {
            PrintLanguageWindow window = new PrintLanguageWindow();
            window.ShowDialog(this);
            if (window.DialogResult != System.Windows.Forms.DialogResult.OK)
                return;

            long Lang = window.ReturnValue;

            StartLoad(this, null);


            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.CurrentOrder.tb_orderdetail = CurrentOrderDetails;
                    bool result = Res.Tools.Print.Instance.PrintOrder(this.CurrentOrder, Lang);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {

                        }
                        else
                        {

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
                        }), false, Resources.GetRes().GetString("Exception_OperateRequestFaild"));
                    }));
                }
                StopLoad(this, null);
            });
        }


        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;


        public class ToServerServiceReplaceOrder
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "OldOrder")]
            public string OldOrder { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "NewOrder")]
            public string NewOrder { get; set; }


        }


        public class ToServerServiceEditOrder
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "Order")]
            public string Order { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "OrderPays")]
            public string OrderPays { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "Rechecked")]
            public bool Rechecked { get; set; }
        }


        public class ToServerServiceNewOrder
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "Order")]
            public string Order { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "OrderDetails")]
            public string OrderDetails { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "OrderPays")]
            public string OrderPays { get; set; }
        }



        public class ToServerServiceSaveOrderDetail
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "Order")]
            public string Order { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "OrderDetailsAdd")]
            public string OrderDetailsAdd { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "OrderPaysAdd")]
            public string OrderPaysAdd { get; set; }


        }


        public class ToServerServiceDelOrderDetail
        {
            [Newtonsoft.Json.JsonProperty(PropertyName = "Order")]
            public string Order { get; set; }
            [Newtonsoft.Json.JsonProperty(PropertyName = "OrderDetails")]
            public string OrderDetails { get; set; }

        }


    }
}
