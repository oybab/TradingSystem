using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Oybab.DAL;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oybab.Report.Model;
using Oybab.Report;

namespace Oybab.Reports.Common
{
    public sealed partial class ImportReport : DevExpress.XtraReports.UI.XtraReport
    {
        private Printer Printer;
        private List<Product> ProductList;
        private Import Import;
        private long Lang;
        private string PriceSymbol;

        public ImportReport(Import import, List<Product> productList, Printer printer, string PriceSymbol, long Lang = -1)
        {
            this.Import = import;
            this.ProductList = productList;
            this.Printer = printer;
            this.Lang = Lang;
            this.PriceSymbol = PriceSymbol;

            InitializeComponent();


            FixMoneyFormat(new List<XRBinding>() {
                xrTableCell11.DataBindings.FirstOrDefault(),
                xrTableCell23.DataBindings.FirstOrDefault(),
                xrTableCell33.DataBindings.FirstOrDefault(),
                xrTableCell37.DataBindings.FirstOrDefault(),
                xrTableCell40.DataBindings.FirstOrDefault(),
                xrTableCell42.DataBindings.FirstOrDefault(),
                xrTableCell46.DataBindings.FirstOrDefault(),
                xrTableCell62.DataBindings.FirstOrDefault(),
                xrTableCell48.DataBindings.FirstOrDefault(),
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

            long lang = Printer.Lang;

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
                    if (Price.Equals(PriceSymbol + "0.00"))
                    {
                        item.Cells[2].Text = "";

                    }
                }
            }
        }


    }
}
