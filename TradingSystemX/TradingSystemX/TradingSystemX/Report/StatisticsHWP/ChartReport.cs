using Newtonsoft.Json;
using Oybab.Report.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oybab.TradingSystemX.VM.ModelsForViews;

namespace Oybab.TradingSystemX.Report.StatisticsHWP
{
    public sealed partial class ChartReport : HWPReport
    {


        public ChartReport(string PriceSymbol)
        {
            this.PriceSymbol = PriceSymbol;


            WidhtMillimeters = 210 - 40; // html right20 + left20 margin 40
            HeightMillimeters = 280 - 20; // html top10 + bottom10 margin 20

        }



        internal override string ProcessHTMLContent(StatisticModel reportModel)
        {

            string htmlContent = GetResourceFileContentAsString("ChartReport.html");

            htmlContent = htmlContent.Replace(@"<!--${DynamicImportJquery}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.jquery-1.12.4.min.js")));
            htmlContent = htmlContent.Replace(@"<!--${DynamicImportCharJs}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.echarts.min.js")));


            // add viewport for phone browser
            htmlContent = htmlContent.Replace("<meta charset=\"UTF-8\">", "<meta charset=\"UTF-8\"><meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0, minimum-scale=1.0, user-scalable=no'/>");

            StringBuilder newStr = new StringBuilder();


            // 新增fong-face
            newStr.Append(GetFont(reportModel));


            // 先处理字体
            foreach (var item in reportModel.Fonts)
            {
                newStr.Append(".").Append(item.Key).Append("{").Append("color:#000000; font-family:'").Append(item.Value.FontFamily).Append("'; font-size:").Append(item.Value.FontSize + "pt;}").AppendLine();
            }
            htmlContent = htmlContent.Replace("/*${DynamicStyles}*/", newStr.ToString());


            // 处理参数
            newStr.Clear();



            var parametersJson = reportModel.Parameters.Select(x => new Dictionary<string, string>()
            {
                { x.Key, (x.Value is double) ? string.Format("{0:" + PriceSymbol + "0.00}", x.Value) : x.Value.ToString()}
            });



            newStr.Append("var parameters = \'").Append(JsonConvert.SerializeObject(parametersJson)).Append("\';");
            htmlContent = htmlContent.Replace("/*${DynamicParameter}*/", newStr.ToString());


            List<RecordChart> dataSource = reportModel.DataSource as List<RecordChart>;
            // 处理DataSource
            if (null != dataSource && dataSource.Count > 0)
            {
                newStr.Clear();
                var detailsJson = dataSource.Select(x => new
                {
                    Name = x.Name,
                    Value = x.Price,
                });

                newStr.Append("var dataSource = \'").Append(JsonConvert.SerializeObject(detailsJson)).Append("\';");
                htmlContent = htmlContent.Replace("/*${DynamicDataSource}*/", newStr.ToString());
            }



            htmlContent = htmlContent.Replace("/*${DynamicIsGenerated}*/", "var isGenerated = true;");




            return htmlContent;

        }







    }
}
