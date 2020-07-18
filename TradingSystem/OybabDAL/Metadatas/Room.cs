using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(RoomMetadata))]
    public partial class Room
    {
    }


    public class RoomMetadata
    {
        [JsonIgnore]
        public virtual ICollection<Device> tb_device { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> tb_order { get; set; }
    }
}
