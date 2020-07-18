using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Oybab.Res.Tools;
using Microsoft.Win32;
using Oybab.Res.View.Converters;
using Oybab.Res.View.Component;
using System.Collections.Specialized;
using System.Net;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class SettingViewModel : ViewModelBase
    {
        private UIElement _element;
        private Panel DrawList;
        private Panel PriceList;
        private Panel BarcodeList;
        private Panel CardList;
        private Panel LanguageList;
        private Style DrawSelectStyle;
        private Style PriceSelectStyle;
        private Style BarcodeSelectStyle;
        private Style CardSelectStyle;
        private Style LangSelectStyle;


        internal SettingViewModel(UIElement element, Panel drawList, Panel priceList, Panel barcodeList, Panel cardList, Panel languageList)
        {
            this._element = element;
            this.DrawList = drawList;
            this.PriceList = priceList;
            this.BarcodeList = barcodeList;
            this.CardList = cardList;
            this.LanguageList = languageList;

            DrawSelectStyle = (DrawList as FrameworkElement).FindResource("cbSelectStyle") as Style;
            PriceSelectStyle = (DrawList as FrameworkElement).FindResource("cbSelectStyle") as Style;
            BarcodeSelectStyle = (BarcodeList as FrameworkElement).FindResource("cbSelectStyle") as Style;
            CardSelectStyle = (CardList as FrameworkElement).FindResource("cbSelectStyle") as Style;
            LangSelectStyle = (languageList as FrameworkElement).FindResource("cbSelectStyle") as Style;

            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
        }

        private int lastLangIndex = 0;
        private List<RoomStateModel> resultDrawList = new List<RoomStateModel>();
        private List<RoomStateModel> resultPriceList = new List<RoomStateModel>();
        private List<RoomStateModel> resultBarcodeList = new List<RoomStateModel>();
        private List<RoomStateModel> resultCardList = new List<RoomStateModel>();
        private List<RoomStateModel> resultLanguageList = new List<RoomStateModel>();
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Initial()
        {
            ServerIP = Resources.GetRes().SERVER_ADDRESS;
            IsLocalPrint = Resources.GetRes().IsLocalPrintCustomOrder;

            // 初始化抽屉
            InitDrawbox();
            // 初始化客显
            InitPriceMonitor();
            // 初始化条码阅读器
            InitBarcodeReader();
            // 初始化卡片阅读器
            InitCardReader();

            if (resultDrawList.Count > 0 && resultDrawList.Where(x => x.UseState).Count() > 0 && resultDrawList.Where(x => x.UseState).FirstOrDefault().RoomNo != Resources.GetRes().GetString("None"))
                HideOpen = false;
            else
                HideOpen = true;

            if (resultPriceList.Count > 0 && resultPriceList.Where(x => x.UseState).Count() > 0 && resultPriceList.Where(x => x.UseState).FirstOrDefault().RoomNo != Resources.GetRes().GetString("None"))
                HideTest = false;
            else
                HideTest = true;


            resultLanguageList.Clear();
            this.LanguageList.Children.Clear();

            // 初始化语言
            

            foreach (var item in Resources.GetRes().AllLangList)
            {
                RoomStateModel Model0 = new RoomStateModel() { RoomNo = Resources.GetRes().GetString("LangName", item.Value.Culture), RoomId = item.Value.LangIndex };

                if (Model0.RoomId == Resources.GetRes().CurrentLangIndex)
                {
                    Model0.UseState = true;
                    lastLangIndex = Resources.GetRes().CurrentLangIndex;
                }
                resultLanguageList.Add(Model0);
            }

           



            foreach (var item in resultLanguageList)
            {
                AddLanguageItem(item);
            }


            // 扫码/刷卡
            krpcbBarcodeReader_SelectedIndexChanged();
            krpcbCardReader_SelectedIndexChanged();
        }




        /// <summary>
        /// 初始化钱箱设置
        /// </summary>
        private void InitDrawbox()
        {

            resultDrawList.Clear();
            this.DrawList.Children.Clear();

            RoomStateModel nullRoomStateModel = new RoomStateModel() { RoomNo = Resources.GetRes().GetString("None")};
            resultDrawList.Add(nullRoomStateModel);


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
                            RoomStateModel model = new RoomStateModel() { RoomNo = ComStr.ToString() };


                            if (!string.IsNullOrWhiteSpace(Resources.GetRes().CashDrawer) && null != ComStr && !string.IsNullOrWhiteSpace(ComStr.ToString()) && Resources.GetRes().CashDrawer == ComStr.ToString())
                            {
                                model.UseState = true;
                            }

                            if (resultDrawList.Where(x=>x.RoomNo == model.RoomNo).Count() == 0)
                                resultDrawList.Add(model);
                        }
                    }
                }



                const string local = "System";
                RoomStateModel modelLocal = new RoomStateModel() { RoomNo = local };
                resultDrawList.Add(modelLocal);

                if (!string.IsNullOrWhiteSpace(Resources.GetRes().CashDrawer) && Resources.GetRes().CashDrawer == local)
                {
                    modelLocal.UseState = true;
                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }



            foreach (var item in resultDrawList)
            {
                AddDrawItem(item);
            }




            if (!resultDrawList.Any(x=>x.UseState))
            {
                nullRoomStateModel.UseState = true;
            }
        }





        /// <summary>
        /// 初始化客显设置
        /// </summary>
        private void InitPriceMonitor()
        {

            resultPriceList.Clear();
            this.PriceList.Children.Clear();

            RoomStateModel nullRoomStateModel = new RoomStateModel() { RoomNo = Resources.GetRes().GetString("None") };
            resultPriceList.Add(nullRoomStateModel);


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
                            RoomStateModel model = new RoomStateModel() { RoomNo = ComStr.ToString() };


                            if (!string.IsNullOrWhiteSpace(Resources.GetRes().PriceMonitor) && null != ComStr && !string.IsNullOrWhiteSpace(ComStr.ToString()) && Resources.GetRes().PriceMonitor == ComStr.ToString())
                            {
                                model.UseState = true;
                            }

                            if (resultPriceList.Where(x => x.RoomNo == model.RoomNo).Count() == 0)
                                resultPriceList.Add(model);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }



            foreach (var item in resultPriceList)
            {
                AddPriceItem(item);
            }

            if (resultPriceList.Count == 1)
            {
                nullRoomStateModel.UseState = true;
            }
        }





        /// <summary>
        /// 初始化条码阅读器
        /// </summary>
        private void InitBarcodeReader()
        {

            resultBarcodeList.Clear();
            this.BarcodeList.Children.Clear();

            RoomStateModel nullRoomStateModel = new RoomStateModel() { RoomNo = Resources.GetRes().GetString("None") };
            resultBarcodeList.Add(nullRoomStateModel);


            try
            {
                KeyboardHook hook = new KeyboardHook();
                var availbleScanners = hook.GetKeyboardDevices();
                if (null != availbleScanners && availbleScanners.Count > 0)
                {
                    foreach (var item in availbleScanners.Distinct())
                    {
                        RoomStateModel model = new RoomStateModel() { RoomNo = String.Format("{0:X}", item.GetHashCode()) };


                        if (String.Format("{0:X}", item.GetHashCode()) == Resources.GetRes().BarcodeReader)
                        {
                            model.UseState = true;
                        }

                        if (resultBarcodeList.Where(x => x.RoomNo == model.RoomNo).Count() == 0)
                            resultBarcodeList.Add(model);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            foreach (var item in resultBarcodeList)
            {
                AddBarcodeItem(item);
            }

            if (resultBarcodeList.Where(x=>x.UseState).Count() == 0)
            {
                nullRoomStateModel.UseState = true;
            }
        }




        /// <summary>
        /// 初始化卡片阅读器
        /// </summary>
        private void InitCardReader()
        {

            resultCardList.Clear();
            this.CardList.Children.Clear();

            RoomStateModel nullRoomStateModel = new RoomStateModel() { RoomNo = Resources.GetRes().GetString("None") };
            resultCardList.Add(nullRoomStateModel);


            try
            {
                KeyboardHook hook = new KeyboardHook();
                var availbleScanners = hook.GetKeyboardDevices();
                if (null != availbleScanners && availbleScanners.Count > 0)
                {
                    foreach (var item in availbleScanners.Distinct())
                    {
                        RoomStateModel model = new RoomStateModel() { RoomNo = String.Format("{0:X}", item.GetHashCode()) };


                        if (String.Format("{0:X}", item.GetHashCode()) == Resources.GetRes().CardReader)
                        {
                            model.UseState = true;
                        }

                        if (resultCardList.Where(x => x.RoomNo == model.RoomNo).Count() == 0)
                            resultCardList.Add(model);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex);
            }

            foreach (var item in resultCardList)
            {
                AddCardItem(item);
            }

            if (resultCardList.Where(x => x.UseState).Count() == 0)
            {
                nullRoomStateModel.UseState = true;
            }
        }




        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.ServerIP.Length < 100)
            {
                this.ServerIP += no;
            }
        }

        private int _displayMode;
        /// <summary>
        /// 显示模式(1服务器IP)
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


        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.ServerIP.Length > 0)
            {
                if (IsAll)
                    this.ServerIP = "";
                else
                    this.ServerIP = this.ServerIP.Remove(this.ServerIP.Length - 1);
            }
        }




        /// <summary>
        /// 命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetCommand(string no)
        {
            // 确定
            if (no == "OK")
            {
                this.KeyboardLittle.IsDisplayKeyboard = false;
                if (this.IsDisplay)
                    this.DisplayMode = 0;
                ClearFocus();
            }
            // 取消
            else if (no == "Cancel")
            {
                RemoveText(true);
            }
            // 删除
            else if (no == "Del")
            {
                RemoveText(false);
            }
        }




        /// <summary>
        /// 添加抽屉
        /// </summary>
        /// <param name="item"></param>
        private void AddDrawItem(RoomStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = DrawSelectStyle;
                btn.DataContext = item;
                btn.Command = SelectDraw;
                btn.CommandParameter = item;
                DrawList.Children.Add(btn);
            }));
        }


        /// <summary>
        /// 添加客显
        /// </summary>
        /// <param name="item"></param>
        private void AddPriceItem(RoomStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = PriceSelectStyle;
                btn.DataContext = item;
                btn.Command = SelectPrice;
                btn.CommandParameter = item;
                PriceList.Children.Add(btn);
            }));
        }



        /// <summary>
        /// 添加条码阅读器
        /// </summary>
        /// <param name="item"></param>
        private void AddBarcodeItem(RoomStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = BarcodeSelectStyle;
                btn.DataContext = item;
                btn.Command = SelectBarcode;
                btn.CommandParameter = item;
                BarcodeList.Children.Add(btn);
            }));
        }



        /// <summary>
        /// 添加卡片阅读器
        /// </summary>
        /// <param name="item"></param>
        private void AddCardItem(RoomStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = CardSelectStyle;
                btn.DataContext = item;
                btn.Command = SelectCard;
                btn.CommandParameter = item;
                CardList.Children.Add(btn);
            }));
        }



        /// <summary>
        /// 添加语言
        /// </summary>
        /// <param name="item"></param>
        private void AddLanguageItem(RoomStateModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = LangSelectStyle;
                btn.DataContext = item;
                btn.Command = SelectLanguage;
                btn.CommandParameter = item;
                LanguageList.Children.Add(btn);
            }));
        }


        private KeyboardLittleViewModel _keyboardLittle;
        /// <summary>
        /// 小键盘
        /// </summary>
        public KeyboardLittleViewModel KeyboardLittle
        {
            get { return _keyboardLittle; }
            set
            {
                _keyboardLittle = value;
                OnPropertyChanged("KeyboardLittle");
            }
        }


        /// <summary>
        /// 去掉焦点
        /// </summary>
        private void ClearFocus()
        {
            var scope = FocusManager.GetFocusScope(_element); // elem is the UIElement to unfocus
            FocusManager.SetFocusedElement(scope, null); // remove logical focus
            Keyboard.ClearFocus(); // remove keyboard focus
        }



        /// <summary>
        /// 选择抽屉
        /// </summary>
        private RelayCommand _selectDraw;
        public ICommand SelectDraw
        {
            get
            {
                if (_selectDraw == null)
                {
                    _selectDraw = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in resultDrawList)
                        {
                            if (model.RoomNo == item.RoomNo)
                                item.UseState = true;
                            else
                                item.UseState = false;
                        }


                        if (resultDrawList.Count > 0 && resultDrawList.Where(x => x.UseState).Count() > 0 && resultDrawList.Where(x => x.UseState).FirstOrDefault().RoomNo != Resources.GetRes().GetString("None"))
                            HideOpen = false;
                        else
                            HideOpen = true;


                    });
                }
                return _selectDraw;
            }
        }




        /// <summary>
        /// 选择客显
        /// </summary>
        private RelayCommand _selectPrice;
        public ICommand SelectPrice
        {
            get
            {
                if (_selectPrice == null)
                {
                    _selectPrice = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in resultPriceList)
                        {
                            if (model.RoomNo == item.RoomNo)
                                item.UseState = true;
                            else
                                item.UseState = false;
                        }


                        if (resultPriceList.Count > 0 && resultPriceList.Where(x => x.UseState).Count() > 0 && resultPriceList.Where(x => x.UseState).FirstOrDefault().RoomNo != Resources.GetRes().GetString("None"))
                            HideTest = false;
                        else
                            HideTest = true;


                    });
                }
                return _selectPrice;
            }
        }





        /// <summary>
        /// 选择条码阅读器
        /// </summary>
        private RelayCommand _selectBarcode;
        public ICommand SelectBarcode
        {
            get
            {
                if (_selectBarcode == null)
                {
                    _selectBarcode = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in resultBarcodeList)
                        {
                            if (model.RoomNo == item.RoomNo)
                                item.UseState = true;
                            else
                                item.UseState = false;
                        }
                        krpcbBarcodeReader_SelectedIndexChanged();

                    });
                }
                return _selectBarcode;
            }
        }





        /// <summary>
        /// 选择卡片阅读器
        /// </summary>
        private RelayCommand _selectCard;
        public ICommand SelectCard
        {
            get
            {
                if (_selectCard == null)
                {
                    _selectCard = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in resultCardList)
                        {
                            if (model.RoomNo == item.RoomNo)
                                item.UseState = true;
                            else
                                item.UseState = false;
                        }
                        krpcbCardReader_SelectedIndexChanged();


                    });
                }
                return _selectCard;
            }
        }



        /// <summary>
        /// 打开语言
        /// </summary>
        private RelayCommand _selectLanguage;
        public ICommand SelectLanguage
        {
            get
            {
                if (_selectLanguage == null)
                {
                    _selectLanguage = new RelayCommand(param =>
                    {
                        RoomStateModel model = param as RoomStateModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }


                        foreach (var item in resultLanguageList)
                        {
                            if (model.RoomId == item.RoomId)
                                item.UseState = true;
                            else
                                item.UseState = false;

                        }


                    });
                }
                return _selectLanguage;
            }
        }



        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {
            IsDisplay = true;
            IsShow = true;

        }



        /// <summary>
        /// 隐藏
        /// </summary>
        internal void Hide()
        {
            IsShow = false;

            new Action(() =>
            {
                System.Threading.Thread.Sleep(Resources.GetRes().AnimateTime);

                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsDisplay = false;


                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));

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
                }));

            }).BeginInvoke(null, null);

        }





        private bool _isShow = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                OnPropertyChanged("IsShow");
            }
        }



        private bool _isLocalPrint = false;
        /// <summary>
        /// 是否本地打印
        /// </summary>
        public bool IsLocalPrint
        {
            get { return _isLocalPrint; }
            set
            {
                _isLocalPrint = value;
                OnPropertyChanged("IsLocalPrint");
            }
        }


        
        private bool _isDisplay = false;
        /// <summary>
        /// 是否显示动画
        /// </summary>
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set
            {
                _isDisplay = value;
                if (_isDisplay == true)
                    Init();
                OnPropertyChanged("IsDisplay");
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            

        }










        private string _serverIP = "";
        /// <summary>
        /// 新雅座编号
        /// </summary>
        public string ServerIP
        {
            get { return _serverIP; }
            set
            {
                _serverIP = value;
                OnPropertyChanged("ServerIP");
            }
        }



        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(param =>
                    {

                        if (string.IsNullOrWhiteSpace(ServerIP.Trim()))
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), Resources.GetRes().GetString("ServerIpAddress2")), null, PopupType.Warn));
                            return;
                        }

                        int lang = (int)resultLanguageList.Where(x => x.UseState).FirstOrDefault().RoomId;

                        string drawer = "";
                        RoomStateModel model = resultDrawList.Where(x => x.UseState).FirstOrDefault();
                        if (null != model)
                            drawer = model.RoomNo;


                        string priceMonitor = "";
                        RoomStateModel modelOfPrice = resultPriceList.Where(x => x.UseState).FirstOrDefault();
                        if (null != modelOfPrice)
                            priceMonitor = modelOfPrice.RoomNo;


                        string barcode = "";
                        model = resultBarcodeList.Where(x => x.UseState).FirstOrDefault();
                        if (null != model)
                            barcode = model.RoomNo;

                        string card = "";
                        model = resultCardList.Where(x => x.UseState).FirstOrDefault();
                        if (null != model)
                            card = model.RoomNo;

                        string cashDrawerValue = (drawer == Resources.GetRes().GetString("None") ? "" : drawer);
                        string priceMonitorValue = (priceMonitor == Resources.GetRes().GetString("None") ? "" : priceMonitor);
                        string barcodeReaderValue = (barcode == Resources.GetRes().GetString("None") ? "" : barcode);
                        string cardReaderValue = (card == Resources.GetRes().GetString("None") ? "" : card);
                        if (Config.GetConfig().SetConfig(lang, ServerIP.Trim(), IsLocalPrint, true, cashDrawerValue, priceMonitorValue, barcodeReaderValue, cardReaderValue))
                        {

                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateSuccess"), Resources.GetRes().GetString("Change")), null, PopupType.Information));

                            if (lang != lastLangIndex)
                            {

                                LangConverter.Instance.ChangeCulture(lang);


                                Notification.Instance.ActionLanguage(null, lang, null);
                            }

                            this.Hide();
                        }
                        else
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("OperateFaild"), Resources.GetRes().GetString("Change")), null, PopupType.Warn));
                        }
                    });
                }
                return _okCommand;
            }
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(param =>
                    {
                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }




        private bool _hideOpen = false;
        /// <summary>
        /// 隐藏打开抽屉
        /// </summary>
        public bool HideOpen
        {
            get { return _hideOpen; }
            set
            {
                _hideOpen = value;
                OnPropertyChanged("HideOpen");
            }
        }


        private bool _hideTest = false;
        /// <summary>
        /// 隐藏显示测试客显
        /// </summary>
        public bool HideTest
        {
            get { return _hideTest; }
            set
            {
                _hideTest = value;
                OnPropertyChanged("HideTest");
            }
        }


        /// <summary>
        /// 打开按钮
        /// </summary>
        private RelayCommand _openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(param =>
                    {
                        try
                        {
                            RoomStateModel model = resultDrawList.Where(x => x.UseState).FirstOrDefault();

                            if (null != model)
                            {
                                Common.GetCommon().OpenCashDrawer(model.RoomNo);
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionPro.ExpLog(ex);
                        }
                    });
                }
                return _openCommand;
            }
        }





        /// <summary>
        /// 打开按钮
        /// </summary>
        private RelayCommand _testCommand;
        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new RelayCommand(param =>
                    {
                        try
                        {
                            RoomStateModel model = resultPriceList.Where(x => x.UseState).FirstOrDefault();

                            if (null != model)
                            {
                                if (string.IsNullOrWhiteSpace(model.RoomNo))
                                    return;
                                try
                                {
                                    Common.GetCommon().OpenPriceMonitor(new Random().Next(1, 9999).ToString(), model.RoomNo);
                                }
                                catch (Exception ex)
                                {
                                    ExceptionPro.ExpLog(ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionPro.ExpLog(ex);
                        }
                    });
                }
                return _testCommand;
            }
        }



        private KeyboardHook hookCard = new KeyboardHook();
        private KeyboardHook hookBarcode = new KeyboardHook();
        private bool cardHooked = false;
        private bool barcodeHooked = false;


        private string keyInput = "";
        private void OnBarcodeKey(object sender, KeyPressedEventArgs e)
        {

            if (this.IsDisplay)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput.Trim() != "")
                    {
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SuccessReadBarcode"), null, PopupType.Information));
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

            if (this.IsDisplay)
            {
                // 如果是确认, 则搜索卡号增加到队列
                if (e.Text == "\r")
                {
                    if (keyInput2.Trim() != "" && keyInput2.Trim().Length == 10)
                    {
                        _element.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SuccessReadCardNo"), null, PopupType.Information));
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

        /// <summary>
        /// 更换条码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbBarcodeReader_SelectedIndexChanged()
        {
            if (this.IsDisplay)
            {
                if (barcodeHooked)
                {
                    hookBarcode.RemoveHook();
                    barcodeHooked = false;
                }
                RoomStateModel model = resultBarcodeList.Where(x => x.UseState).FirstOrDefault();
                if (null != model)
                {
                    var availbleScanners = hookBarcode.GetKeyboardDevices();
                    string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == model.RoomNo).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        hookBarcode.SetDeviceFilter(first);

                        hookBarcode.KeyPressed -= OnBarcodeKey;
                        hookBarcode.KeyPressed += OnBarcodeKey;

                        barcodeHooked = true;
                        hookBarcode.AddHook(_element as Window);
                    }
                }
            }
        }

        /// <summary>
        /// 更换读卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpcbCardReader_SelectedIndexChanged()
        {
            if (this.IsDisplay)
            {
                if (cardHooked)
                {
                    hookCard.RemoveHook();
                    cardHooked = false;
                }

                RoomStateModel model = resultCardList.Where(x => x.UseState).FirstOrDefault();
                if (null != model)
                {
                    var availbleScanners = hookCard.GetKeyboardDevices();
                    string first = availbleScanners.Where(x => String.Format("{0:X}", x.GetHashCode()) == model.RoomNo).FirstOrDefault();

                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        hookCard.SetDeviceFilter(first);

                        hookCard.KeyPressed -= OnCardKey;
                        hookCard.KeyPressed += OnCardKey;

                        cardHooked = true;
                        hookCard.AddHook(_element as Window);
                    }
                }
            }
        }

    }
}
