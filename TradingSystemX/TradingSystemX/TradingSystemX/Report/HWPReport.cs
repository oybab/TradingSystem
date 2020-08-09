using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Oybab.TradingSystemX.Report
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

        internal long Lang { get; set; }
        internal string PriceSymbol { get; set; }

        internal long BillLang { get; set; } = -1;






        /// <summary>
        /// Turn report model to html content
        /// </summary>
        /// <param name="reportModel"></param>
        /// <returns></returns>
        internal abstract string ProcessHTMLContent(StatisticModel reportModel);









        /// <summary>
        /// 从当前资源读取文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal string GetResourceFileContentAsString(string fileName, string Resources = null)
        {
            
            string resourceName = null;
            if (null == Resources)
                resourceName = "Oybab.TradingSystemX.Report.StatisticsHWP.Resources." + fileName;
            else
                resourceName = Resources + fileName;

            

            var assembly = this.GetType().GetTypeInfo().Assembly; // you can replace "this.GetType()" with "typeof(MyType)", where MyType is any type in your assembly.
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

    }
}
