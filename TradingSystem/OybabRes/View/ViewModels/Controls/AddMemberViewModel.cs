using Oybab.DAL;
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
    public sealed class AddMemberViewModel : ViewModelBase
    {
        private UIElement _element;
        private bool IsMember = false;
        private List<long> Ids = new List<long>();
        private bool IsScan = false;
        private Action<object> ReturnValue = null;


        internal AddMemberViewModel(UIElement element, Action<object> ReturnValue)
        {
            this._element = element;
            this.ReturnValue = ReturnValue;
            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
        }



        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.MemberNo.Length < 40)
            {
                this.MemberNo += no;
            }
        }

        /// <summary>
        /// 回车
        /// </summary>
        public void Handle(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_keyboardLittle.IsDisplayKeyboard)
                {
                    _keyboardLittle.OKCommand.Execute(null);
                }
                else
                {
                    // 无法用, 因为已经失去了焦点
                    this.OKCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Escape)
            {
                if (_keyboardLittle.IsDisplayKeyboard)
                {
                    _keyboardLittle.IsDisplayKeyboard = false;
                }
            }
        }



        /// <summary>
        /// 数字移出
        /// </summary>
        private void RemoveText(bool IsAll)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.MemberNo.Length > 0)
            {
                if (IsAll)
                    this.MemberNo = "";
                else
                    this.MemberNo = this.MemberNo.Remove(this.MemberNo.Length - 1);

                if (MemberNo == "")
                    MemberNo = "";

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
        /// 初始化
        /// </summary>
        internal void Init(bool IsMember = true, List<long> Ids = null, bool IsScan = false)
        {
            this.IsMember = IsMember;
            this.Ids = Ids;
            if (null == Ids)
                Ids = new List<long>();
            this.IsScan = IsScan;

            MemberNoValue = "";

            if (!IsScan)
                IsSave = true;
            else
                IsSave = false;



            if (IsMember)
            {
                this.Title = Resources.GetRes().GetString("AddMember");
                MemberNo = Resources.GetRes().GetString("MemberNo");
            }
            else
            {
                this.Title = Resources.GetRes().GetString("AddSupplier");
                MemberNo = Resources.GetRes().GetString("SupplierNo");
            }
        }




        private bool _isSave = false;
        /// <summary>
        /// 会员名称内容显示
        /// </summary>
        public bool IsSave
        {
            get { return _isSave; }
            set
            {
                _isSave = value;
                OnPropertyChanged("IsSave");
            }
        }


        private string cardNo = null;

        internal void SearchByScanner(string cardNo)
        {
            if (null != cardNo)
            {
                this.cardNo = cardNo;
            }
        }




     
        private int _displayMode;
        /// <summary>
        /// 显示模式(1备注)
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
        /// 显示
        /// </summary>
        internal void Show()
        {
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

            IsDisplay = true;
            IsShow = true;

            if (IsScan && null != cardNo)
            {
                OKCommand.Execute(null);
            }
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
                OnPropertyChanged("IsDisplay");
            }
        }


        /// <summary>
        /// 初始化
        /// </summary>
        internal void Clear()
        {

            MemberNo = "";


        }







        private string _memberNo = "";
        /// <summary>
        /// 会员号名
        /// </summary>
        public string MemberNo
        {
            get { return _memberNo; }
            set
            {
                _memberNo = value;
                OnPropertyChanged("MemberNo");
            }
        }


        private string _title = "";
        /// <summary>
        /// 会员号名
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



        private string _memberNoValue = "";
        /// <summary>
        /// 会员号
        /// </summary>
        public string MemberNoValue
        {
            get { return _memberNoValue; }
            set
            {
                _memberNoValue = value;
                OnPropertyChanged("MemberNoValue");
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



                        //先不让用户单击按钮
                        IsSave = false;
                        bool returnResult = false;

                        //判断是否空
                        if (null == cardNo && MemberNoValue.Trim().Equals(""))
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("LoginValid"), MemberNo), null, PopupType.Warn));
                        }
                        else
                        {
                            string memberNo = null;
                            if (!string.IsNullOrWhiteSpace(MemberNoValue.Trim()))
                                memberNo = MemberNoValue.Trim();

                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

                            Task.Factory.StartNew(() =>
                            {
                                try
                                {

                                    if (IsMember)
                                    {
                                        List<Member> Members;
                                        bool result = OperatesService.GetOperates().ServiceGetMembers(0, memberNo, cardNo, null, null, true, out Members);

                                        //如果验证成功
                                        //修改成功
                                        _element.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            if (result && Members.Count > 0)
                                            {
                                                // 检查下会员是否到期先
                                                if (Members.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Members.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.Now)
                                                {
                                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("MemberExpired"), null, PopupType.Warn));
                                                }
                                                else if (Members.FirstOrDefault().IsEnable == 0)
                                                {
                                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("Exception_MemberDisabled"), null, PopupType.Warn));
                                                }
                                                else if (Ids.Contains(Members.FirstOrDefault().MemberId))
                                                {
                                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("Member")), null, PopupType.Warn));
                                                }
                                                else
                                                {
                                                    ReturnValue(Members.FirstOrDefault());
                                                    returnResult = true;
                                                    this.Hide();
                                                }

                                            }
                                            else
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("MemberNo")), null, PopupType.Warn));
                                            }
                                        }));
                                    }
                                    else
                                    {
                                        List<Supplier> Suppliers;
                                        bool result = OperatesService.GetOperates().ServiceGetSupplier(0, memberNo, cardNo, null, null, true, out Suppliers);

                                        //如果验证成功
                                        //修改成功
                                        _element.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            if (result && Suppliers.Count > 0)
                                            {
                                                // 检查下会员是否到期先
                                                if (Suppliers.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Suppliers.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.Now)
                                                {

                                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SupplierExpired"), null, PopupType.Warn));
                                                }
                                                else if (Suppliers.FirstOrDefault().IsEnable == 0)
                                                {
                                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("Exception_SupplierDisabled"), null, PopupType.Warn));
                                                }
                                                else if (Ids.Contains(Suppliers.FirstOrDefault().SupplierId))
                                                {
                                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("PropertyExists"), Resources.GetRes().GetString("Supplier")), null, PopupType.Warn));
                                                }
                                                else
                                                {
                                                    ReturnValue(Suppliers.FirstOrDefault());
                                                    returnResult = true;
                                                    this.Hide();
                                                }

                                            }
                                            else
                                            {
                                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, string.Format(Resources.GetRes().GetString("PropertyNotFound"), Resources.GetRes().GetString("SupplierNo")), null, PopupType.Warn));
                                            }
                                        }));
                                    }

                                }
                                catch (Exception ex)
                                {
                                    _element.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                        {
                                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));
                                        }));
                                    }));
                                }

                                _element.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                                }));


                            });
                        }
                        if (!IsScan)
                            IsSave = true;



                        if (IsScan && returnResult)
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
                       
                        this.Hide();
                    });
                }
                return _cancelCommand;
            }
        }



    }
}
