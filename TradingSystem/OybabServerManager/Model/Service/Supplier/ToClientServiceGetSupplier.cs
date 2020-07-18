using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Supplier
{

    public class ToClientServiceGetSupplier : ToClientService
    {

        public bool Result { get; set; }

        public string Suppliers { get; set; }
    }
}
