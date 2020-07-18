using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Oybab.Res.View.Events
{
    public sealed class PublicEvents : UIElement
    {
        #region PopupEvent
        /// <summary>
        /// 路由处理
        /// </summary>
        public static readonly RoutedEvent PopupEvent = EventManager.RegisterRoutedEvent("PopupEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PublicEvents));
        public event RoutedEventHandler PoPup
        {
            add { AddHandler(PopupEvent, value); }
            remove { RemoveHandler(PopupEvent, value); }
        }
        #endregion


        #region ForwardEvent
        /// <summary>
        /// 弹出框处理
        /// </summary>
        public static readonly RoutedEvent ForwardEvent = EventManager.RegisterRoutedEvent("ForwardEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PublicEvents));
        public event RoutedEventHandler Forward
        {
            add { AddHandler(ForwardEvent, value); }
            remove { RemoveHandler(ForwardEvent, value); }
        }
        #endregion


        #region BoxEvent
        /// <summary>
        /// 弹出窗口处理
        /// </summary>
        public static readonly RoutedEvent BoxEvent = EventManager.RegisterRoutedEvent("BoxEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PublicEvents));
        public event RoutedEventHandler Box
        {
            add { AddHandler(BoxEvent, value); }
            remove { RemoveHandler(BoxEvent, value); }
        }
        #endregion
    }
}
