using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class RecordProducts
    {
        long id;
        string price;
        string name;
        double count;
        double totalPrice;

        public RecordProducts(long id, string name, string price, double count, double totalPrice)
        {
            this.id = id;
            this.name = name;
            this.count = count;
            this.price = price;
            this.totalPrice = totalPrice;
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
        public string Price
        {
            get { return price; }
            set { price = value; }
        }
        public double TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
    }
}
