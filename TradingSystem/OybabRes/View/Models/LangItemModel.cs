using Oybab.DAL;
using Oybab.Res.Tools;
using Oybab.Res.View.Commands;
using Oybab.Res.View.Enums;
using Oybab.Res.View.EventArgs;
using Oybab.Res.View.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Oybab.Res.View.Models
{
    public sealed class LangItemModel : ViewModels.ViewModelBase
    {
   
        internal Action<int> Command;




        private bool _isSelected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
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
            }
        }


        private string _languageName = "";
        /// <summary>
        /// 选择名
        /// </summary>
        public string LanguageName
        {
            get { return _languageName; }
            set
            {
                _languageName = value;
                OnPropertyChanged("LanguageName");
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
                        LangItemModel model = param as LangItemModel;

                        if (model.IsIgnore)
                        {
                            model.IsIgnore = false;
                            return;
                        }


                        Command(model.LanguageMode);
                    });
                }
                return _changeLang;
            }
        }


    }
}
