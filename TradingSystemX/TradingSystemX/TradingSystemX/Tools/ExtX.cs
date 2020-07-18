using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.Tools
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class ExtX
    {
        /// <summary>
        /// 跨平台睡眠
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static async Task Sleep(int ms)
        {
            await Task.Delay(ms);
        }


        /// <summary>
        /// 跨平台睡眠(动画)
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static async Task WaitForLoading()
        {
            await Task.Delay(100);
        }


        
    }

}
