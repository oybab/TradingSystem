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
using Oybab.ServicePC.DialogWindow;
using Oybab.Report.Model;
using Oybab.Res.View.Models;
using Oybab.Report.StatisticsForm;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class FinanceLogWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Log> resultList = null;
        private TimeSpan TimeLimit = TimeSpan.FromDays(Resources.GetRes().ShorDay);
        public Member ReturnValue { get; private set; } //返回值

        public FinanceLogWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();

            new CustomTooltip(this.krpdgList);
            this.Text = Resources.GetRes().GetString("FinanceLog");
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
            krplBalancePay.Text = Resources.GetRes().GetString("BalancePay");

            krplAddTime.Text = Resources.GetRes().GetString("AddTime");

            krptbStartTime.Value =  DateTime.Now.AddDays(-1);
            krptbEndTime.Value = DateTime.Now.AddMinutes(5);


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }
            
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.FinanceLog.ico"));

            //初始化
            Init();

            // 防止没搜索就增加完时加入时报错
            this.resultList = new List<Log>();


            krpcbBalancePay.Items.Clear();
            this.Shown += (sender, args) =>
            {
                if (!IsInitBalance)
                {
                    IsInitBalance = true;
                    InitBalance();
                }
            };

        }

        private bool IsInitBalance = false;

        /// <summary>
        /// 初始化
        /// </summary>
        internal void InitBalance()
        {
            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {

                    List<Log> logs;
                    List<Balance> Balances;

                    bool result = OperatesService.GetOperates().ServiceGetLog(1, -1, 0, 0, out Balances, out logs);

                    //如果验证成功
                    //修改成功
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            // 更新支付余额
                            ReloadBalanceList(Balances);
                        }
                    }));

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }));
                }
                StopLoad(this, null);


            });
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
            krpcmLogId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmChangePrice.HeaderText = Resources.GetRes().GetString("ChangePrice");
            krpcmBalancePrice.HeaderText = Resources.GetRes().GetString("BalancePrice");
            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcmOperateName.HeaderText = Resources.GetRes().GetString("OperateName");
        }



        /// <summary>
        /// 刷新支付列表
        /// </summary>
        private void ReloadBalanceList(List<Balance> balances)
        {
            int SelectedIndex = krpcbBalancePay.SelectedIndex;

            krpcbBalancePay.Items.Clear();
            if (null != balances && balances.Count > 0)
            {

                foreach (var item in balances.OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId))
                {

                    Balance currentBalance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.BalanceId).FirstOrDefault();

                    string BalanceName = "";


                    BalanceName += Resources.GetRes().PrintInfo.PriceSymbol + item.BalancePrice + "";


                    BalanceName += "  ";
                    if (Resources.GetRes().MainLangIndex == 0)
                        BalanceName += currentBalance.BalanceName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        BalanceName += currentBalance.BalanceName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        BalanceName += currentBalance.BalanceName2;

                    krpcbBalancePay.Items.Add(new BalanceItemModel() { Text = BalanceName, Balance = item, IsBalance = true, IsChange = true });


                }

                krpcbBalancePay.SelectedIndex = SelectedIndex;
                if (krpcbBalancePay.SelectedIndex == -1)
                    krpcbBalancePay.SelectedIndex = 0;
            }
        }


        private Dictionary<long, Balance> LastBalanceList = new Dictionary<long, Balance>();
        /// <summary>
        /// 获取当时余额列表
        /// </summary>
        /// <param name="balances"></param>
        /// <returns></returns>
        private void ReturnBalanceListInfo(List<Balance> balances, out string ChangePrice, out string BalancePrice)
        {
            ChangePrice = "";
            BalancePrice = "";
            bool IsFirstChange = false;

            if (LastBalanceList.Count == 0)
                IsFirstChange = true;
            
            if (null != balances && balances.Count > 0)
            {
                foreach (var item in balances)
                {
                    string differentPrice = null;
                    Balance currentBalance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.BalanceId).FirstOrDefault();

                    if (LastBalanceList.ContainsKey(item.BalanceId))
                    {
                        if (LastBalanceList[item.BalanceId].BalancePrice == item.BalancePrice)
                        {
                            continue;
                        }
                        else
                        {
                            differentPrice = Math.Round(item.BalancePrice - LastBalanceList[item.BalanceId].BalancePrice, 2).ToString();
                            LastBalanceList[item.BalanceId] = item;
                        }
                    }
                    else
                    {
                        LastBalanceList.Add(item.BalanceId, item);
                    }

                    if (null == differentPrice)
                        differentPrice = "0";

                    if (double.Parse(differentPrice) > 0)
                        differentPrice = "+" + differentPrice;


                    if (Resources.GetRes().MainLangIndex == 0)
                    {
                        BalancePrice += currentBalance.BalanceName0;
                        ChangePrice += currentBalance.BalanceName0;
                    }
                    else if (Resources.GetRes().MainLangIndex == 1)
                    {
                        BalancePrice += currentBalance.BalanceName1;
                        ChangePrice += currentBalance.BalanceName1; 
                    }
                    else if (Resources.GetRes().MainLangIndex == 2)
                    {
                        BalancePrice += currentBalance.BalanceName2;
                        ChangePrice += currentBalance.BalanceName2;
                    }

                    BalancePrice += "  ";
                    ChangePrice += "  ";

                    BalancePrice += Resources.GetRes().PrintInfo.PriceSymbol + item.BalancePrice;
                    ChangePrice += Resources.GetRes().PrintInfo.PriceSymbol +  differentPrice;



                    BalancePrice += "  ";
                    ChangePrice += "  ";

                }
            }

            if (IsFirstChange)
                ChangePrice = "-";
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

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<Log> logs;
                    List<Balance> Balances;


                    bool result = OperatesService.GetOperates().ServiceGetLog(2, 1, addTimeStart, addTimeEnd, out Balances, out logs);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {

                            // 更新支付余额
                            ReloadBalanceList(Balances);

                            this.resultList = logs.OrderBy(x=> x.AddTime).ThenBy(x => x.LogId).ToList();
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
            LastBalanceList.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid("", item.LogId.ToString(), item.OperateId.ToString(), item.OperateName, item.Balance, item.AddTime);
            }

            if (krpdgList.Rows.Count > 0)
                krpdgList.Rows[0].Selected = true;
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="OperateName"></param>
        /// <param name="ChangePrice"></param>
        /// <param name="BalancePrice"></param>
        /// <param name="CardChangePrice"></param>
        /// <param name="CardBalancePrice"></param>
        /// <param name="Other"></param>
        /// <param name="AddTime"></param>
        private void AddToGrid(string editMark, string Id, string OperateId, string OperateName, string Balance, long AddTime)
        {
            string AddTimeStr = "";
            string OperateNameStr = "";
            string ChangePriceStr = "";
            string BalancePriceStr = "";


            try
            {
                OperateNameStr = GetOperateName(OperateName);

                OperateNameStr = OperateNameStr  + " - " + OperateId;

                ReturnBalanceListInfo(Balance.DeserializeObject<List<Balance>>(), out ChangePriceStr, out BalancePriceStr);



                AddTimeStr = DateTime.ParseExact(AddTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, OperateNameStr, ChangePriceStr, BalancePriceStr, AddTimeStr);
            else
            {
                krpdgList.Rows.Insert(0, editMark, Id, OperateNameStr, ChangePriceStr, BalancePriceStr, AddTimeStr);
                krpdgList.Rows[0].ReadOnly = true;
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
            AddToGrid("*", "-1", "-1", "", "", long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            krpdgList.FirstDisplayedScrollingRowIndex = 0;
        }

        /// <summary>
        /// 保存新增或改动的数据
        /// </summary>
        private void Save()
        {
            
        }


        /// <summary>
        /// 删除数据
        /// </summary>
        private void Delete()
        {
            
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
                }
                kryptonContextMenuItemAdd.Enabled = false;
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
                    //Add();
                }
                //当前选中的行保存
                else if (e.KeyCode == Keys.S)
                {

                }
                //当前选中的可以删除
                if (e.KeyCode == Keys.D)
                {

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
        /// 返回操作名称
        /// </summary>
        /// <param name="operate"></param>
        /// <returns></returns>
        private string GetOperateName(string operate)
        {
            if (operate.StartsWith("Order#"))
                return Resources.GetRes().GetString("Income");
            else if (operate.StartsWith("OrderDetail#"))
                return Resources.GetRes().GetString("Income");
            else if (operate.StartsWith("Takeout#"))
                return Resources.GetRes().GetString("Income");
            else if (operate.StartsWith("Import#"))
                return Resources.GetRes().GetString("Expenditure");
            else if (operate.StartsWith("Return#"))
                return Resources.GetRes().GetString("Return");
            else if (operate.StartsWith("AdminPay#"))
                return Resources.GetRes().GetString("AdminPay");
            else if (operate.StartsWith("MemberPay#"))
                return Resources.GetRes().GetString("MemberPay");
            else if (operate.StartsWith("SupplierPay#"))
                return Resources.GetRes().GetString("SupplierPay");
            else if (operate.StartsWith("BalancePay#"))
                return Resources.GetRes().GetString("BalancePay");
            else if (operate.StartsWith("Member#"))
                return Resources.GetRes().GetString("Member");
            else if (operate.StartsWith("Supplier#"))
                return Resources.GetRes().GetString("Supplier");
            else
                return Resources.GetRes().GetString("Unknown");
        }



        private void OpenStatistick()
        {
            List<RecordTime> records = new List<RecordTime>();

            foreach (var item in resultList)
            {
                double price = 0;
             

                DateTime AddTime = DateTime.ParseExact(item.AddTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);

                records.Add(new RecordTime(item.LogId, AddTime, price));
            }

            StatisticModel statisticModel = new StatisticModel();
            statisticModel.Title = this.Text;
            statisticModel.Font = new System.Drawing.Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            statisticModel.EnableAntialiasing = Resources.GetRes().GetString("EnableAntialiasing") == "1";

            //StatisticXYDiagramWindow window = new StatisticXYDiagramWindow(records, statisticModel);
            //window.ShowDialog(this);
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (resultList.Count > 1)
            {
                OpenStatistick();
            }
        }
       
        
    }
}
