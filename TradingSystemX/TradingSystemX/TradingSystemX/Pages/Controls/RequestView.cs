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
	public partial class RequestView : ContentView
    {

        public RequestView()
		{
            InitializeComponent();

        }

       
        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        internal ControlTemplate GetListTemplate()
        {
            return this.Resources["RequestListTemplate"] as ControlTemplate;
        }

        /// <summary>
        /// 返回产品容器
        /// </summary>
        /// <returns></returns>
        internal StackLayout GetListContent()
        {
            return lvList;
        }
    }
}
