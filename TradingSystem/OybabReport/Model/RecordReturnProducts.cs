using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class RecordReturnProducts
    {
        long id;
        double changeCount;
        string name;
        double count;
        double totalPrice;

        public RecordReturnProducts(long id, string name, double changeCount, double count, double totalPrice)
        {
            this.id = id;
            this.name = name;
            this.count = count;
            this.changeCount = changeCount;
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
        public double ChangeCount
        {
            get { return changeCount; }
            set { changeCount = value; }
        }
        public double TotalPrice
        {
            get { return totalPrice; }
            set { totalPrice = value; }
        }
    }
}
