using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Tools
{
    public static class ExtX
    {

        private static float lastMagnification = -1;
        /// <summary>
        /// 返回当前DPI倍值
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static float GetMagnification(this System.Windows.Forms.Form form)
        {
            if (lastMagnification == -1)
            {
                var dpiXProperty = typeof(System.Windows.SystemParameters).GetProperty("DpiX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var dpiYProperty = typeof(System.Windows.SystemParameters).GetProperty("Dpi", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                float dpiX = (int)dpiXProperty.GetValue(null, null);
                float dpiY = (int)dpiYProperty.GetValue(null, null);

                lastMagnification = dpiX / 96f;
            }
            return lastMagnification;
        }


        /// <summary>
        /// 重新计算宽高
        /// </summary>
        /// <param name="view"></param>
        public static void RecalcMagnification(this System.Windows.Forms.DataGridView view)
        {
            float Magnification = lastMagnification;

            if (Magnification != 1)
            {
                view.RowTemplate.Height = (int)(Magnification * view.RowTemplate.Height);
                // 更改所有列的宽度
                foreach (System.Windows.Forms.DataGridViewColumn item in view.Columns)
                {
                    item.Width = (int)(item.Width * Magnification);
                }
            }
        }


        /// <summary>
        /// 重新计算DPI值
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static int RecalcMagnification(this int original)
        {
            return (int)(original * lastMagnification);
        }

        /// <summary>
        /// 重新计算DPI值
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static int RecalcMagnification2(this int original)
        {
            return original;
        }



    }
}
