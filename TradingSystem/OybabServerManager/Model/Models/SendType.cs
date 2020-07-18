using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    public enum SendType
    {
        
        /// <summary>
        /// 无
        /// </summary>
        None,
        
        /// <summary>
        /// 未知
        /// </summary>
        Unknow,
        
        /// <summary>
        /// 关机
        /// </summary>
        Shutdown,
        
        /// <summary>
        /// 关机
        /// </summary>
        ShutdownServer,
        
        /// <summary>
        /// 重启
        /// </summary>
        Restart,
        
        /// <summary>
        /// 锁KEY
        /// </summary>
        KeyLock,
        
        /// <summary>
        /// 解锁KEY
        /// </summary>
        KeyUnlock,
        
        /// <summary>
        /// 重启
        /// </summary>
        RestartServer,
        
        /// <summary>
        /// 火警打开
        /// </summary>
        FireOn,
        
        /// <summary>
        /// 锁住
        /// </summary>
        Lock,
        
        /// <summary>
        /// 解锁
        /// </summary>
        Unlock,
        
        /// <summary>
        /// 呼叫
        /// </summary>
        Call,
        
        /// <summary>
        /// 信息
        /// </summary>
        Message,
        
        /// <summary>
        /// 信息
        /// </summary>
        Printer,
        
        /// <summary>
        /// 上线
        /// </summary>
        Online,
        
        /// <summary>
        /// 离线
        /// </summary>
        Offline,
        /// <summary>
        /// 火警关闭
        /// </summary>
        FireOff,
        /// <summary>
        /// 附加信息
        /// </summary>
        ExtendInfo,
        /// <summary>
        /// 打开钱箱
        /// </summary>
        OpenCashDrawer,
        Custom1,
        Custom2,
        Custom3
    }
}
