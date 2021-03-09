using Oybab.DAL;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.ObjectModel;
using Oybab.TradingSystemX.Tools;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using Oybab.Res.Server.Model;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class ChangeTimeViewModel : ViewModelBase
    {
        private Xamarin.Forms.View _element;
        private long StartTimeLong = 0;
        private long EndTimeLong = 0;
        private Order order;
        private SelectedViewModel selectedModel;
        private Action Recalc;

        internal ChangeTimeViewModel(Xamarin.Forms.View element, SelectedViewModel SelectedViewModel, Action Recalc)
        {
            this._element = element;
            this.selectedModel = SelectedViewModel;
            this.Recalc = Recalc;
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
        /// 初始化需求
        /// </summary>
        internal void InitialView()
        {

            Room room = Resources.Instance.Rooms.Where(x => x.RoomId == selectedModel.RoomId).FirstOrDefault();
            RoomModel model = Resources.Instance.RoomsModel.Where(x => x.RoomId == selectedModel.RoomId).FirstOrDefault();

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
                HourName = Resources.Instance.GetString("Hour");
                MinuteName = Resources.Instance.GetString("Minute");
            }
            else if (selectedModel.RoomType == 2)
            {
                HourName = Resources.Instance.GetString("Day");
                MinuteName = Resources.Instance.GetString("Hour");
            }


            IsDisplayUnlimitedTime = Common.Instance.IsChangeUnlimitedTime(this.order == null);
            UnlimitedTime = selectedModel.TempUnlimitedTime;


            Calc();

            IsShow = true;
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
                if (_isShow == true)
                    NavigationPath.Instance.OpenPanel();
                OnPropertyChanged("IsShow");
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
        public Command OKCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new RelayCommand(param =>
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

                    selectedModel.IsLoading = true;
                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            Calc();


                            long ReturnValue = 0;

                            // 这个改动会将时间最小单位设置为5分钟
                            if (selectedModel.RoomType == 1)
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

                            // 关闭面板
                            NavigationPath.Instance.ClosePanels(false);

                            IsShow = false;
                            selectedModel.IsLoading = false;
                        });
                    });
                }));
            }
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public Command CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new RelayCommand(param =>
                {
                    selectedModel.IsLoading = true;

                    Task.Run(async () =>
                    {

                        await ExtX.WaitForLoading();
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                        {

                            // 关闭面板
                            NavigationPath.Instance.ClosePanels(false);

                            this.IsShow = false;

                            selectedModel.IsLoading = false;

                        });
                    });
                }));
            }
        }


        /// <summary>
        /// 更改时间
        /// </summary>
        public void ChangeTime()
        {
            if (IsShow)
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
