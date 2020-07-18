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
using DevExpress.XtraReports.Localization;
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraReports.UI;
using System.Globalization;
using Oybab.Report.Model;
using Oybab.Report.Statistics;

namespace Oybab.Report.StatisticsForm
{
    public sealed partial class StatisticSummaryReportWindow : KryptonForm
    {
        private StatisticModel Model = null;

        public StatisticSummaryReportWindow(List<SummaryModel> records, List<SummaryModel> records2, StatisticModel Model)
        {

            this.Model = Model;

            InitializeComponent();

            List<SummaryModel> removeRecord = new List<SummaryModel>();

            foreach (var item in records)
            {
                if (item.Income == 0 && item.Spend == 0 && item.Profit == 0)
                    removeRecord.Add(item);
            }

            foreach (var item in removeRecord)
            {
                records.Remove(item);
            }

            removeRecord.Clear();

            foreach (var item in records2)
            {
                if (item.Income == 0 && item.Spend == 0 && item.Profit == 0)
                    removeRecord.Add(item);
            }

            foreach (var item in removeRecord)
            {
                records2.Remove(item);
            }


            PreviewLocalizer.Active = new CutomPreviewLocalizer(this.Model);

            this.Text = Model.Title;


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.ico"));

            krpbPrint.Click += (x, y) => { Print(); };

            SummaryReport report = new SummaryReport(Model.Parameters["PriceSymbol"]);

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





            //report.DataSource = records;
            DetailReportBand detailReportBand = (DetailReportBand)report.FindControl("DetailReport", false);
            detailReportBand.DataSource = records;



            DetailReportBand detailReportBand2 = (DetailReportBand)report.FindControl("DetailReport1", false);
            ((DevExpress.DataAccess.ObjectBinding.ObjectDataSource)detailReportBand2.DataSource).DataSource = records2;
            //detailReportBand2.DataSource = records2;





            documentViewer1.DocumentSource = report;


            report.PrintingSystem.RemoveService(typeof(DevExpress.XtraPrinting.Native.GraphicsModifier));
            report.PrintingSystem.AddService(typeof(DevExpress.XtraPrinting.Native.GraphicsModifier), new GdiPlusFixedTextRenderingGraphicsModifier());

            documentViewer1.ShowToolTips = false;





        }



        ///// <summary>
        ///// 开始显示加载
        ///// </summary>
        //public event EventHandler StartLoad;
        ///// <summary>
        ///// 停止显示加载
        ///// </summary>
        //public event EventHandler StopLoad;

        /// <summary>
        /// 打印
        /// </summary>
        public event EventHandler StartPrint;



        /// <summary>
        /// 打印
        /// </summary>
        private void Print()
        {
            if (null != StartPrint)
                StartPrint(null, null);
            
        }


    }


    internal sealed class CutomPreviewLocalizer : PreviewLocalizer
    {
        public CutomPreviewLocalizer(StatisticModel Model)
        {
            this.Model = Model;
        }

        public override string Language { get { return "CustomLang"; } }
        private StatisticModel Model { get; set; }
        public override string GetLocalizedString(PreviewStringId id)
        {
            switch (id)
            {
                case PreviewStringId.WaitForm_Caption: return this.Model.Parameters["PleaseWait"];
                case PreviewStringId.Msg_CreatingDocument: return this.Model.Parameters["CreatingTheDocument"];
            }
            return base.GetLocalizedString(id);
        }
    }






}
