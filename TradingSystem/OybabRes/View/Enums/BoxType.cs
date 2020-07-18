using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Enums
{
    public enum BoxType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 请求
        /// </summary>
        Request,
        /// <summary>
        /// 请求
        /// </summary>
        ChangePrice,
        /// <summary>
        /// 修改数量
        /// </summary>
        ChangeCount,
        /// <summary>
        /// 修改时间
        /// </summary>
        ChangeTime,
        /// <summary>
        /// 搜索
        /// </summary>
        Search,
          /// <summary>
          /// 地址
          /// </summary>
        Address,
        /// <summary>
        /// 修改会员价格
        /// </summary>
        ChangeMemberPrice,
        /// <summary>
        /// 修改支付价格
        /// </summary>
        ChangePaidPrice

    }
}
