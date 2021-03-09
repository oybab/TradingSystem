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
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class ImportWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Import> resultList = null;
        private TimeSpan TimeLimit = TimeSpan.FromDays(Resources.GetRes().ShorDay);

        public ImportWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            this.ControlBox = false;

            this.Text = Resources.GetRes().GetString("ExpenditureManager");
            new CustomTooltip(this.krpdgList);
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
            krpbAdd.Text = Resources.GetRes().GetString("Add");
            krplAddTime.Text = Resources.GetRes().GetString("AddTime");

            krptbStartTime.Value =  DateTime.Now.AddDays(-1);
            krptbEndTime.Value = DateTime.Now;


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //显示
            LoadContextMenu(kryptonContextMenuItemShow, Resources.GetRes().GetString("Show"), Resources.GetRes().GetString("ShowDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ExpendDetails.png")), (sender, e) => { Check(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Expenditure.ico"));

            //初始化
            Init();

            // 免得添加新的回调添加时报错
            resultList = new List<Import>();

            Common.GetCommon().OpenPriceMonitor("0");
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
            krpcmImportId.HeaderText = Resources.GetRes().GetString("ImportId");
            krpcmImportTime.HeaderText = Resources.GetRes().GetString("ImportTime");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");

            krpcmPaidPrice.HeaderText = Resources.GetRes().GetString("PaidPrice");
            krpcmBorrowPrice.HeaderText = Resources.GetRes().GetString("BorrowPrice");
            krpcmKeepPrice.HeaderText = Resources.GetRes().GetString("KeepPrice");

            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcmRemark.HeaderText = Resources.GetRes().GetString("Remark");

            krpcmSupplierName.HeaderText = Resources.GetRes().GetString("SupplierName");

            krpcmSupplierPaidPrice.HeaderText = Resources.GetRes().GetString("SupplierPaidPrice");
            krpcmTotalPaidPrice.HeaderText = Resources.GetRes().GetString("TotalPaidPrice");

            krpcmFinishAdminName.HeaderText = Resources.GetRes().GetString("CheckoutBy");
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

            Task.Factory.StartNew(() =>
            {
                try
                {
                    
                    List<Import> imports;
                    bool result = OperatesService.GetOperates().ServiceGetImports(addTimeStart, addTimeEnd, false, out imports);
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            this.resultList = imports.OrderByDescending(x => x.ImportId).ToList();
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
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdd_Click(object sender, EventArgs e)
        {
            
        }


        /// <summary>
        /// 新增
        /// </summary>
        private void Add()
        {
            NewImportDetailsWindow newImport = new NewImportDetailsWindow();
            newImport.StartLoad += (sender2, e2) =>
            {
                StartLoad(sender2, null);
            };
            newImport.StopLoad += (sender2, e2) =>
            {
                StopLoad(sender2, null);
            };
            newImport.ShowDialog(this);


            Import item = newImport.ReturnValue;
            if (null != item)
            {
                resultList.Insert(0, item);

                AddToGrid("-1", item.ImportId.ToString(), item.tb_supplier, item.ImportTime, item.TotalPrice,  item.SupplierPaidPrice, item.PaidPrice,  item.TotalPaidPrice,  item.BorrowPrice, item.KeepPrice, item.AdminId, item.AddTime, item.Remark);
            }
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
                AddToGrid("", item.ImportId.ToString(), item.tb_supplier, item.ImportTime, item.TotalPrice, item.SupplierPaidPrice, item.PaidPrice, item.TotalPaidPrice, item.BorrowPrice, item.KeepPrice, item.AdminId, item.AddTime, item.Remark);
            }
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="supplier"></param>
        /// <param name="ImportTime"></param>
        /// <param name="TotalPrice"></param>
        /// <param name="SupplierDealsPrice"></param>
        /// <param name="DealsPrice"></param>
        /// <param name="ActualPrice"></param>
        /// <param name="SupplierPaidPrice"></param>
        /// <param name="PaidPrice"></param>
        /// <param name="CardPaidPrice"></param>
        /// <param name="TotalPaidPrice"></param>
        /// <param name="ReturnPrice"></param>
        /// <param name="BorrowPrice"></param>
        /// <param name="KeepPrice"></param>
        /// <param name="FinishAdminId"></param>
        /// <param name="AddTime"></param>
        /// <param name="Remark"></param>
        private void AddToGrid(string editMark, string Id, Supplier supplier, long ImportTime, double TotalPrice,  double SupplierPaidPrice, double PaidPrice,  double TotalPaidPrice, double BorrowPrice, double KeepPrice, long FinishAdminId, long AddTime, string Remark)
        {
            string ImportTimeStr = "";
            string AddTimeStr = "";
            string SupplierNameStr = "";
            string FinishAdminName = "";

            try
            {

                if (null != supplier)
                {
                    if (Resources.GetRes().MainLangIndex == 0)
                        SupplierNameStr = supplier.SupplierName0;
                    else if (Resources.GetRes().MainLangIndex == 1)
                        SupplierNameStr = supplier.SupplierName1;
                    else if (Resources.GetRes().MainLangIndex == 2)
                        SupplierNameStr = supplier.SupplierName2;
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


                ImportTimeStr = DateTime.ParseExact(ImportTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");
                AddTimeStr = DateTime.ParseExact(AddTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, SupplierNameStr, TotalPrice.ToString(), SupplierPaidPrice.ToString() ,PaidPrice.ToString(), TotalPaidPrice.ToString(), BorrowPrice.ToString(), KeepPrice.ToString(), FinishAdminName, ImportTimeStr, Remark);
            else 
            {
                if (editMark == "-1")
                {
                    krpdgList.Rows.Insert(0, " ", Id, SupplierNameStr, TotalPrice.ToString(), SupplierPaidPrice.ToString(), PaidPrice.ToString(), TotalPaidPrice.ToString(), BorrowPrice.ToString(), KeepPrice.ToString(), FinishAdminName, ImportTimeStr, Remark);
                    krpdgList.ClearSelection();
                    krpdgList.Rows[0].Selected = true;
                }
                else
                    krpdgList.Rows.Add(editMark, Id, SupplierNameStr, TotalPrice.ToString(), SupplierPaidPrice.ToString(), PaidPrice.ToString(), TotalPaidPrice.ToString(), BorrowPrice.ToString(), KeepPrice.ToString(), FinishAdminName, ImportTimeStr, Remark);
                
                // 颜色提醒显示特殊的交易
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("ExpenditureManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            long importId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmImportId"].Value.ToString());
            string mark = krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.ToString();

            Import import = resultList.Where(x => x.ImportId == importId).FirstOrDefault();
            if (mark != " ")
            {
                StartLoad(this, null);
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        List<ImportDetail> importDetails;
                        List<ImportPay> importPays;

                        bool result = OperatesService.GetOperates().ServiceGetImportDetail(importId, out importDetails, out importPays);
                        this.BeginInvoke(new Action(() =>
                        {
                            if (result)
                            {
                                ImportDetailsWindow details = new ImportDetailsWindow(import, importDetails, importPays);
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
                ImportDetailsWindow details = new ImportDetailsWindow(import, import.tb_importdetail.ToList(), import.tb_importpay.ToList());
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
            //base.OnKeyDown(e);
            if (e.Control && lastKeyPressed != Keys.EraseEof)
            {
                //任何情况下都可以增加
                if (e.KeyCode == Keys.N)
                {
                    Add();
                }
                //当前选中的可以删除
                else if (e.KeyCode == Keys.L)
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
        /// 最后一行改颜色
        /// </summary>
        /// <param name="borrowPriceColor"></param>
        /// <param name="keepPriceColor"></param>
        private void SetColorToLastRow(Color borrowPriceColor, Color keepPriceColor)
        {
            krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmBorrowPrice"].Style.ForeColor = krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmBorrowPrice"].Style.SelectionForeColor = borrowPriceColor;
            krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmKeepPrice"].Style.ForeColor = krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmKeepPrice"].Style.SelectionForeColor = keepPriceColor;
        }



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

       
        
    }
}
