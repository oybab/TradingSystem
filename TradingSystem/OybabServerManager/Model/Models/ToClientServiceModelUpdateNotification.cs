using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{

    public class ToClientServiceModelUpdateNotification
    {

        public string Model { get; set; }

        public string ModelRef { get; set; }

        public string ModelRef2 { get; set; }

        public OperateType OperateType { get; set; }

        public ModelType ModelType { get; set; }
    }

    public enum OperateType
    {
        
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 新增
        /// </summary>
        
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        
        Edit,
        /// <summary>
        /// 删除
        /// </summary>
        
        Delete,
        /// <summary>
        /// 替换
        /// </summary>
        
        Replace,
        /// <summary>
        /// 保存
        /// </summary>
        
        Save,
        /// <summary>
        /// 获取
        /// </summary>
        
        Get,
        /// <summary>
        /// 重置
        /// </summary>
        
        Reset,
        /// <summary>
        /// 转换
        /// </summary>

        Transfer
    }


    /// <summary>
    /// 模型类型. 顺序固定, 请勿在中间添加什么(因为客户端发打印等请求时发了它索引)
    /// </summary>
    public enum ModelType
    {
        
        None,
        
        Room,
        
        Product,
        
        ProductType,
        
        Admin,
        
        Member,
        
        Printer,
        
        Ppr,
        
        Device,
        
        Order,
        
        Import,
        
        OrderDetail,
        
        ImportDetail,
        
        Takeout,
        
        TakeoutDetail,
        
        MemberPay,
        
        Config,
        
        Supplier,
        
        SupplierPay,
        
        AdminLog,
        
        AdminPay,

        Request,

        Balance,

        BalancePay,

        Statistic,
       
        CallBack,

        Custom1,

        Custom2
    }


    /// <summary>
    /// 统计类型. 顺序固定, 请勿在中间添加什么(因为客户端发打印等请求时发了它索引)
    /// </summary>
    public enum StatisticType
    {

        None,

        Summary,

        Sale,

        Spend,

        SaleProduct,

        SpendProduct,

        SaleAdmin,

        SpendAdmin,
    }
}
