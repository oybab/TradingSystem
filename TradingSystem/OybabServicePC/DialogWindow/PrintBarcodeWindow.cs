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
using Oybab.Res.Reports;
using Oybab.Report.Model;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class PrintBarcodeWindow : KryptonForm
    {

        private Product product;
        public PrintBarcodeWindow(Product product)
        {
            this.product = product;
            InitializeComponent();

            this.Text = Resources.GetRes().GetString("PrintBarcodeDescription");
            
            krpbPrint.Text = Resources.GetRes().GetString("Print");
            krplLanguage.Text = Resources.GetRes().GetString("Language");
            krplSize.Text = Resources.GetRes().GetString("Size");
            krplCount.Text = Resources.GetRes().GetString("Count");


            krpcLanguage.Items.AddRange(Resources.GetRes().MainLangList.Select(x => x.Value.LangName).ToArray());
            krpcSize.Items.AddRange(new object[] { "4cmX3cm", "3cmX2cm" });

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.PrintBarcode.ico"));


            if (Resources.GetRes().DefaultPrintLang == -1)
            {
                krpcLanguage.SelectedItem = Resources.GetRes().MainLang.LangName;
            }
            else
            {
                krpcLanguage.SelectedIndex = Resources.GetRes().DefaultPrintLang;
            }

            // 尺寸选择
            if (Resources.GetRes().DefaultBarcodePrintSize == -1)
            {
                krpcSize.SelectedIndex = 0;
            }
            else
            {
                krpcSize.SelectedIndex = Resources.GetRes().DefaultBarcodePrintSize;
            }




           
            



        }

        /// <summary>
        /// 选择打印语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChange_Click(object sender, EventArgs e)
        {
            List<ProductLabel> models = new List<ProductLabel>();
            string productName = "";
            string barcodeNo = product.Barcode;

            if (krpcLanguage.SelectedIndex == 0)
                productName = product.ProductName0;
            else if (krpcLanguage.SelectedIndex == 1)
                    productName = product.ProductName1;
            else if (krpcLanguage.SelectedIndex == 2)
                productName = product.ProductName2;


            for (int i = 0; i < krpcCount.Value; i++)
            {
            	ProductLabel model = new ProductLabel();
                model.BarcodeNo = barcodeNo;
                model.Price = product.Price;
                model.ProductName = productName;

                models.Add(model);
            }


            Print.Instance.PrintBarcode(models, krpcSize.SelectedIndex);

            Resources.GetRes().DefaultPrintLang = krpcLanguage.SelectedIndex;
            Resources.GetRes().DefaultBarcodePrintSize = krpcSize.SelectedIndex;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 回车修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcLanguage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbChange_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
                

        }

    }
}
