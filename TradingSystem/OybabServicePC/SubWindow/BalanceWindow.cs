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

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class BalanceWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Balance> resultList = null;

        public BalanceWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificationBalance += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.BalanceId == value.BalanceId)) krpdgList.Enabled = false; })); };

            new CustomTooltip(this.krpdgList);
            this.ControlBox = false;
            
            krptBalanceName.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("BalanceManager");
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
            krplBalanceName.Text = Resources.GetRes().GetString("BalanceName");



            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptBalanceName.Location = new Point(krptBalanceName.Location.X, krptBalanceName.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });
            // 显示余额支付
            LoadContextMenu(kryptonContextMenuItemPay, Resources.GetRes().GetString("BalancePay"), Resources.GetRes().GetString("BalancePayDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.BalancePay.png")), (sender, e) => { ShowBalancePay(); });
            // 显示转账
            LoadContextMenu(kryptonContextMenuItemTransfer, Resources.GetRes().GetString("BalanceTransfer"), Resources.GetRes().GetString("BalanceTransferDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ReplaceRoom.png")), (sender, e) => { BalanceTransfer(); });


            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.BalanceManager.ico"));

            //初始化
            Init();

            // 实例化一下, 免得新增时出现问题
            resultList = new List<Balance>();

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
            krpcmBalanceId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmBalanceName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("BalanceName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmBalanceName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("BalanceName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmBalanceName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("BalanceName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmAccountBank.HeaderText = Resources.GetRes().GetString("AccountBank");
            krpcmAccountName.HeaderText = Resources.GetRes().GetString("AccountName");
            krpcmAccountNo.HeaderText = Resources.GetRes().GetString("AccountNo");
            krpcmBalancePrice.HeaderText = Resources.GetRes().GetString("BalancePrice");
            krpcmIsBind.HeaderText = Resources.GetRes().GetString("IsBind");
            krpcmOrder.HeaderText = Resources.GetRes().GetString("Order");
            krpcmHideType.HeaderText = Resources.GetRes().GetString("DisplayType");
            krpcmRemark.HeaderText = Resources.GetRes().GetString("Remark");

            ReloadBalanceTextbox();

            krpcmHideType.Items.AddRange(new string[] { Resources.GetRes().GetString("Display"), Resources.GetRes().GetString("Hide"), Resources.GetRes().GetString("Income"), Resources.GetRes().GetString("Expenditure") });


            krpcbIsDisplayAll.Text = Resources.GetRes().GetString("IsDisplayAll");
            krpcbMultipleLanguage.Text = Resources.GetRes().GetString("MultiLanguage");
            krpcbIsDisplayAll_CheckedChanged(null, null);
            krpcbMultipleLanguage_CheckedChanged(null, null);
        }


        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSearch_Click(object sender, EventArgs e)
        {

            if (!krpdgList.Enabled)
            {
                krpdgList.Enabled = true;
                krpdgList.Rows.Clear();
            }


            //为未保存数据而忽略当前操作
            if (!IgnoreOperateForSave())
                return;


 
            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<Balance> balances;
                    
                    bool result = OperatesService.GetOperates().ServiceGetBalances(0, out balances);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            this.resultList = balances.OrderByDescending(x => x.Order).ThenByDescending(x => x.BalanceId).ToList();


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
                AddToGrid("", item.BalanceId.ToString(), item.BalanceName0, item.BalanceName1, item.BalanceName2, item.AccountBank ?? "", item.AccountName ?? "", item.AccountNo ?? "", item.BalancePrice, item.IsBind, item.Order, GetHideType(item.HideType), item.Remark ?? "", 0);
            }

            SetColor();
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="balanceName0"></param>
        /// <param name="balanceName1"></param>
        /// <param name="balanceName2"></param>
        /// <param name="accountBank"></param>
        /// <param name="accountName"></param>
        /// <param name="accountNo"></param>
        /// <param name="balancePrice"></param>
        /// <param name="isBind"></param>
        /// <param name="Order"></param>
        /// <param name="HideType"></param>
        /// <param name="Remark"></param>
        /// <param name="mode"></param>
        private void AddToGrid(string editMark, string Id, string balanceName0, string balanceName1, string balanceName2, string accountBank, string accountName, string accountNo, double balancePrice, long isBind, long Order, string HideType, string Remark, int mode)
        {

            if (mode == 0)
            {
                if (editMark == "*")
                    krpdgList.Rows.Insert(0, editMark, Id, balanceName0, balanceName1, balanceName2, accountBank, accountName, accountNo, balancePrice.ToString(), isBind.ToString(), Order.ToString(), HideType, Remark);
                else
                    krpdgList.Rows.Add(editMark, Id, balanceName0, balanceName1, balanceName2, accountBank, accountName, accountNo, balancePrice.ToString(), isBind.ToString(), Order.ToString(), HideType, Remark);

            }
            else if (mode == 2)
            {

                // 找到并替换
                int rowIndex = -1;
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {

                    //只有有改动才可以继续
                    if (krpdgList.Rows[i].Cells["krpcmBalanceId"].Value.ToString() == Id)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                if (rowIndex != -1)
                {
                    this.krpdgList.Invoke(new Action(() =>
                    {
                        krpdgList.Rows.RemoveAt(rowIndex);
                        krpdgList.Rows.Insert(rowIndex, editMark, Id, balanceName0, balanceName1, balanceName2, accountBank, accountName, accountNo, balancePrice.ToString(), isBind.ToString(), Order.ToString(), HideType, Remark);
                        krpdgList.Rows[rowIndex].Selected = true;
                    }));
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("BalanceManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        private void Add(){
            OpenPageTo(1);
            AddToGrid("*", "-1", "", "", "", "", "", "", 0, 0, 0, GetHideType(0), "", 0);// long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"))
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
                if (krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString().Equals("-1"))
                {
                    Balance model = new Balance();
                    try
                    {

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmBalanceName0"], krpdgList.SelectedRows[0].Cells["krpcmBalanceName1"], krpdgList.SelectedRows[0].Cells["krpcmBalanceName2"], true, false, krpcbMultipleLanguage.Checked);


                        model.BalanceId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString());
                        model.BalanceName0 = krpdgList.SelectedRows[0].Cells["krpcmBalanceName0"].Value.ToString().Trim();
                        model.BalanceName1 = krpdgList.SelectedRows[0].Cells["krpcmBalanceName1"].Value.ToString().Trim();
                        model.BalanceName2 = krpdgList.SelectedRows[0].Cells["krpcmBalanceName2"].Value.ToString().Trim();
                        
                        model.AccountBank = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAccountBank"].Value.ToString());
                        model.AccountName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAccountName"].Value.ToString());
                        model.AccountNo = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAccountNo"].Value.ToString());
                       
                        model.IsBind = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsBind"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());
                        model.Remark = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmRemark"].Value.ToString());

                        model.BalanceType = 1;

                        //判断空
                        if (string.IsNullOrWhiteSpace(model.BalanceName0) || string.IsNullOrWhiteSpace(model.BalanceName1) || string.IsNullOrWhiteSpace(model.BalanceName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                        //判断是否已存在
                        if (resultList.Where(x => (x.BalanceName0.Equals(model.BalanceName0, StringComparison.OrdinalIgnoreCase) || x.BalanceName1.Equals(model.BalanceName1, StringComparison.OrdinalIgnoreCase) || x.BalanceName2.Equals(model.BalanceName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("BalanceName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        try
                        {
                            bool IsBalanceExists = false;
                            bool result = OperatesService.GetOperates().ServiceAddBalance(model, out IsBalanceExists);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value = model.BalanceId;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);

                                    SetColor(true);
                                    ReloadBalanceTextbox(true);

                                    Resources.GetRes().Balances.Add(model);
                                }
                                else
                                {
                                    if (IsBalanceExists)
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("BalanceName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    else
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                }), false, Resources.GetRes().GetString("SaveFailt"));
                            }));
                        }
                        StopLoad(this, null);
                    });
                }
                //如果是编辑
                else
                {
                    Balance model = new Balance();
                    try
                    {

                        model.BalanceId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString());

                        model = resultList.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault().FastCopy();

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmBalanceName0"], krpdgList.SelectedRows[0].Cells["krpcmBalanceName1"], krpdgList.SelectedRows[0].Cells["krpcmBalanceName2"], false, Ext.AllSame(model.BalanceName0, model.BalanceName1, model.BalanceName2), krpcbMultipleLanguage.Checked);

                        model.BalanceId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString());
                        model.BalanceName0 = krpdgList.SelectedRows[0].Cells["krpcmBalanceName0"].Value.ToString().Trim();
                        model.BalanceName1 = krpdgList.SelectedRows[0].Cells["krpcmBalanceName1"].Value.ToString().Trim();
                        model.BalanceName2 = krpdgList.SelectedRows[0].Cells["krpcmBalanceName2"].Value.ToString().Trim();

                        model.AccountBank = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAccountBank"].Value.ToString());
                        model.AccountName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAccountName"].Value.ToString());
                        model.AccountNo = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmAccountNo"].Value.ToString());

                        model.IsBind = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsBind"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());
                        model.Remark = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmRemark"].Value.ToString());


                        //判断空
                        if (string.IsNullOrWhiteSpace(model.BalanceName0) || string.IsNullOrWhiteSpace(model.BalanceName1) || string.IsNullOrWhiteSpace(model.BalanceName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                        //判断是否已存在
                        if (resultList.Where(x => model.BalanceId != x.BalanceId && (x.BalanceName0.Equals(model.BalanceName0, StringComparison.OrdinalIgnoreCase) || x.BalanceName1.Equals(model.BalanceName1, StringComparison.OrdinalIgnoreCase) || x.BalanceName2.Equals(model.BalanceName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("BalanceName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        try
                        {
                            bool IsBalanceExists = false;

                            ResultModel result = OperatesService.GetOperates().ServiceEditBalance(model, out IsBalanceExists);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    Balance oldModel = resultList.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                   
                                    SetColor(true);
                                    ReloadBalanceTextbox(true);

                                    Resources.GetRes().Balances.Remove(Resources.GetRes().Balances.Where(x => x.BalanceId == model.BalanceId).FirstOrDefault());
                                    Resources.GetRes().Balances.Add(model);
                                }
                                else
                                {
                                    if (IsBalanceExists)
                                        KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("BalanceName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    else
                                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveFailt"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                                }), false, Resources.GetRes().GetString("SaveFailt"));
                            }));
                        }
                        StopLoad(this, null);
                    });
                }
            }
        }

        /// <summary>
        /// 转账
        /// </summary>
        private void BalanceTransfer()
        {
            long Id = -1;
            Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString());
            BalanceTransferWindow pay = new BalanceTransferWindow(Id);
            pay.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            pay.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            pay.ShowDialog(this);
           
            if (pay.ReturnValue)
            {
                krpbSearch_Click(null, null);
            }
        }


            /// <summary>
            /// 显示会员支付详情
            /// </summary>
            private void ShowBalancePay()
        {
            long Id = -1;
            Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString());
            BalancePayWindow pay = new BalancePayWindow(resultList.Where(x => x.BalanceId == Id).FirstOrDefault());
            pay.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            pay.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            pay.ShowDialog(this);
            Balance item = pay.ReturnValue;
            if (null != item)
            {
                int no = resultList.FindIndex(x => x.BalanceId == Id);
                resultList.RemoveAt(no);
                resultList.Insert(no, item);

                AddToGrid("", item.BalanceId.ToString(), item.BalanceName0, item.BalanceName1, item.BalanceName2, item.AccountBank ?? "", item.AccountName ?? "", item.AccountNo ?? "", item.BalancePrice, item.IsBind, item.Order, GetHideType(item.HideType), item.Remark ?? "", 2);
                SetColor(true);
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
                }
                //否则先删除数据库
                

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
                    
                    ResultModel result = OperatesService.GetOperates().ServiceDelBalance(resultList.Where(x => x.BalanceId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Balance oldModel = resultList.Where(x => x.BalanceId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);

                            ReloadBalanceTextbox(true);
                        }
                        else
                        {
                            if (result.IsDataHasRefrence)
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    kryptonContextMenuItemPay.Enabled = false;
                    kryptonContextMenuItemTransfer.Enabled = false;
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


                    //如果有改动才可以保存
                    if (krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        kryptonContextMenuItemSave.Enabled = true;

                        // 只有还没保存的才能删除
                        if (krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString() == "-1")
                            kryptonContextMenuItemDelete.Enabled = true;
                    }

                    // 支付记录显示
                    if (krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.Equals("-1") || krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        kryptonContextMenuItemPay.Enabled = false;
                        kryptonContextMenuItemTransfer.Enabled = false;
                    }
                    else
                    {
                        kryptonContextMenuItemPay.Enabled = true;
                        
                        if (resultList.Count > 1)
                            kryptonContextMenuItemTransfer.Enabled = true;
                        else
                            kryptonContextMenuItemTransfer.Enabled = false;
                    }
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
                else if (e.KeyCode == Keys.D)
                {
                    // 只有还没保存的才能删除
                    if (krpdgList.SelectedRows.Count > 0 && krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.ToString() == "-1")
                        Delete();
                }
                
                // 显示支付记录
                else if (e.KeyCode == Keys.E && krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.Equals("-1") && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    ShowBalancePay();
                // 转账
                else if (e.KeyCode == Keys.T && resultList.Count > 1 && krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].Cells["krpcmBalanceId"].Value.Equals("-1") && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*"))
                    BalanceTransfer();

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
        /// 类型被修改
        /// </summary>
        public event EventHandler ChangeType;
        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;


        /// <summary>
        /// 重新加载产品类型搜索框
        /// </summary>
        private void ReloadBalanceTextbox(bool TrigChangeEvent = false)
        {
           

            if (TrigChangeEvent)
            {
                if (null != ChangeType)
                    ChangeType(null, null);
            }
        }


        
        



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
        /// 设置醒目颜色
        /// </summary>
        private void SetColor(bool IsOnlySelected = false)
        {
            // 当前时间
            long now = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

            if (IsOnlySelected)
            {
                try
                {
                    // 余额 小于0
                    double BalancePrice = double.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Value.ToString());
                    if (BalancePrice < 0)
                        krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Red;
                    else
                        krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Empty;
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            }
            else
            {
                //设置所有颜色
                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    try
                    {

                        if (krpdgList.Rows[i].Cells["krpcmBalanceId"].Value.ToString() == "1")
                            krpdgList.Rows[i].ReadOnly = true;
                        // 余额 小于0
                        double BalancePrice = double.Parse(krpdgList.Rows[i].Cells["krpcmBalancePrice"].Value.ToString());
                        if (BalancePrice < 0)
                            krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Red;
                        else
                            krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalancePrice"].Style.SelectionForeColor = Color.Empty;
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
            }
        }



        /// <summary>
        /// 获取隐藏类型编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetHideTypeNo(string hideType)
        {
            if (hideType == Resources.GetRes().GetString("Display"))
                return 0;
            else if (hideType == Resources.GetRes().GetString("Hide"))
                return 1;
            else if (hideType == Resources.GetRes().GetString("Income"))
                return 2;
            else if (hideType == Resources.GetRes().GetString("Expenditure"))
                return 3;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取隐藏类型
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetHideType(long hideTypeNo)
        {
            if (hideTypeNo == 0)
                return Resources.GetRes().GetString("Display");
            else if (hideTypeNo == 1)
                return Resources.GetRes().GetString("Hide");
            else if (hideTypeNo == 2)
                return Resources.GetRes().GetString("Income");
            else if (hideTypeNo == 3)
                return Resources.GetRes().GetString("Expenditure");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 切换显示所有功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbIsDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbIsDisplayAll.Checked)
            {
                krpcmAccountName.Visible = false;
                krpcmAccountNo.Visible = false;
                krpcmAccountBank.Visible = false;
                krpcmHideType.Visible = false;
                krpcmOrder.Visible = false;
                krpcmRemark.Visible = false;

            }
            else
            {
                krpcmAccountName.Visible = true;
                krpcmAccountNo.Visible = true;
                krpcmAccountBank.Visible = true;
                krpcmHideType.Visible = true;
                krpcmOrder.Visible = true;
                krpcmRemark.Visible = true;
            }
        }

      

        /// <summary>
        /// 切换多语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                krpcmBalanceName0.Visible = false;
                krpcmBalanceName1.Visible = false;
                krpcmBalanceName2.Visible = false;
                

                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmBalanceName0.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmBalanceName1.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmBalanceName2.Visible = true;
            }
            else
            {
                krpcmBalanceName0.Visible = true;
                krpcmBalanceName1.Visible = true;
                krpcmBalanceName2.Visible = true;
                
            }
        }
        
    }
}
