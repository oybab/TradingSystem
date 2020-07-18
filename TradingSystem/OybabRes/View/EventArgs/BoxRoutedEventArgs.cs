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
    internal sealed class BoxRoutedEventArgs : RoutedEventArgs
    {
        internal object Param { get; set; }
        public BoxType BoxType { get; set; }
        public Action<string> Operate { get; set; }
        public string Msg { get; set; }

        public BoxRoutedEventArgs(RoutedEvent routedEvent, object source, string Msg, Action<string> Operate, BoxType BoxType, object Param)
            : base(routedEvent, source)
        {
            this.BoxType = BoxType;
            this.Operate = Operate;
            this.Msg = Msg;
            this.Param = Param;
        }
    }
}
