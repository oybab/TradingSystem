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
using Oybab.Report.Model;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using Oybab.Report.StatisticsHWP;

namespace Oybab.Report.StatisticsForm
{
    public sealed partial class StatisticAdminSaleProductReportWindow : KryptonForm
    {
        private StatisticModel Model { get; set; }
        public StatisticAdminSaleProductReportWindow(List<RecordProducts> records, StatisticModel Model)
        {

            this.Model = Model;

            InitializeComponent();


            this.Text = Model.Title;


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.ico"));


            Model.DataSource = records;


            AdminSaleProductReport report = new AdminSaleProductReport(Model.Parameters["PriceSymbol"].ToString());

     


            webBrowser1.Refresh();
            string htmlContent = report.ProcessHTMLContent(Model);
            webBrowser1.DocumentText = htmlContent;


     
        }

       
    }


}
