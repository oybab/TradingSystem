using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Oybab.TradingSystemX.VM.Converters;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.VM.ViewModels.Navigations;
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.DAL;
using Oybab.Res.View.Models;
using Oybab.Report.Model;
using System.Globalization;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class StatisticViewModel : ViewModelBase
    {
        private Page _element;



        public StatisticViewModel(Page _element)
        {
            this._element = _element;

        }



        private bool _isInitial = false;
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (!_isInitial)
            {
                _isInitial = true;

                AddDateStart = DateTime.Now.AddDays(-1);
                AddTimeStart = DateTime.Now.AddDays(-1).TimeOfDay;

                AddDateEnd = DateTime.Now;
                AddTimeEnd = DateTime.Now.TimeOfDay;

                RefreshBillType();

            }
        }

     

        /// <summary>
        /// 刷新账单类型
        /// </summary>
        private void RefreshBillType()
        {

            BillType.Clear();
            BillType.Add(new Dict() { Name = Resources.Instance.GetString("BillTypeInner"), Value = 0 });
            BillType.Add(new Dict() { Name = Resources.Instance.GetString("BillTypeOuter"), Value = 1 });
            BillType.Add(new Dict() { Name = Resources.Instance.GetString("Expenditure"), Value = 2 });

            SelectedBillType = BillType.FirstOrDefault();
        }



        /// <summary>
        /// 刷新类型名称
        /// </summary>
        private void RefreshTypeName()
        {
            TypeName.Clear();

            if (BillType.Count != 0)
            {

                if ((int)(SelectedBillType as Dict).Value == 0 || (int)(SelectedBillType as Dict).Value == 1)
                {
                    TypeName.Add(new Dict() { Name = Resources.Instance.GetString("IncomeStatistic"), Value = 1 });
                    TypeName.Add(new Dict() { Name = Resources.Instance.GetString("IncomeTypeStatistic"), Value = 2 });
                    TypeName.Add(new Dict() { Name = Resources.Instance.GetString("SellProductsStatistic"), Value = 3 });

                }
                else if ((int)(SelectedBillType as Dict).Value == 2)
                {
                    TypeName.Add(new Dict() { Name = Resources.Instance.GetString("SpendStatistic"), Value = -1 });
                    TypeName.Add(new Dict() { Name = Resources.Instance.GetString("SpendTypeStatistic"), Value = -2 });
                    TypeName.Add(new Dict() { Name = Resources.Instance.GetString("SpendProductsStatistic"), Value = -3 });
                }

                SelectedTypeName = TypeName.FirstOrDefault();
            }

        }


      



        private DateTime _addDateStart = DateTime.MinValue;
        /// <summary>
        /// 添加日期(开始)
        /// </summary>
        public DateTime AddDateStart
        {
            get { return _addDateStart; }
            set
            {
                _addDateStart = value;
                OnPropertyChanged("AddDateStart");
            }
        }


        private TimeSpan _addTimeStart = TimeSpan.FromSeconds(0);
        /// <summary>
        /// 添加时间(开始)
        /// </summary>
        public TimeSpan AddTimeStart
        {
            get { return _addTimeStart; }
            set
            {
                _addTimeStart = value;
                OnPropertyChanged("AddTimeStart");
            }
        }




        private DateTime _addDateEnd = DateTime.MinValue;
        /// <summary>
        /// 添加日期(结束)
        /// </summary>
        public DateTime AddDateEnd
        {
            get { return _addDateEnd; }
            set
            {
                _addDateEnd = value;
                OnPropertyChanged("AddDateEnd");
            }
        }


        private TimeSpan _addTimeEnd = TimeSpan.FromSeconds(0);
        /// <summary>
        /// 添加时间(结束)
        /// </summary>
        public TimeSpan AddTimeEnd
        {
            get { return _addTimeEnd; }
            set
            {
                _addTimeEnd = value;
                OnPropertyChanged("AddTimeEnd");
            }
        }


       

        private ObservableCollection<Dict> _billType = new ObservableCollection<Dict>();
        /// <summary>
        /// 账单类型
        /// </summary>
        public ObservableCollection<Dict> BillType
        {
            get { return _billType; }
            set
            {
                _billType = value;
                OnPropertyChanged("BillType");
            }
        }


        private Dict _selectedBillType = null;
        /// <summary>
        /// 选中的账单类型
        /// </summary>
        public Dict SelectedBillType
        {
            get { return _selectedBillType; }
            set
            {
                _selectedBillType = value;
                OnPropertyChanged("SelectedBillType");
                RefreshTypeName();
            }
        }





        private ObservableCollection<Dict> _typeName = new ObservableCollection<Dict>();
        /// <summary>
        /// 类型名称
        /// </summary>
        public ObservableCollection<Dict> TypeName
        {
            get { return _typeName; }
            set
            {
                _typeName = value;
                OnPropertyChanged("TypeName");
            }
        }


        private Dict _selectedTypeName = null;
        /// <summary>
        /// 选中的类型名称
        /// </summary>
        public Dict SelectedTypeName
        {
            get { return _selectedTypeName; }
            set
            {
                _selectedTypeName = value;
                OnPropertyChanged("SelectedTypeName");
            }
        }









        private bool _isLoading;
        /// <summary>
        /// 显示正在加载
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
            }
        }








        /// <summary>
        /// 统计
        /// </summary>
        private RelayCommand _statisticCommand;
        public Command StatisticCommand
        {
            get
            {
                return _statisticCommand ?? (_statisticCommand = new RelayCommand(async param =>
                {
                    //int billType = (int)(SelectedBillType as Dict).Value;
                    int typeName = (int)(SelectedTypeName as Dict).Value;

                    if (typeName == 1)
                        await IncomeStatistic();
                    else if (typeName == 2)
                        await IncomeTypeStatistic();
                    else if (typeName == 3)
                        await SellProductsStatistic();
                    else if (typeName == -1)
                        await SpendStatistic();
                    else if (typeName == -2)
                        await SpendTypeStatistic();
                    else if (typeName == -3)
                        await SpendProductsStatistic();

                }));
            }
        }


       


       



        /// <summary>
        /// 返回
        /// </summary>
        private RelayCommand _backCommand;
        public Command BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.GoNavigateBack();
                }));
            }
        }











        /// <summary>
        /// 收入统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task IncomeStatistic()
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            int OrderType = (int)(SelectedBillType as Dict).Value;

            bool result = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();

            IsLoading = true;


            try
            {


                if (OrderType == 0)
                {
                    var theResult = await OperatesService.Instance.ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, false, -1, -1);
                    result = theResult.result;
                    orders = theResult.orders;
                }
                else if (OrderType == 1)
                {
                    var theResult = await OperatesService.Instance.ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, false, -1, -1);
                    result = theResult.result;
                    takeouts = theResult.takeouts;
                }



                if (result)
                {
                    List<BillModel> resultList = new List<BillModel>();
                    if (OrderType == 0)
                        resultList = orders.Select(x => new BillModel(x)).OrderByDescending(x => x.Id).ToList();
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
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("TotalPrice"), TotalPrice));
                        if (MemberDealsPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("MemberDealsPrice"), MemberDealsPrice));
                        if (DealsPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("DealsPrice"), DealsPrice));
                        if (ActualPrice != 0 && TotalPrice != ActualPrice)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("ActualPrice"), ActualPrice));
                        if (MemberPaidPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("MemberPaidPrice"), MemberPaidPrice));
                        if (PaidPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("PaidPrice"), PaidPrice));
                        if (CardPaidPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("CardPaidPrice"), CardPaidPrice));

                        int payWay = 0;
                        if (PaidPrice > 0)
                            ++payWay;
                        if (CardPaidPrice > 0)
                            ++payWay;
                        if (MemberPaidPrice > 0)
                            ++payWay;

                        if (TotalPaidPrice != 0 && payWay > 1)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("TotalPaidPrice"), TotalPaidPrice));
                        if (ReturnPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("ReturnPrice"), ReturnPrice));
                        if (BorrowPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("OwedPrice"), BorrowPrice));
                        if (KeepPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("KeepPrice"), KeepPrice));



                        StatisticModel statisticModel = new StatisticModel();
                        InitialStatistic(ref statisticModel, Resources.Instance.GetString("IncomeStatistic"));



                        StatisticWebPage page = new StatisticWebPage();
                        (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                        NavigationPath.Instance.GoNavigateNext(page);

                    }
                    else
                    {

                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }));
            }

            IsLoading = false;


        }

        /// <summary>
        /// 支出统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task SpendStatistic()
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Import> imports = new List<Import>();

            IsLoading = true;


            try
            {
                var theResult = await OperatesService.Instance.ServiceGetImports(addTimeFinal, finishTimeFinal, false);
                result = theResult.result;
                imports = theResult.imports;
                if (result)
                {
                    List<Import> resultList = imports.OrderByDescending(x => x.ImportId).ToList();


                    if (resultList.Count() > 0)
                    {

                        double TotalPrice = Math.Round(resultList.Sum(x => x.TotalPrice), 2);
                        double SupplierPaidPrice = Math.Round(resultList.Sum(x => x.SupplierPaidPrice), 2);
                        double PaidPrice = Math.Round(resultList.Sum(x => x.PaidPrice), 2);
                        double TotalPaidPrice = Math.Round(resultList.Sum(x => x.TotalPaidPrice), 2);
                        double BorrowPrice = Math.Round(resultList.Sum(x => x.BorrowPrice), 2);
                        double KeepPrice = Math.Round(resultList.Sum(x => x.KeepPrice), 2);



                        List<RecordChart> records = new List<RecordChart>();
                        int i = 0;
                        if (TotalPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("TotalPrice"), TotalPrice));
                        if (SupplierPaidPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("SupplierPaidPrice"), SupplierPaidPrice));
                        if (PaidPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("PaidPrice"), PaidPrice));


                        int payWay = 0;
                        if (PaidPrice > 0)
                            ++payWay;

                        if (SupplierPaidPrice > 0)
                            ++payWay;

                        if (TotalPaidPrice != 0 && payWay > 1)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("TotalPaidPrice"), TotalPaidPrice));

                        if (BorrowPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("BorrowPrice"), BorrowPrice));
                        if (KeepPrice != 0)
                            records.Add(new RecordChart(i++, Resources.Instance.GetString("KeepPrice"), KeepPrice));


                        StatisticModel statisticModel = new StatisticModel();
                        InitialStatistic(ref statisticModel, Resources.Instance.GetString("SpendStatistic"));


                        StatisticWebPage page = new StatisticWebPage();
                        (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                        NavigationPath.Instance.GoNavigateNext(page);

                    }
                    else
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }));
            }
            IsLoading = false;

        }






        /// <summary>
        /// 收入类型统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async Task IncomeTypeStatistic()
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            int OrderType = (int)(SelectedBillType as Dict).Value;

            bool result = false;
            List<OrderPay> orderPays = new List<OrderPay>();
            List<TakeoutPay> takeoutPays = new List<TakeoutPay>();

            IsLoading = true;


            try
            {


                if (OrderType == 0)
                {
                    var theResult = await OperatesService.Instance.ServiceGetOrderPay(addTimeFinal, finishTimeFinal);
                    result = theResult.result;
                    orderPays = theResult.orderPays;
                }
                else if (OrderType == 1)
                {
                    var theResult = await OperatesService.Instance.ServiceGetTakeoutPay(addTimeFinal, finishTimeFinal);
                    result = theResult.result;
                    takeoutPays = theResult.takeoutPays;
                }



                if (result)
                {
                    List<CommonPayModel> resultList = new List<CommonPayModel>();
                    if (OrderType == 0)
                        resultList = orderPays.Where(x=>x.State != 3).Select(x => new CommonPayModel(x)).OrderByDescending(x => x.PayId).ToList();
                    else if (OrderType == 1)
                        resultList = takeoutPays.Where(x => x.State != 3).Select(x => new CommonPayModel(x)).OrderByDescending(x => x.PayId).ToList();


                    if (resultList.Count() > 0)
                    {
                        List<RecordChart> records = new List<RecordChart>();

                        double totalPrice = 0;

                        foreach (var item in resultList.Where(x => null != x.BalanceId).GroupBy(x => x.BalanceId))
                        {
                            totalPrice = item.Sum(x => x.Price);
                            if (0 != totalPrice)
                            {
                                Balance balance = Resources.Instance.Balances.Where(x => x.BalanceId == item.Key).FirstOrDefault();

                                string name = "";

                                if (Res.Instance.MainLangIndex == 0)
                                    name = balance.BalanceName0;
                                else if (Res.Instance.MainLangIndex == 1)
                                    name = balance.BalanceName1;
                                else if (Res.Instance.MainLangIndex == 2)
                                    name = balance.BalanceName2;

                                records.Add(new RecordChart((int)balance.Order, name, totalPrice));


                            }
                        }

                        records = records.OrderByDescending(x => x.ID).ToList();

                        totalPrice = resultList.Sum(x => x.Price);

                        if (0 != totalPrice)
                        {
                            records.Insert(0, new RecordChart(-1, Resources.Instance.GetString("TotalPrice"), totalPrice));
                        }


                        totalPrice = resultList.Where(x => x.BalanceId == null).Sum(x => x.Price);
                        if (0 != totalPrice)
                        {
                            records.Add(new RecordChart(records.Max(x => x.ID) + 1, Resources.Instance.GetString("Member"), totalPrice));
                        }




                        StatisticModel statisticModel = new StatisticModel();
                        InitialStatistic(ref statisticModel, Resources.Instance.GetString("IncomeTypeStatistic"));


                        StatisticWebPage page = new StatisticWebPage();
                        (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                        NavigationPath.Instance.GoNavigateNext(page);

                    }
                    else
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }));
            }
            IsLoading = false;

        }



        /// <summary>
        /// 支出类型统计
        /// </summary>
        private async Task SpendTypeStatistic()
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            int OrderType = (int)(SelectedBillType as Dict).Value;

            bool result = false;
            List<ImportPay> importPays = new List<ImportPay>();

            IsLoading = true;


            try
            {



                var theResult = await OperatesService.Instance.ServiceGetImportPay(addTimeFinal, finishTimeFinal);

                result = theResult.result;
                importPays = theResult.importPays;


                if (result)
                {
                    List<CommonPayModel> resultList = new List<CommonPayModel>();

                    resultList = importPays.Where(x => x.State != 3).Select(x => new CommonPayModel(x)).OrderByDescending(x => x.PayId).ToList();



                    if (resultList.Count() > 0)
                    {
                        List<RecordChart> records = new List<RecordChart>();

                        double totalPrice = 0;

                        foreach (var item in resultList.Where(x => null != x.BalanceId).GroupBy(x => x.BalanceId))
                        {
                            totalPrice = item.Sum(x => x.Price);
                            if (0 != totalPrice)
                            {
                                Balance balance = Resources.Instance.Balances.Where(x => x.BalanceId == item.Key).FirstOrDefault();

                                string name = "";

                                if (Res.Instance.MainLangIndex == 0)
                                    name = balance.BalanceName0;
                                else if (Res.Instance.MainLangIndex == 1)
                                    name = balance.BalanceName1;
                                else if (Res.Instance.MainLangIndex == 2)
                                    name = balance.BalanceName2;

                                records.Add(new RecordChart((int)balance.Order, name, totalPrice));


                            }
                        }

                        records = records.OrderByDescending(x => x.ID).ToList();


                        totalPrice = resultList.Sum(x => x.Price);

                        if (0 != totalPrice)
                        {
                            records.Insert(0, new RecordChart(-1, Resources.Instance.GetString("TotalPrice"), totalPrice));
                        }


                        totalPrice = resultList.Where(x => x.BalanceId == null).Sum(x => x.Price);
                        if (0 != totalPrice)
                        {
                            records.Add(new RecordChart(records.Max(x => x.ID) + 1, Resources.Instance.GetString("Supplier"), totalPrice));
                        }



                        StatisticModel statisticModel = new StatisticModel();
                        InitialStatistic(ref statisticModel, Resources.Instance.GetString("SpendTypeStatistic"));



                        StatisticWebPage page = new StatisticWebPage();
                        (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                        NavigationPath.Instance.GoNavigateNext(page);

                    }
                    else
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }));
            }
            IsLoading = false;


        }





        /// <summary>
        /// 销售产品统计
        /// </summary>
        private async Task SellProductsStatistic()
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Order> orders = new List<Order>();
            List<Takeout> takeouts = new List<Takeout>();

            int OrderType = (int)(SelectedBillType as Dict).Value;

            IsLoading = true;


            try
            {
                if (OrderType == 0)
                {
                    var theResult = await OperatesService.Instance.ServiceGetOrders(0, 0, addTimeFinal, finishTimeFinal, 0, -1, true, -1, -1);
                    result = theResult.result;
                    orders = theResult.orders;
                }
                else if (OrderType == 1)
                {
                    var theResult = await OperatesService.Instance.ServiceGetTakeouts(0, addTimeFinal, finishTimeFinal, -1, null, null, false, true, -1, -1);
                    result = theResult.result;
                    takeouts = theResult.takeouts;
                }



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
                                    if (Res.Instance.MainLangIndex == 0)
                                        name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName0;
                                    else if (Res.Instance.MainLangIndex == 1)
                                        name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName1;
                                    else if (Res.Instance.MainLangIndex == 2)
                                        name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName2;



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
                                records.Add(new RecordProducts(-1, Resources.Instance.GetString("RoomPrice"), roomPrice.ToString(), 1, roomPrice));
                            }

                            StatisticModel statisticModel = new StatisticModel();

                            statisticModel.Parameters.Add("ProductName", Resources.Instance.GetString("ProductName"));
                            statisticModel.Parameters.Add("Count", Resources.Instance.GetString("Count"));
                            statisticModel.Parameters.Add("Price", Resources.Instance.GetString("Price"));
                            statisticModel.Parameters.Add("TotalPrice", Resources.Instance.GetString("TotalPrice"));
                            statisticModel.Parameters.Add("Proportion", Resources.Instance.GetString("Proportion"));
                            statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));



                            InitialStatistic(ref statisticModel, Resources.Instance.GetString("SellProductsStatistic"));


                            StatisticWebPage page = new StatisticWebPage();
                            (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                            NavigationPath.Instance.GoNavigateNext(page);


                        }
                        else
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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

                                        if (Res.Instance.MainLangIndex == 0)
                                            name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName0;
                                        else if (Res.Instance.MainLangIndex == 1)
                                            name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName1;
                                        else if (Res.Instance.MainLangIndex == 2)
                                            name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName2;
                                    }



                                    string price = "";
                                    double priceMin = item.Min(x => x.Price);
                                    double priceMax = item.Max(x => x.Price);


                                    if (priceMin != priceMax)
                                        price = string.Format("{0} - {1}", priceMin, priceMax);
                                    else
                                        price = priceMin.ToString();


                                    RecordProducts record = new RecordProducts(key, name, price, item.Sum(x => x.Count), Math.Round(item.Sum(x => x.TotalPrice), 2));

                                    records.Add(record);
                                }

                            }


                            StatisticModel statisticModel = new StatisticModel();

                            statisticModel.Parameters.Add("ProductName", Resources.Instance.GetString("ProductName"));
                            statisticModel.Parameters.Add("Count", Resources.Instance.GetString("Count"));
                            statisticModel.Parameters.Add("Price", Resources.Instance.GetString("Price"));
                            statisticModel.Parameters.Add("TotalPrice", Resources.Instance.GetString("TotalPrice"));
                            statisticModel.Parameters.Add("Proportion", Resources.Instance.GetString("Proportion"));
                            statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));

                            InitialStatistic(ref statisticModel, Resources.Instance.GetString("SellProductsStatistic"));


                            StatisticWebPage page = new StatisticWebPage();
                            (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                            NavigationPath.Instance.GoNavigateNext(page);
                        }
                        else
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }));
            }
            IsLoading = false;


        }


        /// <summary>
        /// 购买的产品
        /// </summary>
        private async Task SpendProductsStatistic()
        {
            long addTimeFinal, finishTimeFinal;
            if (!CheckTime(out addTimeFinal, out finishTimeFinal))
                return;

            bool result = false;
            List<Import> imports = new List<Import>();

            IsLoading = true;


            try
            {
                var theResult = await OperatesService.Instance.ServiceGetImports(addTimeFinal, finishTimeFinal, true);
                result = theResult.result;
                imports = theResult.imports;
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
                                if (Res.Instance.MainLangIndex == 0)
                                    name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName0;
                                else if (Res.Instance.MainLangIndex == 1)
                                    name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName1;
                                else if (Res.Instance.MainLangIndex == 2)
                                    name = Resources.Instance.Products.Where(x => x.ProductId == item.Key).FirstOrDefault().ProductName2;

                                string price = "";
                                double priceMin = item.Min(x => x.Price);
                                double priceMax = item.Max(x => x.Price);


                                if (priceMin != priceMax)
                                    price = string.Format("{0} - {1}", priceMin, priceMax);
                                else
                                    price = priceMin.ToString();

                                RecordProducts record = new RecordProducts(item.Key, name, price, item.Sum(x => x.Count), Math.Round(item.Sum(x => x.TotalPrice), 2));

                                records.Add(record);
                            }

                        }


                        StatisticModel statisticModel = new StatisticModel();

                        statisticModel.Parameters.Add("ProductName", Resources.Instance.GetString("ProductName"));
                        statisticModel.Parameters.Add("Count", Resources.Instance.GetString("Count"));
                        statisticModel.Parameters.Add("Price", Resources.Instance.GetString("Price"));
                        statisticModel.Parameters.Add("TotalPrice", Resources.Instance.GetString("TotalPrice"));
                        statisticModel.Parameters.Add("Proportion", Resources.Instance.GetString("Proportion"));
                        statisticModel.Parameters.Add("Time", string.Format("({0} - {1})", DateTime.ParseExact(addTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), DateTime.ParseExact(finishTimeFinal.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm")));

                        InitialStatistic(ref statisticModel, Resources.Instance.GetString("SpendProductsStatistic"));


                        StatisticWebPage page = new StatisticWebPage();
                        (page.BindingContext as StatisticWebViewModel).Initial(records, statisticModel);
                        NavigationPath.Instance.GoNavigateNext(page);
                    }
                    else
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("NotFoundAnyRecords"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }));
            }
            IsLoading = false;

        }






        private TimeSpan TimeLimit = TimeSpan.FromDays(Resources.Instance.ShorDay);
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

           
            startDateTime = AddDateStart + AddTimeStart;
            endDateTime = AddDateEnd + AddTimeEnd;
           


            // 有包厢在用可以搜索7天内的. 没有则可以搜索35天内的
            if (Resources.Instance.RoomsModel.Any(x => null != x.PayOrder))
                TimeLimit = TimeSpan.FromDays(Resources.Instance.DefaultDay);
            else
                TimeLimit = TimeSpan.FromDays(Resources.Instance.LongDay);

            if ((endDateTime - startDateTime).TotalMinutes <= 0 || !((endDateTime - startDateTime).TotalMinutes <= TimeLimit.TotalMinutes))
            {
                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, string.Format(Resources.Instance.GetString("TimeLimit"), TimeLimit.TotalDays), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                return false;
            }

            addTimeFinal = long.Parse(startDateTime.ToString("yyyyMMddHHmmss"));
            finishTimeFinal = long.Parse(endDateTime.ToString("yyyyMMddHHmmss"));

            return true;

        }




        /// <summary>
        /// 初始化参数和风格
        /// </summary>
        /// <param name="Model"></param>
        private void InitialStatistic(ref StatisticModel Model, string Title)
        {

            Model.Title = Title;

            Model.EnableAntialiasing = Resources.Instance.GetString("EnableAntialiasing") == "1";


            Model.Parameters.Add("PleaseWait", Resources.Instance.GetString("PleaseWait"));
            Model.Parameters.Add("CreatingTheDocument", Resources.Instance.GetString("CreatingTheDocument"));
            Model.Parameters.Add("PriceSymbol", Resources.Instance.PrintInfo.PriceSymbol);

            CultureInfo ci = null;
            // 维文
            if (Res.Instance.MainLangIndex == 0)
            {
                // 更改参数
                Model.Parameters.Add("CompanyName", Resources.Instance.KEY_NAME_0);
            }
            // 中文
            else if (Res.Instance.MainLangIndex == 1)
            {
                // 更改参数
                Model.Parameters.Add("CompanyName", Resources.Instance.KEY_NAME_1);
            }

            // 英文
            else if (Res.Instance.MainLangIndex == 2)
            {
                // 更改参数
                Model.Parameters.Add("CompanyName", Resources.Instance.KEY_NAME_2);
            }
            ci = Res.Instance.MainLang.Culture;

            //if (Resources.Instance.GetString("LargeFont", ci) == "1")
            //{
            // 更改样式
            Model.Fonts.Add("xrcsFont10", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 10));
            Model.Fonts.Add("xrcsFont9", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 9));
            Model.Fonts.Add("xrcsFont8", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 8));
            Model.Fonts.Add("xrcsFont11", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 11));
            Model.Fonts.Add("xrcsFont12", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 12));
            Model.Fonts.Add("xrcsFont13", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 13));
            Model.Fonts.Add("xrcsFont14", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 14));
            Model.Fonts.Add("xrcsFont15", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 15));
            Model.Fonts.Add("xrcsFont16", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 16));
            Model.Fonts.Add("xrcsFont17", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 17));
            Model.Fonts.Add("xrcsFont18", new ModelsForViews.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 18));
            //}
            //else
            //{
            //    // 默认样式
            //    Model.Fonts.Add("xrcsFont10", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 10));
            //    Model.Fonts.Add("xrcsFont9", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 9));
            //    Model.Fonts.Add("xrcsFont8", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 8));
            //    Model.Fonts.Add("xrcsFont11", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 11));
            //    Model.Fonts.Add("xrcsFont12", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 12));
            //    Model.Fonts.Add("xrcsFont13", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 13));
            //    Model.Fonts.Add("xrcsFont14", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 14));
            //    Model.Fonts.Add("xrcsFont15", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 15));
            //    Model.Fonts.Add("xrcsFont16", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 16));
            //    Model.Fonts.Add("xrcsFont17", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 17));
            //    Model.Fonts.Add("xrcsFont18", new System.Drawing.Font(Resources.Instance.GetString("FontName", ci).Split(',')[0], 18));
            //}
        }



    }
}
