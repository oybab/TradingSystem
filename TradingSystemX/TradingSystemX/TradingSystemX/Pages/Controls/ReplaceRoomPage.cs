using Oybab.TradingSystemX.VM.ModelsForViews;
using Oybab.TradingSystemX.VM.ViewModels.Controls;
using Oybab.TradingSystemX.VM.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class ReplaceRoomPage : ContentPage
    {
        private ReplaceRoomViewModel viewModel;

        public ReplaceRoomPage()
		{
            InitializeComponent();


            viewModel = new ReplaceRoomViewModel(this, GetListContent(), GetListTemplate());
            
            this.BindingContext = viewModel;
            

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="model"></param>
        internal void Init(RoomStateModel model)
        {
            viewModel.InitialRooms(model);
        }


        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        private ControlTemplate GetListTemplate()
        {
            return this.Resources["ReplaceListTemplate"] as ControlTemplate;
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
