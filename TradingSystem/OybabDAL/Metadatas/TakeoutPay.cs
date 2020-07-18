using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(TakeoutPayMetadata))]
    public partial class TakeoutPay
    {
    }


    public class TakeoutPayMetadata
    {
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Balance tb_balance { get; set; }
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        //[JsonIgnore]
        //public virtual Member tb_member { get; set; }
        [JsonIgnore]
        public virtual Takeout tb_takeout { get; set; }
    }
}
