using Oybab.Res.View.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Oybab.Res.View.EventArgs
{
    /// <summary>
    /// 弹出窗口路由事件参数
    /// </summary>
    public sealed class PopupRoutedEventArgs : RoutedEventArgs
    {
        public PopupType PopupType { get; set; }
        public Action<string> Operate { get; set; }
        public string Msg { get; set; }

        public PopupRoutedEventArgs(RoutedEvent routedEvent, object source, string Msg, Action<string> Operate, PopupType PopupType)
            : base(routedEvent, source)
        {
            this.PopupType = PopupType;
            this.Operate = Operate;
            this.Msg = Msg;
        }
    }
}
