using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Operate
{
    /// <summary>
    /// 处理PC
    /// </summary>
    internal sealed class OperatePC
    {
        private static OperatePC operatePC = null;
        private OperatePC() { }
        public static OperatePC GetOperates()
        {
            if (null == operatePC)
                operatePC = new OperatePC();
            return operatePC;
        }


        /// <summary>
        /// 重启
        /// </summary>
        internal void Resart()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/r /t 0",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

        /// <summary>
        /// 关机
        /// </summary>
        internal void Shutdown()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /t 0",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        }

    }
}
