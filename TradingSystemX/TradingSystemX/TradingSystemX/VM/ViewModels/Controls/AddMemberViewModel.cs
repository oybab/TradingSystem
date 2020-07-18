using Oybab.DAL;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ModelsForViews;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Oybab.Res.Server.Model;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.Res.Server;
using Oybab.Res.Exceptions;
using Oybab.Res.Tools;

namespace Oybab.TradingSystemX.VM.ViewModels.Controls
{
    internal sealed class AddMemberViewModel : ViewModelBase
    {
        private Xamarin.Forms.View _element;
        private bool IsMember = false;
        private List<long> Ids = new List<long>();
        private bool IsScan = false;
        private Action<object> ReturnValue = null;



        internal AddMemberViewModel(Xamarin.Forms.View element, Action<object> ReturnValue)
        {
            this._element = element;

            this.ReturnValue = ReturnValue;

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
                this.Title = Resources.Instance.GetString("AddMember");
                MemberNo = Resources.Instance.GetString("MemberNo");
            }
            else
            {
                this.Title = Resources.Instance.GetString("AddSupplier");
                MemberNo = Resources.Instance.GetString("SupplierNo");
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
            IsLoading = true;

            Task.Run(async () =>
            {

                await ExtX.WaitForLoading();
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    // 关闭面板
                    NavigationPath.Instance.ClosePanels(false);

                    this.IsShow = false;

                    IsLoading = false;

                });
            });

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



        private bool _isLoading = false;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged("IsLoading");
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
        public Command OKCommand
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
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("LoginValid"), MemberNo), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                        }
                        else
                        {
                            string memberNo = null;
                            if (!string.IsNullOrWhiteSpace(MemberNoValue.Trim()))
                                memberNo = MemberNoValue.Trim();

                            IsLoading = true;

                            Task.Run(async () =>
                            {

                                await ExtX.WaitForLoading();
                                try
                                {

                                    if (IsMember)
                                    {
                                        List<Member> Members;
                                        var taskResult = await OperatesService.Instance.ServiceGetMembers(0, memberNo, cardNo, null, null, true);
                                        bool result = taskResult.result;
                                        Members = taskResult.members;

                                        //如果验证成功
                                        //修改成功
                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                        {
                                            if (result && Members.Count > 0)
                                            {
                                                // 检查下会员是否到期先
                                                if (Members.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Members.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.Now)
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("MemberExpired"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                                }
                                                else if (Members.FirstOrDefault().IsEnable == 0)
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("Exception_MemberDisabled"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                                }
                                                else if (Ids.Contains(Members.FirstOrDefault().MemberId))
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("PropertyExists"), Resources.Instance.GetString("Member")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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
                                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("PropertyNotFound"), Resources.Instance.GetString("MemberNo")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                            }
                                        });
                                    }
                                    else
                                    {
                                        List<Supplier> Suppliers = null;
                                        var taskResult = await OperatesService.Instance.ServiceGetSupplier(0, memberNo, cardNo, null, null, true);
                                        bool result = taskResult.result;
                                        Suppliers = taskResult.suppliers;

                                        //如果验证成功
                                        //修改成功
                                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                        {
                                            if (result && Suppliers.Count > 0)
                                            {
                                                // 检查下会员是否到期先
                                                if (Suppliers.FirstOrDefault().ExpiredTime != 0 && DateTime.ParseExact(Suppliers.FirstOrDefault().ExpiredTime.ToString(), "yyyyMMddHHmmss", null) < DateTime.Now)
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("SupplierExpired"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                                }
                                                else if (Suppliers.FirstOrDefault().IsEnable == 0)
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, Resources.Instance.GetString("Exception_SupplierDisabled"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                                }
                                                else if (Ids.Contains(Suppliers.FirstOrDefault().SupplierId))
                                                {
                                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("PropertyExists"), Resources.Instance.GetString("Supplier")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
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
                                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("PropertyNotFound"), Resources.Instance.GetString("SupplierNo")), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                            }
                                        });
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                    {
                                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                                        {
                                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                                        }));
                                    });
                                }

                                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                                {
                                    IsLoading = false;
                                });


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
        public Command CancelCommand
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
