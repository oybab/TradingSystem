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
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.ServicePC.DialogWindow;
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class NewImportDetailsWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 100;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private bool AllowClose = false;
        private List<ImportDetail> resultList;
        public Import ReturnValue { get; private set; } //返回值

        private TimeSpan TimeLimit = TimeSpan.FromDays(3);

        public NewImportDetailsWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            this.krpcmProductName.SetParent(this.krpdgList);

            new CustomTooltip(this.krpdgList);
            this.Text = Resources.GetRes().GetString("AddImport");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbAddByBarcode.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Barcode.png"));
            krpbAddByFastGrid.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.FastGrid.png"));
            krpbAddByBarcode.StateCommon.Back.ImageStyle = krpbAddByFastGrid.StateCommon.Back.ImageStyle = PaletteImageStyle.Stretch;
            krpbAddByBarcode.StateCommon.Back.Draw = krpbAddByFastGrid.StateCommon.Back.Draw = InheritBool.True;

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");


            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");
            krplImportTime.Text = Resources.GetRes().GetString("ImportTime");

            krpbSave.Text = Resources.GetRes().GetString("CheckoutImport");
            krplRemark.Text = Resources.GetRes().GetString("Remark");


            krplTotalPriceValue.Text = "0";

            try
            {
                krptbImportTime.Value = DateTime.Now;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }



            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }

            // 增加右键
            // 添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            // 删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.NewImportDetails.ico"));


            // 是否允许减少商品
            if (Common.GetCommon().IsDecreaseProductCount())
            {
                this.krpcmCount.Minimum = -9999;
            }

            // 没有修改单价权限,就不要显示了
            if (!Common.GetCommon().IsChangeUnitPrice())
                krpcmPrice.Visible = false;

            //初始化
            Init();

            ResetPage();

            resultList = new List<ImportDetail>();//假的,防止翻页报错
            OpenPageTo(1, false);

            // 扫条码
            Notification.Instance.NotificationBarcodeReader += Instance_NotificationBarcodeReader;

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
        /// 扫条形码
        /// </summary>
        private void ScanBarcode(string code)
        {
            // 获取条码产品
         
            List<Product> products =Resources.GetRes().Products.Where(x => (x.Barcode == code) && (x.HideType == 0 || x.HideType == 3)).ToList();

            if (products.Count > 1)
            {
                this.BeginInvoke(new Action(() =>
                {
                    FastGridWindow window = new FastGridWindow(false, true);
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
                       
                        AddToGrid("*", "-1", product.ProductId, product.CostPrice, 1, Math.Round(product.CostPrice * 1, 2), product.Price);
                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsChangeCostPrice())
                            krpdgList.Rows[0].Cells["krpcmCostPrice"].ReadOnly = false;
                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsChangeUnitPrice())
                            krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;

                        Import import;
                        List<ImportDetail> importDetails;
                        Calc(out importDetails, out import);

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
            krpcmImportDetailId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductName.HeaderText = Resources.GetRes().GetString("ProductName");
            krpcmCostPrice.HeaderText = Resources.GetRes().GetString("CostPrice");
            krpcmCount.HeaderText = Resources.GetRes().GetString("Count");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");
            krpcmPrice.HeaderText = Resources.GetRes().GetString("UnitPrice");


            ReloadProduct();
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
            var currentResult = resultList.OrderByDescending(x => x.ImportDetailId).Skip((CurrentPage - 1) * ListCount).Take(ListCount);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid("", item.ImportDetailId.ToString(), item.ProductId, item.Price, item.Count, item.TotalPrice, item.SalePrice);
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
        /// <param name="SalePrice"></param>
        private void AddToGrid(string editMark, string Id, long productId, double Price, double Count, double TotalPrice, double SalePrice)
        {
            string productName = "";

            try
            {
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
                krpdgList.Rows.Insert(0, editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), SalePrice.ToString());
                // 将新增加的暂时数量冻结掉
                krpdgList.Rows[0].Cells["krpcmCostPrice"].ReadOnly = true;
            }
            else
                krpdgList.Rows.Add(editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), SalePrice.ToString());
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


            Import import;
            List<ImportDetail> importDetails;
            Calc(out importDetails, out import);
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
                Notification.Instance.NotificationBarcodeReader -= Instance_NotificationBarcodeReader;
                Common.GetCommon().OpenPriceMonitor(null);
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("AddImport"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
        private void Add()
        {
            OpenPageTo(1);
            AddToGrid("*", "-1", 0, 0, 1, 0, 0);
            krpdgList.FirstDisplayedScrollingRowIndex = 0;


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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmImportDetailId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);

                    Import import;
                    List<ImportDetail> importDetails;
                    Calc(out importDetails, out import);
                }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
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
              
                //当前选中的可以删除
                if (e.KeyCode == Keys.D)
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
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 3).OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductId).Select(x => x.ProductName0).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 3).OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductId).Select(x => x.ProductName1).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 3).OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductId).Select(x => x.ProductName2).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
            }
        }


        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="details"></param>
        /// <param name="import"></param>
        /// <param name="IgnoreError"></param>
        /// <param name="OnlyTotal"></param>
        private void Calc(out List<ImportDetail> details, out Import import, bool IgnoreError = true, bool OnlyTotal = false)
        {
            bool IsError = false;
            details = new List<ImportDetail>();
            import = new Import();

           if (!OnlyTotal)
           {
               for (int i = krpdgList.Rows.Count - 1; i >= 0; i--)
               {

                   ImportDetail importDetails = new ImportDetail();

                   //只有有改动才可以继续
                   if (krpdgList.Rows[i].Cells["krpcmEdit"].Value.Equals("*"))
                   {
                       try
                       {
                           string productName = krpdgList.Rows[i].Cells["krpcmProductName"].Value.ToString();
                           Product product = null;
                           if (Resources.GetRes().MainLangIndex == 0)
                               product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();
                           else if (Resources.GetRes().MainLangIndex == 1)
                               product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();
                           else if (Resources.GetRes().MainLangIndex == 2)
                               product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();

                           importDetails.ProductId = product.ProductId;
                           importDetails.Count = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmCount"].Value.ToString()), 3);

                           krpdgList.Rows[i].Cells["krpcmCostPrice"].Value = importDetails.Price = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmCostPrice"].Value.ToString()), 2);
                           krpdgList.Rows[i].Cells["krpcmTotalPrice"].Value = importDetails.TotalPrice = Math.Round(importDetails.Price * importDetails.Count, 2);
                           importDetails.OriginalTotalPrice = Math.Round(product.CostPrice * importDetails.Count, 2);

                            krpdgList.Rows[i].Cells["krpcmPrice"].Value = importDetails.SalePrice = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmPrice"].Value.ToString()), 2);
                            importDetails.OriginalSalePrice = product.Price;


                           details.Add(importDetails);

                       }
                        catch
#if DEBUG
                       (Exception ex)
#endif
                        {
                            krpdgList.Rows[i].Cells["krpcmCostPrice"].Value = "?";
                           krpdgList.Rows[i].Cells["krpcmTotalPrice"].Value = "?";
                           krpdgList.Rows[i].Cells["krpcmPrice"].Value = "?";

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
               }

               if (IsError)
                   return;
           }



            if (IsError)
                return;

            if (!OnlyTotal)
            {
                krplTotalPriceValue.Text = (import.TotalPrice = Math.Round(details.Sum(x => x.TotalPrice), 2)).ToString();
                import.OriginalTotalPrice = Math.Round(details.Sum(x => x.OriginalTotalPrice), 2);

            }
            else
            {

                return;
            }


            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(Math.Round(import.TotalPaidPrice - import.TotalPrice, 2).ToString());


            import.Remark = GetValueOrNull(krptbRemark.Text);


            DateTime importDateTime = DateTime.Now;

            try
            {
                importDateTime = krptbImportTime.Value;
                import.ImportTime = long.Parse(importDateTime.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
                IsError = true;
                if (!IgnoreError)
                {
                    krptbImportTime.Focus();
                    throw new OybabException(string.Format(Resources.GetRes().GetString("PropertyError"), Resources.GetRes().GetString("Time")));
                }
            }

            if (IsError)
                return;


            ShowOrHideSave();

        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {

            Import import;
            List<ImportDetail> details;

            try
            {
                Calc(out details, out import, false, false);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, Resources.GetRes().GetString("SaveFailt"));
                return;
            }


            //StartLoad(this, null);

            import.tb_importdetail = details;
            ImportCheckoutWindow window = new ImportCheckoutWindow(import);
            window.StartLoad += (x, y) =>
            {
                this.StartLoad(x, null);
            };
            window.StopLoad += (x, y) =>
            {
                this.StopLoad(x, null);
            };
            DialogResult dialogResult = window.ShowDialog(this);
            // 如果成功, 代表有改动, 重修刷新一下当前数据
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                AllowClose = true;

                this.ReturnValue = window.ReturnValue;

            }

            if (AllowClose)
            {
                this.Close();
            }
        }


        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");
       


        /// <summary>
        /// 显示或隐藏保存按钮
        /// </summary>
        private void ShowOrHideSave()
        {
            if (krpdgList.Rows.Count > 0)
                krpbSave.Visible = true;
            else
                krpbSave.Visible = false;
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
        /// 验证产品是否已添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpdgList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((null != krpdgList.EditingControl || null != currentCell) && (e.FormattedValue as string) != temp)//null != krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value && 
            {
                if (e.ColumnIndex == 2)
                {
                    string productName = e.FormattedValue.ToString();

                    Product product = null;
                    if (Resources.GetRes().MainLangIndex == 0)
                        product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 1)
                        product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 2)
                        product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();


                    // 判断是否已存在!
                    if (ProductAlreadyExists(productName, e.RowIndex))
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("CurrentProductAlreadyExists"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Cancel = true;
                        krpdgList.CancelEdit();
                    }

                    // 根据是否修改价格以及权限解冻, 或者冻结
                    if (null != product && product.CostPriceChangeMode == 1 && Common.GetCommon().IsChangeCostPrice())
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmCostPrice"].ReadOnly = false;
                    }
                    else
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmCostPrice"].ReadOnly = true;
                    }
                    if (null != product && product.PriceChangeMode == 1 && Common.GetCommon().IsChangeUnitPrice())
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = false;
                    }
                    else
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = true;
                    }

                    // 将产品原始价格放进去
                    if (null != product)
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmCostPrice"].Value = product.CostPrice.ToString();
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].Value = product.Price.ToString();
                    }
                }
                else if (e.ColumnIndex == 4)
                {
                    double count = Math.Round(double.Parse(e.FormattedValue.ToString()), 3);//long.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmCount"].Value.ToString());
                    if (count == 0)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// 通过扫条码进货
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAddByBarcode_Click(object sender, EventArgs e)
        {
            FastGridWindow window = new FastGridWindow(false, true);
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
            FastGridWindow window = new FastGridWindow(false, false);
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
                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.Key && (x.HideType == 0 || x.HideType == 3)).FirstOrDefault();

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
                        AddToGrid("*", "-1", product.ProductId, item.Value.Price, item.Value.Count, Math.Round(item.Value.Price * item.Value.Count, 2), product.Price);
                        if (product.CostPriceChangeMode == 1 && Common.GetCommon().IsChangeCostPrice())
                            krpdgList.Rows[0].Cells["krpcmCostPrice"].ReadOnly = false;
                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsChangeUnitPrice())
                            krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;
                    }



                }

                Import import;
                List<ImportDetail> importDetails;
                Calc(out importDetails, out import);
            }
        }


    }
}
