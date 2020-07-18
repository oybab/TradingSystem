using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Server.Model
{
    internal class NotificationCache
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "9.@")]
        public NotificationType Type { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "%")]
        public string Notification { get; set; }
    }


    internal enum NotificationType
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "A#")]
        None,
        [Newtonsoft.Json.JsonProperty(PropertyName = "FGH")]
        Send,
        [Newtonsoft.Json.JsonProperty(PropertyName = "^&$%")]
        OrderUpdate,
        [Newtonsoft.Json.JsonProperty(PropertyName = "aW#")]
        TakeoutUpdate,
        [Newtonsoft.Json.JsonProperty(PropertyName = "SDF")]
        ProductCountUpdate,
        [Newtonsoft.Json.JsonProperty(PropertyName = "43")]
        DeviceModeUpdate,
        [Newtonsoft.Json.JsonProperty(PropertyName = "HG$")]
        OrderDetailsAdd,
        [Newtonsoft.Json.JsonProperty(PropertyName = "AWE^")]
        TakeoutAdd,
        [Newtonsoft.Json.JsonProperty(PropertyName = "W#")]
        ModelUpdate
    }
}
