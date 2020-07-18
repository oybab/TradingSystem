using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Oybab.TradingSystemX.Pages.Controls
{
	public partial class ProductsPage : ContentPage
	{
		public ProductsPage()
		{
            InitializeComponent ();
		}

        /// <summary>
        /// 返回产品类型容器
        /// </summary>
        /// <returns></returns>
        internal StackLayout GetProductTypeListContent()
        {
            return spProductTypeList;
        }

        /// <summary>
        /// 返回产品模板
        /// </summary>
        /// <returns></returns>
        internal ControlTemplate GetProductTemplate()
        {
            return this.Resources["ProductListTemplate"] as ControlTemplate;
        }

        /// <summary>
        /// 返回产品容器
        /// </summary>
        /// <returns></returns>
        internal StackLayout GetProductContent() {
            return lvList;
        }

        

    }
}
