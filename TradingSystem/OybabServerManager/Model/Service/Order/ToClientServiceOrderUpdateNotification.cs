using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Order
{

    public class ToClientServiceOrderUpdateNotification : ToClientService
    {

        public string OrderNotification { get; set; }
    }

    /// <summary>
    /// 订单更新通知模型
    /// </summary>
    internal sealed class OrderNotificationModel
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "A#F")]
        public long RoomId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "FH@")]
        public DAL.Order Order { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "H#$S")]
        public DAL.Order FinishedOrder { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "J%$WS")]
        public DAL.Member Member { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "H#A")]
        public string OrderSessionId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ADGH@")]
        public bool IsChangeRoom { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "&^G")]
        public Dictionary<long, ProductWithCount> ProductsChange { get; set; }
    }
}
