using Oybab.Res.Exceptions;
using Oybab.Res.Server;
using Oybab.Res.Server.Model;
using Oybab.TradingSystemX.Server;
using Oybab.TradingSystemX.Tools;
using Oybab.TradingSystemX.VM.Commands;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Oybab.TradingSystemX.VM.Converters;
using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.Pages;
using Oybab.Res.Tools;
using System.Collections.Specialized;
using System.Net.Http;
using Oybab.TradingSystemX.Pages.Themes;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages.Controls
{
    internal sealed class SettingViewModel : ViewModelBase
    {
        private Page _element;
        public SettingViewModel(Page _element)
        {
            this._element = _element;


            foreach (var item in Res.Instance.AllLangList.OrderBy(x => x.Value.LangOrder))
            {
                Dict dict = new Dict() { Name = Res.Instance.GetString("LangName", item.Value.Culture), Value = item.Value.LangIndex };
                AllLang.Add(dict);
            }



            AllMode.Add(new Dict() { Name = "Light",  Value = "0" });
            AllMode.Add(new Dict() { Name = "Dark", Value = "1" });

        }




        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            ServerIP = Resources.Instance.SERVER_ADDRESS;



            SelectedLang = AllLang.Where(x => int.Parse(x.Value.ToString()) == Res.Instance.CurrentLangIndex).FirstOrDefault();
            SelectedMode = AllMode[(int)Theme.Instance.CurrentTheme];

        }



        private string _serverIP = "";
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP
        {
            get { return _serverIP; }
            set
            {
                _serverIP = value;
                OnPropertyChanged("ServerIP");
            }
        }


        private ObservableCollection<Dict> _allLang = new ObservableCollection<Dict>();
        /// <summary>
        /// 所有语言
        /// </summary>
        public ObservableCollection<Dict> AllLang
        {
            get { return _allLang; }
            set
            {
                _allLang = value;
                OnPropertyChanged("AllLang");
            }
        }




        private ObservableCollection<Dict> _allMode = new ObservableCollection<Dict>();
        /// <summary>
        /// 所有风格
        /// </summary>
        public ObservableCollection<Dict> AllMode
        {
            get { return _allMode; }
            set
            {
                _allMode = value;
                OnPropertyChanged("AllMode");
            }
        }


        private Dict _selectedLang = null;
        /// <summary>
        /// 所选语言
        /// </summary>
        public Dict SelectedLang
        {
            get { return _selectedLang; }
            set
            {
                _selectedLang = value;
                OnPropertyChanged("SelectedLang");
            }
        }



        private Dict _selectedMode = null;
        /// <summary>
        /// 所选语言
        /// </summary>
        public Dict SelectedMode
        {
            get { return _selectedMode; }
            set
            {
                _selectedMode = value;
                OnPropertyChanged("SelectedMode");
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
        /// 保存
        /// </summary>
        private RelayCommand _changeCommand;
        public Command ChangeCommand
        {
            get
            {
                return _changeCommand ?? (_changeCommand = new RelayCommand(async param =>
                {

                    try
                    {


                        // 判断输入的地址是否有效
                        if (string.IsNullOrWhiteSpace(ServerIP.Trim()))
                        {
                            string message = string.Format(Resources.Instance.GetString("LoginValid"), Resources.Instance.GetString("ServerIpAddress"));
                            QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Warn, message, MessageBoxMode.Dialog, MessageBoxImageMode.Warn, MessageBoxButtonMode.OK, null, null);
                            return;
                        }

                        bool IsChangeServerIP = false;
                        bool IsChangeLang = false;

                        // 设置地址并改掉
                        if (Resources.Instance.SERVER_ADDRESS != ServerIP.Trim())
                        {
                            Resources.Instance.SERVER_ADDRESS = ServerIP.Trim();
                            IsChangeServerIP = true;
                        }
                        if (Res.Instance.CurrentLangIndex != int.Parse(SelectedLang.Value.ToString()))
                        {
                            IsChangeLang = true;
                        }

                        //Theme.Instance.ChangeTheme((ThemeMode)int.Parse(SelectedMode.Value.ToString()));

                        // 保存设置
                        if (IsChangeLang || IsChangeServerIP)
                            await Common.Instance.SetBak();

                        // 关闭连接(因为IP变了)
                        if (IsChangeServerIP)
                            await Common.Instance.Close();

                        if (IsChangeLang)
                        {
                            LangConverter.Instance.ChangeCulture(int.Parse(SelectedLang.Value.ToString()));
                            // 更改语言

                            int langIndex = int.Parse(SelectedLang.Value.ToString());

                            Notification.Instance.ActionLanguage(null, langIndex, null);
                        }

                        QueueMessageBoxNotification.Instance.ActionMessageBox(null, null, CommandTitles.Instance.Information, string.Format(Resources.Instance.GetString("OperateSuccess"), Resources.Instance.GetString("Change")), MessageBoxMode.Dialog, MessageBoxImageMode.Information, MessageBoxButtonMode.OK, null, null);


                        NavigationPath.Instance.GoNavigateBack();
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
                        NavigationPath.Instance.GoNavigateBack();
                }));
            }
        }




    }
}
