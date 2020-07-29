using DevExpress.XtraReports.UI;
using Oybab.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public class ReportPrinter
    {
        public void PrintReport(object xtraReport, ReportModel reportModel, string PrinterDeviceName)
        {

            XtraReport report = xtraReport as XtraReport;

            if (reportModel.PageHeight != 0)
                report.PageHeight = reportModel.PageHeight;

            if (reportModel.IsEAN13Generator)
            {
                ((XRBarCode)report.FindControl("xrBarCode1", false)).Symbology = new DevExpress.XtraPrinting.BarCode.EAN13Generator();
            }

            foreach (var item in reportModel.Parameters)
            {
                if (null != report.Parameters[item.Key])
                    report.Parameters[item.Key].Value = item.Value;
            }

            foreach (var item in reportModel.Font)
            {
                if (null != report.StyleSheet[item.Key])
                    report.StyleSheet[item.Key].Font = item.Value;
            }

            if (null != reportModel.DataSource)
                report.DataSource = reportModel.DataSource;

            if (null != reportModel.DetailReport)
            {
                DetailReportBand detailReportBand = (DetailReportBand)report.FindControl("DetailReport", false);
                detailReportBand.DataSource = reportModel.DetailReport;
            }
            if (null != reportModel.DetailReport2)
            {
                DetailReportBand detailReportBand = (DetailReportBand)report.FindControl("DetailReport1", false);
                detailReportBand.DataSource = reportModel.DetailReport2;
            }



            ReportPrintTool tool = new ReportPrintTool(report);

            report.ShowPrintStatusDialog = false;
            report.ShowPrintMarginsWarning = false;

            report.PrintingSystem.RemoveService(typeof(DevExpress.XtraPrinting.Native.GraphicsModifier));
            report.PrintingSystem.AddService(typeof(DevExpress.XtraPrinting.Native.GraphicsModifier), new GdiPlusFixedTextRenderingGraphicsModifier());


            if (null != PrinterDeviceName)
            {
                tool.Print(PrinterDeviceName);
            }
            else
            {
                tool.PrintDialog();
            }
                

        }
    }
}
