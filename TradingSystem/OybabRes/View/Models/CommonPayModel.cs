using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Models
{
    public class CommonPayModel
    {
        public long ParentId { get; set; }
        public Nullable<long> BalanceId { get; set; }
        public Nullable<long> MemberId { get; set; }
        public Nullable<long> SupplierId { get; set; }
        public long IsPayByCard { get; set; }
        public double OriginalPrice { get; set; }
        public double Rate { get; set; }
        public double RemovePrice { get; set; }
        public double Price { get; set; }

        public Member Member { get; set; }
        public Supplier Supplier { get; set; }
        public bool IsChange { get; set; }

        public long AddTime { get; set; }

        public long State { get; set; }









        public long PayId { get; set; }
        public double BalancePrice { get; set; }
        public string Remark { get; set; }
        public double? ParentBalancePrice { get; set; }
        public long TransferId { get; set; }


        


        public string Type { get; set; }


 









        public CommonPayModel() { }
        public CommonPayModel(OrderPay model)
        {
            this.ParentId = model.OrderId;
            this.BalanceId = model.BalanceId;
            this.MemberId = model.MemberId;
            this.OriginalPrice = model.OriginalPrice;
            this.BalancePrice = model.BalancePrice;
            this.Rate = model.Rate;
            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;
            this.Member = model.tb_member;
            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "OrderPay";
            this.PayId = model.OrderPayId;
            this.State = model.State;
        }

        public CommonPayModel(TakeoutPay model)
        {
            this.ParentId = model.TakeoutId;
            this.BalanceId = model.BalanceId;
            this.MemberId = model.MemberId;
            this.BalancePrice = model.BalancePrice;
            this.OriginalPrice = model.OriginalPrice;
            this.Rate = model.Rate;
            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;
            this.Member = model.tb_member;
            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "TakeoutPay";
            this.PayId = model.TakeoutId;
            this.State = model.State;
        }

       

        public CommonPayModel(ImportPay model)
        {
            this.ParentId = model.ImportId;
            this.BalanceId = model.BalanceId;
            this.SupplierId = model.SupplierId;
            this.BalancePrice = model.BalancePrice;
            this.OriginalPrice = model.OriginalPrice;
            this.Rate = model.Rate;
            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;
            this.Supplier = model.tb_supplier;
            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "ImportPay";
            this.PayId = model.ImportPayId;
            this.State = model.State;
        }


        public OrderPay GetOrderPay()
        {
            OrderPay model = new OrderPay();
            model.OrderId = ParentId;
            model.BalanceId = BalanceId;
            model.MemberId = MemberId;
            model.BalancePrice = BalancePrice;
            model.OriginalPrice = OriginalPrice;
            model.Rate = Rate;
            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.tb_member = Member;
            model.AddTime = AddTime;
            model.Remark = Remark;
            model.OrderPayId = PayId;
            model.State = State;

            return model;
        }


        public TakeoutPay GetTakeoutPay()
        {
            TakeoutPay model = new TakeoutPay();
            model.TakeoutId = ParentId;
            model.BalanceId = BalanceId;
            model.MemberId = MemberId;
            model.BalancePrice = BalancePrice;
            model.OriginalPrice = OriginalPrice;
            model.Rate = Rate;
            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.tb_member = Member;
            model.AddTime = AddTime;
            model.Remark = Remark;
            model.TakeoutPayId = PayId;
            model.State = State;

            return model;
        }


       


        public ImportPay GetImportPay()
        {
            ImportPay model = new ImportPay();
            model.ImportId = ParentId;
            model.BalanceId = BalanceId;
            model.SupplierId = SupplierId;
            model.BalancePrice = BalancePrice;
            model.OriginalPrice = OriginalPrice;
            model.Rate = Rate;
            model.Rate = Rate;
            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.AddTime = AddTime;
            model.Remark = Remark;
            model.ImportPayId = PayId;
            model.State = State;

            return model;
        }


     




        public CommonPayModel(BalancePay model)
        {
            this.ParentId = model.BalanceId;
            this.BalanceId = model.BalanceId;

            this.OriginalPrice = model.OriginalPrice;
            this.BalancePrice = model.BalancePrice;

            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;

            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "BalancePay";
            this.PayId = model.BalancePayId;
            this.TransferId = model.TransferId;
        }

        public CommonPayModel(AdminPay model)
        {
            this.ParentId = model.AdminId;
            this.BalanceId = model.BalanceId;

            this.BalancePrice = model.BalancePrice;
            this.OriginalPrice = model.OriginalPrice;
            this.Price = model.Price;

            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "AdminPay";
            this.PayId = model.AdminPayId;
            this.ParentBalancePrice = model.ParentBalancePrice;
        }

        public CommonPayModel(MemberPay model)
        {
            this.ParentId = model.MemberId;
            this.BalanceId = model.BalanceId;
            this.MemberId = model.MemberId;
            this.BalancePrice = model.BalancePrice;
            this.OriginalPrice = model.OriginalPrice;
            this.Price = model.Price;
            this.Member = model.tb_member;
            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "MemberPay";
            this.PayId = model.MemberPayId;
            this.ParentBalancePrice = model.ParentBalancePrice;
        }

        public CommonPayModel(SupplierPay model)
        {
            this.ParentId = model.SupplierId;
            this.BalanceId = model.BalanceId;
            this.SupplierId = model.SupplierId;
            this.BalancePrice = model.BalancePrice;
            this.OriginalPrice = model.OriginalPrice;
            this.Price = model.Price;
            this.Supplier = model.tb_supplier;
            this.AddTime = model.AddTime;
            this.Remark = model.Remark;
            this.Type = "SupplierPay";
            this.PayId = model.SupplierPayId;
            this.ParentBalancePrice = model.ParentBalancePrice;
        }




        


        public BalancePay GetBalancePay()
        {
            BalancePay model = new BalancePay();
            model.BalancePayId = PayId;
            model.BalanceId = BalanceId ?? 0;

            model.BalancePrice = BalancePrice;
            model.OriginalPrice = OriginalPrice;

            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.AddTime = AddTime;
            model.Remark = Remark;
            model.TransferId = TransferId;

            return model;
        }


        public MemberPay GetMemberPay()
        {
            MemberPay model = new MemberPay();
            model.MemberId = ParentId;
            model.BalanceId = BalanceId ?? 0;

            model.BalancePrice = BalancePrice;
            model.OriginalPrice = OriginalPrice;

            model.Price = Price;
            model.AddTime = AddTime;
            model.Remark = Remark;
            model.MemberPayId = PayId;
            model.ParentBalancePrice = ParentBalancePrice;
            return model;
        }


        public SupplierPay GetSupplierPay()
        {
            SupplierPay model = new SupplierPay();
            model.SupplierId = ParentId;
            model.BalanceId = BalanceId ?? 0;

            model.BalancePrice = BalancePrice;
            model.OriginalPrice = OriginalPrice;

            model.Price = Price;
            model.AddTime = AddTime;
            model.Remark = Remark;
            model.SupplierPayId = PayId;
            model.ParentBalancePrice = ParentBalancePrice;

            return model;
        }



    }
}
