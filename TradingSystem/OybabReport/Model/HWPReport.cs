using OpenHtmlToPdf;
using Oybab.DAL;
using Oybab.Report.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Oybab.Report.Model
{
    public abstract class HWPReport
    {
        /// <summary>
        /// Report page heigth(Millimeters)
        /// </summary>
        internal double HeightMillimeters { get; set; }
        /// <summary>
        /// Report page width(Millimeters)
        /// </summary>
        internal double WidhtMillimeters { get; set; }
        /// <summary>
        /// Report page margin(Millimeters)
        /// </summary>
        internal PaperMargins MarginsMillimeters { get; set; }
        /// <summary>
        /// Is that a roll printer
        /// </summary>
        internal bool IsThermalPrinter { get; set; }

        //internal string HtmlContentToReport { get; set; }
        //internal string JsContentToReport { get; set; }
        internal List<Product> ProductList { get; set; }
        internal Printer Printer { get; set; }
        internal long Lang { get; set; }
        internal string PriceSymbol { get; set; }

        internal long BillLang { get; set; } = -1;






        /// <summary>
        /// Turn report model to html content
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        internal abstract string ProcessHTMLContent(ReportModel reportModel);









        /// <summary>
        /// 从当前资源读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal string GetResourceFileContentAsString(string fileName, string Resources = null)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = null;
            if (null == Resources)
                resourceName = "Oybab.Report.CommonHWP.Resources." + fileName;
            else
                resourceName = Resources + fileName;

            string resource = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    resource = reader.ReadToEnd();
                }
            }
            return resource;
        }




        /// <summary>
        /// 从当前资源读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal string GetResourceHTMLResourceFileFromLocal(string fileName)
        {
            string resource = null;
            string file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CommonHWP", "Resources" , fileName);

            if (System.IO.File.Exists(file))
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        resource = sr.ReadToEnd();
                    }
                }
                return resource;
            }
            else
            {
                return GetResourceFileContentAsString(fileName);
            }
        }



        /// <summary>
        /// 获取产品名称
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        internal string GetProductName(long ProductId)
        {
            Product product = ProductList.Where(x => x.ProductId == ProductId).FirstOrDefault();

            long lang = -1;
            if (null != Printer)
            {
                lang = Printer.Lang;
            }

            if (-1 == lang && BillLang != -1)
            {
                lang = BillLang;
            }

            // 覆盖打印语言
            if (Lang != -1)
                lang = Lang;


            if (lang == 0)
                return product.ProductName0;
            else if (lang == 1)
                return product.ProductName1;
            else if (lang == 2)
                return product.ProductName2;

            return "";
        }


    }
}
