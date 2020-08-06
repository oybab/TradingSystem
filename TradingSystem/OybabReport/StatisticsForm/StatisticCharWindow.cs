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
using System.IO;
using System.Collections;
using System.Diagnostics;
using Oybab.Report.Model;
using Oybab.Report.StatisticsHWP;

namespace Oybab.Report.StatisticsForm
{
    public sealed partial class StatisticCharWindow : KryptonForm
    {
        private StatisticModel Model { get; set; }
        public StatisticCharWindow(List<RecordChart> records, StatisticModel Model)
        {
            this.Model = Model;

            InitializeComponent();


            this.Text = Model.Title;


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.ico"));




            Model.DataSource = records;



            ChartReport report = new ChartReport(Model.Parameters["PriceSymbol"].ToString());



            webBrowser1.ScrollBarsEnabled = false;
            webBrowser1.Refresh();
            string htmlContent = report.ProcessHTMLContent(Model);
            webBrowser1.DocumentText = htmlContent;

            webBrowser1.DocumentCompleted -= WebBrowser1_DocumentCompleted;
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;

           






        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.Document.Body.Style = "overflow:hidden";
        }
    }
}
