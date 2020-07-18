using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oybab.Report.Model
{
    public sealed class RecordTime
    {
        long id;
        double price;
        DateTime time;
        public RecordTime(long id, DateTime time, double price)
        {
            this.id = id;
            this.time = time;
            this.price = price;
        }
        public long ID
        {
            get { return id; }
            set { id = value; }
        }
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
        public double Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
