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
    public sealed partial class ReturnProductReport : DevExpress.XtraReports.UI.XtraReport
    {
        private double MaxPrice = 0;
        private string PriceSymbol = "￥";
        public ReturnProductReport(double maxPrice, string PriceSymbol)
        {
            InitializeComponent();
            this.MaxPrice = maxPrice;
            this.PriceSymbol = PriceSymbol;



            FixMoneyFormat(new List<XRBinding>() {
                xrTableCell11.DataBindings.FirstOrDefault()
            });

            FixMoneyFormat2(xrTableCell23);
        }


        private void FixMoneyFormat(List<XRBinding> bindings)
        {
            foreach (var item in bindings)
            {
                item.FormatString = "{0:" + PriceSymbol + "0.00}";
            }
        }

        private void FixMoneyFormat2(DevExpress.XtraReports.UI.XRTableCell cell)
        {
            cell.Text = cell.Text.Replace("￥", PriceSymbol);
        }


        private void progressBar1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            RecordReturnProducts product = GetCurrentRow() as RecordReturnProducts;

            ProgressBar bar = sender as ProgressBar;
            bar.MaxValue = MaxPrice;
            bar.Position = product.TotalPrice;
        }


    }
}
