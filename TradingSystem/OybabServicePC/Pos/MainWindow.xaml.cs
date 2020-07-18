using Oybab.Res.Tools;
using Oybab.Res.View.Converters;
using Oybab.Res.View.ViewModels.Pos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Oybab.ServicePC.Pos
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        TakeoutViewModel viewModel = null;
        public MainWindow()
        {
            InitializeComponent();



            // 显示隐藏状态更改
            this.IsVisibleChanged += (x, y) =>
            {
                lock (IsOpeningObj)
                {
                    if ((bool)y.NewValue)
                    {
                        IsOpening = true;
                    }
                    else
                    {
                        IsOpening = false;
                    }
                }
            };
            


            this.Activated += (x, y) =>
            {
                lock (IsOpeningObj)
                {
                    IsActiving = true;
                }
            };

            this.Deactivated += (x, y) =>
            {
                lock (IsOpeningObj)
                {
                    IsActiving = false;
                }
            };

            this.GotKeyboardFocus += (x, y) =>
            {
                lock (IsOpeningObj)
                {
                    IsFocusing = true;
                }
            };

            this.LostKeyboardFocus += (x, y) =>
            {
                lock (IsOpeningObj)
                {
                    IsFocusing = false;
                }
            };



            // 保持窗口开启事件
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                for (;;)
                {
                    System.Threading.Thread.Sleep(3000);
                    lock (IsOpeningObj)
                    {
                        if (IsOpening)
                        {
                            if (IsActiving && IsFocusing)
                            {
                                continue;
                            }

                            System.Threading.Tasks.Task.Factory.StartNew(() =>
                            {
                                
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                   
#if !DEBUG
                                    this.Activate();

                                    this.Topmost = true;
#endif

                                    this.Focus();

                                }));
                            });
                        }

                    }
                }
            }, TaskCreationOptions.LongRunning);

            // 貌似这个不会发生, 不用担心(因为不可点, 所以无法获取)
            this.MouseDown += (x, y) => {
                this.Close();
            };

#if !DEBUG
            this.WindowState = System.Windows.WindowState.Maximized;
            this.Topmost = true;

            // 防止超出第一屏幕在第二屏幕边缘也显示
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
#endif



            // 计算两个List的高度参数
            CalcSize();
            viewModel = new TakeoutViewModel(this, this.ctrProducts.grdListParent, this.ctrSelected.grdListParent, this.ctrCheckout);
            this.DataContext = viewModel;

            this.Loaded += (x, y) =>
            {
                if (!IsLoadedIt)
                {
                    IsLoadedIt = true;
 
                    viewModel.Init();

                }
            };



        }

        private bool IsFocusing = false;
        private bool IsActiving = false;
        private bool IsOpening = false;
        private object IsOpeningObj = new object();


        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            viewModel.Init();
        }

        internal bool IsLoadedIt = false;


        /// <summary>
        /// 计算尺寸
        /// </summary>
        private void CalcSize() {
            // 价格区高度
            double priceSize = 165 + 3;
            // 其他区高度
            double otherSize = 82 + 5 + 3 + 128 + 4;

#if DEBUG
            Res.Resources.GetRes().setSize(new Size(1024, 768));
#endif

            // 已选和产品列表高度
            double selectedListSize = Res.Resources.GetRes().Size.Height - otherSize - priceSize  - (46 + 30 + 35 + 1 + 10 + 4); // 后面这个是标题+线+margin5+5, 后面按个4不知道哪儿忘记的反正对比后发现少了4
            double productListSize = selectedListSize + priceSize;

            // 每行高度
            int rowHeight = 36;

            // 计算每页能放入多少个数据(已选)
            PosLine.ListCountSelected = (int)(selectedListSize / rowHeight);
            // 计算每页能放入多少个数据(产品)
            PosLine.ListCountProduct = (int)(productListSize / rowHeight);


            // 高低的间距
            if (selectedListSize > rowHeight * PosLine.ListCountSelected + 4)
            {
                double LeftHeighMargin = selectedListSize - (rowHeight * PosLine.ListCountSelected);

                double selectedMargin = (LeftHeighMargin - 2) / PosLine.ListCountSelected / 2;

                PosLine.MarginSelectedList = new Thickness(0, selectedMargin, 0, selectedMargin);
            }
            else
            {
                PosLine.MarginSelectedList = new Thickness(0, 0, 0, 0);
            }




            // 高低的间距
            if (productListSize > rowHeight * PosLine.ListCountProduct + 4)
            {
                double LeftHeighMargin = productListSize - (rowHeight * PosLine.ListCountProduct);

                double productMargin = (LeftHeighMargin - 2) / PosLine.ListCountProduct / 2;

                PosLine.MarginSearchList = new Thickness(0, productMargin, 0, productMargin);
            }
            else
            {
                PosLine.MarginSearchList = new Thickness(0, 0, 0, 0);
            }

        }

        private void window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsLoadedIt)
                return;

            viewModel.HandleKey(e);
        }

       

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            e.Cancel = true;
            this.Visibility = Visibility.Hidden;

            // 显示客显(实际客户需要支付的赊账)
                Common.GetCommon().OpenPriceMonitor(null);
            // 刷新第二屏幕
            if (FullScreenMonitor.Instance._isInitialized)
            {
                FullScreenMonitor.Instance.RefreshSecondMonitorList(null);
            }
        }






        bool AltKeyPressed = false;
        private void window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (AltKeyPressed)
            {
                AltKeyPressed = false;
                if (e.SystemKey == Key.Z) Common.GetCommon().OpenCashDrawer();

                // Others ...
            }

            if (e.SystemKey == Key.LeftAlt || e.SystemKey == Key.RightAlt)
            {
                AltKeyPressed = true;
            }
        }




        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;









        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private new bool IsActive(Window wnd)
        {
            // workaround for minimization bug
            // Managed .IsActive may return wrong value
            if (wnd == null) return false;
            return GetForegroundWindow() == new WindowInteropHelper(wnd).Handle;
        }

        private bool IsApplicationActive()
        {


            if (IsActive(this)) return true;
            return false;
        }

        private void Window_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (viewModel.DisplayMode == 3)
            {
                viewModel.Products.HandleSearchKey(e);
            }
        }
    }
}
