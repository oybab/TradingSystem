using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using Xamarin.Forms;
using Oybab.Res.Tools;

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



        internal string GetFont(StatisticModel reportModel)
        {

            if (Res.Instance.GetString("UseCustomFont") == "1")
            {
                string fontFamilyName = reportModel.Fonts.FirstOrDefault().Value.FontFamily;
                string fontAddress = "https://oybab.net/res/tradingsystem/fonts/" + fontFamilyName + "/" + fontFamilyName;

                if (Device.RuntimePlatform == Device.iOS)
                {
                    fontAddress = "src: url('" + Resources.Instance.GetString("CustomFont_iOS") + ".ttf');";
                }
                else if (Device.RuntimePlatform == Device.Android)
                {
                    fontAddress = "src: url('file:///android_asset/" + Resources.Instance.GetString("CustomFont_Android").Split('#')[0] + "');";
                }
                else if (Device.RuntimePlatform == Device.UWP)
                {
                    // Debug not working for now
                    fontAddress = @"src: url('ms-appx-web://" + Resources.Instance.GetString("CustomFont_UWP").Split('#')[0] + "');";
                }
                else
                {
                    fontAddress = "src:url('" + fontAddress + ".eot');"
                                + "src:url('" + fontAddress + ".eot?#iefix') format('embedded-opentype'),url('" + fontAddress + ".woff2') format('woff2'),url(" + fontAddress + ".woff') format('woff'),url('" + fontAddress + ".ttf') format('truetype'); ";
                }

                // 新增fong-face
                return "@font-face {"
                                + "font-family: '" + fontFamilyName + "';"
                                + fontAddress
                                + "font-weight: normal;"
                                + "font-style: normal;"
                                + "}";
            }

            return "";

            //else
            //{
            //    // 新增fong-face
            //    return "@font-face {"
            //                    + "font-family: 'Arial';"
            //                    + "font-weight: normal;"
            //                    + "font-style: normal;"
            //                    + "}";
            //}

          


            

        }

    }
}
