using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class ProductLabel
    {
        public int Id { get; set; }
        public string BarcodeNo { get; set; }
        public string ProductName { get; set; }

        public double Price { get; set; }
    }
}
