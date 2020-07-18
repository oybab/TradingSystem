using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(TakeoutDetailMetadata))]
    public partial class TakeoutDetail
    {
    }


    public class TakeoutDetailMetadata
    {
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Device tb_device1 { get; set; }
        [JsonIgnore]
        public virtual Takeout tb_takeout { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin1 { get; set; }
        [JsonIgnore]
        public virtual Product tb_product { get; set; }
    }
}
