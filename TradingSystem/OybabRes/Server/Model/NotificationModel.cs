using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Server.Model
{
    /// <summary>
    ///  /// <summary>
    /// 外卖更新通知模型
    /// </summary>
    /// </summary>
    internal sealed class TakeoutNotificationModel
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "$#FA")]
        public DAL.Takeout Takeout { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "H$WS")]
        public DAL.Takeout FinishedTakeout { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "G#$@s")]
        public DAL.Member Member { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "JG%")]
        public string TakeoutSessionId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Q@Z")]
        public Dictionary<long, ProductWithCount> ProductsChange { get; set; }
    }


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
