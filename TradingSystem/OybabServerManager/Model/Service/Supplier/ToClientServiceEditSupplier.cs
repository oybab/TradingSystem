using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Oybab.ServerManager.Model.Service.Supplier
{

    public class ToClientServiceEditSupplier : ToClientService
    {

        public bool Result { get; set; }

        public bool IsSupplierExists { get; set; }

        public bool IsCardExists { get; set; }

        public string Supplier { get; set; }
    }
}
