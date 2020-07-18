using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(ImportMetadata))]
    public partial class Import
    {
    }

    
    public class ImportMetadata
    {
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<ImportDetail> tb_importdetail { get; set; }
        //[JsonIgnore]
        //public virtual Supplier tb_supplier { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<ImportPay> tb_importpay { get; set; }
    }
}
