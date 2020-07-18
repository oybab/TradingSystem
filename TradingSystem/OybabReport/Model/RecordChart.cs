using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class RecordChart
    {
        int id;
        double price;
        string name;
        public RecordChart(int id, string name, double price)
        {
            this.id = id;
            this.name = name;
            this.price = price;
        }

        
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
