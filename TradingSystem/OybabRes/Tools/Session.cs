using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;

namespace Oybab.Res.Tools
{
    public sealed class Session
    {
        #region Instance
        /// <summary>
        /// For Instance
        /// </summary>
        private static Session _instance;
        public static Session Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Session();
                return _instance;
            }
        }

        public Session()
        {
            _instance = this;
        }


        #endregion

        private System.Timers.Timer timer = null;
        public Action<bool> Keep = null;
        private int defaultLong = 1000 * 60 * 3; // 3分钟
        private int defaultShort = 1000 * 60 * 1; // 1分钟



        internal void Stop()
        {
            timer.Stop();
        }

        internal void Start()
        {
            timer.Interval = defaultLong;
            timer.Start();
        }

        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="action"></param>
        public void StartSession(Action<bool> action)
        {
            this.Keep = action;
            //X毫秒后执行(1000=1S),然后每隔X执行
            timer = new System.Timers.Timer(defaultLong);
            timer.Elapsed += (sender, e) =>
            {
                action(true);
            };
            timer.Start();
        }

        /// <summary>
        /// 修改检查频率
        /// </summary>
        internal void ChangeInterval(bool IsLong)
        {
            if (null != timer && null != this.Keep)
            {
                if (IsLong)
                {
                    timer.Interval = defaultLong;//3分钟
                }
                else
                {
                    timer.Interval = defaultShort;//1分钟
                }
            }
        }
        
    }
}
