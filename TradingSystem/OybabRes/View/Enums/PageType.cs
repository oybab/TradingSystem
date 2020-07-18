using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Res.View.Enums
{
    public enum PopupType
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 锁屏开启
        /// </summary>
        LockOn,
        /// <summary>
        /// 锁屏关闭
        /// </summary>
        LockOff,
        /// <summary>
        /// 动画开启
        /// </summary>
        AnimationOn,
        /// <summary>
        /// 动画关闭
        /// </summary>
        AnimationOff,
        /// <summary>
        /// 提示
        /// </summary>
        Information,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 提问
        /// </summary>
        Question
    }
}
