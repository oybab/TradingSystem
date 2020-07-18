using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    public enum ServiceType
    {
        
        /// <summary>
        /// 无
        /// </summary>
        None,
        
        /// <summary>
        /// 登录
        /// </summary>
        Login,
        
        /// <summary>
        /// 关闭
        /// </summary>
        Close
    }
}
