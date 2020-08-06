using Newtonsoft.Json;
using OpenHtmlToPdf;
using Oybab.DAL;
using Oybab.Report.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.CommonHWP
{
    public sealed partial class SummaryReport : HWPReport
    {


        public SummaryReport(string PriceSymbol)
        {
            this.PriceSymbol = PriceSymbol;


            WidhtMillimeters = 48;
            MarginsMillimeters = PaperMargins.None();
            IsThermalPrinter = true;
        }



        internal override string ProcessHTMLContent(ReportModel reportModel)
        {
          
            string htmlContent = GetResourceFileContentAsString("SummaryReport.html");


            htmlContent = htmlContent.Replace(@"<!--${DynamicImportJquery}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.jquery-1.12.4.min.js")));





            StringBuilder newStr = new StringBuilder();

            // 先处理字体
            foreach (var item in reportModel.Fonts)
            {
                newStr.Append(".").Append(item.Key).Append("{").Append("color:#000000; font-family:'").Append(item.Value.FontFamily.Name).Append("'; font-size:").Append(item.Value.Size + "pt;}").AppendLine();
            }
            htmlContent = htmlContent.Replace("/*${DynamicStyles}*/", newStr.ToString());


            // 处理参数
            newStr.Clear();

            reportModel.Parameters.Add("CalcIncome", string.Format("{0:" + PriceSymbol + "0.00}", Math.Round((reportModel.DetailReport2 as List<SummaryModel>).Sum(x => x.Income), 2)));
            reportModel.Parameters.Add("CalcSpend", string.Format("{0:" + PriceSymbol + "0.00}", Math.Round((reportModel.DetailReport2 as List<SummaryModel>).Sum(x => x.Spend), 2)));
            reportModel.Parameters.Add("CalcProfit", string.Format("{0:" + PriceSymbol + "0.00}", Math.Round((reportModel.DetailReport2 as List<SummaryModel>).Sum(x => x.Profit), 2)));
            var parametersJson = reportModel.Parameters.Select(x => new Dictionary<string, string>()
            {
                { x.Key, x.Value is double ? string.Format("{0:" + PriceSymbol + "0.00}", x.Value) : x.Value.ToString()}
            });

            newStr.Append("var parameters = \'").Append(JsonConvert.SerializeObject(parametersJson)).Append("\';");
            htmlContent = htmlContent.Replace("/*${DynamicParameter}*/", newStr.ToString());


            List<SummaryModel>  dataSource = reportModel.DetailReport as List<SummaryModel>;
            // 处理DataSource
            if (null != dataSource && dataSource.Count > 0)
            {
                newStr.Clear();
                var detailsJson = dataSource.Select(x => new
                {
                    Name = x.Name,
                    TypeName = x.TypeName,
                    Income = string.Format("{0:" + PriceSymbol + "0.00}", x.Income),
                    Spend = string.Format("{0:" + PriceSymbol + "0.00}", x.Spend),
                    Profit = string.Format("{0:" + PriceSymbol + "0.00}", x.Profit),
                });

                newStr.Append("var dataSource = \'").Append(JsonConvert.SerializeObject(detailsJson)).Append("\';");
                htmlContent = htmlContent.Replace("/*${DynamicDataSource}*/", newStr.ToString());
            }


            dataSource = reportModel.DetailReport2 as List<SummaryModel>;
            // 处理DataSource2
            if (null != dataSource && dataSource.Count > 0)
            {
                newStr.Clear();
                var detailsJson = dataSource.Select(x => new
                {
                    Name = x.Name,
                    TypeName = x.TypeName,
                    Income = string.Format("{0:" + PriceSymbol + "0.00}", x.Income),
                    Spend = string.Format("{0:" + PriceSymbol + "0.00}", x.Spend),
                    Profit = string.Format("{0:" + PriceSymbol + "0.00}", x.Profit),
                });

                newStr.Append("var dataSource2 = \'").Append(JsonConvert.SerializeObject(detailsJson)).Append("\';");
                htmlContent = htmlContent.Replace("/*${DynamicDataSource2}*/", newStr.ToString());
            }




            htmlContent = htmlContent.Replace("/*${DynamicIsGenerated}*/", "var isGenerated = true;");
            



            return htmlContent;

        }


        




    }
}
