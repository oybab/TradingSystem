using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public class ReportModel
    {

        public Dictionary<string, object> Parameters = new Dictionary<string, object>();
        public Dictionary<string, Font> Font = new Dictionary<string, Font>();
        public object DataSource { get; set; }
        public object DetailReport { get; set; }
        public object DetailReport2 { get; set; }


        public int PageHeight { get; set; }
        public bool IsEAN13Generator { get; set; }
    }
}
