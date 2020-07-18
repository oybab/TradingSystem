using Oybab.TradingSystemX.Pages;
using Oybab.TradingSystemX.Pages.Controls;
using Oybab.TradingSystemX.Pages.Navigations;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace Oybab.TradingSystemX.Tools
{
    public sealed class NavigationPath
    {
        #region Instance
        private NavigationPath() { }

        private static readonly Lazy<NavigationPath> _instance = new Lazy<NavigationPath>(() => new NavigationPath());
        public static NavigationPath Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance


        private Dictionary<NavigationPage, List<Page>> NavigationList = new Dictionary<NavigationPage, List<Page>>();
        private Dictionary<NavigationPage, List<Page>> NavigationMasterDetailList = new Dictionary<NavigationPage, List<Page>>();

        /// <summary>
        /// 进入下一页
        /// </summary>
        /// <param name="page"></param>
        internal void GoNavigateNext(Page page, bool IsAnimate = true, bool IsModal = false)
        {
            //if (NavigationList[CurrentNavigate].Peek() == page)
            //    NavigationList[CurrentNavigate].Pop();   
            if (NavigationList[CurrentNavigate].Contains(page))
                NavigationList[CurrentNavigate].Remove(page);

            NavigationList[CurrentNavigate].Add(page);

            if (IsModal)
                CurrentNavigate.Navigation.PushModalAsync(page, IsAnimate);
            else
                CurrentNavigate.PushAsync(page, IsAnimate);

        }



        /// <summary>
        /// 返回
        /// </summary>
        internal void GoNavigateBack(bool IsAnimate = true, bool IsModal = false)
        {
            if (NavigationList[CurrentNavigate].Count > 0)
            {
                NavigationList[CurrentNavigate].RemoveAt(NavigationList[CurrentNavigate].Count - 1);

                if (IsModal)
                    CurrentNavigate.Navigation.PopModalAsync(IsAnimate);
                else
                    CurrentNavigate.PopAsync(IsAnimate);
            }

        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public bool GoBack()
        {

            if (NavigationMode == 1 && CurrentNavigate.Navigation.NavigationStack.Count > 1)
            {
                GoNavigateBack();
                return true;
            }
            else if (NavigationMode == 2 && CurrentMasterDetailNavigate.Navigation.NavigationStack.Count > 1)
            {
                GoMasterDetailNavigateBack();
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取当前页
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack
        {
            get
            {

                if (this.NavigationMode == 3 || this.NavigationMode == 4)
                {
                    return false;
                }
                //else if (this.NavigationMode == 2 && null != CurrentMasterDetailPage && (CurrentMasterDetailPage == CheckoutPage || CurrentMasterDetailPage == TakeoutCheckoutPage || CurrentMasterDetailPage == ImportCheckoutPage|| CurrentMasterDetailPage == NewMemberPage))
                //{
                //    return true;
                //}
                // Order 返回到 RoomList 或者Takeout, import 返回到 Main
                else if (this.NavigationMode == 2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 关闭结账
        /// </summary>
        /// <returns></returns>
        public bool CloseModelPages()
        {
            if (this.NavigationMode == 2 && null != CurrentMasterDetailPage && (CurrentMasterDetailPage == CheckoutPage || CurrentMasterDetailPage == TakeoutCheckoutPage || CurrentMasterDetailPage == ImportCheckoutPage || CurrentMasterDetailPage == NewMemberPage))
            {
                NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// 获取当前页
        /// </summary>
        /// <returns></returns>
        internal Page CurrentPage
        {
            get
            {
                if (NavigationList[CurrentNavigate].Count != 0)
                    return NavigationList[CurrentNavigate].LastOrDefault();
                else
                    return null;
            }
        }


        private NavigationPage _currentNavigate;
        /// <summary>
        /// 获取当前页
        /// </summary>
        /// <returns></returns>
        internal NavigationPage CurrentNavigate
        {
            get
            {
                return _currentNavigate;
            }
        }

     

        /// <summary>
        /// 切换主导航
        /// </summary>
        /// <param name="navigate"></param>
        internal void SwitchNavigate(int navigate)
        {
            if (navigate == 0)
            {
                _currentNavigate = loginNavigation;
                NavigationMode = 0;
            }
            else if (navigate == 1)
            {
                _currentNavigate = mainNavigation;
                NavigationMode = 1;
            }

            App.Current.MainPage = _currentNavigate;
        }

        // 0为Login, 1为Main, 2 为Order, Takeout. 3为Order和Takeout的弹出页面.  4为Checkout的弹出页面
        public int NavigationMode = 0;


        /// <summary>
        /// 初始化导航
        /// </summary>
        /// <param name="loginNavigation"></param>
        /// <param name="mainNavigation"></param>
        internal void InitialLoginNavigations(NavigationPage loginNavigation)
        {
            this.loginNavigation = loginNavigation;
            NavigationList.Add(loginNavigation, new List<Page>());
            
        }

        /// <summary>
        /// 初始化主界面导航
        /// </summary>
        /// <param name="mainNavigation"></param>
        internal void InitialMainNavigations(NavigationPage mainNavigation)
        {
            this.mainNavigation = mainNavigation;
            NavigationList.Add(mainNavigation, new List<Page>());
        }






























        private NavigationPage loginNavigation;
        private NavigationPage mainNavigation;

        private MasterDetailPage orderNavigation;
        private NavigationPage productNavigation;
        private NavigationPage selectedNavigation;





        /// <summary>
        /// 获取当前页
        /// </summary>
        /// <returns></returns>
        internal Page CurrentMasterDetailPage
        {
            get
            {
                if (NavigationMasterDetailList[CurrentMasterDetailNavigate].Count != 0)
                    return NavigationMasterDetailList[CurrentMasterDetailNavigate].LastOrDefault();
                else
                    return null;
            }
        }



        
        /// <summary>
        /// 获取当前页
        /// </summary>
        /// <returns></returns>
        public MasterDetailPage CurrentMasterDetail
        {
            get
            {
                return orderNavigation;
            }
        }


        /// <summary>
        /// 是否在主页面
        /// </summary>
        /// <returns></returns>
        public bool IsGoBackground
        {
            get
            {
                if ((NavigationMode == 1 && CurrentNavigate?.Navigation?.NavigationStack.LastOrDefault() == MainListPage))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 是否在登录页面
        /// </summary>
        /// <returns></returns>
        public bool IsExit
        {
            get
            {
                if (NavigationMode == 0 &&  CurrentNavigate?.Navigation?.NavigationStack.LastOrDefault() == NewLoginPage)
                    return true;
                else
                    return false;
            }
        }


        private NavigationPage _currentMasterDetailNavigate;
        /// <summary>
        /// 获取当前页
        /// </summary>
        /// <returns></returns>
        internal NavigationPage CurrentMasterDetailNavigate
        {
            get
            {
                if (null == _currentMasterDetailNavigate)
                {
                    if (null != orderNavigation)
                    {
                        _currentMasterDetailNavigate = orderNavigation.Detail as NavigationPage;
                    }
                    
                }
                return _currentMasterDetailNavigate;
            }
        }






        /// <summary>
        /// 进入下一页
        /// </summary>
        /// <param name="page"></param>
        internal void GoMasterDetailsNavigateNext(Page page, bool IsAnimate = true, bool IsModal = false)
        {

            //if (NavigationMasterDetailList[CurrentMasterDetailNavigate].Peek() == page)
            //    NavigationMasterDetailList[CurrentMasterDetailNavigate].Pop();
            if (NavigationMasterDetailList[CurrentMasterDetailNavigate].Contains(page))
                NavigationMasterDetailList[CurrentMasterDetailNavigate].Remove(page);

            NavigationMasterDetailList[CurrentMasterDetailNavigate].Add(page);

            if (IsModal)
                CurrentMasterDetailNavigate.Navigation.PushModalAsync(page, IsAnimate);
            else
                CurrentMasterDetailNavigate.Navigation.PushAsync(page, IsAnimate);

        }


        /// <summary>
        /// 返回
        /// </summary>
        internal void GoMasterDetailNavigateBack(bool IsAnimate = true, bool IsModal = false)
        {
            if (NavigationMasterDetailList[CurrentMasterDetailNavigate].Count > 0)
            {
                NavigationMasterDetailList[CurrentMasterDetailNavigate].RemoveAt(NavigationMasterDetailList[CurrentMasterDetailNavigate].Count - 1);

                if (IsModal)
                    CurrentMasterDetailNavigate.Navigation.PopModalAsync(IsAnimate);
                else
                    CurrentMasterDetailNavigate.Navigation.PopAsync(IsAnimate);


            }

        }

        

        // 防止打开checkout中返回到了登录页面, 导致下次登录后无法进入checkout页面的BUG
        internal void BackMasterDetailsNavigationPage()
        {
            if (NavigationMode == 2 && null != CurrentMasterDetailPage  && (CurrentMasterDetailPage == CheckoutPage || CurrentMasterDetailPage == TakeoutCheckoutPage || CurrentMasterDetailPage == ImportCheckoutPage || CurrentMasterDetailPage == NewMemberPage))
                GoMasterDetailNavigateBack(false, true);
        }



        /// <summary>
        /// 切换主导航
        /// </summary>
        /// <param name="navigate"></param>
        internal void SwitchMasterDetailNavigate(int navigate)
        {

            if (navigate == 0)
                _currentMasterDetailNavigate = productNavigation;
            else if (navigate == 1)
                _currentMasterDetailNavigate = selectedNavigation;



            if (CurrentMasterDetailNavigate != orderNavigation.Detail)
                orderNavigation.Detail = CurrentMasterDetailNavigate;


            
        }


        private bool _isInitialMasterDetailAlready = false;
        /// <summary>
        /// 设置订单页导航
        /// </summary>
        /// <param name="orderPage"></param>
        internal void InitialMasterDetail(MasterDetailPage orderPage, NavigationPage productNavigation, NavigationPage selectedNavigation)
        {
           
            if (!_isInitialMasterDetailAlready || this.orderNavigation != orderPage)
            {
                this.orderNavigation = orderPage;

                this.productNavigation = productNavigation;
                this.selectedNavigation = selectedNavigation;

                NavigationMasterDetailList.Clear();

                NavigationMasterDetailList.Add(productNavigation, new List<Page>());
                NavigationMasterDetailList.Add(selectedNavigation, new List<Page>());

                _currentMasterDetailNavigate = null;

                _isInitialMasterDetailAlready = true;
            }

            App.Current.MainPage = this.orderNavigation;
            NavigationMode = 2;
        }









        internal LoginMainPage LoginNavigation { get; set; }
        internal MainPage MainNavigation { get; set; }
        internal LoginPage NewLoginPage { get; set; }


        internal SettingPage SettingPage { get; set; }


        internal MainListPage MainListPage { get; set; }
        internal RoomListPage RoomListPage { get; set; }
        internal ChangePasswordPage ChangePasswordPage { get; set; }
        internal AboutPage AboutPage { get; set; }

        internal BalancePage BalancePage { get; set; }






        internal ProductsPage ProductPage { get; set; }
        internal SelectedPage SelectedPage { get; set; }

        internal MasterDetailNPage ProductNavigationPage { get; set; }
        internal MasterDetailNPage SelectedNavigationPage { get; set; }


        public OrderPage OrderPage { get; set; }



        internal CheckoutPage CheckoutPage { get; set; }
        internal ReplaceRoomPage ReplaceRoomPage { get; set; }





        internal ProductsPage TakeoutProductPage { get; set; }
        internal SelectedPage TakeoutSelectedPage { get; set; }

        internal MasterDetailNPage TakeoutProductNavigationPage { get; set; }
        internal MasterDetailNPage TakeoutSelectedNavigationPage { get; set; }

        public TakeoutPage TakeoutPage { get; set; }
        internal TakeoutCheckoutPage TakeoutCheckoutPage { get; set; }
        internal AddressPage AddressPage { get; set; }




        internal ProductsPage ImportProductPage { get; set; }
        internal SelectedPage ImportSelectedPage { get; set; }

        internal MasterDetailNPage ImportProductNavigationPage { get; set; }
        internal MasterDetailNPage ImportSelectedNavigationPage { get; set; }

        public ImportPage ImportPage { get; set; }
        internal ImportCheckoutPage ImportCheckoutPage { get; set; }

        internal NewMemberPage NewMemberPage { get; set; }


        // 关闭面板
        internal Action OrderPanelClose;
        internal Action TakeoutPanelClose;
        internal Action ImportPanelClose;

        internal Action OrderCheckoutPaidPanelClose;
        internal Action TakeoutCheckoutPaidPanelClose;
        internal Action ImportCheckoutPaidPanelClose;

        // 关闭页面
        internal Action OrderClose;
        internal Action TakeoutClose;
        internal Action ImportClose;

        // 订单产品和已选页重新加载
        internal Action ReloadOrderProductPage;
        internal Action ReloadOrderSelectPage;

        // 外卖产品和已选页重新加载
        internal Action ReloadTakeoutProductPage;
        internal Action ReloadTakeoutSelectPage;

        // 外卖产品和已选页重新加载
        internal Action ReloadImportProductPage;
        internal Action ReloadImportSelectPage;

        /// <summary>
        /// 关闭订单弹出面板
        /// </summary>
        public void ClosePanels(bool CloseAll)
        {
            NavigationMode = 2;
            
            if (CloseAll)
            {
                if (null != OrderPanelClose)
                    OrderPanelClose();
                if (null != TakeoutPanelClose)
                    TakeoutPanelClose();
                if (null != ImportPanelClose)
                    ImportPanelClose();
            }
        }

        /// <summary>
        /// 关闭结账有关的菜单
        /// </summary>
        /// <param name="CloseAll"></param>
        public void CloseCheckoutPanels(bool CloseAll)
        {
            NavigationMode = 2;

            if (CloseAll)
            {
                if (null != OrderCheckoutPaidPanelClose)
                    OrderCheckoutPaidPanelClose();
                if (null != TakeoutCheckoutPaidPanelClose)
                    TakeoutCheckoutPaidPanelClose();
                if (null != ImportCheckoutPaidPanelClose)
                    ImportCheckoutPaidPanelClose();
            }
        }

        /// <summary>
        /// 打开订单弹出面板
        /// </summary>
        internal void OpenPanel()
        {
            NavigationMode = 3;
        }
        /// <summary>
        /// 打开结账时菜单
        /// </summary>
        internal void OpenCheckoutPaidPanel()
        {
            NavigationMode = 4;
        }


        /// <summary>
        /// 关闭订单页面
        /// </summary>
        public void CloseOrder()
        {
            if (null != OrderClose)
                OrderClose();
        }



        /// <summary>
        /// 关闭Takeout页面
        /// </summary>
        public void CloseTakeout()
        {
            if (null != TakeoutClose)
                TakeoutClose();
        }


        /// <summary>
        /// 关闭Import页面
        /// </summary>
        public void CloseImport()
        {
            if (null != ImportClose)
                ImportClose();
        }


    }
}
