using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{

    public sealed class SummaryModelPackage
    {
        public List<SummaryModel> Records { get; set; }
        public List<SummaryModel> Records2 { get; set; }
        public List<SummaryModel> Records3 { get; set; }

        public string Time { get; set; }

        public long Lang { get; set; } = -1;
    }
    public sealed class SummaryModel
    {
        public string Name
        {
            get;
            set;
        }
        //[Newtonsoft.Json.JsonIgnore]
        public string TypeName
        {
            get;
            set;
        }
        public double Income
        {
            get;
            set;
        }
        public double Spend
        {
            get;
            set;
        }

        public double Profit
        {
            get;
            set;
        }
    }
}
