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
using DevExpress.XtraCharts;
using System.Collections;
using System.Diagnostics;
using Oybab.Reports.Common;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Localization;
using System.Globalization;
using Oybab.Report.Model;
using Oybab.Report.Statistics;

namespace Oybab.Report.StatisticsForm
{
    public sealed partial class StatisticProductProfitReportWindow : KryptonForm
    {
        private StatisticModel Model { get; set; }
        public StatisticProductProfitReportWindow(List<RecordProfitProducts> records, StatisticModel Model)
        {

            this.Model = Model;

            InitializeComponent();

            PreviewLocalizer.Active = new CutomPreviewLocalizer(this.Model);

            this.Text = Model.Title;


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.ico"));


            ProductProfitReport report = new ProductProfitReport(records.Max(x => x.ProfitPrice), Model.Parameters["PriceSymbol"]);

            

            foreach (var item in Model.Parameters)
            {
                if (null != report.Parameters[item.Key])
                    report.Parameters[item.Key].Value = item.Value;
            }


            foreach (var item in Model.Fonts)
            {
                if (null != report.StyleSheet[item.Key])
                    report.StyleSheet[item.Key].Font = item.Value;
            }


            report.DataSource = records.OrderByDescending(x=>x.ProfitPrice).ToList();

            documentViewer1.DocumentSource = report;


            report.PrintingSystem.RemoveService(typeof(DevExpress.XtraPrinting.Native.GraphicsModifier));
            report.PrintingSystem.AddService(typeof(DevExpress.XtraPrinting.Native.GraphicsModifier), new GdiPlusFixedTextRenderingGraphicsModifier());

            documentViewer1.ShowToolTips = false;

           
            
                

        }

       
    }


}
