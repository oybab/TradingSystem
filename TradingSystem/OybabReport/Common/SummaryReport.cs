using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Oybab.Report.Model;

namespace Oybab.Reports.Common
{
    public sealed partial class SummaryReport : DevExpress.XtraReports.UI.XtraReport
    {
        public SummaryReport()
        {
            InitializeComponent();
           
        }

        private void xrTable6_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTable table = ((XRTable)sender);


            string Price = table.Rows[0].Cells[0].Text;
            if (string.IsNullOrWhiteSpace(Price))
            {
                table.Rows[0].Cells[0].Text = "　";
                table.Rows[1].Cells[0].Text = "　";
                table.Rows[1].Cells[1].Text = "　";
                table.Rows[1].Cells[2].Text = "　";
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
