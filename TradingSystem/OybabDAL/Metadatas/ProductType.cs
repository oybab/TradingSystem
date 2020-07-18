using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(ProductTypeMetadata))]
    public partial class ProductType
    {
    }


    public class ProductTypeMetadata
    {
        [JsonIgnore]
        public virtual ICollection<Product> tb_product { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductType> tb_producttype1 { get; set; }
        [JsonIgnore]
        public virtual ProductType tb_producttype2 { get; set; }
    }
}
