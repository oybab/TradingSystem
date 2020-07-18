using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    internal sealed class Client
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "S#F@#sf")]
        internal string IP { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "$#@FaJ$")]
        internal string ClientIP { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "A@#Fs")]
        internal string ClientMAC { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "afsd#")]
        internal long AdminId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "GHTY$@#")]
        internal long DeviceId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "D#gsfg")]
        internal long? RoomId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "gDFH$#")]
        internal long DeviceType { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "JKT$#")]
        internal long Mode { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "asD #")]
        internal string SessionId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "fsdT#!@35")]
        internal bool IsLocalPrintCustomOrder { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "g#$Gsd")]
        internal DateTime FirstLogin { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "sdf S#$R%G")]
        internal DateTime LastLogin { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "%E RF")]
        internal DateTime CheckDate { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "AW#^^W")]
        internal int LostCount { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "22SDGH")]
        internal int OldRemoveCount { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "SD #$@GDS")]
        internal DateTime OldRemoveCheckData { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "%$WF")]
        internal int SessionUpdateCount { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = ">OUE")]
        internal bool IsExpired { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Q@#sds")]
        internal bool IsConnected { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "#@SFD22")]
        [Newtonsoft.Json.JsonIgnore]
        internal IContextChannel ClientChannel { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "#@Tld29")]
        [Newtonsoft.Json.JsonIgnore]
        internal string SignalRClientSessionId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "gD$@q")]
        [Newtonsoft.Json.JsonIgnore]
        internal IServiceCallback ClientCallback { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "fSDF@2r43")]
        [Newtonsoft.Json.JsonIgnore]
        internal List<NotificationCache> NotificationCaches { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        internal DateTime LastSessionUpdateTime = DateTime.Now;


        [Newtonsoft.Json.JsonProperty(PropertyName = "SD#$g")]
        [Newtonsoft.Json.JsonIgnore]
        internal object Lock { get; set; } = new object();
        [Newtonsoft.Json.JsonProperty(PropertyName = "SD2$g")]
        [Newtonsoft.Json.JsonIgnore]
        internal BackgroundJobSchedueller TheTask { get; set; } = new BackgroundJobSchedueller();

        internal void Dispose()
        {
            TheTask.StopSchedueller();
        }
    }

    internal class NotificationCache{
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
