using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Takeout
{

    public class ToClientServiceTakeoutUpdateNotification : ToClientService
    {

        public string TakeoutNotification { get; set; }
    }
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
}
