using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public class StatisticModel
    {
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();
        public Dictionary<string, Font> Fonts = new Dictionary<string, Font>();
 


        public string Title { get; set; }
        public bool EnableAntialiasing { get; set; }

        public Font Font { get; set; }

    }
}
