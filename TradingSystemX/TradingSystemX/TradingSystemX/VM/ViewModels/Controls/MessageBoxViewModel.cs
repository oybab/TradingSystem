using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Oybab.TradingSystemX.VM.ViewModels.Navigations;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class MessageBoxViewModel : ViewModelBase
    {
        private Action<string> Command;
        private Page _element;

        internal MessageBoxViewModel(Page _element)
        {
            this._element = _element;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="page"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        /// <param name="imageMode"></param>
        /// <param name="buttonMode"></param>
        /// <param name="command"></param>
        /// <param name="args"></param>
        internal void Instance_NotificationMessageBox(object sender, object page, string title, string content, MessageBoxMode mode, MessageBoxImageMode imageMode, MessageBoxButtonMode buttonMode, Action<string> command, object args)
        {
            bool isCurrentPage = false;
            lock (NavigationPath.Instance)
            {
                if (_element == NavigationPath.Instance.CurrentNavigate)
                    isCurrentPage = true;
                
            }

            if (isCurrentPage)
            {
                Show(title, content, mode, imageMode, buttonMode, command, args);
            }
        }

        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="mode"></param>
        /// <param name="imageMode"></param>
        /// <param name="buttonMode"></param>
        /// <param name="Command"></param>
        internal void Show(string title, string content, MessageBoxMode mode, MessageBoxImageMode imageMode, MessageBoxButtonMode buttonMode, Action<string> Command, object args)
        {
            this.Command = Command;
            this.Mode = (int)mode;
            this.ImageMode = (int)imageMode;
            this.ButtonMode = (int)buttonMode;
            this.Title = title;
            this.Content = content;

            IsDisplay = true;



            if (buttonMode == MessageBoxButtonMode.YesNo)
            {
               
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var answer = await App.Current.MainPage.DisplayAlert(this.Title, this.Content, CommandTitles.Instance.Yes, CommandTitles.Instance.No);

                        if (null != Command)
                        {
                            if (answer)
                                this.OKCommand.Execute("YES");
                            else
                                this.OKCommand.Execute("NO");
                        }else
                        {
                            ExecuteCommand(null);
                        }
                    });
                
            }
            else if (buttonMode == MessageBoxButtonMode.CustomMultiple)
            {

                Device.BeginInvokeOnMainThread(async () =>
                {
                    Dictionary<string, string> objList = args as Dictionary<string, string>;
                    string answer = "";

                    if (objList.Count == 1)
                        answer = await App.Current.MainPage.DisplayActionSheet(this.Content, CommandTitles.Instance.Cancel, null, objList.ElementAt(0).Key);
                    else if (objList.Count == 2)
                        answer = await App.Current.MainPage.DisplayActionSheet(this.Content, CommandTitles.Instance.Cancel, null, objList.ElementAt(0).Key, objList.ElementAt(1).Key);
                    else if (objList.Count == 3)
                        answer = await App.Current.MainPage.DisplayActionSheet(this.Content, CommandTitles.Instance.Cancel, null, objList.ElementAt(0).Key, objList.ElementAt(1).Key, objList.ElementAt(2).Key);
                    else if (objList.Count == 4)
                        answer = await App.Current.MainPage.DisplayActionSheet(this.Content, CommandTitles.Instance.Cancel, null, objList.ElementAt(0).Key, objList.ElementAt(1).Key, objList.ElementAt(2).Key, objList.ElementAt(3).Key);
                    else if (objList.Count == 5)
                        answer = await App.Current.MainPage.DisplayActionSheet(this.Content, CommandTitles.Instance.Cancel, null, objList.ElementAt(0).Key, objList.ElementAt(1).Key, objList.ElementAt(2).Key, objList.ElementAt(3).Key, objList.ElementAt(4).Key);

                    if (null != Command && !string.IsNullOrWhiteSpace(answer) && objList.ContainsKey(answer))
                    {
                        this.OKCommand.Execute(objList[answer]);
                    }
                    else 
                    {
                        ExecuteCommand(null);
                    }
                });

            }
            else if (buttonMode == MessageBoxButtonMode.RetryExit)
            {

                Device.BeginInvokeOnMainThread(async () =>
                {
                    var answer = await App.Current.MainPage.DisplayAlert(this.Title, this.Content, CommandTitles.Instance.Retry, CommandTitles.Instance.Exit);

                    if (null != Command)
                    {
                        if (answer)
                            this.OKCommand.Execute("Retry");
                        else
                            this.OKCommand.Execute("Exit");
                    }
                    else
                    {
                        ExecuteCommand(null);
                    }
                });

            }
            else if (buttonMode == MessageBoxButtonMode.OK)
            {
               
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await App.Current.MainPage.DisplayAlert(this.Title, this.Content, CommandTitles.Instance.OK);

                        if (null != Command)
                        {
                            this.OKCommand.Execute("OK");
                        }
                        else
                        {
                            ExecuteCommand(null);
                        }

                        if (OperatesService.Instance.IsExpired || OperatesService.Instance.IsAdminUsing)
                        {
                            MainViewModel viewModel = NavigationPath.Instance.CurrentNavigate.BindingContext as MainViewModel;
                            viewModel.ReLyout();
                        }
                    });
                
            }
        }

    


        private bool _isDisplay = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set
            {
                _isDisplay = value;
                OnPropertyChanged("IsDisplay");
            }
        }



        private string _title = "";
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }



        private string _content = "";
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }



        private int _mode = 0;
        /// <summary>
        /// 模式(1窗体弹出)
        /// </summary>
        public int Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                OnPropertyChanged("Mode");
            }
        }

        private int _imageMode = 0;
        /// <summary>
        /// 显示图类型(0蓝色感叹号表示提醒,1黄金感叹号表示警告,2问号表示提问,3红色叉表示错误)
        /// </summary>
        public int ImageMode
        {
            get { return _imageMode; }
            set
            {
                _imageMode = value;
                OnPropertyChanged("ImageMode");
            }
        }



        private int _buttonMode = 0;
        /// <summary>
        /// 显示按钮类型(1确定,2是,否)
        /// </summary>
        public int ButtonMode
        {
            get { return _buttonMode; }
            set
            {
                _buttonMode = value;
                OnPropertyChanged("ButtonMode");
            }
        }


        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="Button"></param>
        private void ExecuteCommand(string command)
        {
            if (!string.IsNullOrWhiteSpace(command))
                Command(command);


            IsDisplay = false;

           QueueMessageBoxNotification.Instance.MoveNext(false);

        }


        /// <summary>
        /// 输入确定
        /// </summary>
        private RelayCommand _oKCommand;
        public Command OKCommand
        {
            get
            {
                return _oKCommand ?? (_oKCommand = new RelayCommand(param => ExecuteCommand(param.ToString())));
            }
        }


    }










    /// <summary>
    /// 单击的按钮
    /// </summary>
    internal enum MessageBoxButton
    {
       Ok,
       Yes,
       No,
       Cancel,
       Ignore,
       Retry,
       Exit
    }






    public enum MessageBoxImageMode
    {
        None,
        Information,
        Warn,
        Question,
        Error
    }


    public enum MessageBoxMode
    {
        Dialog,
        Sheet
    }

    public enum MessageBoxButtonMode
    {
        OK,
        YesNo,
        RetryExit,
        CustomMultiple,
        //OKCancelIgnore,
        //OKCancelRetry,
        //OKIgnoreRetry
    }
}
