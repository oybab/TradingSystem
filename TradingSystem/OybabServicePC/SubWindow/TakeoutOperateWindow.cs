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

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class TakeoutOperateWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 100;
        private int CurrentPage = 1;
        private int AllPage = 1;


        private bool AllowClose = false;
        private bool IsConfirm = false;
        private DialogResult dialogResult = DialogResult.None;



        public TakeoutOperateWindow(bool IsDialong = false)
        {



            InitializeComponent();
            krpdgList.RecalcMagnification();

            new CustomTooltip(this.krpdgList);
            this.krpcmProductName.SetParent(this.krpdgList);

            this.krpcmRequest.SetParent(this.krpdgList, new Func<string, string>((x) =>
            {
                RequestListWindow list = new RequestListWindow(x);
                list.ShowDialog(this);
                if (list.DialogResult == System.Windows.Forms.DialogResult.OK)
                    return list.ReturnValue;
                else
                    return x;
            }));

            this.Text = Resources.GetRes().GetString("OuterBill");
            ResetPage();
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            krpbBeginPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveFirst.png"));
            krpbPrewPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.previous.png"));
            krpbNextPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.next.png"));
            krpbEngPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.moveLast.png"));
            krpbClickToPage.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.select.png"));

            krpbBeginPage.StateCommon.Back.ImageStyle = krpbPrewPage.StateCommon.Back.ImageStyle = krpbNextPage.StateCommon.Back.ImageStyle = krpbEngPage.StateCommon.Back.ImageStyle = krpbClickToPage.StateCommon.Back.ImageStyle = PaletteImageStyle.CenterMiddle;


            krplPage.Text = Resources.GetRes().GetString("Page");

            krpbCheckout.Text = Resources.GetRes().GetString("CheckoutOrder");
            krplRemark.Text = Resources.GetRes().GetString("Remark");
            krplLanguage.Text = Resources.GetRes().GetString("Language");
            krplTotalPrice.Text = Resources.GetRes().GetString("TotalPrice");


            krpcbLanguage.Items.AddRange(Resources.GetRes().AllLangList.OrderBy(x => x.Value.LangOrder).Select(x => x.Value.LangName).ToArray());

            krpbAddByBarcode.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Barcode.png"));
            krpbAddByFastGrid.StateCommon.Back.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.FastGrid.png"));
            krpbAddByBarcode.StateCommon.Back.ImageStyle = krpbAddByFastGrid.StateCommon.Back.ImageStyle = PaletteImageStyle.Stretch;
            krpbAddByBarcode.StateCommon.Back.Draw = krpbAddByFastGrid.StateCommon.Back.Draw = InheritBool.True;

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());

            }

            //增加右键
            //删除
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            LoadContextMenu(kryptonContextMenuItemRequest, Resources.GetRes().GetString("Request2"), Resources.GetRes().GetString("RequestDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Request.png")), (sender, e) => { ChangeRequest(); });
            // 暂时隐藏. 用下拉框方式显示
            kryptonContextMenuItemRequest.Visible = false;
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });


            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ShoppingCart.ico"));


            // 是否允许减少商品
            if (Common.GetCommon().IsDecreaseProductCount())
            {
                this.krpcmCount.Minimum = -9999;
            }

            krpcmTakeoutDetailId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductName.HeaderText = Resources.GetRes().GetString("ProductName");
            krpcmPrice.HeaderText = Resources.GetRes().GetString("UnitPrice");
            krpcmCount.HeaderText = Resources.GetRes().GetString("Count");
            krpcmTotalPrice.HeaderText = Resources.GetRes().GetString("TotalPrice");
            krpcmIsPack.HeaderText = Resources.GetRes().GetString("Package");
            krpcmState.HeaderText = Resources.GetRes().GetString("State");
            krpcmRequest.HeaderText = Resources.GetRes().GetString("Request2");
            krpcmAddTime.HeaderText = Resources.GetRes().GetString("AddTime");
            krpcbPackage.Text = Resources.GetRes().GetString("Package");


            krplPhone.Text = Resources.GetRes().GetString("Phone");
            krpcbMultipleLanguage.Text = Resources.GetRes().GetString("MultiLanguage");
            krplName0.Text = string.Format("{0}-{1}", Resources.GetRes().GetString("PersonName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krplName1.Text = string.Format("{0}-{1}", Resources.GetRes().GetString("PersonName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krplName2.Text = string.Format("{0}-{1}", Resources.GetRes().GetString("PersonName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krplAddress0.Text = string.Format("{0}-{1}", Resources.GetRes().GetString("Address"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krplAddress1.Text = string.Format("{0}-{1}", Resources.GetRes().GetString("Address"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krplAddress2.Text = string.Format("{0}-{1}", Resources.GetRes().GetString("Address"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);


            ReloadProduct();

            ReloadRequest();

            //初始化
            Init();


            ShowOrHideSave();




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


            if (IsDialong)
            {
                this.ControlBox = false;
            }

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
            List<Product> products = Resources.GetRes().Products.Where(x => (x.Barcode == code || (x.IsScales == 1 && code.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).ToList();

            if (products.Count > 1)
            {
                this.BeginInvoke(new Action(() =>
                {
                    FastGridWindow window = new FastGridWindow(true, true);
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
            if (null != products)
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

                        if (product.IsScales == 1)
                        {
                            AddToGrid("*", "-1", product.ProductId, product.Price, double.Parse(code.Substring(7,2) + "." + code.Substring(9,3)), Math.Round(product.Price * 1, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
                           
                        }
                        else
                        {
                            AddToGrid("*", "-1", product.ProductId, product.Price, 1, Math.Round(product.Price * 1, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
                            
                        }

                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                            krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;

                        Takeout takeout;
                        List<TakeoutDetail> takeoutDetails;
                        Calc(out takeoutDetails, out takeout);

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

            if (Resources.GetRes().DefaultOrderLang == -1)
            {
                krpcbLanguage.SelectedItem = Resources.GetRes().MainLang.LangName;
            }
            else
            {
                krpcbLanguage.SelectedIndex = Resources.GetRes().DefaultOrderLang;

            }

            // 刷新第二屏语言
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex, -1);
            }


            krpcbMultipleLanguage_CheckedChanged(null, null);



            Takeout takeout;
            List<TakeoutDetail> takeoutDetails;
            Calc(out takeoutDetails, out takeout);


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
        /// <param name="IsPack"></param>
        /// <param name="State"></param>
        /// <param name="AddTime"></param>
        private void AddToGrid(string editMark, string Id, long productId, double Price, double Count, double TotalPrice, long IsPack, string State, string Request, string AddTime)
        {
            string productName = "";
            string AddTimeStr = "";
            string RequestNames = "";

            try
            {

                if (!string.IsNullOrWhiteSpace(Request))
                {
                    List<long> requestsIdList = Request.Split(',').Select(x => long.Parse(x)).ToList();
                    List<Request> requestList = Resources.GetRes().Requests.Where(x => requestsIdList.Contains(x.RequestId)).Distinct().ToList();

                    if (Resources.GetRes().MainLangIndex == 0)
                        RequestNames = string.Join("&", requestList.Select(x => x.RequestName0));
                    else if (Resources.GetRes().MainLangIndex == 1)
                        RequestNames = string.Join("&", requestList.Select(x => x.RequestName1));
                    else if (Resources.GetRes().MainLangIndex == 2)
                        RequestNames = string.Join("&", requestList.Select(x => x.RequestName2));


                }

                AddTimeStr = DateTime.ParseExact(AddTime, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");

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
                krpdgList.Rows.Insert(0, editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), IsPack.ToString(), State, RequestNames, AddTimeStr);
                // 将新增加的价格暂时冻结掉, 等选择产品后才可以知道产品是否允许修改价格
                krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = true;
            }
            else
                krpdgList.Rows.Add(editMark, Id, productName, Price.ToString(), Count.ToString(), TotalPrice.ToString(), IsPack.ToString(), State, RequestNames, AddTimeStr);
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


            Takeout takeout;
            List<TakeoutDetail> takeoutDetails;
            Calc(out takeoutDetails, out takeout);
        }


        /// <summary>
        /// 验证产品和数量,免得选择一个数量不够的产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void krpdgList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if ((null != krpdgList.EditingControl || null != currentCell) && (e.FormattedValue as string) != temp)
            {
                // 先确定当前行是不是选择产品和数量, 如果是那就机选一下该产品剩余数是否够购买当前选择的数,够就继续, 不够则直接返回上一个值
                if (e.ColumnIndex == 2 || e.ColumnIndex == 4)
                {
                    string productName = null;
                    double count = 0;

                    if (e.ColumnIndex == 2)
                        productName = e.FormattedValue.ToString(); 
                    else
                        productName = krpdgList.Rows[e.RowIndex].Cells["krpcmProductName"].Value.ToString();

                    if (e.ColumnIndex == 4)
                    {
                        count = Math.Round(double.Parse(e.FormattedValue.ToString()), 3);
                        if (count == 0)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                    else
                        count = Math.Round(double.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmCount"].Value.ToString()), 3);

                    Product product = null;
                    if (Resources.GetRes().MainLangIndex == 0)
                        product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 1)
                        product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                    else if (Resources.GetRes().MainLangIndex == 2)
                        product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();

                    if (null == product)
                        return;


                    // 根据是否修改价格以及权限解冻, 或者冻结
                    if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = false;
                    }
                    else
                    {
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].ReadOnly = true;
                    }


                    long Id = long.Parse(krpdgList.Rows[e.RowIndex].Cells["krpcmTakeoutDetailId"].Value.ToString());

                    // 判断是否已存在!
                    if (Id == -1 && e.ColumnIndex == 2 && ProductAlreadyExists(productName, e.RowIndex))
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("CurrentProductAlreadyExists"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Cancel = true;
                        krpdgList.CancelEdit();
                    }

                    if (e.ColumnIndex == 2)
                    {
                        // 将产品原始价格放进去
                        krpdgList.Rows[e.RowIndex].Cells["krpcmPrice"].Value = product.Price.ToString();
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
            if (e.RowIndex > -1 && e.ColumnIndex > -1 && null != (krpdgList.Rows[e.RowIndex].Cells[e.ColumnIndex] as KryptonDataGridViewComboBoxCell))
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
                //hook.RemoveHook();
                Notification.Instance.NotificationBarcodeReader -= Instance_NotificationBarcodeReader;
                Common.GetCommon().OpenPriceMonitor(null);
                // 刷新第二屏幕
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
                }
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("OrderDetails"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }




        /// <summary>
        /// 查看数据
        /// </summary>
        private void Add()
        {
            OpenPageTo(1);
            AddToGrid("*", "-1", 0, 0, 1, 0, 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmTakeoutDetailId"].Value.ToString());

                //如果是没添加过的记录,就直接删除
                if (Id == -1)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);

                    Takeout takeout;
                    List<TakeoutDetail> takeoutDetails;
                    Calc(out takeoutDetails, out takeout);
                }
                else
                {

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
                    kryptonContextMenuItemRequest.Enabled = false;
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

                    if (krpdgList.SelectedRows[0].Cells["krpcmState"].Value.ToString() != GetOrderDetailsState(3))
                        kryptonContextMenuItemDelete.Enabled = true;

                }

                // 确认的时候不能新增
                if (!IsConfirm)
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
                    {
                        if (krpdgList.SelectedRows[0].Cells["krpcmState"].Value.ToString() != GetOrderDetailsState(3))
                            Delete();
                    }
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
            else if (e.KeyCode == Keys.F1 && krpdgList.Rows.Count > 0 && null != krpdgList.CurrentCell && krpdgList.CurrentCell.ColumnIndex == 8 && !krpdgList.CurrentCell.IsInEditMode)
            {
                krpdgList.BeginEdit(true);

                SendKeys.SendWait("{F1}");
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
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName0).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName1).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] pro = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName2).ToArray();
                    krpcmProductName.SetValues(pro, false);
                }
            }
        }


        private double lastTotal = 0;
        private double lastOriginalTotalPrice = 0;
        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="details"></param>
        /// <param name="import"></param>
        /// <param name="IgnoreError"></param>
        /// <param name="OnlyTotal"></param>
        private void Calc(out List<TakeoutDetail> details, out Takeout takeout, bool IgnoreError = true, bool OnlyTotal = false, bool IgnoreNotConfirm = true, bool IgnoreCanceld = true, long IgnoreCancelId = -999)
        {
            bool IsError = false;
            details = new List<TakeoutDetail>();
            List<TakeoutDetail> detailsAll = new List<TakeoutDetail>();
            takeout = new Takeout();

            if (!OnlyTotal)
            {
                for (int i = krpdgList.Rows.Count - 1; i >= 0; i--)
                {

                    TakeoutDetail takeoutDetails = new TakeoutDetail();


                    try
                    {
                        string productName = krpdgList.Rows[i].Cells["krpcmProductName"].Value.ToString();
                        Product product = null;
                        if (Resources.GetRes().MainLangIndex == 0)
                            product = Resources.GetRes().Products.Where(x => x.ProductName0 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            product = Resources.GetRes().Products.Where(x => x.ProductName1 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            product = Resources.GetRes().Products.Where(x => x.ProductName2 == productName && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();

                        takeoutDetails.ProductId = product.ProductId;

                        takeoutDetails.IsPack = long.Parse(krpdgList.Rows[i].Cells["krpcmIsPack"].Value.ToString());
                        takeoutDetails.Count = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmCount"].Value.ToString()), 3);

                        takeoutDetails.TakeoutDetailId = long.Parse(krpdgList.Rows[i].Cells["krpcmTakeoutDetailId"].Value.ToString());

                        if (takeoutDetails.TakeoutDetailId == -1)
                        {

                            if (!string.IsNullOrWhiteSpace(krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString()))
                            {

                                List<Request> requests = new List<Request>();
                                if (Resources.GetRes().MainLangIndex == 0)
                                    requests = Resources.GetRes().Requests.Where(x => krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString().Split('&').Contains(x.RequestName0)).ToList<Request>();
                                else if (Resources.GetRes().MainLangIndex == 1)
                                    requests = Resources.GetRes().Requests.Where(x => krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString().Split('&').Contains(x.RequestName1)).ToList<Request>();
                                else if (Resources.GetRes().MainLangIndex == 2)
                                    requests = Resources.GetRes().Requests.Where(x => krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString().Split('&').Contains(x.RequestName2)).ToList<Request>();



                                if (requests.Count > 0)
                                {
                                    requests = requests.Distinct().ToList();

                                    takeoutDetails.Request = string.Join(",", requests.Select(x => x.RequestId.ToString()));

                                }
                                else
                                {
                                    if (!string.IsNullOrWhiteSpace(krpdgList.Rows[i].Cells["krpcmRequest"].Value.ToString()))
                                    {
                                        throw new Exception(string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("RequestName")));
                                    }

                                }
                            }

                        }
                        else
                        {

                        }

                        takeoutDetails.State = GetOrderDetailsStateNo(krpdgList.Rows[i].Cells["krpcmState"].Value.ToString());

                        krpdgList.Rows[i].Cells["krpcmPrice"].Value = takeoutDetails.Price = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmPrice"].Value.ToString()), 2);
                        krpdgList.Rows[i].Cells["krpcmTotalPrice"].Value = takeoutDetails.TotalPrice = Math.Round(takeoutDetails.Price * takeoutDetails.Count, 2);
                        takeoutDetails.OriginalTotalPrice = Math.Round(product.Price * takeoutDetails.Count, 2);

                        takeoutDetails.TotalCostPrice = Math.Round(product.CostPrice * takeoutDetails.Count, 2);

                        if (product.CostPrice == 0 && null != product.ProductParentId)
                        {
                            Product parentProduct = Resources.GetRes().Products.FirstOrDefault(x => x.ProductId == product.ProductParentId);

                            if (null != parentProduct)
                            {
                                double price = Math.Round(parentProduct.CostPrice / product.ProductParentCount, 2);
                                takeoutDetails.TotalCostPrice = Math.Round(price * takeoutDetails.Count, 2);
                            }
                        }


                        if (krpdgList.Rows[i].Cells["krpcmEdit"].Value.Equals("*"))
                            details.Add(takeoutDetails);
                        detailsAll.Add(takeoutDetails);

                    }
                    catch
#if DEBUG
                       (Exception ex)
#endif
                    {
                        krpdgList.Rows[i].Cells["krpcmPrice"].Value = "?";
                        krpdgList.Rows[i].Cells["krpcmTotalPrice"].Value = "?";

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

                if (IsError)
                    return;
            }


            
            if (!OnlyTotal)
            {
                IEnumerable<TakeoutDetail> totalDetails = detailsAll;

                if (IgnoreNotConfirm)
                    totalDetails = totalDetails.Where(x => x.State != 1);
                if (IgnoreCanceld)
                    totalDetails = totalDetails.Where(x => x.State != 3);

                lastTotal = Math.Round(totalDetails.Sum(x => x.TotalPrice), 2);
                lastOriginalTotalPrice = Math.Round(totalDetails.Sum(x => x.OriginalTotalPrice), 2);

                if (IgnoreCancelId != -999 && totalDetails.Where(x => x.TakeoutDetailId == IgnoreCancelId).Count() > 0)
                {
                    lastTotal = Math.Round(lastTotal - totalDetails.Where(x => x.TakeoutDetailId == IgnoreCancelId).FirstOrDefault().TotalPrice, 2);
                    lastOriginalTotalPrice = Math.Round(lastOriginalTotalPrice - totalDetails.Where(x => x.TakeoutDetailId == IgnoreCancelId).FirstOrDefault().OriginalTotalPrice, 2);
                }
               
            }






            takeout.Lang = Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex;
                takeout.IsPack = krpcbPackage.Checked ? 1 : 0;
            

            takeout.TotalPaidPrice = Math.Round(takeout.MemberPaidPrice + takeout.PaidPrice, 2);


            


            

            krptTotalPrice.Text = (takeout.TotalPrice = lastTotal).ToString();
            takeout.OriginalTotalPrice = lastOriginalTotalPrice;



            double balancePrice = 0;
            balancePrice = Math.Round(takeout.TotalPaidPrice - takeout.TotalPrice, 2);


            if (balancePrice > 0)
            {
                takeout.KeepPrice = balancePrice;
                takeout.BorrowPrice = 0;
            }

            else if (balancePrice < 0)
            {
                takeout.BorrowPrice = balancePrice;
                takeout.KeepPrice = 0;
            }

            else if (balancePrice == 0)
            {
                takeout.BorrowPrice = 0;
                takeout.KeepPrice = 0;
            }

            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(takeout.BorrowPrice.ToString());
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(takeout, details, null, true));
            }


            if (OnlyTotal)
                return;



            if (!string.IsNullOrWhiteSpace(krptbRemark.Text))
                takeout.Remark = krptbRemark.Text;



            ShowOrHideSave();

        }



        /// <summary>
        /// 打开请求页面
        /// </summary>
        /// <param name="requests"></param>
        /// <param name="rowIndex"></param>
        /// <param name="columnIndex"></param>
        private void ChangeRequest()
        {
            int rowIndex = krpdgList.SelectedRows[0].Index;
            int columnIndex = krpdgList.SelectedRows[0].Cells["krpcmRequest"].ColumnIndex;
            string requests = krpdgList.SelectedRows[0].Cells["krpcmRequest"].Value.ToString();
            RequestListWindow list = new RequestListWindow(requests);
            list.ShowDialog(this);
            if (list.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                krpdgList.Rows[rowIndex].Cells[columnIndex].Value = list.ReturnValue;
                krpdgList.Rows[rowIndex].Cells[0].Value = "*";
            }
        }

        


        /// <summary>
        /// 结账
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbCheckout_Click(object sender, EventArgs e)
        {

            Takeout takeout;
            List<TakeoutDetail> details;

            bool IgnoreNotConfirm = true;

            if (IsConfirm)
                IgnoreNotConfirm = false;

            try
            {
                Calc(out details, out takeout, false, false, IgnoreNotConfirm);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }), false, Resources.GetRes().GetString("SaveFailt"));
                return;
            }


            CopyForHide();



            takeout.Phone = GetValueOrNull(krptPhone.Text);

            takeout.Name0 =  GetValueOrNull(krptName0.Text);
            takeout.Name1 = GetValueOrNull(krptName1.Text);
            takeout.Name2 = GetValueOrNull(krptName2.Text);

            takeout.Address0 = GetValueOrNull(krptAddress0.Text);
            takeout.Address1 = GetValueOrNull(krptAddress1.Text);
            takeout.Address2 = GetValueOrNull(krptAddress2.Text);

            takeout.tb_takeoutdetail = details;

            AllowClose = false;
            // 如果是结账
            TakeoutCheckoutWindow window = new TakeoutCheckoutWindow(takeout);
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
                this.dialogResult = System.Windows.Forms.DialogResult.OK;
                AllowClose = true;
            }
            // 重试代表订单数据不是最新的, 重新获取
            else if (dialogResult == System.Windows.Forms.DialogResult.Retry)
            {
                this.dialogResult = System.Windows.Forms.DialogResult.Retry;
                AllowClose = true;
            }

            if (AllowClose)
            {
                krpdgList.Rows.Clear();


                krptTotalPrice.Text = "0";
                krptbRemark.Text = "";

                lastTotal = lastOriginalTotalPrice = 0;

                krptPhone.Text = "";

                krptName0.Text = "";
                krptName1.Text = "";
                krptName2.Text = "";

                krptAddress0.Text = "";
                krptAddress1.Text = "";
                krptAddress2.Text = "";
                krpcbMultipleLanguage.Checked = false;


                Takeout takeout2;
                List<TakeoutDetail> takeoutDetails;
                Calc(out takeoutDetails, out takeout2);
            }
        }


        /// <summary>
        /// 获取订单详情状态编号
        /// </summary>
        /// <param name="hideType"></param>
        /// <returns></returns>
        private int GetOrderDetailsStateNo(string orderState)
        {
            if (orderState == "-")
                return 0;
            else if (orderState == Resources.GetRes().GetString("Requesting"))
                return 1;
            else if (orderState == Resources.GetRes().GetString("Confirmed"))
                return 2;
            else if (orderState == Resources.GetRes().GetString("Canceld"))
                return 3;
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        /// <summary>
        /// 获取订单详情状态
        /// </summary>
        /// <param name="orderStateNo"></param>
        /// <returns></returns>
        private string GetOrderDetailsState(long orderStateNo)
        {
            if (orderStateNo == 0)
                return "-";
            else if (orderStateNo == 1)
                return Resources.GetRes().GetString("Requesting");
            else if (orderStateNo == 2)
                return Resources.GetRes().GetString("Confirmed");
            else if (orderStateNo == 3)
                return Resources.GetRes().GetString("Canceld");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }


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
        /// 通过条形码添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ktpbAddByBarcode_Click(object sender, EventArgs e)
        {
            FastGridWindow window = new FastGridWindow(true, true);
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
            FastGridWindow window = new FastGridWindow(true, false);
            window.ShowDialog(this);


            HandleFastGrid(window);

        }

        /// <summary>
        /// 处理快速搜索
        /// </summary>
        /// <param name="window"></param>
        private void HandleFastGrid(FastGridWindow window)
        {
            if (window.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var item in window.ReturnValue)
                {
                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.Key && (x.HideType == 0 || x.HideType == 2)).FirstOrDefault();

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
                        AddToGrid("*", "-1", product.ProductId, item.Value.Price, item.Value.Count, Math.Round(item.Value.Price * item.Value.Count, 2), 0, GetOrderDetailsState(0), "", DateTime.Now.ToString("yyyyMMddHHmmss"));
                        if (product.PriceChangeMode == 1 && Common.GetCommon().IsTemporaryChangePrice())
                            krpdgList.Rows[0].Cells["krpcmPrice"].ReadOnly = false;
                    }



                }

                Takeout takeout;
                List<TakeoutDetail> takeoutDetails;
                Calc(out takeoutDetails, out takeout);
            }
        }
       


        /// <summary>
        /// 重新加载请求列表框
        /// </summary>
        internal void ReloadRequest()
        {
            krpcmRequest.SetValues(null, false);
            if (Resources.GetRes().Requests.Where(x => x.IsEnable == 1).Count() > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmRequest.SetValues(Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).Select(x => x.RequestName0).ToArray(), true);
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmRequest.SetValues(Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).Select(x => x.RequestName1).ToArray(), true);
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmRequest.SetValues(Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).Select(x => x.RequestName2).ToArray(), true);
            }


        }

        /// <summary>
        /// 显示或隐藏保存按钮
        /// </summary>
        private void ShowOrHideSave()
        {
            if (krpdgList.Rows.Count > 0 && Common.GetCommon().IsIncomeTradingManage())
                krpbCheckout.Visible = true;
            else
                krpbCheckout.Visible = false;
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
        /// 显示所有语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbMultipleLanguage_CheckedChanged(object sender, EventArgs e)
        {

            if (!krpcbMultipleLanguage.Checked)
            {
                flpName2.Visible = false;
                flpName1.Visible = false;
                flpName0.Visible = false;

                flpAddress2.Visible = false;
                flpAddress1.Visible = false;
                flpAddress0.Visible = false;

                if (Resources.GetRes().MainLangIndex == 0)
                {
                    flpName0.Visible = true;
                    flpAddress0.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    flpName1.Visible = true;
                    flpAddress1.Visible = true;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    flpName2.Visible = true;
                    flpAddress2.Visible = true;
                }

                this.Size = new System.Drawing.Size(this.Width, this.Height - 80);
            }
            else
            {
                flpName2.Visible = true;
                flpName1.Visible = true;
                flpName0.Visible = true;

                flpAddress2.Visible = true;
                flpAddress1.Visible = true;
                flpAddress0.Visible = true;

                this.Size = new System.Drawing.Size(this.Width, this.Height + 80);
            }
        }

        /// <summary>
        /// 隐藏功能时复制名称
        /// </summary>
        private void CopyForHide()
        {
            if (!krpcbMultipleLanguage.Checked)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    if (string.IsNullOrWhiteSpace(krptName1.Text))
                        krptName1.Text = krptName0.Text;
                    if (string.IsNullOrWhiteSpace(krptName2.Text))
                        krptName2.Text = krptName0.Text;

                    if (string.IsNullOrWhiteSpace(krptAddress1.Text))
                        krptAddress1.Text = krptAddress0.Text;
                    if (string.IsNullOrWhiteSpace(krptAddress2.Text))
                        krptAddress2.Text = krptAddress0.Text;
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    if (string.IsNullOrWhiteSpace(krptName0.Text))
                        krptName0.Text = krptName1.Text;
                    if (string.IsNullOrWhiteSpace(krptName2.Text))
                        krptName2.Text = krptName1.Text;

                    if (string.IsNullOrWhiteSpace(krptAddress0.Text))
                        krptAddress0.Text = krptAddress1.Text;
                    if (string.IsNullOrWhiteSpace(krptAddress2.Text))
                        krptAddress2.Text = krptAddress1.Text;
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    if (string.IsNullOrWhiteSpace(krptName1.Text))
                        krptName1.Text = krptName2.Text;
                    if (string.IsNullOrWhiteSpace(krptName0.Text))
                        krptName0.Text = krptName2.Text;

                    if (string.IsNullOrWhiteSpace(krptAddress1.Text))
                        krptAddress1.Text = krptAddress2.Text;
                    if (string.IsNullOrWhiteSpace(krptAddress0.Text))
                        krptAddress0.Text = krptAddress2.Text;
                }
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

        private void krpcbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 刷新第二屏语言
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangName(krpcbLanguage.SelectedItem.ToString()).LangIndex, -1);
            }
        }


    }
}
