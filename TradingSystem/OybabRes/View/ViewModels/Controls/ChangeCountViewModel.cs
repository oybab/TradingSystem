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
using System.Text.RegularExpressions;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class ChangeCountViewModel : ViewModelBase
    {
        private UIElement _element;
        private DetailsModel detailsModel;
        private Action Recalc;


        internal ChangeCountViewModel(UIElement element, Action Recalc)
        {
            this._element = element;
            this.Recalc = Recalc;
            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(DetailsModel detailsModel)
        {
            this.detailsModel = detailsModel;
        }



        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.NewCount.Length < 10)
            {
                if (this.NewCount == "0" && no != ".")
                    NewCount = no;
                else
                    this.NewCount += no;

                ChangeCount();
            }
        }

        private int _displayMode;
        /// <summary>
        /// 显示模式(1服务器IP)
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


        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.NewCount.Length > 0)
            {
                if (IsAll)
                    this.NewCount = "0";
                else
                    this.NewCount = this.NewCount.Remove(this.NewCount.Length - 1);

                if (NewCount == "")
                    NewCount = "0";

                ChangeCount();
            }
        }

        /// <summary>
        /// 修改数量
        /// </summary>
        public void ChangeCount()
        {
            if (this.NewCount == "")
                this.NewCount = "0";
            else
            {
                double count = 0;
                if (!double.TryParse(NewCount, out count))
                {
                    this.NewCount = "0";
                }

                if (count > 9999)
                {
                    count = 9999;
                    this.NewCount = "9999";
                }
                    

                if (!NewCount.EndsWith("."))
                    this.NewCount = Math.Round(count, 3).ToString("0.###");
            }
            
        }




        /// <summary>
        /// 命令输入
        /// </summary>
        /// <param name="no"></param>
        private void SetCommand(string no)
        {
            // 确定
            if (no == "OK")
            {
                this.KeyboardLittle.IsDisplayKeyboard = false;
                if (this.IsDisplay)
                    this.DisplayMode = 0;
                ClearFocus();
            }
            // 取消
            else if (no == "Cancel")
            {
                RemoveText(true);
            }
            // 删除
            else if (no == "Del")
            {
                RemoveText(false);
            }
        }


        private KeyboardLittleViewModel _keyboardLittle;
        /// <summary>
        /// 小键盘
        /// </summary>
        public KeyboardLittleViewModel KeyboardLittle
        {
            get { return _keyboardLittle; }
            set
            {
                _keyboardLittle = value;
                OnPropertyChanged("KeyboardLittle");
            }
        }


        /// <summary>
        /// 去掉焦点
        /// </summary>
        private void ClearFocus()
        {
            var scope = FocusManager.GetFocusScope(_element); // elem is the UIElement to unfocus
            FocusManager.SetFocusedElement(scope, null); // remove logical focus
            Keyboard.ClearFocus(); // remove keyboard focus
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

            OldCount = detailsModel.OrderDetail.Count.ToString();

            NewCount = detailsModel.OrderDetail.Count.ToString();
        }










        private string _oldCount = "0";
        /// <summary>
        /// 老数量
        /// </summary>
        public string OldCount
        {
            get { return _oldCount; }
            set
            {
                _oldCount = value;
                OnPropertyChanged("OldCount");
            }
        }



        private string _newCount = "0";
        /// <summary>
        /// 新数量
        /// </summary>
        public string NewCount
        {
            get { return _newCount; }
            set
            {
                _newCount = value;
                OnPropertyChanged("NewCount");
            }
        }


        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,3})?$");
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
                        if (NewCount == "")
                        {
                            NewCount = "0";
                            return;
                        }
                        if (!match.IsMatch(NewCount))
                        {
                            NewCount = OldCount;
                            return;
                        }


                        if (OldCount != NewCount)
                        {
                            detailsModel.Count = double.Parse(NewCount);
                        }
                        else
                        {
                            detailsModel.Count = double.Parse(NewCount);
                        }

                        if (null != Recalc)
                            Recalc();

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
                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }


    }
}
