using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oybab.DAL;

namespace Oybab.Res.Server.Model
{
    public sealed class RoomModel : Room
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "S#RG")]
        public int State;//0未连接, 1桌面, 2播放器, 3锁屏
        [Newtonsoft.Json.JsonProperty(PropertyName = "q@f")]
        public Order PayOrder;
        //public List<OrderDetail> OrderDetails;
        [Newtonsoft.Json.JsonProperty(PropertyName = "h$")]
        public string OrderSession;
        
    }

   
}
