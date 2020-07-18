using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class AddressViewModel : ViewModelBase
    {
        private UIElement _element;
        private SelectedViewModel _selectedViewModel;

        internal AddressViewModel(UIElement element)
        {
            this._element = element;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(SelectedViewModel selectedViewModel)
        {
            this._selectedViewModel = selectedViewModel;
        }

        /// <summary>
        /// 设置显示模式
        /// </summary>
        /// <param name="mode"></param>
        public void SetDisplay(int mode)
        {
            this.DisplayMode = mode;
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


        private int _displayMode;
        /// <summary>
        /// 显示模式(1电话, 2名字, 3地址, 4备注)
        /// </summary>
        public int DisplayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                OnPropertyChanged("DisplayMode");
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
                OnPropertyChanged("IsDisplay");
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        internal void Clear()
        {
            Name = "";

            Address = "";

            Phone = "";

            Remark = "";

            this.DisplayMode = -1;
        }







        private string _remark = "";
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set
            {
                _remark = value;
                OnPropertyChanged("Remark");
            }
        }







        private string _phone = "";
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone
        {
            get { return _phone; }
            set
            {
               
                _phone = value;
                OnPropertyChanged("Phone");
            }
        }

        private string _name = "";
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {

                _name = value;
                OnPropertyChanged("Name");
            }
        }


        private string _address = "";
        /// <summary>
        /// 地址
        /// </summary>
        public string Address
        {
            get { return _address; }
            set
            {

                _address = value;
                OnPropertyChanged("Address");
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

                        if (!string.IsNullOrWhiteSpace(this.Phone) || !string.IsNullOrWhiteSpace(this.Name) || !string.IsNullOrWhiteSpace(this.Address))
                            _selectedViewModel.HasAddress = 2;
                        else
                            _selectedViewModel.HasAddress = 1;

                      
                        this.DisplayMode = -1;
                        this.Hide();

                    });
                }
                return _okCommand;
            }
        }


        /// <summary>
        /// 取消按钮
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
                        Clear();
                        _selectedViewModel.HasAddress = 1;
                        this.DisplayMode = -1;

                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }



    }
}
