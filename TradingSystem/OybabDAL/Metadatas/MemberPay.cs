using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(MemberPayMetadata))]
    public partial class MemberPay
    {
    }


    public class MemberPayMetadata
    {
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Member tb_member { get; set; }
        [JsonIgnore]
        public virtual Balance tb_balance { get; set; }
    }
}
