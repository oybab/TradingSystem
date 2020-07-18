using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(PrinterMetadata))]
    public partial class Printer
    {
    }


    public class PrinterMetadata
    {
        [JsonIgnore]
        public virtual ICollection<Ppr> tb_Ppr { get; set; }
    }
}
