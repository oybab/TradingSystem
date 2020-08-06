using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public class StatisticModel : ReportModel
    {

        public string Title { get; set; }
        public bool EnableAntialiasing { get; set; }

        public Font Font { get; set; }

    }
}
