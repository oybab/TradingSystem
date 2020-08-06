using Newtonsoft.Json;
using OpenHtmlToPdf;
using Oybab.DAL;
using Oybab.Report.CommonHWP;
using Oybab.Report.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.StatisticsHWP
{
    public sealed partial class ChartReport2 : HWPReport
    {


        public ChartReport2(string PriceSymbol)
        {
            this.PriceSymbol = PriceSymbol;


            WidhtMillimeters = 210 - 40; // html right20 + left20 margin 40
            HeightMillimeters = 280 - 20; // html top10 + bottom10 margin 20
            MarginsMillimeters = PaperMargins.All(Length.Millimeters(0));
        }



        internal override string ProcessHTMLContent(ReportModel reportModel)
        {
          
            string htmlContent = GetResourceFileContentAsString("ChartReport.html", "Oybab.Report.StatisticsHWP.Resources.");

            htmlContent = htmlContent.Replace(@"<!--${DynamicImportJquery}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.jquery-1.12.4.min.js")));
            htmlContent = htmlContent.Replace(@"<!--${DynamicImportCharJs}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.Chart.min.js")));
            




            StringBuilder newStr = new StringBuilder();

            // 先处理字体
            foreach (var item in reportModel.Fonts)
            {
                newStr.Append(".").Append(item.Key).Append("{").Append("color:#000000; font-family:'").Append(item.Value.FontFamily.Name).Append("'; font-size:").Append(item.Value.Size + "pt;}").AppendLine();
            }
            htmlContent = htmlContent.Replace("/*${DynamicStyles}*/", newStr.ToString());


            // 处理参数
            newStr.Clear();

            double maxPrice  = (reportModel.DataSource as List<RecordChart>).Max(x => x.Price);
            double minPrice = (reportModel.DataSource as List<RecordChart>).Min(x => x.Price);

            maxPrice = maxPrice + (maxPrice / 10);
            minPrice = minPrice - (maxPrice / 10);

            if (minPrice > 0)
                minPrice = 0;

            reportModel.Parameters.Add("MaxValue", Math.Round(maxPrice, 2));
            reportModel.Parameters.Add("MinValue", Math.Round(minPrice, 2));

            var parametersJson = reportModel.Parameters.Select(x => new Dictionary<string, string>()
            {
                { x.Key, (x.Value is double && (x.Key != "MaxValue" && x.Key != "MinValue" )) ? string.Format("{0:" + PriceSymbol + "0.00}", x.Value) : x.Value.ToString()}
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
