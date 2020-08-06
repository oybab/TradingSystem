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
using System.Globalization;
using Oybab.Report.Model;
using Oybab.Report.StatisticsHWP;

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


            this.Text = Model.Title;


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.ico"));

            krpbPrint.Click += (x, y) => { Print(); };


            Model.DetailReport = records;
            Model.DetailReport2 = records2;

            SummaryReport report = new SummaryReport(Model.Parameters["PriceSymbol"].ToString());




            webBrowser1.Refresh();
            string htmlContent = report.ProcessHTMLContent(Model);
            webBrowser1.DocumentText = htmlContent;






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






}
