using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Models
{
    public sealed class BillModel
    {
        public long Id { get; set; }

        public long BillType { get; set; }
        public long RoomId { get; set; }
        public double RoomPrice { get; set; }
        public long IsAutoPay { get; set; }
        public long RoomPriceCalcTime { get; set; }
        public Nullable<long> StartTime { get; set; }
        public Nullable<long> EndTime { get; set; }




        public string ServerTokenId { get; set; }
        public string ServerTakeoutId { get; set; }
        public string ServerTakeoutSessionId { get; set; }
        public long State { get; set; }
        public double OriginalTotalPrice { get; set; }
        public double TotalPrice { get; set; }
        public double DealsPrice { get; set; }
        public double MemberDealsPrice { get; set; }
        public double ActualPrice { get; set; }
        public double MemberPaidPrice { get; set; }
        public double CardPaidPrice { get; set; }
        public double PaidPrice { get; set; }
        public double TotalPaidPrice { get; set; }
        public double ReturnPrice { get; set; }
        public double BorrowPrice { get; set; }
        public double KeepPrice { get; set; }
        public long Lang { get; set; }
        public long ReturnType { get; set; }
        public long AdminId { get; set; }
        public long DeviceId { get; set; }
        public Nullable<long> MemberId { get; set; }

        public Nullable<long> SupplierId { get; set; }
        public long Mode { get; set; }
        public long IsPack { get; set; }
        public long PersonCount { get; set; }
        public string Name1 { get; set; }
        public string Name0 { get; set; }
        public string Name2 { get; set; }
        public string Request { get; set; }
        public string Phone { get; set; }
        public string Address1 { get; set; }
        public string Address0 { get; set; }
        public string Address2 { get; set; }
        public long PrintCount { get; set; }
        public long AddTime { get; set; }
        public long ReCheckedCount { get; set; }
        public long IsFreeRoomPrice { get; set; }
        public Nullable<long> UpdateTime { get; set; }
        public long FinishAdminId { get; set; }
        public long FinishDeviceId { get; set; }
        public Nullable<long> FinishTime { get; set; }
        public Nullable<long> SendAdminId { get; set; }
        public string Remark { get; set; }

        
        public Member tb_member { get; set; }

        public Supplier tb_supplier { get; set; }

        public ICollection<TakeoutPay> tb_takeoutpay { get; set; }

        public ICollection<OrderPay> tb_orderpay { get; set; }

        public ICollection<ImportPay> tb_importpay { get; set; }


        public RoomInfoModel RoomInfo { get; set; }


        public List<BillDetailsModel> BillDetailsModelList { get; set; }


        public BillModel(Order source)
        {
            this.BillType = 1;
            this.Id = source.OrderId;
            this.AddTime = source.AddTime;
            this.EndTime = source.EndTime;
            this.FinishTime = source.FinishTime;
            this.FinishAdminId = source.FinishAdminId;
            this.FinishDeviceId = source.FinishDeviceId;
            this.Lang = source.Lang;
            this.PaidPrice = source.PaidPrice;
            this.Remark = source.Remark;
            this.RoomId = source.RoomId;
            this.StartTime = source.StartTime;
            this.State = source.State;
            this.TotalPrice = source.TotalPrice;
            this.OriginalTotalPrice = source.OriginalTotalPrice;
            this.Mode = source.Mode;
            this.IsPack = source.IsPack;
            this.RoomPrice = source.RoomPrice;
            this.IsAutoPay = source.IsAutoPay;
            this.PrintCount = source.PrintCount;
            this.AdminId = source.AdminId;
            this.BorrowPrice = source.BorrowPrice;
            this.DeviceId = source.DeviceId;
            this.IsPack = source.IsPack;
            this.KeepPrice = source.KeepPrice;
            this.MemberId = source.MemberId;
            this.MemberPaidPrice = source.MemberPaidPrice;
            this.Request = source.Request;
            this.RoomPriceCalcTime = source.RoomPriceCalcTime;
            this.TotalPaidPrice = source.TotalPaidPrice;
            this.UpdateTime = source.UpdateTime;
            this.ReCheckedCount = source.ReCheckedCount;
            this.IsFreeRoomPrice = source.IsFreeRoomPrice;

            this.tb_member = source.tb_member;
        }



        public BillModel(Takeout source, ICollection<TakeoutDetail> details, RoomInfoModel roomInfo, bool IsForm = false) : this(source)
        {
            List<BillDetailsModel> billDetailsModelList = new List<BillDetailsModel>();
            if (null != details && details.Count > 0) {
                foreach (var item in details.Where(x => x.State == 0 || x.State == 2))
                {
                    Res.View.Models.BillDetailsModel model = new Res.View.Models.BillDetailsModel();
                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();

                    model.Price = item.Price;
                    model.TotalPrice = item.TotalPrice;
                    model.Count = item.Count;
                    model.ProductName0 = product.ProductName0;
                    model.ProductName1 = product.ProductName1;
                    model.ProductName2 = product.ProductName2;

                    billDetailsModelList.Add(model);
                }

                if (IsForm)
                    billDetailsModelList.Reverse();
            }

            this.BillDetailsModelList = billDetailsModelList;
            this.RoomInfo = roomInfo;
        }
        public BillModel(Order source, Order oldOrder, ICollection<OrderDetail> details, RoomInfoModel roomInfo, bool IsCheckouting, bool IsForm = false) : this(source)
        {
            List<BillDetailsModel> billDetailsModelList = new List<BillDetailsModel>();
            List<BillDetailsModel> billDetailsModelListOriginal = new List<BillDetailsModel>();
            List<BillDetailsModel> billDetailsModelListTemp = new List<BillDetailsModel>();

            if (!IsCheckouting)
            {
                if (null!= oldOrder && null != oldOrder.tb_orderdetail)
                {
                    foreach (var item in oldOrder.tb_orderdetail.Where(x => x.State == 0 || x.State == 2))
                    {
                        Res.View.Models.BillDetailsModel model = new Res.View.Models.BillDetailsModel();
                        Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();

                        model.Price = item.Price;
                        model.TotalPrice = item.TotalPrice;
                        model.Count = item.Count;
                        model.ProductName0 = product.ProductName0;
                        model.ProductName1 = product.ProductName1;
                        model.ProductName2 = product.ProductName2;

                        billDetailsModelListOriginal.Add(model);
                    }
                    
                }
            }

            if (null != details && details.Count > 0)
            {
               
                foreach (var item in details.Where(x => x.State == 0 || x.State == 2))
                {
                    Res.View.Models.BillDetailsModel model = new Res.View.Models.BillDetailsModel();
                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();

                    model.Price = item.Price;
                    model.TotalPrice = item.TotalPrice;
                    model.Count = item.Count;
                    model.ProductName0 = product.ProductName0;
                    model.ProductName1 = product.ProductName1;
                    model.ProductName2 = product.ProductName2;

                    billDetailsModelListTemp.Add(model);
                }

        

            }

           

            if (!IsCheckouting && null != details && details.Count > 0 && !IsForm)
                billDetailsModelListTemp.Reverse();



            billDetailsModelList.AddRange(billDetailsModelListOriginal);

            billDetailsModelList.AddRange(billDetailsModelListTemp);


            billDetailsModelList.Reverse();



            this.BillDetailsModelList = billDetailsModelList;
            this.RoomInfo = roomInfo;

            if (null != this.RoomInfo)
            {
                if (this.RoomInfo.TotalTime == "0:0" || this.RoomInfo.TotalTime == "0/0:0")
                this.RoomInfo.TotalTime = "";
            }
        }
        


        public BillModel(Takeout source)
        {
            this.BillType = 2;
            this.Address1 = source.Address0;
            this.Address0 = source.Address1;
            this.Address2 = source.Address2;
            this.AddTime = source.AddTime;
            this.AdminId = source.AdminId;
            this.BorrowPrice = source.BorrowPrice;
            this.DeviceId = source.DeviceId;
            this.FinishAdminId = source.FinishAdminId;
            this.FinishDeviceId = source.FinishDeviceId;
            this.FinishTime = source.FinishTime;
            this.IsPack = source.IsPack;
            this.KeepPrice = source.KeepPrice;
            this.Lang = source.Lang;
            this.MemberId = source.MemberId;
            this.MemberPaidPrice = source.MemberPaidPrice;
            this.Mode = source.Mode;
            this.Name0 = source.Name0;
            this.Name1 = source.Name0;
            this.Name2 = source.Name2;
            this.OriginalTotalPrice = source.OriginalTotalPrice;
            this.PaidPrice = source.PaidPrice;
            this.PersonCount = source.PersonCount;
            this.Phone = source.Phone;
            this.PrintCount = source.PrintCount;
            this.ReCheckedCount = source.ReCheckedCount;
            this.Remark = source.Remark;
            this.Request = source.Request;
            this.SendAdminId = source.SendAdminId;
            this.ServerTakeoutId = source.ServerTakeoutId;
            this.ServerTakeoutSessionId = source.ServerTakeoutSessionId;
            this.ServerTokenId = source.ServerTokenId;
            this.State = source.State;
            this.Id = source.TakeoutId;
            this.TotalPaidPrice = source.TotalPaidPrice;
            this.TotalPrice = source.TotalPrice;
            this.UpdateTime = source.UpdateTime;

            this.tb_member = source.tb_member;
        }





        /// <summary>
        /// 返回订单
        /// </summary>
        /// <returns></returns>
        public Order GetOrder()
        {
            if (this.BillType != 1)
                return null;

            Order newModel = new Order();
            newModel.OrderId = this.Id;
            newModel.AddTime = this.AddTime;
            newModel.EndTime = this.EndTime;
            newModel.FinishTime = this.FinishTime;
            newModel.FinishAdminId = this.FinishAdminId;
            newModel.FinishDeviceId = this.FinishDeviceId;
            newModel.Lang = this.Lang;
            newModel.PaidPrice = this.PaidPrice;
            newModel.Remark = this.Remark;
            newModel.RoomId = this.RoomId;
            newModel.StartTime = this.StartTime;
            newModel.State = this.State;
            newModel.TotalPrice = this.TotalPrice;
            newModel.OriginalTotalPrice = this.OriginalTotalPrice;
            newModel.Mode = this.Mode;
            newModel.IsPack = this.IsPack;
            newModel.RoomPrice = this.RoomPrice;
            newModel.IsAutoPay = this.IsAutoPay;
            newModel.PrintCount = this.PrintCount;
            newModel.AdminId = this.AdminId;
            newModel.BorrowPrice = this.BorrowPrice;
            newModel.DeviceId = this.DeviceId;
            newModel.IsPack = this.IsPack;
            newModel.KeepPrice = this.KeepPrice;
            newModel.MemberId = this.MemberId;
            newModel.MemberPaidPrice = this.MemberPaidPrice;
            newModel.Request = this.Request;
            newModel.RoomPriceCalcTime = this.RoomPriceCalcTime;
            newModel.TotalPaidPrice = this.TotalPaidPrice;
            newModel.UpdateTime = this.UpdateTime;
            newModel.ReCheckedCount = this.ReCheckedCount;
            newModel.IsFreeRoomPrice = this.IsFreeRoomPrice;

            newModel.tb_member = this.tb_member;


            return newModel;

        }






        /// <summary>
        /// 返回外卖
        /// </summary>
        /// <returns></returns>
        public Takeout GetTakeout()
        {
            if (this.BillType != 2)
                return null;

            Takeout newModel = new Takeout();
            newModel.Address0 = this.Address0;
            newModel.Address1 = this.Address1;
            newModel.Address2 = this.Address2;
            newModel.AddTime = this.AddTime;
            newModel.AdminId = this.AdminId;
            newModel.BorrowPrice = this.BorrowPrice;
            newModel.DeviceId = this.DeviceId;
            newModel.FinishAdminId = this.FinishAdminId;
            newModel.FinishDeviceId = this.FinishDeviceId;
            newModel.FinishTime = this.FinishTime;
            newModel.IsPack = this.IsPack;
            newModel.KeepPrice = this.KeepPrice;
            newModel.Lang = this.Lang;
            newModel.MemberId = this.MemberId;
            newModel.MemberPaidPrice = this.MemberPaidPrice;
            newModel.Mode = this.Mode;
            newModel.Name0 = this.Name0;
            newModel.Name1 = this.Name1;
            newModel.Name2 = this.Name2;
            newModel.OriginalTotalPrice = this.OriginalTotalPrice;
            newModel.PaidPrice = this.PaidPrice;
            newModel.PersonCount = this.PersonCount;
            newModel.Phone = this.Phone;
            newModel.PrintCount = this.PrintCount;
            newModel.ReCheckedCount = this.ReCheckedCount;
            newModel.Remark = this.Remark;
            newModel.Request = this.Request;
            newModel.SendAdminId = this.SendAdminId;
            newModel.ServerTakeoutId = this.ServerTakeoutId;
            newModel.ServerTakeoutSessionId = this.ServerTakeoutSessionId;
            newModel.ServerTokenId = this.ServerTokenId;
            newModel.State = this.State;
            newModel.TakeoutId = this.Id;
            newModel.TotalPaidPrice = this.TotalPaidPrice;
            newModel.TotalPrice = this.TotalPrice;
            newModel.UpdateTime = this.UpdateTime;

            newModel.tb_member = this.tb_member;

            return newModel;

        }






    }
}
