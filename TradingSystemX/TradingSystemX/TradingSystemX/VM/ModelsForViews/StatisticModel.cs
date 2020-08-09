using Oybab.Report.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oybab.TradingSystemX.VM.ModelsForViews
{
    public sealed class StatisticModel 
    {
        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
        public object DataSource { get; set; }
        public object DetailReport { get; set; }
        public object DetailReport2 { get; set; }


        public int PageHeight { get; set; }
        public string Title { get; set; }
        public bool EnableAntialiasing { get; set; }

        public Font Font { get; set; }
    }
}
