using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.Res.Server.Model
{
    internal sealed class ToServerSessionRoomInfo
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "df#")]
        public long RoomId { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "a@D")]
        public int State { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ADSA@")]
        public string OrderSession { get; set; }
    }
}
