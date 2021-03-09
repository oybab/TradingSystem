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
using Oybab.ServicePC.DialogWindow.ConditionWindow;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class ProductWindow : KryptonForm
    {
        //页数变量
        private int ListCount = 50;
        private int CurrentPage = 1;
        private int AllPage = 1;
        private List<Product> resultList = null;

        public ProductWindow()
        {
            InitializeComponent();
            krpdgList.RecalcMagnification();
            
            Notification.Instance.NotificationProduct += (obj, value, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.ProductId == value.ProductId || x.ProductParentId == value.ProductId)) krpdgList.Enabled = false; })); };
            Notification.Instance.NotificationPprs += (obj, values, value2, args) => { this.BeginInvoke(new Action(() => { if (krpdgList.Enabled && resultList.Any(x => x.ProductId == value2.ProductId || x.ProductParentId == value2.ProductId)) krpdgList.Enabled = false; })); };

            new CustomTooltip(this.krpdgList);
            this.ControlBox = false;
            this.krpcmProductTypeName.SetParent(this.krpdgList);
            this.krpcmProductParentName.SetParent(this.krpdgList);
            this.krpcmPrinters.SetParent(this.krpdgList, new Func<string, string>((x) =>
            {
                PrinterListWindow list = new PrinterListWindow(x);
                list.ShowDialog(this);
                if (list.DialogResult == System.Windows.Forms.DialogResult.OK)
                    return list.ReturnValue;
                else
                    return x;
            }));

            
            krptProductName.Font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            this.Text = Resources.GetRes().GetString("ProductManager");
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
            krplProductName.Text = Resources.GetRes().GetString("ProductName");
            krplProductTypeName.Text = Resources.GetRes().GetString("ProductTypeName");


            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krplPage.StateCommon.Padding = new Padding(0, 0, 0, int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptCurrentPage.Location = new Point(krptCurrentPage.Location.X, krptCurrentPage.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptProductName.Location = new Point(krptProductName.Location.X, krptProductName.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());
                krpcbProductType.Location = new Point(krpcbProductType.Location.X, krpcbProductType.Location.Y + (int.Parse(Resources.GetRes().GetString("HightFix")) / 2).RecalcMagnification2());
            }

            


            //增加右键
            //添加
            LoadContextMenu(kryptonContextMenuItemAdd, Resources.GetRes().GetString("Add"), Resources.GetRes().GetString("AddDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Add.png")), (sender, e) => { Add(); });
            //保存
            LoadContextMenu(kryptonContextMenuItemSave, Resources.GetRes().GetString("Save"), Resources.GetRes().GetString("SaveDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Save.png")), (sender, e) => { Save(); });
            //删除
            LoadContextMenu(kryptonContextMenuItemDelete, Resources.GetRes().GetString("Delete"), Resources.GetRes().GetString("DeleteDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Delete.png")), (sender, e) => { Delete(); });
            // 显示产品到期时间
            LoadContextMenu(kryptonContextMenuItemChangeTime, Resources.GetRes().GetString("ChangeTime"), Resources.GetRes().GetString("ProductExpiredDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeTime.png")), (sender, e) => { ChangeTime(); });
            //打印
            LoadContextMenu(kryptonContextMenuItemPrint, Resources.GetRes().GetString("Print"), Resources.GetRes().GetString("PrintBarcodeDescription"), Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.PrintBarcode.png")), (sender, e) => { Print(); });
            
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Product.ico"));

            //初始化
            Init();

            krplAllPrice.Text = Resources.GetRes().PrintInfo.PriceSymbol + "0";
            krplAllCostPrice.Text = Resources.GetRes().PrintInfo.PriceSymbol + "0";


            if (Resources.GetRes().AdminModel.Mode != 2)
            {
                krplAllPrice.Visible = krplAllCostPrice.Visible = krplPriceSperator.Visible = false;
            }
            

            //防止直接增加数据的时候插入本地队列失败
            resultList = new List<Product>();

            // 扫条码
            Notification.Instance.NotificationBarcodeReader += Instance_NotificationBarcodeReader;
        }

        /// <summary>
        /// 扫条码
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
            if (!this.ContainsFocus)
                return;


            if (krptProductName.Focused)
            {
                this.BeginInvoke(new Action(() =>
                {
                    krptProductName.Text = code;

                }));
            }
            else if (null != krpdgList.CurrentCell && krpdgList.CurrentCell.ColumnIndex == 7 && krpdgList.CurrentCell.Selected)
            {
                this.BeginInvoke(new Action(() =>
                {
                    krpdgList.CurrentCell.Value = code;
                    krpdgList.Rows[krpdgList.CurrentCell.RowIndex].Cells["krpcmEdit"].Value = "*";
                    
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
            krpcmProductId.HeaderText = Resources.GetRes().GetString("Id");
            krpcmProductTypeName.HeaderText = Resources.GetRes().GetString("ProductTypeName");
            krpcmProductName0.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("ProductName"), Resources.GetRes().GetMainLangByMainLangIndex(0).LangName);
            krpcmProductName1.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("ProductName"), Resources.GetRes().GetMainLangByMainLangIndex(1).LangName);
            krpcmProductName2.HeaderText = string.Format("{0}-{1}", Resources.GetRes().GetString("ProductName"), Resources.GetRes().GetMainLangByMainLangIndex(2).LangName);
            krpcmBarcode.HeaderText = Resources.GetRes().GetString("BarcodeNo");
            krpcmPrice.HeaderText = Resources.GetRes().GetString("UnitPrice");
            krpcmPriceChangeMode.HeaderText = Resources.GetRes().GetString("ChangePrice");
            krpcmCostPrice.HeaderText = Resources.GetRes().GetString("CostPrice");
            krpcmCostPriceChangeMode.HeaderText = Resources.GetRes().GetString("ChangeCostPrice");
            krpcmBalanceCount.HeaderText = Resources.GetRes().GetString("BalanceCount");
            krpcmWarningCount.HeaderText = Resources.GetRes().GetString("WarningCount");
            krpcmIsBindCount.HeaderText = Resources.GetRes().GetString("IsBindCount");
            krpcmImageName.HeaderText = Resources.GetRes().GetString("ImageName");
            krpcmOrder.HeaderText = Resources.GetRes().GetString("Order");
            krpcmHideType.HeaderText = Resources.GetRes().GetString("DisplayType");
            krpcmPrinters.HeaderText = Resources.GetRes().GetString("PrinterName");
            krpcmExpiredTime.HeaderText = Resources.GetRes().GetString("ExpiredTime");
            krpcmProductParentName.HeaderText = Resources.GetRes().GetString("ProductParentName");
            krpcmProductParentCount.HeaderText = Resources.GetRes().GetString("ProductParentCount");
            krpcmIsScales.HeaderText = Resources.GetRes().GetString("IsBindWeigh");

            ReloadProductType();
            ReloadProductTextbox();
            ReloadPrinters();

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

            //查找数据
            resultList = Resources.GetRes().Products.OrderByDescending(x=>x.Order).ThenByDescending(x=>x.ProductId).ToList();
            if (krpcbProductType.SelectedIndex > 0)
            {
                string krpcbProductTypeSelectedItemToString = krpcbProductType.SelectedItem.ToString();
                if (Resources.GetRes().MainLangIndex == 0)
                    resultList = resultList.Where(x => x.ProductTypeId == Resources.GetRes().ProductTypes.Where(y => y.ProductTypeName0 == krpcbProductTypeSelectedItemToString).FirstOrDefault().ProductTypeId).ToList();
                else if (Resources.GetRes().MainLangIndex == 1)
                    resultList = resultList.Where(x => x.ProductTypeId == Resources.GetRes().ProductTypes.Where(y => y.ProductTypeName1 == krpcbProductTypeSelectedItemToString).FirstOrDefault().ProductTypeId).ToList();
                else if (Resources.GetRes().MainLangIndex == 2)
                    resultList = resultList.Where(x => x.ProductTypeId == Resources.GetRes().ProductTypes.Where(y => y.ProductTypeName2 == krpcbProductTypeSelectedItemToString).FirstOrDefault().ProductTypeId).ToList();
            }
            else if (krpcbProductType.SelectedIndex == -1)
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("ProductTypeName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string krptProductNameTextTrim = krptProductName.Text.Trim();
            if (krptProductNameTextTrim != "")
                resultList = resultList.Where(x => x.Barcode == krptProductNameTextTrim || ( x.ProductName0.Contains(krptProductNameTextTrim, StringComparison.OrdinalIgnoreCase) || x.ProductName1.Contains(krptProductNameTextTrim, StringComparison.OrdinalIgnoreCase) || x.ProductName2.Contains(krptProductNameTextTrim, StringComparison.OrdinalIgnoreCase))).ToList();
            

            //设定页面数据
            ResetPage();
            if (resultList.Count() > 0)
            {
                CalcProductPriceAndCostPrice();


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
                krplAllPrice.Text = Resources.GetRes().PrintInfo.PriceSymbol + "0";
                krplAllCostPrice.Text = Resources.GetRes().PrintInfo.PriceSymbol + "0";
            }

        }

        /// <summary>
        /// 计算产品价格
        /// </summary>
        private void CalcProductPriceAndCostPrice()
        {
            double validPrice = Math.Round(resultList.Where(x => x.Price != 0).Sum(x => x.Price * x.BalanceCount), 2);
            double validCostPrice = Math.Round(resultList.Where(x => x.CostPrice != 0).Sum(x => x.CostPrice * x.BalanceCount), 2);


            double invalidPrice = 0;

            foreach (var item in resultList.Where(x => x.Price == 0 && x.BalanceCount != 0))
            {
                Product childProduct = Resources.GetRes().Products.Where(x => x.ProductParentId == item.ProductId && x.Price != 0).FirstOrDefault();
                if (null != childProduct)
                {
                    invalidPrice += (childProduct.ProductParentCount / item.BalanceCount) * childProduct.Price;
                }

                else if (null != item.ProductParentId)
                {
                    Product parentProduct = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductParentId && x.Price != 0).FirstOrDefault();
                    if (null != parentProduct)
                    {
                        invalidPrice += Math.Round(parentProduct.Price / item.ProductParentCount * item.BalanceCount, 2);
                    }
                }
            }

            double invalidCostPrice = 0;
            foreach (var item in resultList.Where(x => x.CostPrice == 0 && x.BalanceCount != 0))
            {
                Product childProduct = Resources.GetRes().Products.Where(x => x.ProductParentId == item.ProductId && x.CostPrice != 0).FirstOrDefault();
                if (null != childProduct)
                {
                    invalidCostPrice += (childProduct.ProductParentCount / item.BalanceCount) * childProduct.CostPrice;
                }

                else if (null != item.ProductParentId)
                {
                    Product parentProduct = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductParentId && x.CostPrice != 0).FirstOrDefault();
                    if (null != parentProduct)
                    {
                        invalidCostPrice += Math.Round(parentProduct.CostPrice / item.ProductParentCount * item.BalanceCount, 2);
                    }
                }
            }

            krplAllPrice.Text = string.Format("{1}{0}", Math.Round(validPrice + invalidPrice, 2), Resources.GetRes().PrintInfo.PriceSymbol);
            krplAllCostPrice.Text = string.Format("{1}{0}", Math.Round(validCostPrice + invalidCostPrice, 2), Resources.GetRes().PrintInfo.PriceSymbol);
        }

        /// <summary>
        ///  高级搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSearch_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                //为未保存数据而忽略当前操作
                if (!IgnoreOperateForSave())
                    return;

                if (!krpdgList.Enabled)
                {
                    krpdgList.Enabled = true;
                    krpdgList.Rows.Clear();
                }

                ProductConditionWindow condition = new ProductConditionWindow();
                
                condition.ShowDialog(this);


                List<Product> item = condition.ReturnValue as List<Product>;
                if (null != item)
                {
                    //查找数据
                    resultList = item;


                    CalcProductPriceAndCostPrice();


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

                        krplAllPrice.Text = Resources.GetRes().PrintInfo.PriceSymbol + "0";
                        krplAllCostPrice.Text = Resources.GetRes().PrintInfo.PriceSymbol + "0";
                    }
                }
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
                AddToGrid("", item.ProductId.ToString(), item.ProductName0, item.ProductName1, item.ProductName2, item.ProductTypeId, null, item.IsScales, item.Barcode ?? "", item.Price, item.PriceChangeMode, item.CostPrice, item.CostPriceChangeMode, item.BalanceCount, item.WarningCount, item.IsBindCount, item.ExpiredTime, item.ProductParentId ?? 0, null, item.ProductParentCount, item.ImageName ?? "", GetHideType(item.HideType), item.Order);
            }

            SetColor();
        }



        /// <summary>
        /// 添加到列表
        /// </summary>
        /// <param name="editMark"></param>
        /// <param name="Id"></param>
        /// <param name="productNameZH"></param>
        /// <param name="productNameUG"></param>
        /// <param name="productNameEn"></param>
        /// <param name="productTypeId"></param>
        /// <param name="productTypeNameOld"></param>
        /// <param name="IsScales"></param>
        /// <param name="barCode"></param>
        /// <param name="price"></param>
        /// <param name="priceChangeMode"></param>
        /// <param name="costPrice"></param>
        /// <param name="costPriceChangeMode"></param>
        /// <param name="BalanceCount"></param>
        /// <param name="WarningCount"></param>
        /// <param name="IsBindCount"></param>
        /// <param name="ExpiredTime"></param>
        /// <param name="ProductParentId"></param>
        /// <param name="ProductParentNameOld"></param>
        /// <param name="ProductParentCount"></param>
        /// <param name="imageName"></param>
        /// <param name="HideType"></param>
        /// <param name="Order"></param>
        private void AddToGrid(string editMark, string Id, string productName0, string productName1, string productName2, long productTypeId, string productTypeNameOld, long IsScales, string barCode, double price, long priceChangeMode, double costPrice, long costPriceChangeMode, double BalanceCount, double WarningCount, long IsBindCount, long ExpiredTime, long ProductParentId, string ProductParentNameOld, double ProductParentCount, string imageName, string HideType, long Order)
        {
            string productTypeName = "";
            string printers = "";
            long productId = long.Parse(Id);
            string ExpiredTimeStr = "";
            string ProductParentNameStr = "";

            try
            {
                if (ExpiredTime != 0)
                    ExpiredTimeStr = DateTime.ParseExact(ExpiredTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm");

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            if (productId > 0)
            {
                List<long> printersIdList = Resources.GetRes().Pprs.Where(x => x.ProductId == productId).Select(x=>x.PrinterId).ToList();
                List<Printer> printerList = Resources.GetRes().Printers.Where(x=> printersIdList.Contains(x.PrinterId)).Distinct().ToList();
                if (Resources.GetRes().MainLangIndex == 0)
                    printers = string.Join("&", printerList.Select(x => x.PrinterName0));
                else if (Resources.GetRes().MainLangIndex == 1)
                    printers = string.Join("&", printerList.Select(x => x.PrinterName1));
                else if (Resources.GetRes().MainLangIndex == 2)
                    printers = string.Join("&", printerList.Select(x => x.PrinterName2));
            }

            if (productTypeId > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    productTypeName = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId == productTypeId).Select(x => x.ProductTypeName0).FirstOrDefault().ToString();
                else if (Resources.GetRes().MainLangIndex == 1)
                    productTypeName = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId == productTypeId).Select(x => x.ProductTypeName1).FirstOrDefault().ToString();
                else if (Resources.GetRes().MainLangIndex == 2)
                    productTypeName = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeId == productTypeId).Select(x => x.ProductTypeName2).FirstOrDefault().ToString();
            }
            else if (!string.IsNullOrWhiteSpace(productTypeNameOld))
            {
                productTypeName = productTypeNameOld;
            }


            if (ProductParentId > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    ProductParentNameStr = Resources.GetRes().Products.Where(x => x.ProductId == ProductParentId).Select(x => x.ProductName0).FirstOrDefault().ToString();
                else if (Resources.GetRes().MainLangIndex == 1)
                    ProductParentNameStr = Resources.GetRes().Products.Where(x => x.ProductId == ProductParentId).Select(x => x.ProductName1).FirstOrDefault().ToString();
                else if (Resources.GetRes().MainLangIndex == 2)
                    ProductParentNameStr = Resources.GetRes().Products.Where(x => x.ProductId == ProductParentId).Select(x => x.ProductName2).FirstOrDefault().ToString();
            }
            else if (!string.IsNullOrWhiteSpace(ProductParentNameOld))
            {
                ProductParentNameStr = ProductParentNameOld;
            }

            if (editMark == "*")
                krpdgList.Rows.Insert(0, editMark, Id, productName0, productName1, productName2, productTypeName, IsScales, barCode, price.ToString(), priceChangeMode.ToString(), costPrice.ToString(), costPriceChangeMode.ToString(), BalanceCount.ToString(), WarningCount.ToString(), IsBindCount.ToString(), ExpiredTimeStr, ProductParentNameStr, ProductParentCount, imageName, printers, Order.ToString(), HideType);
            else
                krpdgList.Rows.Add(editMark, Id, productName0, productName1, productName2, productTypeName, IsScales, barCode, price.ToString(), priceChangeMode.ToString(), costPrice.ToString(), costPriceChangeMode.ToString(), BalanceCount.ToString(), WarningCount.ToString(), IsBindCount.ToString(), ExpiredTimeStr, ProductParentNameStr, ProductParentCount, imageName, printers, Order.ToString(), HideType);
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



        /// <summary>
        /// 由于多选输入框按Enter时会跳到下一个CELL, 所以这里判断一下. 除非输入框的下拉框没显示, 不然不能跳(选择打印机)
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter && this.krpdgList.IsCurrentCellInEditMode && this.krpdgList.CurrentCell.ColumnIndex == 19 && this.krpdgList.CurrentCell is Oybab.ServicePC.Tools.ComTextColumn.ComCell)    //监听回车事件 
            {
                return true;
            }
            else
                return base.ProcessCmdKey(ref msg, keyData);
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
            }else {
                Notification.Instance.NotificationBarcodeReader -= Instance_NotificationBarcodeReader;
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
                var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("IgnoreData"), Resources.GetRes().GetString("ProductManager"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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


            long isBindCount = 0;

            // 如果KEY中桌子数为0, 则绑定数量
            if (Resources.GetRes().RoomCount <= 0)
            {
                isBindCount = 1;
            }


            AddToGrid("*", "-1", "", "", "", -1, null, 0, "", 0, 1, 0, 1, 0, 0, isBindCount, 0, 0, null, 1, "", GetHideType(0), 0);
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
                if (krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value.ToString().Equals("-1"))
                {
                    Product model = new Product();
                    List<Ppr> pprs = new List<Ppr>();
                    try
                    {
                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmProductName0"], krpdgList.SelectedRows[0].Cells["krpcmProductName1"], krpdgList.SelectedRows[0].Cells["krpcmProductName2"], true, false, krpcbMultipleLanguage.Checked);

                        model.ProductId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value.ToString());
                        
                        model.ProductName0 = krpdgList.SelectedRows[0].Cells["krpcmProductName0"].Value.ToString().Trim();
                        model.ProductName1 = krpdgList.SelectedRows[0].Cells["krpcmProductName1"].Value.ToString().Trim();
                        model.ProductName2 = krpdgList.SelectedRows[0].Cells["krpcmProductName2"].Value.ToString().Trim();
                        model.IsScales = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsScales"].Value.ToString());
                        model.Barcode = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value.ToString());
                        // 准确生成条码
                        if (null != model.Barcode)
                        {
                            string resultBarcode = null;

                            if (model.IsScales == 1)
                            {
                                resultBarcode = CommonOperates.GetCommonOperates().GenerateScaleNo(model.Barcode);
                            }
                            else
                            {
                                if (model.Barcode.StartsWith("22"))
                                {
                                    model.Barcode = model.Barcode.ReplaceFirstOccurrance("22", "23");
                                }

                                resultBarcode = CommonOperates.GetCommonOperates().GenerateEAN8(model.Barcode);
                            }

                            if (!string.IsNullOrWhiteSpace(resultBarcode))
                            {
                                krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value = model.Barcode = resultBarcode;
                            }
                            else
                            {
                                krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value = "";
                                model.Barcode = null;
                            }
                        }

                        model.Price = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrice"].Value.ToString()), 2);
                        model.PriceChangeMode = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmPriceChangeMode"].Value.ToString());
                        model.CostPrice = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmCostPrice"].Value.ToString()), 2);
                        model.CostPriceChangeMode = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmCostPriceChangeMode"].Value.ToString());
                        model.BalanceCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Value.ToString()),3);
                        model.WarningCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmWarningCount"].Value.ToString()),3);
                        model.IsBindCount = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsBindCount"].Value.ToString());
                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            model.ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));
                        else
                            model.ExpiredTime = 0;
                        model.ProductParentCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductParentCount"].Value.ToString()), 3);
                        model.ImageName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmImageName"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());

                      


                        List<Printer> printer = new List<Printer>();
                        if (Resources.GetRes().MainLangIndex == 0)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName0)).ToList<Printer>();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName1)).ToList<Printer>();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName2)).ToList<Printer>();
                       
                        
                        if (printer.Count > 0)
                        {
                            printer = printer.Distinct().ToList();
                            

                            foreach (var item in printer)
                            {
                                Ppr ppr = new Ppr();
                                ppr.PrinterId = item.PrinterId;
                                ppr.ProductId = model.ProductId;

                                pprs.Add(ppr);
                            }

                            model.tb_Ppr = pprs;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString()))
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("PrinterName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            model.tb_Ppr = null;
                        }


                        //判断空
                        if (string.IsNullOrWhiteSpace(model.ProductName0) || string.IsNullOrWhiteSpace(model.ProductName1) || string.IsNullOrWhiteSpace(model.ProductName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        
                        //判断是否已存在
                        if (Resources.GetRes().Products.Where(x => (x.ProductName0.Equals(model.ProductName0, StringComparison.OrdinalIgnoreCase) || x.ProductName1.Equals(model.ProductName1, StringComparison.OrdinalIgnoreCase) || x.ProductName2.Equals(model.ProductName2, StringComparison.OrdinalIgnoreCase))).Count() > 0) // x.ProductTypeId == productType.ProductTypeId && 
                        {

                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("ProductName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }



                        // 判断产品类别
                        ProductType productType = null;
                        string productTypeName = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName"].Value.ToString();
                        if (Resources.GetRes().MainLangIndex == 0)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName0 == productTypeName).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName1 == productTypeName).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName2 == productTypeName).FirstOrDefault();

                        //先判断下拉框
                        if (null == productType)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("ProductTypeName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        model.ProductTypeId = productType.ProductTypeId;



                        // 判断产品父级
                        Product productParent = null;
                        string productParentName = krpdgList.SelectedRows[0].Cells["krpcmProductParentName"].Value.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(productParentName))
                        {
                            if (Resources.GetRes().MainLangIndex == 0)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName0 == productParentName).FirstOrDefault();
                            else if (Resources.GetRes().MainLangIndex == 1)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName1 == productParentName).FirstOrDefault();
                            else if (Resources.GetRes().MainLangIndex == 2)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName2 == productParentName).FirstOrDefault();


                            //先判断下拉框
                            if (null == productParent)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("ProductParentName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            model.ProductParentId = productParent.ProductId;
                        }
                        else
                        {
                            model.ProductParentId = null;
                        }


                        // 判断一些条码秤有关的
                        if (model.IsScales == 1)
                        {
                            // 如果条码空的
                            if (string.IsNullOrWhiteSpace(model.Barcode))
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("BarcodeNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 如果条码已存在
                            if (null != model.Barcode && Resources.GetRes().Products.Where(x => x.IsScales == 1 && x.Barcode == model.Barcode).Count() > 0)
                            {

                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("BarcodeNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 如果价格大于9999元
                            if (model.Price >= 10000)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("Price")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

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
                            List<Ppr> pprsList;
                            bool result = OperatesService.GetOperates().ServiceAddProduct(model, pprs, out pprsList);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value = model.ProductId;
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    Resources.GetRes().Pprs.AddRange(pprsList);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    resultList.Insert(0, model);
                                    Resources.GetRes().Products.Add(model);
                                    ReloadProductTextbox(true);
                                    SetColor(true);
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
                    Product model = new Product();
                    List<Ppr> pprs = new List<Ppr>();
                    try
                    {
                        model.ProductId = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value.ToString());

                        model = Resources.GetRes().Products.Where(x => x.ProductId == model.ProductId).FirstOrDefault().FastCopy();

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmProductName0"], krpdgList.SelectedRows[0].Cells["krpcmProductName1"], krpdgList.SelectedRows[0].Cells["krpcmProductName2"], false, Ext.AllSame(model.ProductName0, model.ProductName1, model.ProductName2), krpcbMultipleLanguage.Checked);


                        model.ProductName0 = krpdgList.SelectedRows[0].Cells["krpcmProductName0"].Value.ToString().Trim();
                        model.ProductName1 = krpdgList.SelectedRows[0].Cells["krpcmProductName1"].Value.ToString().Trim();
                        model.ProductName2 = krpdgList.SelectedRows[0].Cells["krpcmProductName2"].Value.ToString().Trim();
                        model.IsScales = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsScales"].Value.ToString());
                        model.Barcode = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value.ToString());
                        // 准确生成条码
                        if (null != model.Barcode)
                        {
                            string resultBarcode = null;

                            if (model.IsScales == 1)
                            {
                                resultBarcode = CommonOperates.GetCommonOperates().GenerateScaleNo(model.Barcode);
                            }
                            else
                            {
                                if (model.Barcode.StartsWith("22"))
                                {
                                    model.Barcode = model.Barcode.ReplaceFirstOccurrance("22", "23");
                                }

                                resultBarcode = CommonOperates.GetCommonOperates().GenerateEAN8(model.Barcode);
                            }

                            if (!string.IsNullOrWhiteSpace(resultBarcode))
                            {
                                krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value = model.Barcode = resultBarcode;
                            }
                            else
                            {
                                krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value = "";
                                model.Barcode = null;
                            }
                        }
                        model.Price = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrice"].Value.ToString()), 2);
                        model.PriceChangeMode = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmPriceChangeMode"].Value.ToString());
                        model.CostPrice = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmCostPrice"].Value.ToString()), 2);
                        model.CostPriceChangeMode = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmCostPriceChangeMode"].Value.ToString());
                        model.BalanceCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Value.ToString()), 3);
                        model.WarningCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmWarningCount"].Value.ToString()), 3);
                        model.IsBindCount = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsBindCount"].Value.ToString());
                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            model.ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));
                        else
                            model.ExpiredTime = 0;
                        model.ProductParentCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductParentCount"].Value.ToString()), 3);
                        model.ImageName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmImageName"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());


                        List<Printer> printer = new List<Printer>();
                        if (Resources.GetRes().MainLangIndex == 0)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName0)).ToList<Printer>();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName1)).ToList<Printer>();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName2)).ToList<Printer>();


                        if (printer.Count > 0)
                        {
                         
                            printer = printer.Distinct().ToList();
                            

                            foreach (var item in printer)
                            {
                                Ppr ppr = new Ppr();
                                ppr.PrinterId = item.PrinterId;
                                ppr.ProductId = model.ProductId;

                                Ppr oldPpr = Resources.GetRes().Pprs.Where(x => x.PrinterId == ppr.PrinterId && x.ProductId == ppr.ProductId).FirstOrDefault();
                                if (null != oldPpr)
                                    ppr.PprId = oldPpr.PprId;

                                pprs.Add(ppr);
                            }

                            model.tb_Ppr = pprs;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString()))
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("PrinterName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            model.tb_Ppr = null;
                        }



                        //判断空
                        if (string.IsNullOrWhiteSpace(model.ProductName0) || string.IsNullOrWhiteSpace(model.ProductName1) || string.IsNullOrWhiteSpace(model.ProductName2))
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("CompleteInput"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        //判断是否已存在
                        if (Resources.GetRes().Products.Where(x => x.ProductId != model.ProductId && (x.ProductName0.Equals(model.ProductName0, StringComparison.OrdinalIgnoreCase) || x.ProductName1.Equals(model.ProductName1, StringComparison.OrdinalIgnoreCase) || x.ProductName2.Equals(model.ProductName2, StringComparison.OrdinalIgnoreCase))).Count() > 0) // && x.ProductTypeId == productType.ProductTypeId
                        {

                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("ProductName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }


                        // 判断产品类别
                        ProductType productType = null;
                        string productTypeName = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName"].Value.ToString();
                        if (Resources.GetRes().MainLangIndex == 0)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName0 == productTypeName).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName1 == productTypeName).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName2 == productTypeName).FirstOrDefault();

                        //先判断下拉框
                        if (null == productType)
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("ProductTypeName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                        model.ProductTypeId = productType.ProductTypeId;



                        // 判断产品父级
                        Product productParent = null;
                        string productParentName = krpdgList.SelectedRows[0].Cells["krpcmProductParentName"].Value.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(productParentName))
                        {
                            if (Resources.GetRes().MainLangIndex == 0)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName0 == productParentName).FirstOrDefault();
                            else if (Resources.GetRes().MainLangIndex == 1)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName1 == productParentName).FirstOrDefault();
                            else if (Resources.GetRes().MainLangIndex == 2)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName2 == productParentName).FirstOrDefault();


                            //先判断下拉框
                            if (null == productParent || productParent.ProductId == model.ProductId)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("Exception_NotFound"), Resources.GetRes().GetString("ProductParentName")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            model.ProductParentId = productParent.ProductId;
                        }
                        else
                        {
                            model.ProductParentId = null;
                        }



                        // 判断一些条码秤有关的
                        if (model.IsScales == 1)
                        {
                            // 如果条码空的
                            if (string.IsNullOrWhiteSpace(model.Barcode))
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("BarcodeNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 如果条码已存在
                            if (null != model.Barcode && Resources.GetRes().Products.Where(x => x.ProductId != model.ProductId && (x.IsScales == 1 && x.Barcode == model.Barcode)).Count() > 0)
                            {

                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("BarcodeNo")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            // 如果价格大于9999元
                            if (model.Price >= 10000)
                            {
                                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("Price")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

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
                            List<Ppr> pprsList;
                            
                            ResultModel result = OperatesService.GetOperates().ServiceEditProduct(model, pprs, out pprsList);

                            this.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("SaveSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";
                                    Product oldModel = resultList.Where(x => x.ProductId == model.ProductId).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, model);

                                    no = Resources.GetRes().Products.IndexOf(oldModel);
                                    Resources.GetRes().Products.RemoveAt(no);
                                    Resources.GetRes().Products.Insert(no, model);


                                    pprs = Resources.GetRes().Pprs.Where(x => x.ProductId == model.ProductId).ToList();
                                    foreach (var item in pprs)
                                    {
                                        Resources.GetRes().Pprs.Remove(item);
                                    }

                                    Resources.GetRes().Pprs.AddRange(pprsList);

                                    ReloadProductTextbox(true);
                                    SetColor(true);
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

                Id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value.ToString());

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
                try
                {
                    
                    ResultModel result = OperatesService.GetOperates().ServiceDelProduct(Resources.GetRes().Products.Where(x => x.ProductId == Id).FirstOrDefault());
                    this.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("DeleteSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Product oldModel = Resources.GetRes().Products.Where(x => x.ProductId == Id).FirstOrDefault();
                            krpdgList.Rows.Remove(krpdgList.SelectedRows[0]);
                            resultList.Remove(oldModel);
                            Resources.GetRes().Products.Remove(oldModel);

                            List<Ppr> pprs = Resources.GetRes().Pprs.Where(x => x.ProductId == Id).ToList();
                            foreach (var item in pprs)
                            {
                                Resources.GetRes().Pprs.Remove(item);
                            }

                            ReloadProductTextbox(true);
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
        /// 更改时间
        /// </summary>
        private void ChangeTime()
        {
            bool NoLimit = false;
            DateTime ExpiredTime = DateTime.Now;
            string krpdgListSelectedRowsCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();

            if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsCellsValueToString))
                ExpiredTime = DateTime.ParseExact(krpdgListSelectedRowsCellsValueToString, "yyyy-MM-dd HH:mm", null);
            else
                NoLimit = true;


            ExpiredTimeWindow expired = new ExpiredTimeWindow(ExpiredTime, NoLimit);
            expired.ShowDialog(this);

            if (expired.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString() != expired.ReturnValue)
                {
                    krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value = expired.ReturnValue;
                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "*";
                }
                SetColor(true);
            }
        }


        /// <summary>
        /// 打印条码
        /// </summary>
        private void Print()
        {
            long id = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value.ToString());
            Product model = Resources.GetRes().Products.Where(x => x.ProductId == id).FirstOrDefault();

            if (model.Barcode.Length != 8 && model.Barcode.Length != 13){
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("BarcodeLengthLimit"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            PrintBarcodeWindow window = new PrintBarcodeWindow(model);
            window.ShowDialog(this);
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
                    kryptonContextMenuItemChangeTime.Enabled = false;
                    kryptonContextMenuItemPrint.Enabled = false;
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

                    kryptonContextMenuItemChangeTime.Enabled = true;

                    if (!krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && !krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value.Equals("") && !krpdgList.SelectedRows[0].Cells["krpcmIsScales"].Value.Equals("1"))
                        kryptonContextMenuItemPrint.Enabled = true;

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
                // 选中时才能更改时间
                else if (e.KeyCode == Keys.T)
                {
                    if (krpdgList.SelectedRows.Count > 0)
                        ChangeTime();
                }
                // 打印
                else if (e.KeyCode == Keys.P)
                {
                    if (krpdgList.SelectedRows.Count > 0 && !krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.Equals("*") && !krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value.Equals("") && !krpdgList.SelectedRows[0].Cells["krpcmIsScales"].Value.Equals("1"))
                        Print();
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
            else if (e.KeyCode == Keys.F1 && krpdgList.Rows.Count > 0 && null != krpdgList.CurrentCell && krpdgList.CurrentCell.ColumnIndex == 16 && !krpdgList.CurrentCell.IsInEditMode)
            {
                krpdgList.BeginEdit(true);

                SendKeys.SendWait("{F1}");
            }
            // 批量保存准备导入的数据
            else if (e.KeyCode == Keys.F1 && e.Alt && krpdgList.RowCount > 0)
            {

                // 两个或以上才批量保存
                int newCount = 0;

                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    if (krpdgList.Rows[i].Cells["krpcmEdit"].Value.Equals("*"))
                    {
                        ++newCount;

                        if (newCount >= 2)
                            break;
                    }
                }

                if (newCount >= 2)
                {
                    SaveAll();
                }
            }
            // 导出数据
            else if (e.KeyCode == Keys.F7 && e.Alt && Resources.GetRes().AdminModel.AdminNo == "1000")
            {
                Oybab.ServicePC.Tools.BakInOperate.Instance.Output(this, new BakInModel() { Products = resultList, ProductTypes = Resources.GetRes().ProductTypes });
            }
            // 导入数据
            else if (e.KeyCode == Keys.F8 && e.Alt && Resources.GetRes().AdminModel.AdminNo == "1000")
            {
                BakInModel model = Oybab.ServicePC.Tools.BakInOperate.Instance.Import<BakInModel>(this);
                if (null != model)
                {
                    if (null != model.Products)
                    {
                        foreach (var item in model.Products)
                        {

                            string productTypeName = null;


                            if (null != model.ProductTypes)
                            {
                                ProductType productType = model.ProductTypes.Where(x => x.ProductTypeId == item.ProductTypeId).FirstOrDefault();

                                if (null != productType)
                                {
                                    if (Resources.GetRes().MainLangIndex == 0)
                                    {
                                        productTypeName = productType.ProductTypeName0;
                                    }
                                    else if (Resources.GetRes().MainLangIndex == 1)
                                    {
                                        productTypeName = productType.ProductTypeName1;
                                    }
                                    else if (Resources.GetRes().MainLangIndex == 2)
                                    {
                                        productTypeName = productType.ProductTypeName2;
                                    }
                                }

                            }

                            string ProductParentName = null;
                            if (null != item.ProductParentId)
                            {

                                Product product = model.Products.Where(x => x.ProductId == item.ProductParentId.Value).FirstOrDefault();

                                if (null != product)
                                {
                                    if (Resources.GetRes().MainLangIndex == 0)
                                    {
                                        ProductParentName = product.ProductName0;
                                    }
                                    else if (Resources.GetRes().MainLangIndex == 1)
                                    {
                                        ProductParentName = product.ProductName1;
                                    }
                                    else if (Resources.GetRes().MainLangIndex == 2)
                                    {
                                        ProductParentName = product.ProductName2;
                                    }
                                }
                            }


                            AddToGrid("*", "-1", item.ProductName0, item.ProductName1, item.ProductName2, -1, productTypeName, item.IsScales, item.Barcode ?? "", item.Price, item.PriceChangeMode, item.CostPrice, item.CostPriceChangeMode, item.BalanceCount, item.WarningCount, item.IsBindCount, item.ExpiredTime, -1, ProductParentName, item.ProductParentCount, item.ImageName ?? "", GetHideType(item.HideType), item.Order);
                        }
                    }
                    else
                    {
                        KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            // 导出条码秤数据(非管理员也可以, 反正有产品页面权限) // 只有管理员可以
            else if (e.KeyCode == Keys.F9 && e.Alt) //  && Resources.GetRes().AdminModel.Mode == 2
            {

                List<Product> products = Resources.GetRes().Products.Where(x => x.IsScales == 1 && (x.HideType == 0 || x.HideType == 2)).ToList();
                if (null == products || products.Count == 0)
                {
                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                BarcodeScales scales = new BarcodeScales();
                scales.StartLoad += (x, y) =>
                {
                    this.StartLoad(x, null);
                };
                scales.StopLoad += (x, y) =>
                {
                    this.StopLoad(x, null);
                };
                scales.ShowDialog(this);
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
                    double BalanceCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Value.ToString()), 3);
                    double WarningCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmWarningCount"].Value.ToString()), 3);

                    if (BalanceCount < WarningCount)
                        krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Style.SelectionForeColor = Color.Red;
                    else
                        krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Style.SelectionForeColor = Color.Empty;




                    // Warn Time (小于0天的显示警告)
                    long ExpiredTime = 0;
                    string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                    if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                        ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));


                    if (ExpiredTime != 0 && (ExpiredTime - now) < 0)
                        krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Red;
                    else
                        krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Empty;

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
                        // 数量警告
                        double BalanceCount = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmBalanceCount"].Value.ToString()), 3);
                        double WarningCount = Math.Round(double.Parse(krpdgList.Rows[i].Cells["krpcmWarningCount"].Value.ToString()), 3);

                        if (BalanceCount < WarningCount)
                            krpdgList.Rows[i].Cells["krpcmBalanceCount"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalanceCount"].Style.SelectionForeColor = Color.Red;
                        else
                            krpdgList.Rows[i].Cells["krpcmBalanceCount"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmBalanceCount"].Style.SelectionForeColor = Color.Empty;



                        // Warn Time (小于0天的显示警告)
                        long ExpiredTime = 0;
                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.Rows[i].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));
                        

                        if (ExpiredTime != 0 && (ExpiredTime - now) <= 0)
                            krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Red;
                        else
                            krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.ForeColor = krpdgList.Rows[i].Cells["krpcmExpiredTime"].Style.SelectionForeColor = Color.Empty;

                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                }
            }
        }


        /// <summary>
        /// 类型被修改
        /// </summary>
        public event EventHandler ChangeProduct;
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
        internal void ReloadProductType(bool TrigChangeEvent = false)
        {
            krpcbProductType.Items.Clear();
            krpcbProductType.Items.Add(Resources.GetRes().GetString("All"));
            krpcmProductTypeName.SetValues(null, false);

            if (Resources.GetRes().ProductTypes.Where(x => x.HideType != 1).Count() > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    string[] types = Resources.GetRes().ProductTypes.Where(x => x.HideType != 1).OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductTypeId).Select(x => x.ProductTypeName0).ToArray();
                    krpcbProductType.Items.AddRange(types);
                    krpcmProductTypeName.SetValues(types, false);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] types = Resources.GetRes().ProductTypes.Where(x => x.HideType != 1).OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductTypeId).Select(x => x.ProductTypeName1).ToArray();
                    krpcbProductType.Items.AddRange(types);
                    krpcmProductTypeName.SetValues(types, false);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] types = Resources.GetRes().ProductTypes.Where(x => x.HideType != 1).OrderByDescending(x => x.Order).ThenByDescending(x=>x.ProductTypeId).Select(x => x.ProductTypeName2).ToArray();
                    krpcbProductType.Items.AddRange(types);
                    krpcmProductTypeName.SetValues(types, false);
                }
            }

            krpcbProductType.SelectedIndex = 0;

        }



        /// <summary>
        /// 重新加载产品搜索框
        /// </summary>
        private void ReloadProductTextbox(bool TrigChangeEvent = false)
        {
            krpcmProductParentName.SetValues(null, false);

            if (Resources.GetRes().Products.Count() > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                {
                    string[] types = Resources.GetRes().Products.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName0).ToArray();
                    krptProductName.SetValues(types, false);
                    krpcmProductParentName.SetValues(types, false);
                }
                else if (Resources.GetRes().MainLangIndex == 1)
                {
                    string[] types = Resources.GetRes().Products.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName1).ToArray();
                    krptProductName.SetValues(types, false);
                    krpcmProductParentName.SetValues(types, false);
                }
                else if (Resources.GetRes().MainLangIndex == 2)
                {
                    string[] types = Resources.GetRes().Products.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).Select(x => x.ProductName2).ToArray();
                    krptProductName.SetValues(types, false);
                    krpcmProductParentName.SetValues(types, false);
                }
            }

            if (TrigChangeEvent)
            {
                if (null != ChangeProduct)
                    ChangeProduct(null, null);
            }
        }



        /// <summary>
        /// 重新加载打印机列表框
        /// </summary>
        internal void ReloadPrinters(bool TrigChangeEvent = false)
        {
            krpcmPrinters.SetValues(null, false);
            if (Resources.GetRes().Printers.Where(x=>x.IsEnable == 1).Count() > 0)
            {
                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmPrinters.SetValues(Resources.GetRes().Printers.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x=>x.PrinterId).Select(x => x.PrinterName0).ToArray(), true);
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmPrinters.SetValues(Resources.GetRes().Printers.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x=>x.PrinterId).Select(x => x.PrinterName1).ToArray(), true);
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmPrinters.SetValues(Resources.GetRes().Printers.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x=>x.PrinterId).Select(x => x.PrinterName2).ToArray(), true);
            }

            if (TrigChangeEvent)
            {
                if (null != ChangeProduct)
                    ChangeProduct(null, null);
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
                krpcmIsScales.Visible = false;
                //krpcmBarcode.Visible = false;
                krpcmPriceChangeMode.Visible = false;
                //krpcmCostPrice.Visible = false;
                krpcmCostPriceChangeMode.Visible = false;
                //krpcmBalanceCount.Visible = false;
                krpcmWarningCount.Visible = false;
                //krpcmIsBindCount.Visible = false;
                krpcmExpiredTime.Visible = false;
                krpcmProductParentName.Visible = false;
                krpcmProductParentCount.Visible = false;
                krpcmImageName.Visible = false;
                krpcmHideType.Visible = false;
                krpcmPrinters.Visible = false;
                krpcmOrder.Visible = false;
            }
            else
            {
                krpcmIsScales.Visible = true;
                //krpcmBarcode.Visible = true;
                krpcmPriceChangeMode.Visible = true;
                //krpcmCostPrice.Visible = true;
                krpcmCostPriceChangeMode.Visible = true;
                //krpcmBalanceCount.Visible = true;
                krpcmWarningCount.Visible = true;
                //krpcmIsBindCount.Visible = true;
                krpcmExpiredTime.Visible = true;
                krpcmProductParentName.Visible = true;
                krpcmProductParentCount.Visible = true;
                krpcmImageName.Visible = true;
                krpcmHideType.Visible = true;
                krpcmPrinters.Visible = true;
                krpcmOrder.Visible = true;
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
                krpcmProductName0.Visible = false;
                krpcmProductName1.Visible = false;
                krpcmProductName2.Visible = false;


                if (Resources.GetRes().MainLangIndex == 0)
                    krpcmProductName0.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 1)
                    krpcmProductName1.Visible = true;
                else if (Resources.GetRes().MainLangIndex == 2)
                    krpcmProductName2.Visible = true;
            }
            else
            {
                krpcmProductName0.Visible = true;
                krpcmProductName1.Visible = true;
                krpcmProductName2.Visible = true;
            }
        }


        /// <summary>
        /// 批量保存所有
        /// </summary>
        private void SaveAll()
        {
            //确认保存
            var confirm = KryptonMessageBox.Show(this, Resources.GetRes().GetString("ConfirmSaveAll"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            StartLoad(this, null);
            int successCount = 0, FailedCount = 0;

                for (int i = 0; i < krpdgList.Rows.Count; i++)
                {
                    krpdgList.ClearSelection();
                    krpdgList.Rows[i].Selected = true;

                    string krpdgListSelectedRowsCellsValue1 = krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value.ToString();
                    string krpdgListSelectedRowsCellsValue2 = krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value.ToString();
                    //如果是新的
                    if (krpdgListSelectedRowsCellsValue2.Equals("*"))
                    {


                        Product model = new Product();
                        List<Ppr> pprs = new List<Ppr>();
                        bool IsNew = true;
                    try
                    {
                        model.ProductId = long.Parse(krpdgListSelectedRowsCellsValue1);
                        // 如果是编辑, 获取
                        if (model.ProductId > 0)
                        {
                            model = Resources.GetRes().Products.Where(x => x.ProductId == model.ProductId).FirstOrDefault().FastCopy();

                            IsNew = false;
                        }

                        // 隐藏功能时先把必要的复制掉(比如语言)
                        Common.GetCommon().CopyForHide(krpdgList.SelectedRows[0].Cells["krpcmProductName0"], krpdgList.SelectedRows[0].Cells["krpcmProductName1"], krpdgList.SelectedRows[0].Cells["krpcmProductName2"], IsNew, Ext.AllSame(model.ProductName0, model.ProductName1, model.ProductName2), krpcbMultipleLanguage.Checked);


                        model.ProductName0 = krpdgList.SelectedRows[0].Cells["krpcmProductName0"].Value.ToString().Trim();
                        model.ProductName1 = krpdgList.SelectedRows[0].Cells["krpcmProductName1"].Value.ToString().Trim();
                        model.ProductName2 = krpdgList.SelectedRows[0].Cells["krpcmProductName2"].Value.ToString().Trim();
                        model.IsScales = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsScales"].Value.ToString());
                        model.Barcode = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value.ToString());
                        // 准确生成条码
                        if (null != model.Barcode)
                        {
                            string resultBarcode = null;

                            if (model.IsScales == 1)
                            {
                                resultBarcode = CommonOperates.GetCommonOperates().GenerateScaleNo(model.Barcode);
                            }
                            else
                            {
                                if (model.Barcode.StartsWith("22"))
                                {
                                    model.Barcode = model.Barcode.ReplaceFirstOccurrance("22", "23");
                                }

                                resultBarcode = CommonOperates.GetCommonOperates().GenerateEAN8(model.Barcode);
                            }

                            if (!string.IsNullOrWhiteSpace(resultBarcode))
                            {
                                krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value = model.Barcode = resultBarcode;
                            }
                            else
                            {
                                krpdgList.SelectedRows[0].Cells["krpcmBarcode"].Value = "";
                                model.Barcode = null;
                            }
                        }
                        model.Price = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmPrice"].Value.ToString()), 2);
                        model.PriceChangeMode = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmPriceChangeMode"].Value.ToString());
                        model.CostPrice = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmCostPrice"].Value.ToString()), 2);
                        model.CostPriceChangeMode = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmCostPriceChangeMode"].Value.ToString());
                        model.BalanceCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmBalanceCount"].Value.ToString()), 3);
                        model.WarningCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmWarningCount"].Value.ToString()), 3);
                        model.IsBindCount = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmIsBindCount"].Value.ToString());
                        string krpdgListSelectedRowsExpiredTimeCellsValueToString = krpdgList.SelectedRows[0].Cells["krpcmExpiredTime"].Value.ToString();
                        if (!string.IsNullOrWhiteSpace(krpdgListSelectedRowsExpiredTimeCellsValueToString))
                            model.ExpiredTime = long.Parse(DateTime.ParseExact(krpdgListSelectedRowsExpiredTimeCellsValueToString, "yyyy-MM-dd HH:mm", null).ToString("yyyyMMddHHmmss"));
                        else
                            model.ExpiredTime = 0;
                        model.ProductParentCount = Math.Round(double.Parse(krpdgList.SelectedRows[0].Cells["krpcmProductParentCount"].Value.ToString()), 3);
                        model.ImageName = GetValueOrNull(krpdgList.SelectedRows[0].Cells["krpcmImageName"].Value.ToString());
                        model.HideType = GetHideTypeNo(krpdgList.SelectedRows[0].Cells["krpcmHideType"].Value.ToString());
                        model.Order = long.Parse(krpdgList.SelectedRows[0].Cells["krpcmOrder"].Value.ToString());


                        List<Printer> printer = new List<Printer>();
                        if (Resources.GetRes().MainLangIndex == 0)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName0)).ToList<Printer>();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName1)).ToList<Printer>();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            printer = Resources.GetRes().Printers.Where(x => krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString().Split('&').Contains(x.PrinterName2)).ToList<Printer>();


                        if (printer.Count > 0)
                        {
                           
                            printer = printer.Distinct().ToList();


                            foreach (var item in printer)
                            {
                                Ppr ppr = new Ppr();
                                ppr.PrinterId = item.PrinterId;
                                ppr.ProductId = model.ProductId;

                                // 如果是编辑
                                if (!IsNew)
                                {
                                    Ppr oldPpr = Resources.GetRes().Pprs.Where(x => x.PrinterId == ppr.PrinterId && x.ProductId == ppr.ProductId).FirstOrDefault();
                                    if (null != oldPpr)
                                        ppr.PprId = oldPpr.PprId;
                                }

                                pprs.Add(ppr);
                            }

                            model.tb_Ppr = pprs;
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(krpdgList.SelectedRows[0].Cells["krpcmPrinters"].Value.ToString()))
                            {
                                ++FailedCount;
                                continue;
                            }
                            model.tb_Ppr = null;
                        }




                        //判断空
                        if (string.IsNullOrWhiteSpace(model.ProductName0) || string.IsNullOrWhiteSpace(model.ProductName1) || string.IsNullOrWhiteSpace(model.ProductName2))
                        {
                            ++FailedCount;
                            continue;
                        }

                        // 判断产品类别
                        ProductType productType = null;
                        string productTypeName = krpdgList.SelectedRows[0].Cells["krpcmProductTypeName"].Value.ToString().Trim().Trim();
                        if (Resources.GetRes().MainLangIndex == 0)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName0 == productTypeName).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 1)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName1 == productTypeName).FirstOrDefault();
                        else if (Resources.GetRes().MainLangIndex == 2)
                            productType = Resources.GetRes().ProductTypes.Where(x => x.ProductTypeName2 == productTypeName).FirstOrDefault();

                        //先判断下拉框
                        if (null == productType)
                        {
                            ++FailedCount;
                            continue;
                        }
                        model.ProductTypeId = productType.ProductTypeId;



                        // 判断产品父级
                        Product productParent = null;
                        string productParentName = krpdgList.SelectedRows[0].Cells["krpcmProductParentName"].Value.ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(productParentName))
                        {
                            if (Resources.GetRes().MainLangIndex == 0)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName0 == productParentName).FirstOrDefault();
                            else if (Resources.GetRes().MainLangIndex == 1)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName1 == productParentName).FirstOrDefault();
                            else if (Resources.GetRes().MainLangIndex == 2)
                                productParent = Resources.GetRes().Products.Where(x => x.ProductName2 == productParentName).FirstOrDefault();


                            //先判断下拉框
                            if (null == productParent || productParent.ProductId == model.ProductId)
                            {
                                ++FailedCount;
                                continue;
                            }
                            model.ProductParentId = productParent.ProductId;
                        }
                        else
                        {
                            model.ProductParentId = null;
                        }





                        //判断是否已存在
                        if (!IsNew)
                        {
                            // 判断产品名是否已存在
                            if (Resources.GetRes().Products.Where(x => x.ProductId != model.ProductId && (x.ProductName0.Equals(model.ProductName0, StringComparison.OrdinalIgnoreCase) || x.ProductName1.Equals(model.ProductName1, StringComparison.OrdinalIgnoreCase) || x.ProductName2.Equals(model.ProductName2, StringComparison.OrdinalIgnoreCase))).Count() > 0) // && x.ProductTypeId == productType.ProductTypeId
                            {
                                ++FailedCount;
                                continue;
                            }

                            // 判断一些条码秤有关的
                            if (model.IsScales == 1)
                            {
                                // 如果条码空的
                                if (string.IsNullOrWhiteSpace(model.Barcode))
                                {
                                    ++FailedCount;
                                    continue;
                                }

                                // 如果条码已存在
                                if (null != model.Barcode && Resources.GetRes().Products.Where(x => x.ProductId != model.ProductId && (x.IsScales == 1 && x.Barcode == model.Barcode)).Count() > 0)
                                {

                                    ++FailedCount;
                                    continue;
                                }

                                // 如果价格大于9999元
                                if (model.Price >= 10000)
                                {
                                    ++FailedCount;
                                    continue;
                                }

                            }
                        }
                        else
                        {
                            // 判断产品名是否已存在
                            if (Resources.GetRes().Products.Where(x => (x.ProductName0.Equals(model.ProductName0, StringComparison.OrdinalIgnoreCase) || x.ProductName1.Equals(model.ProductName1, StringComparison.OrdinalIgnoreCase) || x.ProductName2.Equals(model.ProductName2, StringComparison.OrdinalIgnoreCase))).Count() > 0) // x.ProductTypeId == productType.ProductTypeId && 
                            {
                                ++FailedCount;
                                continue;
                            }


                            // 判断一些条码秤有关的
                            if (model.IsScales == 1)
                            {
                                // 如果条码空的
                                if (string.IsNullOrWhiteSpace(model.Barcode))
                                {
                                    ++FailedCount;
                                    continue;
                                }

                                // 如果条码已存在
                                if (null != model.Barcode && Resources.GetRes().Products.Where(x => x.IsScales == 1 && x.Barcode == model.Barcode).Count() > 0)
                                {

                                    ++FailedCount;
                                    continue;
                                }

                                // 如果价格大于9999元
                                if (model.Price >= 10000)
                                {
                                    ++FailedCount;
                                    continue;
                                }

                            }

                        }



                    }
                    catch
                    {
                        ++FailedCount;
                        continue;
                    }

                            try
                            {
                                List<Ppr> pprsList;
                                bool result = false;
                                ResultModel resultModel = null;

                                if (!IsNew)
                                {
                                    resultModel = OperatesService.GetOperates().ServiceEditProduct(model, pprs, out pprsList);

                                    if (resultModel.Result)
                                        result = true;
                                }
                                else
                                {
                                    result = OperatesService.GetOperates().ServiceAddProduct(model, pprs, out pprsList);
                                }

                                
                                if (result)
                                {
                                    krpdgList.SelectedRows[0].Cells["krpcmProductId"].Value = model.ProductId;
                                    
                                    krpdgList.SelectedRows[0].Cells["krpcmEdit"].Value = "";

                                    if (!IsNew)
                                    {
                                        Product oldModel = resultList.Where(x => x.ProductId == model.ProductId).FirstOrDefault();

                                        int no = resultList.IndexOf(oldModel);
                                        resultList.RemoveAt(no);
                                        resultList.Insert(no, model);

                                        no = Resources.GetRes().Products.IndexOf(oldModel);
                                        Resources.GetRes().Products.RemoveAt(no);
                                        Resources.GetRes().Products.Insert(no, model);


                                        pprs = Resources.GetRes().Pprs.Where(x => x.ProductId == model.ProductId).ToList();
                                        foreach (var item in pprs)
                                        {
                                            Resources.GetRes().Pprs.Remove(item);
                                        }

                                        Resources.GetRes().Pprs.AddRange(pprsList);
                                    }
                                    else
                                    {
                                        resultList.Insert(0, model);
                                        Resources.GetRes().Products.Add(model);

                                        Resources.GetRes().Pprs.AddRange(pprsList);
                                    }

                                    ReloadProductTextbox(true);
                                    SetColor(true);

                                    ++successCount;
                                    continue;
                                }
                                else
                                {
                                    ++FailedCount;
                                    continue;
                                }

                            }
                            catch
                            {
                                ++FailedCount;
                                continue;
                            }

                    }
                }
                StopLoad(this, null);

                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("SuccessAndFailProperty"), successCount, FailedCount), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
