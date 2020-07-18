using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(MemberMetadata))]
    public partial class Member
    {
    }


    public class MemberMetadata
    {
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual ICollection<MemberPay> tb_memberpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> tb_order { get; set; }
        [JsonIgnore]
        public virtual ICollection<Takeout> tb_takeout { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderPay> tb_orderpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<TakeoutPay> tb_takeoutpay { get; set; }
    }
}
