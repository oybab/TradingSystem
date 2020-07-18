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
    public sealed partial class OrderCheckoutReport : DevExpress.XtraReports.UI.XtraReport
    {
        private Printer Printer;
        private List<Product> ProductList;
        private Order Order;
        private long Lang;
        private string PriceSymbol = "￥";

        public OrderCheckoutReport(Order order, List<Product> productList, Printer printer, string PriceSymbol, long Lang = -1)
        {
            this.Order = order;
            this.ProductList = productList;
            this.Printer = printer;
            this.Lang = Lang;
            this.PriceSymbol = PriceSymbol;

            InitializeComponent();


            FixMoneyFormat(new List<XRBinding>() {
                xrTableCell11.DataBindings.FirstOrDefault(),
                xrTableCell6.DataBindings.FirstOrDefault(),
                xrTableCell22.DataBindings.FirstOrDefault(),
                xrTableCell34.DataBindings.FirstOrDefault(),
                xrTableCell37.DataBindings.FirstOrDefault(),
                xrTableCell40.DataBindings.FirstOrDefault(),
                xrTableCell43.DataBindings.FirstOrDefault(),
                xrTableCell46.DataBindings.FirstOrDefault(),
                xrTableCell64.DataBindings.FirstOrDefault(),
                xrTableCell49.DataBindings.FirstOrDefault(),
                xrTableCell52.DataBindings.FirstOrDefault(),
                xrTableCell55.DataBindings.FirstOrDefault(),
                xrTableCell58.DataBindings.FirstOrDefault()
            });


        }


        private void FixMoneyFormat(List<XRBinding> bindings)
        {
            foreach (var item in bindings)
            {
                item.FormatString = "{0:" + PriceSymbol + "0.00}";
            }
        }


        private void xrcProductName_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            string value = ((XRTableCell)sender).Text;

            if (string.IsNullOrWhiteSpace(value))
                return;

            long ProductId = long.Parse(value);
            Product product = ProductList.Where(x => x.ProductId == ProductId).FirstOrDefault();

            long lang = -1;
            if (null != Printer)
            {
                lang = Printer.Lang;
            }

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

        private void xrTable4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTable table = ((XRTable)sender);

            foreach (XRTableRow item in table.Rows)
            {
                if (item.Cells.Count == 3)
                {
                    string Price = item.Cells[2].Text;
                    if (string.IsNullOrWhiteSpace(Price) || Price.Equals(PriceSymbol + "0.00"))
                    {
                        item.Visible = false; //item.Cells[2].Text = "";
                    }
                }
            }
        }


    }
}
