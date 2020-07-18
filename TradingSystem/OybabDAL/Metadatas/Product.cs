using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(ProductMetadata))]
    public partial class Product
    {
    }


    public class ProductMetadata
    {
        [JsonIgnore]
        public virtual ICollection<ImportDetail> tb_importdetail { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderDetail> tb_orderdetail { get; set; }
        [JsonIgnore]
        public virtual ICollection<Ppr> tb_Ppr { get; set; }
        [JsonIgnore]
        public virtual ICollection<Product> tb_product1 { get; set; }
        [JsonIgnore]
        public virtual Product tb_product2 { get; set; }
        [JsonIgnore]
        public virtual ProductType tb_producttype { get; set; }
        [JsonIgnore]
        public virtual ICollection<TakeoutDetail> tb_takeoutdetail { get; set; }
    }
}
