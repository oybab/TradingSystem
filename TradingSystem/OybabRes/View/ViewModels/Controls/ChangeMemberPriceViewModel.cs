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
    public sealed class ChangeMemberPriceViewModel : ViewModelBase
    {
        private UIElement _element;
        private Member member;
        private Action<Member> Recalc;


        internal ChangeMemberPriceViewModel(UIElement element, Action<Member> Recalc)
        {
            this._element = element;
            this._keyboardLittle = new KeyboardLittleViewModel(SetText, SetCommand);
            this.Recalc = Recalc;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init(Member member)
        {
            this.member = member;
        }



        /// <summary>
        /// 数字输入
        /// </summary>
        /// <param name="no"></param>
        private void SetText(string no)
        {
            if (this.IsDisplay && this.DisplayMode == 1 && this.NewPrice.Length < 10)
            {
                if (this.NewPrice == "0" && no != ".")
                    NewPrice = no;
                else
                    this.NewPrice += no;

                ChangePrice();
            }
        }

        private int _displayMode;
        /// <summary>
        /// 显示模式(1价格)
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
            if (this.IsDisplay && this.DisplayMode == 1 && this.NewPrice.Length > 0)
            {
                if (IsAll)
                    this.NewPrice = "0";
                else
                    this.NewPrice = this.NewPrice.Remove(this.NewPrice.Length - 1);

                if (NewPrice == "")
                    NewPrice = "0";

                ChangePrice();
            }
        }


        /// <summary>
        /// 修改价格
        /// </summary>
        public void ChangePrice()
        {
            if (this.NewPrice == "")
                this.NewPrice = "0";
            else
            {
                double price = 0;
                if (!double.TryParse(NewPrice, out price))
                {
                    this.NewPrice = "0";
                }

                if (!NewPrice.EndsWith("."))
                    this.NewPrice = Math.Round(price, 2).ToString();
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

            OldPrice = member.BalancePrice.ToString();
            
            NewPrice = "0";
        }










        private string _oldPrice = "0";
        /// <summary>
        /// 老价格
        /// </summary>
        public string OldPrice
        {
            get { return _oldPrice; }
            set
            {
                _oldPrice = value;
                OnPropertyChanged("OldPrice");
            }
        }



        private string _newPrice = "0";
        /// <summary>
        /// 新价格
        /// </summary>
        public string NewPrice
        {
            get { return _newPrice; }
            set
            {
                _newPrice = value;
                OnPropertyChanged("NewPrice");
            }
        }

        private bool _isPayByCard = false;
        /// <summary>
        /// 刷卡支付
        /// </summary>
        public bool IsPayByCard
        {
            get { return _isPayByCard; }
            set
            {
                _isPayByCard = value;
                OnPropertyChanged("IsPayByCard");
            }
        }

        


        Regex match = new Regex(@"^[0-9]\d*(\.\d{0,2})?$");
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
                        if (NewPrice == "")
                        {
                            NewPrice = "0";
                            return;
                        }
                        if (!match.IsMatch(NewPrice))
                        {
                            NewPrice = "0";
                            return;
                        }

                        AddMemberPay();

                    });
                }
                return _okCommand;
            }
        }


        /// <summary>
        /// 增加会员支付
        /// </summary>
        private void AddMemberPay()
        {
            MemberPay memberpay = new MemberPay();

            memberpay.MemberPayId = -1;
            memberpay.Price = Math.Round(double.Parse(NewPrice), 2);
            memberpay.MemberId = this.member.MemberId;



            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOn));

            Task.Factory.StartNew(() =>
            {
                ResultModel result = new ResultModel();
                double originalBalancePrice = member.BalancePrice;
                try
                {
                    member.BalancePrice = member.BalancePrice + memberpay.Price;

                    Member newMember;
                    MemberPay newMemberPay;
                    result = OperatesService.GetOperates().ServiceAddMemberPay(member, memberpay, out newMember, out newMemberPay);
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (result.Result)
                        {
                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveSuccess"), null, PopupType.Information));

                            this.member = newMember;

                            if (null != Recalc)
                                Recalc(this.member);

                            this.Hide();
                        }
                        else
                        {
                            if (result.IsDataHasRefrence)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PropertyUsed"), null, PopupType.Warn));
                            }
                            else if (result.UpdateModel)
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("PropertyUsed"), null, PopupType.Warn));
                            }
                            else
                            {
                                _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, Resources.GetRes().GetString("SaveFailt"), null, PopupType.Warn));

                            }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    _element.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            if (result.Result)
                                member.BalancePrice = originalBalancePrice;

                            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, message, null, PopupType.Error));

                        }), false, Resources.GetRes().GetString("SaveFailt"));
                    }));
                }
                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.AnimationOff));
                }));

            });
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
