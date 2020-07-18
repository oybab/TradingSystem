using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Oybab.ServerManager.Operate
{
    internal sealed class Time
    {
        #region Instance
        private Time() { }
        private static readonly Lazy<Time> lazy = new Lazy<Time>(() => new Time());
        public static Time Instance { get { return lazy.Value; } }
        #endregion Instance




        #region Time

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
        public extern static bool Win32SetSystemTime(ref SYSTEMTIME st);

        #endregion


        /// <summary>
        /// 设置时间
        /// </summary>
        /// <param name="time"></param>
        internal void SetTime(DateTime time)
        {
            return;
#if !DEBUG

            // 必须先转UTC
            time = time.ToUniversalTime();

            SYSTEMTIME st = new SYSTEMTIME();
            st.wYear = (short)time.Year; // must be short
            st.wMonth = (short)time.Month;
            st.wDay = (short)time.Day;
            st.wHour = (short)time.Hour;
            st.wMinute = (short)time.Minute;
            st.wSecond = (short)time.Second;

#endif
        }
    }
}
