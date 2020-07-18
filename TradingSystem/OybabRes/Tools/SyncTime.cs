using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Oybab.Res.Exceptions;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;

namespace Oybab.Res.Tools
{
    /// <summary>
    /// 时间同步
    /// </summary>
    internal class SyncTime
    {

        private static SyncTime syncTime = null;
        private SyncTime() {
            if (Resources.GetRes().AutoSyncClientTime && null == ExceptionPro.Sync)
            {
                ExceptionPro.Sync = SyncServerTimeToClient;
            }
        }

        public static SyncTime GetSyncTime()
        {
            if (null == syncTime)
                syncTime = new SyncTime();
            return syncTime;
        }



        /// <summary>
        /// 同步服务器时间到本地
        /// </summary>
        internal void SyncServerTimeToClient()
        {
            try
            {

                // 管理员身份才可以(如果关闭UAC, 还是不能管理员身份启动, 只能安装包在快捷方式里增加),不需要清单文件权限
                DateTime time = isIPLocal(Resources.GetRes().SERVER_ADDRESS) ? GetLanTime(Resources.GetRes().SERVER_ADDRESS) : GetNetworkTime(Resources.GetRes().SERVER_ADDRESS);


                SYSTEMTIME st = new SYSTEMTIME();
                st.wYear = (short)time.Year; // must be short
                st.wMonth = (short)time.Month;
                st.wDay = (short)time.Day;
                st.wHour = (short)time.Hour;
                st.wMinute = (short)time.Minute;
                st.wSecond = (short)time.Second;
                bool result = Win32SetSystemTime(ref st); // invoke this method.


            }
            catch (Exception ex)
            {
                ExceptionPro.ExpLog(ex, null, false, Resources.GetRes().GetString("Exception_SetClientTimeFailt"));
            }
        }



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
        /// 查看IP是否是本地IP
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        private bool isIPLocal(string ip)
        {
            IPAddress ipaddress = System.Net.IPAddress.Parse(ip);
            String[] straryIPAddress = ipaddress.ToString().Split(new String[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            int[] iaryIPAddress = new int[] { int.Parse(straryIPAddress[0]), int.Parse(straryIPAddress[1]), int.Parse(straryIPAddress[2]), int.Parse(straryIPAddress[3]) };
            if (iaryIPAddress[0] == 10 || (iaryIPAddress[0] == 192 && iaryIPAddress[1] == 168) || (iaryIPAddress[0] == 172 && (iaryIPAddress[1] >= 16 && iaryIPAddress[1] <= 31)))
            {
                return true;
            }
            else
            {
                // IP Address is "probably" public. This doesn't catch some VPN ranges like OpenVPN and Hamachi.
                return false;
            }
        }

        #region API
        [DllImport("netapi32.dll", EntryPoint = "NetRemoteTOD", SetLastError = true,
        CharSet = CharSet.Unicode, ExactSpelling = true,
        CallingConvention = CallingConvention.StdCall)]
        private static extern int NetRemoteTOD(string UncServerName, ref IntPtr BufferPtr);
        [DllImport("netapi32.dll")]
        private static extern void NetApiBufferFree(IntPtr bufptr);

        [StructLayout(LayoutKind.Sequential)]
        public struct structTIME_OF_DAY_INFO
        {
            public int itod_elapsedt;
            public int itod_msecs;
            public int itod_hours;
            public int itod_mins;
            public int itod_secs;
            public int itod_hunds;
            public int itod_timezone;
            public int itod_tinterval;
            public int itod_day;
            public int itod_month;
            public int itod_year;
            public int itod_weekday;
        }
        #endregion


        #region Core functions
        private int[] xNetRemoteTOD(string ServerName)
        {
            structTIME_OF_DAY_INFO result = new structTIME_OF_DAY_INFO();
            IntPtr pintBuffer = IntPtr.Zero;
            int[] TOD_INFO = new int[12];

            // Get the time of day.
            int pintError = NetRemoteTOD(ServerName, ref pintBuffer);
            if (pintError > 0) { throw new System.ComponentModel.Win32Exception(pintError); }

            // Get the structure.
            result = (structTIME_OF_DAY_INFO)Marshal.PtrToStructure(pintBuffer, typeof(structTIME_OF_DAY_INFO));

            TOD_INFO[0] = result.itod_elapsedt;
            TOD_INFO[1] = result.itod_msecs;
            TOD_INFO[2] = result.itod_hours;
            TOD_INFO[3] = result.itod_mins;
            TOD_INFO[4] = result.itod_secs;
            TOD_INFO[5] = result.itod_hunds;
            TOD_INFO[6] = result.itod_timezone;
            TOD_INFO[7] = result.itod_tinterval;
            TOD_INFO[8] = result.itod_day;
            TOD_INFO[9] = result.itod_month;
            TOD_INFO[10] = result.itod_year;
            TOD_INFO[11] = result.itod_weekday;

            // Free the buffer.
            NetApiBufferFree(pintBuffer);

            return TOD_INFO;

        }

        #endregion



        /// <summary>
        /// 获取局域网时间(必须能访问局域网, 比如如果局域网没登录, 就不能获取. (或者也许局域网没有GUEST访问权限吧估计, 我测试环境没开启那个用户, 所以必须能登录才行))
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private DateTime GetLanTime(string ip)
        {
            int[] TOD_Info = new int[12];


            TOD_Info = xNetRemoteTOD(ip);

            DateTime time = new DateTime(TOD_Info[10], TOD_Info[9], TOD_Info[8], TOD_Info[2], TOD_Info[3], TOD_Info[4]);


            return time;
        }


        /// <summary>
        /// 获取网络时间(注意, 必须在服务器开启windows time,然后再gpedit组策略  -- 计算机配置 -- 管理模版 -- 系统 -- windows时间服务 --时间提供程序，在右侧找到“启用 Windows NTP 服务器”双击选择--“已启用”-- 确定  才行)
        /// </summary>
        /// <returns></returns>
        private DateTime GetNetworkTime(string ip)
        {
            ////default Windows time server
            //const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            //var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            //var ipEndPoint = new IPEndPoint(addresses[0], 123);
            var ipEndPoint = new IPEndPoint(System.Net.IPAddress.Parse(ip), 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 15000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime;
        }

        // stackoverflow.com/a/3294698/162671
        uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

    }
}
