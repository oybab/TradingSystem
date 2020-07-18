using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Oybab.ServicePC.SubWindow;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Tools;
using Oybab.ServicePC.DialogWindow;
using Oybab.Res.Exceptions;
using Oybab.ServicePC.Tools;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace Oybab.ServicePC
{
    internal sealed partial class MainWindow : KryptonForm
    {
        private bool IsOpenKeyWindow = false;

        
        private string UpdateUrl;
        private HomeWindow homeWindow;
        private ProductTypeWindow productTypeWindow;
        private ProductWindow productWindow;
        private RoomWindow roomWindow;
        private OrderWindow orderWindow;
        private ImportWindow importWindow;
        private MemberWindow memberWindow;
        private AdminWindow adminWindow;
        private DeviceWindow deviceWindow;
        private PrinterWindow printerWindow;
        private SupplierWindow supplierWindow;
        private RequestWindow requestWindow;
        private AdminLogWindow adminLogWindow;
        private BalanceWindow balanceWindow;
        private StatisticWindow statisticWindow;




        public MainWindow()
        {
            InitializeComponent();

            
            Font font = new Font(Resources.GetRes().GetString("FontName2"), float.Parse(Resources.GetRes().GetString("FontSize")));

            this.Text = Resources.GetRes().GetString("SoftServiceName");
            krplUpdateInfo.Text = Resources.GetRes().GetString("UpdateInfo");

            tsbAout.Text = tsmiAbout.Text = Resources.GetRes().GetString("About");
            tsbExpenditure.Text = tsmiExpenditure.Text = Resources.GetRes().GetString("ExpenditureManager");
            tsbRoom.Text = tsmiRoom.Text = Resources.GetRes().GetString("RoomManager");
            tsbMembers.Text = tsmiMember.Text = Resources.GetRes().GetString("MemberManager");
            tsbProduct.Text = tsmiProduct.Text = Resources.GetRes().GetString("ProductManager");
            tsbProductType.Text = tsmiProductType.Text = Resources.GetRes().GetString("ProductTypeManager");
            tsbMain.Text = tsmiMain.Text = Resources.GetRes().GetString("MainPage");
            tsbOrder.Text = tsmiOrder.Text = Resources.GetRes().GetString("IncomeManager");
            tsbChangePassword.Text = tsmiChangePassword.Text = Resources.GetRes().GetString("ChangePassword");
            tsbChangeSet.Text = tsmiChangeSet.Text = Resources.GetRes().GetString("ChangeSet");






            tsmiSupplier.Text = Resources.GetRes().GetString("SupplierManager");
            tsmiRequest.Text = Resources.GetRes().GetString("RequestManager");
            tsmiAdminLog.Text = Resources.GetRes().GetString("AdminLog");
            tsmiBalance.Text = Resources.GetRes().GetString("BalanceManager");
            tsmiFinanceLog.Text = Resources.GetRes().GetString("FinanceLog");
            tsmiStatistic.Text = Resources.GetRes().GetString("Statistic");

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            tsbAout.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.About.png"));
            tsbExpenditure.Image = tsmiExpenditure.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Expenditure.png"));
            tsbRoom.Image = tsmiRoom.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Room.png"));
            tsbProduct.Image = tsmiProduct.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Product.png"));
            tsbProductType.Image = tsmiProductType.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ProductType22.png"));
            tsbMain.Image = tsmiMain.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Main.png"));
            tsbOrder.Image = tsmiOrder.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Order.png"));
            tsbMembers.Image = tsmiMember.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Members.png"));
            tsbChangePassword.Image = tsmiChangePassword.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangePassword.png"));
            tsbChangeSet.Image = tsmiChangeSet.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeSet.png"));
            tsmiExit.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Exit.png"));

            tsmiDevice.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Device.png"));
            tsmiAdmin.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Admin.png"));
            tsmiPrinter.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Printer.png"));
            tsmiSupplier.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Suppliers.png"));
            tsmiRequest.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Request.png"));
            tsmiAdminLog.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.AdminLog.png"));
            tsmiFinanceLog.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.FinanceLog.png"));
            tsmiBalance.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.BalanceManager.png"));
            tsmiStatistic.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.Statistic.png"));

            tsList.ImageScalingSize = new Size(32.RecalcMagnification(), 32.RecalcMagnification());

            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.TradingSystem.ico"));



            tsmiFile.Text = Resources.GetRes().GetString("File");
            tsmiMenu.Text = Resources.GetRes().GetString("Menu");
            tsmiLanguage.Text = Resources.GetRes().GetString("Language");
            tsmiExit.Text = Resources.GetRes().GetString("Exit");

            tsmiSystem.Text = Resources.GetRes().GetString("System");
            tsmiAdmin.Text = Resources.GetRes().GetString("AdminManager");
            tsmiDevice.Text = Resources.GetRes().GetString("DeviceManager");
            tsmiPrinter.Text = Resources.GetRes().GetString("PrinterManager");

            tsbAout.Font = tsmiAbout.Font = font;
            tsbExpenditure.Font = tsmiExpenditure.Font = font;
            tsbMembers.Font = tsmiMember.Font = font;
            tsbRoom.Font = tsmiRoom.Font = font;
            tsbProduct.Font = tsmiProduct.Font = font;
            tsbProductType.Font = tsmiProductType.Font = font;
            tsbMain.Font = tsmiMain.Font = font;
            tsbOrder.Font = tsmiOrder.Font = font;
            tsbChangePassword.Font = tsmiChangePassword.Font = font;
            tsbChangeSet.Font = tsmiChangeSet.Font = font;
            tsmiFile.Font = font;
            tsmiMenu.Font = font;
            tsmiExit.Font = font;
      
            tsmiSystem.Font = tsmiAdmin.Font = tsmiDevice.Font = tsmiPrinter.Font = tsmiStatistic.Font = tsmiSupplier.Font = tsmiRequest.Font = tsmiAdminLog.Font = tsmiFinanceLog.Font = tsmiBalance.Font = font;





            tsmiLanguage.Font = font;
            tsmiLanguage.DropDownItems.Clear();
            foreach (var item in Resources.GetRes().MainLangList)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem();
                tsmi.Font = font;
                tsmi.Image = Image.FromStream(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC." + Resources.GetRes().GetString("Tsmi_Lang_Icon")));
                tsmi.Tag = item.Value.LangIndex;
                tsmi.Size = new System.Drawing.Size(88, 22);
                tsmi.Text = item.Value.LangName;
                tsmi.Click += new System.EventHandler(this.tsmi_Click);
                tsmiLanguage.DropDownItems.Add(tsmi);

                if (Resources.GetRes().MainLangIndex == item.Value.MainLangIndex)
                {
                    tsmi.Checked = true;
                    tsmi.Enabled = false;
                }
            }


            //语言
            if (Resources.GetRes().MainLangIndex == 0)
            {
                krplKeyInfo.Text = Resources.GetRes().KEY_NAME_0;
                krplNameInfo.Text = Resources.GetRes().AdminModel.AdminName0;
            }
            else if (Resources.GetRes().MainLangIndex == 1)
            {
                krplKeyInfo.Text = Resources.GetRes().KEY_NAME_1;
                krplNameInfo.Text = Resources.GetRes().AdminModel.AdminName1;
            }
            else if (Resources.GetRes().MainLangIndex == 2)
            {
                krplKeyInfo.Text = Resources.GetRes().KEY_NAME_2;
                krplNameInfo.Text = Resources.GetRes().AdminModel.AdminName2;
            }
            krplAdminInfo.Text = GetModeName(Resources.GetRes().AdminModel.Mode);

            //定时检查
            LoadingKeyCheck();

            //更新
            SetUpdate();

            // 开启有权限的菜单
            EnableByMenu();

            // 打开主页
            if (tsbMain.Enabled)
            {
                tsbMain_Click(null, null);
            }


            // 注册热键(开钱想)
            RegHotKey();



            this.Shown += (z, y) =>
            {
                if (!_isLoaded)
                {
                    _isLoaded = true;

                    // 点歌系统设置呼叫器时间(消准)
                    if (Resources.GetRes().IsRequired("Vod"))
                    {
                        // 设置设备(呼叫器)时间
                        Task.Factory.StartNew(() =>
                        {
                            Common.GetCommon().SetDeviceTime();
                        });
                    }

                    // 加载第二屏幕
                    if (Resources.GetRes().DisplaySecondMonitor)
                    {
                        FullScreenMonitor.Instance.Initial(this.Location.X, this.Location.Y, this.Width, this.Height);
                    }

                    // 扫码,刷卡

                    // 扫条码处理
                    hookBarcode = new KeyboardHook();
                  
                    var availbleScanners = hookBarcode.GetKeyboardDevices();
                    string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == Resources.GetRes().BarcodeReader).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        hookBarcode.SetDeviceFilter(first);

                        hookBarcode.KeyPressed += OnBarcodeKey;

                        hookBarcode.AddHook(this);
                    }


                    hookCard = new KeyboardHook();
                    first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == Resources.GetRes().CardReader).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        hookCard.SetDeviceFilter(first);

                        hookCard.KeyPressed += OnCardKey;

                        hookCard.AddHook(this);
                    }

                    


                }
            };



        }



        private KeyboardHook hookCard;
        private KeyboardHook hookBarcode;



        private string keyInput = "";
        private void OnBarcodeKey(object sender, KeyPressedEventArgs e)
        {

            if (this.Visible)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput.Trim() != "")
                        Res.Server.Notification.Instance.ActionBarcodeReader(null, keyInput, null);


                    keyInput = "";
                }
                else
                {
                    keyInput += e.Text;
                }
            }
        }



        private string keyInput2 = "";
        private void OnCardKey(object sender, KeyPressedEventArgs e)
        {

            if (this.Visible)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput2.Trim() != "" && keyInput2.Trim().Length == 10)
                        Res.Server.Notification.Instance.ActionCardReader(null, keyInput2, null);

                    keyInput2 = "";

                }
                else
                {
                    keyInput2 += e.Text;
                }
            }
        }

        private bool _isLoaded = false;





        private Hotkey hk = new Hotkey();
        /// <summary>
        /// 注册热键
        /// </summary>
        private void RegHotKey()
        {
            if (string.IsNullOrWhiteSpace(Resources.GetRes().CashDrawer))
                return;
            try
            {
                hk.KeyCode = Keys.Z;
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


        /// <summary>
        /// 设置更新
        /// </summary>
        private void SetUpdate()
        {
            Oybab.Res.Tools.Update.SearchUpdate(Resources.GetRes().GetSoftServicePCName(), model =>
            {
                if (!string.IsNullOrWhiteSpace(model.DisplayMsg))
                {
                    if (!string.IsNullOrWhiteSpace(model.Url))
                        UpdateUrl = model.Url;

                    if (!string.IsNullOrWhiteSpace(model.DisplayMsg))
                        this.BeginInvoke(new Action(() =>
                        {
                            krplUpdateInfo.Text = model.DisplayMsg;
                        }));

                    this.BeginInvoke(new Action(() =>
                    {
                        krplUpdateInfo.Visible = true;
                    }));
                }
            });
        }



        public override string Text
        {
            get
            {
                string s = base.Text;
                if (Resources.GetRes().GetString("RightToLeft") == "1" && s.Contains("["))
                {
                    s = s.Replace("[", "").Replace("]", "");
                }
                return s;
            }
            set
            {
                base.Text = value;
            }
        }



        

        /// <summary>
        /// 加载检查KEY
        /// </summary>
        private void LoadingKeyCheck()
        {
            Session.Instance.StartSession((IsAuto) =>
            {

                //如果窗口本来就打开了,就别打开了(自动检查模式下).
                if (IsAuto && IsOpenKeyWindow)
                    return;

                Common.GetCommon().CheckAndAlert(new Action<string>((message) =>
                {

                    IsOpenKeyWindow = true;

                    if (string.IsNullOrWhiteSpace(message))
                        message = Resources.GetRes().GetString("Exception_ServerCantCorrConn");
                    else
                        message = string.Format(Resources.GetRes().GetString("Exception_ExceptionSource"), Resources.GetRes().GetString("Exception_ServerCantCorrConn"), "", message);

                    this.Invoke(new Action(() =>
                    {
                        DialogResult result = KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button2);
                        if (result == System.Windows.Forms.DialogResult.Abort)
                        {
                            this.Close();
                        }
                    }));
                }), new Action(() =>
                {
                    IsOpenKeyWindow = false;

                    if (null != homeWindow && !homeWindow.IsDisposed)
                    {

                        homeWindow.RefreshAll();
                    }

                }));

            });
        }

        /// <summary>
        /// 关闭应用程序
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            // 这个Hide可以防止退出时卡住问题
            this.Hide();
            base.OnClosed(e);
            try
            {
                hookBarcode.RemoveHook();
                hookCard.RemoveHook();

                if (hk.Registered)
                { hk.Unregister(); }
            }
            catch
            {
            }
            Common.GetCommon().Close();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbChangePassword_Click(object sender, EventArgs e)
        {
            PasswordWindow password = new PasswordWindow();
            password.StartLoad += (obj, ev) =>
            {
                StartLoad(obj);
            };
            password.StopLoad += (obj, ev) =>
            {
                StopLoad(obj);
            };
            SetSelect(tsbChangePassword, tsmiChangePassword);
            password.ShowDialog(this);
            SetSelect(checkedBtn, null, false);
        }

        /// <summary>
        /// 打开更新,升级URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krplUpdateInfo_LinkClicked(object sender, EventArgs e)
        {
            if (null != UpdateUrl)
                System.Diagnostics.Process.Start(UpdateUrl);
        }

        /// <summary>
        /// 关于
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbAout_Click(object sender, EventArgs e)
        {
            AboutWindow about = new AboutWindow();
            about.StartLoad += (obj, ev) =>
            {
                StartLoad(obj);
            };
            about.StopLoad += (obj, ev) =>
            {
                StopLoad(obj);
            };
            SetSelect(tsbAout, tsmiAbout);
            about.ShowDialog(this);
            SetSelect(checkedBtn, null, false);
            
        }

        /// <summary>
        /// 打开主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMain_Click(object sender, EventArgs e)
        {
            if (null == homeWindow || homeWindow.IsDisposed)
            {
                homeWindow = new HomeWindow();
                homeWindow.MdiParent = this;
                homeWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                homeWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };

                
                homeWindow.Show();
            }

            SetSelect(tsbMain, tsmiMain);
            homeWindow.Activate();
        }

        /// <summary>
        /// 订单管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbOrder_Click(object sender, EventArgs e)
        {
            if (null == orderWindow || orderWindow.IsDisposed)
            {
                orderWindow = new OrderWindow();
                orderWindow.MdiParent = this;
                orderWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                orderWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                

                orderWindow.Show();
            }

            SetSelect(tsbOrder, tsmiOrder);
            orderWindow.Activate();
        }

        /// <summary>
        /// 支出管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbExpenditure_Click(object sender, EventArgs e)
        {
            if (null == importWindow || importWindow.IsDisposed)
            {
                importWindow = new ImportWindow();
                importWindow.MdiParent = this;
                importWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                importWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                

                importWindow.Show();
            }

            SetSelect(tsbExpenditure, tsmiExpenditure);
            importWindow.Activate();
        }

      
        /// <summary>
        /// 会员管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbMembers_Click(object sender, EventArgs e)
        {
            if (null == memberWindow || memberWindow.IsDisposed)
            {
                memberWindow = new MemberWindow();
                memberWindow.MdiParent = this;
                memberWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                memberWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };


                memberWindow.Show();
            }

            SetSelect(tsbMembers, tsmiMember);
            memberWindow.Activate();
        }

        /// <summary>
        /// 包厢管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbRoom_Click(object sender, EventArgs e)
        {
            if (null == roomWindow || roomWindow.IsDisposed)
            {
                roomWindow = new RoomWindow();
                roomWindow.MdiParent = this;
                roomWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                roomWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                roomWindow.ChangeName += (obj, ev) =>
                {
                    if (null != homeWindow && !homeWindow.IsDisposed)
                    {
                        homeWindow.RoomNoChange(obj as Room);
                    }
                    if (null != deviceWindow && !deviceWindow.IsDisposed)
                    {
                        deviceWindow.RealoadRoomNo();
                    }
                };
                roomWindow.RemoveRoom += (obj, ev) =>
                {
                    if (null != homeWindow && !homeWindow.IsDisposed)
                    {
                        homeWindow.RoomRemove(obj as Room);
                    }
                };
                roomWindow.AddRoom += (obj, ev) =>
                {
                    if (null != homeWindow && !homeWindow.IsDisposed)
                    {
                        homeWindow.RefreshSome(new List<long>() { (obj as Room).RoomId });
                    }
                };
                

                roomWindow.Show();
            }

            SetSelect(tsbRoom, tsmiRoom);
            roomWindow.Activate();
        }

        /// <summary>
        /// 产品类型管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbProductType_Click(object sender, EventArgs e)
        {
            if (null == productTypeWindow || productTypeWindow.IsDisposed)
            {
                productTypeWindow = new ProductTypeWindow();
                productTypeWindow.MdiParent = this;
                productTypeWindow.StartLoad += (obj, ev)=>{
                    StartLoad(obj);
                };
                productTypeWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                productTypeWindow.ChangeType += (x, y) =>
                {
                    if (null != productWindow && !productWindow.IsDisposed)
                        productWindow.ReloadProductType();
                };
                

                productTypeWindow.Show();
            }

            SetSelect(tsbProductType, tsmiProductType);
            productTypeWindow.Activate();
        }

        /// <summary>
        /// 产品管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbProduct_Click(object sender, EventArgs e)
        {
            if (null == productWindow || productWindow.IsDisposed)
            {
                productWindow = new ProductWindow();
                productWindow.MdiParent = this;
                productWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                productWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                

                productWindow.Show();
            }

            SetSelect(tsbProduct, tsmiProduct);
            productWindow.Activate();
        }



        /// <summary>
        /// 员工管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAdmin_Click(object sender, EventArgs e)
        {
            if (null == adminWindow || adminWindow.IsDisposed)
            {
                adminWindow = new AdminWindow();
                adminWindow.MdiParent = this;
                adminWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                adminWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };


                adminWindow.Show();
            }

            SetSelect(null, tsmiAdmin);
            adminWindow.Activate();
        }

        /// <summary>
        /// 设备管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDevice_Click(object sender, EventArgs e)
        {
            if (null == deviceWindow || deviceWindow.IsDisposed)
            {
                deviceWindow = new DeviceWindow();
                deviceWindow.MdiParent = this;
                deviceWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                deviceWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };


                deviceWindow.Show();
            }

            SetSelect(null, tsmiDevice);
            deviceWindow.Activate();
        }

        /// <summary>
        /// 打印机管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPrinter_Click(object sender, EventArgs e)
        {
            if (null == printerWindow || printerWindow.IsDisposed)
            {
                printerWindow = new PrinterWindow();
                printerWindow.MdiParent = this;
                printerWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                printerWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                printerWindow.ChangePrinter += (obj, ev) =>
                {
                    if (null != productWindow && !productWindow.IsDisposed)
                        productWindow.ReloadPrinters();
                };


                printerWindow.Show();
            }

            SetSelect(null, tsmiPrinter);
            printerWindow.Activate();
        }



        /// <summary>
        /// 打开供应商
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiSupplier_Click(object sender, EventArgs e)
        {
            if (null == supplierWindow || supplierWindow.IsDisposed)
            {
                supplierWindow = new SupplierWindow();
                supplierWindow.MdiParent = this;
                supplierWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                supplierWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };

                supplierWindow.Show();
            }

            SetSelect(null, tsmiSupplier);
            supplierWindow.Activate();
        }


        /// <summary>
        /// 打开需求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiRequest_Click(object sender, EventArgs e)
        {
            if (null == requestWindow || requestWindow.IsDisposed)
            {
                requestWindow = new RequestWindow();
                requestWindow.MdiParent = this;
                requestWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                requestWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };

                requestWindow.Show();
            }

            SetSelect(null, tsmiRequest);
            requestWindow.Activate();
        }

        

        /// <summary>
        /// 打开管理员日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAdminLog_Click(object sender, EventArgs e)
        {
            if (null == adminLogWindow || adminLogWindow.IsDisposed)
            {
                adminLogWindow = new AdminLogWindow();
                adminLogWindow.MdiParent = this;
                adminLogWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                adminLogWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };

                adminLogWindow.Show();
            }

            SetSelect(null, tsmiAdminLog);
            adminLogWindow.Activate();
        }


        /// <summary>
        /// 余额管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiBalance_Click(object sender, EventArgs e)
        {
            if (null == balanceWindow || balanceWindow.IsDisposed)
            {
                balanceWindow = new BalanceWindow();
                balanceWindow.MdiParent = this;
                balanceWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                balanceWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };
                balanceWindow.Show();
            }

            SetSelect(null, tsmiStatistic);
            balanceWindow.Activate();
        }

        /// <summary>
        /// 打开财务日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiFinanceLog_Click(object sender, EventArgs e)
        {
            FinanceLogWindow log = new FinanceLogWindow();
            log.StartLoad += (obj, ev) =>
            {
                StartLoad(obj);
            };
            log.StopLoad += (obj, ev) =>
            {
                StopLoad(obj);
            };
          

            SetSelect(null, tsmiFinanceLog);
            log.ShowDialog(this);
            SetSelect(checkedBtn, null, false);
        }

        /// <summary>
        /// 打开统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiStatistic_Click(object sender, EventArgs e)
        {
            if (null == statisticWindow || statisticWindow.IsDisposed)
            {
                statisticWindow = new StatisticWindow();
                statisticWindow.MdiParent = this;
                statisticWindow.StartLoad += (obj, ev) =>
                {
                    StartLoad(obj);
                };
                statisticWindow.StopLoad += (obj, ev) =>
                {
                    StopLoad(obj);
                };

                statisticWindow.Show();
            }

            SetSelect(null, tsmiStatistic);
            statisticWindow.Activate();
        }

        /// <summary>
        /// 更改设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsbChangeSet_Click(object sender, EventArgs e)
        {
            SettingsWindow setting = new SettingsWindow(false);
            setting.StartLoad += (obj, ev) =>
            {
                StartLoad(obj);
            };
            setting.StopLoad += (obj, ev) =>
            {
                StopLoad(obj);
            };


            SetSelect(tsbChangeSet, tsmiChangeSet);
            setting.ShowDialog(this);
            SetSelect(checkedBtn, null, false);
        }


        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 切换语言为中文
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmi_Click(object sender, EventArgs e)
        {

            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;

            if (Config.GetConfig().SetConfig(int.Parse((tsmi.Tag.ToString())), Resources.GetRes().SERVER_ADDRESS, Resources.GetRes().IsLocalPrintCustomOrder, false, Resources.GetRes().CashDrawer, Resources.GetRes().PriceMonitor, Resources.GetRes().BarcodeReader, Resources.GetRes().CardReader))
            {


                foreach (var item in tsmiLanguage.DropDownItems.OfType<ToolStripMenuItem>())
                {
                    if (item == tsmi)
                    {
                        item.Enabled = false;
                        item.Checked = true;
                    }
                    else
                    {
                        item.Enabled = true;
                        item.Checked = false;
                    }
                }

               


                KryptonMessageBox.Show(this, Resources.GetRes().GetString("ChangeLanguageSuccess"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                KryptonMessageBox.Show(this, Resources.GetRes().GetString("ChangeLanguageFailt"), Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
        }
        



        /// <summary>
        /// 开始加载
        /// </summary>
        private void StartLoad(object obj)
        {
            if (!pbProgress.Visible)
            {
                this.Invoke(new Action(() =>
                {
                    bool IsDialog = false;
                    if (null != obj)
                    {
                        Form form = obj as Form;
                        if (form != null && form != this && !form.IsMdiChild)
                        {
                            form.Enabled = false;
                            IsDialog = true;
                        }
                            
                    }
                    
                    if (!IsDialog)
                        this.Enabled = false;
                    pbProgress.Visible = true;
                }));
            }
        }

        /// <summary>
        /// 开始加载
        /// </summary>
        private void StopLoad(object obj)
        {
            if (pbProgress.Visible)
            {
                this.Invoke(new Action(() =>
                {
                    bool IsDialog = false;
                    if (null != obj)
                    {
                        Form form = obj as Form;
                        if (form != null && form != this && !form.IsMdiChild)
                        {
                            form.Enabled = true;
                            IsDialog = true;
                        }
                    }
                    if (!IsDialog)
                        this.Enabled = true;
                    pbProgress.Visible = false;
                }));
            }
        }

        ToolStripButton checkedBtn = null;
        /// <summary>
        /// 设置已开启
        /// </summary>
        /// <param name="tspBtn"></param>
        /// <param name="tspMenuItem"></param>
        private void SetSelect(ToolStripButton tspBtn, ToolStripMenuItem tspMenuItem, bool IsSaveChcecked = true)
        {
            if (IsSaveChcecked)
            {
                foreach (var item in tsList.Items.OfType<ToolStripButton>())
                {
                    if (null != item && item.Checked)
                    {
                        checkedBtn = item;
                        break;
                    }
                }

            }

            foreach (var item in tsList.Items.OfType<ToolStripButton>())
            {
                if (tspBtn == item)
                    item.Checked = true;
                else
                    item.Checked = false;
            }

        }

        /// <summary>
        /// 根据设置开启
        /// </summary>
        private void EnableByMenu()
        {
            if (Resources.GetRes().AdminModel.Mode == 0)
            {
                //tsbMain.Visible = tsmiMain.Visible = false;
                tsbOrder.Visible = tsmiOrder.Visible = false;
                tsbMembers.Visible = tsmiMember.Visible = false;
                tsbRoom.Visible = tsmiRoom.Visible = false;
                tsbProduct.Visible = tsmiProduct.Visible = false;
                tsbProductType.Visible = tsmiProductType.Visible = false;
                tsbExpenditure.Visible = tsmiExpenditure.Visible = false;
                tsbChangeSet.Visible = tsmiChangeSet.Visible = false;
                tsbChangePassword.Visible = tsmiChangePassword.Visible = false;

                tsmiMenu.Visible = false;

                tsmiAdmin.Visible = false;
                tsmiDevice.Visible = false;
                tsmiPrinter.Visible = false;
                tsmiSupplier.Visible = false;
                tsmiRequest.Visible = false;
                tsmiAdminLog.Visible = false;
                tsmiBalance.Visible = false;
                tsmiFinanceLog.Visible = false;
                tsmiStatistic.Visible = false;

                tsmiSystem.Visible = false;
            }
            else if (Resources.GetRes().AdminModel.Mode == 1)
            {
                if (string.IsNullOrWhiteSpace(Resources.GetRes().AdminModel.Menu))
                {
                    //tsbMain.Visible = tsmiMain.Visible = false;
                    tsbOrder.Visible = tsmiOrder.Visible = false;
                    tsbMembers.Visible = tsmiMember.Visible = false;
                    tsbRoom.Visible = tsmiRoom.Visible = false;
                    tsbProduct.Visible = tsmiProduct.Visible = false;
                    tsbProductType.Visible = tsmiProductType.Visible = false;
                    tsbExpenditure.Visible = tsmiExpenditure.Visible = false;
                    tsbChangeSet.Visible = tsmiChangeSet.Visible = false;

                    tsmiMenu.Visible = false;

                    tsmiAdmin.Visible = false;
                    tsmiDevice.Visible = false;
                    tsmiPrinter.Visible = false;
                    tsmiRequest.Visible = false;
                    tsmiSupplier.Visible = false;
                    tsmiAdminLog.Visible = false;
                    tsmiBalance.Visible = false;
                    tsmiFinanceLog.Visible = false;
                    tsmiStatistic.Visible = false;

                    tsmiSystem.Visible = false;
                }
                else
                {
                    int MenuHideCount = 0;
                    int SystemHideCount = 0;

                    string[] menuList = Resources.GetRes().AdminModel.Menu.Split('&');

                    //if (!menuList.Contains("1000"))
                    //{
                    //    tsbMain.Visible = tsmiMain.Visible = false;
                    //    toolStripSeparator1.Visible = false;
                    //    ++MenuHideCount;

                    //}
                    if (!menuList.Contains("1100"))
                    {
                        tsbOrder.Visible = tsmiOrder.Visible = false;
                        toolStripSeparator2.Visible = false;
                        ++MenuHideCount;
                    }
                    if (!menuList.Contains("1200"))
                    {
                        tsbExpenditure.Visible = tsmiExpenditure.Visible = false;
                        toolStripSeparator8.Visible = false;
                        ++MenuHideCount;
                    }
                    if (!menuList.Contains("1300"))
                    {
                        tsbMembers.Visible = tsmiMember.Visible = false;
                        toolStripSeparator3.Visible = false;
                        ++MenuHideCount;
                    }
                    if (!menuList.Contains("1400"))
                    {
                        tsbRoom.Visible = tsmiRoom.Visible = false;
                        toolStripSeparator4.Visible = false;
                        ++MenuHideCount;
                    }
                    if (!menuList.Contains("1500"))
                    {
                        tsbProductType.Visible = tsmiProductType.Visible = false;
                        toolStripSeparator5.Visible = false;
                        ++MenuHideCount;
                    }
                    if (!menuList.Contains("1600"))
                    {
                        tsbProduct.Visible = tsmiProduct.Visible = false;
                        ++MenuHideCount;
                    }
                    if (!menuList.Contains("3000"))
                    {
                        tsmiAdmin.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3100"))
                    {
                        tsmiDevice.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3200"))
                    {
                        tsmiPrinter.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3300"))
                    {
                        tsmiSupplier.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3400"))
                    {
                        tsmiAdminLog.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3500"))
                    {
                        tsmiFinanceLog.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3600"))
                    {
                        tsmiStatistic.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3700"))
                    {
                        tsmiRequest.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("3800"))
                    {
                        tsmiBalance.Visible = false;
                        ++SystemHideCount;
                    }
                    if (!menuList.Contains("5000"))
                    {
                        tsbChangeSet.Visible = tsmiChangeSet.Visible = false;
                    }


                    if (MenuHideCount == tsmiMenu.DropDownItems.Count)
                    {
                        tsmiMenu.Visible = false;
                    }

                    if (SystemHideCount == tsmiSystem.DropDownItems.Count)
                    {
                        tsmiSystem.Visible = false;
                    }
                }


            }


            

        }



        /// <summary>
        /// 获取管理员模式名称
        /// </summary>
        /// <param name="hideTypeNo"></param>
        /// <returns></returns>
        private string GetModeName(long typeNo)
        {
            if (typeNo == 0)
                return Resources.GetRes().GetString("Guest");
            else if (typeNo == 1)
                return Resources.GetRes().GetString("Employee");
            else if (typeNo == 2)
                return Resources.GetRes().GetString("Admin");
            else
            {
                throw new Exception(Resources.GetRes().GetString("Exception_InvalidType"));
            }
        }

        
    }
}
