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
    public sealed partial class ProductLabelMedium : HWPReport
    {

        public ProductLabelMedium(string PriceSymbol)
        {
            this.PriceSymbol = PriceSymbol;


            WidhtMillimeters = 40;
            HeightMillimeters = 30;
            MarginsMillimeters = PaperMargins.None();
        }



        internal override string ProcessHTMLContent(ReportModel reportModel)
        {
           
            string htmlContent = GetResourceFileContentAsString("ProductLabelMedium.html");


            htmlContent = htmlContent.Replace(@"<!--${DynamicImportJquery}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.jquery-1.12.4.min.js")));

            htmlContent = htmlContent.Replace(@"<!--${DynamicImportJsBarcode}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.JsBarcode.all.min.js")));



            StringBuilder newStr = new StringBuilder();

            // 先处理字体
            foreach (var item in reportModel.Fonts)
            {
                newStr.Append(".").Append(item.Key).Append("{").Append("color:#000000; font-family:'").Append(item.Value.FontFamily.Name).Append("'; font-size:").Append(item.Value.Size + "pt;}").AppendLine();
            }
            htmlContent = htmlContent.Replace("/*${DynamicStyles}*/", newStr.ToString());



            List<ProductLabel> dataSource = reportModel.DataSource as List<ProductLabel>;
            // 处理DataSource
            if (null != dataSource && dataSource.Count > 0)
            {
                newStr.Clear();
                var detailsJson = dataSource.Select(x => new
                {
                    ProductName = x.ProductName,
                    BarcodeNo = x.BarcodeNo,
                    Price = string.Format("{0:" + PriceSymbol + "0.00}", x.Price)
                }) ;

                newStr.Append("var dataSource = \'").Append(JsonConvert.SerializeObject(detailsJson)).Append("\';");
                htmlContent = htmlContent.Replace("/*${DynamicDataSource}*/", newStr.ToString());
            }

            newStr.Append("var dataSource = \'").Append(JsonConvert.SerializeObject(reportModel.DataSource)).Append("\';");
            htmlContent = htmlContent.Replace("/*${DynamicDataSource}*/", newStr.ToString());
            





            htmlContent = htmlContent.Replace("/*${DynamicIsGenerated}*/", "var isGenerated = true;");
            



            return htmlContent;

        }


        




    }
}
