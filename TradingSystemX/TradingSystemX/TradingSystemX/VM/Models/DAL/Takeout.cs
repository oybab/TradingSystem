//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Oybab.DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    
    public partial class Takeout
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Takeout()
        {
            this.tb_takeoutdetail = new HashSet<TakeoutDetail>();
            this.tb_takeoutpay = new HashSet<TakeoutPay>();
        }

        [JsonIgnore]
        public virtual Device tb_device { get; set; }
        [JsonIgnore]
        public virtual Device tb_device1 { get; set; }
        //[JsonIgnore]
        //public virtual ICollection<TakeoutDetail> tb_takeoutdetail { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin1 { get; set; }
        [JsonIgnore]
        public virtual Admin tb_admin2 { get; set; }

        public long TakeoutId { get; set; }
        public string ServerTokenId { get; set; }
        public string ServerTakeoutId { get; set; }
        public string ServerTakeoutSessionId { get; set; }
        public long State { get; set; }
        public double OriginalTotalPrice { get; set; }
        public double TotalPrice { get; set; }
        public double MemberPaidPrice { get; set; }
        public double PaidPrice { get; set; }
        public double TotalPaidPrice { get; set; }
        public double BorrowPrice { get; set; }
        public double KeepPrice { get; set; }
        public long Lang { get; set; }
        public long AdminId { get; set; }
        public long DeviceId { get; set; }
        public Nullable<long> MemberId { get; set; }
        public long Mode { get; set; }
        public long IsPack { get; set; }
        public long PersonCount { get; set; }
        public string Name0 { get; set; }
        public string Name1 { get; set; }
        public string Name2 { get; set; }
        public string Request { get; set; }
        public string Phone { get; set; }
        public string Address0 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public long PrintCount { get; set; }
        public long AddTime { get; set; }
        public long ReCheckedCount { get; set; }
        public Nullable<long> UpdateTime { get; set; }
        public long FinishAdminId { get; set; }
        public long FinishDeviceId { get; set; }
        public Nullable<long> FinishTime { get; set; }
        public Nullable<long> SendAdminId { get; set; }
        public Nullable<long> SendTime { get; set; }
        public string Remark { get; set; }
    
      
        public virtual Member tb_member { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TakeoutDetail> tb_takeoutdetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TakeoutPay> tb_takeoutpay { get; set; }





        
    }
}
