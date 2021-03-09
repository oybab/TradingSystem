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
using Oybab.ServicePC.DialogWindow;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class HomeWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 100;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<RoomModel> resultList = null;


        private Image EmptyImage;
        private Image OccupiedImage;
        private Image OccupiedImage_Timeup;
        private Image NewOrderAlertImage;
        private Image CustomerCallImage;


        private Pos.MainWindow posMainWindow = new Pos.MainWindow();

        public HomeWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificateSendFromServer += (obj, value, args) => {  if (null != args && args.ToString() == "Call") RoomCallAdd(value); else RefreshSome(new List<long>() { value }); };
            Notification.Instance.NotificateSendsFromServer += (obj, value, args) => { RefreshSome(value); };
            Notification.Instance.NotificateSend += (obj, value, args) => { if (value == -1) this.BeginInvoke(new Action(() => { InitFire(); })); };


            krpdgList.SortCompare += customSortCompare;


            this.ControlBox = false;

            flpRooms.Visible = false;

            new CustomTooltip(this.krpdgList);

            this.Text = Resources.GetRes().GetString("MainPage");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            EmptyImage = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Empty.png")); 
            OccupiedImage = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Occupied.png"));
            OccupiedImage_Timeup = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Occupied_timeup.png")); 
            NewOrderAlertImage = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.NewOrderAlert.png")); 
            CustomerCallImage = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.CustomerCall.png")); 

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");

            krpbListMode.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Listview.png"));
            krpbImageMode.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ImageMode.png"));
            krpbAddTakeout.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ShoppingCart.png"));
            krpbPos.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Pos.png"));
            krpbListMode.StateCommon.Back.ImageStyle = krpbImageMode.StateCommon.Back.ImageStyle = krpbAddTakeout.StateCommon.Back.ImageStyle = krpbPos.StateCommon.Back.ImageStyle = PaletteImageStyle.Stretch;
            krpbListMode.StateCommon.Back.Draw = krpbImageMode.StateCommon.Back.Draw = krpbAddTakeout.StateCommon.Back.Draw = krpbPos.StateCommon.Back.Draw = InheritBool.True;

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }
            //增加右键
            LoadContextMenu(kryptonContextMenuItemAddTakeout, Resources.GetRes().GetString("NewOrder"), Resources.GetRes().GetString("AddOuterBill"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.NewOrder.png")), (sender, e) => {krpbAddTakeout_Click(new object(), null);});
            //增加右键
            LoadContextMenu(kryptonContextMenuItemNewOrder, Resources.GetRes().GetString("NewOrder"), Resources.GetRes().GetString("NewOrderDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.NewOrder.png")), (sender, e) => { NewOrder(); });
            LoadContextMenu(kryptonContextMenuItemShowOrder, Resources.GetRes().GetString("ShowOrder"), Resources.GetRes().GetString("ShowOrderDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ShowOrder.png")), (sender, e) => { ShowOrder(); });
            LoadContextMenu(kryptonContextMenuItemCheckoutOrder, Resources.GetRes().GetString("CheckoutOrder"), Resources.GetRes().GetString("CheckoutOrderDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.CheckoutOrder.png")), (sender, e) => { CheckoutOrder(); });
            LoadContextMenu(kryptonContextMenuItemCancelOrder, Resources.GetRes().GetString("CancelOrder"), Resources.GetRes().GetString("CancelOrderDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.CancelOrder.png")), (sender, e) => { CancelOrder(); });
            LoadContextMenu(kryptonContextMenuItemReplaceRoom, Resources.GetRes().GetString("ReplaceRoom"), Resources.GetRes().GetString("ReplaceRoomDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ReplaceRoom.png")), (sender, e) => { ReplaceRoom(); });
            LoadContextMenu(kryptonContextMenuItemRestart, Resources.GetRes().GetString("Restart"), Resources.GetRes().GetString("RestartDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Restart.png")), (sender, e) => { Restart(); });
            LoadContextMenu(kryptonContextMenuItemShutdown, Resources.GetRes().GetString("Shutdown"), Resources.GetRes().GetString("ShutdownDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Shutdown.png")), (sender, e) => { Shutdown(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Main.ico"));

            //初始化
            Init();


            resultList = new List<RoomModel>();
           

            Display();

            WindowState = FormWindowState.Maximized;







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

            krpcmCall.HeaderText = krplNewCall.Text = Resources.GetRes().GetString("Call");
            krpcmOrder.HeaderText = krplNewOrder.Text = Resources.GetRes().GetString("Consumption");
            krpcmRoomNo.HeaderText = Resources.GetRes().GetString("RoomNo");
            krpcmStartTime.HeaderText = Resources.GetRes().GetString("StartTime");
            krpcmEndTime.HeaderText = Resources.GetRes().GetString("EndTime");
            krpcmState.HeaderText = Resources.GetRes().GetString("State");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");
            krpcmPaidPrice.HeaderText = Resources.GetRes().GetString("PaidPrice");
            krpcmMemberPaidPrice.HeaderText = Resources.GetRes().GetString("MemberPaidPrice");
            krpcmBalancePrice.HeaderText = Resources.GetRes().GetString("BalancePrice");
            krpcmLang.HeaderText = Resources.GetRes().GetString("Language");
            krpcmRemark.HeaderText = Resources.GetRes().GetString("Remark");

            krpbSendFireAlarm.Text = Resources.GetRes().GetString("SendFireAlarm");

            krplNewOrderValue.Text = "0";
            krplNewCallValue.Text = "0";

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                this.krpbSendFireAlarm.Size = new System.Drawing.Size(130.RecalcMagnification(), this.krpbSendFireAlarm.Size.Height);
                
            }

            // 点歌系统就显示呼叫
            if (Resources.GetRes().IsRequired("Vod"))
                krpcmCall.Visible = krplNewCall.Visible = krplNewCallValue.Visible = kryptonLabel1.Visible = true;


            InitFire();

            // 如果KEY中桌子数为0, 则不显示桌子
            if (Resources.GetRes().RoomCount <= 0 || !Common.GetCommon().IsAddInnerBill()) // <=
            {
                flpRooms.Visible = false;
                krpdgList.Visible = false;

                krpbListMode.Visible = false;
                krpbImageMode.Visible = false;

                krplNewCall.Visible = krplNewCallValue.Visible = kryptonLabel1.Visible = false;
            }


            // 如果不允许用户用外部账单, 则不显示外部账单
            if (!Common.GetCommon().IsAddOuterBill())
            {
                krpbAddTakeout.Visible = false;
                krpbPos.Visible = false;
                tlpTakeout.Visible = false;



            }
            else
            {


                TakeoutOperateWindow window = new TakeoutOperateWindow(true);
                window.StartLoad += (obj, e2) =>
                {
                    StartLoad(obj, null);
                };
                window.StopLoad += (obj, e2) =>
                {
                    StopLoad(obj, null);
                };

                window.TopLevel = false;
                tlpTakeout.Controls.Add(window);
                window.Anchor = AnchorStyles.None;

                window.Show();

                if (Common.GetCommon().IsAddInnerBill() && Resources.GetRes().RoomsModel.Count > 0)
                {
                    krpbMode_Click(krpbListMode, null);
                }
                else
                {
                    krpbAddTakeout_Click(null, null);
                }


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
                krpbSendFireAlarm.Visible = true;
                if (Resources.GetRes().IsFireAlarmEnable)
                {
                    krpbSendFireAlarm.Text = Resources.GetRes().GetString("CancelFireAlarm");
                    krpbSendFireAlarm.StateCommon.Content.ShortText.Color1 = Color.Red;
                }
                else {
                    krpbSendFireAlarm.Text = Resources.GetRes().GetString("SendFireAlarm");
                    krpbSendFireAlarm.StateCommon.Content.ShortText.Color1 = Color.Empty;
                }
            }
        }


        private void Display()
        {
            //为未保存数据而忽略当前操作
            if (!IgnoreOperateForSave())
                return;

            //查找数据

            //写入
            foreach (var item in Resources.GetRes().RoomsModel.OrderByDescending(x => x.Order).ThenBy(x => x.RoomNo.Length).ThenBy(x => x.RoomNo))
            {
                resultList.Add(new RoomModel() { RoomId = item.RoomId, Order = item.Order, OrderSession = item.OrderSession, State = item.State, HideType = item.HideType, RoomNo = item.RoomNo, PayOrder = item.PayOrder });
            }
            

            //设定页面数据
            ResetPage();
            if (resultList.Count() > 0)
            {
                //resultList.Reverse();
                AllPage = (int)((resultList.Count() - 1 + ListCount) / ListCount);
                krplPageCount.Text = AllPage.ToString();

                CurrentPage = 1;
                krptCurrentPage.Text = CurrentPage.ToString();

                //打开第一页
                OpenPageTo(CurrentPage, false);
            }
            else
            {
                krpdgList.Rows.Clear();
            }
            RefreshNotification();
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
            var currentResult = resultList.Skip((CurrentPage - 1) * ListCount).Take(ListCount);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid(item, item.RoomId, item.PayOrder, item.OrderSession, item.State, 0);
            }
            SetColor(false);
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="model"></param>
        /// <param name="RoomId"></param>
        /// <param name="order"></param>
        /// <param name="orderSession"></param>
        /// <param name="state"></param>
        /// <param name="mode"></param>
        private void AddToGrid(RoomModel model, long RoomId, Order order, string orderSession, int state, int mode)
        {
            string NewCall = "";
            string NewOrder = "";
            string OrderSessionStr = "";
            string RoomNo = "";
            string StateStr = "";
            string Lang = "";
            string Remark = "";
            string TotalPrice = "";
            string PaidPrice = "";
            string MemberPaidPrice = "";
            string BalancePrice = "";
            string StartTimeStr = "";
            string EndTimeStr = "";
            long OrderId = 0;
            bool IsNew = false;
            bool IsCall = false;
           

            try
            {
                if (!string.IsNullOrWhiteSpace(orderSession))
                    OrderSessionStr = orderSession;

                StateStr = GetState(state);
                RoomNo = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).Select(x => x.RoomNo).FirstOrDefault();

                if (null != order)
                {
                    Lang = GetLanguage(order.Lang);
                    Remark = order.Remark;
                    TotalPrice = order.TotalPrice.ToString();
                    PaidPrice = order.TotalPaidPrice.ToString();
                    MemberPaidPrice = order.MemberPaidPrice.ToString();
                    OrderId = order.OrderId;

                    if (null != order.tb_orderdetail && order.tb_orderdetail.Where(x => x.State == 1).Count() > 0)
                    {
                        NewOrder = "New";
                        IsNew = true;
                    }

                    if (null != order.StartTime && order.StartTime != 0)
                        StartTimeStr = DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");
                    if (null != order.StartTime && order.EndTime != 0)
                        EndTimeStr = DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");

                        // 计算余额
                        BalancePrice = Math.Round(order.TotalPaidPrice - order.TotalPrice, 2).ToString();


                }

                if (Resources.GetRes().CallNotifications.ContainsKey(RoomId))
                {
                    if (Resources.GetRes().CallNotifications[RoomId])
                    {
                        NewCall = "New";
                        IsCall = true;
                    }
                        
                    
                }



                


            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
            // 添加
            if (mode == 0)
            {
                krpdgList.Rows.Add(NewCall, NewOrder, orderSession, OrderId, RoomId, RoomNo, StartTimeStr, EndTimeStr, StateStr, TotalPrice, PaidPrice, MemberPaidPrice, BalancePrice, Lang, Remark);

                this.SuspendLayout();
                flpRooms.Controls.Add(new RoomControl(this, this.flpRooms, model, RoomNo, IsCall, IsNew, StateStr, MouseRightClick, MouseDoubleClicks, Selected, KeyDowns, EmptyImage, OccupiedImage, OccupiedImage_Timeup, NewOrderAlertImage, CustomerCallImage));
                this.ResumeLayout(true);
            }
            // 添加
            if (mode == 1)
            {
                this.krpdgList.Invoke(new Action(() =>
                {
                    krpdgList.Rows.Add(NewCall, NewOrder, orderSession, OrderId, RoomId, RoomNo, StartTimeStr, EndTimeStr, StateStr, TotalPrice, PaidPrice, MemberPaidPrice, BalancePrice, Lang, Remark);

                    this.SuspendLayout();
                    flpRooms.Controls.Add(new RoomControl(this, this.flpRooms, model, RoomNo, IsCall, IsNew, StateStr, MouseRightClick, MouseDoubleClicks, Selected, KeyDowns, EmptyImage, OccupiedImage, OccupiedImage_Timeup, NewOrderAlertImage, CustomerCallImage));
                    this.ResumeLayout(true);
                }));
            }
            // 找到并替换
            if (mode == 2)
            {
                int rowIndex = -1;
                int controlIndex = -1;

                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {

                    //只有有改动才可以继续
                    if (long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString()) == RoomId)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                for (int i = 0; i < flpRooms.Controls.Count; i++)
                {

                    //只有有改动才可以继续
                    if ((flpRooms.Controls[i] as RoomControl).RoomId == RoomId)
                    {
                        controlIndex = i;
                        break;
                    }
                }

                // 找到后替换, 否则新加
                if (rowIndex != -1)
                {
                    this.krpdgList.Invoke(new Action(() =>
                    {
                        krpdgList.Rows.RemoveAt(rowIndex);
                        krpdgList.Rows.Insert(rowIndex, NewCall, NewOrder, orderSession, OrderId, RoomId, RoomNo, StartTimeStr, EndTimeStr, StateStr, TotalPrice, PaidPrice, MemberPaidPrice, BalancePrice, Lang, Remark);
                        krpdgList.Rows[rowIndex].Selected = true;

                        this.SuspendLayout();
                        flpRooms.Controls.RemoveAt(controlIndex);
                        RoomControl control = new RoomControl(this, this.flpRooms, model, RoomNo, IsCall, IsNew, StateStr, MouseRightClick, MouseDoubleClicks, Selected, KeyDowns, EmptyImage, OccupiedImage, OccupiedImage_Timeup, NewOrderAlertImage, CustomerCallImage);
                        flpRooms.Controls.Add(control);
                        flpRooms.Controls.SetChildIndex(control, controlIndex);
                        this.ResumeLayout(true);
                    }));
                }
                else
                {
                    this.krpdgList.Invoke(new Action(() =>
                    {
                        krpdgList.Rows.Add(NewCall, NewOrder, orderSession, OrderId, RoomId, RoomNo, StartTimeStr, EndTimeStr, StateStr, TotalPrice, PaidPrice, MemberPaidPrice, BalancePrice, Lang, Remark);

                        this.SuspendLayout();
                        flpRooms.Controls.Add(new RoomControl(this, this.flpRooms, model, RoomNo, IsCall, IsNew, StateStr, MouseRightClick, MouseDoubleClicks, Selected, KeyDowns, EmptyImage, OccupiedImage, OccupiedImage_Timeup, NewOrderAlertImage, CustomerCallImage));
                        this.ResumeLayout(true);
                    }));
                }

            }
        }

        /// <summary>
        /// 更新所有
        /// </summary>
        internal void RefreshAll()
        {
            this.BeginInvoke(new Action(() =>
            {
                SetColor(true);
                RefreshNotification();
            }));

        }


        /// <summary>
        /// 更新部分
        /// </summary>
        internal void RefreshSome(List<long> RoomsId)
        {
            foreach (var RoomId in RoomsId)
            {
                RoomModel item = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                RoomModel model = resultList.Where(x => x.RoomId == RoomId).FirstOrDefault();
                if (null == model)
                {
                    resultList.Add(new RoomModel() { RoomId = item.RoomId, Order = item.Order, OrderSession = item.OrderSession, State = item.State, HideType = item.HideType, RoomNo = item.RoomNo, PayOrder = item.PayOrder });
                    AddToGrid(item, item.RoomId, item.PayOrder, item.OrderSession, item.State, 1);
                }
                else
                {
                    if (null != item && null != model && model.OrderSession != item.OrderSession)
                    {
                        int no = resultList.FindIndex(x => null != x.OrderSession && x.OrderSession.Equals(model.OrderSession, StringComparison.Ordinal));
                        resultList.RemoveAt(no);
                        resultList.Insert(no, new RoomModel() { RoomId = item.RoomId, Order = item.Order, OrderSession = item.OrderSession, State = item.State, HideType = item.HideType, RoomNo = item.RoomNo, PayOrder = item.PayOrder });

                        AddToGrid(item, item.RoomId, item.PayOrder, item.OrderSession, item.State, 2);
                    }
                    else
                    {
                        if (null != model && item == null)
                        {
                            int no = resultList.FindIndex(x => x.OrderSession.Equals(model.OrderSession, StringComparison.Ordinal));
                            if (-1 != no)
                            {
                                resultList.RemoveAt(no);
                            }
                            RoomRemove(model);
                        }
                    }
                }
            }
            this.BeginInvoke(new Action(() =>
            {
                SetColor(true);
                RefreshNotification();
            }));
            
        }


        /// <summary>
        /// 从服务器刷新
        /// </summary>
        /// <param name="RoomsId"></param>
        internal void RefreshSomeFromServer(List<long> RoomsId)
        {

            StartLoad(this, null);

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
                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("RefreshOrderFailed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("RefreshOrderFailed"));
                    }));
                }
                StopLoad(this, null);
            });

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
            return true;
        }




        /// <summary>
        /// 新建订单
        /// </summary>
        private void NewOrder()
        {
            DisplayOrder();
        }

        /// <summary>
        /// 查看订单
        /// </summary>
        private void ShowOrder()
        {
            DisplayOrder();
        }

        private void DisplayOrder()
        {
            RoomModel model = null;
            bool IsCalled = false;

            if (!krpbImageMode.Enabled)
            {
                model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == SelectedRoomId).FirstOrDefault();
                if (SelectedRoomControl.Calld)
                    IsCalled = true;
            }
            else
            {
                long RoomId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString());
                model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();


                if (!krpdgList.SelectedRows[0].Cells["krpcmCall"].Value.Equals(""))
                {
                    IsCalled = true;
                }
            }

            if (IsCalled)
            {
                if (Resources.GetRes().CallNotifications.ContainsKey(model.RoomId))
                {
                    Resources.GetRes().CallNotifications[model.RoomId] = false;
                }

                //处理
                RoomCallRemove(model);
                RefreshNotification();
                return;
            }

            OrderOperateWindow window = new OrderOperateWindow(model);
            window.StartLoad += (obj, e) =>
            {
                StartLoad(obj, null);
            };
            window.StopLoad += (obj, e) =>
            {
                StopLoad(obj, null);
            };
            DialogResult dialogResult = window.ShowDialog(this);
            // 如果成功, 代表有改动, 重修刷新一下当前数据
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                // 更新该订单信息
                RefreshSome(new List<long>() { model.RoomId });
            }
            // 重试代表订单数据不是最新的, 重新获取
            else if (dialogResult == System.Windows.Forms.DialogResult.Retry)
            {
                RefreshSomeFromServer(new List<long>() { model.RoomId });
            }
        }


       
        /// <summary>
        /// 替换包厢
        /// </summary>
        private void ReplaceRoom()
        {
            // 如果是结账
            long oldRoomId = 0;

            if (!krpbImageMode.Enabled)
                oldRoomId = SelectedRoomId;
            else
                oldRoomId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString());


            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == oldRoomId).FirstOrDefault();
            if (null == model.PayOrder)
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshSomeFromServer(new List<long>() { model.RoomId });
                return;
            }

            ReplaceRoomWindow window = new ReplaceRoomWindow(oldRoomId);
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
                // 更新该订单信息
                RefreshSome(new List<long>() { oldRoomId, window.ReturnValue });
            }
            // 重试代表订单数据不是最新的, 重新获取
            else if (dialogResult == System.Windows.Forms.DialogResult.Retry)
            {
                RefreshSomeFromServer(new List<long>() { window.ReturnValue });
            }
        }

        /// <summary>
        /// 结账订单
        /// </summary>
        private void CheckoutOrder()
        {
            // 如果是结账
            long RoomId = 0;

            if (!krpbImageMode.Enabled)
                RoomId = SelectedRoomId;
            else
                RoomId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString());


            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

            if (null == model.PayOrder)
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshSomeFromServer(new List<long>() { model.RoomId });
                return;
            }

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
                // 更新该订单信息
                RefreshSome(new List<long>() { RoomId });
            }
            // 重试代表订单数据不是最新的, 重新获取
            else if (dialogResult == System.Windows.Forms.DialogResult.Retry)
            {
                RefreshSomeFromServer(new List<long>() { RoomId });
            }
        }

        /// <summary>
        /// 取消订单
        /// </summary>
        private void CancelOrder()
        {
            //确认取消
            var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmCancelOrder"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            RoomModel model = null;
            if (!krpbImageMode.Enabled)
                model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == SelectedRoomId).FirstOrDefault();
            else
            {
                long Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString());
                model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == Id).FirstOrDefault();
            }

            if (null == model.PayOrder)
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshSomeFromServer(new List<long>() { model.RoomId });
                return;
            }

            Order cancelOrder = model.PayOrder.FastCopy();
            cancelOrder.State = 2;

            string newRoomSessionId;
            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.GetRes().GetString("CancelOrder");

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    

                    long UpdateTime;

                    ResultModel result = OperatesService.GetOperates().ServiceEditOrder(cancelOrder, null, null, model.OrderSession, false, out newRoomSessionId, out UpdateTime);

                    

                    this.BeginInvoke(new Action(() =>
                    {
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
                                        product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);
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
                            


                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), SucMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                            model.PayOrder = null;
                            model.OrderSession = newRoomSessionId;
                            if (Resources.GetRes().CallNotifications.ContainsKey(model.RoomId))
                                Resources.GetRes().CallNotifications.Remove(model.RoomId);
                            RefreshSome(new List<long>() { model.RoomId });
                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("FaildThenRefreshModel"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                RefreshSomeFromServer(new List<long>() { model.RoomId });
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

        /// <summary>
        /// 关机
        /// </summary>
        private void Shutdown()
        {
            long RoomId = 0;

            if (!krpbImageMode.Enabled)
                RoomId = SelectedRoomId;
            else
                RoomId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString());


            string ErrMsgName = Resources.GetRes().GetString("Shutdown");
            Send(new List<long>() { RoomId }, ErrMsgName, 2);
        }

        /// <summary>
        /// 重启
        /// </summary>
        private void Restart()
        {
            long RoomId = 0;

            if (!krpbImageMode.Enabled)
                RoomId = SelectedRoomId;
            else
                RoomId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmRoomId"].Value.ToString());


            string ErrMsgName = Resources.GetRes().GetString("Restart");
            Send(new List<long>() { RoomId }, ErrMsgName, 4);
        }

        /// <summary>
        /// 发送火警
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSendFireAlarm_Click(object sender, EventArgs e)
        {
            string ErrMsgName = Resources.GetRes().GetString("SendFireAlarm");

            int sendType = 8; // FireOn

            if (Resources.GetRes().IsFireAlarmEnable)
            {
                ErrMsgName = Resources.GetRes().GetString("CancelFireAlarm");

                sendType = 16; // FireOff
            }

            var confirm = KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("ConfirmOperate"), ErrMsgName), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                // 所有设备ID都发送
                Send(Resources.GetRes().Devices.Select(x => x.DeviceId).ToList(), ErrMsgName, sendType, () =>
                {
                    InitFire();
                });
            }

           
        }

        /// <summary>
        /// 发送
        /// </summary>
        private void Send(List<long> RoomsId, string ErrMsgName, int SendType, Action success = null)
        {

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {


                    bool result = OperatesService.GetOperates().ServiceSend(RoomsId, SendType);

                    // 如果成功则提示
                    if (result)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), ErrMsgName), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                            if (null != success)
                            {
                                success();
                            }
                        }));
                    }
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


        /// <summary>
        /// 刷新通知状态
        /// </summary>
        private void RefreshNotification(){
            int NewOrderCount = 0;
            if (null != resultList)
                NewOrderCount = resultList.Where(x => null != x.PayOrder && null != x.PayOrder.tb_orderdetail && x.PayOrder.tb_orderdetail.Any(y => y.State == 1)).Count();
            int NewCallCount = Resources.GetRes().CallNotifications.Where(x => x.Value).Count();


            if (NewCallCount > 0)
                krplNewCallValue.StateCommon.ShortText.Color1 = Color.Red;
            else if (NewCallCount == 0)
                krplNewCallValue.StateCommon.ShortText.Color1 = Color.Empty;

            if (NewOrderCount > 0)
                krplNewOrderValue.StateCommon.ShortText.Color1 = Color.Red;
            else if (NewOrderCount == 0)
                krplNewOrderValue.StateCommon.ShortText.Color1 = Color.Empty;


            krplNewOrderValue.Text = NewOrderCount.ToString();
            krplNewCallValue.Text = NewCallCount.ToString();


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
                if (e.RowIndex == -1 || null == krpdgList.Rows[e.RowIndex] || krpdgList.RowCount == 0)
                {
                    kryptonContextMenuItemNewOrder.Enabled = false;
                    kryptonContextMenuItemShowOrder.Enabled = false;
                    kryptonContextMenuItemCheckoutOrder.Enabled = false;
                    kryptonContextMenuItemCancelOrder.Enabled = false;
                    kryptonContextMenuItemReplaceRoom.Enabled = false;
                    kryptonContextMenuItemRestart.Enabled = false;
                    kryptonContextMenuItemShutdown.Enabled = false;
                }
                else
                {
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
                            ExitEditMode();

                            krpdgList.Rows[e.RowIndex].Selected = true;
                            if (krpdgList.SelectedRows.Count == 0)
                                return;
                        }
                    }

                    // 只有订单编号为0,当前主机还没有新建账单
                    if (krpdgList.SelectedRows[0].Cells["krpcmOrderId"].Value.ToString().Equals("0"))
                    {
                        kryptonContextMenuItemNewOrder.Enabled = true;
                        kryptonContextMenuItemShowOrder.Enabled = false;
                        kryptonContextMenuItemCheckoutOrder.Enabled = false;
                        kryptonContextMenuItemCancelOrder.Enabled = false;
                        kryptonContextMenuItemReplaceRoom.Enabled = false;
                    }
                    else
                    {
                        kryptonContextMenuItemNewOrder.Enabled = false;
                        kryptonContextMenuItemShowOrder.Enabled = true;
                        if (krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.Equals("") && krpdgList.SelectedRows[0].Cells["krpcmCall"].Value.Equals(""))
                        {
                            kryptonContextMenuItemCheckoutOrder.Enabled = true;
                            kryptonContextMenuItemCancelOrder.Enabled = true;
                        }

                        if (Common.GetCommon().IsReplaceRoom())
                            kryptonContextMenuItemReplaceRoom.Enabled = true;

                        // 如果当前是员工模式, 并且如果只能打开自己, 则....
                        if (!krpdgList.SelectedRows[0].Cells["krpcmOrderSession"].Value.Equals(""))
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.OrderSession == krpdgList.SelectedRows[0].Cells["krpcmOrderSession"].Value.ToString()).FirstOrDefault();
                            if (null != model && null != model.PayOrder)
                            {

                                if (!Common.GetCommon().IsIncomeTradingManage())
                                    kryptonContextMenuItemCheckoutOrder.Enabled = false;

                                if (!Common.GetCommon().IsCancelOrder())
                                    kryptonContextMenuItemCancelOrder.Enabled = false;
                            }
                        }


                    }

                    // 只有当前主机在线的时候才能重启关机(1播放器-在线, 2锁屏)
                    if (krpdgList.SelectedRows[0].Cells["krpcmState"].Value.Equals(GetState(1)) || krpdgList.SelectedRows[0].Cells["krpcmState"].Value.Equals(GetState(2)))
                    {
                    }
                    else
                    {
                        kryptonContextMenuItemRestart.Enabled = false;
                        kryptonContextMenuItemShutdown.Enabled = false;
                    }
                }

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
                //当前选中的时候才能操作
                if (krpdgList.SelectedRows.Count > 0)
                {
                    // 只有在当前订单编号为0,当前主机还没有新建账单
                    if (krpdgList.SelectedRows[0].Cells["krpcmOrderId"].Value.ToString().Equals("0"))
                    {
                        if (e.KeyCode == Keys.N)
                            NewOrder();
                        
                    }
                    else
                    {
                        bool canGo = true;
                        bool canCancel = true;
                        // 如果当前是员工模式, 并且如果只能打开自己, 则....
                        if (!krpdgList.SelectedRows[0].Cells["krpcmOrderSession"].Value.Equals(""))
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.OrderSession == krpdgList.SelectedRows[0].Cells["krpcmOrderSession"].Value.ToString()).FirstOrDefault();
                            if (null != model && null != model.PayOrder)
                            {
                                if (!Common.GetCommon().IsIncomeTradingManage())
                                    canGo = false;
                                if (!Common.GetCommon().IsCancelOrder())
                                    canCancel = false;
                            }
                        }

                       
                            
                        if (canGo && e.KeyCode == Keys.O && (krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.Equals("")) && krpdgList.SelectedRows[0].Cells["krpcmCall"].Value.Equals(""))
                            CheckoutOrder();
                        else if (canCancel && e.KeyCode == Keys.C && (krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.Equals("")) && krpdgList.SelectedRows[0].Cells["krpcmCall"].Value.Equals(""))
                            CancelOrder();
                            
                       

                        if (e.KeyCode == Keys.L)
                            ShowOrder();
                        else if (e.KeyCode == Keys.N && krpdgList.SelectedRows[0].Cells["krpcmOrderId"].Value.ToString().Equals("0"))
                            NewOrder();
                        else if (e.KeyCode == Keys.R)
                            ReplaceRoom();
                        
                    }

                    // 只有当前主机在线的时候才能重启关机(1播放器-在线, 2锁屏)
                    if (krpdgList.SelectedRows[0].Cells["krpcmState"].Value.Equals(GetState(1)) || krpdgList.SelectedRows[0].Cells["krpcmState"].Value.Equals(GetState(2)))
                    {
                    }
                    
                }
            }
            else if (e.Control)
                lastKeyPressed = e.KeyCode;
        }




        private void KeyDowns(RoomModel model, RoomControl control, KeyEventArgs e)
        {
            if (SelectedRoomControl != control || SelectedRoomId != model.RoomId)
                return;

                    // 只有在当前订单为空,当前主机还没有新建账单
                    if (null == model.PayOrder)
                    {
                        if (e.KeyCode == Keys.N)
                            NewOrder();

                    }
                    else
                    {
                        bool canGo = true;
                        bool canCancel = true;
                        // 如果当前是员工模式, 并且如果只能打开自己, 则....
                        if (model.OrderSession != "")
                        {
                            if (null != model && null != model.PayOrder)
                            {
                                if (!Common.GetCommon().IsIncomeTradingManage())
                                    canGo = false;
                                if (!Common.GetCommon().IsCancelOrder())
                                    canCancel = false;
                             }
                         }

                        
                        if (canGo && e.KeyCode == Keys.O && (!SelectedRoomControl.NewOrderd) && !SelectedRoomControl.Calld)
                            CheckoutOrder();
                        else if (canCancel && e.KeyCode == Keys.C && (!SelectedRoomControl.NewOrderd) && !SelectedRoomControl.Calld)
                            CancelOrder();
                            
                        

                        if (e.KeyCode == Keys.L)
                                ShowOrder();
                        else if (e.KeyCode == Keys.N && null == model.PayOrder)
                                NewOrder();
                        else if (e.KeyCode == Keys.R)
                            ReplaceRoom();


                    }

                    // 只有当前主机在线的时候才能重启关机(1播放器-在线, 2锁屏)
                    if (SelectedRoomControl.OnlineState.Equals(GetState(1)) || SelectedRoomControl.OnlineState.Equals(GetState(2)))
                    {
                    }

        }


        private void krpdgList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && krpdgList.SelectedRows.Count > 0)
            {
                DataGridView.HitTestInfo hit = krpdgList.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.ColumnHeader)
                    return;

                // 当前订单编号为0, 说明是还没新建订单
                if (krpdgList.SelectedRows[0].Cells["krpcmOrderId"].Value.ToString().Equals("0"))
                    NewOrder();
                else
                {
                    ShowOrder();
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
                krpdgList.CurrentCell = e.Row.Cells[5];// 前面几个隐藏了 默认:e.Row.Cells[0];
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
        /// 获取状态编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetStateNo(string state)
        {
            if (state == Resources.GetRes().GetString("Offline"))
                return 0;
            else if (state == Resources.GetRes().GetString("Online"))
                return 1;
            else if (state == Resources.GetRes().GetString("Locked"))
                return 2;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetState(int stateNo)
        {
            if (stateNo == 0)
                return Resources.GetRes().GetString("Offline");
            else if (stateNo == 1)
                return Resources.GetRes().GetString("Online");
            else if (stateNo == 2)
                return Resources.GetRes().GetString("Locked");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }




        /// <summary>
        /// 获取订单语言编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetLanguageNo(string language)
        {
            return Resources.GetRes().GetLangByLangName(language).LangIndex;
        }

        /// <summary>
        /// 获取订单语言状态
        /// </summary>
        /// <param name="orderStateNo"></param>
        /// <returns></returns>
        private string GetLanguage(long languageNo)
        {
            return Resources.GetRes().GetLangByLangIndex((int)languageNo).LangName;
        }

        /// <summary>
        /// 包厢名有变化
        /// </summary>
        /// <param name="room"></param>
        internal void RoomNoChange(Room room)
        {
            for (int i = 0; i < krpdgList.Rows.Count; i++)
            {
                if (long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString()) == room.RoomId)
                {
                    krpdgList.Rows[i].Cells["krpcmRoomNo"].Value = room.RoomNo;
                    break;
                }
            }

            for (int i = 0; i < flpRooms.Controls.Count; i++)
            {
                if ((flpRooms.Controls[i] as RoomControl).RoomId == room.RoomId)
                {
                    (flpRooms.Controls[i] as RoomControl).RoomNo = room.RoomNo;
                    break;
                }
            }
        }


        /// <summary>
        /// 包厢去掉
        /// </summary>
        /// <param name="room"></param>
        internal void RoomRemove(Room room)
        {
            RoomModel oldModel = resultList.Where(x => x.RoomId == room.RoomId).FirstOrDefault();
            if (null != oldModel)
                resultList.Remove(oldModel);

            this.Invoke(new Action(() =>
            {
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    if (long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString()) == room.RoomId)
                    {
                        krpdgList.Rows.RemoveAt(i);
                        break;
                    }
                }

                for (int i = 0; i < flpRooms.Controls.Count; i++)
                {
                    if ((flpRooms.Controls[i] as RoomControl).RoomId == room.RoomId)
                    {
                        this.SuspendLayout();
                        flpRooms.Controls.RemoveAt(i);
                        this.ResumeLayout(true);
                        break;
                    }
                }
            }));
        }


        /// <summary>
        /// 包厢呼叫去掉
        /// </summary>
        /// <param name="room"></param>
        internal void RoomCallAdd(long RoomId)
        {
            Room room = resultList.Where(x => x.RoomId == RoomId).FirstOrDefault();
            if (null == room)
                return;

            bool isCall = false;

            for (int i = 0; i < krpdgList.Rows.Count; i++)
            {
                if (long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString()) == room.RoomId)
                {
                    krpdgList.Rows[i].Cells["krpcmCall"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmCall"].Style.SelectionForeColor = Color.Red;
                    krpdgList.Rows[i].Cells["krpcmCall"].Value = "New";
                    isCall = true;
                    break;
                }
            }

            for (int i = 0; i < flpRooms.Controls.Count; i++)
            {
                if ((flpRooms.Controls[i] as RoomControl).RoomId == room.RoomId)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        RoomControl crl = (flpRooms.Controls[i] as RoomControl);
                        crl.Calld = true;
                        crl.RefreshImageState();
                        isCall = true;
                    }));
                    break;
                }
            }


            // 如果需要闪烁
            if (isCall)
            {
                this.BeginInvoke(new Action(() =>
                {
                    // Flash window 3 times
                    FlashWindow.Instance.Flash(this.MdiParent, 3);
                    // Refresh state, like call count.
                    RefreshNotification();
                }));

                // 点歌系统就显示呼叫
                if (Resources.GetRes().IsRequired("Vod"))
                {
                    // 呼叫
                    Common.GetCommon().CallDevice(room.RoomNo);
                }
            }
        }


        /// <summary>
        /// 包厢呼叫去掉
        /// </summary>
        /// <param name="room"></param>
        internal void RoomCallRemove(Room room)
        {
            for (int i = 0; i < krpdgList.Rows.Count; i++)
            {
                if (long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString()) == room.RoomId)
                {
                    krpdgList.Rows[i].Cells["krpcmCall"].Value = "";
                    break;
                }
            }

            for (int i = 0; i < flpRooms.Controls.Count; i++)
            {
                if ((flpRooms.Controls[i] as RoomControl).RoomId == room.RoomId)
                {
                    RoomControl crl = (flpRooms.Controls[i] as RoomControl);
                    crl.Calld = false;
                    crl.RefreshImageState();
                    break;
                }
            }
        }
        

        /// <summary>
        /// 时间快到10分钟的, 直接显示红色
        /// </summary>
        /// <param name="Rooms"></param>
        private void SetColor(bool IsAlert, List<long> Rooms = null)
        {
            bool IsFlash = false;
            int time = 10;

            // 点歌系统就显示呼叫
            if (Resources.GetRes().IsRequired("Vod"))
                time = 30;

            // 如果是新新部分
            if (null != Rooms)
            {
                // 待确认改为红色
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    try
                    {
                        long RoomId = long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString());


                        if (Rooms.Contains(RoomId))
                        {
                            string endTimeStr = krpdgList.Rows[i].Cells["krpcmEndTime"].Value.ToString();

                            if (!string.IsNullOrWhiteSpace(endTimeStr))
                            {
                                Room room = Resources.GetRes().Rooms.Where(x=> x.RoomId == RoomId).FirstOrDefault();
                                bool isTimeLeft = false;

                                // 针对不同的时间类型有不同的判断, 按小时, 剩余10分钟.  按日, 剩余时间60分钟
                                if (room.IsPayByTime == 1 && (DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null) - DateTime.Now).TotalMinutes <= time)
                                    isTimeLeft = true;
                                else if (room.IsPayByTime == 2 && (DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null) - DateTime.Now).TotalMinutes <= 60)
                                    isTimeLeft = true;

                                if (isTimeLeft)
                                {
                                    // 如果是到期了, 则暗红色显示
                                    if (DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null) <= DateTime.Now)
                                    {
                                        if (krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor != Color.DarkRed)
                                        {
                                            krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmEndTime"].Style.SelectionForeColor = Color.DarkRed;
                                            IsFlash = true;
                                        }
                                    }
                                    else if (krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor != Color.Red)
                                    {
                                        krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmEndTime"].Style.SelectionForeColor = Color.Red;
                                        //IsFlash = true;
                                    }
                                }
                                else if (krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor != Color.Empty)
                                {
                                    krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmEndTime"].Style.SelectionForeColor = Color.Empty;
                                }

                            }

                            // 设置余额颜色
                            string BalancePriceStr = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Value.ToString();
                            if (!string.IsNullOrWhiteSpace(BalancePriceStr))
                            {
                                double BalancePrice = double.Parse(BalancePriceStr);
                                if (BalancePrice > 0 && krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor != Color.Blue)
                                    krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Blue;
                                else if (BalancePrice < 0 && krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor != Color.Red)
                                    krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Red;
                                else if (BalancePrice == 0 && krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor != Color.Empty)
                                    krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Empty;
                            }
                            

                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }



                // 待确认改为红色
                for (int i = 0; i < flpRooms.Controls.Count; i++)
                {
                    try
                    {
                        RoomControl control = flpRooms.Controls[i] as RoomControl;


                        if (Rooms.Contains(control.RoomId))
                        {
                            control.RefreshImageState();
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
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    try
                    {
                        long RoomId = long.Parse(krpdgList.Rows[i].Cells["krpcmRoomId"].Value.ToString());

                        string endTimeStr = krpdgList.Rows[i].Cells["krpcmEndTime"].Value.ToString();

                        if (!string.IsNullOrWhiteSpace(endTimeStr))
                        {
                            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();
                            bool isTimeLeft = false;

                            // 针对不同的时间类型有不同的判断, 按小时, 剩余10分钟.  按日, 剩余时间60分钟
                            if (room.IsPayByTime == 1 && (DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null) - DateTime.Now).TotalMinutes <= time)
                                isTimeLeft = true;
                            else if (room.IsPayByTime == 2 && (DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null) - DateTime.Now).TotalMinutes <= 60)
                                isTimeLeft = true;

                            if (isTimeLeft)
                            {
                                // 如果是到期了, 则暗红色显示
                                if (DateTime.ParseExact(endTimeStr, "yyyy-MM-dd HH:mm", null) <= DateTime.Now)
                                {
                                    if (krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor != Color.DarkRed)
                                    {
                                        krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmEndTime"].Style.SelectionForeColor = Color.DarkRed;
                                        IsFlash = true;
                                    }
                                }
                                else if (krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor != Color.Red)
                                {
                                    krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmEndTime"].Style.SelectionForeColor = Color.Red;
                                }
                            }
                            else if (krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor != Color.Empty)
                            {
                                krpdgList.Rows[i].Cells["krpcmEndTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmEndTime"].Style.SelectionForeColor = Color.Empty;
                            }
                        }



                        // 设置余额颜色
                        string BalancePriceStr = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(BalancePriceStr))
                        {
                            double BalancePrice = double.Parse(BalancePriceStr);
                            if (BalancePrice > 0 && krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor != Color.Blue)
                                krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Blue;
                            else if (BalancePrice < 0 && krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor != Color.Red)
                                krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Red;
                            else if (BalancePrice == 0 && krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor != Color.Empty)
                                krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }


                // 待确认改为红色
                for (int i = 0; i < flpRooms.Controls.Count; i++)
                {
                    try
                    {
                        RoomControl control = flpRooms.Controls[i] as RoomControl;

                        control.RefreshImageState();
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
                // Flash window 3 times
                FlashWindow.Instance.Flash(this.MdiParent, 3);
            }
        }




        /// <summary>
        /// 自定义排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customSortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Name == "krpcmRoomNo")
            {
                string a = e.CellValue1.ToString(), b = e.CellValue2.ToString();


               RoomModel aRoom = resultList.Where(x => x.RoomNo == a).FirstOrDefault(), bRoom = resultList.Where(x => x.RoomNo == b).FirstOrDefault();
               int orderResult = 0;

                orderResult = 0;

                if (null != aRoom && null != bRoom)
                {
                    orderResult = bRoom.Order.CompareTo(aRoom.Order);
                }

                if (orderResult == 0)
                {
                    orderResult = a.Length.CompareTo(b.Length);
                }
                if (orderResult == 0)
                {
                    orderResult = a.CompareTo(b);
                }


                e.SortResult = orderResult;

                e.Handled = true;
            }
        }

        /// <summary>
        /// 切换显示模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbImageMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpbImageMode.Enabled)
            {
                krpdgList.Visible = false;
                flpRooms.Visible = true;
            }
            else
            {
                krpdgList.Visible = true;
                flpRooms.Visible = false;
            }
        }


        private long SelectedRoomId;
        private RoomControl SelectedRoomControl;
        /// <summary>
        /// 选中
        /// </summary>
        /// <param name="model"></param>
        /// <param name="control"></param>
        private void Selected(RoomModel model, RoomControl control)
        {
            if (null != SelectedRoomControl && SelectedRoomControl != control)
                SelectedRoomControl.Selected = false;
            SelectedRoomId = model.RoomId;
            SelectedRoomControl = control;
        }

        /// <summary>
        /// 双击
        /// </summary>
        /// <param name="model"></param>
        /// <param name="control"></param>
        private void MouseRightClick(RoomModel model, RoomControl control)
        {
            if (null != SelectedRoomControl && SelectedRoomControl != control)
                SelectedRoomControl.Selected = false;
            SelectedRoomId = model.RoomId;
            SelectedRoomControl = control;

            kryptonContextMenuItemNewOrder.Enabled = false;
            kryptonContextMenuItemShowOrder.Enabled = false;
            kryptonContextMenuItemCheckoutOrder.Enabled = false;
            kryptonContextMenuItemCancelOrder.Enabled = false;
            kryptonContextMenuItemReplaceRoom.Enabled = false;
            kryptonContextMenuItemRestart.Enabled = false;
            kryptonContextMenuItemShutdown.Enabled = false;



            // 订单null, 当前主机还没有新建账单
            if (null == model.PayOrder)
            {
                kryptonContextMenuItemNewOrder.Enabled = true;
                kryptonContextMenuItemShowOrder.Enabled = false;
                kryptonContextMenuItemCheckoutOrder.Enabled = false;
                kryptonContextMenuItemCancelOrder.Enabled = false;
                kryptonContextMenuItemReplaceRoom.Enabled = false;
            }
            else
            {
                kryptonContextMenuItemNewOrder.Enabled = false;
                kryptonContextMenuItemShowOrder.Enabled = true;
                if (!control.NewOrderd && !control.Calld)
                {
                    kryptonContextMenuItemCheckoutOrder.Enabled = true;
                    kryptonContextMenuItemCancelOrder.Enabled = true;
                }

                if (Common.GetCommon().IsReplaceRoom())
                    kryptonContextMenuItemReplaceRoom.Enabled = true;

                // 如果当前是员工模式, 并且如果只能打开自己, 则....
                if (model.OrderSession != "")
                {
                    if (null != model && null != model.PayOrder )
                    {
                        if (!Common.GetCommon().IsIncomeTradingManage())
                            kryptonContextMenuItemCheckoutOrder.Enabled = false;
                        if (!Common.GetCommon().IsCancelOrder())
                            kryptonContextMenuItemCancelOrder.Enabled = false;
                    }
                }


            }

            // 只有当前主机在线的时候才能重启关机(1播放器-在线, 2锁屏)
            if (control.OnlineState.Equals(GetState(1)) || control.OnlineState.Equals(GetState(2)))
            {
            }
            else
            {
                kryptonContextMenuItemRestart.Enabled = false;
                kryptonContextMenuItemShutdown.Enabled = false;
            }


            //显示
            krpContextMenu.Show(control.RectangleToScreen(control.ClientRectangle),
                 KryptonContextMenuPositionH.Left, KryptonContextMenuPositionV.Top);
        }

        /// <summary>
        /// 单击
        /// </summary>
        /// <param name="model"></param>
        /// <param name="control"></param>
        private void MouseDoubleClicks(RoomModel model, RoomControl control)
        {
            if (null != SelectedRoomControl && SelectedRoomControl != control)
                SelectedRoomControl.Selected = false;
            SelectedRoomId = model.RoomId;
            SelectedRoomControl = control;

            // 当前订单编号为0, 说明是还没新建订单
            if (null == model.PayOrder)
                NewOrder();
            else
            {
                ShowOrder();
            }
        }


       


       

        private void krpbMode_Click(object sender, EventArgs e)
        {
            KryptonButton btn =  sender as KryptonButton;
            if (null != btn && btn.Name == "krpbImageMode")
            {
                krpbImageMode.Enabled = false;
                krpbListMode.Enabled = true;
                krpbAddTakeout.Enabled = true;
                flpRooms.Visible = true;
                krpdgList.Visible = false;
                tlpTakeout.Visible = false;
            }
            else if (null != btn && btn.Name == "krpbListMode")
            {
                krpbListMode.Enabled = false;
                krpbAddTakeout.Enabled = true;
                krpbImageMode.Enabled = true;
                krpdgList.Visible = true;
                flpRooms.Visible = false;
                tlpTakeout.Visible = false;
                krpdgList.Focus();
            }
        }


        /// <summary>
        /// 新增外部账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAddTakeout_Click(object sender, EventArgs e)
        {
            krpbImageMode.Enabled = true;
            krpbListMode.Enabled = true;
            krpbAddTakeout.Enabled = false;
            flpRooms.Visible = false;
            krpdgList.Visible = false;
            tlpTakeout.Visible = true;


            
        }

        ///// <summary>
        ///// Takeout添加功能
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void HomeWindow_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if ((Resources.GetRes().RoomCount <= 0 || !Common.GetCommon().IsAddInnerBill()) && e.Button == System.Windows.Forms.MouseButtons.Right)
        //    {
        //        //显示
        //        krpContextMenuTakeout.Show(this.RectangleToScreen(this.ClientRectangle),
        //             KryptonContextMenuPositionH.Left, KryptonContextMenuPositionV.Top);
        //    }
        //}


        /// <summary>
        /// 打开VOD滚动编辑功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krplNewCallValue_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.LeftCtrl) && !System.Windows.Input.Keyboard.IsKeyDown(System.Windows.Input.Key.RightCtrl))
                return;

                if (!Resources.GetRes().IsRequired("Vod"))
                return;

            if (null == Resources.GetRes().AdminModel || Resources.GetRes().AdminModel.AdminNo != "1000")
                return;

            VodScrollWindow window = new VodScrollWindow();
            window.StartLoad += (obj, e2) =>
            {
                StartLoad(obj, null);
            };
            window.StopLoad += (obj, e2) =>
            {
                StopLoad(obj, null);
            };

            window.ShowDialog(this);
        }

        private void krpbPos_Click(object sender, EventArgs e)
        {
            // 加载完毕再初始化
            if (posMainWindow.IsLoaded)
                posMainWindow.Init();

            posMainWindow.ShowDialog();
        }
    }
}
