using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using Oybab.DAL;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Oybab.Report.Model;

namespace Oybab.Reports.Common
{
    public sealed partial class BackstageReport : DevExpress.XtraReports.UI.XtraReport
    {
        private Printer Printer;
        private List<Product> ProductList;
        private Order Order;
        private long Lang;
        private List<Request> Requests;

        public BackstageReport(Order order, List<Product> productList, Printer printer, List<Request> Requests, long Lang = -1)
        {
            this.Order = order;
            this.ProductList = productList;
            this.Printer = printer;
            this.Lang = Lang;
            this.Requests = Requests;

            InitializeComponent();
        }

        private void xrcProductName_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

            string value = ((XRTableCell)sender).Text;

            if (string.IsNullOrWhiteSpace(value))
                return;

            long ProductId = long.Parse(value);
            Product product = ProductList.Where(x => x.ProductId == ProductId).FirstOrDefault();

            long lang = Printer.Lang;

            if (-1 == lang)
            {
                lang = Order.Lang;
            }

            // 覆盖打印语言
            if (Lang != -1)
                lang = Lang;

            if (lang == 0)
                ((XRTableCell)sender).Text = product.ProductName0;
            else if (lang == 1)
                ((XRTableCell)sender).Text = product.ProductName1;
            else if (lang == 2)
                ((XRTableCell)sender).Text = product.ProductName2;

        }

        private void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRTable table = ((XRTable)sender);

            foreach (XRTableRow item in table.Rows)
            {
                if (item.Cells.Count == 3)
                {
                    string IsPack = item.Cells[1].Text;
                    if (IsPack.Equals("1"))
                    {
                        item.Cells[1].Text = "✔";
                    }
                    else
                    {
                        item.Cells[1].Text = "";
                    }
                }
            }
        }

        /// <summary>
        /// 打印前绑定需求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DetailReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            string sourceRequest = (string)this.GetCurrentColumnValue("Request");
            (sender as DetailReportBand).DataSource = GetDataSource(sourceRequest);
        }

        /// <summary>
        /// 获取需求
        /// </summary>
        /// <param name="sourceRemark"></param>
        /// <returns></returns>
        private List<ProductRequest> GetDataSource(string sourceRequest)
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
                    return SelectedRequests.Select(x => new ProductRequest() { RequestName = x.RequestName0 }).ToList();
                else if (lang == 1)
                    return SelectedRequests.Select(x => new ProductRequest() { RequestName = x.RequestName1 }).ToList();
                else if (lang == 2)
                    return SelectedRequests.Select(x => new ProductRequest() { RequestName = x.RequestName2 }).ToList();
                else
                    return null;
            }
        }


    }
}
