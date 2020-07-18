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
    public sealed partial class SummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        private string PriceSymbol = "￥";
        public SummaryReport(string PriceSymbol)
        {
            InitializeComponent();
            this.PriceSymbol = PriceSymbol;

            FixMoneyFormat(new List<XRBinding>() {
                xrTableCell35.DataBindings.FirstOrDefault(),
                xrTableCell36.DataBindings.FirstOrDefault(),
                xrTableCell38.DataBindings.FirstOrDefault(),
                xrTableCell12.DataBindings.FirstOrDefault(),
                xrTableCell13.DataBindings.FirstOrDefault(),
                xrTableCell14.DataBindings.FirstOrDefault(),
                xrTableCell19.DataBindings.FirstOrDefault(),
                xrTableCell20.DataBindings.FirstOrDefault(),
                xrTableCell28.DataBindings.FirstOrDefault(),
                xrTableCell22.DataBindings.FirstOrDefault(),
                xrTableCell25.DataBindings.FirstOrDefault()
            });

          
        }


        private void FixMoneyFormat(List<XRBinding> bindings)
        {
            foreach (var item in bindings)
            {
                item.FormatString = "{0:" + PriceSymbol + "0.00}";
            }
        }

       

        private void xrTable6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTable table = ((XRTable)sender);

            foreach (XRTableRow item in table.Rows)
            {

                string Price = item.Cells[0].Text;
                if (string.IsNullOrWhiteSpace(Price))
                {
                    item.Cells[0].Text = "　";
                    item.Cells[1].Text = "　";
                    item.Cells[2].Text = "　";
                    item.Cells[3].Text = "　";
                }

            }
        }

        private void DetailReport2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            if (this.Parameters["UnrecordTotalPriceValue"].Value.ToString() == "0" && this.Parameters["UnrecordReceivePriceValue"].Value.ToString() == "0")
                Detail2.Visible = false;
        }

        private void DetailReport1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DetailReportBand detailReportBand = (DetailReportBand)this.FindControl("DetailReport1", false);
            //List<SummaryModel> models = ((DevExpress.DataAccess.ObjectBinding.ObjectDataSource)detailReportBand.DataSource).DataSource as List<SummaryModel>;
            List<SummaryModel> models = detailReportBand.DataSource as List<SummaryModel>;
            if (null == models || !models.Any(x => x.Income != 0 || x.Profit != 0 || x.Spend != 0))
                DetailReport1.Visible = false;

        }

        private void DetailReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            DetailReportBand detailReportBand = (DetailReportBand)this.FindControl("DetailReport", false);
            List<SummaryModel> models = detailReportBand.DataSource as List<SummaryModel>;
            if (null == models || !models.Any(x => x.Income != 0 || x.Profit != 0 || x.Spend != 0))
                DetailReport.Visible = false;
        }



    }
}
