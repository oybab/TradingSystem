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

namespace Oybab.TradingSystemX.VM.ViewModels.Pages
{
    internal sealed class MainListViewModel : ViewModelBase
    {
        private Page _element;
        private MainListModel _financeLog;
        private MainListModel _openCashDrawer;
        private MainListModel _balanceManager;
        private MainListModel _addInnerBill;
        private MainListModel _addOuterBill;
        private MainListModel _importManager;
        private MainListModel _statistic;

        private Xamarin.Forms.StackLayout _spList;
        private Xamarin.Forms.ControlTemplate _ctControlTemplate;

        public MainListViewModel(Page _element, Xamarin.Forms.StackLayout spList, Xamarin.Forms.ControlTemplate ctControlTemplate)
        {
            this._element = _element;
            
            this._spList = spList;
            this._ctControlTemplate = ctControlTemplate;

            // 菜单列表填充一下



            _addInnerBill = new MainListModel() { Name = "InnerBill", GoCommand = this.GoCommand };
            AddList(_addInnerBill);
            _addOuterBill = new MainListModel() { Name = "OuterBill", GoCommand = this.GoCommand };
            AddList(_addOuterBill);
            _importManager = new MainListModel() { Name = "ExpenditureManager", GoCommand = this.GoCommand };
            AddList(_importManager);
            _financeLog = new MainListModel() { Name = "FinanceLog", GoCommand = this.GoCommand };
            AddList(_financeLog);
            _openCashDrawer = new MainListModel() { Name = "OpenCashDrawer", GoCommand = this.GoCommand };
            AddList(_openCashDrawer);
            _balanceManager = new MainListModel() { Name = "BalanceManager", GoCommand = this.GoCommand };
            AddList(_balanceManager);
            _statistic = new MainListModel() { Name = "Statistic", GoCommand = this.GoCommand };
            AddList(_statistic);
            AddList(new MainListModel() { Name = "ChangeLanguage", GoCommand = this.GoCommand });
            AddList(new MainListModel() { Name = "ChangePassword", GoCommand = this.GoCommand });
            AddList(new MainListModel() { Name = "About", GoCommand = this.GoCommand });
            AddList(new MainListModel() { Name = "Exit", GoCommand = this.GoCommand });


            Notification.Instance.NotificationLanguage += (obj, value, args) => { Device.BeginInvokeOnMainThread(() => { LoginRefresh(); }); };
            Notification.Instance.NotificationLogin += (obj, value, args) => { Device.BeginInvokeOnMainThread(() => { LoginRefresh(); }); };

            // 设置语言
            LoginRefresh();
        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            
        }


       
        private ObservableCollection<MainListModel> _lists = new ObservableCollection<MainListModel>();
        /// <summary>
        /// 菜单列表
        /// </summary>
        public ObservableCollection<MainListModel> Lists
        {
            get { return _lists; }
            set
            {
                _lists = value;
                OnPropertyChanged("Lists");
            }
        }




        /// <summary>
        /// 清空
        /// </summary>
        private void ClearList()
        {
            Lists.Clear();
            
            this._spList.Children.Clear();

        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Index"></param>
        private void AddList(MainListModel model, int Index = -1)
        {

            if (Index != -1)
                Lists.Insert(Index, model);
            else
                Lists.Add(model);

            AddSelectedItem(model, Index);

        }



        /// <summary>
        /// 添加已选对象
        /// </summary>
        /// <param name="item"></param>
        private void AddSelectedItem(MainListModel item, int Index = -1)
        {

            Xamarin.Forms.TemplatedView view = new Xamarin.Forms.TemplatedView();
            view.ControlTemplate = _ctControlTemplate;
            view.BindingContext = item;

            if (Index != -1)
                view.IsVisible = true;
            else
                _spList.Children.Add(view);
        }

        /// <summary>
        /// 删除已选
        /// </summary>
        /// <param name="item"></param>
        private void RemoveSelected(MainListModel item)
        {
            Lists.Remove(item);

            Xamarin.Forms.TemplatedView _view = null;
            foreach (Xamarin.Forms.TemplatedView items in this._spList.Children)
            {
                if (items.BindingContext == item)
                {
                    _view = items;
                    break;
                }
            }

            if (null != _view)
                _view.IsVisible = false;
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



        private string _ownerName;
        /// <summary>
        /// 拥有者名字
        /// </summary>
        public string OwnerName
        {
            get { return _ownerName; }
            set
            {
                _ownerName = value;
                OnPropertyChanged("OwnerName");
            }
        }


        /// <summary>
        /// 登录后刷新一些数据
        /// </summary>
        private void LoginRefresh()
        {
            OwnerName = "";
            if (Res.Instance.MainLangIndex == 0)
                OwnerName = Resources.Instance.KEY_NAME_0;
            else if (Res.Instance.MainLangIndex == 1)
                OwnerName = Resources.Instance.KEY_NAME_1;
            else if (Res.Instance.MainLangIndex == 2)
                OwnerName = Resources.Instance.KEY_NAME_2;



            foreach (var item in Lists)
            {
                item.LangChange();
            }


            // 根据权限显示财务日志
            if (Common.Instance.IsAllowFinancceLog())
            {
                if (!_financeLog.Visibility)
                {
                    _financeLog.Visibility = true;
                    AddList(_financeLog, Lists.Count - 4);
                }
            }
            else
            {
                if (_financeLog.Visibility)
                {
                    _financeLog.Visibility = false;
                    RemoveSelected(_financeLog);
                }
            }



            // 根据权限显示打开钱箱
            if (Common.Instance.IsAllowOpenCashDrawer())
            {
                if (!_openCashDrawer.Visibility)
                {
                    _openCashDrawer.Visibility = true;
                    AddList(_openCashDrawer, Lists.Count - 4);
                }
            }
            else
            {
                if (_openCashDrawer.Visibility)
                {
                    _openCashDrawer.Visibility = false;
                    RemoveSelected(_openCashDrawer);
                }
            }

            // 根据权限显示余额管理
            if (Common.Instance.IsAllowBalanceManager())
            {
                if (!_balanceManager.Visibility)
                {
                    _balanceManager.Visibility = true;
                    AddList(_balanceManager, Lists.Count - 4);
                }
            }
            else
            {
                if (_balanceManager.Visibility)
                {
                    _balanceManager.Visibility = false;
                    RemoveSelected(_balanceManager);
                }
            }


            // 根据权限显示统计
            if (Common.Instance.IsAllowStatistic())
            {
                if (!_statistic.Visibility)
                {
                    _statistic.Visibility = true;
                    AddList(_statistic, Lists.Count - 4);
                }
            }
            else
            {
                if (_statistic.Visibility)
                {
                    _statistic.Visibility = false;
                    RemoveSelected(_statistic);
                }
            }



            // 根据权限显示内部账单账单
            if (Resources.Instance.RoomCount > 0 && Common.Instance.IsAddInnerBill())
            {
                if (!_addInnerBill.Visibility)
                {
                    _addInnerBill.Visibility = true;
                    AddList(_addInnerBill, Lists.Count - 4);
                }
            }
            else
            {
                if (_addInnerBill.Visibility)
                {
                    _addInnerBill.Visibility = false;
                    RemoveSelected(_addInnerBill);
                }
            }

            // 根据权限显示添加外部账单
            if (Common.Instance.IsAddOuterBill())
            {
                if (!_addOuterBill.Visibility)
                {
                    _addOuterBill.Visibility = true;
                    AddList(_addOuterBill, Lists.Count - 4);
                }
            }
            else
            {
                if (_addOuterBill.Visibility)
                {
                    _addOuterBill.Visibility = false;
                    RemoveSelected(_addOuterBill);
                }
            }



            // 根据权限显示支出管理
            if (Common.Instance.IsAllowImportManager())
            {
                if (!_importManager.Visibility)
                {
                    _importManager.Visibility = true;
                    AddList(_importManager, Lists.Count - 4);
                }
            }
            else
            {
                if (_importManager.Visibility)
                {
                    _importManager.Visibility = false;
                    RemoveSelected(_importManager);
                }
            }

        }


        private bool _isOrderInitial = false;
        private bool _isTakeoutInitial = false;
        private bool _isImportInitial = false;


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

                    MainListModel model = param as MainListModel;
                    if (null != model)
                    {
                      
                        switch (model.Name)
                        {
                            // 内部账单
                            case "InnerBill":

                                IsLoading = true;
                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        if (!_isOrderInitial)
                                        {

                                            NavigationPath.Instance.ProductPage = new ProductsPage();
                                            NavigationPath.Instance.SelectedPage = new SelectedPage();

                                            NavigationPath.Instance.ProductNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.ProductPage);
                                            NavigationPath.Instance.SelectedNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.SelectedPage);


                                            NavigationPath.Instance.OrderPage = new OrderPage();


                                            NavigationPath.Instance.CheckoutPage = new CheckoutPage();
                                            NavigationPath.Instance.ReplaceRoomPage = new ReplaceRoomPage();

                                            _isOrderInitial = true;
                                        }

                                        NavigationPath.Instance.RoomListPage.Init();
                                        NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.RoomListPage);
                                        IsLoading = false;
                                    });
                                });
                                break;
                            // 外部账单
                            case "OuterBill":

                                IsLoading = true;
                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                    Device.BeginInvokeOnMainThread(() =>
                                    {

                                        if (!_isTakeoutInitial)
                                        {
                                            NavigationPath.Instance.TakeoutProductPage = new ProductsPage();
                                            NavigationPath.Instance.TakeoutSelectedPage = new SelectedPage();

                                            NavigationPath.Instance.TakeoutProductNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.TakeoutProductPage);
                                            NavigationPath.Instance.TakeoutSelectedNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.TakeoutSelectedPage);

                                            NavigationPath.Instance.TakeoutPage = new TakeoutPage();
                                            NavigationPath.Instance.TakeoutCheckoutPage = new TakeoutCheckoutPage();
                                            NavigationPath.Instance.AddressPage = new AddressPage();

                                            _isTakeoutInitial = true;
                                        }

                                        NavigationPath.Instance.TakeoutPage.Init(()=>
                                        {

                                            NavigationPath.Instance.TakeoutPage.Detail = NavigationPath.Instance.TakeoutProductNavigationPage;

                                            NavigationPath.Instance.InitialMasterDetail(NavigationPath.Instance.TakeoutPage, NavigationPath.Instance.TakeoutProductNavigationPage, NavigationPath.Instance.TakeoutSelectedNavigationPage);

                                        });

                                        

                                      
                                        IsLoading = false;
                                    });
                                });
                                break;
                            // 支出管理
                            case "ExpenditureManager":

                                IsLoading = true;
                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                    Device.BeginInvokeOnMainThread(() =>
                                    {

                                        if (!_isImportInitial)
                                        {
                                            NavigationPath.Instance.ImportProductPage = new ProductsPage();
                                            NavigationPath.Instance.ImportSelectedPage = new SelectedPage();

                                            NavigationPath.Instance.ImportProductNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.ImportProductPage);
                                            NavigationPath.Instance.ImportSelectedNavigationPage = new TradingSystemX.Pages.Navigations.MasterDetailNPage(NavigationPath.Instance.ImportSelectedPage);

                                            NavigationPath.Instance.ImportPage = new ImportPage();
                                            NavigationPath.Instance.ImportCheckoutPage = new ImportCheckoutPage();
                                            NavigationPath.Instance.AddressPage = new AddressPage();

                                            _isImportInitial = true;
                                        }

                                        NavigationPath.Instance.ImportPage.Init(() =>
                                        {

                                            NavigationPath.Instance.ImportPage.Detail = NavigationPath.Instance.ImportProductNavigationPage;

                                            NavigationPath.Instance.InitialMasterDetail(NavigationPath.Instance.ImportPage, NavigationPath.Instance.ImportProductNavigationPage, NavigationPath.Instance.ImportSelectedNavigationPage);

                                        });




                                        IsLoading = false;
                                    });
                                });
                                break;
                            // 财务日志
                            case "FinanceLog":
                                IsLoading = true;
                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                    try
                                    {
                                        

                                        var taskResult = await OperatesService.Instance.ServiceGetLog(1, -1, 0, 0);
                                        bool result = taskResult.result;
                                        List <Oybab.DAL.Log> logs = taskResult.logs;
                                        List<DAL.Balance> Balance = taskResult.balances;

                                        if (result)
                                        {
                                            Device.BeginInvokeOnMainThread(() =>
                                            {
                                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, ReloadBalanceList(Balance), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);

                                            });
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                        {
                                            ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                            {
                                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                                            }));
                                        }));
                                    }

                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                    {
                                        IsLoading = false;
                                    }));
                                });
                                break;
                            case "OpenCashDrawer":

                                //确认取消
                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmOperate"), Resources.Instance.GetString("OpenCashDrawer")), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, (string msg) =>
                                {
                                    if (msg == "NO")
                                        return;

                                    IsLoading = true;
                                    Task.Run(async () =>
                                    {
                                        await ExtX.WaitForLoading();

                                        try
                                        {
                                            int sendType = 18; // OpenCashDrawer

                                            var result = await OperatesService.Instance.ServiceSend(null, sendType, null, "System");


                                            if (!result)
                                            {
                                                Device.BeginInvokeOnMainThread(() =>
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("OperateFaild"), Resources.Instance.GetString("OpenCashDrawer")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null) ;
                                                });
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                            {
                                                ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                                                }));
                                            }));
                                        }

                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(new Action(() =>
                                        {
                                            IsLoading = false;
                                        }));
                                    });
                                }, null);
                                break;
                            // 余额管理
                            case "BalanceManager":
                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await (NavigationPath.Instance.BalancePage.BindingContext as BalanceManagerViewModel).Refresh();

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.BalancePage.Init();
                                        NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.BalancePage);
                                        IsLoading = false;
                                    });
                                });

                                break; 
                            // 统计
                            case "Statistic":
                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.Sleep(200);

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.StatisticPage.Init();
                                        NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.StatisticPage);
                                        IsLoading = false;
                                    });
                                });


                                break;
                            // 修改语言
                            case "ChangeLanguage":

                                Dictionary<string, string> langs = new Dictionary<string, string>();
                                foreach (var item in Res.Instance.AllLangList)
                                {
                                    langs.Add(Res.Instance.GetString("LangName", item.Value.Culture), item.Value.LangIndex.ToString());
                                }

                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("ChangeLanguage")), MessageBoxMode.Sheet, MessageBoxImageMode.Information, MessageBoxButtonMode.CustomMultiple, (string operate) =>
                                {
                                    int langIndex = int.Parse(operate);

                                    Device.BeginInvokeOnMainThread(async () =>
                                    {
                                        if (Res.Instance.CurrentLangIndex != langIndex)
                                        {
                                            IsLoading = true;


                                            await ExtX.Sleep(1000);

                                            LangConverter.Instance.ChangeCulture(langIndex);
                                            Notification.Instance.ActionLanguage(null, langIndex, null);

                                            await Common.Instance.SetBak();

                                            await ExtX.Sleep(500);

                                            IsLoading = false;

                                        }
                                    });

                                }, langs);
                                break;
                            // 修改密码
                            case "ChangePassword":
                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.ChangePasswordPage.Init();
                                        NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.ChangePasswordPage);
                                        IsLoading = false;
                                    });
                                });
                                break;
                            // 关于
                            case "About":
                                IsLoading = true;

                                Task.Run(async () =>
                                {
                                    await ExtX.WaitForLoading();

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        NavigationPath.Instance.AboutPage.Init();
                                        NavigationPath.Instance.GoNavigateNext(NavigationPath.Instance.AboutPage);
                                        IsLoading = false;
                                    });
                                });
                                break;
                            // 退出
                            case "Exit":
                                IsLoading = true;
                                try
                                {
                                    Task.Run(async () =>
                                    {
                                        await ExtX.WaitForLoading();

                                        Resources.Instance.IsSavePassword = false;
                                        Resources.Instance.LastLoginPassword = null;
                                        await Common.Instance.SetBak();

                                        Device.BeginInvokeOnMainThread(() =>
                                        {
                                            Common.Instance.Exit();

                                            IsLoading = false;
                                        });
                                    });
                                }
                                catch (Exception ex)
                                {
                                    ExceptionPro.ExpLog(ex);
                                }
                                
                                break;
                        }
                    }


                }));
            }
        }



        /// <summary>
        /// 刷新支付列表
        /// </summary>
        private string ReloadBalanceList(List<Oybab.DAL.Balance> balances)
        {
            string Log = "";
            if (null != balances && balances.Count > 0)
            {

                foreach (var item in balances)
                {
                    Oybab.DAL.Balance currentBalance = Resources.Instance.Balances.Where(x => x.BalanceId == item.BalanceId).FirstOrDefault();

                    if (Res.Instance.MainLangIndex == 0)
                        Log += currentBalance.BalanceName0;
                    else if (Res.Instance.MainLangIndex == 1)
                        Log += currentBalance.BalanceName1;
                    else if (Res.Instance.MainLangIndex == 2)
                        Log += currentBalance.BalanceName2;

                    Log += "  ";

                    Log += Resources.Instance.PrintInfo.PriceSymbol + item.BalancePrice + "";

                    Log += Environment.NewLine;


                }

            }
            return Log;
        }


    }
}
