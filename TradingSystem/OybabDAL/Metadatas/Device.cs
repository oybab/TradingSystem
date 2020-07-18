using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(DeviceMetadata))]
    public partial class Device
    {
    }

    
    public class DeviceMetadata
    {
        
        [JsonIgnore]
        public virtual ICollection<AdminLog> tb_adminlog { get; set; }
        [JsonIgnore]
        public virtual ICollection<AdminPay> tb_adminpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<BalancePay> tb_balancepay { get; set; }
        public virtual Room tb_room { get; set; }
        [JsonIgnore]
        public virtual ICollection<Import> tb_import { get; set; }
        [JsonIgnore]
        public virtual ICollection<ImportDetail> tb_importdetail { get; set; }
        [JsonIgnore]
        public virtual ICollection<MemberPay> tb_memberpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> tb_order { get; set; }
        [JsonIgnore]
        public virtual ICollection<Order> tb_order1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderDetail> tb_orderdetail { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderDetail> tb_orderdetail1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderPay> tb_orderpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<SupplierPay> tb_supplierpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<Takeout> tb_takeout { get; set; }
        [JsonIgnore]
        public virtual ICollection<Takeout> tb_takeout1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<TakeoutDetail> tb_takeoutdetail { get; set; }
        [JsonIgnore]
        public virtual ICollection<TakeoutDetail> tb_takeoutdetail1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<TakeoutPay> tb_takeoutpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<ImportPay> tb_importpay { get; set; }
    }
}
