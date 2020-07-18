using Oybab.Res.View.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Oybab.Res.View.EventArgs
{
        /// <summary>
    /// 页面跳转路由事件参数
    /// </summary>
    public sealed class ForwardRoutedEventArgs : RoutedEventArgs
    {
        public PageType PageType { get; set; }
        public object Param { get; set; }

        public ForwardRoutedEventArgs(RoutedEvent routedEvent, object source, PageType PageType)
            : base(routedEvent, source)
        {
            this.PageType = PageType;
        }

        public ForwardRoutedEventArgs(RoutedEvent routedEvent, object source, PageType PageType, object Param)
            : base(routedEvent, source)
        {
            this.PageType = PageType;
            this.Param = Param;
        }
    }

}
