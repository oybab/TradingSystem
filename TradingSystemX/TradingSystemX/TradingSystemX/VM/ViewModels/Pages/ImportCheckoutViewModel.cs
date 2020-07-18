using Oybab.DAL;
using Oybab.Res.Server.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oybab.Res.Tools;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Oybab.TradingSystemX.VM.Commands;
using System.Threading.Tasks;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.Server;
using Oybab.Res.Exceptions;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.Res.Server;
using Oybab.TradingSystemX.VM.ModelsForViews;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class ImportCheckoutViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;

        private Import import = null;
        private List<ImportDetail> details = null;
        private List<ImportPay> payList = null;
        private List<ImportPay> tempPayList = new List<ImportPay>();



        public ImportCheckoutViewModel(Xamarin.Forms.Page element, StackLayout paidPricePanel, ControlTemplate paidPriceTemplate)
        {
            this._element = element;
            this.ChangePaidPrice = new ChangePaidPriceViewModel(null, RecalcPaidPrice, paidPricePanel, paidPriceTemplate);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object obj)
        {
            this.PaidPrice = "0";
            
            this.SupplierPaidPrice = "0";

            this.BalanceMode = 0;


            Import model = obj as Import;

            if (null != model.tb_importdetail)
                details = model.tb_importdetail.ToList();
            else
                details = new List<ImportDetail>();

            if (null != model.tb_importpay)
                payList = model.tb_importpay.ToList();
            else
                payList = new List<ImportPay>();

            this.tempPayList = payList.ToList();



            import = model.FastCopy();


            if (!string.IsNullOrWhiteSpace(import.Remark))
                this.ChangePaidPrice.Remark = import.Remark;
            else
                this.ChangePaidPrice.Remark = null;



            import.tb_supplier = model.tb_supplier;


            Calc();
        }



        private List<CommonPayModel> oldList = null;
        /// <summary>
        /// 打开修改金额
        /// </summary>
        private RelayCommand _changePriceCommand;
        public Xamarin.Forms.Command ChangePriceCommand
        {
            get
            {
                return _changePriceCommand ?? (_changePriceCommand = new RelayCommand(param =>
                {


                    IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {
                            oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                            ChangePaidPrice.InitialView(this.import.TotalPrice, oldList, false, true, 3);

                            IsLoading = false;
                        });
                    });
                }));

            }
        }



        /// <summary>
        /// 重新计算支付金额
        /// </summary>
        private void RecalcPaidPrice()
        {

            bool RoomPaidPriceChanged = false;



            // 如果跟订单上次保存的不一样,就提示未保存提示
            if (oldList.All(ChangePaidPrice.PayModel.Contains) && tempPayList.Count == ChangePaidPrice.PayModel.Count)
            {
            }
            else
            {
                RoomPaidPriceChanged = true;
                tempPayList = ChangePaidPrice.PayModel.Select(x => x.GetImportPay()).ToList();
            }

            if (RoomPaidPriceChanged)
            {


                Calc();

            }
        }




        /// <summary>
        /// 计算
        /// </summary>
        private void Calc(bool OnlyResult = false)
        {




            this.TotalPrice = import.TotalPrice.ToString();

            import.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            import.SupplierPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.SupplierId).Sum(x => x.OriginalPrice), 2);




            this.SupplierPaidPrice = import.SupplierPaidPrice.ToString();
            this.PaidPrice = import.PaidPrice.ToString();


            this.TotalPaidPrice = (import.TotalPaidPrice = Math.Round(import.SupplierPaidPrice + import.PaidPrice, 2)).ToString();





            double balancePrice = Math.Round(import.TotalPaidPrice - import.TotalPrice, 2);

            // 客户给的钱减去原价, 剩余说明 有钱需要退回
            if (balancePrice > 0)
            {
                this.KeepPrice = (import.KeepPrice = balancePrice).ToString();
                this.BorrowPrice = (import.BorrowPrice = 0).ToString();
            }
            else if (balancePrice < 0)
            {
                this.BorrowPrice = (import.BorrowPrice = balancePrice).ToString();
                this.KeepPrice = (import.KeepPrice = 0).ToString();


            }
            else if (balancePrice == 0)
            {
                this.BorrowPrice = (import.BorrowPrice = 0).ToString();
                this.KeepPrice = (import.KeepPrice = 0).ToString();
            }


            if (balancePrice > 0)
                BalanceMode = 1;
            else if (balancePrice < 0)
                BalanceMode = 2;
            else
                BalanceMode = 0;

        }







        private int _balanceMode = 0;
        /// <summary>
        /// 余额模式(0默认,1蓝色2红色)
        /// </summary>
        public int BalanceMode
        {
            get { return _balanceMode; }
            set
            {
                _balanceMode = value;
                OnPropertyChanged("BalanceMode");
            }
        }






        private string _totalPrice;
        /// <summary>
        /// 总价
        /// </summary>
        public string TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged("TotalPrice");
            }
        }




        private string _supplierPaidPrice;
        /// <summary>
        /// 会员支付价
        /// </summary>
        public string SupplierPaidPrice
        {
            get { return _supplierPaidPrice; }
            set
            {
                _supplierPaidPrice = value;
                OnPropertyChanged("SupplierPaidPrice");
            }
        }



        private string _paidPrice;
        /// <summary>
        /// 现金支付价格
        /// </summary>
        public string PaidPrice
        {
            get { return _paidPrice; }
            set
            {
                _paidPrice = value;
                OnPropertyChanged("PaidPrice");

            }
        }






        private string _totalPaidPrice;
        /// <summary>
        /// 总支付价格
        /// </summary>
        public string TotalPaidPrice
        {
            get { return _totalPaidPrice; }
            set
            {
                _totalPaidPrice = value;
                OnPropertyChanged("TotalPaidPrice");
            }
        }







        private string _borrowPrice;
        /// <summary>
        /// 借款价格
        /// </summary>
        public string BorrowPrice
        {
            get { return _borrowPrice; }
            set
            {
                _borrowPrice = value;
                OnPropertyChanged("BorrowPrice");
            }
        }



        private string _keepPrice;
        /// <summary>
        /// 保留价格
        /// </summary>
        public string KeepPrice
        {
            get { return _keepPrice; }
            set
            {
                _keepPrice = value;
                OnPropertyChanged("KeepPrice");
            }
        }








        /// <summary>
        /// 后退按钮
        /// </summary>
        private RelayCommand _backCommand;
        public Command BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                }));
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






        private ChangePaidPriceViewModel _changePaidPrice;
        /// <summary>
        /// 修改支付价格控件
        /// </summary>
        public ChangePaidPriceViewModel ChangePaidPrice
        {
            get { return _changePaidPrice; }
            set
            {
                _changePaidPrice = value;
                OnPropertyChanged("ChangePaidPrice");
            }
        }




        /// <summary>
        /// 结账命令
        /// </summary>
        private RelayCommand _checkoutCommand;
        public Command CheckoutCommand
        {
            get
            {
                return _checkoutCommand ?? (_checkoutCommand = new RelayCommand(param =>
                {

                    // 奇怪, 这个逻辑不是应该丢弃吗? 为什么在这里和TakeoutCheckout里还存在?(也许Order的Checkout也有)
                    if (null != import.tb_supplier && import.tb_supplier.IsAllowBorrow == 0 && import.tb_supplier.BalancePrice < double.Parse(SupplierPaidPrice))
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SupplierBalanceNotEnough"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        return;
                        }



                        //余额不平确认
                        if (import.BorrowPrice != 0 || import.KeepPrice != 0)
                        {

                        //确认取消
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("ConfirmBalanceInjustice"), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                        {
                            if (msg == "NO")
                                return;

                            CheckPayAndCheckout();

                        }, null);
                        }
                        else
                        {
                            CheckPayAndCheckout();
                        }

                    }));
            }
        }


        /// <summary>
        /// 直接输入缺少的金额
        /// </summary>
        public void FinishPaidPrice()
        {

            IsLoading = true;

            Task.Run(async () =>
            {

                await ExtX.WaitForLoading();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                    ChangePaidPrice.InitialView(this.import.TotalPrice, oldList, false, true, 3);

                    double lessPrice = Math.Round(import.TotalPrice - import.TotalPaidPrice, 2);

                    if (lessPrice != 0)
                    {
                        if (lessPrice > 0)
                        {
                            ChangePaidPrice.SetChangePrice(lessPrice, "+");
                        }
                        else if (lessPrice < 0)
                        {
                            ChangePaidPrice.SetChangePrice(Math.Abs(lessPrice), "-");
                        }

                    }


                    IsLoading = false;
                });
            });

            
        }



        /// <summary>
        /// 检查支付并结账
        /// </summary>
        private void CheckPayAndCheckout()
        {
            if (import.PaidPrice == 0 && import.SupplierPaidPrice == 0)
            {
                //确认取消
                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("ConfirmNotPay"), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                {
                    if (msg == "NO")
                        return;

                    Checkout();

                }, null);
            }
            else
            {
                Checkout();
            }
        }


        /// <summary>
        /// 结账
        /// </summary>
        private void Checkout()
        {


            string ErrMsgName, SucMsgName;
            ErrMsgName = SucMsgName = Resources.Instance.GetString("CheckoutImport");

            if (import.Remark != ChangePaidPrice.Remark)
            {
                if (!string.IsNullOrWhiteSpace(ChangePaidPrice.Remark))
                    import.Remark = this.ChangePaidPrice.Remark;
                else
                    import.Remark = null;
            }

            IsLoading = true;

            Task.Factory.StartNew(async () =>
            {
   
                try
                {


                    var taskResult = await OperatesService.Instance.ServiceAddImportWithDetail(import, details, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.ImportId = import.ImportId; return x; }).ToList());

                    ResultModel result = taskResult.resultModel;
                    List<ImportDetail> resultDetails = taskResult.importDetailsAddResult;
                    List <ImportPay> temp = taskResult.importPaysAddResult;

                    long UpdateTime = taskResult.UpdateTime;

                    if (result.Result)
                    {

                        if (null != import.tb_supplier)
                        {
                            Notification.Instance.ActionSupplier(this, import.tb_supplier, null);
                            import.SupplierId = import.tb_supplier.SupplierId;
                        }


                        // 如果成功, 则新增产品
                        foreach (var item in resultDetails)
                        {
                            Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                            if (product.IsBindCount == 1 || item.TotalPrice != item.OriginalTotalPrice || item.SalePrice != item.OriginalSalePrice)
                            {
                                if (product.IsBindCount == 1)
                                    product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);

                                product.UpdateTime = UpdateTime;

                                // 如果总支出价格和应该算出来的价格不一样, 则更改产品支出价格
                                if (item.TotalPrice != item.OriginalTotalPrice)
                                {
                                    product.CostPrice = item.Price;
                                }

                                // 如果产品价格和原始价格不同, 则更改产品价格
                                if (item.SalePrice != item.OriginalSalePrice)
                                {
                                    product.Price = item.SalePrice;
                                }

                                Notification.Instance.ActionProduct(null, product, 2);
                            }
                        }


                    }

                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        if (result.Result)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), SucMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, (msg)=>
                            {
                                NavigationPath.Instance.ImportPage.Init();

                                NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                NavigationPath.Instance.SwitchMasterDetailNavigate(0);


                            }, null);


                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (msg) =>
                                {
                                    NavigationPath.Instance.ImportPage.Init();
                                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                    NavigationPath.Instance.SwitchMasterDetailNavigate(0);


                                }, null);
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenWaitRetry"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }
                            else
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, (msg) =>
                            {
                                if (OperatesService.Instance.IsExpired || OperatesService.Instance.IsAdminUsing)
                                {
                                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                                }
                            }, null);
                        }), false, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName));
                    }));
                }

               

                Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                {
                    IsLoading = false;
                }));
            });
        }

    }
}
