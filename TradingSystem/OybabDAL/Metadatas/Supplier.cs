using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(SupplierMetadata))]
    public partial class Supplier
    {
    }


    public class SupplierMetadata
    {
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual ICollection<Import> tb_import { get; set; }
        [JsonIgnore]
        public virtual ICollection<SupplierPay> tb_supplierpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<ImportPay> tb_importpay { get; set; }

    }
}
