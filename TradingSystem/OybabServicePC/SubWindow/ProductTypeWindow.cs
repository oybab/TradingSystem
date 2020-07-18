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
    internal sealed partial class ProductTypeWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<ProductType> resultList = null;

        public ProductTypeWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            Notification.Instance.NotificationProductType += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.ProductTypeId == value.ProductTypeId)) krpdgList.Enabled = false; })); };

            new CustomTooltip(this.krpdgList);
            this.ControlBox = false;
            
            krptProductTypeName.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("ProductTypeManager");
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
            krplProductTypeName.Text = Resources.GetRes().GetString("ProductTypeName");


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptProductTypeName.Location = new Point(krptProductTypeName.Location.X, krptProductTypeName.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());

            }
            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ProductType22.ico"));

            //初始化
            Init();


            //防止直接增加数据的时候插入本地队列失败
            resultList = new List<ProductType>();

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
            krpcmProductTypeId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductTypeName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("ProductTypeName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmProductTypeName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("ProductTypeName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmProductTypeName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("ProductTypeName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmImageName.HeaderText = Resources.GetRes().GetString("ImageName");
            krpcmOrder.HeaderText = Resources.GetRes().GetString("Order");
            krpcmHideType.HeaderText = Resources.GetRes().GetString("DisplayType");
            

            ReloadProductTypeTextbox();


            krpcmHideType.Items.AddRange(new string[] { Resources.GetRes().GetString("Display"), Resources.GetRes().GetString("Hide"), Resources.GetRes().GetString("Income"), Resources.GetRes().GetString("Expenditure") });//, Resources.GetRes().GetString("Backstage")


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
            resultList = Resources.GetRes().ProductTypes.OrderByDescending(x=>x.Order).ThenByDescending(x=>x.ProductTypeId).ToList();
            if (krptProductTypeName.Text.Trim() != "")
                resultList = resultList.Where(x => x.ProductTypeName0.Contains(krptProductTypeName.Text.Trim(), StringComparison.OrdinalIgnoreCase) || x.ProductTypeName1.Contains(krptProductTypeName.Text.Trim(), StringComparison.OrdinalIgnoreCase) || x.ProductTypeName2.Contains(krptProductTypeName.Text.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            

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
                AddToGrid("", item.ProductTypeId.ToString(), item.ProductTypeName0, item.ProductTypeName1, item.ProductTypeName2, item.ImageName ?? "", GetHideType(item.HideType), item.Order);
            }
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="productTypeNameZH"></param>
        /// <param name="productTypeNameUG"></param>
        /// <param name="productTypeNameEn"></param>
        /// <param name="imageName"></param>
        /// <param name="HideType"></param>
        /// <param name="Order"></param>
        private void AddToGrid(string editMark, string Id, string productTypeName0, string productTypeName1, string productTypeName2, string imageName, string HideType, long Order)
        {
            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, productTypeName0, productTypeName1, productTypeName2, imageName, Order.ToString(), HideType);
            else
                krpdgList.Rows.Add(editMark, Id, productTypeName0, productTypeName1, productTypeName2, imageName, Order.ToString(), HideType);
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("ProductTypeManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            AddToGrid("*", "-1", "", "", "", "", GetHideType(0), 0);
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
                if (krpdgList.SelectedRows[0].Cells["krpcmProductTypeId"].Value.ToString().Equals("-1"))
                {
                    ProductType model = new ProductType();
                    try
                    {
                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmProductTypeName0"], krpdgList.SelectedRows[0].Cells["krpcmProductTypeName1"], krpdgList.SelectedRows[0].Cells["krpcmProductTypeName2"], true, false, krpcbMultipleLanguage.Checked);

                        model.ProductTypeId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductTypeId"].Value.ToString());
                        model.ProductTypeName0 = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName0"].Value.ToString().Trim();
                        model.ProductTypeName1 = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName1"].Value.ToString().Trim();
                        model.ProductTypeName2 = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName2"].Value.ToString().Trim();
                        model.ImageName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmImageName"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());


                        //判断空
                        if (string.IsNullOrWhiteSpace(model.ProductTypeName0) || string.IsNullOrWhiteSpace(model.ProductTypeName1) || string.IsNullOrWhiteSpace(model.ProductTypeName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().ProductTypes.Where(x => (x.ProductTypeName0.Equals(model.ProductTypeName0, StringComparison.OrdinalIgnoreCase) || x.ProductTypeName1.Equals(model.ProductTypeName1, StringComparison.OrdinalIgnoreCase) || x.ProductTypeName2.Equals(model.ProductTypeName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("ProductTypeName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            bool result = OperatesService.GetOperates().ServiceAddProductType(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmProductTypeId"].Value = model.ProductTypeId;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);
                                    Resources.GetRes().ProductTypes.Add(model);
                                    ReloadProductTypeTextbox(true);
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
                    ProductType model = new ProductType();
                    try
                    {
                        model.ProductTypeId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductTypeId"].Value.ToString());

                        model = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId == model.ProductTypeId).FirstOrDefault().FastCopy();

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmProductTypeName0"], krpdgList.SelectedRows[0].Cells["krpcmProductTypeName1"], krpdgList.SelectedRows[0].Cells["krpcmProductTypeName2"], false, Ext.AllSame(model.ProductTypeName0, model.ProductTypeName1, model.ProductTypeName2), krpcbMultipleLanguage.Checked);

                        model.ProductTypeName0 = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName0"].Value.ToString().Trim();
                        model.ProductTypeName1 = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName1"].Value.ToString().Trim();
                        model.ProductTypeName2 = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName2"].Value.ToString().Trim();
                        model.ImageName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmImageName"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());

                        //判断空
                        if (string.IsNullOrWhiteSpace(model.ProductTypeName0) || string.IsNullOrWhiteSpace(model.ProductTypeName1) || string.IsNullOrWhiteSpace(model.ProductTypeName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId != model.ProductTypeId && (x.ProductTypeName0.Equals(model.ProductTypeName0, StringComparison.OrdinalIgnoreCase) || x.ProductTypeName1.Equals(model.ProductTypeName1, StringComparison.OrdinalIgnoreCase) || x.ProductTypeName2.Equals(model.ProductTypeName2, StringComparison.OrdinalIgnoreCase))).Count() > 0)
                        {

                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("ProductTypeName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                            
                            ResultModel result = OperatesService.GetOperates().ServiceEditProductType(model);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    ProductType oldModel = resultList.Where(x => x.ProductTypeId == model.ProductTypeId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                    no = Resources.GetRes().ProductTypes.IndexOf(oldModel);
                                    Resources.GetRes().ProductTypes.RemoveAt(no);
                                    Resources.GetRes().ProductTypes.Insert(no, model);

                                    ReloadProductTypeTextbox(true);
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductTypeId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                    return;
                }


                //如果已经有使用,则提示并拒绝删除
                if (Resources.GetRes().Products.Where(x => x.ProductTypeId == Id).Count() > 0)
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
                    
                    ResultModel result = OperatesService.GetOperates().ServiceDelProductType(Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ProductType oldModel = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);
                            Resources.GetRes().ProductTypes.Remove(oldModel);
                            ReloadProductTypeTextbox(true);
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

            }// 导出数据
            else if (e.KeyCode == Keys.F7 && e.Alt)
            {
                Oybab.ServicePC.Tools.BakInOperate.Instance.Output(this, new BakInModel() { ProductTypes = resultList });
            }
            // 导入数据
            else if (e.KeyCode == Keys.F8 && e.Alt)
            {
                BakInModel model = Oybab.ServicePC.Tools.BakInOperate.Instance.Import<BakInModel>(this);
                if (null != model)
                {
                    if (null != model.ProductTypes)
                    {
                        foreach (var item in model.ProductTypes)
                        {
                            AddToGrid("*", "-1", item.ProductTypeName0, item.ProductTypeName1, item.ProductTypeName2, item.ImageName ?? "", GetHideType(item.HideType), item.Order);
                        }
                    }
                    else
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
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
        private void ReloadProductTypeTextbox(bool TrigChangeEvent = false)
        {
            krptProductTypeName.SetValues(null, false);
            if (Resources.GetRes().ProductTypes.Count > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krptProductTypeName.SetValues(Resources.GetRes().ProductTypes.OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductTypeId).Select(x => x.ProductTypeName0).ToArray(), false); //new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "A3", "4B", "C5", "D6", "E7", "F8", "G9", "0H", "Irt", "J3", "K3", "3L", "3M", "3N", "O3", "P5", "Q6", "6R", "S6", "T6", "6U", "6V", "6W", "X6", "Y6", "6Z", "06", "16", "26", "36", "46", "65", "66", "76", "As", "dB", "dC", "Dd", "dE", "dF", "dG", "Hd", "dI", "Jd", "Kd", "dL", "Md", "Nd", "Od", "Pd", "dQ", "Rd", "Sd", "Td", "Ud", "dV", "Wd", "dX", "Yd", "Zd", "d0", "1d", "d2", "3d", "d4", "5d", "6d", "7d", "cA", "cB", "Cc", "cD", "Ec", "Fc", "cG", "Hc", "cI", "gJ", "gK", "gL", "gM", "gN", "gO", "gP", "gQ", "gR", "Sg", "gT", "gU", "gV", "gW", "gX", "Yg", "Zg", "g0", "g1", "g2", "g3", "g4", "g5", "g6", "g7", };
                else if (Resources.GetRes().MainLangIndex == 1)
                    krptProductTypeName.SetValues(Resources.GetRes().ProductTypes.OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductTypeId).Select(x => x.ProductTypeName1).ToArray(), false); 
                else if (Resources.GetRes().MainLangIndex == 2)
                    krptProductTypeName.SetValues(Resources.GetRes().ProductTypes.OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductTypeId).Select(x => x.ProductTypeName2).ToArray(), false); 
            }

            if (TrigChangeEvent)
            {
                if (null != ChangeType)
                    ChangeType(null, null);
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
            else if (hideType == Resources.GetRes().GetString("Income"))//Backstage
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
        /// 切换显示所有功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbIsDisplayAll_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbIsDisplayAll.Checked)
            {
                krpcmHideType.Visible = false;
                krpcmOrder.Visible = false;

            }
            else
            {
                krpcmHideType.Visible = true;
                krpcmOrder.Visible = true;
            }
        }

        
        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                krpcmProductTypeName0.Visible = false;
                krpcmProductTypeName1.Visible = false;
                krpcmProductTypeName2.Visible = false;
               


                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmProductTypeName0.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmProductTypeName1.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmProductTypeName2.Visible = true;
            }
            else
            {
                krpcmProductTypeName0.Visible = true;
                krpcmProductTypeName1.Visible = true;
                krpcmProductTypeName2.Visible = true;
                
            }
        }
        
        
    }
}
