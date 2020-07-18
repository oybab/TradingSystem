using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.TradingSystemX.VM.ModelsForViews
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


        public CommonPayModel() { }
        public CommonPayModel(OrderPay model)
        {
            this.ParentId = model.OrderId;
            this.BalanceId = model.BalanceId;
            this.MemberId = model.MemberId;
            this.OriginalPrice = model.OriginalPrice;
            this.Rate = model.Rate;
            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;
            this.Member = model.tb_member;
            this.AddTime = model.AddTime;
        }

        public CommonPayModel(TakeoutPay model)
        {
            this.ParentId = model.TakeoutId;
            this.BalanceId = model.BalanceId;
            this.MemberId = model.MemberId;
            this.OriginalPrice = model.OriginalPrice;
            this.Rate = model.Rate;
            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;
            this.Member = model.tb_member;
            this.AddTime = model.AddTime;
        }

       

        public CommonPayModel(ImportPay model)
        {
            this.ParentId = model.ImportId;
            this.BalanceId = model.BalanceId;
            this.SupplierId = model.SupplierId;
            this.OriginalPrice = model.OriginalPrice;
            this.Rate = model.Rate;
            this.RemovePrice = model.RemovePrice;
            this.Price = model.Price;
            this.Supplier = model.tb_supplier;
            this.AddTime = model.AddTime;
        }


        public OrderPay GetOrderPay()
        {
            OrderPay model = new OrderPay();
            model.OrderId = ParentId;
            model.BalanceId = BalanceId;
            model.MemberId = MemberId;
            model.OriginalPrice = OriginalPrice;
            model.Rate = Rate;
            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.tb_member = Member;
            model.AddTime = AddTime;

            return model;
        }


        public TakeoutPay GetTakeoutPay()
        {
            TakeoutPay model = new TakeoutPay();
            model.TakeoutId = ParentId;
            model.BalanceId = BalanceId;
            model.MemberId = MemberId;
            model.OriginalPrice = OriginalPrice;
            model.Rate = Rate;
            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.tb_member = Member;
            model.AddTime = AddTime;


            return model;
        }


      


        public ImportPay GetImportPay()
        {
            ImportPay model = new ImportPay();
            model.ImportId = ParentId;
            model.BalanceId = BalanceId;
            model.SupplierId = SupplierId;
            model.OriginalPrice = OriginalPrice;
            model.Rate = Rate;
            model.Rate = Rate;
            model.RemovePrice = RemovePrice;
            model.Price = Price;
            model.AddTime = AddTime;


            return model;
        }
    }
}
