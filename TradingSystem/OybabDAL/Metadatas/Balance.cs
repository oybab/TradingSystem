using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Oybab.DAL
{
    [MetadataTypeAttribute(typeof(BalanceMetadata))]
    public partial class Balance
    {
    }


    public class BalanceMetadata
    {

        [JsonIgnore]
        public virtual ICollection<MemberPay> tb_memberpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<SupplierPay> tb_supplierpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<BalancePay> tb_balancepay { get; set; }
        [JsonIgnore]
        public virtual ICollection<ImportPay> tb_importpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderPay> tb_orderpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<TakeoutPay> tb_takeoutpay { get; set; }
        [JsonIgnore]
        public virtual ICollection<AdminPay> tb_adminpay { get; set; }

    }
}
