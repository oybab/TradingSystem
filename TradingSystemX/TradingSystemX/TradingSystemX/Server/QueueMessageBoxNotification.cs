using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oybab.TradingSystemX.Server
{
    /// <summary>
    /// 顺序提示框提示
    /// </summary>
    public sealed class QueueMessageBoxNotification
    {

        #region Instance
        private QueueMessageBoxNotification()
        {
            ActionMessageBox = (sender, page, title, content, mode, imageMode, buttonMode, command, args) =>
            {
                MsgList.Enqueue(new MessageBoxInfo() { Sender= sender, Page = page, Title = title, Content = content, Mode = mode, ImageMode = imageMode, ButtonMode = buttonMode, Command = command, Args = args });
                MoveNext();
            };

        }

        private static readonly Lazy<QueueMessageBoxNotification> _instance = new Lazy<QueueMessageBoxNotification>(() => new QueueMessageBoxNotification());
        public static QueueMessageBoxNotification Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #endregion Instance

        /// <summary>
        /// 信息列表
        /// </summary>
        private Queue<MessageBoxInfo> MsgList = new Queue<MessageBoxInfo>();
        private bool IsDisplay { get; set; }

        internal void MoveNext(bool? IsDisplay = null)
        {
            if (null != IsDisplay)
                this.IsDisplay = IsDisplay.Value;


            // 这里解决一个BUG. 由于显示多选菜单栏时按back按钮而没单击菜单上的的按钮 可能会导致菜单没能从下面的队列中清空, 导致后续任何弹出框不会再出现
            if (null != NotificationMessageBox && this.IsDisplay == true)
            {
                if (MsgList.Count == 1 && lastMessage.ButtonMode == MessageBoxButtonMode.CustomMultiple)
                {
                    IsDisplay = false;
                    MoveNext(false);
                }
            }

            if (null != NotificationMessageBox && this.IsDisplay == false)
            {
                if (MsgList.Count > 0)
                {
                    this.IsDisplay = true;
                    MessageBoxInfo messageBox = MsgList.Dequeue();
                    lastMessage = messageBox;

                    NotificationMessageBox(messageBox.Sender, messageBox.Page, messageBox.Title, messageBox.Content, messageBox.Mode, messageBox.ImageMode, messageBox.ButtonMode, messageBox.Command, messageBox.Args);
                }
            }

            


            // 如果上面的BUG没能解决, 只能频繁出现这种问题时手动清空(如果一直没出现这个BUG报告, 考虑去除
            if (MsgList.Count > 3)
            {
                foreach (var item in MsgList)
                {
                    App.Current.MainPage.DisplayAlert("BUG", item.Content, CommandTitles.Instance.OK);
                }

                this.IsDisplay = false;
                MsgList.Clear();
            }

        }


        internal event EventMessageBox NotificationMessageBox;

        public Action<object, object, string, string, MessageBoxMode, MessageBoxImageMode, MessageBoxButtonMode, Action<string>, object> ActionMessageBox;




        // For fix not show alert for multiple messagebox BUG
        private MessageBoxInfo lastMessage = null;

    }


    

    internal class MessageBoxInfo
    {
        internal object Sender { get; set; }
        internal Object Page { get; set; }
        internal string Title { get; set; }
        internal string Content { get; set; }
        internal MessageBoxMode Mode { get; set; }
        internal MessageBoxImageMode ImageMode { get; set; }
        internal MessageBoxButtonMode ButtonMode { get; set; }
        internal Action<string> Command { get; set; }
        internal object Args { get; set; }
    }



    internal delegate void EventMessageBox(object sender, object page, string title, string content, MessageBoxMode mode, MessageBoxImageMode imageMode, MessageBoxButtonMode buttonMode, Action<string> command, object args);
}
