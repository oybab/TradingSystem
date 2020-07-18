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
using Oybab.ServicePC.Tools;
using Oybab.Res.View.Models;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class MemberPayWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<MemberPay> resultList = null;
        private List<CommonPayModel> commonPayList = null;
        private Member Member = new Member();
        private TimeSpan TimeLimit = TimeSpan.FromDays(Resources.GetRes().ShorDay);
        public Member ReturnValue { get; private set; } //返回值
        private bool OnlyIncrease = false;

        public MemberPayWindow(Member Member, bool OnlyIncrease = false)
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            this.Member = Member;
            this.OnlyIncrease = OnlyIncrease;

            string name = "";
            if (Resources.GetRes().MainLangIndex == 0)
                name = Member.MemberName0;
            else if (Resources.GetRes().MainLangIndex == 1)
                name = Member.MemberName1;
            else if (Resources.GetRes().MainLangIndex == 2)
                name = Member.MemberName2;

            new CustomTooltip(this.krpdgList);
            this.Text = Resources.GetRes().GetString("MemberPay") + " - " + name;
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
            krptbEndTime.Value = DateTime.Now.AddMinutes(5);

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.MemberPay.ico"));

            //初始化
            Init();

            // 防止没搜索就增加完时加入时报错
            this.resultList = new List<MemberPay>();
            this.commonPayList = new List<CommonPayModel>();
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
            krpcmMemberPayId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmPrice.HeaderText = Resources.GetRes().GetString("Price");
            krpcmBalanceType.HeaderText = Resources.GetRes().GetString("BalanceName");
            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcmRemark.HeaderText = Resources.GetRes().GetString("Remark");
            krpcmOperateName.HeaderText = Resources.GetRes().GetString("OperateName");

            ReloadBalances();

            krpcbPayType.Items.AddRange(new string[] { Resources.GetRes().GetString("BillTypeInner"), Resources.GetRes().GetString("BillTypeOuter"), Resources.GetRes().GetString("All") });
            krpcbPayType.SelectedIndex = 0;
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

            if ((endDateTime - startDateTime).TotalMinutes <= 0 || !((endDateTime - startDateTime).TotalMinutes <= TimeLimit.TotalMinutes))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("TimeLimit"), TimeLimit.TotalDays), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            long addTimeStart = long.Parse(startDateTime.ToString("yyyyMMddHHmmss"));
            long addTimeEnd = long.Parse(endDateTime.ToString("yyyyMMddHHmmss"));
            long balanceType = krpcbPayType.SelectedIndex;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<MemberPay> memberPays;
                    List<OrderPay> orderPays;
                    List<TakeoutPay> takeoutPays;

                    bool result = OperatesService.GetOperates().ServiceGetMemberPay(balanceType, Member.MemberId, addTimeStart, addTimeEnd, -1, out memberPays, out orderPays, out takeoutPays);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            List<CommonPayModel> commonPays = new List<CommonPayModel>();

                            commonPays.AddRange(orderPays.Select(x => new CommonPayModel(x)));
                            commonPays.AddRange(takeoutPays.Select(x => new CommonPayModel(x)));
                            commonPays.AddRange(memberPays.Select(x => new CommonPayModel(x)));

                            this.resultList = memberPays.OrderByDescending(x => x.MemberPayId).ToList();
                            this.commonPayList = commonPays.OrderByDescending(x => x.AddTime).ToList();
                            //设定页面数据
                            ResetPage();
                            if (commonPayList.Count() > 0)
                            {
                                //resultList.Reverse();
                                AllPage = (int)((commonPayList.Count() - 1 + ListCount) / ListCount);
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
            var currentResult = commonPayList.Skip((CurrentPage - 1) * ListCount).Take(ListCount);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid("", item.PayId.ToString(), item.Price, GetBalanceType(item.BalanceId), item.Remark, item.AddTime, item);
            }
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="Price"></param>
        /// <param name="IsPayByCard"></param>
        /// <param name="Remark"></param>
        /// <param name="AddTime"></param>
        private void AddToGrid(string editMark, string Id, double Price, string BalanceName, string Remark, long AddTime, CommonPayModel model)
        {
            string AddTimeStr = "";
            string OperateNameStr = "";

            try
            {
                AddTimeStr = DateTime.ParseExact(AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");

                if (null != model)
                    OperateNameStr = GetOperateName(model);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, OperateNameStr, GetOperateSymbal(model, Price).ToString(), BalanceName, Remark, AddTimeStr);
            else
            {
                krpdgList.Rows.Add(editMark, Id, OperateNameStr, GetOperateSymbal(model, Price).ToString(), BalanceName, Remark, AddTimeStr);
                krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmPrice"].ReadOnly = true;
                krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmBalanceType"].ReadOnly = true;
                krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmRemark"].ReadOnly = true;
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("MemberPay"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        /// 新增数据
        /// </summary>
        private void Add()
        {
            OpenPageTo(1);

            string balanceName = null;
            Balance balance = Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).FirstOrDefault();
            if (null != balance)
                balanceName = GetBalanceType(balance.BalanceId);

            AddToGrid("*", "-1", 0, balanceName, "", long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), null);
            krpdgList.FirstDisplayedScrollingRowIndex = 0;
        }

        /// <summary>
        /// 保存新增或改动的数据
        /// </summary>
        private void Save()
        {
            if (null != krpdgList.SelectedRows[0])
            {
                //如果是插入
                if (krpdgList.SelectedRows[0].Cells["krpcmMemberPayId"].Value.ToString().Equals("-1"))
                {
                    MemberPay memberpay = new MemberPay();
                    try
                    {
                        memberpay.MemberPayId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmMemberPayId"].Value.ToString());
                        memberpay.Price = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrice"].Value.ToString()), 2);
                        memberpay.BalanceId = GetBalanceTypeId(krpdgList.SelectedRows[0].Cells["krpcmBalanceType"].Value.ToString());
                        memberpay.Remark = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmRemark"].Value.ToString());
                        memberpay.MemberId = this.Member.MemberId;


                        if (this.Member.IsAllowBorrow == 0 && this.Member.BalancePrice + memberpay.Price < 0)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("MemberBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning); 
                            return;
                        }

                        if (OnlyIncrease && memberpay.Price < 0)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("MemberBalanceNotEnough"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("SaveFailt"));
                        return;
                    }

                    StartLoad(this, null);

                    Task.Factory.StartNew(() =>
                    {
                        ResultModel result = new ResultModel();
                        double originalBalancePrice = Member.BalancePrice;
                        try
                        {
                            Member.BalancePrice = Member.BalancePrice + memberpay.Price;

                            Member newMember;
                            MemberPay newMemberPay;
                            result = OperatesService.GetOperates().ServiceAddMemberPay(Member, memberpay, out newMember, out newMemberPay);
                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmMemberPayId"].Value = newMemberPay.MemberPayId;
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    //resultList.Insert(0, model);
                                    krpdgList.SelectedRows[0].Cells["krpcmAddTime"].Value = DateTime.ParseExact(newMemberPay.AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm");
                                    this.Member = newMember;
                                    this.ReturnValue = newMember;
                                    this.resultList.Insert(0, newMemberPay);
                                    this.commonPayList.Insert(0, new CommonPayModel(newMemberPay));

                                    krpdgList.SelectedRows[0].Cells["krpcmPrice"].ReadOnly = true;
                                    krpdgList.SelectedRows[0].Cells["krpcmBalanceType"].ReadOnly = true;
                                    krpdgList.SelectedRows[0].Cells["krpcmRemark"].ReadOnly = true;
                                    krptbEndTime.Text = DateTime.Now.AddMinutes(5).ToString("yyyyMMddHHmm");
                                }
                                else
                                {
                                    if (result.IsDataHasRefrence)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else if (result.UpdateModel)
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUnSame"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }
                                    else
                                    {
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                    if (result.Result)
                                        Member.BalancePrice = originalBalancePrice;

                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

                                }), false, Resources.GetRes().GetString("SaveFailt"));
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
                //如果是编辑
                else
                {
                    // 没有编辑, 也不能编辑
                }
            }
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmMemberPayId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
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

            StartLoad(this, null);


            Task.Factory.StartNew(() =>
            {
                MemberPay memberpay = resultList.Where(x => x.MemberPayId == Id).FirstOrDefault();
                double originalBalancePrice = Member.BalancePrice;
                ResultModel result = new ResultModel();
                try
                {
                    Member.BalancePrice = Member.BalancePrice - memberpay.Price;

                    Member newMember;

                    
                    result = OperatesService.GetOperates().ServiceDelMemberPay(Member, memberpay, out newMember);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            
                            MemberPay oldModel = resultList.Where(x => x.MemberPayId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);

                            CommonPayModel oldCommonModel = this.commonPayList.Where(x => x.PayId == Id && x.Type == "MemberPay").FirstOrDefault();
                            commonPayList.Remove(oldCommonModel);

                            this.ReturnValue = newMember;
                            this.Member = newMember;
                        }
                        else
                        {
                            if (result.IsDataHasRefrence)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            else if (result.UpdateModel)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUnSame"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            if (result.Result)
                                Member.BalancePrice = originalBalancePrice;

                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("DeleteFailt"));
                    }));
                }
                StopLoad(this, null);
            });
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
                    kryptonContextMenuItemSave.Enabled = false;
                    kryptonContextMenuItemDelete.Enabled = false;
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
                    else
                    {
                        ExitEditMode();

                        krpdgList.Rows[e.RowIndex].Selected = true;
                        if (krpdgList.SelectedRows.Count == 0)
                            return;
                    }


                    //如果有改动才可以保存
                    if (krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        kryptonContextMenuItemSave.Enabled = true;

                        // 只有还没保存的才能删除
                        if (krpdgList.SelectedRows[0].Cells["krpcmMemberPayId"].Value.ToString() == "-1")
                            kryptonContextMenuItemDelete.Enabled = true;
                    }
                    //kryptonContextMenuItemDelete.Enabled = true;
                }
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
                    Add();
                }
                //当前选中的行保存
                else if (e.KeyCode == Keys.S)
                {
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        ExitEditMode();
                        Save();
                    }
                        
                }
                //当前选中的可以删除
                if (e.KeyCode == Keys.D)
                {
                    // 只有还没保存的才能删除
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && krpdgList.SelectedRows[0].Cells["krpcmMemberPayId"].Value.ToString() == "-1")
                        Delete();
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
        /// 返回值或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValueOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return value.Trim();
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
        /// 获取余额类型编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private long GetBalanceTypeId(string balanceName)
        {
            Balance balance = null;
            if (Resources.GetRes().MainLangIndex == 0)
                balance = Resources.GetRes().Balances.Where(x => x.BalanceName0 == balanceName).FirstOrDefault();
            else if (Resources.GetRes().MainLangIndex == 1)
                balance = Resources.GetRes().Balances.Where(x => x.BalanceName1 == balanceName).FirstOrDefault();
            else if (Resources.GetRes().MainLangIndex == 2)
                balance = Resources.GetRes().Balances.Where(x => x.BalanceName2 == balanceName).FirstOrDefault();

            if (null == balance)
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));

            return balance.BalanceId;
        }

        /// <summary>
        /// 获取余额类型
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetBalanceType(long? balanceId)
        {
            if (null == balanceId)
                return "";

            Balance balance = null;

            balance = Resources.GetRes().Balances.Where(x => x.BalanceId == balanceId).FirstOrDefault();

            if (null == balance)
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));


            if (Resources.GetRes().MainLangIndex == 0)
                return balance.BalanceName0;
            else if (Resources.GetRes().MainLangIndex == 1)
                return balance.BalanceName1;
            else if (Resources.GetRes().MainLangIndex == 2)
                return balance.BalanceName2;

            return null;

        }


        /// <summary>
        /// 加载余额列表
        /// </summary>
        internal void ReloadBalances()
        {
            if (Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == 2).Count() > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmBalanceType.Items.AddRange(Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).Select(x => x.BalanceName0).ToArray());
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmBalanceType.Items.AddRange(Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).Select(x => x.BalanceName1).ToArray());
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmBalanceType.Items.AddRange(Resources.GetRes().Balances.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).Select(x => x.BalanceName2).ToArray());
            }

        }



        /// <summary>
        /// 返回金额正反
        /// </summary>
        /// <param name="model"></param>
        /// <param name="price"></param>
        /// <returns></returns>
        private double GetOperateSymbal(CommonPayModel model, double price)
        {
            if (null != model && (model.Type == "OrderPay" || model.Type == "TakeoutPay"))
                return -price;
            else
                return price;
        }

        /// <summary>
        /// 返回操作名称
        /// </summary>
        /// <param name="operate"></param>
        /// <returns></returns>
        private string GetOperateName(CommonPayModel model)
        {
            string OperateName = "";



            if (model.Type == "OrderPay")
                OperateName += Resources.GetRes().GetString("Income");
            else if (model.Type == "TakeoutPay")
                OperateName += Resources.GetRes().GetString("Income");
            else if (model.Type == "ImportPay")
                OperateName += Resources.GetRes().GetString("Expenditure");
            else if (model.Type == "AdminPay")
                OperateName += Resources.GetRes().GetString("AdminPay");
            else if (model.Type == "MemberPay")
                OperateName += ""; //Resources.GetRes().GetString("MemberPay");
            else if (model.Type == "SupplierPay")
                OperateName += Resources.GetRes().GetString("SupplierPay");
            else if (model.Type == "BalancePay")
                OperateName += Resources.GetRes().GetString("BalancePay");
            else
                OperateName += Resources.GetRes().GetString("Unknown");

            OperateName += "  ";

            OperateName = OperateName + (model.Type == "MemberPay" ? "" : model.ParentId + " - ");

            OperateName += "  ";

            OperateName += Resources.GetRes().PrintInfo.PriceSymbol + (model.ParentBalancePrice ?? model.BalancePrice);


            return OperateName;
        }

        private void krpcbPayType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (krpcbPayType.SelectedIndex > 0)
                krpcmOperateName.Visible = true;
            else
                krpcmOperateName.Visible = false;
        }
    }
}
