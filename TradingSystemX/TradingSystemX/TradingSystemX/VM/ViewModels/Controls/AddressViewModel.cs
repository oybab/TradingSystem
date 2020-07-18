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
    internal sealed class AddressViewModel : ViewModelBase
    {
        private Xamarin.Forms.Page _element;


        internal AddressViewModel(Xamarin.Forms.Page element)
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
            PersonName0Label = string.Format("{0}-{1}", Resources.Instance.GetString("PersonName"), Res.Instance.GetMainLangByMainLangIndex(0).LangName);
            PersonName1Label = string.Format("{0}-{1}", Resources.Instance.GetString("PersonName"), Res.Instance.GetMainLangByMainLangIndex(1).LangName);
            PersonName2Label = string.Format("{0}-{1}", Resources.Instance.GetString("PersonName"), Res.Instance.GetMainLangByMainLangIndex(2).LangName);
            Address0Label = string.Format("{0}-{1}", Resources.Instance.GetString("Address"), Res.Instance.GetMainLangByMainLangIndex(0).LangName);
            Address1Label = string.Format("{0}-{1}", Resources.Instance.GetString("Address"), Res.Instance.GetMainLangByMainLangIndex(1).LangName);
            Address2Label = string.Format("{0}-{1}", Resources.Instance.GetString("Address"), Res.Instance.GetMainLangByMainLangIndex(2).LangName);
        }



        /// <summary>
        /// 初始化雅座
        /// </summary>
        internal void Initial()
        {
            ShowInfoByLang();
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
            Phone = "";
            PersonName0 = "";
            Address0 = "";
            PersonName1 = "";
            Address1 = "";
            PersonName2 = "";
            Address2 = "";

            Remark = "";
            MultiLanguage = false;
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


        private string _address0 = "";
        /// <summary>
        /// 地址(汉)
        /// </summary>
        public string Address0
        {
            get { return _address0; }
            set
            {
                _address0 = value;
                OnPropertyChanged("Address0");
            }
        }


        private string _address0Label = "";
        /// <summary>
        /// 地址(汉)
        /// </summary>
        public string Address0Label
        {
            get { return _address0Label; }
            set
            {
                _address0Label = value;
                OnPropertyChanged("Address0Label");
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



        private string _address1 = "";
        /// <summary>
        /// 地址(维)
        /// </summary>
        public string Address1
        {
            get { return _address1; }
            set
            {
                _address1 = value;
                OnPropertyChanged("Address1");
            }
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



        private string _address1Label = "";
        /// <summary>
        /// 地址(维)
        /// </summary>
        public string Address1Label
        {
            get { return _address1Label; }
            set
            {
                _address1Label = value;
                OnPropertyChanged("Address1Label");
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




        private string _address2 = "";
        /// <summary>
        /// 地址(英)
        /// </summary>
        public string Address2
        {
            get { return _address2; }
            set
            {
                _address2 = value;
                OnPropertyChanged("Address2");
            }
        }



        private string _address2Label = "";
        /// <summary>
        /// 地址(英)
        /// </summary>
        public string Address2Label
        {
            get { return _address2Label; }
            set
            {
                _address2Label = value;
                OnPropertyChanged("Address2Label");
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




    }
}
