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
using System.Text.RegularExpressions;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class ChangeTimeViewModel : ViewModelBase
    {
        private UIElement _element;
        private SelectedViewModel selectedModel;

        private long StartTimeLong = 0;
        private long EndTimeLong = 0;
        private Order order;
        private Action Recalc;


        internal ChangeTimeViewModel(UIElement element, Action Recalc)
        {
            this._element = element;
            this.Recalc = Recalc;
            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
        }


        /// 重置时间
        /// </summary>
        /// <param name="room"></param>
        private void ResetTime(Room room)
        {
            if (null == order && selectedModel.StartTimeTemp == selectedModel.EndTimeTemp)
            {
                if (room.IsPayByTime == 1 || room.IsPayByTime == 2)
                {
                    DateTime now = DateTime.Now;
                    selectedModel.StartTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));
                    selectedModel.EndTimeTemp = long.Parse(now.ToString("yyyyMMddHHmm00"));
                    selectedModel.RoomTime = "0:0";
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(SelectedViewModel selectedModel)
        {
            this.selectedModel = selectedModel;

            Room room = Resources.GetRes().Rooms.Where(x => x.RoomId == selectedModel.RoomId).FirstOrDefault();
            RoomModel model = Resources.GetRes().RoomsModel.Where(x => x.RoomId == selectedModel.RoomId).FirstOrDefault();

            ResetTime(room);

           
            this.order = model.PayOrder;
            this.StartTimeLong = order == null ? selectedModel.StartTimeTemp : order.StartTime.Value;
            this.EndTimeLong = selectedModel.EndTimeTemp;

            this.StartTime = DateTime.ParseExact(StartTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            this.EndTime = DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");


            this.Mode = 1;
            this.Hour = "0";
            this.Minute = "0";

            if (selectedModel.RoomType == 1)
            {
                HourName = Resources.GetRes().GetString("Hour");
                MinuteName = Resources.GetRes().GetString("Minute");
            }
            else if (selectedModel.RoomType == 2)
            {
                HourName = Resources.GetRes().GetString("Day");
                MinuteName = Resources.GetRes().GetString("Hour");
            }


            IsDisplayUnlimitedTime = Common.GetCommon().IsChangeUnlimitedTime(this.order == null);
            UnlimitedTime = selectedModel.TempUnlimitedTime;

            
            Calc();
        }



        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.Hour.Length < 2)
            {
                if (this.Hour == "0" && no != ".")
                    Hour = no;
                else
                    this.Hour += no;

                ChangeTime();
            }
            else if (this.IsDisplay && this.DisplayMode == 2 && this.Minute.Length < 2)
            {
                if (this.Minute == "0" && no != ".")
                    Minute = no;
                else
                    this.Minute += no;

                ChangeTime();
            }
        }

        private int _displayMode;
        /// <summary>
        /// 显示模式(1时间2分钟)
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
            if (this.IsDisplay && this.DisplayMode == 1 && this.Hour.Length > 0)
            {
                if (IsAll)
                    this.Hour = "0";
                else
                    this.Hour = this.Hour.Remove(this.Hour.Length - 1);

                if (Hour == "")
                    Hour = "0";

                ChangeTime();
            }
            else if (this.IsDisplay && this.DisplayMode == 2 && this.Minute.Length > 0)
            {
                if (IsAll)
                    this.Minute = "0";
                else
                    this.Minute = this.Minute.Remove(this.Minute.Length - 1);

                if (Minute == "")
                    Minute = "0";

                ChangeTime();
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
        /// 回车
        /// </summary>
        public void HandleKeyboard(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_keyboardLittle.IsDisplayKeyboard)
                {
                    _keyboardLittle.OKCommand.Execute(null);
                }
                else
                {
                    // 无法用, 因为已经失去了焦点
                    this.OKCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Escape)
            {
                if (_keyboardLittle.IsDisplayKeyboard)
                {
                    _keyboardLittle.IsDisplayKeyboard = false;
                }
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
        /// 显示
        /// </summary>
        internal void Show()
        {
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

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




        private int _mode = 1;
        /// <summary>
        /// 模式(1增加2减少)
        /// </summary>
        public int Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                OnPropertyChanged("Mode");
            }
        }



        private bool _unlimitedTime = false;
        /// <summary>
        /// 无限时间
        /// </summary>
        public bool UnlimitedTime
        {
            get { return _unlimitedTime; }
            set
            {
                _unlimitedTime = value;
                OnPropertyChanged("UnlimitedTime");
            }
        }


        private bool _isDisplayUnlimitedTime = false;
        /// <summary>
        /// 是否显示无限时间
        /// </summary>
        public bool IsDisplayUnlimitedTime
        {
            get { return _isDisplayUnlimitedTime; }
            set
            {
                _isDisplayUnlimitedTime = value;
                OnPropertyChanged("IsDisplayUnlimitedTime");
            }
        }





        private string _hourName = "";
        /// <summary>
        /// 小时(名)
        /// </summary>
        public string HourName
        {
            get { return _hourName; }
            set
            {
                _hourName = value;
                OnPropertyChanged("HourName");
            }
        }


        private string _minuteName = "";
        /// <summary>
        /// 分钟(名)
        /// </summary>
        public string MinuteName
        {
            get { return _minuteName; }
            set
            {
                _minuteName = value;
                OnPropertyChanged("MinuteName");
            }
        }



        private string _hour = "0";
        /// <summary>
        /// 小时
        /// </summary>
        public string Hour
        {
            get { return _hour; }
            set
            {
                _hour = value;
                OnPropertyChanged("Hour");
            }
        }



        private string _minute = "0";
        /// <summary>
        /// 分钟
        /// </summary>
        public string Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                OnPropertyChanged("Minute");
            }
        }



        private string _startTime = "";
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }


        private string _endTime = "";
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime
        {
            get { return _endTime; }
            set
            {
                _endTime = value;
                OnPropertyChanged("EndTime");
            }
        }



        private string _totalTime = "";
        /// <summary>
        /// 总时间
        /// </summary>
        public string TotalTime
        {
            get { return _totalTime; }
            set
            {
                _totalTime = value;
                OnPropertyChanged("TotalTime");
            }
        }


        private string _remainingTime = "";
        /// <summary>
        /// 剩余时间
        /// </summary>
        public string RemainingTime
        {
            get { return _remainingTime; }
            set
            {
                _remainingTime = value;
                OnPropertyChanged("RemainingTime");
            }
        }




        Regex matchHour = new Regex(@"^\d{0,2}$");
        Regex matchMinute = new Regex(@"^([0-9]|[1-4][0-9]|5[0-7])$");


        Regex matchHour2 = new Regex(@"^([0-9]|1[0-9]|2[0-3])$");

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
                        
                        if (Hour == "")
                        {
                            Hour = "0";
                            return;
                        }
                        if (!matchHour.IsMatch(Hour))
                        {
                            Hour = "0";
                            return;
                        }




                        if (Minute == "")
                        {
                            Minute = "0";
                            return;
                        }
                        if (selectedModel.RoomType == 1)
                        {
                            if (!matchMinute.IsMatch(Minute))
                            {
                                Minute = "0";
                                return;
                            }
                        }
                        else if (selectedModel.RoomType == 2)
                        {
                            if (!matchHour2.IsMatch(Minute))
                            {
                                Minute = "0";
                                return;
                            }
                        }
                        

                        Calc();


                        long ReturnValue = 0;

                        // 这个改动会将时间最小单位设置为5分钟
                        if(selectedModel.RoomType == 1)
                        {
                            double minuteDouble = double.Parse(Minute);
                            Minute = RoundToNumber(minuteDouble).ToString();
                        }


                        if (Mode == 1)
                        {
                            
                            if (selectedModel.RoomType == 1)
                            {
                                 ReturnValue = long.Parse(DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddHours(int.Parse(Hour)).AddMinutes(int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                            }
                            else if (selectedModel.RoomType == 2)
                            {
                                ReturnValue = long.Parse(DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(int.Parse(Hour)).AddHours(int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                            }

                        }
                        else if (Mode == 2)
                        {
                            if (selectedModel.RoomType == 1)
                            {
                                ReturnValue = long.Parse(DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddHours(-int.Parse(Hour)).AddMinutes(-int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                            }
                            else if (selectedModel.RoomType == 2)
                            {
                                ReturnValue = long.Parse(DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-int.Parse(Hour)).AddHours(-int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                            }
                        }


                        // 如果跟订单上次保存的不一样,就提示未保存提示
                        if (null != order && order.EndTime == ReturnValue)
                        {
                            selectedModel.EndTimeTemp = ReturnValue;
                            selectedModel.RoomTimeChange = false;
                            selectedModel.RefreshTime();
                        }
                        else
                        {
                            selectedModel.EndTimeTemp = ReturnValue;
                            selectedModel.RoomTimeChange = true;
                            selectedModel.RefreshTime();
                        }


                        if (selectedModel.TempUnlimitedTime != UnlimitedTime && !selectedModel.RoomTimeChange)
                        {
                            selectedModel.RoomTimeChange = true;
                            selectedModel.RefreshTime();
                        }

                        selectedModel.TempUnlimitedTime = UnlimitedTime;

                        if (null != Recalc)
                            Recalc();

                        this.Hide();

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

        /// <summary>
        /// 更改时间
        /// </summary>
        public void ChangeTime()
        {
            if (DisplayMode == 1)
            {
                if (Hour == "")
                {
                    Hour = "0";
                }
                if (!matchHour.IsMatch(Hour))
                {
                    Hour = "0";
                }
                else
                {
                    Hour = int.Parse(Hour).ToString();
                }
            }
            else if (DisplayMode == 2)
            {
                if (Minute == "")
                {
                    Minute = "0";
                }
                else
                {
                    int minute = 0;
                    if (!int.TryParse(Minute, out minute))
                    {
                        this.Minute = "0";
                    }

                    Minute = minute.ToString();
                }


                if (selectedModel.RoomType == 1)
                {
                    if (!matchMinute.IsMatch(Minute))
                    {
                        Minute = "0";
                    }
                }
                else if (selectedModel.RoomType == 2)
                {
                    if (!matchHour2.IsMatch(Minute))
                    {
                        Minute = "0";
                    }
                }
                
            }


            Calc();
        }


        /// <summary>
        /// 计算
        /// </summary>
        private void Calc()
        {
            double DayD = 0;
            double HourD = 0;
            double MinuteD = 0;

            if (selectedModel.RoomType == 1)
            {
                // 这个改动会将时间最小单位设置为5分钟
                double minuteDouble = double.Parse(Minute);

                HourD = int.Parse(Hour);
                MinuteD = RoundToNumber(minuteDouble);
            }
            else if (selectedModel.RoomType == 2)
            {
                DayD = int.Parse(Hour);
                HourD = int.Parse(Minute);
            }

            if (Mode == 1)
            {
                TimeSpan total = (DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(DayD).AddHours(HourD).AddMinutes(MinuteD) - DateTime.ParseExact(StartTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                TimeSpan balance = (DateTime.Now - DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(DayD).AddHours(HourD).AddMinutes(MinuteD));

                if (selectedModel.RoomType == 1)
                    TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                else if (selectedModel.RoomType == 2)
                    TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                if (balance.TotalMilliseconds < 0)
                {
                    if (selectedModel.RoomType == 1)
                        RemainingTime = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                    else if (selectedModel.RoomType == 2)
                        RemainingTime = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                }
                else
                    RemainingTime = "0:0";


                this.EndTime = DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(DayD).AddHours(HourD).AddMinutes(MinuteD).ToString("yyyy-MM-dd HH:mm:ss");


            }
            else if (Mode == 2)
            {
                TimeSpan total = (DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-DayD).AddHours(-HourD).AddMinutes(-MinuteD) - DateTime.ParseExact(StartTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                TimeSpan balance = (DateTime.Now - DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-DayD).AddHours(-HourD).AddMinutes(-MinuteD));

                if (selectedModel.RoomType == 1)
                    TotalTime = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                else if (selectedModel.RoomType == 2)
                    TotalTime = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                if (balance.TotalMilliseconds < 0) 
                {
                    if (selectedModel.RoomType == 1)
                        RemainingTime = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                    else if (selectedModel.RoomType == 2)
                        RemainingTime = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                }
                else
                    RemainingTime = "0:0";

                this.EndTime = DateTime.ParseExact(EndTimeLong.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-DayD).AddHours(-HourD).AddMinutes(-MinuteD).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }


            /// <summary>
        /// 计算出整数最近的整除5的值
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        private int RoundToNumber(double D)
        {
            //return (int)(5 * Math.Floor(Math.Round(D / 5)));
            return (int)D;
        } 

        

    }
}
