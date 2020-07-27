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

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class PrinterWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Printer> resultList = null;

        public PrinterWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificationPrinter += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.PrinterId == value.PrinterId)) krpdgList.Enabled = false; })); };

            new CustomTooltip(this.krpdgList);
            this.ControlBox = false;
            
            krptPrinterName.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("PrinterManager");
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
            krplPrinterName.Text = Resources.GetRes().GetString("PrinterName");


            if(int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptPrinterName.Location = new Point(krptPrinterName.Location.X, krptPrinterName.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Printer.ico"));

            //初始化
            Init();


            //防止直接增加数据的时候插入本地队列失败
            resultList = new List<Printer>();

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
            krpcmPrinterId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmPrinterName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("PrinterName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmPrinterName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("PrinterName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmPrinterName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("PrinterName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmPrinterDeviceName.HeaderText = Resources.GetRes().GetString("PrinterDeviceName");
            krpcmPrintType.HeaderText = Resources.GetRes().GetString("PrintType");
            krpcmIsMain.HeaderText = Resources.GetRes().GetString("PrintAll");
            krpcmIsCashDrawer.HeaderText = Resources.GetRes().GetString("CashDrawer");
            krpcmLang.HeaderText = Resources.GetRes().GetString("Language");
            krpcmOrder.HeaderText = Resources.GetRes().GetString("Order");
            krpcmIsEnable.HeaderText = Resources.GetRes().GetString("IsEnable");
            

            ReloadPrinterTextbox();


            krpcmPrintType.Items.AddRange(new string[] { Resources.GetRes().GetString("Bill"), Resources.GetRes().GetString("Backstage"), Resources.GetRes().GetString("BillWide") });


            krpcmLang.Items.Add(Resources.GetRes().GetString("Customer"));
            krpcmLang.Items.AddRange(Resources.GetRes().MainLangList.Select(x => x.Value.LangName).ToArray());



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

            //查找数据
            resultList = Resources.GetRes().Printers.OrderByDescending(x => x.Order).ThenByDescending(x => x.PrinterId).ToList();
            if (krptPrinterName.Text.Trim() != "")
                resultList = resultList.Where(x => x.PrinterName0.Contains(krptPrinterName.Text.Trim(), StringComparison.OrdinalIgnoreCase) || x.PrinterName1.Contains(krptPrinterName.Text.Trim(), StringComparison.OrdinalIgnoreCase) || x.PrinterName2.Contains(krptPrinterName.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            

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
                AddToGrid("", item.PrinterId.ToString(), item.PrinterName0, item.PrinterName1, item.PrinterName2, item.PrinterDeviceName, GetPrintType(item.PrintType), item.IsMain, item.IsCashDrawer, GetLanguage(item.Lang), item.Order, item.IsEnable);
            }
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="printerNameZH"></param>
        /// <param name="printerNameUG"></param>
        /// <param name="printerNameEN"></param>
        /// <param name="printerDeviceName"></param>
        /// <param name="printType"></param>
        /// <param name="IsMain"></param>
        /// <param name="lang"></param>
        /// <param name="order"></param>
        /// <param name="isEnable"></param>
        private void AddToGrid(string editMark, string Id, string printerName0, string printerName1, string printerName2, string printerDeviceName, string printType, long isMain, long IsCashDrawer, string lang, long order, long isEnable)
        {
            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, printerName0, printerName1, printerName2, printerDeviceName, printType, isMain.ToString(), IsCashDrawer.ToString(), lang, order.ToString(), isEnable.ToString());
            else
                krpdgList.Rows.Add(editMark, Id, printerName0, printerName1, printerName2, printerDeviceName, printType, isMain.ToString(), IsCashDrawer.ToString(), lang, order.ToString(), isEnable.ToString());
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("PrinterManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            AddToGrid("*", "-1", "", "", "", "", GetPrintType(0), 0, 0, GetLanguage(-1), 0, 1);
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
                if (krpdgList.SelectedRows[0].Cells["krpcmPrinterId"].Value.ToString().Equals("-1"))
                {
                    Printer model = new Printer();
                    try
                    {

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmPrinterName0"], krpdgList.SelectedRows[0].Cells["krpcmPrinterName1"], krpdgList.SelectedRows[0].Cells["krpcmPrinterName2"], true, false, krpcbMultipleLanguage.Checked);


                        model.PrinterId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrinterId"].Value.ToString());
                        model.PrinterName0 = krpdgList.SelectedRows[0].Cells["krpcmPrinterName0"].Value.ToString().Trim();
                        model.PrinterName1 = krpdgList.SelectedRows[0].Cells["krpcmPrinterName1"].Value.ToString().Trim();
                        model.PrinterName2 = krpdgList.SelectedRows[0].Cells["krpcmPrinterName2"].Value.ToString().Trim();
                        model.PrinterDeviceName = krpdgList.SelectedRows[0].Cells["krpcmPrinterDeviceName"].Value.ToString().Trim();
                        model.PrintType = GetPrintTypeNo(krpdgList.SelectedRows[0].Cells["krpcmPrintType"].Value.ToString());
                        model.IsMain = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsMain"].Value.ToString());
                        model.IsCashDrawer = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsCashDrawer"].Value.ToString());
                        model.Lang = GetLanguageNo(krpdgList.SelectedRows[0].Cells["krpcmLang"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());

                        //判断空
                        if (string.IsNullOrWhiteSpace(model.PrinterName0) || string.IsNullOrWhiteSpace(model.PrinterName1) || string.IsNullOrWhiteSpace(model.PrinterName2) || string.IsNullOrWhiteSpace(model.PrinterDeviceName))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Printers.Where(x => (x.PrinterName0.Equals(model.PrinterName0, StringComparison.OrdinalIgnoreCase) || x.PrinterName1.Equals(model.PrinterName1, StringComparison.OrdinalIgnoreCase) || x.PrinterName2.Equals(model.PrinterName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("PrinterName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                        if (model.IsCashDrawer == 1 && model.PrintType != 0)
                        {
                            krpdgList.SelectedRows[0].Cells["krpcmIsCashDrawer"].Value = "0";
                            model.IsCashDrawer = 0;
                        }

                        if (model.IsCashDrawer == 1 && Resources.GetRes().Printers.Where(x=>x.IsCashDrawer == 1).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("CashDrawer")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            bool result = OperatesService.GetOperates().ServiceAddPrinter(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmPrinterId"].Value = model.PrinterId;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);
                                    Resources.GetRes().Printers.Add(model);
                                    ReloadPrinterTextbox(true);
                                }
                                else
                                {
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
                    Printer model = new Printer();
                    try
                    {

                        model.PrinterId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrinterId"].Value.ToString());

                        model = Resources.GetRes().Printers.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault().FastCopy();

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmPrinterName0"], krpdgList.SelectedRows[0].Cells["krpcmPrinterName1"], krpdgList.SelectedRows[0].Cells["krpcmPrinterName2"], false, Ext.AllSame(model.PrinterName0, model.PrinterName1, model.PrinterName2), krpcbMultipleLanguage.Checked);


                        model.PrinterName0 = krpdgList.SelectedRows[0].Cells["krpcmPrinterName0"].Value.ToString().Trim();
                        model.PrinterName1 = krpdgList.SelectedRows[0].Cells["krpcmPrinterName1"].Value.ToString().Trim();
                        model.PrinterName2 = krpdgList.SelectedRows[0].Cells["krpcmPrinterName2"].Value.ToString().Trim();
                        model.PrinterDeviceName = krpdgList.SelectedRows[0].Cells["krpcmPrinterDeviceName"].Value.ToString().Trim();
                        model.PrintType = GetPrintTypeNo(krpdgList.SelectedRows[0].Cells["krpcmPrintType"].Value.ToString());
                        model.IsMain = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsMain"].Value.ToString());
                        model.IsCashDrawer = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsCashDrawer"].Value.ToString());
                        model.Lang = GetLanguageNo(krpdgList.SelectedRows[0].Cells["krpcmLang"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());
                        model.IsEnable = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsEnable"].Value.ToString());

                        //判断空
                        if (string.IsNullOrWhiteSpace(model.PrinterName0) || string.IsNullOrWhiteSpace(model.PrinterName1) || string.IsNullOrWhiteSpace(model.PrinterName2) || string.IsNullOrWhiteSpace(model.PrinterDeviceName))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Printers.Where(x => x.PrinterId != model.PrinterId && (x.PrinterName0.Equals(model.PrinterName0, StringComparison.OrdinalIgnoreCase) || x.PrinterName1.Equals(model.PrinterName1, StringComparison.OrdinalIgnoreCase) || x.PrinterName2.Equals(model.PrinterName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("PrinterName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (model.IsCashDrawer == 1 && model.PrintType != 0)
                        {
                            krpdgList.SelectedRows[0].Cells["krpcmIsCashDrawer"].Value = "0";
                            model.IsCashDrawer = 0;
                        }


                        if (model.IsCashDrawer == 1 && Resources.GetRes().Printers.Where(x => x.IsCashDrawer == 1 && x.PrinterId != model.PrinterId).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("CashDrawer")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            
                            ResultModel result = OperatesService.GetOperates().ServiceEditPrinter(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    Printer oldModel = resultList.Where(x => x.PrinterId == model.PrinterId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                    no = Resources.GetRes().Printers.IndexOf(oldModel);
                                    Resources.GetRes().Printers.RemoveAt(no);
                                    Resources.GetRes().Printers.Insert(no, model);

                                    ReloadPrinterTextbox(true);
                                }
                                else
                                {
                                    if (result.UpdateModel)
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrinterId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
                }
        

                //如果已经有使用,则提示并拒绝删除
                if (Resources.GetRes().Pprs.Where(x => x.PrinterId == Id).Count() > 0)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("PropertyUsed"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                try
                {
                    
                    ResultModel result = OperatesService.GetOperates().ServiceDelPrinter(Resources.GetRes().Printers.Where(x => x.PrinterId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Printer oldModel = Resources.GetRes().Printers.Where(x => x.PrinterId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);
                            Resources.GetRes().Printers.Remove(oldModel);
                            ReloadPrinterTextbox(true);
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
                        kryptonContextMenuItemSave.Enabled = true;
                    kryptonContextMenuItemDelete.Enabled = true;
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
                    if (krpdgList.SelectedRows.Count > 0)
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
        /// 类型被修改
        /// </summary>
        public event EventHandler ChangePrinter;
        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;


        /// <summary>
        /// 重新加载打印机搜索框
        /// </summary>
        private void ReloadPrinterTextbox(bool TrigChangeEvent = false)
        {
            krptPrinterName.SetValues(null, false);
            if (Resources.GetRes().Printers.Count > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krptPrinterName.SetValues(Resources.GetRes().Printers.OrderByDescending(x => x.Order).ThenByDescending(x=>x.PrinterId).Select(x => x.PrinterName0).ToArray(), false); 
                else if (Resources.GetRes().MainLangIndex == 1)
                    krptPrinterName.SetValues(Resources.GetRes().Printers.OrderByDescending(x => x.Order).ThenByDescending(x=>x.PrinterId).Select(x => x.PrinterName1).ToArray(), false);
                else if (Resources.GetRes().MainLangIndex == 2)
                    krptPrinterName.SetValues(Resources.GetRes().Printers.OrderByDescending(x => x.Order).ThenByDescending(x=>x.PrinterId).Select(x => x.PrinterName2).ToArray(), false);
            }

            if (TrigChangeEvent)
            {
                if (null != ChangePrinter)
                    ChangePrinter(null, null);
            }
        }


        /// <summary>
        /// 获取语言编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetLanguageNo(string language)
        {

            if (language == Resources.GetRes().GetString("Customer"))
                return -1;
            else
                return Resources.GetRes().GetMainLangByLangName(language).LangIndex;


        }

        /// <summary>
        /// 获取语言
        /// </summary>
        /// <param name="orderStateNo"></param>
        /// <returns></returns>
        private string GetLanguage(long languageNo)
        {
            if (languageNo == -1)
                return Resources.GetRes().GetString("Customer");
           else
                return Resources.GetRes().GetMainLangByLangIndex((int)languageNo).LangName;
        }



        /// <summary>
        /// 获取打印类型编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetPrintTypeNo(string type)
        {
            if (type == Resources.GetRes().GetString("Bill"))
                return 0;
            else if (type == Resources.GetRes().GetString("Backstage"))
                return 1;
            else if (type == Resources.GetRes().GetString("BillWide"))
                return 2;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取打印类型
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetPrintType(long typeNo)
        {
            if (typeNo == 0)
                return Resources.GetRes().GetString("Bill");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Backstage");
            else if (typeNo == 2)
                return Resources.GetRes().GetString("BillWide");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
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
        /// 切换显示所有功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbIsDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbIsDisplayAll.Checked)
            {
               
                krpcmOrder.Visible = false;
                krpcmIsCashDrawer.Visible = false;

            }
            else
            {
                krpcmOrder.Visible = true;
                krpcmIsCashDrawer.Visible = true;
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
                krpcmPrinterName0.Visible = false;
                krpcmPrinterName1.Visible = false;
                krpcmPrinterName2.Visible = false;


                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmPrinterName0.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmPrinterName1.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmPrinterName2.Visible = true;
            }
            else
            {
                krpcmPrinterName0.Visible = true;
                krpcmPrinterName1.Visible = true;
                krpcmPrinterName2.Visible = true;
            }
        }
       
        
    }
}
