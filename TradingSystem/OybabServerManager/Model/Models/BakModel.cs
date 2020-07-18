using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Model.Models
{
    // This is old for use, missing pays and some new added things. need to add all to new and test
    internal sealed class BakModel
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "HRT%")]
        public List<Oybab.DAL.ProductType> ProductTypes { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "FH")]
        public List<Oybab.DAL.Product> Products { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "DFG#$")]
        public List<Oybab.DAL.Room> Rooms { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = " #$$")]
        public List<Oybab.DAL.Admin> Admins { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "$# BG")]
        public List<Oybab.DAL.Device> Devices { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "E BRT")]
        public List<Oybab.DAL.Printer> Printers { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "#$ V")]
        public List<Oybab.DAL.Ppr> Pprs { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "TRT")]
        public List<Oybab.DAL.Member> Members { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "AW#")]
        public List<Oybab.DAL.Supplier> Suppliers { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "AW@#G")]
        public List<Oybab.DAL.AdminLog> AdminLogs { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ASD ")]
        public List<Oybab.DAL.Order> Orders { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "W#32")]
        public List<Oybab.DAL.OrderDetail> OrderDetails { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "32FW")]
        public List<Oybab.DAL.Takeout> Takeouts { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "W# VA")]
        public List<Oybab.DAL.TakeoutDetail> TakeoutDetails { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "$%BSV")]
        public List<Oybab.DAL.Import> Imports { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "DF G34")]
        public List<Oybab.DAL.ImportDetail> ImportDetails { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "Aq#")]
        public List<Oybab.DAL.AdminPay> AdminPays { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "ASRR#VG")]
        public List<Oybab.DAL.MemberPay> MemberPays { get; set; }
        [Newtonsoft.Json.JsonProperty(PropertyName = "AW#22")]
        public List<Oybab.DAL.SupplierPay> SupplierPays { get; set; }
    }
}
