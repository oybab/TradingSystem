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
    public sealed partial class TakeoutCheckoutMiddleReport : HWPReport
    {

 

      
        private Takeout Takeout;

        public TakeoutCheckoutMiddleReport(Takeout takeout, List<Product> productList, Printer printer, int heightMillimeters, string PriceSymbol, long Lang = -1)
        {
            this.Takeout = takeout;
            this.ProductList = productList;
            this.Printer = printer;
            this.Lang = Lang;
            this.PriceSymbol = PriceSymbol;
            this.BillLang = takeout.Lang;



            WidhtMillimeters = 200;
            HeightMillimeters = heightMillimeters - 10;
            MarginsMillimeters = PaperMargins.All(Length.Millimeters(5));
        }



        internal override string ProcessHTMLContent(ReportModel reportModel)
        {

            string htmlContent = GetResourceHTMLResourceFileFromLocal("TakeoutCheckoutMiddleReport.html");


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
            
            var parametersJson = reportModel.Parameters.Select(x => new Dictionary<string, string>()
            {
                { x.Key, x.Value is double ? string.Format("{0:" + PriceSymbol + "0.00}", x.Value) : x.Value.ToString()}
            });

            newStr.Append("var parameters = \'").Append(JsonConvert.SerializeObject(parametersJson)).Append("\';");
            htmlContent = htmlContent.Replace("/*${DynamicParameter}*/", newStr.ToString());


            List<TakeoutDetail>  dataSource = reportModel.DataSource as List<TakeoutDetail>;
            // 处理DataSource
            if (null != dataSource && dataSource.Count > 0)
            {
                newStr.Clear();
                var detailsJson = dataSource.Where(x=> null != x.ProductId).Select(x => new
                {
                    ProductName = GetProductName(x.ProductId.Value),
                    Count = x.Count,
                    Price = string.Format("{0:" + PriceSymbol + "0.00}", x.Price),
                    TotalPrice = string.Format("{0:" + PriceSymbol + "0.00}", x.TotalPrice)
                });

                newStr.Append("var dataSource = \'").Append(JsonConvert.SerializeObject(detailsJson)).Append("\';");
                htmlContent = htmlContent.Replace("/*${DynamicDataSource}*/", newStr.ToString());
            }


            htmlContent = htmlContent.Replace("/*${DynamicIsGenerated}*/", "var isGenerated = true;");
            



            return htmlContent;

        }


        




    }
}
