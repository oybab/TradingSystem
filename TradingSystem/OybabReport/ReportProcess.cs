using OpenHtmlToPdf;
using Oybab.Report.Model;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Oybab.Report
{
    public sealed class ReportProcess
    {

        #region Instance
        private ReportProcess() { }
        private static readonly Lazy<ReportProcess> lazy = new Lazy<ReportProcess>(() => new ReportProcess());
        public static ReportProcess Instance { get { return lazy.Value; } }
        #endregion Instance



        /// <summary>
        /// 打印(打算做成2个操作, 先把HTML生成PDF 再打印. 一方面因为后续用HTML做报表以及自定义非常方便, 同时以后还可以再做一个PDF浏览器让用户实时查看报表)
        /// 这次决心去掉收费报表组件DevExpress.好让本系统能被用户和开发者更好的使用和利用.
        /// </summary>
        /// <param name="report"></param>
        /// <param name="reportModel"></param>
        /// <param name="PrinterDeviceName"></param>
        public void PrintReport(object report, ReportModel reportModel, string PrinterDeviceName)
        {

            HWPReport hwpReport = (report as HWPReport);
            string content = hwpReport.ProcessHTMLContent(reportModel);

            double height = hwpReport.HeightMillimeters;
            byte[] pdf = GetPDF(content, hwpReport.WidhtMillimeters, ref height, hwpReport.IsThermalPrinter, hwpReport.MarginsMillimeters);

            PrintPDF(PrinterDeviceName, 1, hwpReport.WidhtMillimeters, height, new MemoryStream(pdf));


        }



        /// <summary>
        /// 生成PDF
        /// </summary>
        /// <param name="htmlText"></param>
        /// <param name="widthMillimeters"></param>
        /// <param name="heightMillimeters"></param>
        /// <param name="IsThermalPrinter"></param>
        internal byte[] GetPDF(string htmlText, double widthMillimeters, ref double heightMillimeters, bool IsThermalPrinter, PaperMargins margins)
        {

            int dpi = GetDPI();
            double zoom = 1 * (GetWindowsScaling() / 100.0);

            OpenHtmlToPdf.PaperSize size = null;


            if (IsThermalPrinter)
                size = new OpenHtmlToPdf.PaperSize(Length.Millimeters(widthMillimeters), Length.Millimeters(10));
            else
                size = new OpenHtmlToPdf.PaperSize(Length.Millimeters(widthMillimeters), Length.Millimeters(heightMillimeters));

            IPdfDocument doc = Pdf.From(htmlText).EncodedWith("utf-8");

            byte[] finalPDF = null;
            finalPDF = doc.WithResolution(dpi).WithObjectSetting("load.zoomFactor", zoom.ToString()).WithObjectSetting("web.enableIntelligentShrinking", "false").WithMargins(margins).OfSize(size).Content();


            var document = PdfReader.Open(new MemoryStream(finalPDF), PdfDocumentOpenMode.InformationOnly);
            int numberOfPages = document.PageCount;


            if (numberOfPages > 1 && IsThermalPrinter)
            {
                heightMillimeters = 10 * numberOfPages;
                finalPDF = doc.WithResolution(dpi).WithObjectSetting("load.zoomFactor", zoom.ToString()).WithObjectSetting("web.enableIntelligentShrinking", "false").WithMargins(margins).OfSize(new OpenHtmlToPdf.PaperSize(Length.Millimeters(widthMillimeters), Length.Millimeters(heightMillimeters))).Content();
            }


            //File.WriteAllText("y:\\test1111.html", htmlText);
            //using (FileStream fs = new FileStream("y:\\test1111.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //    fs.Write(finalPDF, 0, finalPDF.Length);
            //}


            return finalPDF;
        }




        /// <summary>
        /// 打印PDF
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="copies"></param>
        /// <param name="widthMillimeters"></param>
        /// <param name="heightMillimeters"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal bool PrintPDF(string printer, int copies, double widthMillimeters, double heightMillimeters, Stream stream)
        {
            
            // Create the printer settings for our printer
            var printerSettings = new PrinterSettings
            {
                PrinterName = printer,
                Copies = (short)copies,
            };

            var pageSize = new System.Drawing.Printing.PaperSize();
            pageSize.Height = (int)PrinterUnitConvert.Convert(heightMillimeters * 100, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.Display);
            pageSize.Width = (int)PrinterUnitConvert.Convert(widthMillimeters * 100, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.Display);
            pageSize.RawKind = 0;// Printing.PaperKind.Custom此枚举的值是0,

            // Create our page settings for the paper size selected
            var pageSettings = new PageSettings(printerSettings)
            {
                Margins = new Margins(0, 0, 0, 0),

                PaperSize = pageSize, 
                    
                Landscape = false

            };

                

            if (printer == null)
            {
                System.Windows.Forms.PrintDialog printDlg = new System.Windows.Forms.PrintDialog();
                var document = PdfiumViewer.PdfDocument.Load(stream);
                PrintDocument printDoc = document.CreatePrintDocument();

                printDoc.PrinterSettings = printerSettings;
                printDoc.DefaultPageSettings = pageSettings;
                //printDoc.PrintController = new StandardPrintController();
                printDoc.OriginAtMargins = false;

                printDoc.DocumentName = "Print Document";
                printDlg.Document = printDoc;
                printDlg.AllowSelection = true;
                printDlg.AllowSomePages = true;
                //Call ShowDialog  

                Form defaultForm = Application.OpenForms.Cast<Form>().Where(x => null != x.Tag && x.Tag.ToString() == "Main").FirstOrDefault();

                defaultForm.BeginInvoke(new Action(() =>
                {
                    if (printDlg.ShowDialog() == DialogResult.OK)
                    {
                        printDoc.Print();
                    }
                    printDoc.Dispose();
                    document.Dispose();
                }));
                  

            }
            else
            {

                DetectPrinter(printer.ToUpper());

                // Now print the PDF document
                using (var document = PdfiumViewer.PdfDocument.Load(stream))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.PrinterSettings = printerSettings;
                        printDocument.DefaultPageSettings = pageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.OriginAtMargins = false;


                        printDocument.Print();
                    }
                }

            }

               
            return true;
           
        }


        // Plan B if DPI still has problem(Not test yet)
        //https://stackoverflow.com/questions/7003316/windows-display-setting-at-150-still-shows-96-dpi?noredirect=1&lq=1


        public static double GetWindowsScaling()
        {
            return (100 * Screen.PrimaryScreen.Bounds.Width / System.Windows.SystemParameters.PrimaryScreenWidth);
        }

        private static int GetDPI()
        {

            using (Graphics graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                return (int)graphics.DpiX;
                //float dpiY = graphics.DpiY;
            }
        }




        /// <summary>
        /// Detect printer
        /// </summary>
        /// <param name="requiredPrinterName"></param>
        private bool DetectPrinter(string requiredPrinterName)
        {
            bool _isPrinterExists = false;
            bool _isPrinterIsValid = false;
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {

                if (requiredPrinterName.ToUpper() == printerName)
                {
                    _isPrinterExists = true;
                    // Retrieve the printer settings.
                    PrinterSettings printer = new PrinterSettings();
                    printer.PrinterName = printerName;

                    // Check that this is a valid printer.
                    // (This step might be required if you read the printer name
                    // from a user-supplied value or a registry or configuration file
                    // setting.)
                    if (printer.IsValid)
                    {
                        _isPrinterIsValid = true;
                        break;
                    }
                    else
                    {
                        _isPrinterIsValid = false;
                        break;
                    }
                }
                else
                {
                    continue;
                }
            }


            if (!_isPrinterExists)
                throw new Exception("Printer: " + requiredPrinterName + " is not exists!");
            if (!_isPrinterIsValid)
                throw new Exception("Printer: " + requiredPrinterName + " is not valid!");


            return true;



        }



    }
}
