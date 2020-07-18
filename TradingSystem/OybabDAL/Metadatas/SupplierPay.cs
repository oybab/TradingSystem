using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(SupplierPayMetadata))]
    public partial class SupplierPay
    {
    }


    public class SupplierPayMetadata
    {
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Supplier tb_supplier { get; set; }
        [JsonIgnore]
        public virtual Balance tb_balance { get; set; }
    }
}
