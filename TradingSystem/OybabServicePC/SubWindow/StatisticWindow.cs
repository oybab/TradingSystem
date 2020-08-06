using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Report.Model;
using Oybab.ServicePC.DialogWindow;
using Oybab.Res.View.Models;
using System.Globalization;
using Oybab.Report.StatisticsForm;

namespace Oybab.ServicePC.SubWindow
{
    internal sealed partial class StatisticWindow : KryptonForm
    {
        //页数变量
        private TimeSpan TimeLimit = TimeSpan.FromDays(Resources.GetRes().ShorDay);

        public StatisticWindow()
        {
            InitializeComponent();
            this.ControlBox = false;


            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
           
            krplAddTime.Text = Resources.GetRes().GetString("AddTime");
            //krplEndTime.Text = Resources.GetRes().GetString("EndTime");

            krptbStartTime.Value =  DateTime.Now.AddDays(-1);

      

            krptbEndTime.Value = DateTime.Now;

            this.Text = Resources.GetRes().GetString("Statistic");


            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.ico"));

            krpbIncomeStatistic.Text = Resources.GetRes().GetString("IncomeStatistic");
            krpbSpendStatistic.Text = Resources.GetRes().GetString("SpendStatistic");

            krpbSellProductsStatistic.Text = Resources.GetRes().GetString("SellProductsStatistic");
            krpbSpendProductsStatistic.Text = Resources.GetRes().GetString("SpendProductsStatistic");

            krpbSalePaysStatistic.Text = Resources.GetRes().GetString("SalePaysStatistic");
            krpbSpendPaysStatistic.Text = Resources.GetRes().GetString("SpendPaysStatistic");

            krpbBalancePaysIncomeStatistic.Text = Resources.GetRes().GetString("BalancePaysIncomeStatistic");
            krpbBalancePaysSpendStatistic.Text = Resources.GetRes().GetString("BalancePaysSpendStatistic");

            krpbAdminSaleStatistic.Text = Resources.GetRes().GetString("AdminSaleStatistic");
            krpbProductProfitStatistic.Text = Resources.GetRes().GetString("ProductProfitStatistic");

            krpbIncomeTypeStatistic.Text = Resources.GetRes().GetString("IncomeTypeStatistic");
            krpbSpendTypeStatistic.Text = Resources.GetRes().GetString("SpendTypeStatistic");


            krpbSummary.Text = Resources.GetRes().GetString("Summary");





            krplOrderType.Text = Resources.GetRes().GetString("BillType");
            krpcbOrderType.Items.AddRange(new string[] { Resources.GetRes().GetString("BillTypeInner"), Resources.GetRes().GetString("BillTypeOuter") });

            if (Resources.GetRes().RoomCount <= 0)
            {
                krpcbOrderType.SelectedIndex = 1;
                krpcbOrderType.Visible = false;
                krplOrderType.Visible = false;
            }
            else
            {
                krpcbOrderType.SelectedIndex = 0;
            }


        }

        /// <summary>
        /// 检查并格式化时间
        /// </summary>
        /// <param name="addTimeFinal"></param>
        /// <param name="finishTimeFinal"></param>
        /// <returns></returns>
        private bool CheckTime(out long addTimeFinal, out long finishTimeFinal)
        {
            addTimeFinal = 0;
            finishTimeFinal = 0;

            DateTime startDateTime = DateTime.Now;
            DateTime endDateTime = DateTime.Now;

            try
            {
                startDateTime = krptbStartTime.Value;
                endDateTime = krptbEndTime.Value;
            }
            catch (Exception ex)
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("PropertyError"), Resources.GetRes().GetString("Time")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ExceptionPro.ExpLog(ex);
                return false ;
            }


            // 有包厢在用可以搜索7天内的. 没有则可以搜索35天内的
            if (Resources.GetRes().RoomsModel.Any(x => null != x.PayOrder))
                TimeLimit = TimeSpan.FromDays(Resources.GetRes().DefaultDay);
            else
                TimeLimit = TimeSpan.FromDays(Resources.GetRes().LongDay);

            if ((endDateTime - startDateTime).TotalMinutes <= 0 || !((endDateTime - startDateTime).TotalMinutes <= TimeLimit.TotalMinutes))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("TimeLimit"), TimeLimit.TotalDays), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            addTimeFinal = long.Parse(startDateTime.ToString("yyyyMMddHHmmss"));
            finishTimeFinal = long.Parse(endDateTime.ToString("yyyyMMddHHmmss"));

            return true;

        }


        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;



        /// <summary>
        /// 收入统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbImcomeStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            int OrderType = krpcbOrderType.SelectedIndex;

            bool result = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    

                    if (OrderType == 0)
                        result = OperatesService.GetOperates().ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, false, -1, -1, out orders);
                    else if (OrderType == 1)
                        result = OperatesService.GetOperates().ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, false, -1, -1, out takeouts);



                    if (result)
                    {
                        List<BillModel> resultList = new List<BillModel>();
                        if (OrderType == 0)
                            resultList  = orders.Select(x=> new BillModel(x)).OrderByDescending(x => x.Id).ToList();
                        else if (OrderType == 1)
                            resultList = takeouts.Select(x => new BillModel(x)).OrderByDescending(x => x.Id).ToList();


                        if (resultList.Count() > 0)
                        {
                            double TotalPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.TotalPrice), 2);
                            double MemberDealsPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.MemberDealsPrice), 2);
                            double DealsPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.DealsPrice), 2);
                            double ActualPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.ActualPrice), 2);
                            double MemberPaidPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.MemberPaidPrice), 2);
                            double PaidPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.PaidPrice), 2);
                            double CardPaidPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.CardPaidPrice), 2);
                            double TotalPaidPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.TotalPaidPrice), 2);
                            double ReturnPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.ReturnPrice), 2);
                            double BorrowPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.BorrowPrice), 2);
                            double KeepPrice = Math.Round(resultList.Where(x => x.State <= 1).Sum(x => x.KeepPrice), 2);



                            List<RecordChart> records = new List<RecordChart>();
                            int i = 0;
                            if (TotalPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("TotalPrice"), TotalPrice));
                            if (MemberDealsPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("MemberDealsPrice"), MemberDealsPrice));
                            if (DealsPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("DealsPrice"), DealsPrice));
                            if (ActualPrice != 0 && TotalPrice != ActualPrice)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("ActualPrice"), ActualPrice));
                            if (MemberPaidPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("MemberPaidPrice"), MemberPaidPrice));
                            if (PaidPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("PaidPrice"), PaidPrice));
                            if (CardPaidPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("CardPaidPrice"), CardPaidPrice));

                            int payWay = 0;
                            if (PaidPrice > 0)
                                ++payWay;
                            if (CardPaidPrice > 0)
                                ++payWay;
                            if (MemberPaidPrice > 0)
                                ++payWay;

                            if (TotalPaidPrice != 0 && payWay > 1)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("TotalPaidPrice"), TotalPaidPrice));
                            if (ReturnPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("ReturnPrice"), ReturnPrice));
                            if (BorrowPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("OwedPrice"), BorrowPrice));
                            if (KeepPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("KeepPrice"), KeepPrice));



                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbIncomeStatistic.Text);


                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);

                


            });
        }

        /// <summary>
        /// 支出统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSpendStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Import> imports = new List<Import>();

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    result = OperatesService.GetOperates().ServiceGetImports(addTimeFinal, finishTimeFinal, false, out imports);
                    if (result)
                    {
                        List<Import> resultList = imports.OrderByDescending(x => x.ImportId).ToList();


                        if (resultList.Count() > 0)
                        {

                            double TotalPrice = Math.Round(resultList.Sum(x => x.TotalPrice) ,2);
                            double SupplierPaidPrice = Math.Round(resultList.Sum(x => x.SupplierPaidPrice) ,2);
                            double PaidPrice = Math.Round(resultList.Sum(x => x.PaidPrice) ,2);
                            double TotalPaidPrice = Math.Round(resultList.Sum(x => x.TotalPaidPrice) ,2);
                            double BorrowPrice = Math.Round(resultList.Sum(x => x.BorrowPrice) ,2);
                            double KeepPrice = Math.Round(resultList.Sum(x => x.KeepPrice), 2);



                            List<RecordChart> records = new List<RecordChart>();
                            int i = 0;
                            if (TotalPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("TotalPrice"), TotalPrice));
                            if (SupplierPaidPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("SupplierPaidPrice"), SupplierPaidPrice));
                            if (PaidPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("PaidPrice"), PaidPrice));


                            int payWay = 0;
                            if (PaidPrice > 0)
                                ++payWay;

                            if (SupplierPaidPrice > 0)
                                ++payWay;

                            if (TotalPaidPrice != 0 && payWay > 1)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("TotalPaidPrice"), TotalPaidPrice));

                            if (BorrowPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("BorrowPrice"), BorrowPrice));
                            if (KeepPrice != 0)
                                records.Add(new RecordChart(i++, Resources.GetRes().GetString("KeepPrice"), KeepPrice));


                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbSpendStatistic.Text);

                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);

            });
        }

        /// <summary>
        /// 销售产品统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSellProductsStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();

            int OrderType = krpcbOrderType.SelectedIndex;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (OrderType == 0)
                        result = OperatesService.GetOperates().ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, true, -1, -1, out orders);
                    else if (OrderType == 1)
                        result = OperatesService.GetOperates().ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, true, -1, -1, out takeouts);



                    if (result)
                    {
                        if (OrderType == 0)
                        {

                            List<Order> resultList = orders.OrderByDescending(x => x.OrderId).ToList();


                            if (resultList.Count() > 0 && resultList.Any(x => x.State <= 1 && x.tb_orderdetail.Count > 0 && x.tb_orderdetail.Any(y => y.State == 0 || y.State == 2)))
                            {


                                List<OrderDetail> details = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_orderdetail).Where(x => x.State == 0 || x.State == 2).ToList();
                                List<RecordProducts> records = new List<RecordProducts>();

                                if (null != details && details.Count > 0)
                                {
                                    foreach (var item in details.GroupBy(x => x.ProductId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                    {
                                        string name = "";
                                        if (Resources.GetRes().MainLangIndex == 0)
                                            name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName0;
                                        else if (Resources.GetRes().MainLangIndex == 1)
                                            name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName1;
                                        else if (Resources.GetRes().MainLangIndex == 2)
                                            name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName2;



                                        string price = "";
                                        double priceMin = item.Min(x => x.Price);
                                        double priceMax = item.Max(x => x.Price);


                                        if (priceMin != priceMax)
                                            price = string.Format("{0} - {1}", priceMin, priceMax);
                                        else
                                            price = priceMin.ToString();

                                        RecordProducts record = new RecordProducts(item.Key, name, price, item.Sum(x => x.Count), item.Sum(x => x.TotalPrice));

                                        records.Add(record);
                                    }

                                  
                                }

                                double roomPrice = resultList.Where(x => x.State <= 1).Sum(x => x.RoomPrice);
                                if (roomPrice > 0)
                                {
                                    records.Add(new RecordProducts(-1, Resources.GetRes().GetString("RoomPrice"), roomPrice.ToString(), 1, roomPrice));
                                }

                                StatisticModel statisticModel = new StatisticModel();

                                statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                                statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("Count"));
                                statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                                statisticModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice"));
                                statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));
                                statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                                InitialStatistic(ref statisticModel, krpbSellProductsStatistic.Text);


                                this.BeginInvoke(new Action(() =>
                                {
                                    StatisticProductReportWindow window = new StatisticProductReportWindow(records, statisticModel);
                                    window.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }));
                            }
                        }
                        else if (OrderType == 1)
                        {

                            List<Takeout> resultList = takeouts.OrderByDescending(x => x.TakeoutId).ToList();


                            if (resultList.Count() > 0 && resultList.Any(x => x.State <= 1 && x.tb_takeoutdetail.Count > 0 && x.tb_takeoutdetail.Any(y => y.State == 0 || y.State == 2)))
                            {


                                List<TakeoutDetail> details = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_takeoutdetail).Where(x => x.State == 0 || x.State == 2).ToList();
                                List<RecordProducts> records = new List<RecordProducts>();

                                if (null != details && details.Count > 0)
                                {
                                    foreach (var item in details.GroupBy(x => x.ProductId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                    {
                                        long key = 0;
                                        string name = "";
                                        if (null != item.Key)
                                        {
                                            key = item.Key.Value;

                                            if (Resources.GetRes().MainLangIndex == 0)
                                                name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName0;
                                            else if (Resources.GetRes().MainLangIndex == 1)
                                                name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName1;
                                            else if (Resources.GetRes().MainLangIndex == 2)
                                                name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName2;
                                        }



                                        string price = "";
                                        double priceMin = item.Min(x => x.Price);
                                        double priceMax = item.Max(x => x.Price);


                                        if (priceMin != priceMax)
                                            price = string.Format("{0} - {1}", priceMin, priceMax);
                                        else
                                            price = priceMin.ToString();


                                        RecordProducts record = new RecordProducts(key, name, price, item.Sum(x => x.Count), Math.Round(item.Sum(x => x.TotalPrice) ,2));

                                        records.Add(record);
                                    }

                                }


                                StatisticModel statisticModel = new StatisticModel();

                                statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                                statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("Count"));
                                statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                                statisticModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice"));
                                statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));
                                statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                                InitialStatistic(ref statisticModel, krpbSellProductsStatistic.Text);

                                this.BeginInvoke(new Action(() =>
                                {
                                    StatisticProductReportWindow window = new StatisticProductReportWindow(records, statisticModel);
                                    window.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);




            });
        }


        /// <summary>
        /// 购买的产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSpendProductsStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Import> imports = new List<Import>();

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    result = OperatesService.GetOperates().ServiceGetImports(addTimeFinal, finishTimeFinal, true, out imports);
                    if (result)
                    {
                        List<Import> resultList = imports.OrderByDescending(x => x.ImportId).ToList();


                        if (resultList.Count() > 0 && resultList.Any(x => x.tb_importdetail.Count > 0))
                        {


                            List<ImportDetail> details = resultList.SelectMany(x => x.tb_importdetail).ToList();
                            List<RecordProducts> records = new List<RecordProducts>();

                            if (null != details && details.Count > 0)
                            {
                                foreach (var item in details.GroupBy(x => x.ProductId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                {
                                    string name = "";
                                    if (Resources.GetRes().MainLangIndex == 0)
                                        name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName0;
                                    else if (Resources.GetRes().MainLangIndex == 1)
                                        name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName1;
                                    else if (Resources.GetRes().MainLangIndex == 2)
                                        name = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName2;

                                    string price = "";
                                    double priceMin = item.Min(x => x.Price);
                                    double priceMax = item.Max(x => x.Price);


                                    if (priceMin != priceMax)
                                        price = string.Format("{0} - {1}", priceMin, priceMax);
                                    else
                                        price = priceMin.ToString();

                                    RecordProducts record = new RecordProducts(item.Key, name, price, item.Sum(x => x.Count), Math.Round(item.Sum(x => x.TotalPrice) ,2));

                                    records.Add(record);
                                }

                            }


                            StatisticModel statisticModel = new StatisticModel();

                            statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                            statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("Count"));
                            statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                            statisticModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice"));
                            statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));
                            statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                            InitialStatistic(ref statisticModel, krpbSpendProductsStatistic.Text);

                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticProductReportWindow window = new StatisticProductReportWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);



            });
        }

        /// <summary>
        /// 检查同一个队列是否有不同的值
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool ValidateIsSame(List<double> values)
        {
            return values.Distinct().Count() < 2;
        }

        /// <summary>
        /// 支出收入统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSpendPaysStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool resultAdminPays = false;
            bool resultMemberPays = false;
            bool resultSupplierPays = false;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<AdminPay> adminPays;
                    resultAdminPays = OperatesService.GetOperates().ServiceGetAdminPay(-1, -1, addTimeFinal, finishTimeFinal, -1, out adminPays);

                    List<MemberPay> memberPays;
                    resultMemberPays = OperatesService.GetOperates().ServiceGetMemberPay(0, -1, addTimeFinal, finishTimeFinal, -1, out memberPays, out List<OrderPay> tempOrderPay, out List<TakeoutPay> tempTakeoutPay);

                    List<SupplierPay> supplierPays;
                    resultSupplierPays = OperatesService.GetOperates().ServiceGetSupplierPay(0, -1, addTimeFinal, finishTimeFinal, -1, out supplierPays, out List<ImportPay> tempImportPay);

                    if (resultAdminPays && resultMemberPays && resultSupplierPays)
                    {

                        double memberPayPrice = memberPays.Where(x => x.Price < 0).Sum(x=>x.Price);
                        double supplierPayPrice = supplierPays.Where(x => x.Price > 0).Sum(x => x.Price);
                        double adminPayPrice = adminPays.Where(x => x.Price > 0).Sum(x => x.Price);

                       

                        if (memberPayPrice == 0 && supplierPayPrice == 0 && adminPayPrice == 0)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));

                        }
                        else
                        {

                            List<RecordChart> records = new List<RecordChart>();
                            int i = 0;

                            //records.Add(new RecordChartStack(i++, Resources.GetRes().GetString("AdminPay"), Resources.GetRes().GetString("Cash"), Math.Round(adminPayPrice ,2)));
                            records.Add(new RecordChart(i++, Resources.GetRes().GetString("AdminPay"), Math.Round(adminPayPrice, 2))); 
                            records.Add(new RecordChart(i++, Resources.GetRes().GetString("MemberPay"), Math.Round(memberPayPrice ,2)));
                            records.Add(new RecordChart(i++, Resources.GetRes().GetString("SupplierPay"), Math.Round(supplierPayPrice ,2)));


                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbSpendPaysStatistic.Text);


                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }

                    } 

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);



            });
        }

        /// <summary>
        /// 支付收入统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSalePaysStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool resultAdminPays = false;
            bool resultMemberPays = false;
            bool resultSupplierPays = false;
            
            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    List<AdminPay> adminPays;
                    resultAdminPays = OperatesService.GetOperates().ServiceGetAdminPay(-1, -1, addTimeFinal, finishTimeFinal, -1, out adminPays);

                    List<MemberPay> memberPays;
                    resultMemberPays = OperatesService.GetOperates().ServiceGetMemberPay(0, -1, addTimeFinal, finishTimeFinal, -1, out memberPays, out List<OrderPay> tempOrderPay, out List<TakeoutPay> tempTakeoutPay);

                    List<SupplierPay> supplierPays;
                    resultSupplierPays = OperatesService.GetOperates().ServiceGetSupplierPay(0, -1, addTimeFinal, finishTimeFinal, -1, out supplierPays, out List<ImportPay> tempImportPay);
                    
                    


                    if (resultAdminPays && resultMemberPays && resultSupplierPays)
                    {

                        double memberPayPrice = memberPays.Where(x => x.Price > 0).Sum(x => x.Price);
                        double supplierPayPrice = supplierPays.Where(x => x.Price < 0).Sum(x => x.Price);
                        double adminPayPrice = adminPays.Where(x => x.Price < 0).Sum(x => x.Price);



                        if (memberPayPrice == 0 && supplierPayPrice == 0 && adminPayPrice == 0)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));

                        }
                        else
                        {
                            List<RecordChart> records = new List<RecordChart>();
                            int i = 0;

                            records.Add(new RecordChart(i++, Resources.GetRes().GetString("AdminPay"), Math.Round(adminPayPrice, 2)));
                            records.Add(new RecordChart(i++, Resources.GetRes().GetString("MemberPay"), Math.Round(memberPayPrice, 2)));
                            records.Add(new RecordChart(i++, Resources.GetRes().GetString("SupplierPay"), Math.Round(supplierPayPrice, 2)));

                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbSalePaysStatistic.Text);

                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }

                    }

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);



            });
        }



        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSummary_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            // 定义好参数
            bool resultAdminPays = false;
            bool resultMemberPays = false;
            bool resultSupplierPays = false;
            bool resultBalancePays = false;

            bool resultImport = false;
            List<Import> imports = new List<Import>();

            bool resultOrder = false;
            bool resultTakeout = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();


            List<AdminPay> adminPays;
            List<MemberPay> memberPays;
            List<SupplierPay> supplierPays;
            List<BalancePay> balancePays;

            StartLoad(this, null);
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        // 全部获取

                        resultOrder = OperatesService.GetOperates().ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, false, -1, -1, out orders);
                        resultTakeout = OperatesService.GetOperates().ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, false, -1, -1, out takeouts);
                        resultImport = OperatesService.GetOperates().ServiceGetImports(addTimeFinal, finishTimeFinal, false, out imports);




                        resultAdminPays = OperatesService.GetOperates().ServiceGetAdminPay(-1, -1, addTimeFinal, finishTimeFinal, -1, out adminPays);
                        resultMemberPays = OperatesService.GetOperates().ServiceGetMemberPay(0 ,-1, addTimeFinal, finishTimeFinal, -1, out memberPays, out List<OrderPay> tempOrderPay, out List<TakeoutPay> tempTakeoutPay);
                        resultSupplierPays = OperatesService.GetOperates().ServiceGetSupplierPay(0 ,-1, addTimeFinal, finishTimeFinal, -1, out supplierPays, out List<ImportPay> tempImportPay);
                        resultBalancePays = OperatesService.GetOperates().ServiceGetBalancePay(0 ,-1, -1, -1, addTimeFinal, finishTimeFinal, out balancePays, out List<OrderPay> tempOrderPay1, out List<TakeoutPay> tempTakeoutPay1, out List<MemberPay> tempMemberPay1, out List<SupplierPay> tempSupplierPay1, out List<AdminPay> tempAdminPay, out List<ImportPay> tempImportPay1);
                        

                        if (resultOrder && resultTakeout && resultImport && resultAdminPays && resultMemberPays && resultSupplierPays && resultBalancePays)
                        {


                            // 内部收入

                            double TotalPriceOrder = 0;
                            double MemberPaidPriceOrder = 0;
                            double PaidPriceOrder = 0;
                            double TotalPaidPriceOrder = 0;
                            double BorrowPriceOrder = 0;
                            double KeepPriceOrder = 0;

                            if (orders.Count() > 0)
                            {
                                TotalPriceOrder = orders.Where(x => x.State <= 1).Sum(x => x.TotalPrice);
                                MemberPaidPriceOrder = orders.Where(x => x.State <= 1).Sum(x => x.MemberPaidPrice);
                                PaidPriceOrder = orders.Where(x => x.State <= 1).Sum(x => x.PaidPrice);
                                TotalPaidPriceOrder = orders.Where(x => x.State <= 1).Sum(x => x.TotalPaidPrice);
                                BorrowPriceOrder = orders.Where(x => x.State <= 1).Sum(x => x.BorrowPrice);
                                KeepPriceOrder = orders.Where(x => x.State <= 1).Sum(x => x.KeepPrice);


                            }


                            // 外部收入

                            double TotalPriceTakeout = 0;
                            double MemberPaidPriceTakeout = 0;
                            double PaidPriceTakeout = 0;
                            double TotalPaidPriceTakeout = 0;
                            double BorrowPriceTakeout = 0;
                            double KeepPriceTakeout = 0;

                            if (takeouts.Count() > 0)
                            {
                                TotalPriceTakeout = takeouts.Where(x => x.State <= 1).Sum(x => x.TotalPrice);
                                MemberPaidPriceTakeout = takeouts.Where(x => x.State <= 1).Sum(x => x.MemberPaidPrice);
                                PaidPriceTakeout = takeouts.Where(x => x.State <= 1).Sum(x => x.PaidPrice);
                                TotalPaidPriceTakeout = takeouts.Where(x => x.State <= 1).Sum(x => x.TotalPaidPrice);
                                BorrowPriceTakeout = takeouts.Where(x => x.State <= 1).Sum(x => x.BorrowPrice);
                                KeepPriceTakeout = takeouts.Where(x => x.State <= 1).Sum(x => x.KeepPrice);


                            }




                         

                            // 支出

                            double TotalPriceImport = 0;
                            double SupplierPaidPriceImport = 0;
                            double PaidPriceImport = 0;
                            double TotalPaidPriceImport = 0;
                            double BorrowPriceImport = 0;
                            double KeepPriceImport = 0;

                            if (imports.Count() > 0)
                            {
                                TotalPriceImport = imports.Sum(x => x.TotalPrice);
                                SupplierPaidPriceImport = imports.Sum(x => x.SupplierPaidPrice);
                                PaidPriceImport = imports.Sum(x => x.PaidPrice);
                                TotalPaidPriceImport = imports.Sum(x => x.TotalPaidPrice);
 
                                BorrowPriceImport = imports.Sum(x => x.BorrowPrice);
                                KeepPriceImport = imports.Sum(x => x.KeepPrice);
                            }


                            // 支付收入

                            double memberPayPriceIncome = memberPays.Where(x => x.Price > 0).Sum(x => x.Price);
                            double supplierPayPriceIncome = supplierPays.Where(x => x.Price < 0).Sum(x => x.Price);
                            double adminPayPriceIncome = adminPays.Where(x => x.Price < 0).Sum(x => x.Price);
                            double balancePayPriceIncome = balancePays.Where(x => x.Price > 0).Sum(x => x.Price);

                           
                            // 支付支出

                            double memberPayPriceSpend = memberPays.Where(x => x.Price < 0).Sum(x => x.Price);
                            double supplierPayPriceSpend = supplierPays.Where(x => x.Price > 0).Sum(x => x.Price);
                            double adminPayPriceSpend = adminPays.Where(x => x.Price > 0).Sum(x => x.Price);
                            double balancePayPriceSpend = balancePays.Where(x => x.Price < 0).Sum(x => x.Price);


                            // 加入统计报表显示队列中
                            List<SummaryModel> summarys = new List<SummaryModel>();

                            // 总价
                            double TotalIncome =  Math.Round(TotalPriceOrder + TotalPriceTakeout + memberPayPriceIncome + Math.Abs(supplierPayPriceIncome) + Math.Abs(adminPayPriceIncome), 2);
                            double TotalSpend = -Math.Round(TotalPriceImport + Math.Abs(memberPayPriceSpend) + supplierPayPriceSpend + adminPayPriceSpend, 2);
                            double TotalProfit = Math.Round(TotalIncome + TotalSpend, 2);
                            summarys.Add(new SummaryModel() { Name = "TotalPrice", TypeName = Resources.GetRes().GetString("TotalPrice"), Income = TotalIncome, Spend = TotalSpend, Profit = TotalProfit });
                            
                           


                            // 会员余额支付价格
                            double MemberBalancePriceIncome = -Math.Round(MemberPaidPriceOrder + MemberPaidPriceTakeout, 2);
                            double MemberBalancePriceSpend = 0;
                            summarys.Add(new SummaryModel() { Name = "MemberBalancePrice", TypeName = Resources.GetRes().GetString("MemberBalancePrice"), Income = MemberBalancePriceIncome, Spend = MemberBalancePriceSpend, Profit = Math.Round(MemberBalancePriceIncome + MemberBalancePriceSpend, 2) });

                            // 供应商余额支付价格
                            double SupplierBalancePriceIncome = 0;
                            double SupplierBalancePriceSpend = SupplierPaidPriceImport;
                            summarys.Add(new SummaryModel() { Name = "SupplierBalancePrice", TypeName = Resources.GetRes().GetString("SupplierBalancePrice"), Income = SupplierBalancePriceIncome, Spend = SupplierBalancePriceSpend, Profit = Math.Round(SupplierBalancePriceIncome + SupplierBalancePriceSpend, 2) });
                            


                            summarys.Add(new SummaryModel());

                            // 会员支付现金价格
                            double MemberCashIncome = memberPayPriceIncome;
                            double MemberCashSpend = -Math.Abs(memberPayPriceSpend);
                            summarys.Add(new SummaryModel() { Name = "MemberPay", TypeName = Resources.GetRes().GetString("MemberPay"), Income = MemberCashIncome, Spend = MemberCashSpend, Profit = Math.Round(MemberCashIncome + MemberCashSpend, 2) });

                          

                            // 供应商支付现金价格
                            double SupplierCashIncome = supplierPayPriceIncome;
                            double SupplierCashSpend = -Math.Abs(supplierPayPriceSpend);
                            summarys.Add(new SummaryModel() { Name = "SupplierPay", TypeName = Resources.GetRes().GetString("SupplierPay"), Income = SupplierCashIncome, Spend = SupplierCashSpend, Profit = Math.Round(SupplierCashIncome + SupplierCashSpend, 2) });

                           

                            // 管理员支付现金价格
                            double AdminCashIncome = adminPayPriceIncome;
                            double AdminCashSpend = -Math.Abs(adminPayPriceSpend);
                            summarys.Add(new SummaryModel() { Name = "AdminPay", TypeName = Resources.GetRes().GetString("AdminPay"), Income = AdminCashIncome, Spend = AdminCashSpend, Profit = Math.Round(AdminCashIncome + AdminCashSpend, 2) });

                         

                            // 余额支付现金价格
                            double BalanceCashIncome = balancePayPriceIncome;
                            double BalanceCashSpend = -Math.Abs(balancePayPriceSpend);
                            summarys.Add(new SummaryModel() { Name = "BalancePay", TypeName = Resources.GetRes().GetString("BalancePay"), Income = BalanceCashIncome, Spend = BalanceCashSpend, Profit = Math.Round(BalanceCashIncome + BalanceCashSpend, 2) });

                          

                            // 加入辅助统计报表显示队列中
                            List<SummaryModel> summarys2 = new List<SummaryModel>();

                           

                            // 其他价格
                            double OtherPriceIncome = KeepPriceOrder + KeepPriceTakeout + Math.Abs(BorrowPriceImport);
                            double OtherPriceSpend = -(Math.Abs(BorrowPriceOrder) + Math.Abs(BorrowPriceTakeout) + KeepPriceImport);
                            summarys2.Add(new SummaryModel() { Name = "Other", TypeName = Resources.GetRes().GetString("Other"), Income = Math.Round(OtherPriceIncome, 2), Spend = Math.Round(OtherPriceSpend,2), Profit = Math.Round(OtherPriceIncome + OtherPriceSpend, 2) });


                            StatisticModel statisticModel = new StatisticModel();
                            


                            statisticModel.Parameters.Add("TypeName", Resources.GetRes().GetString("TypeName"));
                            statisticModel.Parameters.Add("Income", Resources.GetRes().GetString("Income"));
                            statisticModel.Parameters.Add("Spend", Resources.GetRes().GetString("Expenditure"));
                            statisticModel.Parameters.Add("Profit", Resources.GetRes().GetString("Profit"));
                            statisticModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice"));
                            statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                            InitialStatistic(ref statisticModel, krpbSummary.Text);


                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticSummaryReportWindow window = new StatisticSummaryReportWindow(summarys, summarys2, statisticModel );
                                //window.StartLoad += (sender2, e2) =>
                                //{
                                //    StartLoad(sender2, null);
                                //};
                                //window.StopLoad += (sender2, e2) =>
                                //{
                                //    StopLoad(sender2, null);
                                //};
                                window.StartPrint += (sender3, e3) =>
                                {
                                    PrintLanguageWindow printWindow = new PrintLanguageWindow();
                                    printWindow.ShowDialog(this);
                                    if (printWindow.DialogResult != System.Windows.Forms.DialogResult.OK)
                                        return;

                                    long Lang = printWindow.ReturnValue;

                                    StartLoad(this, null);

                                    Task.Factory.StartNew(() =>
                                    {
                                        try
                                        {
                                            SummaryModelPackage package = new SummaryModelPackage();
                                            package.Records = summarys;
                                            package.Records2 = summarys2;
                                            package.Time = statisticModel.Parameters["Time"].ToString();
                                            package.Lang = Lang;

                                            bool result = Print.Instance.PrintSummary(package, Lang);
                                            window.BeginInvoke(new Action(() =>
                                            {
                                                if (result)
                                                {

                                                }
                                                else
                                                {

                                                }
                                            }));
                                        }
                                        catch (Exception ex)
                                        {
                                            window.BeginInvoke(new Action(() =>
                                            {
                                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                                {
                                                    KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                                }), false, Resources.GetRes().GetString("Exception_OperateRequestFaild"));
                                            }));
                                        }
                                        StopLoad(this, null);
                                    });
                                };
                                window.ShowDialog(this);
                            }));
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }

                    }
                    catch (Exception ex)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                            {
                                KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                        }));
                    }
                    StopLoad(this, null);




                });

        }


        /// <summary>
        /// 余额支付收入统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbBalancePaysIncomeStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;


            bool resultBalancePays = false;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {

                    List<BalancePay> balancePays;
                    resultBalancePays = OperatesService.GetOperates().ServiceGetBalancePay(0, -1, -1, -1, addTimeFinal, finishTimeFinal, out balancePays, out List<OrderPay> tempOrderPay1, out List<TakeoutPay> tempTakeoutPay1, out List<MemberPay> tempMemberPay1, out List<SupplierPay> tempSupplierPay1, out List<AdminPay> tempAdminPay, out List<ImportPay> tempImportPay1);


                    if (resultBalancePays)
                    {

                        double balancePayPrice = balancePays.Where(x => x.Price > 0).Sum(x => x.Price);



                        if (balancePayPrice == 0)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));

                        }
                        else
                        {

                            List<RecordChart> records = new List<RecordChart>();
                            int i = 0;

                            foreach (var item in balancePays.GroupBy(x=>x.BalanceId))
                            {
                                Balance balance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.Key).FirstOrDefault();

                                string name = "";

                                if (Resources.GetRes().MainLangIndex == 0)
                                    name = balance.BalanceName0;
                                else if (Resources.GetRes().MainLangIndex == 1)
                                    name = balance.BalanceName1;
                                else if (Resources.GetRes().MainLangIndex == 2)
                                    name = balance.BalanceName2;


                                records.Add(new RecordChart(i++, name, Math.Round(item.Sum(x=>x.Price), 2)));
                            }

                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbBalancePaysIncomeStatistic.Text);

                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }

                    }

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);
            });

        }

        /// <summary>
        /// 余额支付支出统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbBalancePaysSpendStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;


            bool resultBalancePays = false;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {

                    List<BalancePay> balancePays;
                    resultBalancePays = OperatesService.GetOperates().ServiceGetBalancePay(0, -1, -1, -1, addTimeFinal, finishTimeFinal, out balancePays, out List<OrderPay> tempOrderPay1, out List<TakeoutPay> tempTakeoutPay1, out List<MemberPay> tempMemberPay1, out List<SupplierPay> tempSupplierPay1, out List<AdminPay> tempAdminPay, out List<ImportPay> tempImportPay1);


                    if (resultBalancePays)
                    {

                        double balancePayPrice = balancePays.Where(x => x.Price < 0).Sum(x => x.Price);



                        if (balancePayPrice == 0)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));

                        }
                        else
                        {

                            List<RecordChart> records = new List<RecordChart>();
                            int i = 0;

                            foreach (var item in balancePays.GroupBy(x => x.BalanceId))
                            {
                                Balance balance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.Key).FirstOrDefault();

                                string name = "";

                                if (Resources.GetRes().MainLangIndex == 0)
                                    name = balance.BalanceName0;
                                else if (Resources.GetRes().MainLangIndex == 1)
                                    name = balance.BalanceName1;
                                else if (Resources.GetRes().MainLangIndex == 2)
                                    name = balance.BalanceName2;


                                records.Add(new RecordChart(i++, name, Math.Round(item.Sum(x => x.Price), 2)));
                            }

                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbBalancePaysSpendStatistic.Text);


                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }

                    }

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);
            });

        }



       


        /// <summary>
        /// 按管理员统计出售的商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbAdminSaleStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();

            int OrderType = krpcbOrderType.SelectedIndex;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (OrderType == 0)
                        result = OperatesService.GetOperates().ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, true, -1, -1, out orders);
                    else if (OrderType == 1)
                        result = OperatesService.GetOperates().ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, true, -1, -1, out takeouts);



                    if (result)
                    {
                        if (OrderType == 0)
                        {

                            List<Order> resultList = orders.OrderByDescending(x => x.OrderId).ToList();


                            if (resultList.Count() > 0 && resultList.Any(x => x.State <= 1 && x.tb_orderdetail.Count > 0 && x.tb_orderdetail.Any(y => y.State == 0 || y.State == 2)))
                            {


                                List<OrderDetail> details = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_orderdetail).Where(x => x.State == 0 || x.State == 2).ToList();
                                List<RecordProducts> records = new List<RecordProducts>();

                                if (null != details && details.Count > 0)
                                {
                                    foreach (var item in details.GroupBy(x => x.AdminId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                    {
                                        string name = "";
                                        if (Resources.GetRes().MainLangIndex == 0)
                                            name = Resources.GetRes().Admins.Where(x => x.AdminId == item.Key).FirstOrDefault().AdminName0;
                                        else if (Resources.GetRes().MainLangIndex == 1)
                                            name = Resources.GetRes().Admins.Where(x => x.AdminId == item.Key).FirstOrDefault().AdminName1;
                                        else if (Resources.GetRes().MainLangIndex == 2)
                                            name = Resources.GetRes().Admins.Where(x => x.AdminId == item.Key).FirstOrDefault().AdminName2;



                                        RecordProducts record = new RecordProducts(item.Key, name, "", item.Count(), item.Sum(x => x.TotalPrice));

                                        records.Add(record);
                                    }

                                }

                                StatisticModel statisticModel = new StatisticModel();

                                statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                                statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("RecordCount"));
                                statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                                statisticModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice"));
                                statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));
                               
                                statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                                InitialStatistic(ref statisticModel, krpbAdminSaleStatistic.Text);


                                this.BeginInvoke(new Action(() =>
                                {
                                    StatisticAdminSaleProductReportWindow window = new StatisticAdminSaleProductReportWindow(records, statisticModel);
                                    window.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }));
                            }
                        }
                        else if (OrderType == 1)
                        {

                            List<Takeout> resultList = takeouts.OrderByDescending(x => x.TakeoutId).ToList();


                            if (resultList.Count() > 0 && resultList.Any(x => x.State <= 1 && x.tb_takeoutdetail.Count > 0 && x.tb_takeoutdetail.Any(y => y.State == 0 || y.State == 2)))
                            {


                                List<TakeoutDetail> details = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_takeoutdetail).Where(x => x.State == 0 || x.State == 2).ToList();
                                List<RecordProducts> records = new List<RecordProducts>();

                                if (null != details && details.Count > 0)
                                {
                                    foreach (var item in details.GroupBy(x => x.AdminId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                    {
                                        string name = "";
                                        if (Resources.GetRes().MainLangIndex == 0)
                                            name = Resources.GetRes().Admins.Where(x => x.AdminId == item.Key).FirstOrDefault().AdminName0;
                                        else if (Resources.GetRes().MainLangIndex == 1)
                                            name = Resources.GetRes().Admins.Where(x => x.AdminId == item.Key).FirstOrDefault().AdminName1;
                                        else if (Resources.GetRes().MainLangIndex == 2)
                                            name = Resources.GetRes().Admins.Where(x => x.AdminId == item.Key).FirstOrDefault().AdminName2;



                                        RecordProducts record = new RecordProducts(item.Key, name, "", item.Count(), item.Sum(x => x.TotalPrice));

                                        records.Add(record);
                                    }

                                }


                                StatisticModel statisticModel = new StatisticModel();

                                statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                                statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("RecordCount"));
                                statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                                statisticModel.Parameters.Add("TotalPrice", Resources.GetRes().GetString("TotalPrice"));
                                statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));

                                statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                                InitialStatistic(ref statisticModel, krpbAdminSaleStatistic.Text);


                                this.BeginInvoke(new Action(() =>
                                {
                                    StatisticAdminSaleProductReportWindow window = new StatisticAdminSaleProductReportWindow(records, statisticModel);
                                    window.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);




            });
        }

        /// <summary>
        /// 产品绿润统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbProductProfitStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();

            int OrderType = krpcbOrderType.SelectedIndex;

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (OrderType == 0)
                        result = OperatesService.GetOperates().ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, true, -1, -1, out orders);
                    else if (OrderType == 1)
                        result = OperatesService.GetOperates().ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, true, -1, -1, out takeouts);



                    if (result)
                    {
                        if (OrderType == 0)
                        {

                            List<Order> resultList = orders.OrderByDescending(x => x.OrderId).ToList();


                            if (resultList.Count() > 0) 
                            {


                                List<OrderDetail> details = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_orderdetail).Where(x => x.State == 0 || x.State == 2).ToList();
                                List<RecordProfitProducts> records = new List<RecordProfitProducts>();

                                if (null != details && details.Count > 0)
                                {
                                    foreach (var item in details.GroupBy(x => x.ProductId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                    {
                                        Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault();
                                        string name = "";
                                        if (Resources.GetRes().MainLangIndex == 0)
                                        {
                                            name = product.ProductName0;
                                        }
                                        else if (Resources.GetRes().MainLangIndex == 1)
                                        {
                                            name = product.ProductName1;
                                        }
                                        else if (Resources.GetRes().MainLangIndex == 2)
                                        {
                                            name = product.ProductName2;
                                        }


                                        double count = item.Sum(x => x.Count);
                                        double totalPrice = item.Sum(x => x.TotalPrice);
                                        double totalCostPrice = item.Sum( x=> x.TotalCostPrice);// product.CostPrice * count;


                                        RecordProfitProducts record = new RecordProfitProducts(item.Key, name, totalCostPrice, totalPrice, totalPrice - totalCostPrice, count);

                                        records.Add(record);
                                    }
                                }

                                double roomPrice = resultList.Where(x => x.State <= 1).Sum(x => x.RoomPrice);
                                if (roomPrice > 0)
                                {
                                    records.Add(new RecordProfitProducts(-1, Resources.GetRes().GetString("RoomPrice"), 0, roomPrice, roomPrice, 1));
                                }



                                List<Order> cancelOrders = resultList.Where(x => x.State == 2).ToList();
                                List<OrderDetail> deleteDetails = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_orderdetail).Where(x => x.State == 3).ToList();


                                // 这两个单独显示
                                //if (cancelOrders.Count > 0)
                                //{
                                //    double count = cancelOrders.Count;
                                //    double totalPrice = cancelOrders.Sum(x => x.TotalPrice);
                                //    double totalCostPrice = 0;

                                //    RecordProfitProducts record = new RecordProfitProducts(-999990, Resources.GetRes().GetString("CancelOrder"), -totalCostPrice, -totalPrice, 0, count);

                                //    records.Add(record);
                                //}

                                //if (deleteDetails.Count > 0)
                                //{
                                //    double count = deleteDetails.Sum(x => x.Count);
                                //    double totalPrice = deleteDetails.Sum(x => x.TotalPrice);
                                //    double totalCostPrice = deleteDetails.Sum(x => x.TotalCostPrice);

                                //    RecordProfitProducts record = new RecordProfitProducts(-999991, Resources.GetRes().GetString("DeleteProduct"), totalCostPrice, totalPrice, 0, count);

                                //    records.Add(record);
                                //}

                                StatisticModel statisticModel = new StatisticModel();

                                statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                                statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("Count"));
                                statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                                statisticModel.Parameters.Add("CostPrice", Resources.GetRes().GetString("CostPrice"));
                                statisticModel.Parameters.Add("ProfitPrice", Resources.GetRes().GetString("Profit"));
                                statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));
                                statisticModel.Parameters.Add("Total", Resources.GetRes().GetString("Total") + " :");
                                statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                                InitialStatistic(ref statisticModel, krpbProductProfitStatistic.Text);

                                this.BeginInvoke(new Action(() =>
                                {
                                    StatisticProductProfitReportWindow window = new StatisticProductProfitReportWindow(records, statisticModel);
                                    window.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }));
                            }
                        }
                        else if (OrderType == 1)
                        {

                            List<Takeout> resultList = takeouts.OrderByDescending(x => x.TakeoutId).ToList();


                            if (resultList.Count() > 0 && resultList.Any(x => x.State <= 1 && x.tb_takeoutdetail.Count > 0 && x.tb_takeoutdetail.Any(y => y.State == 0 || y.State == 2)))
                            {


                                List<TakeoutDetail> details = resultList.Where(x => x.State <= 1).SelectMany(x => x.tb_takeoutdetail).Where(x => x.State == 0 || x.State == 2).ToList();
                                List<RecordProfitProducts> records = new List<RecordProfitProducts>();

                                if (null != details && details.Count > 0)
                                {
                                    foreach (var item in details.GroupBy(x => x.ProductId).OrderByDescending(x => x.Sum(y => y.TotalPrice)))
                                    {
                                        Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.Key).FirstOrDefault();
                                        long key = 0;
                                        string name = "";
                                        if (null != item.Key)
                                        {
                                            key = item.Key.Value;

                                            if (Resources.GetRes().MainLangIndex == 0)
                                            {

                                                name = product.ProductName0;
                                            }
                                            else if (Resources.GetRes().MainLangIndex == 1)
                                            {
                                                name = product.ProductName1;
                                            }
                                            else if (Resources.GetRes().MainLangIndex == 2)
                                            {
                                                name = product.ProductName2;
                                            }
                                        }



                                        double count = item.Sum(x => x.Count);
                                        double totalPrice = item.Sum(x => x.TotalPrice);
                                        double totalCostPrice = item.Sum(x => x.TotalCostPrice);// product.CostPrice * count;


                                        RecordProfitProducts record = new RecordProfitProducts(key, name, totalCostPrice, totalPrice, totalPrice - totalCostPrice, count);

                                        records.Add(record);
                                    }

                                }


                                StatisticModel statisticModel = new StatisticModel();

                                statisticModel.Parameters.Add("ProductName", Resources.GetRes().GetString("ProductName"));
                                statisticModel.Parameters.Add("Count", Resources.GetRes().GetString("Count"));
                                statisticModel.Parameters.Add("Price", Resources.GetRes().GetString("Price"));
                                statisticModel.Parameters.Add("CostPrice", Resources.GetRes().GetString("CostPrice"));
                                statisticModel.Parameters.Add("ProfitPrice", Resources.GetRes().GetString("Profit"));
                                statisticModel.Parameters.Add("Proportion", Resources.GetRes().GetString("Proportion"));
                                statisticModel.Parameters.Add("Total", Resources.GetRes().GetString("Total") + " :");
                                statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));
                                InitialStatistic(ref statisticModel, krpbProductProfitStatistic.Text);

                                this.BeginInvoke(new Action(() =>
                                {
                                    StatisticProductProfitReportWindow window = new StatisticProductProfitReportWindow(records, statisticModel);
                                    window.ShowDialog(this);
                                }));
                            }
                            else
                            {
                                this.BeginInvoke(new Action(() =>
                                {
                                    KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);




            });
        }



        /// <summary>
        /// 收入类型统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbIncomeTypeStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            int OrderType = krpcbOrderType.SelectedIndex;

            bool result = false;
            List<OrderPay> orderPays = new List<OrderPay>();
            List<TakeoutPay> takeoutPays = new List<TakeoutPay>();

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {


                    if (OrderType == 0)
                        result = OperatesService.GetOperates().ServiceGetOrderPay(addTimeFinal, finishTimeFinal, out orderPays);
                    else if (OrderType == 1)
                        result = OperatesService.GetOperates().ServiceGetTakeoutPay(addTimeFinal, finishTimeFinal, out takeoutPays);



                    if (result)
                    {
                        List<CommonPayModel> resultList = new List<CommonPayModel>();
                        if (OrderType == 0)
                            resultList = orderPays.Select(x => new CommonPayModel(x)).OrderByDescending(x => x.PayId).ToList();
                        else if (OrderType == 1)
                            resultList = takeoutPays.Select(x => new CommonPayModel(x)).OrderByDescending(x => x.PayId).ToList();


                        if (resultList.Count() > 0)
                        {
                            List<RecordChart> records = new List<RecordChart>();

                            double totalPrice = 0;

                            foreach (var item in resultList.Where(x=> null != x.BalanceId).GroupBy(x=>x.BalanceId))
                            {
                                totalPrice = item.Sum(x => x.Price);
                                if (0 != totalPrice)
                                {
                                    Balance balance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.Key).FirstOrDefault();

                                    string name = "";

                                    if (Resources.GetRes().MainLangIndex == 0)
                                        name = balance.BalanceName0;
                                    else if (Resources.GetRes().MainLangIndex == 1)
                                        name = balance.BalanceName1;
                                    else if (Resources.GetRes().MainLangIndex == 2)
                                        name = balance.BalanceName2;

                                    records.Add(new RecordChart((int)balance.Order, name, totalPrice));


                                }
                            }

                            records = records.OrderByDescending(x => x.ID).ToList();

                            totalPrice = resultList.Sum(x => x.Price);

                            if (0 != totalPrice)
                            {
                                records.Insert(0, new RecordChart(-1, Resources.GetRes().GetString("TotalPrice"), totalPrice));
                            }


                            totalPrice = resultList.Where(x=>x.BalanceId == null).Sum(x => x.Price);
                            if (0 != totalPrice)
                            {
                                records.Add(new RecordChart(records.Max(x=>x.ID) + 1, Resources.GetRes().GetString("Member"), totalPrice));
                            }




                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbIncomeTypeStatistic.Text);

                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);




            });
        }



        /// <summary>
        /// 支出类型统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSpendTypeStatistic_Click(object sender, EventArgs e)
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            int OrderType = krpcbOrderType.SelectedIndex;

            bool result = false;
            List<ImportPay> importPays = new List<ImportPay>();

            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {



                        result = OperatesService.GetOperates().ServiceGetImportPay(addTimeFinal, finishTimeFinal, out importPays);




                    if (result)
                    {
                        List<CommonPayModel> resultList = new List<CommonPayModel>();

                        resultList = importPays.Select(x => new CommonPayModel(x)).OrderByDescending(x => x.PayId).ToList();
  


                        if (resultList.Count() > 0)
                        {
                            List<RecordChart> records = new List<RecordChart>();

                            double totalPrice = 0;

                            foreach (var item in resultList.Where(x => null != x.BalanceId).GroupBy(x => x.BalanceId))
                            {
                                totalPrice = item.Sum(x => x.Price);
                                if (0 != totalPrice)
                                {
                                    Balance balance = Resources.GetRes().Balances.Where(x => x.BalanceId == item.Key).FirstOrDefault();

                                    string name = "";

                                    if (Resources.GetRes().MainLangIndex == 0)
                                        name = balance.BalanceName0;
                                    else if (Resources.GetRes().MainLangIndex == 1)
                                        name = balance.BalanceName1;
                                    else if (Resources.GetRes().MainLangIndex == 2)
                                        name = balance.BalanceName2;

                                    records.Add(new RecordChart((int)balance.Order, name, totalPrice));


                                }
                            }

                            records = records.OrderByDescending(x => x.ID).ToList();


                            totalPrice = resultList.Sum(x => x.Price);

                            if (0 != totalPrice)
                            {
                                records.Insert(0, new RecordChart(-1, Resources.GetRes().GetString("TotalPrice"), totalPrice));
                            }


                            totalPrice = resultList.Where(x => x.BalanceId == null).Sum(x => x.Price);
                            if (0 != totalPrice)
                            {
                                records.Add(new RecordChart(records.Max(x => x.ID) + 1, Resources.GetRes().GetString("Supplier"), totalPrice));
                            }



                            StatisticModel statisticModel = new StatisticModel();
                            InitialStatistic(ref statisticModel, krpbSpendTypeStatistic.Text);


                            this.BeginInvoke(new Action(() =>
                            {
                                StatisticCharWindow window = new StatisticCharWindow(records, statisticModel);
                                window.ShowDialog(this);
                            }));
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                KryptonMessageBox.Show(this, Resources.GetRes().GetString("NotFoundAnyRecords"), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Search")));
                    }));
                }
                StopLoad(this, null);




            });
        }


        /// <summary>
        /// 初始化参数和风格
        /// </summary>
        /// <param name="Model"></param>
        private void InitialStatistic(ref StatisticModel Model, string Title)
        {

            Model.Title = Title;
            Model.Font = new System.Drawing.Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));
            Model.EnableAntialiasing = Resources.GetRes().GetString("EnableAntialiasing") == "1";


            Model.Parameters.Add("PleaseWait", Resources.GetRes().GetString("PleaseWait"));
            Model.Parameters.Add("CreatingTheDocument", Resources.GetRes().GetString("CreatingTheDocument"));
            Model.Parameters.Add("PriceSymbol", Resources.GetRes().PrintInfo.PriceSymbol);

            CultureInfo ci = null;
            // 维文
            if (Resources.GetRes().MainLangIndex == 0)
            {
                // 更改参数
                Model.Parameters.Add("CompanyName", Resources.GetRes().KEY_NAME_0);
            }
            // 中文
            else if (Resources.GetRes().MainLangIndex == 1)
            {
                // 更改参数
                Model.Parameters.Add("CompanyName", Resources.GetRes().KEY_NAME_1);
            }

            // 英文
            else if (Resources.GetRes().MainLangIndex == 2)
            {
                // 更改参数
                Model.Parameters.Add("CompanyName", Resources.GetRes().KEY_NAME_2);
            }
            ci = Resources.GetRes().MainLang.Culture;

            //if (Resources.GetRes().GetString("LargeFont", ci) == "1")
            //{
                // 更改样式
                Model.Fonts.Add("xrcsFont10", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 10));
                Model.Fonts.Add("xrcsFont9", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 9));
                Model.Fonts.Add("xrcsFont8", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 8));
                Model.Fonts.Add("xrcsFont11", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 11));
                Model.Fonts.Add("xrcsFont12", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 12));
                Model.Fonts.Add("xrcsFont13", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 13));
                Model.Fonts.Add("xrcsFont14", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 14));
                Model.Fonts.Add("xrcsFont15", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 15));
                Model.Fonts.Add("xrcsFont16", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 16));
                Model.Fonts.Add("xrcsFont17", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 17));
                Model.Fonts.Add("xrcsFont18", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 18));
            //}
            //else
            //{
            //    // 默认样式
            //    Model.Fonts.Add("xrcsFont10", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 10));
            //    Model.Fonts.Add("xrcsFont9", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 9));
            //    Model.Fonts.Add("xrcsFont8", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 8));
            //    Model.Fonts.Add("xrcsFont11", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 11));
            //    Model.Fonts.Add("xrcsFont12", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 12));
            //    Model.Fonts.Add("xrcsFont13", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 13));
            //    Model.Fonts.Add("xrcsFont14", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 14));
            //    Model.Fonts.Add("xrcsFont15", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 15));
            //    Model.Fonts.Add("xrcsFont16", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 16));
            //    Model.Fonts.Add("xrcsFont17", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 17));
            //    Model.Fonts.Add("xrcsFont18", new System.Drawing.Font(Resources.GetRes().GetString("FontName", ci).Split(',')[0], 18));
            //}
        }
    }
}
