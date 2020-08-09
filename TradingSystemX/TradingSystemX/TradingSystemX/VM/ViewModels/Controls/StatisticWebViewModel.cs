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
using Newtonsoft.Json;
using Oybab.ServerManager.Model.Models;
using Oybab.Report.Model;
using Oybab.TradingSystemX.Report.StatisticsHWP;

namespace Oybab.TradingSystemX.VM.ViewModels.Pages.Controls
{
    internal sealed class StatisticWebViewModel : ViewModelBase
    {
        private Page _element;
        private WebView _browser;
        public StatisticWebViewModel(Page _element)
        {
            this._element = _element;
        }

        public void InitialComponent(WebView _browser)
        {
            this._browser = _browser;
        }


        internal void Initial(List<RecordChart> records, StatisticModel statisticModel)
        {

            statisticModel.DataSource = records;
            ChartReport report = new ChartReport(statisticModel.Parameters["PriceSymbol"].ToString());

            string htmlContent = report.ProcessHTMLContent(statisticModel);

            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = htmlContent;

            _browser.Source = htmlSource;
        }




        internal void Initial(List<RecordProducts> records, StatisticModel statisticModel)
        {

            statisticModel.DataSource = records;


            ProductReport report = new ProductReport(statisticModel.Parameters["PriceSymbol"].ToString());


            string htmlContent = report.ProcessHTMLContent(statisticModel);

            var htmlSource = new HtmlWebViewSource();
            htmlSource.Html = htmlContent;

            _browser.Source = htmlSource;
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


    }
}
