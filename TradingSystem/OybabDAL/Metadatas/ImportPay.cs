using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(ImportPayMetadata))]
    public partial class ImportPay
    {
    }


    public class ImportPayMetadata
    {
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Balance tb_balance { get; set; }
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Import tb_import { get; set; }
        //[JsonIgnore]
        //public virtual Supplier tb_supplier { get; set; }
    }
}
