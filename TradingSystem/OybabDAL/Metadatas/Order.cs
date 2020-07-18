using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(OrderMetadata))]
    public partial class Order
    {
    }


    public class OrderMetadata
    {
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Device tb_device1 { get; set; }
        [JsonIgnore]
        public virtual Room tb_room { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<OrderDetail> tb_orderdetail { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin1 { get; set; }
        //[JsonIgnore]
        //public virtual Member tb_member { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<OrderPay> tb_orderpay { get; set; }
    }
}
