using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(PprMetadata))]
    public partial class Ppr
    {
    }


    public class PprMetadata
    {
        [JsonIgnore]
        public virtual Product tb_product { get; set; }
        [JsonIgnore]
        public virtual Printer tb_printer { get; set; }
    }
}
