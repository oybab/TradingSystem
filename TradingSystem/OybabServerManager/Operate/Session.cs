using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.ServerManager.Operate
{
    internal sealed class Session
    {
        internal TimeSpan ShortTime = TimeSpan.FromMinutes(Resources.GetRes().DB_BACKUP_RETRY);
        internal TimeSpan LongTime = TimeSpan.FromHours(Resources.GetRes().DB_BACKUP_TIME);

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

        internal Session()
        {
            _instance = this;
        }


        #endregion

        private System.Timers.Timer timer = null;
        private Action<bool> Keep = null;


        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="action"></param>
        public void StartSession(Action<bool> action)
        {
            this.Keep = action;
            //X毫秒后执行(1000=1S),然后每隔X执行
            timer = new System.Timers.Timer(LongTime.TotalMilliseconds);
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
                    timer.Interval = LongTime.TotalMilliseconds;//3分钟
                }
                else
                {
                    timer.Interval = ShortTime.TotalMilliseconds;//1分钟
                }
            }
        }

    }
}
