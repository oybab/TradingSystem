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
using Oybab.Report.Model;
using Oybab.ServicePC.DialogWindow;
using System.Drawing.Drawing2D;
using Oybab.ServicePC.Tools;
using Oybab.Res.View.Models;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class OrderWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<BillModel> resultList = null;
        private TimeSpan TimeLimit = TimeSpan.FromDays(Resources.GetRes().ShorDay);

        public OrderWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();

            this.ControlBox = false;
            new CustomTooltip(this.krpdgList);

            this.Text = Resources.GetRes().GetString("IncomeManager");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");

            krpbSearch.Text = Resources.GetRes().GetString("Search");
            krplAddTime.Text = Resources.GetRes().GetString("AddTime");


            krptbStartTime.Value =  DateTime.Now.AddDays(-1);
            krptbEndTime.Value = DateTime.Now;

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krpcbOrderType.Location = new Point(krpcbOrderType.Location.X, krpcbOrderType.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());
            }
            

            
            //增加右键

            //显示
            LoadContextMenu(kryptonContextMenuItemShow, Resources.GetRes().GetString("Show"), Resources.GetRes().GetString("ShowDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.OrderDetails.png")), (sender, e) => { Check(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Order.ico"));

            //初始化
            Init();

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
            krpcmOrderId.HeaderText = Resources.GetRes().GetString("OrderNo");
            krpcmRoomNo.HeaderText = Resources.GetRes().GetString("RoomNo");
            krpcmStartTime.HeaderText = Resources.GetRes().GetString("StartTime");
            krpcmEndTime.HeaderText = Resources.GetRes().GetString("EndTime");
            krpcmState.HeaderText = Resources.GetRes().GetString("State");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");
            krpcmPaidPrice.HeaderText = Resources.GetRes().GetString("PaidPrice");
            krpcmLang.HeaderText = Resources.GetRes().GetString("Language");
            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcmFinishTime.HeaderText = Resources.GetRes().GetString("FinishTime");
            krpcmRemark.HeaderText = Resources.GetRes().GetString("Remark");
;
            krpcmBorrowPrice.HeaderText = Resources.GetRes().GetString("OwedPrice");
            krpcmKeepPrice.HeaderText = Resources.GetRes().GetString("KeepPrice");
            krpcmMemberName.HeaderText = Resources.GetRes().GetString("MemberName");
            krpcmMemberPaidPrice.HeaderText = Resources.GetRes().GetString("MemberPaidPrice");
            krpcmTotalPaidPrice.HeaderText = Resources.GetRes().GetString("TotalPaidPrice");

            krpcmFinishAdminName.HeaderText = Resources.GetRes().GetString("CheckoutBy");


            krpcmPhone.HeaderText = Resources.GetRes().GetString("Phone");
            krpcbIsDisplayAll.Text = Resources.GetRes().GetString("IsDisplayAll");
            krpcbMultipleLanguage.Text = Resources.GetRes().GetString("MultiLanguage");
            
            krpcmName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("PersonName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("PersonName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("PersonName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);

            krplOrderType.Text = Resources.GetRes().GetString("BillType");
            krpcbOrderType.Items.AddRange(new string[] { Resources.GetRes().GetString("BillTypeInner"), Resources.GetRes().GetString("BillTypeOuter") });

            if (Resources.GetRes().RoomCount <= 0)
            {
                krpcbOrderType.SelectedIndex = 1;
                krpcbOrderType.Visible = false;
                krplOrderType.Visible = false;
            }
            else
            {
                krpcbOrderType.SelectedIndex = 0;
            }

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


            DateTime startDateTime = DateTime.Now;
            DateTime endDateTime = DateTime.Now;

            try
            {
                startDateTime = krptbStartTime.Value;
                endDateTime = krptbEndTime.Value;
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyError"), Resources.GetRes().GetString("Time")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ExceptionPro.ExpLog(ex);
                return;
            }

            // 有包厢在用可以搜索7天内的. 没有则可以搜索35天内的
            if (Resources.GetRes().RoomsModel.Any(x => null != x.PayOrder))
                TimeLimit = TimeSpan.FromDays(Resources.GetRes().DefaultDay);
            else
                TimeLimit = TimeSpan.FromDays(Resources.GetRes().LongDay);

            if ((endDateTime - startDateTime).TotalMinutes <= 0 || !((endDateTime - startDateTime).TotalMinutes <= TimeLimit.TotalMinutes))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("TimeLimit"), TimeLimit.TotalDays), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long addTimeStart = long.Parse(startDateTime.ToString("yyyyMMddHHmmss"));
            long addTimeEnd = long.Parse(endDateTime.ToString("yyyyMMddHHmmss"));

            StartLoad(this, null);

            int OrderType = krpcbOrderType.SelectedIndex;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<Order> orders = new List<Order>();
                    List<Takeout> takeouts = new List<Takeout>();
                    bool result = false;

                    if (OrderType == 0)
                        result = OperatesService.GetOperates().ServiceGetOrders(0, 0, addTimeStart, addTimeEnd, 0, -1, false, -1, -1, out orders);
                    else if (OrderType == 1)
                        result = OperatesService.GetOperates().ServiceGetTakeouts(0, addTimeStart, addTimeEnd, -1, null, null, false, false, -1, -1, out takeouts);


                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            if (OrderType == 0)
                                this.resultList = orders.Select(x=> new BillModel(x)).OrderByDescending(x => x.Id).ToList();
                            else if (OrderType == 1)
                                this.resultList = takeouts.Select(x => new BillModel(x)).OrderByDescending(x => x.Id).ToList();



                            
                            //设定页面数据
                            ResetPage();
                            if (resultList.Count() > 0)
                            {
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
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);

                // 防止滚动条比例没准确显示导致不显示底部的数据
                this.BeginInvoke(new Action(() =>
                {
                    krpdgList.PerformLayout();
                }));
            });
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
                AddToGrid("", item.Id.ToString(), Resources.GetRes().Rooms.Where(x => x.RoomId == item.RoomId).Select(x => x.RoomNo).FirstOrDefault(), item.tb_member, item.TotalPrice, item.MemberPaidPrice, item.PaidPrice, item.TotalPaidPrice, item.BorrowPrice, item.KeepPrice, item.FinishAdminId, item.StartTime, item.EndTime, item.State, GetLanguage(item.Lang), item.AddTime, item.FinishTime, item.Phone, item.Name0, item.Name1, item.Name2, item.Remark, item);
            }
        }


        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="roomNo"></param>
        /// <param name="member"></param>
        /// <param name="TotalPrice"></param>
        /// <param name="MemberDealsPrice"></param>
        /// <param name="DealsPrice"></param>
        /// <param name="ActualPrice"></param>
        /// <param name="MemberPaidPrice"></param>
        /// <param name="PaidPrice"></param>
        /// <param name="CardPaidPrice"></param>
        /// <param name="TotalPaidPrice"></param>
        /// <param name="ReturnPrice"></param>
        /// <param name="BorrowPrice"></param>
        /// <param name="KeepPrice"></param>
        /// <param name="FinishAdminId"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="State"></param>
        /// <param name="Language"></param>
        /// <param name="AddTime"></param>
        /// <param name="FinishTime"></param>
        /// <param name="Phone"></param>
        /// <param name="NameZH"></param>
        /// <param name="NameUG"></param>
        /// <param name="NameEN"></param>
        /// <param name="Remark"></param>

        private void AddToGrid(string editMark, string Id, string roomNo, Member member, double TotalPrice, double MemberPaidPrice, double PaidPrice, double TotalPaidPrice, double BorrowPrice, double KeepPrice, long FinishAdminId, long? StartTime, long? EndTime, long State, string Language, long AddTime, long? FinishTime, string Phone, string NameZH, string NameUG, string NameEN, string Remark, BillModel model = null)
        {
            string StartTimeStr = "";
            string EndTimeStr = "";
            string AddTimeStr = "";
            string FinishTimeStr = "";
            string MemberNameStr = "";
            string FinishAdminName = "";

            try
            {
                if (null != member)
                {
                    if (Resources.GetRes().MainLangIndex == 0)
                        MemberNameStr = member.MemberName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        MemberNameStr = member.MemberName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        MemberNameStr = member.MemberName2;
                }


                if (FinishAdminId > 0)
                {
                    Admin FinishAdmin = Resources.GetRes().Admins.Where(x => x.AdminId == FinishAdminId).FirstOrDefault();
                    if (null != FinishAdmin)
                    {
                        if (Resources.GetRes().MainLangIndex == 0)
                            FinishAdminName = FinishAdmin.AdminName0;
                        else if (Resources.GetRes().MainLangIndex == 1)
                            FinishAdminName = FinishAdmin.AdminName1;
                        else if (Resources.GetRes().MainLangIndex == 2)
                            FinishAdminName = FinishAdmin.AdminName2;
                    }
                }


                if (null != StartTime)
                    StartTimeStr = DateTime.ParseExact(StartTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                if (null != EndTime)
                    EndTimeStr = DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                    AddTimeStr = DateTime.ParseExact(AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                if (null != FinishTime)
                    FinishTimeStr = DateTime.ParseExact(FinishTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
            {
                krpdgList.Rows.Insert(0, editMark, Id, roomNo, MemberNameStr, TotalPrice.ToString(), MemberPaidPrice.ToString(), PaidPrice.ToString(), TotalPaidPrice.ToString(), BorrowPrice.ToString(), KeepPrice.ToString(), FinishAdminName, StartTimeStr, EndTimeStr, GetOrderState(State), Language, AddTimeStr, FinishTimeStr,  Phone, NameZH, NameUG, NameEN, Remark);
            }
            else
            {
                krpdgList.Rows.Add(editMark, Id, roomNo, MemberNameStr, TotalPrice.ToString(), MemberPaidPrice.ToString(), PaidPrice.ToString(), TotalPaidPrice.ToString(), BorrowPrice.ToString(), KeepPrice.ToString(), FinishAdminName, StartTimeStr, EndTimeStr, GetOrderState(State), Language, AddTimeStr, FinishTimeStr, Phone, NameZH, NameUG, NameEN, Remark);
                // 颜色提醒显示特殊的交易
                if (State == 2)
                {
                    SetColorToLastRowForState(Color.Gray);
                }


                if (KeepPrice > 0 || BorrowPrice < 0)
                {
                    Color keepPriceColor = Color.Empty;
                    Color borrowPriceColor = Color.Empty;

                    if (KeepPrice > 0)
                        keepPriceColor = Color.Blue;
                    if (BorrowPrice < 0)
                        borrowPriceColor = Color.Red;

                    SetColorToLastRow(borrowPriceColor, keepPriceColor);
                }

                
            }
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("IncomeManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        /// 查看数据
        /// </summary>
        private void Check()
        {
            if (krpcbOrderType.SelectedIndex == 0)
            {
                StartLoad(this, null);

                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        List<OrderDetail> orderdetails;
                        List<OrderPay> orderpays;
                        long orderId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrderId"].Value.ToString());
                        bool result = OperatesService.GetOperates().ServiceGetOrderDetail(orderId, out orderdetails, out orderpays);
                        this.BeginInvoke(new Action(() =>
                        {
                            if (result)
                            {
                                Order order = resultList.Where(x => x.Id == orderId).FirstOrDefault().GetOrder();
                                OrderDetailsWindow details = new OrderDetailsWindow(order, orderdetails, orderpays);
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
            else
            {
                StartLoad(this, null);

                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        List<TakeoutDetail> takeoutdetails;
                        List<TakeoutPay> takeoutpays;
                        long takeoutId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrderId"].Value.ToString());
                        bool result = OperatesService.GetOperates().ServiceGetTakeoutDetail(takeoutId, out takeoutdetails, out takeoutpays);
                        this.BeginInvoke(new Action(() =>
                        {
                            if (result)
                            {
                                Takeout takeout = resultList.Where(x => x.Id == takeoutId).FirstOrDefault().GetTakeout();
                                TakeoutDetailsWindow details = new TakeoutDetailsWindow(takeout, takeoutdetails, takeoutpays);
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
                    kryptonContextMenuItemShow.Enabled = false;
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

                    kryptonContextMenuItemShow.Enabled = true;
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
               
                //当前选中的可以删除
                if (e.KeyCode == Keys.L)
                {
                    if (krpdgList.SelectedRows.Count > 0)
                        Check();
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
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;
       

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
        /// 获取订单语言状态
        /// </summary>
        /// <param name="orderStateNo"></param>
        /// <returns></returns>
        private string GetLanguage(long languageNo)
        {
            return Resources.GetRes().GetLangByLangIndex((int)languageNo).LangName;
        }

        private void SetColorToLastRow(Color borrowPriceColor, Color keepPriceColor)
        {
            krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmBorrowPrice"].Style.ForeColor = krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmBorrowPrice"].Style.SelectionForeColor = borrowPriceColor;
            krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmKeepPrice"].Style.ForeColor = krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmKeepPrice"].Style.SelectionForeColor = keepPriceColor;
        }

        private void SetColorToLastRowForState(Color NewColor)
        {
            for (int j = 0; j < krpdgList.Rows[krpdgList.Rows.Count - 1].Cells.Count; j++)
            {
                krpdgList.Rows[krpdgList.Rows.Count - 1].Cells[j].Style.ForeColor = krpdgList.Rows[krpdgList.Rows.Count - 1].Cells[j].Style.SelectionForeColor = NewColor;
            }
        }


        /// <summary>
        /// 双击打开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && krpdgList.SelectedRows.Count > 0)
            {
                DataGridView.HitTestInfo hit = krpdgList.HitTest(e.X, e.Y);
                if (hit.Type == DataGridViewHitTestType.ColumnHeader)
                    return;
                Check();

            }
        }


        /// <summary>
        /// 回车确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbSearch_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// 切换账单类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbOrderType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //设定页面数据
            ResetPage();
            krpdgList.Rows.Clear();
            
            if (null != this.resultList)
                this.resultList.Clear();


            if (krpcbOrderType.SelectedIndex == 0)
            {
                krpcbMultipleLanguage.Visible = false;
                krpcmPhone.Visible = false;



                krpcmRoomNo.Visible = true;
                krpcmStartTime.Visible = true;
                krpcmEndTime.Visible = true;



                krpcmName0.Visible = false;
                krpcmName1.Visible = false;
                krpcmName2.Visible = false;

                krpcmState.Visible = true;

            }
            else
            {
                krpcbMultipleLanguage.Visible = true;
                krpcbMultipleLanguage_CheckedChanged(null, null);
                krpcmPhone.Visible = true;



                krpcmRoomNo.Visible = false;
                krpcmStartTime.Visible = false;
                krpcmEndTime.Visible = false;

                krpcmState.Visible = false;
            }
        }


        /// <summary>
        /// 切换完整显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbIsDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbIsDisplayAll.Checked)
            {
                krpcmName0.Visible = false;
                krpcmName1.Visible = false;
                krpcmName2.Visible = false;

                if (Resources.GetRes().MainLangIndex == 0)
                {
                    krpcmName0.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    krpcmName1.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    krpcmName2.Visible = true;
                }
            }
            else
            {
                krpcmName0.Visible = true;
                krpcmName1.Visible = true;
                krpcmName2.Visible = true;

            }
        }

        /// <summary>
        /// 多语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                krpcmName0.Visible = false;
                krpcmName1.Visible = false;
                krpcmName2.Visible = false;


                if (Resources.GetRes().MainLangIndex == 0)
                {
                    krpcmName0.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    krpcmName1.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    krpcmName2.Visible = true;
                }
            }
            else
            {
                krpcmName0.Visible = true;
                krpcmName1.Visible = true;
                krpcmName2.Visible = true;



            }
        }
        
    }
}
