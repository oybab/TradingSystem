using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class RecordChartStack
    {
        int id;
        double price;
        string name;
        string name2;
        public RecordChartStack(int id, string name, string name2, double price)
        {
            this.id = id;
            this.name = name;
            this.name2 = name2;
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
        public string Name2
        {
            get { return name2; }
            set { name2 = value; }
        }
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
