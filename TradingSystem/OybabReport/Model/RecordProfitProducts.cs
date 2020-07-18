using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class RecordProfitProducts
    {
        long id;
        double costprice;
        string name;
        double count;
        double price;
        double profitprice;

        public RecordProfitProducts(long id, string name, double costprice, double price, double profitprice, double count)
        {
            this.id = id;
            this.name = name;
            this.count = count;
            this.price = price;
            this.costprice = costprice;
            this.profitprice = profitprice;
        }
        public long ID
        {
            get { return id; }
            set { id = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public double Count
        {
            get { return count; }
            set { count = value; }
        }
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
        public double CostPrice
        {
            get { return costprice; }
            set { costprice = value; }
        }
        public double ProfitPrice
        {
            get { return profitprice; }
            set { profitprice = value; }
        }

    }
}
