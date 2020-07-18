using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using System.IO;
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow.ConditionWindow
{
    internal sealed partial class ProductConditionWindow : KryptonForm
    {
        public object ReturnValue { get; private set; } //返回值


        public ProductConditionWindow()
        {
            InitializeComponent();

            this.Text = Resources.GetRes().GetString("Search");
            krpbSearch.Text = Resources.GetRes().GetString("Search");
            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptProductName.Location = new Point(krptProductName.Location.X, krptProductName.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));

           
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Product.ico"));


            krplBarcode.Text = Resources.GetRes().GetString("Barcode");
            krplProductName.Text = Resources.GetRes().GetString("ProductName");
            krplWarn.Text = Resources.GetRes().GetString("Warn");
            krpcbCount.Text = Resources.GetRes().GetString("Count");
            krpcbTime.Text = Resources.GetRes().GetString("Time");
            krplHideType.Text = Resources.GetRes().GetString("DisplayType");
            krpcmHideType.Items.AddRange(new string[] { Resources.GetRes().GetString("Display"), Resources.GetRes().GetString("Hide"), Resources.GetRes().GetString("Income"), Resources.GetRes().GetString("Expenditure") });

        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSearch_Click(object sender, EventArgs e)
        {
            string krptProductNameTextTrim = krptProductName.Text.Trim();
            string krptBarcodeTextTrim = krptBarcode.Text.Trim();


            //查找数据
            List<Product> resultList = Resources.GetRes().Products.OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();

            // 产品名
            if (krptProductNameTextTrim != "")
                resultList = resultList.Where(x => x.ProductName0.Contains(krptProductNameTextTrim, StringComparison.OrdinalIgnoreCase) || x.ProductName1.Contains(krptProductNameTextTrim, StringComparison.OrdinalIgnoreCase) || x.ProductName2.Contains(krptProductNameTextTrim, StringComparison.OrdinalIgnoreCase)).ToList();

            // Barcode
            if (krptBarcode.Text.Trim() != "")
                resultList = resultList.Where(x => null != x.Barcode && x.Barcode.Contains(krptBarcodeTextTrim, StringComparison.OrdinalIgnoreCase)).ToList();

            // HideType
            if (krpcmHideType.SelectedIndex > 0)
            {
                resultList = resultList.Where(x => x.HideType == GetHideTypeNo(krpcmHideType.SelectedItem.ToString())).ToList();
            }

            // Warn Count
            if (krpcbCount.Checked)
                resultList = resultList.Where(x => x.BalanceCount < x.WarningCount).ToList();

            long now = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
            // Warn Time (小于15天的显示)
            if (krpcbTime.Checked)
                resultList = resultList.Where(x => x.ExpiredTime != 0 && (x.ExpiredTime - now) < 15000000).ToList();

            ReturnValue = resultList;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
        

        private void krptMemberNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbSearch_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
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



    }
}
