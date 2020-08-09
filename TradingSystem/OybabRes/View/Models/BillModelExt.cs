using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Models
{
    public sealed partial class BillModel
    {
        public RoomInfoModel RoomInfo { get; set; }


        public List<BillDetailsModel> BillDetailsModelList { get; set; }




        public BillModel(Takeout source, ICollection<TakeoutDetail> details, RoomInfoModel roomInfo, bool IsForm = false) : this(source)
        {
            List<BillDetailsModel> billDetailsModelList = new List<BillDetailsModel>();
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
                if (null != oldOrder && null != oldOrder.tb_orderdetail)
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




    }
}
