using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Reports;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Component;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Oybab.Res.View.ViewModels.Pages
{
    public sealed class OrderViewModel: ViewModelBase
    {
        private UIElement _element;
        private UIElement _selectedUI;
        private long RoomId;
        private Order order;
        private string RoomStateSession;
        private List<OrderDetail> resultList = null;
        private List<OrderPay> payList = null;
        private ListBox _lbSelectedList;

        private List<OrderPay> tempPayList = new List<OrderPay>();

        public OrderViewModel(UIElement element, UIElement productsUI, UIElement selectedUI, StackPanel spProductTypeList, ScrollViewer scrollViewerProducts, ListBox lbSelectedList, UniformGrid ugRequest, WrapPanel paidPricePanel)
        {
            this._element = element;
            this._selectedUI = selectedUI;

            this.Products = new ProductsViewModel(productsUI, spProductTypeList, scrollViewerProducts);
            this.Products.ProductChange = ProductChange;
            this.Products.DetectProductIsSelected = DetectProductIsSelected;
            this.Products.Operate = OperateDetails;


            this.Selected = new SelectedViewModel(selectedUI, lbSelectedList);
            this.Selected.Save = Save;
            this.Selected.Checkout = Checkout;
            this.Selected.RefreshTime = RefreshTime;
            this.Selected.RefreshCommand = RefreshCommand;

            this.Language = new LanguageViewModel(element, ChangeLanguage);


            this.Request = new RequestViewModel(element, ugRequest);
            this._lbSelectedList = lbSelectedList;

            this.ChangePrice = new ChangePriceViewModel(element, ReCalc);
            this.ChangeCount = new ChangeCountViewModel(element, ReCalc);

            this.ChangeTime = new ChangeTimeViewModel(element, ReCalc);
            this.Search = new SearchViewModel(element, AddProduct);
            this.ChangePaidPrice = new ChangePaidPriceViewModel(element, RecalcPaidPrice, paidPricePanel);


            // 添加处理事件
            this._element.AddHandler(PublicEvents.BoxEvent, new RoutedEventHandler(HandleBox), true);


            // 订单更新
            Notification.Instance.NotificateSendFromServer += (obj, value, args) => { if (null == args) RefreshSome(new List<long>() { value }); };
            Notification.Instance.NotificateSendsFromServer += (obj, value, args) => { RefreshSome(value); };

        }
       

        /// <summary>
        /// 重新计算支付金额
        /// </summary>
        private void RecalcPaidPrice()
        {
            if (null != order && this.ChangePaidPrice.Remark != this.order.Remark)
            {
                this.Selected.RemarkChanged = true;
            }

            

            


            // 如果跟订单上次保存的不一样,就提示未保存提示
            if (oldList.All(ChangePaidPrice.PayModel.Contains) && tempPayList.Count == ChangePaidPrice.PayModel.Count)
            {
            }
            else
            {
                this.Selected.RoomPaidPriceChanged = true;
                tempPayList = ChangePaidPrice.PayModel.Select(x => x.GetOrderPay()).ToList();
            }

            if (this.Selected.RoomPaidPriceChanged || this.Selected.RemarkChanged)
            {
                

                RefreshTime();

            }
        }


            /// <summary>
            /// 扫条形码
            /// </summary>
            private void ScanBarcode(string code)
        {
            // 获取条码产品
            List<Product> ResultProducts = Resources.GetRes().Products.Where(x => (x.Barcode == code || (x.IsScales == 1 && code.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).ToList();

            if (ResultProducts.Count > 1)
            {
                ResultProducts = ResultProducts.OrderByDescending(x => x.Order).ThenBy(x => x.ProductParentCount).ToList();
                // 刷新产品
                Products.RefreshProduct(ResultProducts);
                this.DisplayMode = 1;

                return;
            }
            else if (ResultProducts.Count == 0)
            {
                return;
            }

            Product CurrentProduct = ResultProducts.FirstOrDefault();
            
            // 如果找到了
            if (null != CurrentProduct)
            {
                DetailsModel details = Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == CurrentProduct && x.OrderDetail.OrderDetailId <= 0).FirstOrDefault();
                // 已存在就增加1个(暂时改为提醒已存在)
                if (null != details)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("CurrentProductAlreadyExists"), null, PopupType.Warn));
                    }));
                }
                // 不存在新加一个
                else
                {
                   
                   if (CurrentProduct.IsScales == 1)
                        ProductChange(true, CurrentProduct.ProductId, double.Parse(code.Substring(7, 2) + "." + code.Substring(9, 3)));
                   else
                        ProductChange(true, CurrentProduct.ProductId, 1);
                }
            }
        }



        /// <summary>
        /// 刷新客显
        /// </summary>
        public void RefreshPM()
        {
            RefreshState();
        }


        public void PrePrintOrder()
        {
            if (_element.Visibility == Visibility.Visible && !this.Language.IsShow && !this.ChangePaidPrice.IsShow && !this.ChangeCount.IsShow && !this.ChangePrice.IsShow && !this.ChangeTime.IsShow && !this.Request.IsShow)
            {
                this.Language.Show();
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        private void PrintOrder(long Lang)
        {

            //确认删除
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("ConfirmOperate"), Resources.GetRes().GetString("Print")), msg =>
            {
                if (msg == "NO")
                    return;


                Task.Factory.StartNew(() =>
                {
                    try
                    {



                        this.order.tb_orderdetail = resultList;
                        bool result = Print.Instance.PrintOrder(order, Lang);
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            if (result)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("Print")), null, PopupType.Information));
                            }
                            else
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Print")), null, PopupType.Information));
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                           {
                               ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                               {
                                   _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                               }), false, Resources.GetRes().GetString("Exception_OperateRequestFaild"));
                           }));
                    }
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));

                       
                    }));
                });
            }, PopupType.Question));
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init(object roomId)
        {
            this.RoomId = (long)roomId;
            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == this.RoomId).FirstOrDefault();

            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == this.RoomId).FirstOrDefault();

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
            tempPayList = payList.ToList();

            Selected.RoomId = model.RoomId;
            Selected.RoomNo = model.RoomNo;
            Selected.AllowPaid = true; //只是更新它以达到刷新作用

            Selected.RoomType = (int)room.IsPayByTime;


            DateTime now = DateTime.Now;

            if (null == order)
            {


                // 初始化参数
                DisplayMode = 1;

                // 语言默认, 或者上次选择
                if (Resources.GetRes().DefaultOrderLang == -1)
                {

                    Language.LanguageMode = Resources.GetRes().MainLang.LangIndex;
                    
                }
                else
                {
                    Language.LanguageMode = Resources.GetRes().DefaultOrderLang;
                }


                // 刷新第二屏语言
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(Language.LanguageMode).LangIndex, -1);
                }

                Selected.StartTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));


                // 新打开的订单直接显示包厢价格
                Selected.RoomPrice = CommonOperates.GetCommonOperates().GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, 0, false, null);

                Selected.TotalPrice = 0;
                Selected.PaidPrice = 0;

                Common.GetCommon().OpenPriceMonitor("0");


                // 刷新第二屏幕
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
                }


                //tempRemark = null;
                ChangePaidPrice.Remark = "";
                Selected.TempUnlimitedTime = false;

            }
            else
            {
                // 初始化参数
                DisplayMode = 2;

                Language.LanguageMode = Resources.GetRes().GetMainLangByLangIndex((int)order.Lang).LangIndex;

                Selected.RoomPrice = this.order.RoomPrice;

                Selected.TotalPrice = this.order.TotalPrice;
                Selected.PaidPrice = this.order.TotalPaidPrice;

                
                ChangePaidPrice.Remark = order.Remark;

                Selected.TempUnlimitedTime = (order.IsFreeRoomPrice == 2 ? true : false);

                Common.GetCommon().OpenPriceMonitor(order.BorrowPrice.ToString());

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

                        TimeSpan balance = (DateTime.Now - DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null));
                        // 如果剩余时间已经超出了, 默认0:0显示
                        if (DateTime.Now < DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null))
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




            Selected.CurrentSelectedList.Clear();


            if (null != resultList)
            {
                foreach (var item in resultList.OrderByDescending(x => x.OrderDetailId))
                {
                    Selected.CurrentSelectedList.Add(new DetailsModel() { _element = _lbSelectedList, AddTime = DateTime.ParseExact(item.AddTime.ToString(), "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm"), Product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault(), OrderDetail = item, Operate = OperateDetails, IsOnlyOrder = true });
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




            if (!IsScanReady)
            {
                IsScanReady = true;
                // 扫条码
                Notification.Instance.NotificationBarcodeReader += Instance_NotificationBarcodeReader;
                Notification.Instance.NotificationCardReader += Instance_NotificationCardReader;
            }


        }
        private bool IsScanReady = false;


        /// <summary>
        /// 扫条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationBarcodeReader(object sender, string value, object args)
        {
            if (_selectedUI.IsVisible)
                ScanBarcode(value);
        }


        /// <summary>
        /// 扫条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        /// <param name="args"></param>
        private void Instance_NotificationCardReader(object sender, string value, object args)
        {
            if (ChangePaidPrice.IsShow)
            {
                ChangePaidPrice.OpenMemberByScanner(value);
                return;
            }
        }




        /// <summary>
        /// 处理窗口弹出按钮路由
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleBox(object sender, RoutedEventArgs args)
        {
            BoxRoutedEventArgs boxArgs = args as BoxRoutedEventArgs;
            if (null != boxArgs)
            {
                switch (boxArgs.BoxType)
                {
                    case BoxType.Request:
                        this.Request.InitialRequest(boxArgs.Param as DetailsModel);
                        this.Request.Show();
                        break;
                    case BoxType.ChangePrice:
                        this.ChangePrice.Init(boxArgs.Param as DetailsModel);
                        this.ChangePrice.Show();
                        break;
                    case BoxType.ChangeCount:
                        this.ChangeCount.Init(boxArgs.Param as DetailsModel);
                        this.ChangeCount.Show();
                        break;
                    case BoxType.ChangeTime:
                        this.ChangeTime.Init(boxArgs.Param as SelectedViewModel);
                        this.ChangeTime.Show();
                        break;
                    case BoxType.Search:
                        this.Search.Show();
                        break;
                    case BoxType.ChangePaidPrice:
                        oldList = tempPayList.Select(x => new CommonPayModel(x)).ToList();
                        this.ChangePaidPrice.Init(this.Selected.TotalPrice, oldList, true, false);
                        this.ChangePaidPrice.Show();
                        break;
                    default:
                        break;
                }
            }
        }

        private List<CommonPayModel> oldList = null;
        /// <summary>
        /// 操作订单详情(0新增1删除,2刷新)
        /// </summary>
        /// <param name="IsAdd"></param>
        /// <param name="details"></param>
        private void OperateDetails(int mode, DetailsModel details)
        {
            if (mode == 0)
                Selected.CurrentSelectedList.Insert(0, details);
            else if (mode == 1)
            {
                Selected.CurrentSelectedList.Remove(details);
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


        /// <summary>
        /// 添加产品
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="key"></param>
        private void AddProduct(int mode, string key)
        {
            List<Product> productList = new List<Product>();

            if (mode == 1)
            {
                //string TrimCode = key.TrimStart('0');
                // 获取条码产品
                productList = Resources.GetRes().Products.Where(x => (x.Barcode == key || (x.IsScales == 1 && key.StartsWith("22" + x.Barcode))) && (x.HideType == 0 || x.HideType == 2)).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId).ToList();
            }
            else if (mode == 2)
            {
                var proList = Resources.GetRes().Products.Where(x => x.HideType == 0 || x.HideType == 2).OrderByDescending(x => x.Order).ThenByDescending(x => x.ProductId);

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
            Product product = Resources.GetRes().Products.Where(x => x.ProductId == productId).FirstOrDefault();
            if (!IsSelected)
            {
                Selected.CurrentSelectedList.Remove(Selected.CurrentSelectedList.Where(x => x.IsNew && x.Product == product).FirstOrDefault());
            }
            else
            {
                model = new DetailsModel() { _element = _lbSelectedList, IsNew = true, AddTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm"), Product = product, OrderDetail = new OrderDetail() { OrderDetailId = -1, ProductId = product.ProductId, Count = count, AddTime = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")), Price = product.Price, State = 0, TotalPrice = product.Price }, Operate = OperateDetails, IsOnlyOrder = true };
                Selected.CurrentSelectedList.Insert(0, model);
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
            if (Selected.RemarkChanged || Selected.RoomPaidPriceChanged || Selected.RoomTimeChange  || Selected.CurrentSelectedList.Any(x => x.IsNew))
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
                    if (Common.GetCommon().IsIncomeTradingManage())
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

            TimeSpan balance = (DateTime.Now - DateTime.ParseExact(Selected.EndTimeTemp.ToString(), "yyyyMMddHHmmss", null));
            // 如果剩余时间已经超出了, 默认0:0显示
            if (DateTime.Now < DateTime.ParseExact(Selected.EndTimeTemp.ToString(), "yyyyMMddHHmmss", null))
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
        /// 修改语言
        /// </summary>
        /// <param name="lang"></param>
        private void ChangeLanguage(int lang)
        {
            if (this.Language.IsShow)
            {
                this.Language.Hide(null);
                PrintOrder(lang);
            }
            else
            {
                // 是否允许更改语言
                if (null == this.order || Common.GetCommon().IsAllowChangeLanguage())
                {
                    Language.LanguageMode = lang;

                    this.Selected.RemarkChanged = true;

                    RefreshState();
                }

                // 刷新第二屏语言
                if (FullScreenMonitor.Instance._isInitialized)
                {
                    FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(Language.LanguageMode).LangIndex, -1);
                }
            }

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



        private double _lastBorrowPrice = 0;
        /// <summary>
        /// 结账
        /// </summary>
        private void Checkout()
        {
            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

            if (null == model.PayOrder)
            {
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), (x) =>
                {
                    _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Room, this.RoomId));
                }, PopupType.Warn));
                
            }

            else
            {
                _lastBorrowPrice = model.PayOrder.BorrowPrice;
                _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.CheckoutOrder, model));
            }
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
                        Product parentProduct = Resources.GetRes().Products.FirstOrDefault(x => x.ProductId == item.Product.ProductParentId);

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
                if (Common.GetCommon().IsAllowChangeLanguage())
                {
                    order.Lang = Language.LanguageMode;

                }
            }
            else
            {
                order.Lang = Language.LanguageMode;
                order.IsPack = 0;
                order.StartTime = Selected.StartTimeTemp;
                order.IsFreeRoomPrice = (Selected.TempUnlimitedTime ? 2 : 0);
            }




            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();

            int totalMinutes = 0;
            bool IsSubTime = false;

            if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
            {
                // 直接计算时间
                if (null == this.order)
                    totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                else
                {
                    // 如果是时间少了, 则还是老方法计算
                    if (DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.ParseExact(order.RoomPriceCalcTime.ToString(), "yyyyMMddHHmmss", null))
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                        IsSubTime = true;
                    }
                    // 如果不是, 则在上次的时间加上新时间价格
                    else
                    {
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.RoomPriceCalcTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                    }

                }
            }



            Selected.RoomPrice = order.RoomPrice = CommonOperates.GetCommonOperates().GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, totalMinutes, IsSubTime, order.EndTime);
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
                        totalMinutes = (int)DateTime.ParseExact(order.EndTime.ToString(), "yyyyMMddHHmmss", null).Subtract(DateTime.ParseExact(order.StartTime.ToString(), "yyyyMMddHHmmss", null)).TotalMinutes;
                    }
                    Selected.RoomPrice = order.RoomPrice = CommonOperates.GetCommonOperates().GetRoomPrice(this.order, room.Price, room.PriceHour, room.IsPayByTime, totalMinutes, IsSubTime, order.EndTime, true);
                    Selected.TotalPrice = order.TotalPrice = Math.Round(lastTotal + order.RoomPrice, 2);
                    order.OriginalTotalPrice = Math.Round(lastOriginalTotalPrice + order.RoomPrice, 2);
                }
                    order.IsFreeRoomPrice = 0;
            }




            double balancePrice = 0;


            order.PaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.BalanceId).Sum(x => x.OriginalPrice), 2);

            order.MemberPaidPrice = Math.Round(tempPayList.Where(x => x.State != 2 && null != x.MemberId).Sum(x => x.OriginalPrice), 2);



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


            // 显示客显(实际客户需要支付的赊账)
            Common.GetCommon().OpenPriceMonitor(order.BorrowPrice.ToString());

            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                RoomInfoModel roomInfo = new RoomInfoModel();
                roomInfo.RoomNo = Selected.RoomNo;
                roomInfo.RoomPrice = Selected.RoomPrice;
                roomInfo.TotalTime = Selected.RoomTime;

                FullScreenMonitor.Instance.RefreshSecondMonitorList(new Res.View.Models.BillModel(order, this.order, details, roomInfo, false));
            }

            if (OnlyTotal)
                return;


            if (string.IsNullOrWhiteSpace(this.ChangePaidPrice.Remark))
                order.Remark = null;
            else
                order.Remark = this.ChangePaidPrice.Remark;

            if (room.IsPayByTime == 0)
            {
                order.EndTime = null;
                order.StartTime = null;
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



        /// <summary>
        /// 操作: 0新建, 1保存, 2确认, 3结账, 4取消
        /// </summary>
        private void Operate(int mode)
        {
            Order order;
            List<OrderDetail> details;

            bool IgnoreNotConfirm = true;
            Calc(out details, out order, false, false, IgnoreNotConfirm);


            if (null == this.order && Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder).Count() >= Resources.GetRes().RoomCount)
            {
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("Exception_RoomCountOutOfLimit"), ""), null, PopupType.Warn));
                return;
            }



            ResultModel result = null;
            bool IsRetry = false;


            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
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
                        faildMsgName = successMsgName = Resources.GetRes().GetString("Save");


                        long UpdateTime;

                        result = OperatesService.GetOperates().ServiceAddOrder(order, details, tempPayList.Where(x => x.AddTime == 0).ToList(), RoomStateSession, out resultDetails, out resultPays, out newRoomSession, out UpdateTime);

                        if (result.Result)
                        {
                            Resources.GetRes().DefaultOrderLang = Resources.GetRes().GetMainLangByLangIndex((int)order.Lang).MainLangIndex;
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                            Order oldOrder = this.order;
                            model.PayOrder = this.order = order;
                            model.PayOrder.tb_orderdetail = this.resultList = resultDetails;
                            model.PayOrder.tb_orderpay = this.payList = resultPays;
                            tempPayList = payList.ToList();
                            model.OrderSession = this.RoomStateSession = newRoomSession;

                            // 新增部分去掉现有数量
                            foreach (var item in resultDetails)
                            {
                                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {

                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

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

                            Print.Instance.PrintOrderAfterBuy(order, resultDetails, oldOrder);
                        }

                    }
                    // 如果是确认
                    else if (mode == 2)
                    {
                        faildMsgName = successMsgName = Resources.GetRes().GetString("Confirm");




                        List<OrderDetail> orderDetailsAddResult;
                        List<OrderDetail> orderDetailsEditResult;
                        List<OrderDetail> orderDetailsConfirmResult;


                        long UpdateTime;

                        result = OperatesService.GetOperates().ServiceSaveOrderDetail(order, null, null, null, orderDetailsConfirm, RoomStateSession, out orderDetailsAddResult, out List<OrderPay> temp, out orderDetailsEditResult, out orderDetailsConfirmResult, out newRoomSession, out UpdateTime);

                        // 根据待确认的编号来修改它的状态信息(里面的就算了, 反正马上关闭窗口)
                        if (result.Result)
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

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
                                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                    if (product.IsBindCount == 1)
                                    {

                                        if (product.BalanceCount < item.Count)
                                        {
                                            // 如果有父级
                                            if (null != product.ProductParentId)
                                            {
                                                Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

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
                            tempPayList = payList.ToList();

                            Print.Instance.PrintOrderAfterBuy(order, orderDetailsConfirmResult, oldOrder);
                        }
                    }
                    // 如果是修改
                    else if (mode == 1)
                    {
                        faildMsgName = successMsgName = Resources.GetRes().GetString("Save");


                        List<OrderDetail> orderDetailsAddResult;
                        List<OrderPay> orderPaysAddResult;
                        List<OrderDetail> orderDetailsEditResult;
                        List<OrderDetail> orderDetailsConfirmResult;


                        long UpdateTime;

                        result = OperatesService.GetOperates().ServiceSaveOrderDetail(order, orderDetailsAdd, tempPayList.Where(x => x.AddTime == 0).Select(x => { x.OrderId = order.OrderId; return x; }).ToList(), orderDetailsEdit, null, RoomStateSession, out orderDetailsAddResult, out orderPaysAddResult, out orderDetailsEditResult, out orderDetailsConfirmResult, out newRoomSession, out UpdateTime);

                        // 根据待确认的编号来修改它的状态信息(里面的就算了, 反正马上关闭窗口)
                        if (result.Result)
                        {
                            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();

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
                                Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                if (product.IsBindCount == 1)
                                {


                                    if (product.BalanceCount < item.Count)
                                    {
                                        // 如果有父级
                                        if (null != product.ProductParentId)
                                        {
                                            Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

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
                                    Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
                                    if (product.IsBindCount == 1)
                                    {

                                        if (product.BalanceCount < (-OrderDetails.Count + item.Count))
                                        {
                                            // 如果有父级
                                            if (null != product.ProductParentId)
                                            {
                                                Product productParent = Resources.GetRes().Products.Where(x => x.ProductId == product.ProductParentId).FirstOrDefault();

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
                            tempPayList = payList.ToList();

                            Print.Instance.PrintOrderAfterBuy(order, orderDetailsAddResult, oldOrder);
                        }
                    }
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), successMsgName), null, PopupType.Information));

                        }
                        else
                        {
                            if (result.IsRefreshSessionModel)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                
                                IsRetry = true;
                            }
                            else if (result.IsSessionModelSameTimeOperate)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenWaitRetry"), null, PopupType.Warn));
                            }
                            else
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), faildMsgName), null, PopupType.Warn));
                            }

                        }


                        if (IsRetry)
                        {
                            _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Room, this.RoomId));
                        }
                        else if (null != result && result.Result)
                        {
                            Init(this.RoomId);
                        }
                    }));
                }
                catch (Exception ex)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                        }), false, string.Format(Resources.GetRes().GetString("OperateFaild"), faildMsgName));
                    }));
                }

                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
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
                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SureDelete"), msg => {


                    if (msg == "NO")
                        return;

                
                    if (Selected.CurrentSelectedList.Any(x => x.IsNew))
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveBeforeDelete"), null, PopupType.Warn));
                        return;
                    }


                    // 不允许权限不够的人删除
                    if (Id != -1 && !Common.GetCommon().IsDeleteProduct())
                    {
                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PermissionDenied"), null, PopupType.Warn));
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
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                        }), false, Resources.GetRes().GetString("DeleteFailt"));
                        return;
                    }

                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                    Task.Factory.StartNew(() =>
                    {
                        try
                        {


                            OrderDetail old = resultList.Where(x => x.OrderDetailId == Id).FirstOrDefault();
                            old.State = 3;
                            List<OrderDetail> orderDetails = new List<OrderDetail>() { old };

                            string newRoomSession = null;
                            long UpdateTime;
                            List<OrderDetail> newOrderDetails;


                            ResultModel result = OperatesService.GetOperates().ServiceDelOrderDetail(order, orderDetails, RoomStateSession, out newRoomSession, out newOrderDetails, out UpdateTime);

                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (result.Result)
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("DeleteSuccess"), null, PopupType.Information));
                                    RoomModel model = Resources.GetRes().RoomsModel.Where(x => null != x.PayOrder && x.PayOrder.OrderId == order.OrderId).FirstOrDefault();
                                    OrderDetail oldModel = model.PayOrder.tb_orderdetail.Where(x => x.OrderDetailId == Id).FirstOrDefault();

                                    oldModel.State = 3;

                                    OrderDetail oldModel2 = resultList.Where(x => x.OrderDetailId == Id).FirstOrDefault();

                                    int no = resultList.IndexOf(oldModel2);
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
                                        Product product = Resources.GetRes().Products.Where(x => x.ProductId == item.ProductId).FirstOrDefault();
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
                                    

                                    // 按理说这里需要刷新上一页中的该房间模型, 但是每次返回它都会刷新, 所以忽略了
                                    
                                    
                                    Init(this.RoomId);
                                   
                                }
                                else
                                {
                                    if (result.IsRefreshSessionModel)
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenRefreshModel"), null, PopupType.Warn));
                                        _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Room, this.RoomId));
                                    }
                                    else if (result.IsSessionModelSameTimeOperate)
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("FaildThenWaitRetry"), null, PopupType.Warn));
                                    }
                                    else
                                    {
                                        _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("DeleteFailt"), null, PopupType.Warn));
                                    }
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            _element.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                                }), false, Resources.GetRes().GetString("DeleteFailt"));
                            }));
                        }
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                        }));
                    });
                

                }, PopupType.Question));

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Warn));
                }), false, Resources.GetRes().GetString("DeleteFailt"));
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



        private LanguageViewModel _language;
        /// <summary>
        /// 显示已选控件
        /// </summary>
        public LanguageViewModel Language
        {
            get { return _language; }
            set
            {
                _language = value;
                OnPropertyChanged("Language");
            }
        }





        private RequestViewModel _request;
        /// <summary>
        /// 请求控件
        /// </summary>
        public RequestViewModel Request
        {
            get { return _request; }
            set
            {
                _request = value;
                OnPropertyChanged("Request");
            }
        }




        private ChangeCountViewModel _changeCount;
        /// <summary>
        /// 修改数量控件
        /// </summary>
        public ChangeCountViewModel ChangeCount
        {
            get { return _changeCount; }
            set
            {
                _changeCount = value;
                OnPropertyChanged("ChangeCount");
            }
        }


        private ChangePriceViewModel _changePrice;
        /// <summary>
        /// 修改价格控件
        /// </summary>
        public ChangePriceViewModel ChangePrice
        {
            get { return _changePrice; }
            set
            {
                _changePrice = value;
                OnPropertyChanged("ChangePrice");
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


        private ChangeTimeViewModel _changeTime;
        /// <summary>
        /// 修改时间控件
        /// </summary>
        public ChangeTimeViewModel ChangeTime
        {
            get { return _changeTime; }
            set
            {
                _changeTime = value;
                OnPropertyChanged("ChangeTime");
            }
        }

        


        private SearchViewModel _search;
        /// <summary>
        /// 搜索控件
        /// </summary>
        public SearchViewModel Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnPropertyChanged("Search");
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
        /// 后退按钮
        /// </summary>
        private RelayCommand _backCommand;
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(param => _element.RaiseEvent(new ForwardRoutedEventArgs(PublicEvents.ForwardEvent, null, PageType.Back)));
                }
                return _backCommand;
            }
        }




        /// <summary>
        /// 打开产品列表按钮
        /// </summary>
        private RelayCommand _selectedCommand;
        public ICommand SelectedCommand
        {
            get
            {
                if (_selectedCommand == null)
                {
                    _selectedCommand = new RelayCommand(param => DisplayMode = 1);
                }
                return _selectedCommand;
            }
        }


        /// <summary>
        /// 打开产品列表按钮
        /// </summary>
        private RelayCommand _productsCommand;
        public ICommand ProductsCommand
        {
            get
            {
                if (_productsCommand == null)
                {
                    _productsCommand = new RelayCommand(param => DisplayMode = 2);
                }
                return _productsCommand;
            }
        }




        /// <summary>
        /// 切换语言按钮
        /// </summary>
        private RelayCommand _changeOrderLanguageCommand;
        public ICommand ChangeOrderLanguageCommand
        {
            get
            {
                if (_changeOrderLanguageCommand == null)
                {
                    _changeOrderLanguageCommand = new RelayCommand(param =>
                        { 
                            // 是否允许更改语言
                            if (null == this.order || Common.GetCommon().IsAllowChangeLanguage())
                            {

     
                                this.Language.LanguageMode = Resources.GetRes().GetMainLangByMainLangIndex(Resources.GetRes().GetMainLangByLangIndex(this.Language.LanguageMode).MainLangIndex + 1).LangIndex;
                                

                                this.Selected.RemarkChanged = true;

                                RefreshState();

                                // 刷新第二屏语言
                                if (FullScreenMonitor.Instance._isInitialized)
                                {
                                    FullScreenMonitor.Instance.RefreshSecondMonitorLanguage(Resources.GetRes().GetMainLangByLangIndex(Language.LanguageMode).LangIndex, -1);
                                }
                            }
                            
                        });
                }
                return _changeOrderLanguageCommand;
            }
        }




        /// <summary>
        /// 搜索按钮
        /// </summary>
        private RelayCommand _searchCommand;
        public ICommand SearchCommand
        {
            get
            {
                if (_searchCommand == null)
                {
                    _searchCommand = new RelayCommand(param =>
                    {
                        _element.RaiseEvent(new BoxRoutedEventArgs(PublicEvents.BoxEvent, null, null, null, BoxType.Search, null));

                    });
                }
                return _searchCommand;
            }
        }


       

        /// <summary>
        /// 更新部分
        /// </summary>
        private void RefreshSome(List<long> RoomsId)
        {
            if (RoomsId.Contains(this.RoomId) && !Selected.IsRefresh)
            {
                Selected.IsRefresh = true;
            }
        }



        /// <summary>
        /// 更新按钮
        /// </summary>
        private RelayCommand _refreshCommand;
        public ICommand RefreshCommand
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


                        

                        RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == RoomId).FirstOrDefault();
                        this.RoomId = model.RoomId;
                        this.order = model.PayOrder;
                        this.resultList = (null == model.PayOrder ? null : model.PayOrder.tb_orderdetail.ToList());
                        this.RoomStateSession = model.OrderSession;

                        this.payList = (null == model.PayOrder ? null : model.PayOrder.tb_orderpay.ToList());
                        if (null == payList)
                        {
                            payList = new List<OrderPay>();

                        }
                        tempPayList = payList.ToList();

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
