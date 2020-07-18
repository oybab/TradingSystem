using Oybab.Res.Exceptions;
using Oybab.Res.Tools;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.ViewModels.Pages;
using Oybab.ServiceTablet.Pages;
using Oybab.ServiceTablet.Resources.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Oybab.ServiceTablet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 窗口
        private RoomPage roomPage;
        private OrderPage orderPage = new OrderPage();
        private CheckoutPage checkoutPage = new CheckoutPage();
        private SystemPage systemPage = new SystemPage();
        private LoginWindow loginWindow;
        private TakeoutPage takeoutPage = new TakeoutPage();
        private TakeoutCheckoutPage takeoutCheckoutPage = new TakeoutCheckoutPage();
        private ImportPage importPage = new ImportPage();
        private ImportCheckoutPage importCheckoutPage = new ImportCheckoutPage();

        public MainWindow(LoginWindow loginWindow)
        {
            // 这就是主窗口
            Application.Current.MainWindow = this;

            this.loginWindow = loginWindow;
            roomPage = new RoomPage(this);

            InitializeComponent();

#if !DEBUG
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Topmost = true;

            // 防止超出第一屏幕在第二屏幕边缘也显示
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
#endif


            // 添加处理事件
            this.AddHandler(PublicEvents.ForwardEvent, new RoutedEventHandler(HandleForward), true);


            MainViewModel viewModel = new MainViewModel(this, new Action(() =>
            {
                new Action(() =>
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        loginWindow.Init();
                        loginWindow.Show();
                        this.Hide();
                        return;
                    }));
                }).BeginInvoke(null, null);
            }), new Action(() =>
            {
                roomPage.RefreshRoomList();
            }));

            viewModel.Init();
            this.DataContext = viewModel;



           



            //鼠标不需要显示
            if (!Res.Resources.GetRes().DisplayCursor)
                Mouse.OverrideCursor = Cursors.None;

            // 注册热键(开钱箱)
            //RegHotKey();
            RoutedCommand openCashCmd = new RoutedCommand();
            openCashCmd.InputGestures.Add(new KeyGesture(Key.Z, ModifierKeys.Alt));
            CommandBindings.Add(new CommandBinding(openCashCmd, (x,y) =>
            {
                Common.GetCommon().OpenCashDrawer();
            }));

            
            InitAnimation();



            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

            this.Loaded += (x, y) =>
            {
                if (!_isLoaded)
                {
                    _isLoaded = true;

                    // 注册热键
                    RegHotKey();
                    //RegHotKey2();

                    // 点歌系统设置呼叫器时间(消准)
                    if (Res.Resources.GetRes().IsRequired("Vod"))
                    {
                        // 设置设备(呼叫器)时间
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            Common.GetCommon().SetDeviceTime();
                        });
                    }


                    // 加载第二屏幕
                    if (Res.Resources.GetRes().DisplaySecondMonitor)
                    {
                        FullScreenMonitor.Instance.Initial((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);
                    }
                }
            };
            
        }

        private bool _isLoaded = false;





        private Hotkey hk = new Hotkey();
        /// <summary>
        /// 注册热键
        /// </summary>
        private void RegHotKey()
        {
            if (string.IsNullOrWhiteSpace(Res.Resources.GetRes().CashDrawer))
                return;
            try
            {
                hk.KeyCode = System.Windows.Forms.Keys.Z;
                hk.Alt = true;
                hk.Pressed += delegate
                {
                    Common.GetCommon().OpenCashDrawer();
                };

                if (!hk.GetCanRegister(this))
                { ExceptionPro.ExpErrorLog("Open cash drawer key can't register!"); }
                else
                { hk.Register(this); }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }




        private Hotkey hk2 = new Hotkey();
        /// <summary>
        /// 注册热键(为备注)
        /// </summary>
        private void RegHotKey2()
        {
            try
            {
                hk2.KeyCode = System.Windows.Forms.Keys.F9;
                hk2.Pressed += delegate
                {
                    Oybab.Res.View.Component.RemarkEvent.Instance.ActionRemark(null, null, null);
                };

                if (!hk2.GetCanRegister(this))
                { ExceptionPro.ExpErrorLog("Open remark key can't register!"); }
                else
                { hk2.Register(this); }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }



        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(bool IsFirst)
        {
            roomPage.Init((long)0);
            
            // 第一次切换到首页
            if (IsFirst)
            {
                 if (Res.Resources.GetRes().RoomCount <= 0 || !Common.GetCommon().IsAddInnerBill())
                    mainFrame.Navigate(systemPage);
                 else
                    mainFrame.Navigate(roomPage);
            }
            else
            {
                MainViewModel mainViewModel = this.DataContext as MainViewModel;
                if (null != mainViewModel)
                    mainViewModel.Resize();
            }
        }

        /// <summary>
        /// 处理旋转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowState = System.Windows.WindowState.Maximized;
        }


        /// <summary>
        /// 处理跳转路由
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleForward(object sender, RoutedEventArgs args)
        {
            ForwardRoutedEventArgs forwardArgs = args as ForwardRoutedEventArgs;
            if (null != forwardArgs)
            {
               
                BeforeLoad(forwardArgs.PageType == PageType.Back || forwardArgs.PageType == PageType.Room || forwardArgs.PageType == PageType.TakeoutBack || forwardArgs.PageType == PageType.ImportBack);



                switch (forwardArgs.PageType)
                {
                    //雅座页面
                    case PageType.Room:
                        mainFrame.Navigate(roomPage);
                        if (null != forwardArgs.Param)
                            roomPage.Init(forwardArgs.Param);
                        break;
                    //订单页面
                    case PageType.Order:
                        mainFrame.Navigate(orderPage);
                        orderPage.Init(forwardArgs.Param);
                        break;
                    //结账页面
                    case PageType.CheckoutOrder:
                        mainFrame.Navigate(checkoutPage);
                        checkoutPage.Init(forwardArgs.Param);
                        break;
                    //外卖页面
                    case PageType.Takeout:
                        mainFrame.Navigate(takeoutPage);
                        takeoutPage.Init();
                        break;
                    //返回外卖页面
                    case PageType.TakeoutBack:
                        mainFrame.Navigate(takeoutPage);
                        //takeoutPage.Init();

                        mainFrame.GoBack();
                        takeoutPage.Init();

                        break;
                    //结账外卖页面
                    case PageType.CheckoutTakeout:
                        mainFrame.Navigate(takeoutCheckoutPage);
                        takeoutCheckoutPage.Init(forwardArgs.Param);
                        break;
                    //支出页面
                    case PageType.Import:
                        mainFrame.Navigate(importPage);
                        importPage.Init();
                        break;
                    //返回支出页面
                    case PageType.ImportBack:
                        mainFrame.Navigate(importPage);
                        //importPage.Init();

                        mainFrame.GoBack();
                        importPage.Init();

                        break;
                    //结账支出页面
                    case PageType.CheckoutImport:
                        mainFrame.Navigate(importCheckoutPage);
                        importCheckoutPage.Init(forwardArgs.Param);
                        break;
                    //关于页面
                    case PageType.About:
                       
                        break;
                    //切换语言页面
                    case PageType.Language:
                      
                        break;
                    //修改密码页面
                    case PageType.ChangePassword:

                        break;
                    //退出页面
                    case PageType.Exit:
                        this.Close();
                        break;
                    //系统页面
                    case PageType.System:
                        mainFrame.Navigate(systemPage);
                        systemPage.Init(forwardArgs.Param);
                        break;
                    //返回页面
                    case PageType.Back:

                        if (mainFrame.CanGoBack)
                            mainFrame.GoBack();


                        // 有些页面返回后需要刷新
                        Type type = mainFrame.Content.GetType();

                        // 刷新包厢
                        if (type == typeof(OrderPage))
                            roomPage.Init((long)-1);
                        // 刷新客显
                        else if (type == typeof(CheckoutPage))
                            orderPage.RefreshPM();
                        // 刷新客显
                        else if (type == typeof(TakeoutCheckoutPage))
                            takeoutPage.RefreshPM();
                        // 刷新客显
                        else if (type == typeof(ImportCheckoutPage))
                            importPage.RefreshPM();
                        // 刷新客显
                        else if (type == typeof(TakeoutPage))
                        {
                            Common.GetCommon().OpenPriceMonitor("0");
                            // 刷新第二屏幕
                            if (FullScreenMonitor.Instance._isInitialized)
                            {
                                FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
                            }
                        }

                        break;
                    //清空历史
                    case PageType.Clear:
                        mainFrame.Navigate(roomPage);
                        if (mainFrame.CanGoBack)
                            mainFrame.RemoveBackEntry();
                        break;
                    default:
                        break;
                }

                AfterLoad();
            }
        }




        #region 导航动画
        private Duration _duration = new Duration(TimeSpan.FromSeconds(0.3));
        private Duration _durationOriginal = new Duration(TimeSpan.FromSeconds(0));
        private TimeSpan _durationTimeFirst = new TimeSpan(0, 0, 0);
        private KeySpline _durationKeySpline = new KeySpline(0.57, 1.0, 0.22, 0.80);
        private TimeSpan _durationTimeSecond = new TimeSpan(0, 0, 0, 0, 300);

        // 旋转图
        private RenderTargetBitmap rtbitmap;
        /// <summary>
        /// 绘制图
        /// </summary>
        private void SetImage()
        {
            if (null == rtbitmap)
            {

                var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
                var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

                var dpiX = (int)dpiXProperty.GetValue(null, null);
                var dpiY = (int)dpiYProperty.GetValue(null, null);


                rtbitmap = new RenderTargetBitmap((int)grdPage.ActualWidth * dpiX / 96, (int)grdPage.ActualHeight * dpiY / 96, dpiX, dpiY, PixelFormats.Pbgra32);
                imgPage.Source = rtbitmap;

                imgPage.CacheMode = new BitmapCache();
            }

            rtbitmap.Clear();
            rtbitmap.Render(grdPage);


        }



        int imgFrom, imgTo, grdFrom, grdTo;
        /// <summary>
        /// 加载前
        /// </summary>
        /// <param name="IsRight"></param>
        private void BeforeLoad(bool IsRight)
        {
            if (IsRight)
            {
                imgFrom = 0;
                imgTo = (int)grdTopPage.ActualWidth;
                grdFrom = -(int)grdTopPage.ActualWidth / 3;
                grdTo = 0;



                SetImage();
                grdImgPage.Visibility = System.Windows.Visibility.Visible;

                Panel.SetZIndex(grdTopPage, 0);
                Panel.SetZIndex(grdImgPage, 1);
            }
            else
            {
                imgFrom = 0;
                imgTo = -(int)grdTopPage.ActualWidth / 3;
                grdFrom = (int)grdTopPage.ActualWidth; ;
                grdTo = 0;


                SetImage();
                grdImgPage.Visibility = System.Windows.Visibility.Visible;



                Panel.SetZIndex(grdTopPage, 1);
                Panel.SetZIndex(grdImgPage, 0);
            }
        }



        private Storyboard storyBoards = new Storyboard();

        private DoubleAnimationUsingKeyFrames animation0 = new DoubleAnimationUsingKeyFrames();
        private DoubleAnimationUsingKeyFrames animation1 = new DoubleAnimationUsingKeyFrames();

        /// <summary>
        /// 初始化动画参数
        /// </summary>
        private void InitAnimation()
        {
            animation0.Duration = _duration;
            animation1.Duration = _duration;

            Storyboard.SetTarget(animation0, grdTopPage);
            Storyboard.SetTarget(animation1, grdImgPage);

            Storyboard.SetTargetProperty(animation0, new PropertyPath("RenderTransform.(TranslateTransform.X)"));

            Storyboard.SetTargetProperty(animation1, new PropertyPath("RenderTransform.(TranslateTransform.X)"));

            storyBoards.Children.Add(animation0);
            storyBoards.Children.Add(animation1);
            storyBoards.SetValue(Storyboard.DesiredFrameRateProperty, 30);//动画不需要高帧率，而系统默认为60frames/sec
            storyBoards.Completed -= storyBoards_Completed;
            storyBoards.Completed += storyBoards_Completed;
        }

        /// <summary>
        /// 加载后
        /// </summary>
        private void AfterLoad()
        {

            animation0.KeyFrames.Clear();
            animation1.KeyFrames.Clear();
            
            
            animation0.KeyFrames.Add(new DiscreteDoubleKeyFrame(grdFrom, _durationTimeFirst));

            animation0.KeyFrames.Add(new SplineDoubleKeyFrame(grdTo, _durationTimeSecond, _durationKeySpline));

            animation1.KeyFrames.Add(new DiscreteDoubleKeyFrame(imgFrom, _durationTimeFirst));

            animation1.KeyFrames.Add(new SplineDoubleKeyFrame(imgTo, _durationTimeSecond, _durationKeySpline));

            

            
            storyBoards.Begin();

        }

        private void storyBoards_Completed(object sender, EventArgs e)
        {
            grdImgPage.Visibility = System.Windows.Visibility.Collapsed;
        }

                

                
        #endregion 导航动画

        /// <summary>
        /// 关闭应用程序
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown(0);
        }
       
    }
}
