using Oybab.DAL;
using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using Oybab.Res.View.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Oybab.Res.Tools;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class RequestViewModel : ViewModelBase
    {
        private UIElement _element;
        private Panel RequestList;
        private DetailsModel model;
        private Style requestStyle;

        internal RequestViewModel(UIElement element, Panel requestList)
        {
            this._element = element;
            this.RequestList = requestList;

            this.requestStyle = (RequestList as FrameworkElement).FindResource("cbRequestSelectStyle") as Style;
        }


        private List<RequestModel> resultRequestList = new List<RequestModel>();
        /// <summary>
        /// 初始化需求
        /// </summary>
        internal void InitialRequest(DetailsModel model)
        {
            this.model = model;
            resultRequestList.Clear();
            this.RequestList.Children.Clear();

            List<Request> lists = Resources.GetRes().Requests.Where(x => x.IsEnable == 1).OrderByDescending(x => x.Order).ThenByDescending(x => x.RequestId).ToList();
            
            

            foreach (var item in lists)
            {
                RequestModel request = new RequestModel() {  IsSelected  = false, Request = item };

                if (!string.IsNullOrWhiteSpace(model.OrderDetail.Request))
                {
                    if (model.OrderDetail.Request.Split(',').Contains(item.RequestId.ToString()))
                    {
                        request.IsSelected = true;
                    }
                }

                resultRequestList.Add(request);
            }


            foreach (var item in resultRequestList)
            {
                AddRequestsItem(item);
            }
        }





        /// <summary>
        /// 添加雅座
        /// </summary>
        /// <param name="item"></param>
        private void AddRequestsItem(RequestModel item)
        {
            _element.Dispatcher.BeginInvoke(new Action(() =>
            {
                CheckBox btn = new CheckBox();
                btn.Style = requestStyle;
                btn.DataContext = item;
                btn.Command = SelectRequest;
                btn.CommandParameter = item;
                RequestList.Children.Add(btn);
            }));
        }



        /// <summary>
        /// 选择需求
        /// </summary>
        private RelayCommand _selectRequest;
        public ICommand SelectRequest
        {
            get
            {
                if (_selectRequest == null)
                {
                    _selectRequest = new RelayCommand(param =>
                    {

                        RequestModel model = param as RequestModel;
                        if (null == model)
                            return;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }

                        if (model.IsLong)
                        {
                            model.IsLong = false;
                        }

                        bool IsSelected = false;
                        if (model.IsSelected)
                            IsSelected = false;
                        else
                            IsSelected = true;

                        List<string> seleced = resultRequestList.Where(x => x.IsSelected).Select(x => x.Request.RequestId.ToString()).ToList();

                        if (seleced.Count >= 3 && IsSelected)
                        {
                            return;
                        }

                        foreach (var item in resultRequestList)
                        {
                            if (model.Request.RequestId == item.Request.RequestId)
                                item.IsSelected = IsSelected;

                        }



                    });
                }
                return _selectRequest;
            }
        }



        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

            IsDisplay = true;
            IsShow = true;
        }



        /// <summary>
        /// 隐藏
        /// </summary>
        internal void Hide()
        {
            IsShow = false;

            new Action(() =>
            {
                System.Threading.Thread.Sleep(Resources.GetRes().AnimateTime);

                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsDisplay = false;


                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));
                }));

            }).BeginInvoke(null, null);

        }





        private bool _isShow = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow
        {
            get { return _isShow; }
            set
            {
                _isShow = value;
                OnPropertyChanged("IsShow");
            }
        }


        

        

        private bool _isDisplay = false;
        /// <summary>
        /// 是否显示动画
        /// </summary>
        public bool IsDisplay
        {
            get { return _isDisplay; }
            set
            {
                _isDisplay = value;
                if (_isDisplay == true)
                    Init();
                OnPropertyChanged("IsDisplay");
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            

        }






        



        private string _oldRoomNo = "";
        /// <summary>
        /// 新雅座编号
        /// </summary>
        public string OldRoomNo
        {
            get { return _oldRoomNo; }
            set
            {
                _oldRoomNo = value;
                OnPropertyChanged("OldRoomNo");
            }
        }



        /// <summary>
        /// 确定按钮
        /// </summary>
        private RelayCommand _okCommand;
        public ICommand OKCommand
        {
            get
            {
                if (_okCommand == null)
                {
                    _okCommand = new RelayCommand(param =>
                    {
                        List<string> seleced = resultRequestList.Where(x => x.IsSelected).Select(x=>x.Request.RequestId.ToString()).ToList();

                        if (seleced.Count > 0)
                        {
                            this.model.OrderDetail.Request = string.Join(",", seleced);
                            this.model.IsRequest = false;
                        }
                        else
                        {
                            this.model.OrderDetail.Request = null;
                            this.model.IsRequest = false;
                        }

                        this.Hide();
                        
                    });
                }
                return _okCommand;
            }
        }


        /// <summary>
        /// 删除按钮
        /// </summary>
        private RelayCommand _cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                {
                    _cancelCommand = new RelayCommand(param =>
                    {
                        this.model.OrderDetail.Request = null;
                        this.model.IsRequest = false;

                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }







    }
}
