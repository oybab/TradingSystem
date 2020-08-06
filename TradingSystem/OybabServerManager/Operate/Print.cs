using Oybab.DAL;
using Oybab.Report;
using Oybab.Report.CommonHWP;
using Oybab.Report.Model;
using Oybab.ServerManager.Exceptions;
using Oybab.ServerManager.Model.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Oybab.ServerManager.Resources;

namespace Oybab.ServerManager.Operate
{
    internal sealed class Print
    {
        private Print() { }
        private static readonly Lazy<Print> lazy = new Lazy<Print>(() => new Print());
        public static Print Instance { get { return lazy.Value; } }



        /// <summary>
        /// 购买时打印订单
        /// </summary>
        /// <param name="client"></param>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="oldOrder"></param>
        public void PrintOrderAfterBuy(Client client, Order order, List<OrderDetail> orderDetails, Order oldOrder)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {

                    // 存储打印-属于该打印机打印的客户订单详情
                    Dictionary<Printer, List<OrderDetail>> printDetails = new Dictionary<Printer, List<OrderDetail>>();

                    // 循环获取所有订单详情
                    if (null != orderDetails)
                    {
                        foreach (var item in orderDetails)
                        {
                            // 查询在该详情的产品名中有无跟打印和产品关联的
                            List<Ppr> pprs = Resources.GetRes().PPRS.Where(x => x.ProductId == item.ProductId).ToList();
                            if (null != pprs && pprs.Count > 0)
                            {
                                foreach (var ppr in pprs)
                                {
                                    // 获取有该详情有关联的打印机, 并确保是账单类型的, 并开启的
                                    List<Printer> printers = Resources.GetRes().PRINTERS.Where(x => x.PrinterId == ppr.PrinterId && x.IsEnable == 1).Distinct().ToList();

                                    if (null != printers)
                                    {
                                        foreach (var printer in printers)
                                        {
                                            if (printDetails.ContainsKey(printer))
                                            {
                                                printDetails[printer].Add(item);
                                            }
                                            else
                                            {
                                                printDetails.Add(printer, new List<OrderDetail>() { item });
                                            }
                                        }
                                    }
                                }
                            }

                            // 获取所有打印机中的开启的并打印所有的打印机
                            List<Printer> mainPrinters = Resources.GetRes().PRINTERS.Where(x => x.IsEnable == 1 && x.IsMain == 1).Distinct().ToList();
                            if (null != mainPrinters)
                            {
                                foreach (var printer in mainPrinters)
                                {
                                    if (printDetails.ContainsKey(printer))
                                    {
                                        if (!printDetails[printer].Contains(item))
                                            printDetails[printer].Add(item);
                                    }
                                    else
                                    {
                                        printDetails.Add(printer, new List<OrderDetail>() { item });
                                    }

                                }
                            }
                        }
                    }

                    // 打印客户账单
                    // 客户端没自己打印的情况下则打印出来
                    // 如果是购买时打印才打印出来
                    if (Resources.GetRes().PrintInfo.IsPrintBillAfterBuy && !client.IsLocalPrintCustomOrder)
                    {
                        // 开始循环打印客户账单部分
                        Dictionary<Printer, List<OrderDetail>> printOrderDetails = printDetails.Where(x => x.Key.PrintType == 0 || x.Key.PrintType == 2).ToDictionary(x => x.Key, x => x.Value);
                        PrintCustomerOrderAfterBuy(printOrderDetails, order, oldOrder);
                    }

                    // 任何时候都要打印后厨账单
                    // 开始循环打印客户账单部分
                    Dictionary<Printer, List<OrderDetail>> printOrderDetails2 = printDetails.Where(x => x.Key.PrintType == 1).ToDictionary(x => x.Key, x => x.Value);
                    foreach (var item in printOrderDetails2)
                    {
                        PrintOrderToBackstageProcess(item.Key, item.Value, order);
                    }


                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            });

        }



        /// <summary>
        /// 结账时打印订单
        /// </summary>
        /// <param name="client"></param>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="Lang"></param>
        public void PrintOrderAfterCheckout(Client client, Order order, List<OrderDetail> orderDetails, long Lang = -1)
        {
            // 如果是本地打印账单才继续
            if ((Resources.GetRes().PrintInfo.IsPrintBillAfterCheckout || Lang != -1) && (null == client || !client.IsLocalPrintCustomOrder))
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        // 存储打印-属于该打印机打印的客户订单详情
                        Dictionary<Printer, List<OrderDetail>> printDetails = new Dictionary<Printer, List<OrderDetail>>();

                        if (null != orderDetails)
                        {
                            // 循环获取所有订单详情
                            foreach (var item in orderDetails)
                            {
                                // 查询在该详情的产品名中有无跟打印和产品关联的
                                List<Ppr> pprs = Resources.GetRes().PPRS.Where(x => x.ProductId == item.ProductId).ToList();
                                if (null != pprs && pprs.Count > 0)
                                {
                                    foreach (var ppr in pprs)
                                    {
                                        // 获取有该详情有关联的打印机, 并确保是账单类型的, 并开启的
                                        List<Printer> printers = Resources.GetRes().PRINTERS.Where(x => x.PrinterId == ppr.PrinterId && x.IsEnable == 1).Distinct().ToList();

                                        if (null != printers)
                                        {
                                            foreach (var printer in printers)
                                            {
                                                if (printDetails.ContainsKey(printer))
                                                {
                                                    printDetails[printer].Add(item);
                                                }
                                                else
                                                {
                                                    printDetails.Add(printer, new List<OrderDetail>() { item });
                                                }
                                            }
                                        }
                                    }
                                }


                                // 获取所有打印机中的开启的并打印所有的打印机
                                List<Printer> mainPrinters = Resources.GetRes().PRINTERS.Where(x => x.IsEnable == 1 && x.IsMain == 1).Distinct().ToList();
                                if (null != mainPrinters)
                                {
                                    foreach (var printer in mainPrinters)
                                    {
                                        if (printDetails.ContainsKey(printer))
                                        {
                                            if (!printDetails[printer].Contains(item))
                                                printDetails[printer].Add(item);
                                        }
                                        else
                                        {
                                            printDetails.Add(printer, new List<OrderDetail>() { item });
                                        }

                                    }
                                }
                            }
                        }

                        // 打印客户账单
                        // 开始循环打印客户账单部分
                        Dictionary<Printer, List<OrderDetail>> printOrderDetails = printDetails.Where(x => x.Key.PrintType == 0 || x.Key.PrintType == 2).ToDictionary(x => x.Key, x => x.Value);
                        PrintCustomerOrderAfterCheckout(printOrderDetails, order, Lang);



                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                });
            }
        }




        /// <summary>
        /// 结账时打印外卖
        /// </summary>
        /// <param name="client"></param>
        /// <param name="order"></param>
        /// <param name="orderDetails"></param>
        /// <param name="Lang"></param>
        public void PrintOrderAfterCheckout(Client client, Takeout takeout, List<TakeoutDetail> takeoutDetails, long Lang = -1)
        {

            Task.Factory.StartNew(() =>
            {
                try
                {

                    // 存储打印-属于该打印机打印的客户订单详情
                    Dictionary<Printer, List<TakeoutDetail>> printDetails = new Dictionary<Printer, List<TakeoutDetail>>();

                    if (null != takeoutDetails)
                    {
                        // 循环获取所有订单详情
                        foreach (var item in takeoutDetails)
                        {
                            // 查询在该详情的产品名中有无跟打印和产品关联的
                            List<Ppr> pprs = Resources.GetRes().PPRS.Where(x => x.ProductId == item.ProductId).ToList();
                            if (null != pprs && pprs.Count > 0)
                            {
                                foreach (var ppr in pprs)
                                {
                                    // 获取有该详情有关联的打印机, 并确保是账单类型的, 并开启的
                                    List<Printer> printers = Resources.GetRes().PRINTERS.Where(x => x.PrinterId == ppr.PrinterId && x.IsEnable == 1).Distinct().ToList();

                                    if (null != printers)
                                    {
                                        foreach (var printer in printers)
                                        {
                                            if (printDetails.ContainsKey(printer))
                                            {
                                                printDetails[printer].Add(item);
                                            }
                                            else
                                            {
                                                printDetails.Add(printer, new List<TakeoutDetail>() { item });
                                            }
                                        }
                                    }
                                }
                            }


                            // 获取所有打印机中的开启的并打印所有的打印机
                            List<Printer> mainPrinters = Resources.GetRes().PRINTERS.Where(x => x.IsEnable == 1 && x.IsMain == 1).Distinct().ToList();
                            if (null != mainPrinters)
                            {
                                foreach (var printer in mainPrinters)
                                {
                                    if (printDetails.ContainsKey(printer))
                                    {
                                        if (!printDetails[printer].Contains(item))
                                            printDetails[printer].Add(item);
                                    }
                                    else
                                    {
                                        printDetails.Add(printer, new List<TakeoutDetail>() { item });
                                    }

                                }
                            }
                        }
                    }


                    // 如果是购买时打印才打印出来
                    if ((Resources.GetRes().PrintInfo.IsPrintBillAfterCheckout || Lang != -1) && (null == client || !client.IsLocalPrintCustomOrder))
                    {
                        // 打印客户账单
                        // 开始循环打印客户账单部分
                        Dictionary<Printer, List<TakeoutDetail>> printOrderDetails = printDetails.Where(x => x.Key.PrintType == 0 || x.Key.PrintType == 2).ToDictionary(x => x.Key, x => x.Value);
                        PrintCustomerTakeoutAfterCheckout(printOrderDetails, takeout, Lang);
                    }


                    // 有客户端,说明不是历史打印, 则打印后厨
                    if (null != client)
                    {
                        // 任何时候都要打印后厨账单
                        // 开始循环打印客户账单部分
                        Dictionary<Printer, List<TakeoutDetail>> printOrderDetails2 = printDetails.Where(x => x.Key.PrintType == 1).ToDictionary(x => x.Key, x => x.Value);
                        foreach (var item in printOrderDetails2)
                        {
                            PrintOrderToBackstageProcess(item.Key, item.Value, takeout);
                        }

                    }

                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            });
        }

        

       


        /// <summary>
        /// 进货打印
        /// </summary>
        /// <param name="client"></param>
        /// <param name="import"></param>
        /// <param name="importDetails"></param>
        /// <param name="Lang"></param>
        public void PrintImport(Client client, Import import, List<ImportDetail> importDetails, long Lang = -1)
        {
            // 如果是本地打印账单才继续
            if (null == client || !client.IsLocalPrintCustomOrder)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        // 存储打印-属于该打印机打印的客户订单详情
                        Dictionary<Printer, List<ImportDetail>> printDetails = new Dictionary<Printer, List<ImportDetail>>();

                        // 循环获取所有订单详情
                        foreach (var item in importDetails)
                        {
                            // 查询在该详情的产品名中有无跟打印和产品关联的
                            List<Ppr> pprs = Resources.GetRes().PPRS.Where(x => x.ProductId == item.ProductId).ToList();
                            if (null != pprs && pprs.Count > 0)
                            {
                                foreach (var ppr in pprs)
                                {
                                    // 获取有该详情有关联的打印机, 并确保是账单类型的, 并开启的
                                    List<Printer> printers = Resources.GetRes().PRINTERS.Where(x => x.PrinterId == ppr.PrinterId && x.IsEnable == 1 && (x.PrintType == 0 || x.PrintType == 2)).Distinct().ToList();

                                    if (null != printers)
                                    {
                                        foreach (var printer in printers)
                                        {
                                            if (printDetails.ContainsKey(printer))
                                            {
                                                printDetails[printer].Add(item);
                                            }
                                            else
                                            {
                                                printDetails.Add(printer, new List<ImportDetail>() { item });
                                            }
                                        }
                                    }
                                }
                            }


                            // 获取所有打印机中的开启的并打印所有的打印机
                            List<Printer> mainPrinters = Resources.GetRes().PRINTERS.Where(x => x.IsEnable == 1 && x.IsMain == 1 && (x.PrintType == 0 || x.PrintType == 2)).Distinct().ToList();
                            if (null != mainPrinters)
                            {
                                foreach (var printer in mainPrinters)
                                {
                                    if (printDetails.ContainsKey(printer))
                                    {
                                        if (!printDetails[printer].Contains(item))
                                            printDetails[printer].Add(item);
                                    }
                                    else
                                    {
                                        printDetails.Add(printer, new List<ImportDetail>() { item });
                                    }

                                }
                            }
                        }

                        // 打印客户账单
                        // 开始循环打印客户账单部分(因为结账, 所以所有打印机都打印所有内容)
                        foreach (var item in printDetails)
                        {
                            try
                            {
                                object report = null;

                                if (item.Key.PrintType == 0)
                                    report = new ImportReport(import, Resources.GetRes().PRODUCTS, item.Key, Resources.GetRes().PrintInfo.PriceSymbol, Lang);
                                else if (item.Key.PrintType == 2)
                                    report = new ImportMiddleReport(import, Resources.GetRes().PRODUCTS, item.Key, Resources.GetRes().PrintInfo.PageHeight, Resources.GetRes().PrintInfo.PriceSymbol, Lang);

                                ReportModel reportModel = new ReportModel();

                                reportModel.DataSource = importDetails;

                                // 获取打印机语言
                                long lang = item.Key.Lang;

                                // 覆盖打印语言
                                if (Lang != -1)
                                    lang = Lang;



                                Lang mainLang = ReportSettings(lang, reportModel, item.Key, true);
                                CultureInfo ci = mainLang.Culture;

                                // 更改参数
                                reportModel.Parameters.Add("ImportId", Resources.GetRes().GetString("ImportId", ci));
                                reportModel.Parameters.Add("ImportIdValue", string.Format(": {0}", import.ImportId.ToString()));

                                reportModel.Parameters.Add("Time", Resources.GetRes().GetString("Time", ci));
                                reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.ParseExact(import.ImportTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss")));
                                reportModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName", ci));
                                reportModel.Parameters.Add("Count", Resources.GetRes().GetString("Count", ci));
                                reportModel.Parameters.Add("Price", Resources.GetRes().GetString("Price", ci));
                                reportModel.Parameters.Add("ExpendBill", string.Format("({0})", Resources.GetRes().GetString("ExpendBill", ci)));


                                if (!string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Phone))
                                {
                                    reportModel.Parameters.Add("PhoneNo", Resources.GetRes().GetString("Phone", ci));
                                    reportModel.Parameters.Add("PhoneNoValue", string.Format("{0}", Resources.GetRes().PrintInfo.Phone));

                                    if (mainLang.MainLangIndex == 0 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg0))
                                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg0);
                                    else  if (mainLang.MainLangIndex == 1 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg1))
                                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg1);
                                    else if (mainLang.MainLangIndex == 2 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg2))
                                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg2);


                                    if (item.Key.PrintType == 2)
                                    {
                                        reportModel.PageHeight = Resources.GetRes().PrintInfo.PageHeight;
                                    }
                                }



                                // 价钱信息
                                if (import.TotalPrice != 0)
                                {
                                    reportModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice", ci));
                                    reportModel.Parameters.Add("TotalPriceValue", import.TotalPrice);
                                }
                                if (null != import.SupplierId && import.SupplierId != 0)
                                {
                                    Supplier supplier = Resources.GetRes().SUPPLIERS.Where(x => x.SupplierId == import.SupplierId).FirstOrDefault();
                                    if (null != supplier)
                                    {
                                        reportModel.Parameters.Add("SupplierId", Resources.GetRes().GetString("SupplierNo", ci));
                                        reportModel.Parameters.Add("SupplierIdValue", supplier.SupplierNo);
                                    }
                                }
                                if (import.SupplierPaidPrice != 0)
                                {
                                    reportModel.Parameters.Add("SupplierPaidPrice", Resources.GetRes().GetString("SupplierPaidPrice", ci));
                                    reportModel.Parameters.Add("SupplierPaidPriceValue", import.SupplierPaidPrice);
                                }
                                if (import.PaidPrice != 0)
                                {
                                    reportModel.Parameters.Add("PaidPrice", Resources.GetRes().GetString("PaidPrice", ci));
                                    reportModel.Parameters.Add("PaidPriceValue", import.PaidPrice);
                                }
                                
                                int payWay = 0;
                                if (import.PaidPrice > 0)
                                    ++payWay;
                                
                                if (import.SupplierPaidPrice > 0)
                                    ++payWay;

                                if (import.TotalPaidPrice != 0 && payWay > 1)
                                {
                                    reportModel.Parameters.Add("TotalPaidPrice", Resources.GetRes().GetString("TotalPaidPrice", ci));
                                    reportModel.Parameters.Add("TotalPaidPriceValue", import.TotalPaidPrice);
                                }
                                
                                if (import.BorrowPrice != 0)
                                {
                                    reportModel.Parameters.Add("BorrowPrice", Resources.GetRes().GetString("BorrowPrice", ci));
                                    reportModel.Parameters.Add("BorrowPriceValue", import.BorrowPrice);
                                }

                                ReportProcess.Instance.PrintReport(report, reportModel, item.Key.PrinterDeviceName);
                             

                                // 打印完毕后记录一下最后一次使用的可用打印机
                                LastOrderPrinter = item.Key;
                            }
                            catch (Exception ex)
                            {
                                ExceptionPro.ExpLog(ex);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                });
            }
        }







        /// <summary>
        /// 统计打印
        /// </summary>
        /// <param name="client"></param>
        /// <param name="Package"></param>
        public void PrintSummary(Client client, SummaryModelPackage Package, long Lang = -1)
        {
            // 如果是本地打印账单才继续
            if (null == client || !client.IsLocalPrintCustomOrder)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {

                        // 存储打印-属于该打印机打印的客户订单详情
                        List<Printer> printDetails = new List<Printer>();


                        // 获取所有打印机中的开启的并打印所有的打印机
                        List<Printer> mainPrinters = Resources.GetRes().PRINTERS.Where(x => x.IsEnable == 1 && (x.PrintType == 0 || x.PrintType == 2)).Distinct().ToList();
                        if (null != mainPrinters)
                        {
                            foreach (var printer in mainPrinters)
                            {
                                printDetails.Add(printer);
                            }
                        }
                        

                        // 打印客户账单
                        // 开始循环打印客户账单部分(因为结账, 所以所有打印机都打印所有内容)
                        foreach (var item in printDetails)
                        {
                            try
                            {
                                object report = null;

                                if (item.PrintType == 0)
                                    report = new SummaryReport(Resources.GetRes().PrintInfo.PriceSymbol);
                                else if (item.PrintType == 2)
                                    continue;

                                // 获取打印机语言
                                long lang = Lang;


                                ReportModel reportModel = new ReportModel();

                                Lang mainLang = ReportSettings(lang, reportModel, item, true);
                                CultureInfo ci = mainLang.Culture;

                                reportModel.Parameters.Add("TypeName", Resources.GetRes().GetString("TypeName", ci));
                                reportModel.Parameters.Add("Income", Resources.GetRes().GetString("Income", ci));
                                reportModel.Parameters.Add("Spend", Resources.GetRes().GetString("Expenditure", ci));
                                reportModel.Parameters.Add("Profit", Resources.GetRes().GetString("Profit", ci));
                                reportModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice", ci));


                                //reportModel.Parameters.Add("UnrecordPrice", Resources.GetRes().GetString("UnrecordedPrice", ci));
                                //reportModel.Parameters.Add("UnrecordTotalPrice", Resources.GetRes().GetString("TotalPrice", ci));
                                //reportModel.Parameters.Add("UnrecordTotalPriceValue", Resources.GetRes().ROOMS_Model.Where(x => null != x.PayOrder).Sum(x => x.PayOrder.TotalPrice));
                                //reportModel.Parameters.Add("UnrecordReceivePrice", Resources.GetRes().GetString("ReceivedPrice", ci));
                                //reportModel.Parameters.Add("UnrecordReceivePriceValue", Resources.GetRes().ROOMS_Model.Where(x => null != x.PayOrder).Sum(x => x.PayOrder.TotalPaidPrice));

                                reportModel.Parameters.Add("Time", Package.Time);

                                foreach (var item2 in Package.Records)
                                {
                                    if (!string.IsNullOrWhiteSpace(item2.Name))
                                        item2.TypeName = Resources.GetRes().GetString(item2.Name, ci);
                                }

                                foreach (var item2 in Package.Records2)
                                {
                                    if (!string.IsNullOrWhiteSpace(item2.Name))
                                        item2.TypeName = Resources.GetRes().GetString(item2.Name, ci);
                                }


                                //report.DataSource = records;
                                reportModel.DetailReport = Package.Records;



                                reportModel.DetailReport2 = Package.Records2;


                                ReportProcess.Instance.PrintReport(report, reportModel, item.PrinterDeviceName);

                                // 打印完毕后记录一下最后一次使用的可用打印机
                                LastOrderPrinter = item;
                            }
                            catch (Exception ex)
                            {
                                ExceptionPro.ExpLog(ex);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex);
                    }
                });
            }
        }



        private Printer LastOrderPrinter = null;
        /// <summary>
        /// 购买时打印订单(这个有一个问题就是一开始的打印价格用的逻辑, 因为上次查找到的逻辑都是获取打印所有的打印机, 其实打印时间用一个开启过的打印机也够了)
        /// </summary>
        /// <param name="printDetails"></param>
        /// <param name="order"></param>
        /// <param name="oldOrder"></param>
        private void PrintCustomerOrderAfterBuy(Dictionary<Printer, List<OrderDetail>> printDetails, Order order, Order oldOrder)
        {
            Printer lastPrinter = null;
            bool IsSuccessProcessRoomPrice = true; // 是否成功处理了包厢费

            // 老订单存在, 同时新订单包厢费比老订单多了时
            if ((null != oldOrder && order.RoomPrice > oldOrder.RoomPrice && null != order.EndTime && null != oldOrder.EndTime) || (null == oldOrder && order.RoomPrice > 0))
            {
                IsSuccessProcessRoomPrice = false;

                // 获取最后一个打印机
                if (null != printDetails && printDetails.Count > 0)
                {
                    lastPrinter = printDetails.LastOrDefault().Key;
                }
                else
                {
                    // 没获取到, 则从上次打印机中获取
                    if (null != LastOrderPrinter)
                    {
                        lastPrinter = LastOrderPrinter;
                    }
                    else
                    {
                        // 如果还是没有, 则从打印机队列中获取客户订单用的打印机同名打印机并可用的打印机.
                        lastPrinter = GetAvailablePrinter();

                        // 如果依然没有, 则记录日志.
                        if (null == lastPrinter)
                        {
                            ExceptionPro.ExpErrorLog("No available printer for print");
                            return;
                        }
                    }
                }
            }


            // 打印
            foreach (var item in printDetails)
            {
                PrintOrderAfterByProcess(item.Key, item.Value, order, oldOrder, lastPrinter, ref IsSuccessProcessRoomPrice);
            }

            if (!IsSuccessProcessRoomPrice && null != printDetails && printDetails.Count == 0)
            {
                PrintOrderAfterByProcess(lastPrinter, null, order, oldOrder, lastPrinter, ref IsSuccessProcessRoomPrice);
            }
        }





        /// <summary>
        /// 结账时打印订单
        /// </summary>
        /// <param name="printDetails"></param>
        /// <param name="order"></param>
        /// <param name="Lang"></param>
        private void PrintCustomerOrderAfterCheckout(Dictionary<Printer, List<OrderDetail>> printDetails, Order order, long Lang)
        {
            Printer lastPrinter = null;
            bool IsSuccessProcessRoomPrice = true; // 是否成功处理了包厢费

            // 如果订单价格有增多
            if (order.RoomPrice > 0)
            {
                IsSuccessProcessRoomPrice = false;

                // 获取最后一个打印机
                if (null != printDetails && printDetails.Count > 0)
                {
                    lastPrinter = printDetails.LastOrDefault().Key;
                }
                else
                {
                    // 没获取到, 则从上次打印机中获取
                    if (null != LastOrderPrinter)
                    {
                        lastPrinter = LastOrderPrinter;
                    }
                    else
                    {
                        // 如果还是没有, 则从打印机队列中获取客户订单用的打印机同名打印机并可用的打印机.
                        lastPrinter = GetAvailablePrinter();

                        // 如果依然没有, 则记录日志.
                        if (null == lastPrinter)
                        {
                            ExceptionPro.ExpErrorLog("No available printer for print");
                            return;
                        }
                    }
                }
            }


            // 打印
            foreach (var item in printDetails)
            {
                PrintOrderAfterCheckoutProcess(item.Key, item.Value, order, lastPrinter, Lang, ref IsSuccessProcessRoomPrice);
            }

            if (!IsSuccessProcessRoomPrice && null != printDetails && printDetails.Count == 0)
            {
                PrintOrderAfterCheckoutProcess(lastPrinter, null, order, lastPrinter, Lang, ref IsSuccessProcessRoomPrice);
            }
        }





        /// <summary>
        /// 结账时打印订单
        /// </summary>
        /// <param name="printDetails"></param>
        /// <param name="order"></param>
        /// <param name="Lang"></param>
        private void PrintCustomerTakeoutAfterCheckout(Dictionary<Printer, List<TakeoutDetail>> printDetails, Takeout takeout, long Lang)
        {



            // 打印
            foreach (var item in printDetails)
            {
                try
                {
                    object report = null;

                    if (item.Key.PrintType == 0)
                        report = new TakeoutCheckoutReport(takeout, Resources.GetRes().PRODUCTS, item.Key, Resources.GetRes().PrintInfo.PriceSymbol, Lang);
                    else if (item.Key.PrintType == 2)
                        report = new TakeoutCheckoutMiddleReport(takeout, Resources.GetRes().PRODUCTS, item.Key, Resources.GetRes().PrintInfo.PageHeight, Resources.GetRes().PrintInfo.PriceSymbol, Lang);


                    ReportModel reportModel = new ReportModel();

                    reportModel.DataSource = item.Value == null ? new List<TakeoutDetail>() : item.Value.Where(x => x.State != 3).ToList();

                    // 获取打印机语言
                    long lang = item.Key.Lang;

                    // 如果打印机语言是根据订单的则用订单的
                    if (-1 == lang)
                    {
                        lang = takeout.Lang;
                    }

                    // 覆盖打印语言
                    if (Lang != -1)
                        lang = Lang;


                    string PersonName = "";
                    string PersonAddress = "";


                    Lang mainLang = ReportSettings(lang, reportModel, item.Key, true);
                    CultureInfo ci = mainLang.Culture;

                    // 维文
                    if (mainLang.MainLangIndex == 0)
                    {
                        PersonName = takeout.Name0;
                        PersonAddress = takeout.Address0;
                    }
                    // 中文
                    else if (mainLang.MainLangIndex == 1)
                    {
                       
                        PersonName = takeout.Name1;
                        PersonAddress = takeout.Address1;
                    }

                    // 英文
                    else if (mainLang.MainLangIndex == 2)
                    {
                        PersonName = takeout.Name2;
                        PersonAddress = takeout.Address2;
                    }

                    // 更改参数
                    reportModel.Parameters.Add("OrderId", Resources.GetRes().GetString("OrderNo", ci));
                    reportModel.Parameters.Add("OrderIdValue", string.Format(": {0}", takeout.TakeoutId.ToString()));

                    reportModel.Parameters.Add("Time", Resources.GetRes().GetString("Time", ci));
                    reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.ParseExact(takeout.AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss")));
                    reportModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName", ci));
                    reportModel.Parameters.Add("Count", Resources.GetRes().GetString("Count", ci));
                    reportModel.Parameters.Add("Price", Resources.GetRes().GetString("Price", ci));


                    if (!string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Phone))
                    {
                        reportModel.Parameters.Add("PhoneNo", Resources.GetRes().GetString("Phone", ci));
                        reportModel.Parameters.Add("PhoneNoValue", string.Format("{0}", Resources.GetRes().PrintInfo.Phone));

                        if (mainLang.MainLangIndex == 0 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg0))
                            reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg0);
                        else if (mainLang.MainLangIndex == 1 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg1))
                            reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg1);
                        else if (mainLang.MainLangIndex == 2 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg2))
                            reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg2);


                        if (item.Key.PrintType == 2)
                        {
                            reportModel.PageHeight = Resources.GetRes().PrintInfo.PageHeight;
                        }
                    }



                    if (!string.IsNullOrEmpty(takeout.Phone))
                    {
                        reportModel.Parameters.Add("PersonPhone", Resources.GetRes().GetString("Phone", ci));
                        reportModel.Parameters.Add("PersonPhoneValue", string.Format(": {0}", takeout.Phone));
                    }

                    if (!string.IsNullOrEmpty(PersonName))
                    {
                        reportModel.Parameters.Add("PersonName", Resources.GetRes().GetString("PersonName", ci));
                        reportModel.Parameters.Add("PersonNameValue", string.Format(": {0}", PersonName));
                    }

                    if (!string.IsNullOrEmpty(PersonAddress))
                    {
                        reportModel.Parameters.Add("PersonAddress", Resources.GetRes().GetString("Address", ci));
                        reportModel.Parameters.Add("PersonAddressSperator", ": ");
                        reportModel.Parameters.Add("PersonAddressValue", string.Format("{0}", PersonAddress));
                    }



                    // 价钱信息
                    if (takeout.TotalPrice != 0)
                    {
                        reportModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice", ci));
                        reportModel.Parameters.Add("TotalPriceValue", takeout.TotalPrice);
                    }
                    if (null != takeout.MemberId && takeout.MemberId != 0)
                    {
                        Member member = Resources.GetRes().MEMBERS.Where(x => x.MemberId == takeout.MemberId).FirstOrDefault();
                        if (null != member)
                        {
                            reportModel.Parameters.Add("MemberId", Resources.GetRes().GetString("MemberNo", ci));
                            reportModel.Parameters.Add("MemberIdValue", member.MemberNo);
                        }
                    }
                    
                    if (takeout.MemberPaidPrice != 0)
                    {
                        reportModel.Parameters.Add("MemberPaidPrice", Resources.GetRes().GetString("MemberPaidPrice", ci));
                        reportModel.Parameters.Add("MemberPaidPriceValue", takeout.MemberPaidPrice);
                    }
                    if (takeout.PaidPrice != 0)
                    {
                        reportModel.Parameters.Add("PaidPrice", Resources.GetRes().GetString("PaidPrice", ci));
                        reportModel.Parameters.Add("PaidPriceValue", takeout.PaidPrice);
                    }
                   

                    int payWay = 0;
                    if (takeout.PaidPrice > 0)
                        ++payWay;
                    
                    if (takeout.MemberPaidPrice > 0)
                        ++payWay;

                    if (takeout.TotalPaidPrice != 0 && payWay > 1)
                    {
                        reportModel.Parameters.Add("TotalPaidPrice", Resources.GetRes().GetString("TotalPaidPrice", ci));
                        reportModel.Parameters.Add("TotalPaidPriceValue", takeout.TotalPaidPrice);
                    }
                   
                    if (takeout.BorrowPrice != 0)
                    {
                        reportModel.Parameters.Add("BorrowPrice", Resources.GetRes().GetString("OwedPrice", ci));
                        reportModel.Parameters.Add("BorrowPriceValue", takeout.BorrowPrice);
                    }





                    ReportProcess.Instance.PrintReport(report, reportModel, item.Key.PrinterDeviceName);

                    // 打印完毕后记录一下最后一次使用的可用打印机
                    LastOrderPrinter = item.Key;
                }
                catch (Exception ex)
                {
                    ExceptionPro.ExpLog(ex);
                }
            }


        }



        /// <summary>
        /// 后台打印
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="items"></param>
        /// <param name="order"></param>
        private void PrintOrderToBackstageProcess(Printer printer, List<OrderDetail> items, Order order)
        {
            try
            {
                object report = new BackstageReport(order, Resources.GetRes().PRODUCTS, printer, Resources.GetRes().REQUESTS);
                ReportModel reportModel = new ReportModel();
                reportModel.DataSource = items;

                // 获取打印机语言
                long lang = printer.Lang;

                // 如果打印机语言是根据订单的则用订单的
                if (-1 == lang)
                {
                    lang = order.Lang;
                }




                Lang mainLang = ReportSettings(lang, reportModel, printer);
                CultureInfo ci = mainLang.Culture;

                // 更改参数
                reportModel.Parameters.Add("OrderId", Resources.GetRes().GetString("OrderNo", ci));
                reportModel.Parameters.Add("OrderIdValue", string.Format(": {0}", order.OrderId.ToString()));
                reportModel.Parameters.Add("RoomNo", Resources.GetRes().GetString("RoomNo", ci));
                reportModel.Parameters.Add("RoomNoValue", string.Format(": {0}", Resources.GetRes().ROOMS.Where(x => x.RoomId == order.RoomId).FirstOrDefault().RoomNo));
                reportModel.Parameters.Add("Time", Resources.GetRes().GetString("Time", ci));
                reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                reportModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName", ci));
                reportModel.Parameters.Add("Count", Resources.GetRes().GetString("Count", ci));
                if (null != items && items.Count > 0 && items.Where(x => x.IsPack == 1).Count() > 0)
                {
                    reportModel.Parameters.Add("Package", Resources.GetRes().GetString("Package", ci));
                }


                ReportProcess.Instance.PrintReport(report, reportModel, printer.PrinterDeviceName);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }









        /// <summary>
        /// 后台打印
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="items"></param>
        /// <param name="order"></param>
        private void PrintOrderToBackstageProcess(Printer printer, List<TakeoutDetail> items, Takeout takeout)
        {
            try
            {
                object report = new BackstageTakeoutReport(takeout, Resources.GetRes().PRODUCTS, printer, Resources.GetRes().REQUESTS);
                ReportModel reportModel = new ReportModel();
                reportModel.DataSource = items;

                // 获取打印机语言
                long lang = printer.Lang;

                // 如果打印机语言是根据订单的则用订单的
                if (-1 == lang)
                {
                    lang = takeout.Lang;
                }


                Lang mainLang = ReportSettings(lang, reportModel, printer);
                CultureInfo ci = mainLang.Culture;

                // 更改参数
                reportModel.Parameters.Add("OrderId", Resources.GetRes().GetString("OrderNo", ci));
                reportModel.Parameters.Add("OrderIdValue", string.Format(": {0}", takeout.TakeoutId.ToString()));

                reportModel.Parameters.Add("Time", Resources.GetRes().GetString("Time", ci));
                reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                reportModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName", ci));
                reportModel.Parameters.Add("Count", Resources.GetRes().GetString("Count", ci));
                if (null != items && items.Count > 0 && items.Where(x => x.IsPack == 1).Count() > 0)
                {
                    reportModel.Parameters.Add("Package", Resources.GetRes().GetString("Package", ci));
                }


                ReportProcess.Instance.PrintReport(report, reportModel, printer.PrinterDeviceName);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }

        /// <summary>
        /// 打印处理购买后的订单
        /// </summary>
        /// <param name="item"></param>
        /// <param name="order"></param>
        /// <param name="oldOrder"></param>
        /// <param name="lastPrinter"></param>
        /// <param name="IsSuccessProcessRoomPrice"></param>
        private void PrintOrderAfterByProcess(Printer printer, List<OrderDetail> items, Order order, Order oldOrder, Printer lastPrinter, ref bool IsSuccessProcessRoomPrice)
        {
            try
            {
                object report = null;

                if (printer.PrintType == 0)
                    report = new OrderPayReport(order, Resources.GetRes().PRODUCTS, printer, Resources.GetRes().PrintInfo.PriceSymbol);
                else if (printer.PrintType == 2)
                    report = new OrderCheckoutMiddleReport(order, Resources.GetRes().PRODUCTS, printer, Resources.GetRes().PrintInfo.PageHeight, Resources.GetRes().PrintInfo.PriceSymbol);

                ReportModel reportModel = new ReportModel();

                reportModel.DataSource = items;

                // 获取打印机语言
                long lang = printer.Lang;

                // 如果打印机语言是根据订单的则用订单的
                if (-1 == lang)
                {
                    lang = order.Lang;
                }



                Lang mainLang = ReportSettings(lang, reportModel, printer, true);
                CultureInfo ci = mainLang.Culture;

                // 更改参数
                reportModel.Parameters.Add("OrderId", Resources.GetRes().GetString("OrderNo", ci));
                reportModel.Parameters.Add("OrderIdValue", string.Format(": {0}", order.OrderId.ToString()));
                reportModel.Parameters.Add("RoomNo", Resources.GetRes().GetString("RoomNo", ci));
                reportModel.Parameters.Add("RoomNoValue", string.Format(": {0}", Resources.GetRes().ROOMS.Where(x => x.RoomId == order.RoomId).FirstOrDefault().RoomNo));
                reportModel.Parameters.Add("Time", Resources.GetRes().GetString("Time", ci));
                reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                reportModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName", ci));
                reportModel.Parameters.Add("Count", Resources.GetRes().GetString("Count", ci));
                reportModel.Parameters.Add("Price", Resources.GetRes().GetString("Price", ci));
                reportModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice", ci));


                if (!string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Phone))
                {
                    reportModel.Parameters.Add("PhoneNo", Resources.GetRes().GetString("Phone", ci));
                    reportModel.Parameters.Add("PhoneNoValue", string.Format("{0}", Resources.GetRes().PrintInfo.Phone));

                    if (mainLang.MainLangIndex == 0 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg0))
                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg0);
                    else if (mainLang.MainLangIndex == 1 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg1))
                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg1);
                    else if (mainLang.MainLangIndex == 2 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg2))
                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg2);


                    if (printer.PrintType == 2)
                    {
                        reportModel.PageHeight = Resources.GetRes().PrintInfo.PageHeight;
                    }
                }

                Room room = Resources.GetRes().ROOMS.Where(x => x.RoomId == order.RoomId).FirstOrDefault();

                //// 写入打印信息


                // 第二次打印
                if (null != oldOrder)
                {
                    // 价格有变化就写, 没有就不写
                    if (order.RoomPrice - oldOrder.RoomPrice > 0)
                    {
                        reportModel.Parameters.Add("RoomPrice", Resources.GetRes().GetString("RoomPrice", ci));
                        if (null != order.StartTime && null != order.EndTime)
                        {
                            DateTime oldOrderEndTime = DateTime.ParseExact(oldOrder.EndTime.ToString(), "yyyyMMddHHmmss", null);
                            DateTime orderEndTime = DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null);

                            TimeSpan leftTime = (orderEndTime - oldOrderEndTime);


                            if (room.IsPayByTime == 1)
                            {
                                reportModel.Parameters.Add("RoomTime", string.Format("+{0}:{1}", (int)leftTime.TotalHours, leftTime.Minutes));
                            }
                            else if (room.IsPayByTime == 2)
                            {
                                reportModel.Parameters.Add("RoomTime", string.Format("+{0}/{1}", (int)leftTime.TotalDays, leftTime.Hours));
                            }
                        }

                        double roomPrice = order.RoomPrice - oldOrder.RoomPrice;
                        reportModel.Parameters.Add("RoomPriceValue", roomPrice);

                        if (null != items && items.Count > 0)
                            reportModel.Parameters.Add("TotalPriceValue", Math.Round((items.Sum(x => x.TotalPrice)) + roomPrice, 2));
                        else
                            reportModel.Parameters.Add("TotalPriceValue", roomPrice);
                    }
                    else
                    {
                        if (null != items && items.Count > 0)
                            reportModel.Parameters.Add("TotalPriceValue", Math.Round((items.Sum(x => x.TotalPrice)), 2));
                    }
                }
                //  首次打印
                else
                {
                    reportModel.Parameters.Add("RoomPrice", Resources.GetRes().GetString("RoomPrice", ci));
                    if (null != order.StartTime && null != order.EndTime)
                    {
                        DateTime orderStartTime = DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null);
                        DateTime orderEndTime = DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null);

                        TimeSpan leftTime = (orderEndTime - orderStartTime);

                        if (room.IsPayByTime == 1)
                        {
                            reportModel.Parameters.Add("RoomTime", string.Format("+{0}:{1}", (int)leftTime.TotalHours, leftTime.Minutes));
                        }
                        else if (room.IsPayByTime == 2)
                        {
                            reportModel.Parameters.Add("RoomTime", string.Format("+{0}/{1}", (int)leftTime.TotalDays, leftTime.Hours));
                        }
                    }
                    reportModel.Parameters.Add("RoomPriceValue", order.RoomPrice);

                    if (null != items && items.Count > 0)
                        reportModel.Parameters.Add("TotalPriceValue", Math.Round((items.Sum(x => x.TotalPrice)) + order.RoomPrice, 2));
                    else
                        reportModel.Parameters.Add("TotalPriceValue", order.RoomPrice);
                }

                IsSuccessProcessRoomPrice = true;


                ReportProcess.Instance.PrintReport(report, reportModel, printer.PrinterDeviceName);

                // 打印完毕后记录一下最后一次使用的可用打印机
                LastOrderPrinter = printer;
            }
            catch (Exception ex)
            {

                ExceptionPro.ExpLog(ex);
            }
        }


        /// <summary>
        /// 打印处理结账后的订单
        /// </summary>
        /// <param name="printer"></param>
        /// <param name="items"></param>
        /// <param name="order"></param>
        /// <param name="lastPrinter"></param>
        /// <param name="Lang"></param>
        /// <param name="IsSuccessProcessRoomPrice"></param>
        private void PrintOrderAfterCheckoutProcess(Printer printer, List<OrderDetail> items, Order order, Printer lastPrinter, long Lang, ref bool IsSuccessProcessRoomPrice)
        {

            try
            {
                object report = null;

                if (printer.PrintType == 0)
                    report = new OrderCheckoutReport(order, Resources.GetRes().PRODUCTS, printer, Resources.GetRes().PrintInfo.PriceSymbol, Lang);
                else if (printer.PrintType == 2)
                    report = new OrderCheckoutMiddleReport(order, Resources.GetRes().PRODUCTS, printer, Resources.GetRes().PrintInfo.PageHeight, Resources.GetRes().PrintInfo.PriceSymbol);

                ReportModel reportModel = new ReportModel();

                reportModel.DataSource = items == null ? new List<OrderDetail>() : items.Where(x => x.State != 3).ToList();

                // 获取打印机语言
                long lang = printer.Lang;

                // 如果打印机语言是根据订单的则用订单的
                if (-1 == lang)
                {
                    lang = order.Lang;
                }

                // 覆盖打印语言
                if (Lang != -1)
                    lang = Lang;


                Lang mainLang = ReportSettings(lang, reportModel, printer, true);
                CultureInfo ci = mainLang.Culture;



                // 更改参数
                reportModel.Parameters.Add("OrderId", Resources.GetRes().GetString("OrderNo", ci));
                reportModel.Parameters.Add("OrderIdValue", string.Format(": {0}", order.OrderId.ToString()));
                reportModel.Parameters.Add("RoomNo", Resources.GetRes().GetString("RoomNo", ci));
                reportModel.Parameters.Add("RoomNoValue", string.Format(": {0}", Resources.GetRes().ROOMS.Where(x => x.RoomId == order.RoomId).FirstOrDefault().RoomNo));
                reportModel.Parameters.Add("Time", Resources.GetRes().GetString("Time", ci));
                if (order.FinishTime == null)
                    reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                else
                    reportModel.Parameters.Add("TimeValue", string.Format(": {0}", DateTime.ParseExact(order.FinishTime.Value.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss")));
                reportModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName", ci));
                reportModel.Parameters.Add("Count", Resources.GetRes().GetString("Count", ci));
                reportModel.Parameters.Add("Price", Resources.GetRes().GetString("Price", ci));


                if (!string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Phone))
                {
                    reportModel.Parameters.Add("PhoneNo", Resources.GetRes().GetString("Phone", ci));
                    reportModel.Parameters.Add("PhoneNoValue", string.Format("{0}", Resources.GetRes().PrintInfo.Phone));

                    if (mainLang.MainLangIndex == 0 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg0))
                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg0);
                    else if (mainLang.MainLangIndex == 1 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg1))
                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg1);
                    else if (mainLang.MainLangIndex == 2 && !string.IsNullOrEmpty(Resources.GetRes().PrintInfo.Msg2))
                        reportModel.Parameters.Add("PrintMessage", Resources.GetRes().PrintInfo.Msg2);


                    if (printer.PrintType == 2)
                    {
                        reportModel.PageHeight = Resources.GetRes().PrintInfo.PageHeight;
                    }
                }

                // 写入打印信息
                if (order.RoomPrice > 0)
                {
                    reportModel.Parameters.Add("RoomPrice", Resources.GetRes().GetString("RoomPrice", ci));
                    reportModel.Parameters.Add("RoomPriceValue", order.RoomPrice);

                    if (null != order.StartTime && null != order.EndTime)
                    {
                        DateTime orderEndTime = DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null);
                        DateTime orderStartTime = DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null);

                        TimeSpan leftTime = (orderEndTime - orderStartTime);

                        Room room = Resources.GetRes().ROOMS.Where(x => x.RoomId == order.RoomId).FirstOrDefault();
                        if (room.IsPayByTime == 1)
                        {
                            reportModel.Parameters.Add("RoomTime", string.Format("+{0}:{1}", (int)leftTime.TotalHours, leftTime.Minutes));
                        }
                        else if (room.IsPayByTime == 2)
                        {
                            reportModel.Parameters.Add("RoomTime", string.Format("+{0}/{1}", (int)leftTime.TotalDays, leftTime.Hours));
                        }
                    }
                }



                // 价钱信息
                if (order.TotalPrice != 0)
                {
                    reportModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice", ci));
                    reportModel.Parameters.Add("TotalPriceValue", order.TotalPrice);
                }
                if (null != order.MemberId && order.MemberId != 0)
                {
                    Member member = Resources.GetRes().MEMBERS.Where(x => x.MemberId == order.MemberId).FirstOrDefault();
                    if (null != member)
                    {
                        reportModel.Parameters.Add("MemberId", Resources.GetRes().GetString("MemberNo", ci));
                        reportModel.Parameters.Add("MemberIdValue", member.MemberNo);
                    }
                }
               
                if (order.MemberPaidPrice != 0)
                {
                    reportModel.Parameters.Add("MemberPaidPrice", Resources.GetRes().GetString("MemberPaidPrice", ci));
                    reportModel.Parameters.Add("MemberPaidPriceValue", order.MemberPaidPrice);
                }
                if (order.PaidPrice != 0)
                {
                    reportModel.Parameters.Add("PaidPrice", Resources.GetRes().GetString("PaidPrice", ci));
                    reportModel.Parameters.Add("PaidPriceValue", order.PaidPrice);
                }
                
                int payWay = 0;
                if (order.PaidPrice > 0)
                    ++payWay;
               
                if (order.MemberPaidPrice > 0)
                    ++payWay;

                if (order.TotalPaidPrice != 0 && payWay > 1)
                {
                    reportModel.Parameters.Add("TotalPaidPrice", Resources.GetRes().GetString("TotalPaidPrice", ci));
                    reportModel.Parameters.Add("TotalPaidPriceValue", order.TotalPaidPrice);
                }
                
                if (order.BorrowPrice != 0)
                {
                    reportModel.Parameters.Add("BorrowPrice", Resources.GetRes().GetString("OwedPrice", ci));
                    reportModel.Parameters.Add("BorrowPriceValue", order.BorrowPrice);
                }





                ReportProcess.Instance.PrintReport(report, reportModel, printer.PrinterDeviceName);

                // 打印完毕后记录一下最后一次使用的可用打印机
                LastOrderPrinter = printer;
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

        }

        /// <summary>
        /// 获取可用打印机
        /// </summary>
        /// <returns></returns>
        private Printer GetAvailablePrinter()
        {
            // 获取当前连接的所有打印机
            foreach (string printerName in PrinterSettings.InstalledPrinters)
            {

                try
                {
                    // 查询打印机是否可用
                    PrinterSettings printerSetting = new PrinterSettings();
                    printerSetting.PrinterName = printerName;

                    if (printerSetting.IsValid)
                    {
                        Printer printer = Resources.GetRes().PRINTERS.Where(x => x.PrinterDeviceName == printerName && x.IsEnable == 1 && ( x.PrintType == 0  || x.PrintType == 2)).FirstOrDefault();
                        if (null != printer)
                            return printer;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return null;
        }




        /// <summary>
        /// 返回语言
        /// </summary>
        /// <param name="Lang"></param>
        /// <param name="report"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private Lang ReportSettings(long lang, ReportModel report, Printer item, bool IsMiddle = false)
        {

            Lang mainLang = null;

            // 0
            if (lang != -1)
            {
                mainLang = Resources.GetRes().GetMainLangByLangIndex((int)lang);

                if (mainLang.MainLangIndex == 0)
                {
                    // 更改参数
                    report.Parameters.Add("CompanyName",Resources.GetRes().KEY_NAME_0);
                }
                // 1
                else if (mainLang.MainLangIndex == 1)
                {
                    report.Parameters.Add("CompanyName", Resources.GetRes().KEY_NAME_1);
                }

                // 2
                else if (mainLang.MainLangIndex == 2)
                {
                    report.Parameters.Add("CompanyName", Resources.GetRes().KEY_NAME_2);
                }
            }

            if (Resources.GetRes().GetString("LargeFont", mainLang.Culture) == "1")
            {
                report.Fonts.Add("xrcsFont10", new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 10));

                if (null != item && item.PrintType == 2 && IsMiddle)
                {
                    report.Fonts.Add("xrcsFont9", new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 11));
                    report.Fonts.Add("xrcsFont8",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 10));
                }
                else
                {
                    report.Fonts.Add("xrcsFont9",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 9));
                    report.Fonts.Add("xrcsFont8",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 8));
                }
                report.Fonts.Add("xrcsFont7",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 7));
                report.Fonts.Add("xrcsFont6",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 6));
                report.Fonts.Add("xrcsFont5",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 5));
                report.Fonts.Add("xrcsFont4",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 4));
            }
            else
            {
                report.Fonts.Add("xrcsFont10",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 10));
                report.Fonts.Add("xrcsFont9",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 9));
                report.Fonts.Add("xrcsFont8",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 8));
                report.Fonts.Add("xrcsFont7",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 7));
                report.Fonts.Add("xrcsFont6",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 6));
                report.Fonts.Add("xrcsFont5",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 5));
                report.Fonts.Add("xrcsFont4",new System.Drawing.Font(Resources.GetRes().GetString("FontName", mainLang.Culture).Split(',')[0], 4));
            }

            return mainLang;

        }


    }



}
