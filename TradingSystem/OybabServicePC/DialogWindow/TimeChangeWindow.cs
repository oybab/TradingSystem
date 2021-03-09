using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oybab.DAL;
using Oybab.Res;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.Tools;

namespace Oybab.ServicePC.DialogWindow
{
    internal sealed partial class TimeChangeWindow : KryptonForm
    {
        private string Mark = "+";
        private long StartTime = 0;
        private long EndTime = 0;
        private string Minute = "0";
        private string Hour = "0";
        private long IsPayByTime = 0;

        public long ReturnValue { get; private set; } //返回值
        internal bool UnlimitedTime = false; // 返回值

        public TimeChangeWindow(string mark, long startTime, long endTime, long isPayByTime, bool IsTimeCheckUnlimited = false, bool IsnOrdertNull = false)
        {

            InitializeComponent();
            krptHour.LostFocus += krptHour_LostFocus;
            krptMinute.LostFocus += krptMinute_LostFocus;


            // 点歌系统就显示呼叫
            if (Resources.GetRes().IsRequired("Vod") && mark == "-" && isPayByTime == 1)
                krpbRemoveTime.Visible = true;

            this.Mark = mark;
            this.StartTime = startTime;
            this.EndTime = endTime;
            if (Mark == "+")
                krplChangeMark.Text = "+";
            else if (Mark == "-")
                krplChangeMark.Text = "-";

            this.IsPayByTime = isPayByTime;


            krplStartTimeValue.Text = DateTime.ParseExact(StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            krplEndTimeValue.Text = DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");

            krptHour.Text = "0";
            krptMinute.Text = "0";

            this.Text = Resources.GetRes().GetString("ChangeTime");
            krplStartTime.Text = Resources.GetRes().GetString("StartTime");
            krplEndTime.Text = Resources.GetRes().GetString("EndTime");
            krplChangeTime.Text = Resources.GetRes().GetString("ChangeTime");
            krplTotalTime.Text = Resources.GetRes().GetString("TotalTime");
            krplRemainingTime.Text = Resources.GetRes().GetString("RemainingTime");
            krpcbTimeUnlimited.Text = Resources.GetRes().GetString("UnlimitedTime");

            if (IsPayByTime == 1)
            {
                krplHour.Text = Resources.GetRes().GetString("Hour");
                krplMinute.Text = Resources.GetRes().GetString("Minute");
            }
            else if (IsPayByTime == 2)
            {
                krplHour.Text = Resources.GetRes().GetString("Day");
                krplMinute.Text = Resources.GetRes().GetString("Hour");
            }
            krpbSave.Text = Resources.GetRes().GetString("Save");

            if (int.Parse(Resources.GetRes().GetString("HightFix")) != 0)
            {
                krptHour.Location = new Point(krptHour.Location.X, krptHour.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
                krptMinute.Location = new Point(krptMinute.Location.X, krptMinute.Location.Y + int.Parse(Resources.GetRes().GetString("HightFix")).RecalcMagnification2());
            }
            Assembly asm = Assembly.LoadFrom(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Res.dll"));
            this.Icon = new Icon(asm.GetManifestResourceStream(@"Oybab.Res.Resources.Images.PC.ChangeTime.ico"));

            krpcbTimeUnlimited.Checked = IsTimeCheckUnlimited;

            krpcbTimeUnlimited.Visible = Common.GetCommon().IsChangeUnlimitedTime(IsnOrdertNull);


            Calc();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbSave_Click(object sender, EventArgs e)
        {
            if (Mark == "+")
            {
                if (IsPayByTime == 1)
                {
                    ReturnValue = long.Parse(DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddHours(int.Parse(Hour)).AddMinutes(int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                }
                else if (IsPayByTime == 2)
                {
                    ReturnValue = long.Parse(DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(int.Parse(Hour)).AddHours(int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                }
                
            }
            else if (Mark == "-")
            {
                if (IsPayByTime == 1)
                {
                    ReturnValue = long.Parse(DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddHours(-int.Parse(Hour)).AddMinutes(-int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                }
                else if (IsPayByTime == 2)
                {
                    ReturnValue = long.Parse(DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-int.Parse(Hour)).AddHours(-int.Parse(Minute)).ToString("yyyyMMddHHmmss"));
                }
            }


            this.UnlimitedTime = krpcbTimeUnlimited.Checked;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 计算
        /// </summary>
        private void Calc()
        {
            

            double DayD = 0;
            double HourD = 0;
            double MinuteD = 0;

            if (IsPayByTime == 1)
            {
                double minuteDouble = double.Parse(Minute);
                Minute = RoundToNumber(minuteDouble).ToString();

                HourD = int.Parse(Hour);
                MinuteD = int.Parse(Minute);
            }
            else if (IsPayByTime == 2)
            {
                DayD = int.Parse(Hour);
                HourD = int.Parse(Minute);
            }

            if (Mark == "+")
            {
                TimeSpan total = (DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(DayD).AddHours(HourD).AddMinutes(MinuteD) - DateTime.ParseExact(StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                TimeSpan balance = (DateTime.Now - DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(DayD).AddHours(HourD).AddMinutes(MinuteD));
                
                if (IsPayByTime == 1)
                    krplTotalTimeValue.Text = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                else if (IsPayByTime == 2)
                    krplTotalTimeValue.Text = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                if (balance.TotalMilliseconds < 0) 
                {
                    if (IsPayByTime == 1)
                        krplRemainingTimeValue.Text = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                    else if (IsPayByTime == 2)
                        krplRemainingTimeValue.Text = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                }
                else
                    krplRemainingTimeValue.Text = "0:0";

                krplEndTimeValue.Text = DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(DayD).AddHours(HourD).AddMinutes(MinuteD).ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (Mark == "-")
            { 
                TimeSpan total = (DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-DayD).AddHours(-HourD).AddMinutes(-MinuteD) - DateTime.ParseExact(StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                TimeSpan balance = (DateTime.Now - DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-DayD).AddHours(-HourD).AddMinutes(-MinuteD));

                if (IsPayByTime == 1)
                    krplTotalTimeValue.Text = string.Format("{0}:{1}", (int)total.TotalHours, total.Minutes);
                else if (IsPayByTime == 2)
                    krplTotalTimeValue.Text = string.Format("{0}/{1}:{2}", (int)total.TotalDays, total.Hours, total.Minutes);

                if (balance.TotalMilliseconds < 0)
                {
                    if (IsPayByTime == 1)
                        krplRemainingTimeValue.Text = string.Format("{0}:{1}", (int)balance.TotalHours, balance.Minutes);
                    else if (IsPayByTime == 2)
                        krplRemainingTimeValue.Text = string.Format("{0}/{1}:{2}", (int)balance.TotalDays, balance.Hours, balance.Minutes);
                }
                else
                    krplRemainingTimeValue.Text = "0:0";


                krplEndTimeValue.Text = DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(-DayD).AddHours(-HourD).AddMinutes(-MinuteD).ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        Regex matchHour = new Regex(@"^\d{0,2}$");
        Regex matchMinute = new Regex(@"^([0-9]|[1-4][0-9]|5[0-7])$");


        Regex matchHour2 = new Regex(@"^([0-9]|1[0-9]|2[0-3])$");

        /// <summary>
        /// 小时改动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptHour_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                krpbSave_Click(null, null);

            if (krptHour.Text == "")
            {
                krptHour.Text = "0";
                krptHour.SelectAll();
            }
            if (!matchHour.IsMatch(krptHour.Text))
            {
                krptHour.Text = Hour;
                return;
            }
            Hour = krptHour.Text;


            Calc();

            e.SuppressKeyPress = true;
        }

        private void Enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                krpbSave_Click(null, null);
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }


        /// <summary>
        /// 分钟改动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krptMinute_KeyUp(object sender, KeyEventArgs e)
        {
            if (krptMinute.Text == "")
            {
                krptMinute.Text = "0";
                krptMinute.SelectAll();
            }
            if (IsPayByTime == 1)
            {
                if (!matchMinute.IsMatch(krptMinute.Text))
                {
                    krptMinute.Text = Minute;
                    return;
                }
            }
            else if (IsPayByTime == 2)
            {
                if (!matchHour2.IsMatch(krptMinute.Text))
                {
                    krptMinute.Text = Minute;
                    return;
                }
            }
            Minute = krptMinute.Text;
            

            Calc();

        }


        private void krptMinute_LostFocus(object sender, System.EventArgs e)
        {
            if (krptMinute.Text != Minute)
            {
                krptMinute.Text = Minute;
            }

            if (IsPayByTime == 1)
            {
                double minuteDouble = double.Parse(Minute);
                krptMinute.Text = Minute = RoundToNumber(minuteDouble).ToString();
            }
            
            Calc();

        }

        private void krptHour_LostFocus(object sender, System.EventArgs e)
        {
            if (krptHour.Text != Hour)
            {
                krptHour.Text = Hour;
            }
        }


        /// <summary>
        /// 计算出整数最近的整除5的值
        /// </summary>
        /// <param name="D"></param>
        /// <returns></returns>
        private int RoundToNumber(double D)
        {
            return (int)D;
        } 

        /// <summary>
        /// 取最小值
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private int RoundingDown(double d)
        {
            return (int)(Math.Floor(d / 5) * 5);
        }


        /// <summary>
        /// 去掉多余的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void krpbRemoveTime_Click(object sender, EventArgs e)
        {

            TimeSpan balance = (DateTime.Now - DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(0).AddHours(0).AddMinutes(0));

            if (balance.TotalMilliseconds < 0)
            {
                int hours = Math.Abs((int)balance.TotalHours);
                int minutes = Math.Abs(balance.Minutes);

                TimeSpan total = (DateTime.ParseExact(EndTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddDays(0).AddHours(-hours).AddMinutes(-minutes) - DateTime.ParseExact(StartTime.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture));
                
                krptHour.Text = (hours).ToString();
                krptMinute.Text = (minutes).ToString();

                Hour = krptHour.Text;
                Minute = krptMinute.Text;

                Calc();
            }

        }
    }
}
