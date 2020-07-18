using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(OrderDetailMetadata))]
    public partial class OrderDetail
    {
    }


    public class OrderDetailMetadata
    {
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Device tb_device1 { get; set; }
        [JsonIgnore]
        public virtual Order tb_order { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin1 { get; set; }
        [JsonIgnore]
        public virtual Product tb_product { get; set; }
    }
}
