using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages
{
	public partial class RoomListPage : ContentPage
    {
        private RoomListViewModel viewModel;

        public RoomListPage()
		{
            InitializeComponent();


            viewModel = new RoomListViewModel(this, GetListContent(), GetListTemplate());
            this.BindingContext = viewModel;
            
        }

       
        /// <summary>
        /// 初始化
        /// </summary>
        internal void Init()
        {
            viewModel.Init((long)0);
        }


        /// <summary>
        /// 刷新客桌列表
        /// </summary>
        internal void RefreshRoomList()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                viewModel.RefreshAll(true);
            });
        }



        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        private ControlTemplate GetListTemplate()
        {
            return this.Resources["ListTemplate"] as ControlTemplate;
        }

        /// <summary>
        /// 返回产品容器
        /// </summary>
        /// <returns></returns>
        private StackLayout GetListContent()
        {
            return lvList;
        }
    }
}
