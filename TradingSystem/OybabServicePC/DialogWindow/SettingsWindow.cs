using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using System.IO;
using Oybab.Res.Tools;
using Oybab.ServicePC.Pattern;
using Microsoft.Win32;
using Oybab.ServicePC.Tools;
using System.Net;
using System.Collections.Specialized;
using Oybab.ServerManager.Model.Models;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class SettingsWindow : KryptonForm
    {
        private bool IsOnlyLocal = true;
        public string ReturnValue { get; private set; } //返回值
        public SettingsWindow(bool IsOnlyLocal)
        {
            InitializeComponent();
            Notification.Instance.NotificationConfig += (obj, value, args) => { this.BeginInvoke(new Action(() => { InitServerSet(); })); };

            this.IsOnlyLocal = IsOnlyLocal;
            if (IsOnlyLocal)
            {
                this.Size = new Size(370.RecalcMagnification(), this.Size.Height);
                krpbChangeServer.Enabled = false;
            }
            else
            {
                krpcbCardReader.Enabled = false;
                krpcbBarcodeReader.Enabled = false;
            }

            SetLang();

            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeSet.ico"));

            Init();
            InitDrawbox();
            InitPriceMonitor();
            InitBarcodeReader();
            InitCardReader();
            InitServerSet();



            this.Shown += (z, y) =>
            {
                if (!_isLoaded)
                {
                    _isLoaded = true;

                    // 扫码,刷卡

                    // 扫条码处理
                    krpcbBarcodeReader_SelectedIndexChanged(null, null);
                    krpcbCardReader_SelectedIndexChanged(null, null);

                }
            };


        }


        private bool _isLoaded = false;

        private KeyboardHook hookCard = new KeyboardHook();
        private KeyboardHook hookBarcode = new KeyboardHook();
        private bool cardHooked = false;
        private bool barcodeHooked = false;


        private string keyInput = "";
        private void OnBarcodeKey(object sender, KeyPressedEventArgs e)
        {

            if (this.Visible)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput.Trim() != "")
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("SuccessReadBarcode"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }

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
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            KryptonMessageBox.Show(this, Resources.GetRes().GetString("SuccessReadCardNo"), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }));
                    }

                    keyInput2 = "";

                }
                else
                {
                    keyInput2 += e.Text;
                }
            }
        }


        private int lastLangIndex = -1;
        private void SetLang()
        {
            this.Text = Resources.GetRes().GetString("ChangeSet");
            krpSystem.Text = Resources.GetRes().GetString("System");
            krpServer.Text = Resources.GetRes().GetString("Server");
            
            krplIsLocalPrint.Text = Resources.GetRes().GetString("LocalPrint");
            krplCashDrawer.Text = Resources.GetRes().GetString("CashDrawer");
            krplBarcodeReader.Text = Resources.GetRes().GetString("BarcodeReader");
            krplCardReader.Text = Resources.GetRes().GetString("CardReader");
            krpbOpenCashDrawer.Text = Resources.GetRes().GetString("Open");
            KrplServerIpAddress.Text = Resources.GetRes().GetString("ServerIpAddress2");
            krplLanguage.Text = Resources.GetRes().GetString("Language");
            KrplPrintInfo.Text = Resources.GetRes().GetString("PrintInfo");
            krptGlobalSetting.Text = krpbChangePrintInfo.Text = Resources.GetRes().GetString("Change");
            krplPriceMonitor.Text = Resources.GetRes().GetString("PriceMonitor");
            krpbOpenPriceMonitor.Text = Resources.GetRes().GetString("Test");
            krplGlobalSetting.Text = Resources.GetRes().GetString("GlobalSetting");


            krpcIsLocalPrint.Text = Resources.GetRes().GetString("Yes");



            krpbChangeLocal.Text = krpbChangeServer.Text = Resources.GetRes().GetString("Change");



            if (krpcLanguage.Items.Count > 0)
                lastLangIndex = krpcLanguage.SelectedIndex;

            krpcLanguage.Items.Clear();



            krpcLanguage.Items.AddRange(Resources.GetRes().AllLangList.OrderBy(x => x.Value.LangOrder).Select(x => x.Value.LangName).ToArray());

            if (lastLangIndex != -1)
                krpcLanguage.SelectedIndex = lastLangIndex;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {

            krpcLanguage.SelectedItem = Resources.GetRes().GetLangByLangIndex(Resources.GetRes().CurrentLangIndex).LangName;

            lastLangIndex = krpcLanguage.SelectedIndex;


            krptServerIP.Text = Resources.GetRes().SERVER_ADDRESS;
            krpcIsLocalPrint.Checked = Resources.GetRes().IsLocalPrintCustomOrder;


        }


        /// <summary>
        /// 初始化钱箱设置
        /// </summary>
        private void InitDrawbox()
        {

            krpcbCashDrawer.Items.Clear();
            krpcbCashDrawer.Items.Add(Resources.GetRes().GetString("None"));

            RegistryKey ComReg = null;
            string[] ComList;
            object ComStr = null;


            try
            {
                ComReg = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM", false);

                if (null != ComReg)
                {
                    ComList = ComReg.GetValueNames();
                    for (int i = 0; i < ComList.Length; i++)
                    {

                        ComStr = ComReg.GetValue(ComList[i]);
                        if (null != ComStr && (ComStr.ToString() != "") && (("COM").ToLower().CompareTo((ComStr.ToString().Substring(1 - 1, 3)).ToLower()) == 0))
                        {
                            if (!krpcbCashDrawer.Items.Contains(ComStr))
                            {
                                krpcbCashDrawer.Items.Add(ComStr);

                                if (!string.IsNullOrWhiteSpace(Resources.GetRes().CashDrawer) && null != ComStr && !string.IsNullOrWhiteSpace(ComStr.ToString()) && Resources.GetRes().CashDrawer == ComStr.ToString())
                                    krpcbCashDrawer.SelectedIndex = krpcbCashDrawer.Items.Count - 1;
                            }
                        }
                    }
                }


                const string local = "System";
                krpcbCashDrawer.Items.Add(local);

                if (!string.IsNullOrWhiteSpace(Resources.GetRes().CashDrawer) && Resources.GetRes().CashDrawer == local)
                    krpcbCashDrawer.SelectedIndex = krpcbCashDrawer.Items.Count - 1;

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            if (krpcbCashDrawer.SelectedIndex == -1)
                krpcbCashDrawer.SelectedIndex = 0;
        }




        /// <summary>
        /// 初始化客显
        /// </summary>
        private void InitPriceMonitor()
        {

            krpcbPriceMonitor.Items.Clear();
            krpcbPriceMonitor.Items.Add(Resources.GetRes().GetString("None"));

            RegistryKey ComReg = null;
            string[] ComList;
            object ComStr = null;


            try
            {
                ComReg = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM", false);

                if (null != ComReg)
                {
                    ComList = ComReg.GetValueNames();
                    for (int i = 0; i < ComList.Length; i++)
                    {

                        ComStr = ComReg.GetValue(ComList[i]);
                        if (null != ComStr && (ComStr.ToString() != "") && (("COM").ToLower().CompareTo((ComStr.ToString().Substring(1 - 1, 3)).ToLower()) == 0))
                        {
                            if (!krpcbPriceMonitor.Items.Contains(ComStr))
                            {
                                krpcbPriceMonitor.Items.Add(ComStr);

                                if (!string.IsNullOrWhiteSpace(Resources.GetRes().PriceMonitor) && null != ComStr && !string.IsNullOrWhiteSpace(ComStr.ToString()) && Resources.GetRes().PriceMonitor == ComStr.ToString())
                                    krpcbPriceMonitor.SelectedIndex = krpcbPriceMonitor.Items.Count - 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            if (krpcbPriceMonitor.SelectedIndex == -1)
                krpcbPriceMonitor.SelectedIndex = 0;
        }






        /// <summary>
        /// 初始化条码阅读器
        /// </summary>
        private void InitBarcodeReader()
        {

            krpcbBarcodeReader.Items.Clear();
            krpcbBarcodeReader.Items.Add(Resources.GetRes().GetString("None"));
            KeyboardHook hook = new KeyboardHook();
            var availbleScanners = hook.GetKeyboardDevices();
            if (null != availbleScanners && availbleScanners.Count > 0)
            {
                foreach (var item in availbleScanners.Distinct())
                {
                    if (!krpcbBarcodeReader.Items.Contains(String.Format("{0:X}", item.GetHashCode())))
                    {
                        krpcbBarcodeReader.Items.Add(String.Format("{0:X}", item.GetHashCode()));
                        if (String.Format("{0:X}", item.GetHashCode()) == Resources.GetRes().BarcodeReader)
                            krpcbBarcodeReader.SelectedIndex = krpcbBarcodeReader.Items.Count - 1;
                    }
                }
            }

            if (krpcbBarcodeReader.SelectedIndex == -1)
                krpcbBarcodeReader.SelectedIndex = 0;
        }



        /// <summary>
        /// 初始化卡片阅读器
        /// </summary>
        private void InitCardReader()
        {

            krpcbCardReader.Items.Clear();
            krpcbCardReader.Items.Add(Resources.GetRes().GetString("None"));
            KeyboardHook hook = new KeyboardHook();
            var availbleScanners = hook.GetKeyboardDevices();
            if (null != availbleScanners && availbleScanners.Count > 0)
            {
                foreach (var item in availbleScanners.Distinct())
                {
                    if (!krpcbCardReader.Items.Contains(String.Format("{0:X}", item.GetHashCode())))
                    {
                        krpcbCardReader.Items.Add(String.Format("{0:X}", item.GetHashCode()));
                        if (String.Format("{0:X}", item.GetHashCode()) == Resources.GetRes().CardReader)
                            krpcbCardReader.SelectedIndex = krpcbCardReader.Items.Count - 1;
                    }
                }
            }

            if (krpcbCardReader.SelectedIndex == -1)
                krpcbCardReader.SelectedIndex = 0;
        }


        /// <summary>
        /// 加载服务器设置
        /// </summary>
        private void InitServerSet()
        {
            krplPrintInfoAlert.Visible = false;
            krplGlobalSettingAlert.Visible = false;

            printInfo = Resources.GetRes().PrintInfo;
        }


        /// <summary>
        /// 本地设置被修改
        /// </summary>
        public event EventHandler ChangeLocalSet;
        /// <summary>
        /// 本地设置被修改
        /// </summary>
        public event EventHandler ChangeLang;
        /// <summary>
        /// 开始显示加载
        /// </summary>
        public event EventHandler StartLoad;
        /// <summary>
        /// 停止显示加载
        /// </summary>
        public event EventHandler StopLoad;




      
        /// <summary>
        /// 保存本地设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChangeLocal_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(krptServerIP.Text.Trim()))
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("ServerIpAddress2")), Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            

            int lang = Resources.GetRes().GetLangByLangName(krpcLanguage.SelectedItem.ToString()).LangIndex;

            string cashDrawerValue = (krpcbCashDrawer.Text == Resources.GetRes().GetString("None") ? "" : krpcbCashDrawer.Text);
            string priceMonitorValue = (krpcbPriceMonitor.Text == Resources.GetRes().GetString("None") ? "" : krpcbPriceMonitor.Text);
            string barcodeReaderValue = (krpcbBarcodeReader.Text == Resources.GetRes().GetString("None") ? "" : krpcbBarcodeReader.Text);
            string cardReaderValue = (krpcbCardReader.Text == Resources.GetRes().GetString("None") ? "" : krpcbCardReader.Text);
            if (Config.GetConfig().SetConfig(lang, krptServerIP.Text.Trim(), krpcIsLocalPrint.Checked, IsOnlyLocal, cashDrawerValue, priceMonitorValue, barcodeReaderValue, cardReaderValue))
            {
                string msg = "";
                if (IsOnlyLocal)
                    msg = Resources.GetRes().GetString("OperateSuccess");
                else
                    msg = Resources.GetRes().GetString("OperateChangeNextStartup");
                KryptonMessageBox.Show(this, string.Format(msg, Resources.GetRes().GetString("Change")), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (krpcLanguage.SelectedIndex != lastLangIndex && IsOnlyLocal)
                {
                    Resources.GetRes().ReloadResources(lang);
                    PaletteBlue.GetSelf().Reload();
                    //重新加载文字
                    ConfigString.GetConfigString().Config();

                    SetLang();

                    if (null != ChangeLang)
                        ChangeLang(null, null);
                }

                if (null != ChangeLocalSet)
                    ChangeLocalSet(null, null);

                if (IsOnlyLocal)
                    this.Close();
            }
            else
            {
                KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), krpbChangeLocal.Text), Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }


        }


        private PrintInfo printInfo;

        /// <summary>
        /// 修改服务端配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChangeServer_Click(object sender, EventArgs e)
        {
            StartLoad(this, null);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    bool result = OperatesService.GetOperates().ServiceSetCon(printInfo);

                    this.BeginInvoke(new Action(() =>
                    {
                        if (result)
                        {
                            krplPrintInfoAlert.Visible = false;
                            krplGlobalSettingAlert.Visible = false;

                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("Change")), Resources.GetRes().GetString("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);


                            this.Close();
                        }
                        else
                        {
                            KryptonMessageBox.Show(this, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Change")), Resources.GetRes().GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                    }));

                }
                catch (Exception ex)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            KryptonMessageBox.Show(this, message, Resources.GetRes().GetString("Warn"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }), false, Resources.GetRes().GetString("SaveFailt"));
                    }));
                }
                StopLoad(this, null);
            });
        }

        private void krpcIsPrintAfterBuy_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void krpcIsPrintAfterCheckout_CheckedChanged(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 没空时才能显示打开按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbCashDrawer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (krpcbCashDrawer.Text != Resources.GetRes().GetString("None") && !IsOnlyLocal)
            {
                if (krpcbCashDrawer.Text == "System" && !krpcIsLocalPrint.Checked)
                {
                    krpbOpenCashDrawer.Enabled = false;
                }
                else
                {
                    krpbOpenCashDrawer.Enabled = true;
                }
            }

            else
                krpbOpenCashDrawer.Enabled = false;
        }


        /// <summary>
        /// 没空时才显示客显测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbPriceMonitor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (krpcbPriceMonitor.Text != Resources.GetRes().GetString("None"))
                krpbOpenPriceMonitor.Enabled = true;
            else
                krpbOpenPriceMonitor.Enabled = false;
        }

        /// <summary>
        /// 打开钱箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbOpenCashDrawer_Click(object sender, EventArgs e)
        {
            Common.GetCommon().OpenCashDrawer(krpcbCashDrawer.Text);
        }


        /// <summary>
        /// 测试客显
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbOpenPriceMonitor_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(krpcbPriceMonitor.Text))
                return;
            try
            {
                Common.GetCommon().OpenPriceMonitor(new Random().Next(1, 9999).ToString(), krpcbPriceMonitor.Text);
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }
        }


        /// <summary>
        /// 打开设置打印信息窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbChangePrintInfo_Click(object sender, EventArgs e)
        {
            PrintInfoWindow window = new PrintInfoWindow(printInfo);

            if (window.ShowDialog() == DialogResult.OK)
            {
                krplPrintInfoAlert.Visible = true;
                this.printInfo = window.ReturnValue;
            }
        }

        /// <summary>
        /// 是否本地打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcIsLocalPrint_CheckedChanged(object sender, EventArgs e)
        {
            // 钱箱那里必须本地打印才能用
            krpcbCashDrawer_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// 更换条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbBarcodeReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsOnlyLocal && _isLoaded)
            {
                if (barcodeHooked)
                {
                    hookBarcode.RemoveHook();
                    barcodeHooked = false;
                }

                if (krpcbBarcodeReader.SelectedIndex != 0)
                {
                    var availbleScanners = hookBarcode.GetKeyboardDevices();
                    string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == krpcbBarcodeReader.Text).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        hookBarcode.SetDeviceFilter(first);

                        hookBarcode.KeyPressed -= OnBarcodeKey;
                        hookBarcode.KeyPressed += OnBarcodeKey;

                        barcodeHooked = true;
                        hookBarcode.AddHook(this);
                    }
                }
            }
        }

        /// <summary>
        /// 更换读卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbCardReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsOnlyLocal && _isLoaded)
            {
                if (cardHooked)
                {
                    hookCard.RemoveHook();
                    cardHooked = false;
                }

                if (krpcbCardReader.SelectedIndex != 0)
                {
                    var availbleScanners = hookCard.GetKeyboardDevices();
                    string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == krpcbCardReader.Text).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        hookCard.SetDeviceFilter(first);

                        hookCard.KeyPressed -= OnCardKey;
                        hookCard.KeyPressed += OnCardKey;

                        cardHooked = true;
                        hookCard.AddHook(this);
                    }
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (barcodeHooked)
            {
                hookBarcode.RemoveHook();
                barcodeHooked = false;
            }
            if (cardHooked)
            {
                hookCard.RemoveHook();
                cardHooked = false;
            }

        }

        private void krptGlobalSetting_Click(object sender, EventArgs e)
        {
            GlobalSettingWindow window = new GlobalSettingWindow(printInfo);

            if (window.ShowDialog() == DialogResult.OK)
            {
                krplGlobalSettingAlert.Visible = true;
                this.printInfo = window.ReturnValue;
            }
        }


    }
}
