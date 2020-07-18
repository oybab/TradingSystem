using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oybab.Report.Model;

namespace Oybab.Report.Statistics
{
    public sealed partial class ProductProfitReport : DevExpress.XtraReports.UI.XtraReport
    {
        private double MaxPrice = 0;
        private string PriceSymbol = "￥";
        public ProductProfitReport(double maxPrice, string PriceSymbol)
        {
            InitializeComponent();
            this.MaxPrice = maxPrice;
            this.PriceSymbol = PriceSymbol;





            FixMoneyFormat(new List<XRBinding>() {
                xrcCount.DataBindings.FirstOrDefault(),
                xrTableCell7.DataBindings.FirstOrDefault(),
                xrTableCell11.DataBindings.FirstOrDefault(),
                xrTableCell16.DataBindings.FirstOrDefault(),
                xrTableCell22.DataBindings.FirstOrDefault(),
                xrTableCell10.DataBindings.FirstOrDefault()
            });


        }


        private void FixMoneyFormat(List<XRBinding> bindings)
        {
            foreach (var item in bindings)
            {
                item.FormatString = "{0:" + PriceSymbol + "0.00}";
            }
        }

        private void progressBar1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            RecordProfitProducts product = GetCurrentRow() as RecordProfitProducts;

            ProgressBar bar = sender as ProgressBar;
            bar.MaxValue = MaxPrice;
            bar.Position = product.ProfitPrice;
        }


    }
}
