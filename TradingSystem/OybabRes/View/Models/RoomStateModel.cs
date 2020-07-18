using Oybab.DAL;
using Oybab.Res.View.Commands;
using Oybab.Res.View.ViewModels;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Oybab.Res.View.Models
{
    public sealed class RoomStateModel : ViewModelBase
    {

        internal long RoomId { get; set; }

        internal string OrderSession { get; set; }


        internal Order PayOrder { get; set; }

        internal RoomViewModel viewModel { get; set; }


        private string _roomNo;
        /// <summary>
        /// 雅座编号
        /// </summary>
        public string RoomNo
        {
            get { return _roomNo; }
            set
            {
                _roomNo = value;
                OnPropertyChanged("RoomNo");
            }
        }



        private bool _useState;
        /// <summary>
        /// 雅座使用状态(有人,没人)
        /// </summary>
        public bool UseState
        {
            get { return _useState; }
            set
            {
                _useState = value;
                OnPropertyChanged("UseState");
            }
        }



        private bool _called;
        /// <summary>
        /// 是否呼叫
        /// </summary>
        public bool Called
        {
            get { return _called; }
            set
            {
                _called = value;
                OnPropertyChanged("Called");
            }
        }



        private bool _timeup;
        /// <summary>
        /// 雅座使用状态(有人时时间快到期)
        /// </summary>
        public bool Timeup
        {
            get { return _timeup; }
            set
            {
                _timeup = value;
                OnPropertyChanged("Timeup");
            }
        }


        /// <summary>
        /// 刷新状态
        /// </summary>
        internal bool RefreshImageState()
        {
            bool timeup = Timeup;
            bool originalTimeup = Timeup;
            int originalTimeMode = TimeMode;
            bool useState = UseState;
            int timeMode = 0;

            if (null == this.PayOrder)
            {
                timeup = false;
                useState = false;
                TimeMode = 0;
                BalanceMode = 0;
                BalancePrice = "0";
                RoomTime = "0:0";
            }
            else
            {
                int time = 10;

                // 点歌系统剩余半小时显示
                if (Resources.GetRes().IsRequired("Vod"))
                    time = 30;

                timeMode = 1;
                useState = true;
                if (null != this.PayOrder && null != this.PayOrder.EndTime)
                {


                    DateTime now = DateTime.Now;
                    TimeSpan balance = (DateTime.ParseExact(this.PayOrder.EndTime.ToString(), "yyyyMMddHHmmss", null)  - now);
                    Room room = Oybab.Res.Resources.GetRes().Rooms.Where(x => x.RoomId == RoomId).FirstOrDefault();
                    bool isTimeLeft = false;

                    // 针对不同的时间类型有不同的判断, 按小时, 剩余10分钟.  按日, 剩余时间60分钟
                    if (room.IsPayByTime == 1 && (balance).TotalMinutes <= time)
                        isTimeLeft = true;
                    else if (room.IsPayByTime == 2 && (balance).TotalMinutes <= 60)
                        isTimeLeft = true;

                   



                    
                    // 如果剩余时间已经超出了, 默认0:0显示
                    if (now < DateTime.ParseExact(this.PayOrder.EndTime.ToString(), "yyyyMMddHHmmss", null))
                    {
                        if (room.IsPayByTime == 1)
                            RoomTime = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                        else if (room.IsPayByTime == 2)
                            RoomTime = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                    }
                    else
                        RoomTime = "0:0";



                    if (DateTime.ParseExact(this.PayOrder.EndTime.Value.ToString(), "yyyyMMddHHmmss", null) <= now)
                    {
                        timeup = true;
                        timeMode = 3;
                    }

                    if (isTimeLeft)
                    {
                        if (timeMode != 3)
                            timeMode = 2;
                    }
                    else
                    {
                        timeMode = 1;
                    }

                }
                else
                {
                    timeMode = 0;
                }

                if (null != this.PayOrder)
                {
                    double balancePrice = Math.Round(this.PayOrder.TotalPaidPrice - this.PayOrder.TotalPrice, 2);
                    BalancePrice = balancePrice.ToString();
                    if (balancePrice > 0)
                    {
                        BalanceMode = 1;
                    }

                    else if (balancePrice < 0)
                    {
                        BalanceMode = 2;
                    }

                    else if (balancePrice == 0)
                    {
                        BalanceMode = 0;
                    }

                }
                else
                {
                    this.BalanceMode = 0;
                }
            }



            if (UseState != useState)
            {
                UseState = useState;
            }
            if (Timeup != timeup)
            {
                Timeup = timeup;
            }

            if (TimeMode != timeMode)
            {
                TimeMode = timeMode;
            }


            if (Timeup && !originalTimeup)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 打开雅座
        /// </summary>
        public ICommand OpenRoom
        {
            get;
            internal set;
        }



        private int _balanceMode;
        /// <summary>
        /// 余额模式
        /// </summary>
        public int BalanceMode
        {
            get { return _balanceMode; }
            set
            {
                _balanceMode = value;
                OnPropertyChanged("BalanceMode");
            }
        }

        private int _timeMode;
        /// <summary>
        /// 时间模式
        /// </summary>
        public int TimeMode
        {
            get { return _timeMode; }
            set
            {
                _timeMode = value;
                OnPropertyChanged("TimeMode");
            }
        }



        private string _roomTime = "";
        /// <summary>
        /// 雅座时间
        /// </summary>
        public string RoomTime
        {
            get { return _roomTime; }
            set
            {
                _roomTime = value;
                OnPropertyChanged("RoomTime");
            }
        }


        private string _balancePrice;
        /// <summary>
        /// 余额
        /// </summary>
        public string BalancePrice
        {
            get { return _balancePrice; }
            set
            {
                _balancePrice = value;
                OnPropertyChanged("BalancePrice");
            }
        }

    }
}
