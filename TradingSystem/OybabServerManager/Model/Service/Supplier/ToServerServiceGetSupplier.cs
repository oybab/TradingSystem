using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Supplier
{

    public class ToServerServiceGetSupplier : ToServerService
    {

        public long SupplierId { get; set; }

        public bool SingleSupplierNo { get; set; }

        public string SupplierNo { get; set; }

        public string CardNo { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

    }
}
