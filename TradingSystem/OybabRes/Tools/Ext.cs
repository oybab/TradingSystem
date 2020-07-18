using Newtonsoft.Json;
using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.Tools
{
    public static class Ext
    {

        /// <summary>
        /// 替换之前的地方
        /// </summary>
        /// <param name="original"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public static string ReplaceFirstOccurrance(this string original, string oldValue, string newValue)
        {
            if (String.IsNullOrEmpty(original))
                return String.Empty;
            if (String.IsNullOrEmpty(oldValue))
                return original;
            if (String.IsNullOrEmpty(newValue))
                newValue = String.Empty;
            int loc = original.IndexOf(oldValue);
            return original.Remove(loc, oldValue.Length).Insert(loc, newValue);
        }

        /// <summary>
        /// 去掉前部分
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimStart(this string target, string trimString)
        {
            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }

            return result;
        }

        /// <summary>
        /// 去掉后部分
        /// </summary>
        /// <param name="target"></param>
        /// <param name="trimString"></param>
        /// <returns></returns>
        public static string TrimEnd(this string target, string trimString)
        {
            string result = target;
            while (result.EndsWith(trimString))
            {
                result = result.Substring(0, result.Length - trimString.Length);
            }

            return result;
        }

        /// <summary>
        /// 检测3个值是否相等
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <returns></returns>
        public static bool AllSame(string value1, string value2, string value3)
        {
            return (value1 == value2 && value2 == value3);
        }

        /// <summary>
        /// 快速比较比较
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }



        

       


        /// <summary>
        /// 快速复制余额信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Balance FastCopy(this Balance source, bool IsIncludeRef = false)
        {
            Balance newModel = new Balance();
            newModel.AccountBank = source.AccountBank;
            newModel.AccountName = source.AccountName;
            newModel.AccountNo = source.AccountNo;
            newModel.AddTime = source.AddTime;
            newModel.BalanceId = source.BalanceId;
            newModel.BalanceName0 = source.BalanceName0;
            newModel.BalanceName1 = source.BalanceName1;
            newModel.BalanceName2 = source.BalanceName2;
            newModel.BalancePrice = source.BalancePrice;
            newModel.BalanceType = source.BalanceType;
            newModel.IsBind = source.IsBind;
            newModel.Remark = source.Remark;
            newModel.UpdateTime = source.UpdateTime;
            newModel.RemoveRate = source.RemoveRate;
            newModel.HideType = source.HideType;
            newModel.Order = source.Order;

            if (IsIncludeRef)
            {
                newModel.tb_balancepay = source.tb_balancepay;
                newModel.tb_adminpay = source.tb_adminpay;
                newModel.tb_importpay = source.tb_importpay;
                newModel.tb_memberpay = source.tb_memberpay;
                newModel.tb_orderpay = source.tb_orderpay;
                newModel.tb_supplierpay = source.tb_supplierpay;
                newModel.tb_takeoutpay = source.tb_takeoutpay;
            }

            return newModel;
        }


        /// <summary>
        /// 快速复制订单信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Order FastCopy(this Order source, bool IsIncludeRef = false)
        {
            Order newModel = new Order();
            newModel.OrderId = source.OrderId;
            newModel.AddTime = source.AddTime;
            newModel.EndTime = source.EndTime;
            newModel.FinishTime = source.FinishTime;
            newModel.FinishAdminId = source.FinishAdminId;
            newModel.FinishDeviceId = source.FinishDeviceId;
            newModel.Lang = source.Lang;
            newModel.PaidPrice = source.PaidPrice;
            newModel.Remark = source.Remark;
            newModel.RoomId = source.RoomId;
            newModel.StartTime = source.StartTime;
            newModel.State = source.State;
            newModel.TotalPrice = source.TotalPrice;
            newModel.OriginalTotalPrice = source.OriginalTotalPrice;
            newModel.Mode = source.Mode;
            newModel.IsPack = source.IsPack;
            newModel.RoomPrice = source.RoomPrice;
            newModel.IsPayByTime = source.IsPayByTime;
            newModel.IsFreeRoomPrice = source.IsFreeRoomPrice;
            newModel.IsAutoPay = source.IsAutoPay;
            newModel.PrintCount = source.PrintCount;
            newModel.AdminId = source.AdminId;
            newModel.BorrowPrice = source.BorrowPrice;
            newModel.DeviceId = source.DeviceId;
            newModel.IsPack = source.IsPack;
            newModel.KeepPrice = source.KeepPrice;
            newModel.MemberId = source.MemberId;
            newModel.MemberPaidPrice = source.MemberPaidPrice;
            newModel.Request = source.Request;
            newModel.RoomPriceCalcTime = source.RoomPriceCalcTime;
            newModel.TotalPaidPrice = source.TotalPaidPrice;
            newModel.UpdateTime = source.UpdateTime;
            newModel.ReCheckedCount = source.ReCheckedCount;

            if (IsIncludeRef)
            {
                newModel.tb_admin = source.tb_admin;
                newModel.tb_admin1 = source.tb_admin1;
                newModel.tb_device = source.tb_device;
                newModel.tb_device1 = source.tb_device1;
                newModel.tb_member = source.tb_member;
                newModel.tb_orderdetail = source.tb_orderdetail;
                newModel.tb_room = source.tb_room;
                newModel.tb_orderpay = source.tb_orderpay;
            }

            return newModel;
        }





        /// <summary>
        /// 快速复制外卖信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Takeout FastCopy(this Takeout source, bool IsIncludeRef = false)
        {
            Takeout newModel = new Takeout();
            newModel.Address0 = source.Address0;
            newModel.Address1 = source.Address1;
            newModel.Address2 = source.Address2;
            newModel.AddTime = source.AddTime;
            newModel.AdminId = source.AdminId;
            newModel.BorrowPrice = source.BorrowPrice;
            newModel.DeviceId = source.DeviceId;
            newModel.FinishAdminId = source.FinishAdminId;
            newModel.FinishDeviceId = source.FinishDeviceId;
            newModel.FinishTime = source.FinishTime;
            newModel.IsPack = source.IsPack;
            newModel.KeepPrice = source.KeepPrice;
            newModel.Lang = source.Lang;
            newModel.MemberId = source.MemberId;
            newModel.MemberPaidPrice = source.MemberPaidPrice;
            newModel.Mode = source.Mode;
            newModel.Name0 = source.Name0;
            newModel.Name1 = source.Name1;
            newModel.Name2 = source.Name2;
            newModel.OriginalTotalPrice = source.OriginalTotalPrice;
            newModel.PaidPrice = source.PaidPrice;
            newModel.PersonCount = source.PersonCount;
            newModel.Phone = source.Phone;
            newModel.PrintCount = source.PrintCount;
            newModel.ReCheckedCount = source.ReCheckedCount;
            newModel.Remark = source.Remark;
            newModel.Request = source.Request;
            newModel.SendAdminId = source.SendAdminId;
            newModel.ServerTakeoutId = source.ServerTakeoutId;
            newModel.ServerTakeoutSessionId = source.ServerTakeoutSessionId;
            newModel.ServerTokenId = source.ServerTokenId;
            newModel.State = source.State;
            newModel.TakeoutId = source.TakeoutId;
            newModel.TotalPaidPrice = source.TotalPaidPrice;
            newModel.TotalPrice = source.TotalPrice;
            newModel.UpdateTime = source.UpdateTime;
            newModel.SendTime = source.SendTime;


            if (IsIncludeRef)
            {
                newModel.tb_admin = source.tb_admin;
                newModel.tb_admin1 = source.tb_admin1;
                newModel.tb_admin2 = source.tb_admin2;
                newModel.tb_device = source.tb_device1;
                newModel.tb_device1 = source.tb_device1;
                newModel.tb_member = source.tb_member;
                newModel.tb_takeoutdetail = source.tb_takeoutdetail;
                newModel.tb_takeoutpay = source.tb_takeoutpay;
            }

            return newModel;
        }




        /// <summary>
        /// 快速复制支出信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Import FastCopy(this Import source, bool IsIncludeRef = false)
        {
            Import newModel = new Import();
            newModel.AddTime = source.AddTime;
            newModel.AdminId = source.AdminId;
            newModel.BorrowPrice = source.BorrowPrice;
            newModel.DeviceId = source.DeviceId;
            newModel.ImportId = source.ImportId;
            newModel.ImportTime = source.ImportTime;
            newModel.KeepPrice = source.KeepPrice;
            newModel.Mode = source.Mode;
            newModel.OriginalTotalPrice = source.OriginalTotalPrice;
            newModel.PaidPrice = source.PaidPrice;
            newModel.PrintCount = source.PrintCount;
            newModel.ReCheckedCount = source.ReCheckedCount;
            newModel.Remark = source.Remark;
            newModel.State = source.State;
            newModel.SupplierId = source.SupplierId;
            newModel.SupplierPaidPrice = source.SupplierPaidPrice;
            newModel.TotalPaidPrice = source.TotalPaidPrice;
            newModel.TotalPrice = source.TotalPrice;
            newModel.UpdateTime = source.UpdateTime;
            


            if (IsIncludeRef)
            {
                newModel.tb_admin = source.tb_admin;
                newModel.tb_device = source.tb_device;
                newModel.tb_importdetail = source.tb_importdetail;
                newModel.tb_importpay = source.tb_importpay;
                newModel.tb_supplier = source.tb_supplier;
            }

            return newModel;
        }







        /// <summary>
        /// 快速复制产品类型信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Product FastCopy(this Product source, bool IsIncludeRef = false)
        {
            Product newModel = new Product();
            newModel.AddTime = source.AddTime;
            newModel.BalanceCount = source.BalanceCount;
            newModel.Barcode = source.Barcode;
            newModel.CostPrice = source.CostPrice;
            newModel.CostPriceChangeMode = source.CostPriceChangeMode;
            newModel.CostPriceDownLimit = source.CostPriceDownLimit;
            newModel.CostPriceUpLimit = source.CostPriceUpLimit;
            newModel.Description = source.Description;
            newModel.HideType = source.HideType;
            newModel.ImageName = source.ImageName;
            newModel.IsBindCount = source.IsBindCount;
            newModel.ExpiredTime = source.ExpiredTime;
            newModel.ExpiredTimeCount = source.ExpiredTimeCount;
            newModel.ProductParentId = source.ProductParentId;
            newModel.ProductParentCount = source.ProductParentCount;
            newModel.Order = source.Order;
            newModel.Price = source.Price;
            newModel.PriceChangeMode = source.PriceChangeMode;
            newModel.PriceDownLimit = source.PriceDownLimit;
            newModel.PriceUpLimit = source.PriceUpLimit;
            newModel.ProductId = source.ProductId;
            newModel.ProductName0 = source.ProductName0;
            newModel.ProductName1 = source.ProductName1;
            newModel.ProductName2 = source.ProductName2;
            newModel.ProductTypeId = source.ProductTypeId;
            newModel.Remark = source.Remark;
            newModel.UpdateTime = source.UpdateTime;
            newModel.WarningCount = source.WarningCount;

            newModel.Specification0 = source.Specification0;
            newModel.Specification1 = source.Specification1;
            newModel.Specification2 = source.Specification2;
            newModel.Source0 = source.Source0;
            newModel.Source1 = source.Source1;
            newModel.Source2 = source.Source2;
            newModel.Info0 = source.Info0;
            newModel.Info1 = source.Info1;
            newModel.Info2 = source.Info2;


            newModel.ProductLocalType = source.ProductLocalType;
            newModel.ProductServerType = source.ProductServerType;
            newModel.ProductLocalHash = source.ProductLocalHash;
            newModel.ProductServerHash = source.ProductServerHash;
            newModel.IsScales = source.IsScales;

            if (IsIncludeRef)
            {
                newModel.tb_importdetail = source.tb_importdetail;
                newModel.tb_orderdetail = source.tb_orderdetail;
                newModel.tb_Ppr = source.tb_Ppr;
                newModel.tb_producttype = source.tb_producttype;
                newModel.tb_takeoutdetail = source.tb_takeoutdetail;
                newModel.tb_product1 = source.tb_product1;
                newModel.tb_product2 = source.tb_product2;
            }

            return newModel;
        }



        /// <summary>
        /// 快速复制产品类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static ProductType FastCopy(this ProductType source, bool IsIncludeRef = false)
        {
            ProductType newModel = new ProductType();
            newModel.AddTime = source.AddTime;
            newModel.Description = source.Description;
            newModel.HideType = source.HideType;
            newModel.ImageName = source.ImageName;
            newModel.Order = source.Order;
            newModel.ParentId = source.ParentId;
            newModel.ProductTypeId = source.ProductTypeId;
            newModel.ProductTypeName0 = source.ProductTypeName0;
            newModel.ProductTypeName1 = source.ProductTypeName1;
            newModel.ProductTypeName2 = source.ProductTypeName2;
            newModel.Remark = source.Remark;
            newModel.UpdateTime = source.UpdateTime;
            

            if (IsIncludeRef)
            {
                newModel.tb_product = source.tb_product;
                newModel.tb_producttype1 = source.tb_producttype1;
                newModel.tb_producttype2 = source.tb_producttype2;
            }

            return newModel;
        }



        /// <summary>
        /// 快速复制雅座信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Room FastCopy(this Room source, bool IsIncludeRef = false)
        {
            Room newModel = new Room();
            newModel.AddTime = source.AddTime;
            newModel.HideType = source.HideType;
            newModel.IsAutoExtendTime = source.IsAutoExtendTime;
            newModel.IsPayByTime = source.IsPayByTime;
            newModel.Order = source.Order;
            newModel.Price = source.Price;
            newModel.PriceHour = source.PriceHour;
            newModel.FreeRoomPriceLimit = source.FreeRoomPriceLimit;
            newModel.Remark = source.Remark;
            newModel.RoomId = source.RoomId;
            newModel.RoomNo = source.RoomNo;
            newModel.UpdateTime = source.UpdateTime;
            newModel.DeviceOfCount = source.DeviceOfCount;

            newModel.RoomType = source.RoomType;
            newModel.RoomTypeName = source.RoomTypeName;
            

            if (IsIncludeRef)
            {
                newModel.tb_device = source.tb_device;
                newModel.tb_order = source.tb_order;
            }

            return newModel;
        }


        /// <summary>
        /// 快速复制会员信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Member FastCopy(this Member source, bool IsIncludeRef = false)
        {
            Member newModel = new Member();
            newModel.Address0 = source.Address0;
            newModel.Address1 = source.Address1;
            newModel.Address2 = source.Address2;
            newModel.AddTime = source.AddTime;
            newModel.AdminId = source.AdminId;
            newModel.BalancePrice = source.BalancePrice;
            newModel.MinBalancePrice = source.MinBalancePrice;
            newModel.MaxBalancePrice = source.MaxBalancePrice;
            newModel.ExpiredTime = source.ExpiredTime;
            newModel.IsAllowBorrow = source.IsAllowBorrow;
            newModel.FavorablePrice = source.FavorablePrice;
            newModel.IDNumber = source.IDNumber;
            newModel.ImageName = source.ImageName;
            newModel.IsEnable = source.IsEnable;
            newModel.Lang = source.Lang;
            newModel.MemberId = source.MemberId;
            newModel.MemberName0 = source.MemberName0;
            newModel.MemberName1 = source.MemberName1;
            newModel.MemberName2 = source.MemberName2;
            newModel.MemberNo = source.MemberNo;
            newModel.Mobile = source.Mobile;
            newModel.Occupation = source.Occupation;
            newModel.OfferRate = source.OfferRate;
            newModel.Order = source.Order;
            newModel.Password = source.Password;
            newModel.Phone = source.Phone;
            newModel.Remark = source.Remark;
            newModel.Sex = source.Sex;
            newModel.SpendPrice = source.SpendPrice;
            newModel.UpdateTime = source.UpdateTime;
            newModel.AddressLocation = source.AddressLocation;
            newModel.CardNo = source.CardNo;

            if (IsIncludeRef)
            {
                newModel.tb_admin = source.tb_admin;
                newModel.tb_memberpay = source.tb_memberpay;
                newModel.tb_order = source.tb_order;
                newModel.tb_takeout = source.tb_takeout;
                newModel.tb_orderpay = source.tb_orderpay;
                newModel.tb_takeoutpay = source.tb_takeoutpay;
            }

            return newModel;
        }



        /// <summary>
        /// 快速复制供应商信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Supplier FastCopy(this Supplier source, bool IsIncludeRef = false)
        {
            Supplier newModel = new Supplier();
            newModel.Address0 = source.Address0;
            newModel.Address1 = source.Address1;
            newModel.Address2 = source.Address2;
            newModel.AddTime = source.AddTime;
            newModel.AdminId = source.AdminId;
            newModel.BalancePrice = source.BalancePrice;
            newModel.MinBalancePrice = source.MinBalancePrice;
            newModel.MaxBalancePrice = source.MaxBalancePrice;
            newModel.ExpiredTime = source.ExpiredTime;
            newModel.IsAllowBorrow = source.IsAllowBorrow;
            newModel.FavorablePrice = source.FavorablePrice;
            newModel.IDNumber = source.IDNumber;
            newModel.ImageName = source.ImageName;
            newModel.IsEnable = source.IsEnable;
            newModel.Lang = source.Lang;
            newModel.SupplierId = source.SupplierId;
            newModel.SupplierName0 = source.SupplierName0;
            newModel.SupplierName1 = source.SupplierName1;
            newModel.SupplierName2 = source.SupplierName2;
            newModel.SupplierNo = source.SupplierNo;
            newModel.Mobile = source.Mobile;
            newModel.Occupation = source.Occupation;
            newModel.OfferRate = source.OfferRate;
            newModel.Order = source.Order;
            newModel.Password = source.Password;
            newModel.Phone = source.Phone;
            newModel.Remark = source.Remark;
            newModel.Sex = source.Sex;
            newModel.SpendPrice = source.SpendPrice;
            newModel.UpdateTime = source.UpdateTime;
            newModel.AddressLocation = source.AddressLocation;
            newModel.CardNo = source.CardNo;

            if (IsIncludeRef)
            {
                newModel.tb_admin = source.tb_admin;
                newModel.tb_import = source.tb_import;
                newModel.tb_supplierpay = source.tb_supplierpay;
                newModel.tb_importpay = source.tb_importpay;
                newModel.tb_supplierpay = source.tb_supplierpay;
            }

            return newModel;
        }


        /// <summary>
        /// 快速复制设备信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Device FastCopy(this Device source, bool IsIncludeRef = false)
        {
            Device newModel = new Device();
            newModel.AddTime = source.AddTime;
            newModel.DeviceId = source.DeviceId;
            newModel.DeviceNo = source.DeviceNo;
            newModel.DeviceType = source.DeviceType;
            newModel.IpAddress = source.IpAddress;
            newModel.IsEnable = source.IsEnable;
            newModel.MacAddress = source.MacAddress;
            newModel.Order = source.Order;
            newModel.Remark = source.Remark;
            newModel.RoomId = source.RoomId;
            newModel.UpdateTime = source.UpdateTime;

            if (IsIncludeRef)
            {
                newModel.tb_adminpay = source.tb_adminpay;
                newModel.tb_import = source.tb_import;
                newModel.tb_importdetail = source.tb_importdetail;
                newModel.tb_memberpay = source.tb_memberpay;
                newModel.tb_order = source.tb_order;
                newModel.tb_order1 = source.tb_order1;
                newModel.tb_orderdetail = source.tb_orderdetail;
                newModel.tb_orderdetail1 = source.tb_orderdetail1;
                newModel.tb_room = source.tb_room;
                newModel.tb_takeout = source.tb_takeout;
                newModel.tb_takeout1 = source.tb_takeout1;
                newModel.tb_takeoutdetail = source.tb_takeoutdetail;
                newModel.tb_takeoutdetail1 = source.tb_takeoutdetail1;
                newModel.tb_supplierpay = source.tb_supplierpay;
                newModel.tb_adminlog = source.tb_adminlog;
                newModel.tb_balancepay = source.tb_balancepay;


                newModel.tb_orderpay = source.tb_orderpay;
                newModel.tb_takeoutpay = source.tb_takeoutpay;
                newModel.tb_importpay = source.tb_importpay;

            }

            return newModel;
        }



        /// <summary>
        /// 快速复制打印信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Printer FastCopy(this Printer source, bool IsIncludeRef = false)
        {
            Printer newModel = new Printer();
            newModel.AddTime = source.AddTime;
            newModel.IsEnable = source.IsEnable;
            newModel.IsMain = source.IsMain;
            newModel.IsCashDrawer = source.IsCashDrawer;
            newModel.Lang = source.Lang;
            newModel.Order = source.Order;
            newModel.PrinterDeviceName = source.PrinterDeviceName;
            newModel.PrinterAddress = source.PrinterAddress;
            newModel.PrinterId = source.PrinterId;
            newModel.PrinterName0 = source.PrinterName0;
            newModel.PrinterName1 = source.PrinterName1;
            newModel.PrinterName2 = source.PrinterName2;
            newModel.PrintSize = source.PrintSize;
            newModel.PrintType = source.PrintType;
            newModel.Remark = source.Remark;
            newModel.UpdateTime = source.UpdateTime;
            

            if (IsIncludeRef)
            {
                newModel.tb_Ppr = source.tb_Ppr;
            }

            return newModel;
        }




        /// <summary>
        /// 快速复制请求信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Request FastCopy(this Request source, bool IsIncludeRef = false)
        {
            Request newModel = new Request();
            newModel.AddTime = source.AddTime;
            newModel.IsEnable = source.IsEnable;
            newModel.Order = source.Order;
            newModel.Remark = source.Remark;
            newModel.RequestId = source.RequestId;
            newModel.RequestName0 = source.RequestName0;
            newModel.RequestName1 = source.RequestName1;
            newModel.RequestName2 = source.RequestName2;
            newModel.UpdateTime = source.UpdateTime;


            if (IsIncludeRef)
            {
            }

            return newModel;
        }


        /// <summary>
        /// 快速复制管理员信息
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static Admin FastCopy(this Admin source, bool IsIncludeRef = false)
        {
            Admin newModel = new Admin();
            newModel.Address0 = source.Address0;
            newModel.Address1 = source.Address1;
            newModel.Address2 = source.Address2;
            newModel.AddTime = source.AddTime;
            newModel.AdminId = source.AdminId;
            newModel.AdminName0 = source.AdminName0;
            newModel.AdminName1 = source.AdminName1;
            newModel.AdminName2 = source.AdminName2;
            newModel.AdminNo = source.AdminNo;
            newModel.IDNumber = source.IDNumber;
            newModel.ImageName = source.ImageName;
            newModel.IsEnable = source.IsEnable;
            newModel.IsOnlyOwn = source.IsOnlyOwn;
            newModel.Lang = source.Lang;
            newModel.Menu = source.Menu;
            newModel.Mobile = source.Mobile;
            newModel.Mode = source.Mode;
            newModel.Order = source.Order;
            newModel.Password = source.Password;
            newModel.Remark = source.Remark;
            newModel.Sex = source.Sex;
            newModel.UpdateTime = source.UpdateTime;
            newModel.Occupation = source.Occupation;

            newModel.BalancePrice = source.BalancePrice;
            newModel.OfferRate = source.OfferRate;
            newModel.Salt = source.Salt;
            newModel.FavorablePrice = source.FavorablePrice;
            newModel.SpendPrice = source.SpendPrice;

            if (IsIncludeRef)
            {
                newModel.tb_adminpay = source.tb_adminpay;
                newModel.tb_adminpay1 = source.tb_adminpay1;
                newModel.tb_import = source.tb_import;
                newModel.tb_importdetail = source.tb_importdetail;
                newModel.tb_member = source.tb_member;
                newModel.tb_memberpay = source.tb_memberpay;
                newModel.tb_order = source.tb_order;
                newModel.tb_order1 = source.tb_order1;
                newModel.tb_orderdetail = source.tb_orderdetail;
                newModel.tb_orderdetail1 = source.tb_orderdetail1;
                newModel.tb_takeout = source.tb_takeout;
                newModel.tb_takeout1 = source.tb_takeout1;
                newModel.tb_takeout2 = source.tb_takeout2;
                newModel.tb_takeoutdetail = source.tb_takeoutdetail;
                newModel.tb_takeoutdetail1 = source.tb_takeoutdetail1;
                newModel.tb_supplier = source.tb_supplier;
                newModel.tb_supplierpay = source.tb_supplierpay;
                newModel.tb_adminlog = source.tb_adminlog;
                newModel.tb_balancepay = source.tb_balancepay;


                newModel.tb_orderpay = source.tb_orderpay;
                newModel.tb_takeoutpay = source.tb_takeoutpay;
                newModel.tb_importpay = source.tb_importpay;
            }

            return newModel;
        }



        /// <summary>
        /// 快速复制管理员日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="IsIncludeRef"></param>
        /// <returns></returns>
        public static AdminLog FastCopy(this AdminLog source, bool IsIncludeRef = false)
        {
            AdminLog newModel = new AdminLog();
            newModel.AddTime = source.AddTime;
            newModel.AdminId = source.AdminId;
            newModel.AdminLogId = source.AdminLogId;
            newModel.DeviceId = source.DeviceId;
            newModel.IsEnable = source.IsEnable;
            newModel.LogContent = source.LogContent;
            newModel.LogMode = source.LogMode;
            newModel.LogTime = source.LogTime;
            newModel.LogTitle = source.LogTitle;
            newModel.LogType = source.LogType;
            newModel.Mode = source.Mode;
            newModel.Remark = source.Remark;
            newModel.UpdateTime = source.UpdateTime;


            if (IsIncludeRef)
            {
                newModel.tb_admin = source.tb_admin;
                newModel.tb_device = source.tb_device;
            }

            return newModel;
        }


        /// <summary>
        /// 安全解析Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T DeserializeObject<T>(this string json) where T : new()
        {
            return json != null && json != "null"
                           ? JsonConvert.DeserializeObject<T>(json)
                           : new T();
        }


        
    }
}
