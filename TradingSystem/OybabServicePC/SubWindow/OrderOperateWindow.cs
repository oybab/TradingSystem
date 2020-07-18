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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.ServicePC.DialogWindow;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.Reports;
using Oybab.ServicePC.Tools;
using Oybab.Res.View.Models;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class OrderOperateWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 100;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private Order order;
        private List<OrderDetail> resultList = null;
        private List<OrderPay> payList = null;
        private long RoomId;
        private string RoomNo;
        private string RoomStateSession;
        private bool AllowClose = false;
        private bool IsConfirm = false;
        private bool IsRefresh = false;
        private DialogResult dialogResult = DialogResult.None;
        private long StartTimeTemp = 0;


        private List<OrderPay> tempPayList = new List<OrderPay>();

        private TimeSpan TimeLimit = TimeSpan.FromDays(3);

        public OrderOperateWindow(RoomModel model)
        {
            this.RoomId = model.RoomId;
            this.order = model.PayOrder;
            this.resultList = (null == model.PayOrder ? null : model.PayOrder.tb_orderdetail.ToList());
            this.payList = (null == model.PayOrder ? null : model.PayOrder.tb_orderpay.ToList());
            this.RoomStateSession = model.OrderSession;

            InitializeComponent();
            krpdgList.RecalcMagnification();

            new CustomTooltip(this.krpdgList);
            this.krpcmProductName.SetParent(this.krpdgList);

            this.krpcmRequest.SetParent(this.krpdgList, new Func<string, string>((x) =>
            {
                RequestListWindow list = new RequestListWindow(x);
                list.ShowDialog(this);
                if (list.DialogResult == System.Windows.Forms.DialogResult.OK)
                    return list.ReturnValue;
                else
                    return x;
            }));

            this.Text = Resources.GetRes().GetString("OrderDetails");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");


            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            krplPaidPrice.Text = Resources.GetRes().GetString("PaidPrice");
            krplBalancePrice.Text = Resources.GetRes().GetString("BalancePrice");
            krplRoomNo.Text = Resources.GetRes().GetString("RoomNo");

            krplEndTime.Text = Resources.GetRes().GetString("EndTime");

            krpbSave.Text = Resources.GetRes().GetString("Save");
            krpbConfirm.Text = Resources.GetRes().GetString("Confirm");
            krpbRefresh.Text = Resources.GetRes().GetString("Refresh");
            krpbCheckout.Text = Resources.GetRes().GetString("CheckoutOrder");
            krplRemark.Text = Resources.GetRes().GetString("Remark");
            krplLanguage.Text = Resources.GetRes().GetString("Language");
            krplRoomPrice.Text = Resources.GetRes().GetString("RoomPrice");
            krpcbPackage.Text = Resources.GetRes().GetString("Package");


            krpcbLanguage.Items.AddRange(Resources.GetRes().MainLangList.Select(x => x.Value.LangName).ToArray());

            krpbAddByBarcode.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Barcode.png"));
            krpbAddByFastGrid.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.FastGrid.png"));
            krpbAddByBarcode.StateCommon.Back.ImageStyle = krpbAddByFastGrid.StateCommon.Back.ImageStyle = PaletteImageStyle.Stretch;
            krpbAddByBarcode.StateCommon.Back.Draw = krpbAddByFastGrid.StateCommon.Back.Draw = InheritBool.True;

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }

            //增加右键
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            // 请求
            LoadContextMenu(kryptonContextMenuItemRequest, Resources.GetRes().GetString("Request2"), Resources.GetRes().GetString("RequestDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Request.png")), (sender, e) => { ChangeRequest(); });
            // 暂时隐藏. 用下拉框方式显示
            kryptonContextMenuItemRequest.Visible = false;
            // 删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });
            // 查看历史
            LoadContextMenu(kryptonContextMenuItemHistory, Resources.GetRes().GetString("History"), Resources.GetRes().GetString("HistoryDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.RegTime.png")), (sender, e) => { CheckHistory(); });

            if (null == model.PayOrder)
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.NewOrder.ico"));
            else
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ShowOrder.ico"));



            // 是否允许减少商品
            if (Common.GetCommon().IsDecreaseProductCount())
            {
                this.krpcmCount.Minimum = -9999;
            }


            krpcmOrderDetailId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductName.HeaderText = Resources.GetRes().GetString("ProductName");
            krpcmPrice.HeaderText = Resources.GetRes().GetString("UnitPrice");
            krpcmCount.HeaderText = Resources.GetRes().GetString("Count");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");
            krpcmIsPack.HeaderText = Resources.GetRes().GetString("Package");
            krpcmState.HeaderText = Resources.GetRes().GetString("State");
            krpcmRequest.HeaderText = Resources.GetRes().GetString("Request2");
            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");

            ReloadProduct();

            ReloadRequest();

            //初始化
            Init();




            // 扫条码
            Notification.Instance.NotificationBarcodeReader += Instance_NotificationBarcodeReader;
            Notification.Instance.NotificationCardReader += Instance_NotificationCardReader;

            // 订单更新
            Notification.Instance.NotificateSendFromServer += Instance_NotificateSendFromServer;
            Notification.Instance.NotificateSendsFromServer += Instance_NotificateSendsFromServer;

            // 每次打开窗口后先定位到按产品类型购买按钮上
            this.Load += (x, y) =>
            {
                if (!IsLoaded)
                {
                    IsLoaded = true;

                    krpbAddByFastGrid.Select();
                }
            };

        }

      
        private void Instance_NotificateSendFromServer(object obj, long value, object args)
        {
            if (null == args) RefreshSome(new List<long>() { value });
        }
        private void Instance_NotificateSendsFromServer(object obj, List<long> value, object args)
        {
            RefreshSome(value);
        }


        private bool IsLoaded = false;

        /// <summary>
        /// 条码提醒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationBarcodeReader(object sender, string value, object args)
        {
            ScanBarcode(value);
        }



        /// <summary>
        /// 会员卡提醒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationCardReader(object sender, string value, object args)
        {

            if (null != payWindow)
            {
                payWindow.OpenMemberByScanner(value);
                return;
            }
        }



        /// <summary>
        /// 扫条形码
        /// </summary>
        private void ScanBarcode(string code)
        {
            // 获取条码产品

            List<Product> products = Resources.GetRes().Products.Where(x => (x.Barcode == code || (x.IsScales == 1 && code.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).ToList();

            if (products.Count > 1)
            {
                this.BeginInvoke(new Action(() =>
                {
                    FastGridWindow window = new FastGridWindow(true, true, true);
                    window.FastGridBarcodeSearch(products, code);
                    window.ShowDialog(this);

                    HandleFastGrid(window);
                }));
                return;
            }
            else if (products.Count == 0)
            {
                return;
            }

            Product product = products.FirstOrDefault();

            // 如果找到了
            if (null != product)
            {

                string name = "";
                if (Resources.GetRes().MainLangIndex == 0)
                    name = product.ProductName0;
                else if (Resources.GetRes().MainLangIndex == 1)
                    name = product.ProductName1;
                else if (Resources.GetRes().MainLangIndex == 2)
                    name = product.ProductName2;

                // 判断是否已存在!
                this.BeginInvoke(new Action(() =>
                {
                    if (ProductAlreadyExists(name))
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("CurrentProductAlreadyExists"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (product.IsScales == 1)
                        {
                            AddToGrid("*", "-1", product.ProductId, product.Price, double.Parse(code.Substring(7, 2) + "." + code.Substring(9, 3)), Math.Round(product.Price * 1, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));

                        }
                        else
                        {
                            AddToGrid("*", "-1", product.ProductId, product.Price, 1, Math.Round(product.Price * 1, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));

                        }

                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                            krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;

                        Order order;
                        List<OrderDetail> orderDetails;
                        Calc(out orderDetails, out order);
                        CheckChange();

                        if (null != krpdgList.CurrentCell && !krpdgList.IsCurrentCellInEditMode)
                        {
                            krpdgList.Focus();
                            krpdgList.ClearSelection();
                            krpdgList.CurrentCell = krpdgList.Rows[0].Cells["krpcmCount"];
                        }

                    }
                }));
            }
        }



        /// <summary>
        /// 更新部分
        /// </summary>
        private void RefreshSome(List<long> RoomsId)
        {

            if (RoomsId.Contains(this.RoomId) && !IsRefresh)
            {
                IsRefresh = true;

                this.BeginInvoke(new Action(() =>
                {
                    this.krpbRefresh.Visible = true;
                    this.krpbCheckout.Visible = false;
                    this.krpbSave.Visible = false;
                    this.krpbConfirm.Visible = false;
                }));
            }
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

            krplEndTimeChange.Visible = false;
            krplRemarkChange.Visible = false;
            krplPaidPriceChange.Visible = false;
            IsRefresh = false;
            krpbRefresh.Visible = false;

            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();
            RoomNo = room.RoomNo;
            krplRoomNoValue.Text = RoomNo;
            if (room.IsPayByTime == 0)
            {
                krplEndTime.Enabled = false;
                krplEndTimeChange.Enabled = false;
                krplEndTimeValue.Enabled = false;
                btnEndTimeAdd.Enabled = btnEndTimeAdd.Visible = false;
                btnEndTimeSub.Enabled = btnEndTimeSub.Visible = false;
            }
            
            
            if (null == order)
            {
                IsConfirm = false;
                krpbCheckout.Visible = false;
                krpbSave.Visible = false;
                krpbAddByFastGrid.Visible = true;
                krpbAddByBarcode.Visible = true;
                krplTotalPriceValue.Text = "0";
                krplPaidPriceValue.Text = "0";
                krplBalancePriceValue.Text = "0";
                krpcbLanguage.Enabled = true;
                _tempUnlimitedTime = false;

                if (Resources.GetRes().DefaultOrderLang == -1)
                {

                    krpcbLanguage.SelectedItem = Resources.GetRes().MainLang.LangName;

                }
                else
                {
                    krpcbLanguage.SelectedIndex = Resources.GetRes().DefaultOrderLang;
                }

                // 刷新第二屏语言
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex, -1);
                }



                btnEndTimeAdd.Enabled = true;
                btnEndTimeSub.Enabled = true;

                if (Common.GetCommon().IsIncomeTradingManage())
                {
                    btnPaidPriceAdd.Enabled = true;
                    btnPaidPriceSub.Enabled = true;
                }
                else
                {
                    btnPaidPriceAdd.Enabled = false;
                    btnPaidPriceSub.Enabled = false;
                }

                krptbRemark.Enabled = true;
                krpcbPackage.Enabled = true;


                // 新打开的订单直接显示包厢价格
                krplRoomPriceValue.Text = CommonOperates.GetCommonOperates().GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, 0, false, null).ToString();

                Common.GetCommon().OpenPriceMonitor("0");
                // 刷新第二屏幕
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
                }

            }
            else
            {
                krpbSave.Visible = false;
                krpbAddByFastGrid.Visible = true;
                krpbAddByBarcode.Visible = true;
                _tempUnlimitedTime = (order.IsFreeRoomPrice == 2 ? true : false);

                // 是否允许更改语言
                if (Common.GetCommon().IsAllowChangeLanguage())
                {
                    krpcbLanguage.Enabled = true;

                }
                else
                {
                    krpcbLanguage.Enabled = false;
                }

                
                krpcbLanguage.SelectedItem = Resources.GetRes().GetMainLangByLangIndex((int)order.Lang).LangName;
                krpcbPackage.Enabled = false;
                krpcbPackage.Checked = order.IsPack == 1;

                krptbRemark.Text = order.Remark;
                krplTotalPriceValue.Text = Math.Round(order.TotalPrice, 2).ToString();
                krplPaidPriceValue.Text = Math.Round(order.TotalPaidPrice, 2).ToString();


                double balancePrice = order.TotalPaidPrice - order.TotalPrice;
                krplBalancePriceValue.Text = Math.Round(balancePrice, 2).ToString();

                if (balancePrice > 0)
                    krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Blue;
                else if (balancePrice < 0)
                    krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Red;
                else
                    krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Empty;

                // 价格
                Common.GetCommon().OpenPriceMonitor(order.BorrowPrice.ToString());

                if (null == payList)
                {
                    payList = new List<OrderPay>();

                }
                tempPayList = payList.ToList();
                


                krplRoomPriceValue.Text = order.RoomPrice.ToString();

                

                if (null != order.tb_orderdetail && order.tb_orderdetail.Where(x => x.State == 1).Count() > 0)
                {
                    IsConfirm = true;
                    krpbAddByFastGrid.Visible = false;
                    krpbAddByBarcode.Visible = false;
                    krpbCheckout.Visible = false;
                    krpbConfirm.Visible = true;
                    btnEndTimeAdd.Enabled = false;
                    btnEndTimeSub.Enabled = false;
                    btnPaidPriceAdd.Enabled = false;
                    btnPaidPriceSub.Enabled = false;
                    krptbRemark.Enabled = false;

                    lastTotal = Math.Round(this.resultList.Where(x => x.State != 1).Sum(x => x.TotalPrice), 2);
                }
                else
                {
                    if (Common.GetCommon().IsIncomeTradingManage())
                        krpbCheckout.Visible = true;
                    else
                        krpbCheckout.Visible = false;

                    IsConfirm = false;
                    krpbConfirm.Visible = false;
                    btnEndTimeAdd.Enabled = true;
                    btnEndTimeSub.Enabled = true;
                    if (Common.GetCommon().IsIncomeTradingManage())
                    {
                        btnPaidPriceAdd.Enabled = true;
                        btnPaidPriceSub.Enabled = true;
                    }
                    else
                    {
                        btnPaidPriceAdd.Enabled = false;
                        btnPaidPriceSub.Enabled = false;
                    }
                    krptbRemark.Enabled = true;

                    if (null != this.resultList)
                        lastTotal = Math.Round(this.resultList.Sum(x => x.TotalPrice), 2);
                }

                
                    
            }


            try
            {
                if (room.IsPayByTime == 0)
                {
                    
                }

                if (null == order)
                {

                    if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                    {
                        DateTime time = DateTime.Now;
                        StartTimeTemp = long.Parse(time.ToString("yyyyMMddHHmm00"));
                        krplEndTimeValue.Text = time.ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        krplEndTimeValue.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                        krplEndTime.Text = Resources.GetRes().GetString("StartTime");
                    }
                }
                else
                {
                    if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                    {
                            krplEndTimeValue.Text = DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");
                    }
                    else
                    {
                        krplEndTimeValue.Text = DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");
                        krplEndTime.Text = Resources.GetRes().GetString("AddTime");
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            
            ResetPage();

            if (null == resultList)
                resultList = new List<OrderDetail>();//假的,防止翻页报错
            
            OpenPageTo(1, false);




            SetFreeze();

            Order tempOrder;
            List<OrderDetail> details;
            Calc(out details, out tempOrder, true);

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


            //获取数据
            var currentResult = resultList.OrderByDescending(x=>x.OrderDetailId).Skip((CurrentPage - 1) * ListCount).Take(ListCount);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid("", item.OrderDetailId.ToString(), item.ProductId, item.Price, item.Count, item.TotalPrice, item.IsPack, GetOrderDetailsState(item.State), item.Request, item.AddTime.ToString());
            }

            // 待确认改为红色
            for (int i = 0; i < krpdgList.Rows.Count; i++)
            {
                try
                {
                    if (1 == GetOrderDetailsStateNo(krpdgList.Rows[i].Cells["krpcmState"].Value.ToString()))
                        krpdgList.Rows[i].Cells["krpcmState"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmState"].Style.SelectionForeColor = Color.Red;
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
        /// <param name="IsPack"></param>
        /// <param name="State"></param>
        /// <param name="AddTime"></param>
        private void AddToGrid(string editMark, string Id, long productId, double Price, double Count, double TotalPrice, long IsPack, string State, string Request, string AddTime)
        {
            string productName = "";
            string AddTimeStr = "";
            string RequestNames = "";

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

                AddTimeStr = DateTime.ParseExact(AddTime, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                if (productId > 0)
                {
                    if (Resources.GetRes().MainLangIndex == 0)
                        productName = Resources.GetRes().Products.Where(x => x.ProductId == productId).Select(x => x.ProductName0).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 1)
                        productName = Resources.GetRes().Products.Where(x => x.ProductId == productId).Select(x => x.ProductName1).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 2)
                        productName = Resources.GetRes().Products.Where(x => x.ProductId == productId).Select(x => x.ProductName2).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
            {
                krpdgList.Rows.Insert(0, editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), IsPack.ToString(), State, RequestNames, AddTimeStr);
                // 将新增加的价格暂时冻结掉, 等选择产品后才可以知道产品是否允许修改价格
                krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = true;
            }
            else
                krpdgList.Rows.Add(editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), IsPack.ToString(), State, RequestNames, AddTimeStr);
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


            Order order;
            List<OrderDetail> orderDetails;
            Calc(out orderDetails, out order);
            CheckChange();
        }


        /// <summary>
        /// 验证产品和数量,免得选择一个数量不够的产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void krpdgList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((null != krpdgList.EditingControl || null != currentCell) && (e.FormattedValue as string) != temp)//null != krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value && 
            {
                // 先确定当前行是不是选择产品和数量, 如果是那就机选一下该产品剩余数是否够购买当前选择的数,够就继续, 不够则直接返回上一个值
                if (e.ColumnIndex == 2 || e.ColumnIndex == 4)
                {
                    string productName = null;
                    double count = 0;

                    if (e.ColumnIndex == 2)
                        productName = e.FormattedValue.ToString(); //krpdgList.Rows[e.RowIndex].Cells["krpcmProductName"].Value.ToString();
                    else
                         productName = krpdgList.Rows[e.RowIndex].Cells["krpcmProductName"].Value.ToString();

                    if (e.ColumnIndex == 4)
                    {
                        count = Math.Round(double.Parse(e.FormattedValue.ToString()), 3);//long.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmCount"].Value.ToString());
                        if (count == 0)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else
                        count = Math.Round(double.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmCount"].Value.ToString()), 3);

                    Product product = null;
                    if (Resources.GetRes().MainLangIndex == 0)
                        product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 1)
                        product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 2)
                        product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();

                    if (null == product)
                        return;

                    // 根据是否修改价格以及权限解冻, 或者冻结
                    if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = false;
                    }
                    else
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = true;
                    }

                    
                    long Id = long.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmOrderDetailId"].Value.ToString());

                    // 判断是否已存在!
                    if (Id == -1 && e.ColumnIndex == 2 && ProductAlreadyExists(productName, e.RowIndex))
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("CurrentProductAlreadyExists"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Cancel = true;
                        krpdgList.CancelEdit();
                    }

                    if (e.ColumnIndex == 2)
                    {
                        // 将产品原始价格放进去
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].Value = product.Price.ToString();
                    }
                }
            }
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

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1 + ((CurrentPage - 1) * ListCount)).ToString(),
                krpdgList.RowHeadersDefaultCellStyle.Font,
                rectangle,
                krpdgList.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        /// <summary>
        /// 下拉框立即显示(替换为EditingControlShowing)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1 && e.ColumnIndex > -1 && null != (krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex] as KryptonDataGridViewComboBoxCell ))
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
            else
            {
                //hook.RemoveHook();
                Notification.Instance.NotificationBarcodeReader -= Instance_NotificationBarcodeReader;
                Notification.Instance.NotificationCardReader -= Instance_NotificationCardReader;
                Notification.Instance.NotificateSendFromServer -= Instance_NotificateSendFromServer;
                Notification.Instance.NotificateSendsFromServer -= Instance_NotificateSendsFromServer;
                Common.GetCommon().OpenPriceMonitor(null);
                // 刷新第二屏幕
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
                }
                if (dialogResult != System.Windows.Forms.DialogResult.None)
                    DialogResult = dialogResult;
            }
        }

        /// <summary>
        /// 是否为未保存数据而忽略当前的操作
        /// </summary>
        /// <returns></returns>
        private bool IgnoreOperateForSave()
        {
            if (AllowClose)
                return true;

            bool notHandle = CheckChange();

            if (notHandle)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("OrderDetails"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }



        
        /// <summary>
        /// 查看数据
        /// </summary>
        private void Add()
        {
            OpenPageTo(1);
            AddToGrid("*", "-1", 0, 0, 1, 0, 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
            krpdgList.FirstDisplayedScrollingRowIndex = 0;
            CheckChange();

            krpdgList.ClearSelection();
            krpdgList.CurrentCell = krpdgList.Rows[0].Cells["krpcmProductName"];
            
        }



        /// <summary>
        /// 删除数据
        /// </summary>
        private void Delete()
        {
            long Id = -1;
            try
            {
                //确认删除
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("SureDelete"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm != DialogResult.Yes)
                    return;

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrderDetailId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);

                    Order order;
                    List<OrderDetail> orderDetails;
                    Calc(out orderDetails, out order);
                    CheckChange();
                }
                else
                {
                    if (krpbSave.Visible)
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveBeforeDelete"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 不允许权限不够的人删除
                    if (Id != -1 && !Common.GetCommon().IsDeleteProduct())
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("PermissionDenied"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                        
                    }

                    Order order;
                    List<OrderDetail> details;

                    try
                    {
                        Calc(out details, out order, false, false, true, true, Id);
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("DeleteFailt"));
                        return;
                    }

                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {


                            OrderDetail old = resultList.Where(x => x.OrderDetailId == Id).FirstOrDefault();
                            old.State = 3;
                            List<OrderDetail> orderDetails = new List<OrderDetail>() { old };

                           
                            string newRoomSession = null;
                            long UpdateTime;
                            List<OrderDetail> newOrderDetails;
                            
                            
                            ResultModel result = OperatesService.GetOperates().ServiceDelOrderDetail(order, orderDetails, RoomStateSession, out newRoomSession, out newOrderDetails, out UpdateTime);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    RoomModel model = Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder && x.PayOrder.OrderId == order.OrderId).FirstOrDefault();
                                    OrderDetail oldModel = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == Id).FirstOrDefault();

                                    oldModel.State = 3;
                                    krpdgList.SelectedRows[0].ReadOnly = true;
                                    krpdgList.SelectedRows[0].Cells["krpcmState"].Value = GetOrderDetailsState(3);


                                    for (int j = 0; j < krpdgList.SelectedRows[0].Cells.Count; j++)
                                    {
                                        krpdgList.SelectedRows[0].Cells[j].Style.ForeColor = krpdgList.SelectedRows[0].Cells[j].Style.SelectionForeColor = Color.Gray;
                                    }


                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, newOrderDetails.FirstOrDefault());


                                    ICollection<OrderDetail> detailsOld = null;
                                    ICollection<OrderPay> paysOld = null;
                                    if (null != model.PayOrder)
                                        detailsOld = model.PayOrder.tb_orderdetail;
                                    if (null != model.PayOrder)
                                        paysOld = model.PayOrder.tb_orderpay;

                                    model.PayOrder = this.order = order;
                                    if (null != detailsOld)
                                        model.PayOrder.tb_orderdetail = detailsOld;
                                    if (null != paysOld)
                                        model.PayOrder.tb_orderpay = paysOld;


                                    model.OrderSession = this.RoomStateSession = newRoomSession;


                                    foreach (var item in orderDetails)
                                    {
                                        // 删除部分增加现有数量
                                        Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                        if (product.IsBindCount == 1)
                                        {
                                            product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);
                                            product.UpdateTime = UpdateTime;

                                            Notification.Instance.ActionProduct(null, product, 2);
                                         }
                                    }

                                    Order order2;
                                    List<OrderDetail> orderDetails2;
                                    Calc(out orderDetails2, out order2);
                                    CheckChange();

                                    this.dialogResult = System.Windows.Forms.DialogResult.OK;

                                    Init();
                                }
                                else
                                {
                                    if (result.IsRefreshSessionModel)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        this.dialogResult = System.Windows.Forms.DialogResult.Retry;
                                        this.Close();
                                    }
                                    else if (result.IsSessionModelSameTimeOperate)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenWaitRetry"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                }), false, Resources.GetRes().GetString("DeleteFailt"));
                            }));
                        }
                        StopLoad(this, null);
                    });
                }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, Resources.GetRes().GetString("DeleteFailt"));
                return;
            }
        }


        private DataGridViewCell currentCell;
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
                if (e.RowIndex == -1 || null == krpdgList.Rows[e.RowIndex] || krpdgList.RowCount == 0)
                {
                    kryptonContextMenuItemDelete.Enabled = false;
                    kryptonContextMenuItemRequest.Enabled = false;
                    if (this.order != null)
                        kryptonContextMenuItemHistory.Enabled = true;

                }
                else
                {
                    kryptonContextMenuItemDelete.Enabled = false;
                    // 没行选中时判断一下, 是否单击了空的地方, 如果是则只显示新增之类的
                    if (krpdgList.Rows[e.RowIndex].Selected == false)
                    {
                        DataGridView.HitTestInfo hit = krpdgList.HitTest(e.X, e.Y);
                        if (hit.Type == DataGridViewHitTestType.None)
                        {
                            return;
                        }
                        else
                        {
                            if (krpdgList.IsCurrentCellInEditMode)
                            {
                                currentCell = krpdgList.CurrentCell;
                                try
                                {
                                    krpdgList.EndEdit();
                                    krpdgList.CurrentCell = null;
                                    krpdgList.CurrentCell = currentCell;
                                }
                                catch
                                {
                                    DataGridViewCell tempCell = currentCell;
                                    krpdgList.CurrentCell = currentCell = null;
                                    krpdgList.CurrentCell = tempCell;
                                    krpdgList.CurrentCell.Value = temp;
                                    krpdgList.BeginEdit(true);  
                                    return;
                                }
                                currentCell = null;
                                krpdgList.ClearSelection();
                            }

                            krpdgList.Rows[e.RowIndex].Selected = true;
                            if (krpdgList.SelectedRows.Count == 0)
                            {
                               
                                return;
                            }
                                
                        }
                    }

                    if (krpdgList.SelectedRows[0].Cells["krpcmState"].Value.ToString() != GetOrderDetailsState(3))
                        kryptonContextMenuItemDelete.Enabled = true;

                   
                    
                }

                // 确认的时候不能新增
                if (!IsConfirm)
                    kryptonContextMenuItemAdd.Enabled = true;
                //显示
                krpContextMenu.Show(krpdgList.RectangleToScreen(krpdgList.ClientRectangle),
                     KryptonContextMenuPositionH.Left, KryptonContextMenuPositionV.Top);
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
                
                //任何情况下都可以增加
                if (e.KeyCode == Keys.N)
                {
                    // 确认的时候不能新增
                    if (!krpbConfirm.Visible)
                        Add();
                }

                //当前选中的可以删除
                if (e.KeyCode == Keys.D)
                {
                    if (krpdgList.SelectedRows.Count > 0)
                    {
                      
                        if (krpdgList.SelectedRows[0].Cells["krpcmState"].Value.ToString() != GetOrderDetailsState(3))
                            Delete();
                    }
                }
                // 打印
                if (e.KeyCode == Keys.P)
                {
                    PrintOrder();
                }
                // 查看历史
                if (e.KeyCode == Keys.H && this.order != null)
                {
                    CheckHistory();
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
            else if (e.KeyCode == Keys.F1 && krpdgList.Rows.Count > 0 && null != krpdgList.CurrentCell && krpdgList.CurrentCell.ColumnIndex == 8 && !krpdgList.CurrentCell.IsInEditMode)
            {
                krpdgList.BeginEdit(true);

                SendKeys.SendWait("{F1}");
            }
            else if (e.KeyCode == Keys.F12 && e.Alt)
            {
                CheckHistory();
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
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;

        /// <summary>
        /// 加载产品
        /// </summary>
        private void ReloadProduct()
        {
            krpcmProductName.SetValues(null, false);

            if (Resources.GetRes().Products.Count > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName0).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName1).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName2).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
            }
        }


        private double lastTotal = 0;
        private double lastOriginalTotalPrice = 0;
        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="details"></param>
        /// <param name="order"></param>
        /// <param name="IgnoreError"></param>
        /// <param name="OnlyTotal"></param>
        private void Calc(out List<OrderDetail> details, out Order order, bool IgnoreError = true, bool OnlyTotal = false, bool IgnoreNotConfirm = true, bool IgnoreCanceld = true, long IgnoreCancelId = -999)
        {
            bool IsError = false;
            details = new List<OrderDetail>();
            List<OrderDetail>  detailsAll = new List<OrderDetail>();
            order = new Order();

           if (!OnlyTotal)
           {
               for (int i = krpdgList.Rows.Count - 1; i >= 0; i--)
               {

                   OrderDetail orderDetails = new OrderDetail();

                       try
                       {
                           string productName = krpdgList.Rows[i].Cells["krpcmProductName"].Value.ToString();
                           Product product = null;
                           if (Resources.GetRes().MainLangIndex == 0)
                               product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                           else if (Resources.GetRes().MainLangIndex == 1)
                               product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                           else if (Resources.GetRes().MainLangIndex == 2)
                               product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();

                           orderDetails.ProductId = product.ProductId;

                           orderDetails.IsPack = long.Parse(krpdgList.Rows[i].Cells["krpcmIsPack"].Value.ToString());
                           orderDetails.Count = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmCount"].Value.ToString()), 3);

                           orderDetails.OrderDetailId = long.Parse(krpdgList.Rows[i].Cells["krpcmOrderDetailId"].Value.ToString());

                           if (orderDetails.OrderDetailId == -1)
                           {

                               if (!string.IsNullOrWhiteSpace(krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString()))
                               {

                                   List<Request> requests = new List<Request>();
                                   if (Resources.GetRes().MainLangIndex == 0)
                                       requests = Resources.GetRes().Requests.Where(x => krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString().Split('&').Contains(x.RequestName0)).ToList<Request>();
                                   else if (Resources.GetRes().MainLangIndex == 1)
                                       requests = Resources.GetRes().Requests.Where(x => krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString().Split('&').Contains(x.RequestName1)).ToList<Request>();
                                   else if (Resources.GetRes().MainLangIndex == 2)
                                       requests = Resources.GetRes().Requests.Where(x => krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString().Split('&').Contains(x.RequestName2)).ToList<Request>();


                                   
                                   if (requests.Count > 0)
                                   {
                                       requests = requests.Distinct().ToList();

                                       orderDetails.Request = string.Join(",", requests.Select(x => x.RequestId.ToString()));

                                   }
                                   else
                                   {
                                       if (!string.IsNullOrWhiteSpace(krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString()))
                                       {
                                           throw new Exception(string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("RequestName")));
                                       }

                                   }
                               }

                           }
                           else
                           {
                               orderDetails.Request = resultList.Where(x => x.OrderDetailId == orderDetails.OrderDetailId).FirstOrDefault().Request;

                           }

                           orderDetails.State = GetOrderDetailsStateNo(krpdgList.Rows[i].Cells["krpcmState"].Value.ToString());

                           krpdgList.Rows[i].Cells["krpcmPrice"].Value = orderDetails.Price = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmPrice"].Value.ToString()), 2);
                           krpdgList.Rows[i].Cells["krpcmTotalPrice"].Value = orderDetails.TotalPrice = Math.Round(orderDetails.Price * orderDetails.Count, 2);
                           orderDetails.OriginalTotalPrice = Math.Round(product.Price * orderDetails.Count, 2);


                        orderDetails.TotalCostPrice = Math.Round(product.CostPrice * orderDetails.Count, 2);
                        if (product.CostPrice == 0 && null != product.ProductParentId)
                        {
                            Product parentProduct = Resources.GetRes().Products.FirstOrDefault(x => x.ProductId == product.ProductParentId);

                            if (null != parentProduct)
                            {
                                double price = Math.Round(parentProduct.CostPrice / product.ProductParentCount, 2);
                                orderDetails.TotalCostPrice = Math.Round(price * orderDetails.Count, 2);
                            }
                        }
                       

                        if (krpdgList.Rows[i].Cells["krpcmEdit"].Value.Equals("*"))
                            details.Add(orderDetails);
                           detailsAll.Add(orderDetails);

                       }
                    catch
#if DEBUG
                       (Exception ex)
#endif
                        {
                           krpdgList.Rows[i].Cells["krpcmPrice"].Value = "?";
                           krpdgList.Rows[i].Cells["krpcmTotalPrice"].Value = "?";

                           // 产品名空时这种错误很常见, 所以暂时去掉
#if DEBUG
                        ExceptionPro.ExpLog(ex, null, true);
#endif
                           IsError = true;

                           if (!IgnoreError)
                           {
                               krpdgList.Rows[i].Selected = true;
                               throw new OybabException(string.Format(Resources.GetRes().GetString("LineDataError"), i + 1));
                           }
                           else
                               break;
                       }

           
               }

               if (IsError)
                   return;
           }
                
                
           
                IEnumerable<OrderDetail> totalDetails = detailsAll;

                if (IgnoreNotConfirm)
                    totalDetails = totalDetails.Where(x => x.State != 1);
                if (IgnoreCanceld)
                    totalDetails = totalDetails.Where(x => x.State != 3);

                lastTotal = Math.Round(totalDetails.Sum(x => x.TotalPrice), 2);
                lastOriginalTotalPrice = Math.Round(totalDetails.Sum(x => x.OriginalTotalPrice), 2);

                if (IgnoreCancelId != -999 && totalDetails.Where(x => x.OrderDetailId == IgnoreCancelId).Count() > 0)
                {
                    lastTotal = Math.Round(lastTotal - totalDetails.Where(x => x.OrderDetailId == IgnoreCancelId).FirstOrDefault().TotalPrice, 2);
                    lastOriginalTotalPrice = Math.Round(lastOriginalTotalPrice - totalDetails.Where(x => x.OrderDetailId == IgnoreCancelId).FirstOrDefault().OriginalTotalPrice, 2);
                }




            if (krplEndTimeValue.Enabled)
                order.EndTime = long.Parse(DateTime.ParseExact(krplEndTimeValue.Text.ToString(), "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));



            order.RoomId = this.RoomId;
            if (this.order != null)
            {
                order.OrderId = this.order.OrderId;
                
                order.AdminId = this.order.AdminId;
                order.DeviceId = this.order.DeviceId;
                order.AddTime = this.order.AddTime;
                order.UpdateTime = this.order.UpdateTime;
                order.RoomPriceCalcTime = this.order.RoomPriceCalcTime;
                order.Request = this.order.Request;
                order.PrintCount = this.order.PrintCount;
                order.Mode = this.order.Mode;
                order.MemberPaidPrice = this.order.MemberPaidPrice;
                order.MemberId = this.order.MemberId;
                order.IsAutoPay = this.order.IsAutoPay;
                order.BorrowPrice = this.order.BorrowPrice;
                order.Lang = this.order.Lang;
                order.IsPayByTime = this.order.IsPayByTime;
                order.IsFreeRoomPrice = this.order.IsFreeRoomPrice;
                order.IsPack = this.order.IsPack;
                order.StartTime = this.order.StartTime;
                order.State = this.order.State;
                order.ReCheckedCount = this.order.ReCheckedCount;


                // 是否允许更改语言
                if (Common.GetCommon().IsAllowChangeLanguage())
                {
                    order.Lang = Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex;
                }

            }
            else
            {
                order.Lang = Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex;
                order.IsPack = krpcbPackage.Checked ? 1 : 0;
                order.StartTime = StartTimeTemp;
                order.IsFreeRoomPrice = (_tempUnlimitedTime ? 2 : 0);
            }


            
            

            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();

            int totalMinutes = 0;
            bool IsSubTime = false;

            if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
            {
                // 直接计算时间
                if (null == this.order)
                    totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                else
                {
                    // 如果是时间少了, 则还是老方法计算
                    if (DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.ParseExact(order.RoomPriceCalcTime.ToString(), "yyyyMMddHHmmss", null))
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                        IsSubTime = true;
                    }
                    // 如果不是, 则在上次的时间加上新时间价格
                    else{
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.RoomPriceCalcTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                    }
                        
                }
            }



            krplRoomPriceValue.Text = (order.RoomPrice = CommonOperates.GetCommonOperates().GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, totalMinutes, IsSubTime, order.EndTime)).ToString();


            krplTotalPriceValue.Text = (order.TotalPrice = Math.Round(lastTotal + order.RoomPrice, 2)).ToString();
            order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice + order.RoomPrice, 2);

            // 注入雅座消费类型
            order.IsPayByTime = room.IsPayByTime;

            // 如果超出最低消费,则清空雅座费
            if ((_tempUnlimitedTime) || (room.FreeRoomPriceLimit > 0 && lastTotal >= room.FreeRoomPriceLimit))
            {
               
                
                    krplRoomPriceValue.Text = (order.RoomPrice = 0).ToString();
                order.IsFreeRoomPrice = 1;


                if (_tempUnlimitedTime)
                {
                    krplRoomPriceValue.Text = Resources.GetRes().GetString("UnlimitedTime");
                    order.IsFreeRoomPrice = 2;
                }

                    krplTotalPriceValue.Text = (order.TotalPrice = Math.Round(lastTotal, 2)).ToString();
                    order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice, 2);
                    
                
            }
            else
            {
                // 如果之前是免去了包厢费,现在需要去掉, 则重新计算房费
                if (order.IsFreeRoomPrice == 1)
                {
                    if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                    }
                    krplRoomPriceValue.Text = (order.RoomPrice = CommonOperates.GetCommonOperates().GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, totalMinutes, IsSubTime, order.EndTime, true)).ToString();


                    krplTotalPriceValue.Text = (order.TotalPrice = Math.Round(lastTotal + order.RoomPrice, 2)).ToString();
                    order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice + order.RoomPrice, 2);
                }

                order.IsFreeRoomPrice = 0;
            }
            


            double balancePrice = 0;


            order.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            order.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);



            order.TotalPaidPrice = Math.Round(order.MemberPaidPrice + order.PaidPrice, 2);

            krplPaidPriceValue.Text = order.TotalPaidPrice.ToString();


            krplBalancePriceValue.Text = (balancePrice = Math.Round(order.TotalPaidPrice - order.TotalPrice, 2)).ToString();


            if (balancePrice > 0)
            {
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Blue;
                order.KeepPrice = balancePrice;
                order.BorrowPrice = 0;
            }

            else if (balancePrice < 0)
            {
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Red;
                order.BorrowPrice = balancePrice;
                order.KeepPrice = 0;
            }

            else if (balancePrice == 0)
            {
                krplBalancePriceValue.StateCommon.ShortText.Color1 = Color.Empty;
                order.BorrowPrice = 0;
                order.KeepPrice = 0;
            }



            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(order.BorrowPrice.ToString());

            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                RoomInfoModel roomInfo = new RoomInfoModel();

                // 总时间算出来
                if (null != this.order) {
                    if (this.order.StartTime != null && this.order.EndTime != null)
                    {
                        TimeSpan total = (DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null) - DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null));
                        TimeSpan balance = (DateTime.Now - DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null));


                        if (room.IsPayByTime == 1)
                            roomInfo.TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                        else if (room.IsPayByTime == 2)
                            roomInfo.TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                    }
                    else
                    {
                        TimeSpan total = (DateTime.Now - DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", null));

                        if (room.IsPayByTime == 2)
                            roomInfo.TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);
                        else
                            roomInfo.TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                    }
                }
                   
                roomInfo.RoomNo = RoomNo;
                roomInfo.RoomPrice = order.RoomPrice;


                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(order, this.order, details, roomInfo, false, true));
            }

            if (OnlyTotal)
                return;
            


            if (!string.IsNullOrWhiteSpace(krptbRemark.Text))
                order.Remark = krptbRemark.Text;

            if (room.IsPayByTime == 0)
            {
                order.EndTime = null;
                order.StartTime = null;
            }
            
        }



        /// <summary>
        /// 打开请求页面
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        private void ChangeRequest()
        {
            int rowIndex = krpdgList.SelectedRows[0].Index;
            int columnIndex = krpdgList.SelectedRows[0].Cells["krpcmRequest"].ColumnIndex;
            string requests = krpdgList.SelectedRows[0].Cells["krpcmRequest"].Value.ToString(); 
            RequestListWindow list = new RequestListWindow(requests);
            list.ShowDialog(this);
            if (list.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                krpdgList.Rows[rowIndex].Cells[columnIndex].Value = list.ReturnValue;
                krpdgList.Rows[rowIndex].Cells[0].Value = "*";
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {

            Order order;
            List<OrderDetail> details;

            bool IgnoreNotConfirm = true;

            if (IsConfirm)
                IgnoreNotConfirm = false;

            try
            {
                Calc(out details, out order, false, false, IgnoreNotConfirm);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, Resources.GetRes().GetString("SaveFailt"));
                return;
            }

            

            if (null == this.order && Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder).Count() >= Resources.GetRes().RoomCount)
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_RoomCountOutOfLimit"), Common.GetCommon().GetFormat()), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ResultModel result = null;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
            string successMsgName = "";
            string faildMsgName = "";
            try
            {


                List<OrderDetail> resultDetails = null;
                List<OrderPay> resultPays = null;
                string newRoomSession = null;

                // 如果不是新订单, 先分别获取一下用于保存和确认的信息
                List<OrderDetail> orderDetailsAdd = new List<OrderDetail>();
                List<OrderDetail> orderDetailsEdit = new List<OrderDetail>();
                List<OrderDetail> orderDetailsConfirm = new List<OrderDetail>();
                if (null != this.order)
                {
                    foreach (var item in details)
                    {
                        OrderDetail odt = new OrderDetail();

                        odt.Price = item.Price;
                        odt.ProductId = item.ProductId;
                        odt.Count = item.Count;
                        odt.State = item.State;
                        odt.OrderDetailId = item.OrderDetailId;
                        odt.TotalPrice = item.TotalPrice;
                        odt.OriginalTotalPrice = item.OriginalTotalPrice;
                        odt.IsPack = item.IsPack;
                        odt.Request = item.Request;

                        OrderDetail old = resultList.Where(x => x.OrderDetailId == odt.OrderDetailId).FirstOrDefault();
                        if (null != old)
                        {
                            odt.AddTime = old.AddTime;
                            odt.OrderId = old.OrderId;
                            odt.AdminId = old.AdminId;
                            odt.DeviceId = old.DeviceId;

                            odt.Mode = old.Mode;
                            odt.PrintCount = old.PrintCount;
                            odt.Request = old.Request;
                            odt.UpdateTime = item.UpdateTime;

                            item.ConfirmAdminId = item.ConfirmAdminId;
                            item.ConfirmDeviceId = item.ConfirmDeviceId;
                            item.ConfirmTime = item.ConfirmTime;
                            item.Remark = item.Remark;
                        }
                        else if (null != this.order)
                        {
                            odt.OrderId = this.order.OrderId;
                        }

                        if (odt.State == 1)
                        {
                            odt.State = 2;
                            orderDetailsConfirm.Add(odt);
                        }
                        else
                        {
                            if (odt.OrderDetailId == -1)
                                orderDetailsAdd.Add(odt);
                            else
                                orderDetailsEdit.Add(odt);
                        }
                    }
                }

                    // 如果是开张
                    if (null == this.order)
                    {

                       
                        faildMsgName = successMsgName = Resources.GetRes().GetString("Save");


                        long UpdateTime;

                        result = OperatesService.GetOperates().ServiceAddOrder(order, details, tempPayList.Where(x => x.AddTime == 0).ToList(), RoomStateSession, out resultDetails, out resultPays, out newRoomSession, out UpdateTime);

                        if (result.Result)
                        {
                            Resources.GetRes().DefaultOrderLang = Resources.GetRes().GetMainLangByLangIndex((int)order.Lang).MainLangIndex;
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;
                            model.PayOrder.tb_orderdetail = this.resultList = resultDetails;
                            model.PayOrder.tb_orderpay = this.payList = resultPays;
                            tempPayList = payList.ToList();
                            model.OrderSession = this.RoomStateSession = newRoomSession;

                            // 新增部分去掉现有数量
                            foreach (var item in resultDetails)
                            {
                                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {

                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                            if (null != productParent && productParent.IsBindCount == 1)
                                            {
                                                double ParentRemove = 0;
                                                double ProductAdd = 0;


                                                double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                                ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                                ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 


                                                // 从父级中去掉
                                                productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                productParent.UpdateTime = UpdateTime;


                                                // 给产品增加零的
                                                product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                            }
                                        }
                                    }


                                    product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                    product.UpdateTime = UpdateTime;

                                    Notification.Instance.ActionProduct(null, product, 2);
                                }
                            }

                            foreach (var item in resultPays)
                            {
                                if (null != item.MemberId)
                                {
                                    Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                    item.MemberId = item.tb_member.MemberId;
                                }
                            }

                            Print.Instance.PrintOrderAfterBuy(order, resultDetails, oldOrder);
                        }

                    }
                    // 如果是确认
                    else if (IsConfirm)
                    {
                        faildMsgName = successMsgName = Resources.GetRes().GetString("Confirm");




                        List<OrderDetail> orderDetailsAddResult;
                        List<OrderPay> orderPaysAddResult;
                        List<OrderDetail> orderDetailsEditResult;
                        List<OrderDetail> orderDetailsConfirmResult;


                        long UpdateTime;

                        result = OperatesService.GetOperates().ServiceSaveOrderDetail(order, null, null, null, orderDetailsConfirm, RoomStateSession, out orderDetailsAddResult, out orderPaysAddResult, out orderDetailsEditResult, out orderDetailsConfirmResult, out newRoomSession, out UpdateTime);

                        // 根据待确认的编号来修改它的状态信息(里面的就算了, 反正马上关闭窗口)
                        if (result.Result)
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

                            ICollection<OrderDetail> detailsOld = null;
                            ICollection<OrderPay> paysOld = null;
                            if (null != model.PayOrder)
                                detailsOld = model.PayOrder.tb_orderdetail;
                            if (null != model.PayOrder)
                                paysOld = model.PayOrder.tb_orderpay;

                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;

                            if (null != detailsOld)
                                model.PayOrder.tb_orderdetail = detailsOld;
                            if (null != paysOld)
                                model.PayOrder.tb_orderpay = paysOld;


                            foreach (var item in orderDetailsConfirmResult)
                            {
                                OrderDetail OrderDetails = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId && x.State == 1).FirstOrDefault();
                                if (null != OrderDetails)
                                {
                                    OrderDetails.State = 2;

                                    // 确认部分去掉现有数量
                                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                    if (product.IsBindCount == 1)
                                    {

                                        if (product.BalanceCount < item.Count)
                                        {
                                            // 如果有父级
                                            if (null != product.ProductParentId)
                                            {
                                                Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                                if (null != productParent && productParent.IsBindCount == 1)
                                                {
                                                    double ParentRemove = 0;
                                                    double ProductAdd = 0;


                                                    double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3);
                                                    ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                    ParentRemove = (int)Math.Ceiling(ParentRemove);

                                                    ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 


                                                    // 从父级中去掉
                                                    productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                    productParent.UpdateTime = UpdateTime;


                                                    // 给产品增加零的
                                                    product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                                }
                                            }
                                        }


                                        product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                        product.UpdateTime = UpdateTime;

                                        Notification.Instance.ActionProduct(null, product, 2);
                                    }
                                }
                            }
                            model.OrderSession = this.RoomStateSession = newRoomSession;
                            this.resultList = model.PayOrder.tb_orderdetail.ToList();
                            this.payList = model.PayOrder.tb_orderpay.ToList();
                            tempPayList = payList.ToList();

                            Print.Instance.PrintOrderAfterBuy(order, orderDetailsConfirmResult, oldOrder);
                        }
                    }
                    // 如果是修改
                    else if (krpbSave.Visible)
                    {
                        faildMsgName = successMsgName = Resources.GetRes().GetString("Save");


                     
                        List<OrderDetail> orderDetailsAddResult;
                        List<OrderPay> orderPaysAddResult;
                        List<OrderDetail> orderDetailsEditResult;
                        List<OrderDetail> orderDetailsConfirmResult;


                        long UpdateTime;

                        result = OperatesService.GetOperates().ServiceSaveOrderDetail(order, orderDetailsAdd, tempPayList.Where(x => x.AddTime == 0).Select(x => {x.OrderId = order.OrderId; return x; }).ToList(), orderDetailsEdit, null, RoomStateSession, out orderDetailsAddResult, out orderPaysAddResult, out orderDetailsEditResult, out orderDetailsConfirmResult, out newRoomSession, out UpdateTime);

                        // 根据待确认的编号来修改它的状态信息(里面的就算了, 反正马上关闭窗口)
                        if (result.Result)
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

                            ICollection<OrderDetail> detailsOld = null;
                            ICollection<OrderPay> PaysOld = null;
                            if (null != model.PayOrder)
                                detailsOld = model.PayOrder.tb_orderdetail;
                            if (null != model.PayOrder)
                                PaysOld = model.PayOrder.tb_orderpay;

                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;

                            if (null != detailsOld)
                                model.PayOrder.tb_orderdetail = detailsOld;
                            if (null != PaysOld)
                                model.PayOrder.tb_orderpay = PaysOld;


                            foreach (var item in orderDetailsAddResult)
                            {
                                // 新增部分去掉现有数量
                                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {

                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                            if (null != productParent && productParent.IsBindCount == 1)
                                            {
                                                double ParentRemove = 0;
                                                double ProductAdd = 0;


                                                double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                                ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                ParentRemove = (int)Math.Ceiling(ParentRemove);

                                                ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 


                                                // 从父级中去掉
                                                productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                productParent.UpdateTime = UpdateTime;


                                                // 给产品增加零的
                                                product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                            }
                                        }
                                    }


                                    product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                    product.UpdateTime = UpdateTime;

                                    Notification.Instance.ActionProduct(null, product, 2);
                                }

                                model.PayOrder.tb_orderdetail.Add(item);
                            }
                            foreach (var item in orderDetailsEditResult)
                            {
                                OrderDetail OrderDetails = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId).FirstOrDefault();
                                if (null != OrderDetails)
                                {
                                    // 编辑的部分先把数据修正
                                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                    if (product.IsBindCount == 1)
                                    {

                                        if (product.BalanceCount < (-OrderDetails.Count + item.Count))
                                        {
                                            // 如果有父级
                                            if (null != product.ProductParentId)
                                            {
                                                Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                                if (null != productParent && productParent.IsBindCount == 1)
                                                {
                                                    double ParentRemove = 0;
                                                    double ProductAdd = 0;


                                                    double NeedChangeFromParent = Math.Round((-OrderDetails.Count + item.Count) - product.BalanceCount, 3);
                                                    ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3);
                                                    ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                                    ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 


                                                    // 从父级中去掉
                                                    productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                    productParent.UpdateTime = UpdateTime;


                                                    // 给产品增加零的
                                                    product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                                }
                                            }
                                        }


                                        product.BalanceCount = Math.Round(product.BalanceCount - (-OrderDetails.Count + item.Count), 3);
                                        product.UpdateTime = UpdateTime;

                                        Notification.Instance.ActionProduct(null, product, 2);
                                    }


                                    OrderDetails.Count = item.Count;
                                    OrderDetails.Price = item.Price;
                                    OrderDetails.TotalPrice = item.TotalPrice;
                                    OrderDetails.OriginalTotalPrice = item.OriginalTotalPrice;
                                    OrderDetails.UpdateTime = item.UpdateTime;
                                }
                            }
                            model.OrderSession = this.RoomStateSession = newRoomSession;
                            this.resultList = model.PayOrder.tb_orderdetail.ToList();


                            foreach (var item in orderPaysAddResult)
                            {
                                model.PayOrder.tb_orderpay.Add(item);


                                if (null != item.MemberId)
                                {
                                    Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                    item.MemberId = item.tb_member.MemberId;
                                }

                            }
                            this.payList = model.PayOrder.tb_orderpay.ToList();
                            tempPayList = payList.ToList();



                            Print.Instance.PrintOrderAfterBuy(order, orderDetailsAddResult, oldOrder);
                        }
                    } 
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), successMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.dialogResult = System.Windows.Forms.DialogResult.OK;

                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.dialogResult = System.Windows.Forms.DialogResult.Retry;
                                AllowClose = true;
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenWaitRetry"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), faildMsgName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }


                        if (AllowClose)
                        {
                            this.Close();
                        }
                        else if (null != result && result.Result)
                        {
                            Init();
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
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), faildMsgName));
                    }));
                }
                StopLoad(this, null);

            });
        }


        /// <summary>
        /// 确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbConfirm_Click(object sender, EventArgs e)
        {
            krpbSave_Click(null, null);
        }

        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbCheckout_Click(object sender, EventArgs e)
        {

            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

            if (null == model.PayOrder)
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.dialogResult = System.Windows.Forms.DialogResult.Retry;
                this.Close();
                return;
            }


            // 如果是结账
            CheckoutWindow window = new CheckoutWindow(model);
            window.StartLoad += (obj, e2) =>
            {
                StartLoad(obj, null);
            };
            window.StopLoad += (obj, e2) =>
            {
                StopLoad(obj, null);
            };
            DialogResult dialogResult = window.ShowDialog(this);
            // 如果成功, 代表有改动, 重修刷新一下当前数据
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                this.dialogResult = System.Windows.Forms.DialogResult.OK;
                AllowClose = true;
            }
            // 重试代表订单数据不是最新的, 重新获取
            else if (dialogResult == System.Windows.Forms.DialogResult.Retry)
            {
                this.dialogResult = System.Windows.Forms.DialogResult.Retry;
                AllowClose = true;
            }

            if (AllowClose)
            {
                this.Close();
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
        /// 检查修改
        /// </summary>
        private bool CheckChange()
        {
            bool IsChange = false;

            foreach (DataGridViewRow row in krpdgList.Rows)
            {
                if (row.Cells["krpcmEdit"].Value.Equals("*"))
                {
                    IsChange = true;
                    break;
                }
            }

            if (!IsChange)
            {
                if (krplRemarkChange.Visible || krplPaidPriceChange.Visible || krplEndTimeChange.Visible)
                    IsChange = true;
            }

            if (IsChange)
            {
                if (!IsConfirm)
                {
                    krpbSave.Visible = true;
                    krpbAddByFastGrid.Visible = true;
                    krpbAddByBarcode.Visible = true;
                    krpbCheckout.Visible = false; 
                }
            }
            else
            {
                if (!IsConfirm)
                {
                    if (null == this.order)
                    {
                        krpbCheckout.Visible = false;
                        krpbSave.Visible = true;
                    }
                    else
                    {
                        if (Common.GetCommon().IsIncomeTradingManage())
                            krpbCheckout.Visible = true;
                        else
                            krpbCheckout.Visible = false;

                        krpbSave.Visible = false;
                    }
                    krpbAddByFastGrid.Visible = true;
                    krpbAddByBarcode.Visible = true;
                }
            }


            return IsChange;
        }

        /// <summary>
        /// 增加付款价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaidPriceAdd_Click(object sender, EventArgs e)
        {
            ChangePaidPrice("+");
        }

        /// <summary>
        /// 减少付款价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaidPriceSub_Click(object sender, EventArgs e)
        {
            ChangePaidPrice("-");
        }

        private PriceCommonChangeWindow payWindow = null;
        /// <summary>
        /// 修改付款价格
        /// </summary>
        /// <param name="mark"></param>
        private void ChangePaidPrice(string mark)
        {
            double totalPrice = double.Parse(krplTotalPriceValue.Text);

            List<CommonPayModel> oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();

            PriceCommonChangeWindow window = new PriceCommonChangeWindow(mark, totalPrice, oldList.ToList());
            payWindow = window;
            window.StartLoad += (x, y) =>
            {
                this.StartLoad(x, y);
            };
            window.StopLoad += (x, y) =>
            {
                this.StopLoad(x, y);
            };
            if (window.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // 如果跟订单上次保存的不一样,就提示未保存提示
                if (oldList.All(window.PayModel.Contains) && oldList.Count == window.PayModel.Count)
                {
                    //krplPaidPriceChange.Visible = false;
                }
                else
                {
                    krplPaidPriceChange.Visible = true;
                    tempPayList = window.PayModel.Select(x => x.GetOrderPay()).ToList();
                }

               
                if (krplPaidPriceChange.Visible)
                {
                    Order tempOrder;
                    List<OrderDetail> details;
                    Calc(out details, out tempOrder, true);
                    CheckChange();
                }
            }
            payWindow = null;
        }

        /// <summary>
        /// 增加结束时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEndTimeAdd_Click(object sender, EventArgs e)
        {
            ChangeEndTime("+");
        }

        /// <summary>
        /// 减少结束时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEndTimeSub_Click(object sender, EventArgs e)
        {
            ChangeEndTime("-");
        }

        /// <summary>
        /// 重置时间
        /// </summary>
        /// <param name="room"></param>
        private void ResetTime(Room room)
        {
            if (null == order && StartTimeTemp == long.Parse(DateTime.ParseExact(krplEndTimeValue.Text.ToString(), "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss")))
            {
                if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                {
                    DateTime time = DateTime.Now;
                    StartTimeTemp = long.Parse(time.ToString("yyyyMMddHHmm00"));
                    krplEndTimeValue.Text = time.ToString("yyyy-MM-dd HH:mm");
                }
            }
        }
        /// <summary>
        /// 修改结束时间
        /// </summary>
        /// <param name="mark"></param>
        private void ChangeEndTime(string mark)
        {
            

            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == this.RoomId).FirstOrDefault();

            ResetTime(room);

            long startTime = order == null ? StartTimeTemp : order.StartTime.Value;
            long endTime = long.Parse(DateTime.ParseExact(krplEndTimeValue.Text.ToString(), "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));//long endTime = order != null ? order.EndTime.Value : long.Parse(DateTime.ParseExact(krplEndTimeValue.Text.ToString(), "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));


            

            TimeChangeWindow window = new TimeChangeWindow(mark, startTime, endTime, room.IsPayByTime, _tempUnlimitedTime, null == this.order);
            if (window.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                krplEndTimeValue.Text = DateTime.ParseExact(window.ReturnValue.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                // 如果跟订单上次保存的不一样,就提示未保存提示
                if (null != order && order.EndTime == window.ReturnValue)
                    krplEndTimeChange.Visible = false;
                else
                    krplEndTimeChange.Visible = true;


                if (_tempUnlimitedTime != window.UnlimitedTime && !krplEndTimeChange.Visible)
                    krplEndTimeChange.Visible = true;




                if (endTime != window.ReturnValue || _tempUnlimitedTime != window.UnlimitedTime)
                {

                    _tempUnlimitedTime = window.UnlimitedTime ;

                    Order tempOrder;
                    List<OrderDetail> details;
                    Calc(out details, out tempOrder, true);
                    CheckChange();
                }
            }
        }



        private bool _tempUnlimitedTime = false;
        /// <summary>
        /// 修改备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptbRemark_TextChanged(object sender, EventArgs e)
        {
            if (order == null || (order != null && krptbRemark.Text != order.Remark))
                krplRemarkChange.Visible = true;
            else
                krplRemarkChange.Visible = false;

            CheckChange();
        }


        /// <summary>
        /// 设置冻结
        /// </summary>
        /// <param name="IsOnlyCurrent"></param>
        private void SetFreeze(bool IsOnlyCurrent = false)
        {
            // 当前行编辑结束后必须冻结产品单元
            if (IsOnlyCurrent)
            {
                long Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrderDetailId"].Value.ToString());

                if (Id != -1)
                    krpdgList.SelectedRows[0].ReadOnly = true;
            }
            // 如果是确认状态, 让所有非确认状态的列改为只读(不能修改)
            else if (IsConfirm)
            {
                foreach (DataGridViewRow row in krpdgList.Rows)
                {
                    if (row.Cells["krpcmState"].ToString() != GetOrderDetailsState(1))
                        row.ReadOnly = true;
                    else
                        row.ReadOnly = false;
                }
            }
            else
            {
                // 让所有产品名称列表修改为冻结
                foreach (DataGridViewRow row in krpdgList.Rows)
                {
                    if (!row.Cells["krpcmEdit"].Value.Equals("*"))
                        row.ReadOnly = true;
                    else
                        row.ReadOnly = false;
                }
            }
        }




        /// <summary>
        /// 通过条形码添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ktpbAddByBarcode_Click(object sender, EventArgs e)
        {
            FastGridWindow window = new FastGridWindow(true, true, true);
            window.ShowDialog(this);

            HandleFastGrid(window);
            
        }

        /// <summary>
        /// 快速Grid添加(按产品类型)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAddByFastGrid_Click(object sender, EventArgs e)
        {
            FastGridWindow window = new FastGridWindow(true, false, true);
            window.ShowDialog(this);

            HandleFastGrid(window);
              
        }


        /// <summary>
        /// 处理快速查找
        /// </summary>
        /// <param name="window"></param>
        private void HandleFastGrid(FastGridWindow window)
        {
            if (window.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var item in window.ReturnValue)
                {
                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.Key && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();

                    string name = "";
                    if (Resources.GetRes().MainLangIndex == 0)
                        name = product.ProductName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        name = product.ProductName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        name = product.ProductName2;


                    // 判断是否已存在!
                    if (ProductAlreadyExists(name))
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("CurrentProductAlreadyExists"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        AddToGrid("*", "-1", product.ProductId, item.Value.Price, item.Value.Count, Math.Round(item.Value.Price * item.Value.Count, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                            krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;
                    }



                }

                Order import;
                List<OrderDetail> importDetails;
                Calc(out importDetails, out import);
                CheckChange();
            }
        }


        /// <summary>
        /// 重新加载请求列表框
        /// </summary>
        internal void ReloadRequest()
        {
            krpcmRequest.SetValues(null, false);
            if (Resources.GetRes().Requests.Where(x => x.IsEnable == 1).Count() > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmRequest.SetValues(Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).Select(x => x.RequestName0).ToArray(), true);
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmRequest.SetValues(Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).Select(x => x.RequestName1).ToArray(), true);
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmRequest.SetValues(Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).Select(x => x.RequestName2).ToArray(), true);
            }


        }



        /// <summary>
        /// 检查是否已存在
        /// </summary>
        /// <param name="productName"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private bool ProductAlreadyExists(string productName, int rowIndex = -1)
        {
            for (int i = krpdgList.Rows.Count - 1; i >= 0; i--)
            {

                if (krpdgList.Rows[i].Cells["krpcmEdit"].Value.Equals("*"))
                {
                    string name = krpdgList.Rows[i].Cells["krpcmProductName"].Value.ToString();

                    if (!string.IsNullOrWhiteSpace(name) && productName == name)
                    {
                        if (rowIndex != -1 && rowIndex != i)
                            return true;
                        else if (rowIndex == -1)
                            return true;
                    }
                        
                }
            }
            return false;
        }

        /// <summary>
        /// 语言被修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 刷新第二屏语言
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex, -1);
            }

            if (null != this.order && krpcbLanguage.ContainsFocus)
            {
                krplRemarkChange.Visible = true;
                CheckChange();
            }
        }






        /// <summary>
        /// 查看数据
        /// </summary>
        private void CheckHistory()
        {

            if (this.order == null)
                return;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {

                    List<Log> logList = null;

                    DateTime orderAddTime = (DateTime.ParseExact(order.AddTime.ToString(), "yyyyMMddHHmmss", null));

                    DateTime startDateTime = orderAddTime;
                    DateTime endDateTime = orderAddTime;

                    startDateTime = startDateTime.AddDays(-1);
                    endDateTime = endDateTime.AddDays(Resources.GetRes().ShorDay);


                    long startTimeFinal = long.Parse(startDateTime.ToString("yyyyMMddHHmmss"));
                    long endTimeFinal = long.Parse(endDateTime.ToString("yyyyMMddHHmmss"));

                    List<Balance> Balance;


                    bool result = OperatesService.GetOperates().ServiceGetLog(-1, -1, startTimeFinal, endTimeFinal, out Balance, out logList, this.order.OrderId);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {

                            if (null == logList || logList.Count == 0)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            logList = logList.OrderBy(x => x.AddTime).ThenBy(x => x.LogId).ToList();

                            HistoryOrderDetailsWindow details = new HistoryOrderDetailsWindow(logList);
                            details.StartLoad += (sender2, e2) =>
                            {
                                StartLoad(sender2, null);
                            };
                            details.StopLoad += (sender2, e2) =>
                            {
                                StopLoad(sender2, null);
                            };
                            details.ShowDialog(this);
                        }
                        else
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("GetFailed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        }), false, Resources.GetRes().GetString("GetFailed"));
                    }));
                }
                StopLoad(this, null);
            });

        }


        /// <summary>
        /// 打印
        /// </summary>
        private void PrintOrder()
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



                    this.order.tb_orderdetail = resultList;
                    bool result = Res.Reports.Print.Instance.PrintOrder(order, Lang);
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
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbRefresh_Click(object sender, EventArgs e)
        {
            Order order;
            List<OrderDetail> details;

            bool IgnoreNotConfirm = true;


            try
            {
                Calc(out details, out order, false, false, IgnoreNotConfirm);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, Resources.GetRes().GetString("SaveFailt"));
                return;
            }

            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
            this.RoomId = model.RoomId;
            this.order = model.PayOrder;
            this.resultList = (null == model.PayOrder ? null : model.PayOrder.tb_orderdetail.ToList());
            this.payList = (null == model.PayOrder ? null : model.PayOrder.tb_orderpay.ToList());
            this.RoomStateSession = model.OrderSession;

            Init();


            foreach (var item in details)
            {
                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                AddToGrid("*", "-1", product.ProductId, item.Price, item.Count, Math.Round(item.Price * item.Count, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                    krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;
            }


            Order import;
            List<OrderDetail> importDetails;
            Calc(out importDetails, out import);
            CheckChange();

        }
    }
}
