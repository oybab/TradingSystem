using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(TakeoutMetadata))]
    public partial class Takeout
    {
    }


    public class TakeoutMetadata
    {
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Device tb_device1 { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<TakeoutDetail> tb_takeoutdetail { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin1 { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin2 { get; set; }
        //[JsonIgnore]
        //public virtual Member tb_member { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<TakeoutPay> tb_takeoutpay { get; set; }
    }
}
