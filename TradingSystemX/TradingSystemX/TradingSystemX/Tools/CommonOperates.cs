using Oybab.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Oybab.TradingSystemX.Tools
{
    internal sealed class CommonOperates
    {

        #region Instance
        private CommonOperates() { }

        private static readonly Lazy<CommonOperates> _instance = new Lazy<CommonOperates>(() => new CommonOperates());
        internal static CommonOperates Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance

        /// <summary>
        /// 获取雅座费
        /// </summary>
        /// <returns></returns>
        public double GetRoomPrice(Order order, double Price, double PriceHour, long IsPaybyTime, int totalMinute, bool IsSubTime, long? EndTime, bool ForcedCalc = false)
            {
                // 如果不是按小时收费, 则直接返回固定的价格
                if (IsPaybyTime == 0)
                {
                    return Price;
                }
                // 如果是按时间收费则计算时间
                else
                {
                    // 如果是老订单, 并且时间没变, 则无需改动
                    if (null != order && order.RoomPriceCalcTime == EndTime && !ForcedCalc)
                    {
                        return order.RoomPrice;
                    }
                    else
                    {
                        // 总时间0或小于0价格为0
                        if (totalMinute < 0)
                            return 0;
                        // 没定时间, 分钟为0, 或者刚开始
                        else if (totalMinute == 0)
                        {
                            return 0;
                        }
                        else
                        {
                           
                            double timePrice = 0;

                            if (IsPaybyTime == 1)
                                timePrice = CalcHourPrice(totalMinute, PriceHour, Resources.Instance.MinutesIntervalTime);
                            else if (IsPaybyTime == 2)
                                timePrice = CalcDayPrice((int)Math.Ceiling(TimeSpan.FromMinutes(totalMinute).TotalHours), PriceHour, Resources.Instance.HoursIntervalTime);

                            if (null == order || IsSubTime)
                                return Math.Round(timePrice, 2);
                            else
                                return Math.Round(timePrice + order.RoomPrice, 2);
                        }
                    }
                }
            }





            /// <summary>
            /// 获取雅座费(替换时)
            /// </summary>
            /// <returns></returns>
            public double GetRoomPrice(double Price, double PriceHour, long IsPaybyTime, int totalMinute)
            {
                // 如果不是按小时收费, 则直接返回固定的价格
                if (IsPaybyTime == 0)
                {
                    return Price;
                }
                // 如果是按时间收费则计算时间
                else
                {

                    // 总时间0或小于0价格为0
                    if (totalMinute < 0)
                        return 0;
                    // 没定时间, 分钟为0, 或者刚开始
                    else if (totalMinute == 0)
                    {
                        // 刚开始说不定还不到0呢.(貌似这个算法只在自动计算时才能用.)
                        //return Math.Round(PriceHour / (60 / Resources.GetRes().OrderIntervalTime), 2);
                        return 0;
                    }
                    else
                    {

                        double timePrice = 0;

                        if (IsPaybyTime == 1)
                            timePrice = CalcHourPrice(totalMinute, PriceHour, Resources.Instance.MinutesIntervalTime);
                        else if (IsPaybyTime == 2)
                            timePrice = CalcDayPrice((int)Math.Ceiling(TimeSpan.FromMinutes(totalMinute).TotalHours), PriceHour, Resources.Instance.HoursIntervalTime);

                        return Math.Round(timePrice, 2);

                    }
                }
            }




            /// <summary>
            /// 计算时间(小时)
            /// </summary>
            /// <param name="totalMinute"></param>
            /// <param name="PriceHour"></param>
            /// <param name="IntervalMinuteTime"></param>
            /// <returns></returns>
            private double CalcHourPrice(int totalMinute, double PriceHour, int IntervalMinuteTime)
            {
                int hour = totalMinute / 60;
                int minute = totalMinute % 60;

                double hourPrice = hour * PriceHour;
                double minutePrice = 0;

                // 如果剩余的分钟大于 固定分钟 则再取余后+1. 不是则直接+1
                if (minute > 0 && minute <= IntervalMinuteTime)
                    minutePrice = Math.Round(PriceHour / (60 / IntervalMinuteTime), 2);
                else
                {
                    int internalCount = minute / IntervalMinuteTime;

                    if (minute % IntervalMinuteTime != 0)
                        internalCount = internalCount + 1;


                    minutePrice = PriceHour / (60 / IntervalMinuteTime) * internalCount;

                }

                return hourPrice + minutePrice;
            }




            /// <summary>
            /// 计算时间(日)
            /// </summary>
            /// <param name="totalHours"></param>
            /// <param name="PriceDay"></param>
            /// <param name="IntervalHourTime"></param>
            /// <returns></returns>
            private double CalcDayPrice(int totalHours, double PriceDay, int IntervalHourTime)
            {

                int day = totalHours / 24;
                int hour = totalHours % 24;

                double dayPrice = day * PriceDay;
                double hourPrice = 0;

                // 如果剩余的分钟大于 固定分钟 则再取余后+1. 不是则直接+1
                if (hour > 0 && hour <= IntervalHourTime)
                    hourPrice = Math.Round(PriceDay / (24 / IntervalHourTime), 2);
                else
                {
                    int internalCount = hour / IntervalHourTime;

                    if (hour % IntervalHourTime != 0)
                        internalCount = internalCount + 1;


                    hourPrice = PriceDay / (24 / IntervalHourTime) * internalCount;

                }

                return Math.Round(dayPrice + hourPrice, 2);
            }








            private Regex _gtinRegex2 = new System.Text.RegularExpressions.Regex("^\\d{8}$");
            public string GenerateEAN8(string code)
            {
                if (code.Length <= 5)
                {
                    code = code.PadLeft(7, '0');
                    code = code.PadRight(8, '0');
                }
                else
                    return code;

                if (!(_gtinRegex2.IsMatch(code))) return ""; // 检查是否是13位数字
                                                            
                int[] mult = Enumerable.Range(0, 7).Select(i => ((int)char.GetNumericValue(code[i])) * ((i % 2 == 0) ? 3 : 1)).ToArray(); // STEP 1: without check digit, "Multiply value of each position" by 3 or 1
                int sum = mult.Sum(); // STEP 2: "Add results together to create sum"
                return code.Substring(0, 7) + (10 - (sum % 10)) % 10;
            }


            private Regex _gtinRegex = new System.Text.RegularExpressions.Regex("^\\d{13}$");
            private string GenerateEAN13(string code)
            {
                if (code.Length < 13)
                {

                    code = code.PadLeft(12, '0');
                    code = code.PadRight(13, '0');
                }

                if (!(_gtinRegex.IsMatch(code))) return ""; // 检查是否是13位数字
                string newCode = code.PadLeft(14, '0'); // stuff zeros at start to garantee 14 digits
                int[] mult = Enumerable.Range(0, 13).Select(i => ((int)char.GetNumericValue(newCode[i])) * ((i % 2 == 0) ? 3 : 1)).ToArray(); // STEP 1: without check digit, "Multiply value of each position" by 3 or 1
                int sum = mult.Sum(); // STEP 2: "Add results together to create sum"
                return code.Substring(0, 12) + (10 - (sum % 10)) % 10;
            }



        }
    
}
