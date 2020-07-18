using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oybab.DAL;

namespace Oybab.ServerManager.Model.Models
{
    /// <summary>
    /// 为了快速存储或获取产品和产品余额而创建的容器
    /// </summary>
    internal sealed class ProductWithCount
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "AW#$R")]
        public Product Product { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "F#G")]
        public double CountChange { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "EHG#")]
        public long? OriginalUpdateTime { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "J#s")]
        public double? OldPrice { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "S#f")]
        public double? OldCostPrice { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Ks2")]
        public double? NewPrice { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ZsF")]
        public double? NewCostPrice { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "GH2")]
        public long CostPriceMode { get; set; }
    }
}
