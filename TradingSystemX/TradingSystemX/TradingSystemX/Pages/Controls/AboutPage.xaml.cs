using Oybab.TradingSystemX.VM.ViewModels.Pages;
using Oybab.TradingSystemX.VM.ViewModels.Pages.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class AboutPage : ContentPage
    {
        private AboutViewModel viewModel;

        public AboutPage()
		{
            viewModel = new AboutViewModel(this);
            
            this.BindingContext = viewModel;

            InitializeComponent ();

            // iOS暂时不显示捐助, Apple要求使用内置购买功能.....
            if (Device.RuntimePlatform == Device.iOS)
            {
                columnDonate.Width = new GridLength(0);
            }

            lbSoftware.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {

                    if (DisableCount >= 100 || (DisableCount >= 3 && Oybab.TradingSystemX.Resources.Instance.ExtendInfo.SliderMode != "2"))
                    {
                        viewModel.DisableVisibleMode = true;
                    }
                    ++DisableCount;
                }
                
                )
            });

            lbAdmin.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (DisableAlertCount >= 2)
                    {
                        viewModel.DisableFireMode = true;
                    }
                    ++DisableAlertCount;
                })
            });

        }

        private int DisableAlertCount = 0;
        private int DisableCount = 0;
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            viewModel.Init();
            DisableCount = 0;
            DisableAlertCount = 0;
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/tradingsystem/company/" + Res.Instance.MainLang.Culture.Name));
        }

        private async void TapGestureRecognizer_Tapped_1(object sender, EventArgs e)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/tradingsystem/license/" + Res.Instance.MainLang.Culture.Name));

        }

        private async void TapGestureRecognizer_Tapped_2(object sender, EventArgs e)
        {
            var message = new EmailMessage
            {
                Subject = "",
                Body = "",
                To = new List<string>() { "service@oybab.net" },
                //Cc = ccRecipients,
                //Bcc = bccRecipients
            };
            await Email.ComposeAsync(message);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/tradingsystem/sourcecode/" + Res.Instance.MainLang.Culture.Name));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Xamarin.Essentials.Launcher.OpenAsync(new System.Uri("https://oybab.net/tradingsystem/donate/" + Res.Instance.MainLang.Culture.Name));
        }
    }
}
