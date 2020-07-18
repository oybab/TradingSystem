using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Oybab.ServerManager.Model.Service.Common;
using Oybab.ServerManager.Model.Service.OrderDetail;
using Oybab.ServerManager.Model.Service.Order;
using Oybab.ServerManager.Model.Service.Takeout;
using Oybab.ServerManager.Model.Service.TakeoutDetail;
using Oybab.ServerManager.Model.Service.Product;
using Oybab.ServerManager.Model.Service.Device;

namespace Oybab.ServerManager.Model.Models
{
    /// <summary>
    /// 发送到客户端
    /// </summary>
    public interface IServiceCallback
    {
        /// <summary>
        /// 给服务管理发送通知(----->服务管理)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract] //(IsOneWay = true)
        void ServiceSendNotification(ToClientServiceSendNotification toClient);



        /// <summary>
        /// 给服务管理发送更改订单通知(----->服务管理)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceOrderUpdateNotification(ToClientServiceOrderUpdateNotification toClient);



        /// <summary>
        /// 给服务管理发送更改外卖通知(----->服务管理)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceTakeoutUpdateNotification(ToClientServiceTakeoutUpdateNotification toClient);





        /// <summary>
        /// 给服务管理发送产品数量改动通知(----->服务管理)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceProductCountUpdateNotification(ToClientServiceProductCountUpdateNotification toClient);



        /// <summary>
        /// 给服务管理发送设备状态改动通知(----->服务管理)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceDeviceModeUpdateNotification(ToClientServiceDeviceModeUpdateNotification toClient);



        /// <summary>
        /// 给服务管理发送新增订单详情通知(----->服务管理)(客户端顾客先验证模式)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceOrderDetailsAddNotification(ToClientServiceOrderDetailsAddNotification toClient);





        /// <summary>
        /// 给服务管理发送新增外卖详情通知(----->服务管理)(客户端顾客先验证模式)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceTakeoutAddNotification(ToClientServiceTakeoutAddNotification toClient);



        /// <summary>
        /// 给服务管理发送模型修改通知(----->服务管理)
        /// </summary>
        /// <param name="message"></param>
        [OperationContract]
        void ServiceModelUpdateNotification(ToClientServiceModelUpdateNotification toClient);
    }
}
