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
    public sealed partial class BackstageReport : HWPReport
    {

        private Order Order;

        private List<Request> Requests;

        public BackstageReport(Order order, List<Product> productList, Printer printer, List<Request> Requests, long Lang = -1)
        {
            this.Order = order;
            this.ProductList = productList;
            this.Printer = printer;
            this.Lang = Lang;
            this.Requests = Requests;
            this.BillLang = order.Lang;



            WidhtMillimeters = 72.1;
            MarginsMillimeters = PaperMargins.None();
            IsThermalPrinter = true;
        }



        internal override string ProcessHTMLContent(ReportModel reportModel)
        {

            string htmlContent = GetResourceFileContentAsString("BackstageReport.html");


            htmlContent = htmlContent.Replace(@"<!--${DynamicImportJquery}-->", string.Format("<script type=\"text/javascript\" > {0}</script>", GetResourceFileContentAsString("JS.jquery-1.12.4.min.js")));





            StringBuilder newStr = new StringBuilder();

            // 先处理字体
            foreach (var item in reportModel.Fonts)
            {
                newStr.Append(".").Append(item.Key).Append("{").Append("color:#000000; font-family:'").Append(item.Value.FontFamily.Name).Append("', 'Microsoft YaHei', 'Tahoma', 'Times New Roman','Arial'; font-size:").Append(item.Value.Size + "pt;}").AppendLine();
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


            List<OrderDetail>  dataSource = reportModel.DataSource as List<OrderDetail>;
            // 处理DataSource
            if (null != dataSource && dataSource.Count > 0)
            {
                newStr.Clear();
                var detailsJson = dataSource.Select(x => new
                {
                    ProductName = GetProductName(x.ProductId),
                    Count = x.Count,
                    IsPack = x.IsPack == 1 ? "✔" : "",
                    Price = string.Format("{0:" + PriceSymbol + "0.00}", x.Price),
                    Request = GetDataSource(x.Request)
                });

                newStr.Append("var dataSource = \'").Append(JsonConvert.SerializeObject(detailsJson)).Append("\';");
                htmlContent = htmlContent.Replace("/*${DynamicDataSource}*/", newStr.ToString());
            }


            htmlContent = htmlContent.Replace("/*${DynamicIsGenerated}*/", "var isGenerated = true;");
            



            return htmlContent;

        }


        // <summary>
        /// 获取需求
        /// </summary>
        /// <param name="sourceRemark"></param>
        /// <returns></returns>
        private object GetDataSource(string sourceRequest)
        {
            if (string.IsNullOrWhiteSpace(sourceRequest))
            {
                return null;
            }
            else
            {
                long[] requestIds = sourceRequest.Split(',').Select(x => long.Parse(x)).ToArray();
                List<Request> SelectedRequests = Requests.Where(x => requestIds.Contains(x.RequestId)).ToList();



                long lang = Printer.Lang;

                if (-1 == lang)
                {
                    lang = Order.Lang;
                }

                // 覆盖打印语言
                if (Lang != -1)
                    lang = Lang;

                if (lang == 0)
                    return SelectedRequests.Select(x => new { RequestName = x.RequestName0 }).ToList();
                else if (lang == 1)
                    return SelectedRequests.Select(x => new { RequestName = x.RequestName1 }).ToList();
                else if (lang == 2)
                    return SelectedRequests.Select(x => new { RequestName = x.RequestName2 }).ToList();
                else
                    return null;
            }
        }




    }
}
