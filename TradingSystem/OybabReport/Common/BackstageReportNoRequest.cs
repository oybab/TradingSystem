using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Oybab.DAL;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Oybab.Reports.Common
{
    public sealed partial class BackstageReportNoRequest : DevExpress.XtraReports.UI.XtraReport
    {
        private Printer Printer;
        private List<Product> ProductList;
        private Order Order;
        private long Lang;

        public BackstageReportNoRequest(Order order, List<Product> productList, Printer printer, long Lang = -1)
        {
            this.Order = order;
            this.ProductList = productList;
            this.Printer = printer;
            this.Lang = Lang;

            InitializeComponent();
        }

        private void xrcProductName_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            string value = ((XRTableCell)sender).Text;

            if (string.IsNullOrWhiteSpace(value))
                return;

            long ProductId = long.Parse(value);
            Product product = ProductList.Where(x => x.ProductId == ProductId).FirstOrDefault();

            long lang = Printer.Lang;

            if (-1 == lang)
            {
                lang = Order.Lang;
            }

            // 覆盖打印语言
            if (Lang != -1)
                lang = Lang;

            if (lang == 0)
                ((XRTableCell)sender).Text = product.ProductName0;
            else if (lang == 1)
                ((XRTableCell)sender).Text = product.ProductName1;
            else if (lang == 2)
                ((XRTableCell)sender).Text = product.ProductName2;

        }

        private void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTable table = ((XRTable)sender);

            foreach (XRTableRow item in table.Rows)
            {
                if (item.Cells.Count == 3)
                {
                    string IsPack = item.Cells[1].Text;
                    if (IsPack.Equals("1"))
                    {
                        item.Cells[1].Text = "✔";
                    }
                    else
                    {
                        item.Cells[1].Text = "";
                    }
                }
            }
        }


    }
}
