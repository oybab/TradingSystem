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
using Oybab.ServicePC.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class FastGridWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 999;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private bool IsOrder = true;
        private bool IsBarcode = false;
        private List<Product> resultList = null;
        private Dictionary<long, ProductAndPrice> selectedResults = new Dictionary<long, ProductAndPrice>();

        private DialogResult dialogResult = DialogResult.None;
        public Dictionary<long, ProductAndPrice> ReturnValue { get; private set; } //返回值

        public FastGridWindow(bool IsOrder, bool IsBarcode, bool IsOnlyOrder = false)
        {
            // 避免搜索时出现NULL
            this.resultList = new List<Product>();

            InitializeComponent();
            krpdgList.RecalcMagnification();

            this.IsOrder = IsOrder;
            this.IsBarcode = IsBarcode;
            new CustomTooltip(this.krpdgList);
            this.krpcmProductName.SetParent(this.krpdgList);
            this.Text = Resources.GetRes().GetString("Search");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;

            krplPage.Text = Resources.GetRes().GetString("Page");

            krplProductTypeName.Text = Resources.GetRes().GetString("ProductTypeName");
            krpcbOnlyDisplaySelected.Text = Resources.GetRes().GetString("OnlyDisplaySelected");



            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

                krpcbProductType.Location = new Point(krpcbProductType.Location.X, krpcbProductType.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());
            }

            if (IsBarcode)
            {
                krplIndexName.Text = Resources.GetRes().GetString("Barcode");
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Barcode.ico"));
            }
            else
            {
                krplIndexName.Text = Resources.GetRes().GetString("IndexName");
                this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.FastGrid.ico"));
            }


            krpcmProductId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductName.HeaderText = Resources.GetRes().GetString("ProductName");

            if (IsOrder)
                krpcmPrice.HeaderText = Resources.GetRes().GetString("UnitPrice");
            else
                krpcmPrice.HeaderText = Resources.GetRes().GetString("CostPrice");

            krpcmCount.HeaderText = Resources.GetRes().GetString("Count");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");

            krpcbProductType.Text = Resources.GetRes().GetString("TotalPrice");

            krpbAdd.Text = Resources.GetRes().GetString("Add");

            ReloadProduct();
            ReloadProductType();



            // 是否允许减少商品
            if (Common.GetCommon().IsDecreaseProductCount())
            {
                this.krpcmCount.Minimum = -9999;
            }


        }

        /// <summary>
        /// 搜索
        /// </summary>
        private void Search()
        {
            //为未保存数据而忽略当前操作
            if (!IgnoreOperateForSave())
                return;

            int hideIndex = 2;
            if (!IsOrder)
                hideIndex = 3;

            //查找数据
            resultList = resultList = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
            
            // 是否只找已选择的
            if (krpcbOnlyDisplaySelected.Checked)
            {
                resultList = resultList.Where(x => selectedResults.Keys.Contains(x.ProductId)).ToList();
            }
            else
            {
                if (krpcbProductType.SelectedIndex > 0)
                {
                    

                    if (Resources.GetRes().MainLangIndex == 0)
                        resultList = resultList.Where(x => x.ProductTypeId == Resources.GetRes().ProductTypes.Where(y => y.ProductTypeName0 == krpcbProductType.SelectedItem.ToString() && (x.HideType == 0 || x.HideType == hideIndex)).FirstOrDefault().ProductTypeId).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
                    else if (Resources.GetRes().MainLangIndex == 1)
                        resultList = resultList.Where(x => x.ProductTypeId == Resources.GetRes().ProductTypes.Where(y => y.ProductTypeName1 == krpcbProductType.SelectedItem.ToString() && (x.HideType == 0 || x.HideType == hideIndex)).FirstOrDefault().ProductTypeId).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
                    else if (Resources.GetRes().MainLangIndex == 2)
                        resultList = resultList.Where(x => x.ProductTypeId == Resources.GetRes().ProductTypes.Where(y => y.ProductTypeName2 == krpcbProductType.SelectedItem.ToString() && (x.HideType == 0 || x.HideType == hideIndex)).FirstOrDefault().ProductTypeId).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
                }
                else if (krpcbProductType.SelectedIndex == -1)
                {
                    KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("ProductTypeName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // 根据索引名称搜索
                if (!string.IsNullOrWhiteSpace(krptIndexName.Text))
                {
                    if (IsBarcode)
                    {
                        resultList = SearchByBarcode(resultList);
                    }
                    else
                    {
                        resultList = SearchByIndex(resultList);
                    }
                }

            }

            Open();

        }



        /// <summary>
        /// 打开
        /// </summary>
        private void Open()
        {
            // 设定页面数据
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
        /// 重置分页
        /// </summary>
        private void ResetPage()
        {
            krplPageCount.Text = "1";
            krptCurrentPage.Text = "1";
            krptCurrentPage.Enabled = krpbBeginPage.Enabled = krpbPrewPage.Enabled = krpbNextPage.Enabled = krpbEngPage.Enabled = krpbClickToPage.Enabled = false; 
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
            var currentResult = resultList.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Skip((CurrentPage - 1) * ListCount).Take(ListCount);
            //添加到数据集中
            krpdgList.Rows.Clear();
            foreach (var item in currentResult)
            {
                AddToGrid(item);
            }

        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="product"></param>
        private void AddToGrid(Product product)
        {
            string productName = "";
            string editMark = "";
            double price = 0;
            double totalPrice = 0;
            double count = 0;


            try
            {

                if (Resources.GetRes().MainLangIndex == 0)
                    productName = product.ProductName0;
                else if (Resources.GetRes().MainLangIndex == 1)
                    productName = product.ProductName1;
                else if (Resources.GetRes().MainLangIndex == 2)
                    productName = product.ProductName2;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }


            if (selectedResults.Any(x=>x.Key == product.ProductId))
            {
                editMark = "*";
                ProductAndPrice productInfo = selectedResults[product.ProductId];
                count = productInfo.Count;
                price = productInfo.Price;
            }
            else
            {
                count = 0;
                
                if (IsOrder)
                    price = product.Price;
                else
                    price = product.CostPrice;
            }

            totalPrice = Math.Round(price * count, 2);

           

            krpdgList.Rows.Add(editMark, product.ProductId, productName, price, count, totalPrice);

            if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice() && selectedResults.ContainsKey(product.ProductId))
            {
                krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmPrice"].ReadOnly = false;
            }
            else
            {
                krpdgList.Rows[krpdgList.Rows.Count - 1].Cells["krpcmPrice"].ReadOnly = true;
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
            {
                if (!(e.ColumnIndex == 4 && temp == null && krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "0"))
                    krpdgList.Rows[e.RowIndex].Cells["krpcmEdit"].Value = "*";
            }
                
        }


        /// <summary>
        /// 验证产品和数量,免得选择一个数量不够的产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void krpdgList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (null != krpdgList.EditingControl && (e.FormattedValue as string) != temp)
            {
                // 先确定当前行是不是选择产品和数量, 如果是那就机选一下该产品剩余数是否够购买当前选择的数,够就继续, 不够则直接返回上一个值
                if (e.ColumnIndex == 2 || e.ColumnIndex == 3 || e.ColumnIndex == 4)
                {
                    string productName = null;
                    double count = 0;
                    double price = 0;

                    if (e.ColumnIndex == 2)
                        productName = e.FormattedValue.ToString(); 
                    else
                         productName = krpdgList.Rows[e.RowIndex].Cells["krpcmProductName"].Value.ToString();

                    if (e.ColumnIndex == 4)
                        count = Math.Round(double.Parse(e.FormattedValue.ToString()), 3);
                    else
                        count = Math.Round(double.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmCount"].Value.ToString()), 3);

                    Product product = null;
                    if (Resources.GetRes().MainLangIndex == 0)
                        product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 1)
                        product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 2)
                        product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName).FirstOrDefault();

                    if (null == product)
                        return;


                    if (IsOrder && product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                    {
                        if (!selectedResults.ContainsKey(product.ProductId))
                        {
                            price = product.Price;
                        }
                        else
                        {
                            if (e.ColumnIndex == 3)
                                price = Math.Round(double.Parse(e.FormattedValue.ToString()), 2);
                            else
                                price = Math.Round(double.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].Value.ToString()), 2);
                        }
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = false;
                    }
                    else if (!IsOrder && product.CostPriceChangeMode == 1 && Common.GetCommon().IsChangeCostPrice())
                    {
                        if (!selectedResults.ContainsKey(product.ProductId))
                        {
                            price = product.CostPrice;
                        }
                        else
                        {
                            if (e.ColumnIndex == 3)
                                price = Math.Round(double.Parse(e.FormattedValue.ToString()), 2);
                            else
                                price = Math.Round(double.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].Value.ToString()), 2);
                        }
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = false;
                    }
                    else
                    {
                        if (IsOrder)
                            price = product.Price;
                        else
                            price = product.CostPrice;


                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = true;
                    }

                    


                    if (count != 0)
                    {
                        // 更新下总价
                        krpdgList.Rows[e.RowIndex].Cells["krpcmTotalPrice"].Value = Math.Round(price * count, 2);

                        // 已存在就更新一下价格
                        if (selectedResults.ContainsKey(product.ProductId))
                        {
                            selectedResults[product.ProductId].Price = price;
                            selectedResults[product.ProductId].Count = count;
                        }
                        else
                        {
                            selectedResults.Add(product.ProductId, new ProductAndPrice() { Count = count, Price = price, Product = product });
                        }
                    }
                    else
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmTotalPrice"].Value = 0;
                        // 如果数量小于等于0, 则去掉.
                        if (selectedResults.ContainsKey(product.ProductId))
                        {
                            selectedResults.Remove(product.ProductId);
                        }

                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].Value = product.Price;
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = true;

                        krpdgList.Rows[e.RowIndex].Cells["krpcmEdit"].Value = "";
                        temp = null;
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

                    // 依然在数量上停留, 方便上下箭头继续输入数量
                    if (_celWasEndEdit.ColumnIndex == 4)
                        iColumn = iColumn - 1;

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
            bool notHandle = false;

            if (notHandle)
            {
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("Search"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
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
            //base.OnKeyDown(e);
            if (e.Control && lastKeyPressed != Keys.EraseEof)
            {
                
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
        /// 加载产品
        /// </summary>
        internal void ReloadProduct()
        {
            krpcmProductName.SetValues(null, false);

            if (Resources.GetRes().Products.Count != 0)
            {
                int hideIndex = 2;
                if (!IsOrder)
                    hideIndex = 3;

                if (Resources.GetRes().MainLangIndex == 0)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName0).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName1).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName2).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
            }
        }


        /// <summary>
        /// 重新加载产品类型搜索框
        /// </summary>
        internal void ReloadProductType(bool TrigChangeEvent = false)
        {
            krpcbProductType.Items.Clear();
            krpcbProductType.Items.Add(Resources.GetRes().GetString("All"));

            int hideIndex = 2;
            if (!IsOrder)
                hideIndex = 3;


            if (Resources.GetRes().ProductTypes.Where(x => x.HideType == 0 || x.HideType == hideIndex).Count() > 0)
            {
               
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    string[] types = Resources.GetRes().ProductTypes.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId).Select(x => x.ProductTypeName0).ToArray();
                    krpcbProductType.Items.AddRange(types);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] types = Resources.GetRes().ProductTypes.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId).Select(x => x.ProductTypeName1).ToArray();
                    krpcbProductType.Items.AddRange(types);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] types = Resources.GetRes().ProductTypes.Where(x => x.HideType == 0 || x.HideType == hideIndex).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductTypeId).Select(x => x.ProductTypeName2).ToArray();
                    krpcbProductType.Items.AddRange(types);
                }
            }

            krpcbProductType.SelectedIndex = 0;

        }

        /// <summary>
        /// 只显示已选中的
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbOnlyDisplaySelected_CheckedChanged(object sender, EventArgs e)
        {
            // 只显示已选中的
            if (krpcbOnlyDisplaySelected.Checked)
            {
                krpcbProductType.Enabled = false;
            }
            else
            {
                krpcbProductType.Enabled = true;
            }

            // 如果更改了分类则继续重新搜索
            Search();
        }

        /// <summary>
        /// 分类搜索更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbProductType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 如果更改了分类则继续重新搜索
            Search();
        }

        /// <summary>
        /// 将所有选中的产品和数量返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdd_Click(object sender, EventArgs e)
        {
            ReturnValue = selectedResults;
            dialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }




        /// <summary>
        /// 快速查找条码
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        internal void FastGridBarcodeSearch(List<Product> products, string code)
        {
            string barcodeNo = code;
            krptIndexName.Text = code;
            resultList = products.Where(x => x.Barcode == barcodeNo).OrderByDescending(x=> x.Order).ThenBy(x=>x.ProductParentCount).ToList();
            Open();

            krpdgList.Focus();
            krpdgList.CurrentCell = krpdgList.Rows[0].Cells["krpcmCount"];
        }


        /// <summary>
        /// 查找条码
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Product> SearchByBarcode(List<Product> products)
        {
            string barcodeNo = krptIndexName.Text;

            return products.Where(x => (x.Barcode == barcodeNo || (x.IsScales == 1 && barcodeNo.StartsWith("22" + x.Barcode)))).ToList();
        }



        /// <summary>
        /// 快速查找
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        private List<Product> SearchByIndex(List<Product> products)
        {
            string index = krptIndexName.Text;
            return products.Where(x => x.ProductName0.Contains(index) || x.ProductName1.Contains(index) || x.ProductName2.Contains(index)).ToList();
        }

        /// <summary>
        /// 每次输入的索引变动时再搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptIndexName_KeyUp(object sender, KeyEventArgs e)
        {
            Search();
        }
        
    }


    internal sealed class ProductAndPrice
    {
        public double Count { get; set; }
        public double Price { get; set; }
        public Product Product { get; set; }
    }

}
