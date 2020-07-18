using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using Oybab.Res.View.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.ViewModels.Controls
{
    public sealed class LanguageViewModel : ViewModelBase
    {
        private Action<int> Command;
        private UIElement _element;
        private bool IsAllLanguage;

        internal LanguageViewModel(UIElement element, Action<int> Command, bool IsAllLanguage = false)
        {
            this._element = element;
            this.Command = Command;
            this.IsAllLanguage = IsAllLanguage;


            if (IsAllLanguage)
            {
                LanguageList.Clear();

                foreach (var item in Resources.GetRes().AllLangList)
                {
                    LanguageList.Add(new LangItemModel() { LanguageMode = item.Value.LangIndex, LanguageName = item.Value.LangName, Command = this.Command });
                }

            }
            else
            {

                LanguageList.Clear();

                foreach (var item in Resources.GetRes().MainLangList)
                {
                    LanguageList.Add(new LangItemModel() { LanguageMode = item.Value.LangIndex, LanguageName = item.Value.LangName, Command = this.Command });
                }
            }
        }

        /// <summary>
        /// 更改选中
        /// </summary>
        private void ChangeSelected()
        {
            foreach (var item in LanguageList)
            {
                if (this.LanguageMode == item.LanguageMode)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            }
        }


        

        private ObservableCollection<LangItemModel> _languageList = new ObservableCollection<LangItemModel>();
        /// <summary>
        /// 当前已选列表
        /// </summary>
        public ObservableCollection<LangItemModel> LanguageList
        {
            get { return _languageList; }
            set
            {
                _languageList = value;
                OnPropertyChanged("LanguageList");
            }
        }


        /// <summary>
        /// 显示
        /// </summary>
        internal void Show()
        {
            _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOn));

            IsLanguageDisplay = true;
            IsShow = true;
        }



        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide(Action operate)
        {
            IsShow = false;

            new Action(() =>
            {
                System.Threading.Thread.Sleep(Resources.GetRes().AnimateTime);

                _element.Dispatcher.BeginInvoke(new Action(() =>
                {
                    IsLanguageDisplay = false;


                    _element.RaiseEvent(new PopupRoutedEventArgs(PublicEvents.PopupEvent, null, null, null, PopupType.LockOff));

                    if (null != operate)
                        operate();

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



        private bool _isLanguageDisplay = false;
        /// <summary>
        /// 是否显示语言选框
        /// </summary>
        public bool IsLanguageDisplay
        {
            get { return _isLanguageDisplay; }
            set
            {
                _isLanguageDisplay = value;
                OnPropertyChanged("IsLanguageDisplay");
            }
        }

        /// <summary>
        /// 选择语言
        /// </summary>
        private RelayCommand _changeLang;
        public ICommand ChangeLang
        {
            get
            {
                if (_changeLang == null)
                {
                    _changeLang = new RelayCommand(param =>
                        {
                            Command(int.Parse(param.ToString()));
                        });
                }
                return _changeLang;
            }
        }




       

        private int _languageMode = -1;
        /// <summary>
        /// 选择模式 账单的语言 LangIndex
        /// </summary>
        public int LanguageMode
        {
            get { return _languageMode; }
            set
            {
                _languageMode = value;
                OnPropertyChanged("LanguageMode");
                OnPropertyChanged("LanguageName");
                ChangeSelected();
            }
        }


        private string _languageName = "";
        /// <summary>
        /// 选择名
        /// </summary>
        public string LanguageName
        {
            get
            {
                if (IsAllLanguage)
                    return Resources.GetRes().GetLangByLangIndex(_languageMode).LangName;

                else
                    return Resources.GetRes().GetMainLangByLangIndex(_languageMode).LangName;
            }
            set
            {
                _languageName = value;
                OnPropertyChanged("LanguageName");
            }
        }



    }
}
