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
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.DAL;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using Oybab.Res.Tools;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using Xamarin.Essentials;
using Oybab.TradingSystemX.VM.DService;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class OrderViewModel : ViewModelBase
    {
        private Page _element;

        private long RoomId;
        private Order order;
        private string RoomStateSession;
        private List<OrderDetail> resultList = null;
        private List<OrderPay> payList = null;
        


        public OrderViewModel(Page _element)
        {
            this._element = _element;

            // 设置物理后退弹出面板
            if (null == NavigationPath.Instance.OrderPanelClose)
                NavigationPath.Instance.OrderPanelClose = ReInitPanels;
            // 设置物理后退关闭该页面
            if (null == NavigationPath.Instance.OrderClose)
                NavigationPath.Instance.OrderClose = Close;

            // 设置重新加载页面(因为ListView的BUG导致的临时修复)
            if (null == NavigationPath.Instance.ReloadOrderProductPage)
                NavigationPath.Instance.ReloadOrderProductPage = ReloadOrderProductPage;
            if (null == NavigationPath.Instance.ReloadOrderSelectPage)
                NavigationPath.Instance.ReloadOrderSelectPage = ReloadOrderSelectPage;

            Notification.Instance.NotificationLanguage += (obj, value, args) => { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { SetCurrentName(); }); };

            // 设置语言
            SetCurrentName();


            // 菜单列表填充一下
            MainLists.Add(new MainListModel() { Name = "Products", GoCommand = this.GoCommand });
            MainLists.Add(new MainListModel() { Name = "Selected", GoCommand = this.GoCommand });
            MainLists.Add(new MainListModel());
            MainLists.Add(new MainListModel() { Name = "Back", GoCommand = this.GoCommand });


            this.Products = new ProductsViewModel(NavigationPath.Instance.ProductPage, NavigationPath.Instance.ProductPage.GetProductTypeListContent(), NavigationPath.Instance.ProductPage.GetProductContent(), NavigationPath.Instance.ProductPage.GetProductTemplate());
            this.Products.ProductChange = ProductChange;
            this.Products.SearchProduct = AddProduct;
            this.Products.DetectProductIsSelected = DetectProductIsSelected;
            this.Products.GoCommand = GoCommand;
            this.Products.Operate = OperateDetails;


            this.Selected = new SelectedViewModel(NavigationPath.Instance.SelectedPage, ReCalc, RecalcPaidPrice, NavigationPath.Instance.SelectedPage.GetSelectedContent(), NavigationPath.Instance.SelectedPage.GetSelectedTemplate(), NavigationPath.Instance.SelectedPage.GetRequestContent(), NavigationPath.Instance.SelectedPage.GetRequestTemplate(), NavigationPath.Instance.SelectedPage.GetBalanceContent(), NavigationPath.Instance.SelectedPage.GetBalanceTemplate());
            this.Selected.Save = Save;
            this.Selected.Checkout = Checkout;
            this.Selected.RefreshTime = RefreshTime;
            this.Selected.GoCommand = GoCommand;
            this.Selected.RefreshCommand = RefreshCommand;



            NavigationPath.Instance.SelectedPage.BindingContext = this.Selected;
            NavigationPath.Instance.ProductPage.BindingContext = this.Products;



            // 订单更新
            Notification.Instance.NotificateSendFromServer += (obj, value, args) => { if (null == args) RefreshSome(new List<long>() { value }); };
            Notification.Instance.NotificateSendsFromServer += (obj, value, args) => { RefreshSome(value); };


        }



       
        

        /// <summary>
        /// 重新加载产品页
        /// </summary>
        private void ReloadOrderProductPage() {
            NavigationPath.Instance.ProductPage = new ProductsPage();
            Products.ReLoadProductsViewModel(NavigationPath.Instance.ProductPage, NavigationPath.Instance.ProductPage.GetProductTypeListContent());
            NavigationPath.Instance.ProductPage.BindingContext = this.Products;

            NavigationPath.Instance.ProductNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.ProductPage);

            Products.ReloadList();
        }

        /// <summary>
        /// 重新加载已选页
        /// </summary>
        private void ReloadOrderSelectPage()
        {
            NavigationPath.Instance.SelectedPage = new SelectedPage();
            Selected.ReloadSelectedViewModel(NavigationPath.Instance.SelectedPage);
            NavigationPath.Instance.SelectedPage.BindingContext = this.Selected;

            NavigationPath.Instance.SelectedNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.SelectedPage);

            Selected.ReloadList();
        }



        private ObservableCollection<MainListModel> _mainLists = new ObservableCollection<MainListModel>();
        /// <summary>
        /// 所有菜单项
        /// </summary>
        public ObservableCollection<MainListModel> MainLists
        {
            get { return _mainLists; }
            set
            {
                _mainLists = value;
                OnPropertyChanged("MainLists");
            }
        }


        private object _selectedMainItem;
        /// <summary>
        /// 选择的菜单项
        /// </summary>
        public object SelectedMainItem
        {
            get { return _selectedMainItem; }
            set
            {
                _selectedMainItem = value;
                OnPropertyChanged("SelectedMainItem");
            }
        }



        private bool _isPresented;
        /// <summary>
        /// 是否展开字母菜单
        /// </summary>
        public bool IsPresented
        {
            get { return _isPresented; }
            set
            {
                _isPresented = value;
                OnPropertyChanged("IsPresented");
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



        private string _roomNo;
        /// <summary>
        /// 雅座编号
        /// </summary>
        public string RoomNo
        {
            get { return _roomNo; }
            set
            {
                _roomNo = value;
                OnPropertyChanged("RoomNo");
            }
        }


        /// <summary>
        /// 设置语言
        /// </summary>
        private void SetCurrentName()
        {

            foreach (var item in MainLists)
            {
                item.LangChange();
            }
        }




        /// <summary>
        /// 重新计算支付金额
        /// </summary>
        private void RecalcPaidPrice()
        {
            if (null != order && this.Selected.ChangePaidPriceView.Remark != this.order.Remark && !(this.Selected.ChangePaidPriceView.Remark == "" && this.order.Remark == null))
            {
                this.Selected.RemarkChanged = true;
            }

            // 如果跟订单上次保存的不一样,就提示未保存提示
            if (this.Selected.oldList.All(this.Selected.ChangePaidPriceView.PayModel.Contains) && this.Selected.tempPayList.Count == this.Selected.ChangePaidPriceView.PayModel.Count)
            {
            }
            else
            {
                this.Selected.RoomPaidPriceChanged = true;
                this.Selected.tempPayList = this.Selected.ChangePaidPriceView.PayModel.Select(x => x.GetOrderPay()).ToList();
            }

            if (this.Selected.RoomPaidPriceChanged || this.Selected.RemarkChanged)
            {


                RefreshTime();

            }
        }



        /// <summary>
        /// 跳转
        /// </summary>
        private RelayCommand _goCommand;
        public Command GoCommand
        {
            get
            {
                return _goCommand ?? (_goCommand = new RelayCommand(param =>
                {

                    string model = param as string;//param as MainListModel;
                    if (null != model)
                    {
                        switch (model)
                        {
                            case "Products":
                            case "Products1":
                                IsLoading = true;
                                if (model == "Products1")
                                    this.Selected.IsLoading = true;
                                Task.Run(async () =>
                                {

                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.SwitchMasterDetailNavigate(0);
                                        

                                        IsPresented = false;
                                        IsLoading = false;
                                        if (model == "Products1")
                                            this.Selected.IsLoading = false;
                                    });
                                });
                                break;
                            case "Selected":
                            case "Selected1":
                                IsLoading = true;

                                if (model == "Selected1")
                                    this.Products.IsLoading = true;
                                Task.Run(async () =>
                                {

                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        ReInitPanels();

                                        NavigationPath.Instance.SwitchMasterDetailNavigate(1);


                                        var keyBoardHelper = DependencyService.Get<IKeyboardHelper>();
                                        keyBoardHelper?.HideKeyboard();

                                        IsPresented = false;
                                        IsLoading = false;
                                        if (model == "Selected1")
                                            this.Products.IsLoading = false;
                                    });
                                });
                                break;
                            case "ReplaceRoom":

                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.ReplaceRoomPage.Init(new RoomStateModel() { RoomId = this.RoomId, RoomNo = this.RoomNo });
                                        ((OrderPage)this._element).Detail = NavigationPath.Instance.ReplaceRoomPage;
                                        IsPresented = false;
                                        IsLoading = false;
                                    });
                                });
                                break;
                            case "CancelOrder":

                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("ConfirmCancelOrder"), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                                {

                                    if (msg == "NO")
                                        return;


                                    RoomModel currentModel = Resources.Instance.RoomsModel.Where(x => x.RoomId == this.RoomId).FirstOrDefault();


                                    if (null == currentModel.PayOrder)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                        Notification.Instance.ActionGetsFromService(null, new List<long>() { currentModel.RoomId }, null);

                                        Close();
                                        return;
                                    }

                                    Order cancelOrder = currentModel.PayOrder.FastCopy();
                                    cancelOrder.State = 2;

                                    string newRoomSessionId;
                                    string ErrMsgName, SucMsgName;
                                    ErrMsgName = SucMsgName = Resources.Instance.GetString("CancelOrder");


                                    IsLoading = true;

                                    Task.Factory.StartNew(async () =>
                                    {
                                        try
                                        {
                                            await ExtX.WaitForLoading();

                                          

                                            var testResult = await OperatesService.Instance.ServiceEditOrder(cancelOrder, null, null, currentModel.OrderSession, false);
                                            ResultModel result = testResult.resultModel;
                                            long UpdateTime = testResult.UpdateTime;
                                            newRoomSessionId = testResult.newRoomStateSession;


                                            if (result.Result)
                                            {
                                                if (null != currentModel.PayOrder.tb_orderdetail)
                                                {
                                                    foreach (var item in currentModel.PayOrder.tb_orderdetail)
                                                    {
                                                        if (item.State != 1 && item.State != 3)
                                                        {
                                                            Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                                            if (null != product && product.IsBindCount == 1)
                                                            {
                                                                product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);
                                                                product.UpdateTime = UpdateTime;

                                                                Notification.Instance.ActionProduct(null, product, 2);
                                                            }
                                                        }
                                                    }
                                                }


                                                if (null != currentModel.PayOrder.tb_orderpay)
                                                {
                                                    foreach (var item in currentModel.PayOrder.tb_orderpay)
                                                    {
                                                        if (null != item.MemberId)
                                                        {
                                                            Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                                            item.MemberId = item.tb_member.MemberId;
                                                        }
                                                    }
                                                }
                                            }

                                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                            {
                                                if (result.Result)
                                                {

                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), SucMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);

                                                    currentModel.PayOrder = null;
                                                    currentModel.OrderSession = newRoomSessionId;
                                                    if (Resources.Instance.CallNotifications.ContainsKey(currentModel.RoomId))
                                                        Resources.Instance.CallNotifications.Remove(currentModel.RoomId);


                                                    RoomListViewModel viewModel = NavigationPath.Instance.RoomListPage.BindingContext as RoomListViewModel;
                                                    viewModel.Init((long)-1);



                                                }
                                                else
                                                {
                                                    if (result.IsRefreshSessionModel)
                                                    {
                                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                                        Notification.Instance.ActionGetsFromService(null, new List<long>() { currentModel.RoomId }, null);
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

                                                Close();
                                            }));
                                        }
                                        catch (Exception ex)
                                        {
                                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                            {
                                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                                                }), false, string.Format(Resources.Instance.GetString("OperateFaild"), ErrMsgName));
                                            }));

                                        }
                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                        {
                                            IsLoading = false;
                                        }));
                                    });
                                }, null);


                                break;
                            case "Back":
                                Close();
                                break;
                            case "Lang":
                                ChangeOrderLanguageCommand.Execute(null);
                                break;
                            case "Barcode":


                               
                                this.Products.IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                   

                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => 
                                    {
                                        bool RequestCamera = await Common.Instance.CheckCamera();

                                        if (!RequestCamera)
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, Resources.Instance.GetString("PermissionDenied"), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);
                                            this.Products.IsLoading = false;
                                            return;
                                        }



                                        var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
                                        options.PossibleFormats = new List<ZXing.BarcodeFormat>() { ZXing.BarcodeFormat.EAN_8, ZXing.BarcodeFormat.EAN_13, ZXing.BarcodeFormat.QR_CODE };

                                         var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                                        
                                        scanner.CancelButtonText = Resources.Instance.GetString("Back");

                                        //var result = await scanner.Scan(options);

                                        //if (result != null)
                                        //{
                                        //    AddProduct(result.Text, true);
                                        //}
                                        options.DelayBetweenContinuousScans = 2000;

                                        DateTime LastTime = DateTime.MinValue;
                                        scanner.ScanContinuously(options, async x =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(x.Text) && (DateTime.Now - LastTime).TotalSeconds >= 1)
                                            {
                                                LastTime = DateTime.MaxValue;


                                                if (x.BarcodeFormat == ZXing.BarcodeFormat.QR_CODE)
                                                {
                                                    await Common.Instance.CheckMemberBalance(x.Text, ()=>
                                                    {
                                                        LastTime = DateTime.Now;
                                                    }, (member)=>
                                                    {

                                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                                        {
                                                            scanner.Cancel();

                                                            // 打开会员修改面板

                                                            if (null == NavigationPath.Instance.NewMemberPage)
                                                                NavigationPath.Instance.NewMemberPage = new NewMemberPage();


                                                            NavigationPath.Instance.NewMemberPage.Init(member);
                                                            // 跳转到结账页面
                                                            NavigationPath.Instance.GoMasterDetailsNavigateNext(NavigationPath.Instance.NewMemberPage, true, true);
                                                        }));
                                                    });
                                                }
                                                else
                                                {
                                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                                    {
                                                        AddProduct(x.Text, true);
                                                        LastTime = DateTime.Now;
                                                    }));
                                                }
                                                 
                                            }
                                        });

                                        this.Products.IsLoading = false;
                                    });
                                });
                                break;
                            default:
                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        SelectedMainItem = null;
                                        IsLoading = false;
                                    });
                                });
                                break;
                        }


                    }


                }));
            }
        }

        /// <summary>
        /// 重置一下弹出面板
        /// </summary>
        private void ReInitPanels()
        {
            if (Selected.RequestView.IsShow)
                Selected.RequestView.IsShow = false;
            if (Selected.ChangeCountView.IsShow)
                Selected.ChangeCountView.IsShow = false;
            if (Selected.ChangePriceView.IsShow)
                Selected.ChangePriceView.IsShow = false;
            if (Selected.ChangeTimeView.IsShow)
                Selected.ChangeTimeView.IsShow = false;
            if (Selected.ChangePaidPriceView.IsShow)
                Selected.ChangePaidPriceView.IsShow = false;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        private void Close()
        {
            IsLoading = true;
            IsPresented = false;

            Task.Run(async () =>
            {

                
                await ExtX.WaitForLoading();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    NavigationPath.Instance.SwitchNavigate(1);
                    IsLoading = false;
                });
            });
        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object roomId, Action<int> IsFirst = null)
        {


            this.RoomId = (long)roomId;
            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == this.RoomId).FirstOrDefault();

            Room room = Resources.Instance.Rooms.Where(x => x.RoomId == this.RoomId).FirstOrDefault();


            Selected.IsRefresh = false;

            // 显示所有产品
            this.order = model.PayOrder;
            this.resultList = (null == model.PayOrder ? null : model.PayOrder.tb_orderdetail.ToList());
            this.RoomStateSession = model.OrderSession;
            this.payList = (null == model.PayOrder ? null : model.PayOrder.tb_orderpay.ToList());

            if (null == payList)
            {
                payList = new List<OrderPay>();
            }
            this.Selected.tempPayList = payList.ToList();

            Selected.RoomId = model.RoomId;
            Selected.RoomNo = RoomNo = model.RoomNo;
            Selected.AllowPaid = true; //只是更新它以达到刷新作用

            Selected.RoomType = (int)room.IsPayByTime;

            if (!string.IsNullOrWhiteSpace(Products.SearchKey))
                Products.SearchKey = "";


            DateTime now = DateTime.Now;

            if (null == order)
            {

                // 初始化参数
                DisplayMode = 1;

                Selected.GoCommand = null;
                // 语言默认, 或者上次选择
                if (Resources.Instance.DefaultOrderLang == -1)
                {
                     Selected.LanguageMode = Res.Instance.MainLang.LangIndex;
                }
                else
                {
                    Selected.LanguageMode = Resources.Instance.DefaultOrderLang;
                }
                Selected.GoCommand = GoCommand ;

                Selected.LanguageEnable = true;
                Selected.StartTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));


                // 新打开的订单直接显示包厢价格
                Selected.RoomPrice = CommonOperates.Instance.GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, 0, false, null);

                Selected.TotalPrice = 0;
                Selected.PaidPrice = 0;


                Selected.ChangePaidPriceView.Remark = "";
                Selected.TempUnlimitedTime = false;

            }
            else
            {
                // 初始化参数

                DisplayMode = 2;
                Selected.GoCommand = null;
                Selected.LanguageMode = Res.Instance.GetLangByLangIndex((int)order.Lang).LangIndex;
                Selected.GoCommand = GoCommand;


                Selected.RoomPrice = this.order.RoomPrice;

                Selected.TotalPrice = this.order.TotalPrice;
                Selected.PaidPrice = this.order.TotalPaidPrice;


                Selected.ChangePaidPriceView.Remark = order.Remark;
                Selected.TempUnlimitedTime = (order.IsFreeRoomPrice == 2 ? true : false);

                if (Common.Instance.IsAllowChangeLanguage())
                {
                    Selected.LanguageEnable = true;
                }
                else
                {
                    Selected.LanguageEnable = false;
                }
            }


            // 设置时间相关

            if (room.IsPayByTime == 0)
            {

            }

            if (null == order)
            {

                if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                {
                    Selected.StartTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));
                    Selected.EndTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));
                    Selected.RoomTime = "0:0";
                }
                else
                {
                    Selected.EndTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));
                    Selected.RoomTime = "";
                }


            }
            else
            {
                if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                {
                    Selected.EndTimeTemp = order.EndTime.Value;

                    // 设置剩余时间

                    TimeSpan balance = (DateTime.Now - DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                    // 如果剩余时间已经超出了, 默认0:0显示
                    if (DateTime.Now < DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture))
                    {
                        if (room.IsPayByTime == 1)
                            Selected.RoomTime = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                        else if (room.IsPayByTime == 2)
                            Selected.RoomTime = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                    }
                    else
                        Selected.RoomTime = "0:0";
                }
                else
                {
                    Selected.EndTimeTemp = order.AddTime;
                    Selected.RoomTime = "";
                }

            }


  
            Selected.RoomDisplay = true;
            Selected.RoomTimeChange = false;
            Selected.RoomPaidPriceChanged = false;
            Selected.RemarkChanged = false;




            Selected.ClearList();



            if (null != resultList)
            {

                foreach (var item in resultList.OrderBy(x => x.OrderDetailId))
                {
                    Selected.AddList(new DetailsModel() { AddTime = DateTime.ParseExact(item.AddTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm"), Product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault(), OrderDetail = item, Operate = OperateDetails, IsOnlyOrder = true });
                }

            }

            // 刷新初始化页面和总额
            if (Selected.CurrentSelectedList.Count > 0)
            {

                DisplayMode = 2;

            }

            RefreshState();

            // 刷新产品
            Products.Init(this.RoomId);


            // 刷新状态
            OnPropertyChanged("IsExists");
            OnPropertyChanged("IsRoleIn");
            OnPropertyChanged("IsReplaceRoom");
            OnPropertyChanged("IsCancelOrder");
            

            // 重置面板
            ReInitPanels();


            // 第一次时初始化页面子页面
            if (null != IsFirst)
            {
                int navigationIndex = 0;
                if (DisplayMode == 2)
                    navigationIndex = 1;

                IsFirst(navigationIndex);


                IsPresented = false;
            }



        }



        /// <summary>
        /// 操作订单详情(0新增1删除,2刷新)
        /// </summary>
        /// <param name="IsAdd"></param>
        /// <param name="details"></param>
        private void OperateDetails(int mode, DetailsModel details)
        {
            if (mode == 0)
                Selected.AddListToFirst(details);
            else if (mode == 1)
            {
                Selected.RemoveSelected(details);
                ProductStateModel productStateModel = Products.ProductList.Where(x => x.Product == details.Product).FirstOrDefault();
                if (null != productStateModel && productStateModel.IsSelected)
                {
                    productStateModel.IsSelected = false;
                    productStateModel.DetailsModel = null;
                }
            }
            else if (mode == 2)
            {
                Delete(details.OrderDetail.OrderDetailId);
            }

            RefreshState();
        }




        private void AddProduct(string key)
        {
            AddProduct(key, false);
        }


        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="key"></param>
        private void AddProduct(string key, bool IsScan)
        {
            List<Product> productList = new List<Product>();

            long n;
            bool isNumeric = long.TryParse(key, out n);

            if (isNumeric)
            {
                // 获取条码产品
                List<Product> selectedBarcode = Resources.Instance.Products.Where(x => (x.Barcode == key || (x.IsScales == 1 && key.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
                if (null != selectedBarcode && selectedBarcode.Count > 0)
                    productList.AddRange(selectedBarcode);

                // 扫描条码, 并只有一个则选中
                if (IsScan && productList.Count == 1)
                {


                    Product CurrentProduct = productList.FirstOrDefault();

                    // 如果找到了
                    if (null != CurrentProduct)
                    {
                        DetailsModel details = Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == CurrentProduct && x.OrderDetail.OrderDetailId <= 0).FirstOrDefault();
                        // 已存在就增加1个(暂时改为提醒已存在)
                        if (null != details)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("CurrentProductAlreadyExists"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        // 不存在新加一个
                        else
                        {
                            if (CurrentProduct.IsScales == 1)
                                ProductChange(true, CurrentProduct.ProductId, double.Parse(key.Substring(7, 2) + "." + key.Substring(9, 3)));
                            else
                                ProductChange(true, CurrentProduct.ProductId, 1);
                            
                            string name = "";
                            if (Res.Instance.MainLangIndex == 0)
                                name = CurrentProduct.ProductName0;
                            else if (Res.Instance.MainLangIndex == 1)
                                name = CurrentProduct.ProductName1;
                            else if (Res.Instance.MainLangIndex == 2)
                                name = CurrentProduct.ProductName2;

                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, name, MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);

                        }

                    }

                }
            }
            else
            {

                key = key.ToUpper();

                var proList = Resources.Instance.Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId);

                productList = proList.Where(x => x.ProductName0.Contains(key) || x.ProductName1.Contains(key) || x.ProductName2.Contains(key)).ToList();

            }



            // 刷新产品
            Products.RefreshProduct(productList);



        }




        /// <summary>
        /// 产品修改
        /// </summary>
        /// <param name="IsSelected"></param>
        /// <param name="productId"></param>
        private DetailsModel ProductChange(bool IsSelected, long productId)
        {
            return ProductChange(IsSelected, productId, 1);
        }
        /// <summary>
        /// 产品修改
        /// </summary>
        /// <param name="IsSelected"></param>
        /// <param name="productId"></param>
        private DetailsModel ProductChange(bool IsSelected, long productId, double count)
        {
            DetailsModel model = null;
            Product product = Resources.Instance.Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (!IsSelected)
            {
                Selected.RemoveSelected(Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == product).FirstOrDefault());
            }
            else
            {
                model = new DetailsModel() { IsNew = true, IsTakeout = false, AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Product = product, OrderDetail = new OrderDetail() { OrderDetailId = -1, ProductId = product.ProductId, Count = count, AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), Price = product.Price, State = 0, TotalPrice = product.Price }, Operate = OperateDetails, IsOnlyOrder = true };
                Selected.AddListToFirst(model);
            }

            RefreshState();


            return model;
        }



        /// <summary>
        /// 刷新一些控件(每次产品或数量变动)
        /// </summary>
        private void RefreshState()
        {

           
            Order tempOrder;
            List<OrderDetail> details;
            Calc(out details, out tempOrder, true);

            // 保存按钮
            if (Selected.RemarkChanged || Selected.RoomPaidPriceChanged || Selected.RoomTimeChange || Selected.CurrentSelectedList.Any(x => x.IsNew))
            {
                Selected.SaveMode = 1;

                if (Selected.RoomTimeChange)
                {

                }
            }
            else
            {
                if (null == this.order)
                    Selected.SaveMode = 0;
                else
                {
                    if (Common.Instance.IsIncomeTradingManage())
                        Selected.SaveMode = 2;
                    else
                        Selected.SaveMode = 0;
                }
            }

        }





        /// <summary>
        /// 刷新时间
        /// </summary>
        private void RefreshTime()
        {
            // 设置剩余时间

            TimeSpan balance = (DateTime.Now - DateTime.ParseExact(Selected.EndTimeTemp.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
            // 如果剩余时间已经超出了, 默认0:0显示
            if (DateTime.Now < DateTime.ParseExact(Selected.EndTimeTemp.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture))
            {
                if (Selected.RoomType == 1)
                    Selected.RoomTime = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                else if (Selected.RoomType == 2)
                    Selected.RoomTime = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
            }
            else
                Selected.RoomTime = "0:0";


            RefreshState();

        }

        /// <summary>
        /// 检测新产品是否已选中
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        private DetailsModel DetectProductIsSelected(Product product)
        {
            return Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == product && x.OrderDetail.OrderDetailId <= 0).FirstOrDefault();
        }


        /// <summary>
        /// 保存
        /// </summary>
        private void Save()
        {
            if (null == order)
                Operate(0);
            else
                Operate(1);
        }

        /// <summary>
        /// 结账
        /// </summary>
        private void Checkout()
        {
            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

            if (null == model.PayOrder)
            {
                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, (msg) =>
                {
                    RoomListViewModel viewModel = NavigationPath.Instance.CurrentPage.BindingContext as RoomListViewModel;
                    viewModel.Init(this.RoomId);


                    Close();

                }, null);



            }

            else
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    NavigationPath.Instance.CheckoutPage.Init(model);
                    // 跳转到结账页面
                    NavigationPath.Instance.GoMasterDetailsNavigateNext(NavigationPath.Instance.CheckoutPage, true, true);
                });
            }
        }




        /// <summary>
        /// 重新计算
        /// </summary>
        private void ReCalc()
        {
            Order order;
            List<OrderDetail> details;
            Calc(out details, out order);
        }



        private double lastTotal = 0;
        private double lastOriginalTotalPrice = 0;



        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="details"></param>
        /// <param name="import"></param>
        /// <param name="IgnoreError"></param>
        /// <param name="OnlyTotal"></param>
        private void Calc(out List<OrderDetail> details, out Order order, bool IgnoreError = true, bool OnlyTotal = false, bool IgnoreNotConfirm = true, bool IgnoreCanceld = true, long IgnoreCancelId = -999)
        {
            details = new List<OrderDetail>();
            List<OrderDetail> detailsAll = new List<OrderDetail>();
            order = new Order();

            if (!OnlyTotal)
            {
                foreach (var item in Selected.CurrentSelectedList)
                {
                    OrderDetail orderDetails = new OrderDetail();
                    orderDetails.ProductId = item.Product.ProductId;
                    orderDetails.IsPack = item.OrderDetail.IsPack;
                    orderDetails.Count = item.OrderDetail.Count;
                    if (item.NewPrice.HasValue)
                        orderDetails.Price = item.NewPrice.Value;
                    else
                        orderDetails.Price = item.OrderDetail.Price;
                    orderDetails.TotalPrice = item.TotalPrice;
                    orderDetails.OriginalTotalPrice = Math.Round(item.OrderDetail.Price * item.OrderDetail.Count);
                    orderDetails.TotalCostPrice = Math.Round(item.Product.CostPrice * item.OrderDetail.Count);
                    if (item.Product.CostPrice == 0 && null != item.Product.ProductParentId)
                    {
                        Product parentProduct = Resources.Instance.Products.FirstOrDefault(x => x.ProductId == item.Product.ProductParentId);

                        if (null != parentProduct)
                        {
                            double price = Math.Round(parentProduct.CostPrice / item.Product.ProductParentCount, 2);
                            orderDetails.TotalCostPrice = Math.Round(price * orderDetails.Count, 2);
                        }
                    }

                    orderDetails.OrderDetailId = item.OrderDetail.OrderDetailId;
                    orderDetails.State = item.OrderDetail.State;
                    orderDetails.Request = item.OrderDetail.Request;


                    if (item.IsNew)
                        details.Add(orderDetails);
                    detailsAll.Add(orderDetails);




                }

            }


            IEnumerable<OrderDetail> totalDetails = detailsAll;

            if (IgnoreNotConfirm)
                totalDetails = totalDetails.Where(x => x.State != 1);
            if (IgnoreCanceld)
                totalDetails = totalDetails.Where(x => x.State != 3);

            lastTotal = Math.Round(totalDetails.Sum(x => x.TotalPrice), 2);
            lastOriginalTotalPrice = Math.Round(totalDetails.Sum(x => x.OriginalTotalPrice), 2);

            if (IgnoreCancelId != -999 && totalDetails.Where(x => x.OrderDetailId == IgnoreCancelId).Count() > 0)
            {
                lastTotal = Math.Round(lastTotal - totalDetails.Where(x => x.OrderDetailId == IgnoreCancelId).FirstOrDefault().TotalPrice, 2);
                lastOriginalTotalPrice = Math.Round(lastOriginalTotalPrice - totalDetails.Where(x => x.OrderDetailId == IgnoreCancelId).FirstOrDefault().OriginalTotalPrice, 2);
            }



            if (Selected.RoomType != 0)
                order.EndTime = Selected.EndTimeTemp;



            order.RoomId = this.RoomId;
            if (this.order != null)
            {
                order.OrderId = this.order.OrderId;

                order.AdminId = this.order.AdminId;
                order.DeviceId = this.order.DeviceId;
                order.AddTime = this.order.AddTime;
                order.UpdateTime = this.order.UpdateTime;
                order.RoomPriceCalcTime = this.order.RoomPriceCalcTime;
                order.Request = this.order.Request;
                order.PrintCount = this.order.PrintCount;
                order.Mode = this.order.Mode;
                order.MemberPaidPrice = this.order.MemberPaidPrice;
                order.MemberId = this.order.MemberId;
                order.IsAutoPay = this.order.IsAutoPay;
                order.BorrowPrice = this.order.BorrowPrice;
                order.Lang = this.order.Lang;
                order.IsPayByTime = this.order.IsPayByTime;
                order.IsFreeRoomPrice = this.order.IsFreeRoomPrice;
                order.IsPack = this.order.IsPack;
                order.StartTime = this.order.StartTime;
                order.State = this.order.State;
                order.ReCheckedCount = this.order.ReCheckedCount;

                // 是否允许更改语言
                if (Common.Instance.IsAllowChangeLanguage())
                {
                    order.Lang = Selected.LanguageMode;
                }

            }
            else
            {
                order.Lang = Selected.LanguageMode;
                order.IsPack = 0;
                order.StartTime = Selected.StartTimeTemp;
                order.IsFreeRoomPrice = (Selected.TempUnlimitedTime ? 2 : 0);
            }



            Room room = Resources.Instance.Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();

            int totalMinutes = 0;
            bool IsSubTime = false;

            if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
            {
                // 直接计算时间
                if (null == this.order)
                    totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
                else
                {
                    // 如果是时间少了, 则还是老方法计算
                    if (DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture) < DateTime.ParseExact(order.RoomPriceCalcTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture))
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
                        IsSubTime = true;
                    }
                    // 如果不是, 则在上次的时间加上新时间价格
                    else
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(order.RoomPriceCalcTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
                    }

                }
            }



            Selected.RoomPrice = order.RoomPrice = CommonOperates.Instance.GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, totalMinutes, IsSubTime, order.EndTime);
            Selected.TotalPrice = order.TotalPrice = Math.Round(lastTotal + order.RoomPrice, 2);
            order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice + order.RoomPrice, 2);


            // 注入雅座消费类型
            order.IsPayByTime = room.IsPayByTime;

            // 如果超出最低消费,则清空雅座费
            if ((Selected.TempUnlimitedTime) || (room.FreeRoomPriceLimit > 0 && lastTotal >= room.FreeRoomPriceLimit))
            {
                Selected.RoomPrice = order.RoomPrice = 0;
                order.IsFreeRoomPrice = 1;

                if (Selected.TempUnlimitedTime)
                {
                    order.IsFreeRoomPrice = 2;
                }

                Selected.TotalPrice = order.TotalPrice = Math.Round(lastTotal, 2);
                order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice, 2);
                
            }
            else
            {
                // 如果之前是免去了包厢费,现在需要去掉, 则重新计算房费
                if (order.IsFreeRoomPrice == 1)
                {
                    if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture)).TotalMinutes;
                    }
                    Selected.RoomPrice = order.RoomPrice = CommonOperates.Instance.GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, totalMinutes, IsSubTime, order.EndTime, true);
                    Selected.TotalPrice = order.TotalPrice = Math.Round(lastTotal + order.RoomPrice, 2);
                    order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice + order.RoomPrice, 2);
                }

                order.IsFreeRoomPrice = 0;
            }


            double balancePrice = 0;


            order.PaidPrice = Math.Round(this.Selected.tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            order.MemberPaidPrice = Math.Round(this.Selected.tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);

            order.TotalPaidPrice = Math.Round(order.MemberPaidPrice + order.PaidPrice, 2);

            Selected.PaidPrice = order.TotalPaidPrice;

            
            balancePrice = Math.Round(order.TotalPaidPrice - order.TotalPrice, 2);
            Selected.BalancePrice = balancePrice.ToString();

            if (balancePrice > 0)
            {
                order.KeepPrice = balancePrice;
                order.BorrowPrice = 0;
                Selected.BalanceMode = 1;
            }

            else if (balancePrice < 0)
            {
                order.BorrowPrice = balancePrice;
                order.KeepPrice = 0;
                Selected.BalanceMode = 2;
            }

            else if (balancePrice == 0)
            {
                order.BorrowPrice = 0;
                order.KeepPrice = 0;
                Selected.BalanceMode = 0;
            }

            if (OnlyTotal)
                return;



                if (string.IsNullOrWhiteSpace(this.Selected.ChangePaidPriceView.Remark))
                    order.Remark = null;
                else
                    order.Remark = this.Selected.ChangePaidPriceView.Remark;

            if (room.IsPayByTime == 0)
            {
                order.EndTime = null;
                order.StartTime = null;
            }

        }





        /// <summary>
        /// 操作: 0新建, 1保存, 2确认, 3结账, 4取消
        /// </summary>
        private void Operate(int mode)
        {
            Order order;
            List<OrderDetail> details;

            bool IgnoreNotConfirm = true;
            Calc(out details, out order, false, false, IgnoreNotConfirm);


            if (null == this.order && Resources.Instance.RoomsModel.Where(x => null != x.PayOrder).Count() >= Resources.Instance.RoomCount)
            {
                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("Exception_RoomCountOutOfLimit"), ""), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                return;
            }



            ResultModel result = null;
            bool IsRetry = false;


            IsLoading = true;
            if (mode == 0 || mode == 1 || mode == 2 || mode == 4)
                Selected.IsLoading = true;

            Task.Factory.StartNew(async () =>
            {
                await ExtX.WaitForLoading();

                string successMsgName = "";
                string faildMsgName = "";
                try
                {


                    List<OrderDetail> resultDetails = null;
                    List<OrderPay> resultPays = null;
                    string newRoomSession = null;

                    // 如果不是新订单, 先分别获取一下用于保存和确认的信息
                    List<OrderDetail> orderDetailsAdd = new List<OrderDetail>();
                    List<OrderDetail> orderDetailsEdit = new List<OrderDetail>();
                    List<OrderDetail> orderDetailsConfirm = new List<OrderDetail>();

                    // 倒序保存,最后一个才能顶部显示
                    if (null != details)
                        details.Reverse();


                    if (mode != 0) // null != this.order
                    {
                        foreach (var item in details)
                        {
                            OrderDetail odt = new OrderDetail();

                            odt.Price = item.Price;
                            odt.ProductId = item.ProductId;
                            odt.Count = item.Count;
                            odt.State = item.State;
                            odt.OrderDetailId = item.OrderDetailId;
                            odt.TotalPrice = item.TotalPrice;
                            odt.OriginalTotalPrice = item.OriginalTotalPrice;
                            odt.IsPack = item.IsPack;
                            odt.Request = item.Request;

                            OrderDetail old = resultList.Where(x => x.OrderDetailId == odt.OrderDetailId).FirstOrDefault();
                            if (null != old)
                            {
                                odt.AddTime = old.AddTime;
                                odt.OrderId = old.OrderId;
                                odt.AdminId = old.AdminId;
                                odt.DeviceId = old.DeviceId;

                                odt.Mode = old.Mode;
                                odt.PrintCount = old.PrintCount;
                                odt.Request = old.Request;
                                odt.UpdateTime = item.UpdateTime;

                                item.ConfirmAdminId = item.ConfirmAdminId;
                                item.ConfirmDeviceId = item.ConfirmDeviceId;
                                item.ConfirmTime = item.ConfirmTime;
                                item.Remark = item.Remark;
                            }
                            else if (null != this.order)
                            {
                                odt.OrderId = this.order.OrderId;
                            }

                            if (odt.State == 1)
                            {
                                odt.State = 2;
                                orderDetailsConfirm.Add(odt);
                            }
                            else
                            {
                                if (odt.OrderDetailId == -1)
                                    orderDetailsAdd.Add(odt);
                                else
                                    orderDetailsEdit.Add(odt);
                            }
                        }
                    }

                    // 如果是开张
                    if (mode == 0) 
                    {



                        faildMsgName = successMsgName = Res.Instance.GetString("Save");


                        long UpdateTime;

                        var taskResult = await OperatesService.Instance.ServiceAddOrder(order, details, this.Selected.tempPayList.Where(x => x.AddTime == 0).ToList(), RoomStateSession);
                        result = taskResult.resultModel;
                        resultDetails = taskResult.orderDetailsResult;
                        resultPays = taskResult.orderPaysResult;
                        newRoomSession = taskResult.newRoomStateSession;
                        UpdateTime = taskResult.UpdateTime;




                        if (result.Result)
                        {
                            Resources.Instance.DefaultOrderLang = (int)order.Lang;
                            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;
                            model.PayOrder.tb_orderdetail = this.resultList = resultDetails;
                            model.PayOrder.tb_orderpay = this.payList = resultPays;
                            this.Selected.tempPayList = payList.ToList();
                            model.OrderSession = this.RoomStateSession = newRoomSession;

                            // 新增部分去掉现有数量
                            foreach (var item in resultDetails)
                            {
                                Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {

                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.Instance.Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                            if (null != productParent && productParent.IsBindCount == 1)
                                            {
                                                double ParentRemove = 0;
                                                double ProductAdd = 0;


                                                double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                                ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                                ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 


                                                // 从父级中去掉
                                                productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                productParent.UpdateTime = UpdateTime;


                                                // 给产品增加零的
                                                product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                            }
                                        }
                                    }

                                    product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                    product.UpdateTime = UpdateTime;

                                    Notification.Instance.ActionProduct(null, product, 2);
                                }
                            }


                            foreach (var item in resultPays)
                            {
                                if (null != item.MemberId)
                                {
                                    Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                    item.MemberId = item.tb_member.MemberId;
                                }
                            }
                        }

                    }
                    // 如果是确认
                    else if (mode == 2)
                    {
                        faildMsgName = successMsgName = Res.Instance.GetString("Confirm");




                        List<OrderDetail> orderDetailsAddResult;
                        List<OrderDetail> orderDetailsEditResult;
                        List<OrderDetail> orderDetailsConfirmResult;


                        long UpdateTime;

                        var taskResult = await OperatesService.Instance.ServiceSaveOrderDetail(order, null, null, null, orderDetailsConfirm, RoomStateSession);
                        result = taskResult.resultModel;
                        orderDetailsAddResult = taskResult.orderDetailsAddResult;
                        List<OrderPay> temp = taskResult.orderPaysAddResult;
                        orderDetailsEditResult = taskResult.orderDetailsEditResult;
                        orderDetailsConfirmResult = taskResult.orderDetailsConfirmResult;
                        newRoomSession = taskResult.newRoomStateSession;
                        UpdateTime = taskResult.UpdateTime;



                        // 根据待确认的编号来修改它的状态信息(里面的就算了, 反正马上关闭窗口)
                        if (result.Result)
                        {
                            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

                            ICollection<OrderDetail> detailsOld = null;
                            ICollection<OrderPay> paysOld = null;
                            if (null != model.PayOrder)
                                detailsOld = model.PayOrder.tb_orderdetail;
                            if (null != model.PayOrder)
                                paysOld = model.PayOrder.tb_orderpay;

                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;

                            if (null != detailsOld)
                                model.PayOrder.tb_orderdetail = detailsOld;
                            if (null != paysOld)
                                model.PayOrder.tb_orderpay = paysOld;


                            foreach (var item in orderDetailsConfirmResult)
                            {
                                OrderDetail OrderDetails = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId && x.State == 1).FirstOrDefault();
                                if (null != OrderDetails)
                                {
                                    OrderDetails.State = 2;

                                    // 确认部分去掉现有数量
                                    Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                    if (product.IsBindCount == 1)
                                    {

                                        if (product.BalanceCount < item.Count)
                                        {
                                            // 如果有父级
                                            if (null != product.ProductParentId)
                                            {
                                                Product productParent = Resources.Instance.Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                                if (null != productParent && productParent.IsBindCount == 1)
                                                {
                                                    double ParentRemove = 0;
                                                    double ProductAdd = 0;


                                                    double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3);
                                                    ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                    ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                                    ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3);


                                                    // 从父级中去掉
                                                    productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                    productParent.UpdateTime = UpdateTime;


                                                    // 给产品增加零的
                                                    product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                                }
                                            }
                                        }

                                        product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                        product.UpdateTime = UpdateTime;

                                        Notification.Instance.ActionProduct(null, product, 2);
                                    }
                                }
                            }
                            model.OrderSession = this.RoomStateSession = newRoomSession;
                            this.resultList = model.PayOrder.tb_orderdetail.ToList();
                            this.payList = model.PayOrder.tb_orderpay.ToList();
                            this.Selected.tempPayList = payList.ToList();

                        }
                    }
                    // 如果是修改
                    else if (mode == 1)
                    {
                        faildMsgName = successMsgName = Res.Instance.GetString("Save");


                        List<OrderDetail> orderDetailsAddResult;
                        List<OrderPay> orderPaysAddResult;
                        List<OrderDetail> orderDetailsEditResult;
                        List<OrderDetail> orderDetailsConfirmResult;


                        long UpdateTime;

                        var taskResult = await OperatesService.Instance.ServiceSaveOrderDetail(order, orderDetailsAdd, this.Selected.tempPayList.Where(x => x.AddTime == 0).Select(x => { x.OrderId = order.OrderId; return x; }).ToList(), orderDetailsEdit, null, RoomStateSession);
                        result = taskResult.resultModel;
                        orderDetailsAddResult = taskResult.orderDetailsAddResult;
                        orderPaysAddResult = taskResult.orderPaysAddResult;
                        orderDetailsEditResult = taskResult.orderDetailsEditResult;
                        orderDetailsConfirmResult = taskResult.orderDetailsConfirmResult;
                        newRoomSession = taskResult.newRoomStateSession;
                        UpdateTime = taskResult.UpdateTime;


                        // 根据待确认的编号来修改它的状态信息(里面的就算了, 反正马上关闭窗口)
                        if (result.Result)
                        {
                            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

                            ICollection<OrderDetail> detailsOld = null;
                            ICollection<OrderPay> PaysOld = null;
                            if (null != model.PayOrder)
                                detailsOld = model.PayOrder.tb_orderdetail;
                            if (null != model.PayOrder)
                                PaysOld = model.PayOrder.tb_orderpay;

                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;

                            if (null != detailsOld)
                                model.PayOrder.tb_orderdetail = detailsOld;
                            if (null != PaysOld)
                                model.PayOrder.tb_orderpay = PaysOld;


                            foreach (var item in orderDetailsAddResult)
                            {
                                // 新增部分去掉现有数量
                                Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {


                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.Instance.Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                            if (null != productParent && productParent.IsBindCount == 1)
                                            {
                                                double ParentRemove = 0;
                                                double ProductAdd = 0;


                                                double NeedChangeFromParent = Math.Round(item.Count - product.BalanceCount, 3); 
                                                ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                ParentRemove = (int)Math.Ceiling(ParentRemove);

                                                ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3);


                                                // 从父级中去掉
                                                productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                productParent.UpdateTime = UpdateTime;


                                                // 给产品增加零的
                                                product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                            }
                                        }
                                    }

                                    product.BalanceCount = Math.Round(product.BalanceCount - item.Count, 3);
                                    product.UpdateTime = UpdateTime;

                                    Notification.Instance.ActionProduct(null, product, 2);
                                }

                                model.PayOrder.tb_orderdetail.Add(item);
                            }
                            foreach (var item in orderDetailsEditResult)
                            {
                                OrderDetail OrderDetails = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == item.OrderDetailId).FirstOrDefault();
                                if (null != OrderDetails)
                                {
                                    // 编辑的部分先把数据修正
                                    Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                    if (product.IsBindCount == 1)
                                    {

                                        if (product.BalanceCount < (-OrderDetails.Count + item.Count))
                                        {
                                            // 如果有父级
                                            if (null != product.ProductParentId)
                                            {
                                                Product productParent = Resources.Instance.Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

                                                if (null != productParent && productParent.IsBindCount == 1)
                                                {
                                                    double ParentRemove = 0;
                                                    double ProductAdd = 0;


                                                    double NeedChangeFromParent = Math.Round((-OrderDetails.Count + item.Count) - product.BalanceCount, 3);
                                                    ParentRemove = Math.Round(NeedChangeFromParent / product.ProductParentCount, 3); 
                                                    ParentRemove = (int)Math.Ceiling(ParentRemove); 

                                                    ProductAdd = Math.Round(ParentRemove * product.ProductParentCount, 3); 


                                                    // 从父级中去掉
                                                    productParent.BalanceCount = Math.Round(productParent.BalanceCount - ParentRemove, 3);
                                                    productParent.UpdateTime = UpdateTime;


                                                    // 给产品增加零的
                                                    product.BalanceCount = Math.Round(product.BalanceCount + ProductAdd, 3);


                                                }
                                            }
                                        }

                                        product.BalanceCount = Math.Round(product.BalanceCount - (-OrderDetails.Count + item.Count), 3);
                                        product.UpdateTime = UpdateTime;

                                        Notification.Instance.ActionProduct(null, product, 2);
                                    }


                                    OrderDetails.Count = item.Count;
                                    OrderDetails.Price = item.Price;
                                    OrderDetails.TotalPrice = item.TotalPrice;
                                    OrderDetails.OriginalTotalPrice = item.OriginalTotalPrice;
                                    OrderDetails.UpdateTime = item.UpdateTime;
                                }
                            }
                            model.OrderSession = this.RoomStateSession = newRoomSession;
                            this.resultList = model.PayOrder.tb_orderdetail.ToList();


                            foreach (var item in orderPaysAddResult)
                            {
                                model.PayOrder.tb_orderpay.Add(item);


                                if (null != item.MemberId)
                                {
                                    Notification.Instance.ActionMember(this, new Member() { MemberId = item.MemberId.Value }, null);
                                    item.MemberId = item.tb_member.MemberId;
                                }

                            }
                            this.payList = model.PayOrder.tb_orderpay.ToList();
                            this.Selected.tempPayList = payList.ToList();

                        }
                    }
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        if (result.Result)
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), successMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);
    
                            RoomListViewModel viewModel = NavigationPath.Instance.RoomListPage.BindingContext as RoomListViewModel;
                            viewModel.Init((long)-1);
                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                IsRetry = true;
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenWaitRetry"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }
                            else
                            {
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), faildMsgName), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            }

                        }


                        if (IsRetry)
                        {
                            RoomListViewModel viewModel = NavigationPath.Instance.CurrentPage.BindingContext as RoomListViewModel;
                            viewModel.Init(this.RoomId);
                            Close();
                        }
                        else if (null != result && result.Result)
                        {
                            Init(this.RoomId);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                        }), false, string.Format(Resources.Instance.GetString("OperateFaild"), faildMsgName));
                    }));
                }

                Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                {
                    IsLoading = false;
                    if (mode == 0 || mode == 1 || mode == 2 || mode == 4)
                        Selected.IsLoading = false;
                }));


            });



        }



        /// <summary>
        /// 删除数据
        /// </summary>
        private void Delete(long OrderDetailId)
        {
            long Id = OrderDetailId;
            try
            {
                //确认删除
                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SureDelete"), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                {


                    if (msg == "NO")
                        return;


                    if (Selected.CurrentSelectedList.Any(x => x.IsNew))
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SaveBeforeDelete"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        return;
                    }



                    // 不允许权限不够的人删除
                    if (Id != -1 && !Common.Instance.IsDeleteProduct())
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("PermissionDenied"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        return;

                    }

                    Order order;
                    List<OrderDetail> details;

                    try
                    {
                        Calc(out details, out order, false, false, true, true, Id);
                    }
                    catch (Exception ex)
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                        }), false, Resources.Instance.GetString("DeleteFailt"));
                        return;
                    }

                    Selected.IsLoading = true;

                    Task.Factory.StartNew(async () =>
                    {
                        try
                        {

                            await ExtX.WaitForLoading();


                            OrderDetail old = resultList.Where(x => x.OrderDetailId == Id).FirstOrDefault();
                            old.State = 3;
                            List<OrderDetail> orderDetails = new List<OrderDetail>() { old };

                          
                           
                            var taskResult = await OperatesService.Instance.ServiceDelOrderDetail(order, orderDetails, RoomStateSession);

                            ResultModel result = taskResult.resultModel;
                            string newRoomSession = taskResult.newRoomStateSession ;
                            long UpdateTime = taskResult.UpdateTime;
                            List<OrderDetail> newOrderDetails = taskResult.newOrderDetails;
                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, Resources.Instance.GetString("DeleteSuccess"), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);
                                    RoomModel model = Resources.Instance.RoomsModel.Where(x => null != x.PayOrder && x.PayOrder.OrderId == order.OrderId).FirstOrDefault();
                                    OrderDetail oldModel = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == Id).FirstOrDefault();



                                    oldModel.State = 3;

                                    int no = resultList.IndexOf(oldModel);
                                    resultList.RemoveAt(no);
                                    resultList.Insert(no, newOrderDetails.FirstOrDefault());


                                    ICollection<OrderDetail> detailsOld = null;
                                    ICollection<OrderPay> paysOld = null;
                                    if (null != model.PayOrder)
                                        detailsOld = model.PayOrder.tb_orderdetail;
                                    if (null != model.PayOrder)
                                        paysOld = model.PayOrder.tb_orderpay;

                                    model.PayOrder = this.order = order;
                                    if (null != detailsOld)
                                        model.PayOrder.tb_orderdetail = detailsOld;
                                    if (null != paysOld)
                                        model.PayOrder.tb_orderpay = paysOld;


                                    model.OrderSession = this.RoomStateSession = newRoomSession;


                                    foreach (var item in orderDetails)
                                    {
                                        // 删除部分增加现有数量
                                        Product product = Resources.Instance.Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                        if (product.IsBindCount == 1)
                                        {
                                            product.BalanceCount = Math.Round(product.BalanceCount + item.Count, 3);
                                            product.UpdateTime = UpdateTime;

                                            Notification.Instance.ActionProduct(null, product, 2);
                                        }
                                    }

                                    Order order2;
                                    List<OrderDetail> orderDetails2;
                                    Calc(out orderDetails2, out order2);


                                    RoomListViewModel viewModel = NavigationPath.Instance.RoomListPage.BindingContext as RoomListViewModel;
                                    viewModel.Init((long)-1);
 
                                    Init(this.RoomId);

                                }
                                else
                                {
                                    if (result.IsRefreshSessionModel)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenRefreshModel"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);

                                        RoomListViewModel viewModel = NavigationPath.Instance.CurrentPage.BindingContext as RoomListViewModel;
                                        viewModel.Init(this.RoomId);
                                        Close();
                                    }
                                    else if (result.IsSessionModelSameTimeOperate)
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("FaildThenWaitRetry"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                    }
                                    else
                                    {
                                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("DeleteFailt"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                                }), false, Resources.Instance.GetString("DeleteFailt"));
                            }));
                        }
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                        {
                            Selected.IsLoading = false;
                        }));
                    });


                }, null);

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                }), false, Resources.Instance.GetString("DeleteFailt"));
                return;
            }
        }


        private int _displayMode;
        /// <summary>
        /// 显示模式 1产品2已选
        /// </summary>
        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                OnPropertyChanged("DisplayMode");
            }
        }






        private ProductsViewModel _products;
        /// <summary>
        /// 显示产品控件
        /// </summary>
        public ProductsViewModel Products
        {
            get { return _products; }
            set
            {
                _products = value;
                OnPropertyChanged("ProductsViewModel");
            }
        }



        private SelectedViewModel _selected;
        /// <summary>
        /// 显示已选控件
        /// </summary>
        public SelectedViewModel Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }





        /// <summary>
        /// 切换语言按钮
        /// </summary>
        private RelayCommand _changeOrderLanguageCommand;
        public Command ChangeOrderLanguageCommand
        {
            get
            {
                return _changeOrderLanguageCommand ?? (_changeOrderLanguageCommand = new RelayCommand(param =>
                {
                    // 是否允许更改语言
                    //if (null == this.order || Common.Instance.IsAllowChangeLanguage())
                    //{
                    //Selected.LanguageMode = Res.Instance.GetMainLangByMainLangIndex(Res.Instance.GetMainLangByLangIndex(this.Selected.LanguageMode).MainLangIndex + 1).LangIndex;

                    Selected.LanguageMode = (int)Selected.SelectedLang.Value;

                        this.Selected.RemarkChanged = true;

                        RefreshState();
                    //}

                }));
            }
        }




        /// <summary>
        /// 搜索按钮
        /// </summary>
        private RelayCommand _searchCommand;
        public Command SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(param =>
                    {
                        

                    });
                }
                return _searchCommand;
            }
        }



        private bool _isExists = false;
        /// <summary>
        /// 是否存在账单
        /// </summary>
        public bool IsExists
        {
            get { return (null != this.order); }
            set
            {
                _isExists = value;
                OnPropertyChanged("IsExists");
            }
        }


        private bool _isRoleIn = false;
        /// <summary>
        /// 是否允许收入交易管理
        /// </summary>
        public bool IsRoleIn
        {
            get { return Common.Instance.IsIncomeTradingManage(); }
            set
            {
                _isRoleIn = value;
                OnPropertyChanged("IsRoleIn");
            }
        }




        private bool _isReplaceRoom = false;
        /// <summary>
        /// 是否允许更改包厢
        /// </summary>
        public bool IsReplaceRoom
        {
            get { return Common.Instance.IsReplaceRoom(); }
            set
            {
                _isReplaceRoom = value;
                OnPropertyChanged("IsReplaceRoom");
            }
        }



        private bool _isCancelOrder = false;
        /// <summary>
        /// 是否允许取消订单
        /// </summary>
        public bool IsCancelOrder
        {
            get { return Common.Instance.IsCancelOrder(); }
            set
            {
                _isCancelOrder = value;
                OnPropertyChanged("IsCancelOrder");
            }
        }


        /// <summary>
        /// 更新部分
        /// </summary>
        private void RefreshSome(List<long> RoomsId)
        {
            if (RoomsId.Contains(this.RoomId) && !Selected.IsRefresh)
            {
                // 因为SignalR原因, 每次唤醒都会重新刷新, 为了防止错误更新无需更新的内容, 则先做个判断.
                RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == this.RoomId).FirstOrDefault();
                if (model.OrderSession != this.RoomStateSession)
                    Selected.IsRefresh = true;

            }
        }



        /// <summary>
        /// 更新按钮
        /// </summary>
        private RelayCommand _refreshCommand;
        public Xamarin.Forms.Command RefreshCommand
        {
            get
            {
                if (_refreshCommand == null)
                {
                    _refreshCommand = new RelayCommand(param =>
                    {
                        Order order;
                        List<OrderDetail> details;

                        bool IgnoreNotConfirm = true;
                        Calc(out details, out order, false, false, IgnoreNotConfirm);


                        List<DetailsModel> detailsModel = Selected.CurrentSelectedList.Where(x => x.IsNew).ToList();




                        RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                        this.RoomId = model.RoomId;
                        this.order = model.PayOrder;
                        this.resultList = (null == model.PayOrder ? null : model.PayOrder.tb_orderdetail.ToList());
                        this.RoomStateSession = model.OrderSession;
                        this.payList = (null == model.PayOrder ? null : model.PayOrder.tb_orderpay.ToList());

                        if (null == payList)
                        {
                            payList = new List<OrderPay>();
                        }
                        this.Selected.tempPayList = payList.ToList();


                        Init(this.RoomId);


                        foreach (var item in detailsModel)
                        {
                            OperateDetails(0, item);
                        }


                        Order import;
                        List<OrderDetail> importDetails;
                        Calc(out importDetails, out import);


                    });
                }
                return _refreshCommand;
            }
        }

    }
}
