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
    internal sealed class NewMemberViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;
        private Member member;


        internal NewMemberViewModel(Xamarin.Forms.Page element)
        {
            this._element = element;

            Notification.Instance.NotificationLanguage += (obj, value, args) => { Xamarin.Forms.Device.BeginInvokeOnMainThread(() => { InitialLanguage(); }); };

            InitialLanguage();
        }

        /// <summary>
        /// 初始化语言
        /// </summary>
        private void InitialLanguage()
        {
            PersonName0Label = string.Format("{0}-{1}", Resources.Instance.GetString("MemberName"), Res.Instance.GetMainLangByMainLangIndex(0).LangName);
            PersonName1Label = string.Format("{0}-{1}", Resources.Instance.GetString("MemberName"), Res.Instance.GetMainLangByMainLangIndex(1).LangName);
            PersonName2Label = string.Format("{0}-{1}", Resources.Instance.GetString("MemberName"), Res.Instance.GetMainLangByMainLangIndex(2).LangName);
        }



        /// <summary>
        /// 初始化雅座
        /// </summary>
        internal void Initial(Member member)
        {
            this.member = member;
            ResetAll();
            ShowInfoByLang();
            MemberNo = member.MemberNo;
            if (!string.IsNullOrEmpty(member.Phone))
            PhoneNo = member.Phone;

            SexList.Add(new Dict() { Name = Resources.Instance.GetString("Male"), Value = "1" });
            SexList.Add(new Dict() { Name = Resources.Instance.GetString("Female"), Value = "0" });

            SelectedSex = SexList.FirstOrDefault(x => int.Parse(x.Value.ToString()) == member.Sex);
            if (null == SelectedSex)
                SelectedSex = SexList.FirstOrDefault();
        }


        /// <summary>
        /// 根据语言的改动显示语言
        /// </summary>
        internal void ShowInfoByLang()
        {

            if (!MultiLanguage)
            {
                LangMode0 = false;
                LangMode1 = false;
                LangMode2 = false;

                int index = 0;
                if (Res.Instance.MainLangIndex == 0)
                {
                    index = 0;
                    LangMode0 = true;
                }
                else if (Res.Instance.MainLangIndex == 1)
                {
                    index = 1;
                    LangMode1 = true;
                }
                else if (Res.Instance.MainLangIndex == 2)
                {
                    index = 2;
                    LangMode2 = true;
                }

                LangMode = index;
            }
            else
            {
                LangMode0 = true;
                LangMode1 = true;
                LangMode2 = true;

                LangMode = 9;
            }




          
        }


        /// <summary>
        /// 初始化
        /// </summary>
        internal void ResetAll()
        {
            MemberNo = "";
            PhoneNo = "";
            PersonName0 = "";
            PersonName1 = "";
            PersonName2 = "";

            SexList.Clear();
            SelectedSex = null;


            MultiLanguage = false;
        }

        private ObservableCollection<Dict> _sexList = new ObservableCollection<Dict>();
        /// <summary>
        /// 性别列表
        /// </summary>
        public ObservableCollection<Dict> SexList
        {
            get { return _sexList; }
            set
            {
                _sexList = value;
                OnPropertyChanged("SexList");
            }
        }


        private Dict _selectedSex = null;
        /// <summary>
        /// 选中的性别
        /// </summary>
        public Dict SelectedSex
        {
            get { return _selectedSex; }
            set
            {
                _selectedSex = value;
                OnPropertyChanged("SelectedSex");
            }
        }


        private bool _multiLanguage = false;
        /// <summary>
        /// 多语言
        /// </summary>
        public bool MultiLanguage
        {
            get { return _multiLanguage; }
            set
            {
                _multiLanguage = value;
                OnPropertyChanged("MultiLanguage");
                ShowInfoByLang();
            }
        }


        private string _memberNo = "";
        /// <summary>
        /// 会员号
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

        private string _phoneNo = "";
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNo
        {
            get { return _phoneNo; }
            set
            {
                _phoneNo = value;
                OnPropertyChanged("PhoneNo");
            }
        }





        private string _personName0Label = "";
        /// <summary>
        /// 姓名(汉)
        /// </summary>
        public string PersonName0Label
        {
            get { return _personName0Label; }
            set
            {
                _personName0Label = value;
                OnPropertyChanged("PersonName0Label");
            }
        }


        private string _personName0 = "";
        /// <summary>
        /// 姓名(汉)
        /// </summary>
        public string PersonName0
        {
            get { return _personName0; }
            set
            {
                _personName0 = value;
                OnPropertyChanged("PersonName0");
            }
        }






        private string _personName1Label = "";
        /// <summary>
        /// 姓名(维)
        /// </summary>
        public string PersonName1Label
        {
            get { return _personName1Label; }
            set
            {
                _personName1Label = value;
                OnPropertyChanged("PersonName1Label");
            }
        }


        private string _personName1 = "";
        /// <summary>
        /// 姓名(汉)
        /// </summary>
        public string PersonName1
        {
            get { return _personName1; }
            set
            {
                _personName1 = value;
                OnPropertyChanged("PersonName1");
            }
        }








        private string _personName2Label = "";
        /// <summary>
        /// 姓名(英)
        /// </summary>
        public string PersonName2Label
        {
            get { return _personName2Label; }
            set
            {
                _personName2Label = value;
                OnPropertyChanged("PersonName2Label");
            }
        }


        private string _personName2 = "";
        /// <summary>
        /// 姓名(汉)
        /// </summary>
        public string PersonName2
        {
            get { return _personName2; }
            set
            {
                _personName2 = value;
                OnPropertyChanged("PersonName2");
            }
        }



        private int _langMode = -1;
        /// <summary>
        /// 语言模式
        /// </summary>
        public int LangMode
        {
            get { return _langMode; }
            set
            {
                _langMode = value;
                OnPropertyChanged("LangMode");
            }
        }


        private bool _langMode0 = false;
        /// <summary>
        /// 语言(汉)
        /// </summary>
        public bool LangMode0
        {
            get { return _langMode0; }
            set
            {
                _langMode0 = value;
                OnPropertyChanged("LangMode0");
            }
        }



        private bool _langMode1 = false;
        /// <summary>
        /// 语言(维)
        /// </summary>
        public bool LangMode1
        {
            get { return _langMode1; }
            set
            {
                _langMode1 = value;
                OnPropertyChanged("LangMode1");
            }
        }



        private bool _langMode2 = false;
        /// <summary>
        /// 语言(英)
        /// </summary>
        public bool LangMode2
        {
            get { return _langMode2; }
            set
            {
                _langMode2 = value;
                OnPropertyChanged("LangMode2");
            }
        }






        /// <summary>
        /// 保存
        /// </summary>
        private RelayCommand _changeCommand;
        public Command ChangeCommand
        {
            get
            {
                return _changeCommand ?? (_changeCommand = new RelayCommand(param =>
                {

                    if (!MultiLanguage)
                    {
                        string PersonName0Str = PersonName0.Trim();
                        string PersonName1Str = PersonName1.Trim();
                        string PersonName2Str = PersonName2.Trim();

                        Common.Instance.CopyForHide(ref PersonName0Str, ref PersonName1Str, ref PersonName2Str);

                        PersonName0 = PersonName0Str;
                        PersonName1 = PersonName1Str;
                        PersonName2 = PersonName2Str;
                    }

                    if (PersonName0.Trim() == ""  || PhoneNo.Trim() == "")
                        return;


                    try
                    {
                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, string.Format(Resources.Instance.GetString("ConfirmOperate"), Resources.Instance.GetString("Save")), MessageBoxMode.Dialog, MessageBoxImageMode.Question, MessageBoxButtonMode.YesNo, async (string msg) =>
                        {

                            if (msg == "NO")
                                return;



                            long selectedSexId = int.Parse(SelectedSex.Value.ToString());
                            member.Sex = selectedSexId;
                            member.Phone = PhoneNo.Trim();

                           

                            member.MemberName0 = GetValueOrNull(PersonName0);
                            member.MemberName1 = GetValueOrNull(PersonName1);
                            member.MemberName2 = GetValueOrNull(PersonName2);


                            IsLoading = true;




                            var taskResult = await OperatesService.Instance.ServiceEditMember(member);

                            if (taskResult.resultModel.Result)
                            {


                                QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), Resources.Instance.GetString("Save")), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, msg2 =>
                                {
                                    BackCommand.Execute(null);
                                }, null);

                            }
                            else
                            {
                                if (taskResult.resultModel.IsDataHasRefrence)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("PropertyUsed"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                }
                                else if (taskResult.resultModel.UpdateModel)
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("PropertyUnSame"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                }
                                else
                                {
                                    QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, Resources.Instance.GetString("SaveFailt"), MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                                }
                            }



                            IsLoading = false;


                        }, null);

                    }
                    catch (Exception ex)
                    {

                        ExceptionPro.ExpLog(ex, new Action<string>((message) =>
                        {
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Error, message, MessageBoxMode.Dialog, MessageBoxImageMode.Error, MessageBoxButtonMode.OK, null, null);
                        }));

                    }

                }));

            }
        }



        private bool _isLoading;
        /// <summary>
        /// 显示正在加载
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
        /// 返回值或空
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string GetValueOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            else
                return value;
        }

        /// <summary>
        /// 返回
        /// </summary>
        private RelayCommand _backCommand;
        public Command BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new RelayCommand(param =>
                {
                    NavigationPath.Instance.GoMasterDetailNavigateBack(true, true);
                }));
            }
        }


    }
}
